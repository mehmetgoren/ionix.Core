namespace Ionix.Rest
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    /*
     TokenTableAuthAttribute on method is not available. Clients need to group controllers to separate auhorized and unauthhorized methods. Why, bacause this approach is more clean,
     easy to understand and more maintainable.
         */
    public interface IReflectController
    {
        IEnumerable<ControllerActions> FindByAssemblies(IEnumerable<Assembly> assemblies);
    }

    public class ReflectController : IReflectController
    {
        private static readonly Type ControllerType = typeof(ControllerBase);

        private static readonly Type ActionResultType = typeof(IActionResult);


        private static string CreateUniqueName(MethodInfo mi)
        {
            string signatureString = String.Join(",", mi.GetParameters().Select(p => p.ParameterType.Name).ToArray());
            string returnTypeName = mi.ReturnType.Name;

            if (mi.IsGenericMethod)
            {
                string typeParamsString = String.Join(",", mi.GetGenericArguments().Select(g => g.AssemblyQualifiedName).ToArray());


                // returns a string like this: "Assembly.YourSolution.YourProject.YourClass:YourMethod(Param1TypeName,...,ParamNTypeName):ReturnTypeName
                return String.Format("{0}:{1}<{2}>({3}):{4}", mi.DeclaringType.AssemblyQualifiedName, mi.Name, typeParamsString, signatureString, returnTypeName);
            }

            return String.Format("{0}:{1}({2}):{3}", mi.DeclaringType.AssemblyQualifiedName, mi.Name, signatureString, returnTypeName);
        }
        private static readonly HashSet<string> BaseActionsMethods = ControllerType.GetTypeInfo().GetMethods()
            .Where(p => ActionResultType.IsAssignableFrom(p.ReturnType)).Select(CreateUniqueName)
            .ToHashSet();

        private static readonly Type ArgumentlessTaskType = typeof(Task<>);
        protected virtual bool IsAssignableFrom(Type returnType)
        {
            if (ActionResultType.IsAssignableFrom(returnType))
                return true;

            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == ArgumentlessTaskType)
            {
                var kernelReturnType = returnType.GetGenericArguments().FirstOrDefault();
                if (null != kernelReturnType)
                    return this.IsAssignableFrom(kernelReturnType);
            }

            return false;
        }

        protected HashSet<MethodInfo> GetActionMethods(Type controllerType)
        {
            HashSet<MethodInfo> ret = new HashSet<MethodInfo>();
            foreach (MethodInfo mi in controllerType.GetMethods().Where(p => this.IsAssignableFrom(p.ReturnType)))
            {
                if (!BaseActionsMethods.Contains(CreateUniqueName(mi)))
                {
                    ret.Add(mi);
                }
            }

            return ret;
        }

        public IEnumerable<ControllerActions> FindByAssemblies(IEnumerable<Assembly> assemblies)
        {
            List<ControllerActions> ret = new List<ControllerActions>();
            if (!assemblies.IsNullOrEmpty())
            {
                foreach (Assembly asm in assemblies)
                {
                    foreach (Type type in asm.GetTypes())
                    {
                        if (!type.IsAbstract && ControllerType.IsAssignableFrom(type))
                        {
                            TokenTableAuthAttribute aa = type.GetCustomAttribute<TokenTableAuthAttribute>(false);//Eğerki Authorize Attribute u Yok ise Boşuna DB ye eklemesin
                            if (null != aa)
                            {
                                var actionMethods = this.GetActionMethods(type);
                                if (!actionMethods.IsNullOrEmpty())
                                    ret.Add(new ControllerActions(type, actionMethods));
                            }
                        }
                    }
                }
            }

            return ret;
        }
    }

    //Bu Kısım Arayüz Tarafında Map Edilip Delsert işlemi gerçekleşecek. Reflection Helper.
    public sealed class ControllerActions : IEnumerable<MethodInfo>
    {
        public Type ControllerType { get; }

        private readonly Dictionary<string, MethodInfo> actionMethods;//string actionName

        //Default reflection Kullanımu
        public ControllerActions(Type controllerType, IEnumerable<MethodInfo> actionMethods)
            : this(controllerType)
        {
            if (actionMethods.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(actionMethods));

            foreach (MethodInfo item in actionMethods)
            {
                this.AddMethodInfo(item);
            }
        }

        //role için eklendi
        public ControllerActions(Type controllerType)
        {
            if (null == controllerType)
                throw new ArgumentNullException(nameof(controllerType));

            TokenTableAuthAttribute attr = controllerType.GetCustomAttribute<TokenTableAuthAttribute>(false);
            if (null == attr)
                throw  new ArgumentException($"'{controllerType}' is not marked with '{nameof(TokenTableAuthAttribute)}'");

            this.ControllerType = controllerType;

            this.actionMethods = new Dictionary<string, MethodInfo>();
        }

        //role için eklendi
        public void AddMethodInfo(MethodInfo info)
        {
            if (null == info)
                throw new ArgumentNullException(nameof(info));

            this.actionMethods[info.Name] = info;
        }

        public MethodInfo this[string actionName]
        {
            get
            {
                this.actionMethods.TryGetValue(actionName, out MethodInfo action);
                return action;
            }
        }

        public override string ToString()
        {
            return this.ControllerType.FullName;
        }

        public IEnumerator<MethodInfo> GetEnumerator()
        {
            return this.actionMethods.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    //Singleton Controller/Action Collection. can be used both on  mvc and api controllers
    public sealed class ControllerActionsList : IEnumerable<ControllerActions>
    {
        //Bu Kısım Sadece UI Oluşturulurken Kullanılacak.
        public static ControllerActionsList Create<TReflectController>(IEnumerable<Assembly> assemblies)
            where TReflectController : IReflectController, new()
        {
            TReflectController rc = new TReflectController();
            var list = rc.FindByAssemblies(assemblies);

            return new ControllerActionsList(list);
        }


        private readonly Dictionary<string, ControllerActions> dic;//string controllerName
        private ControllerActionsList(IEnumerable<ControllerActions> list)
        {
            this.dic = new Dictionary<string, ControllerActions>();

            if (!list.IsNullOrEmpty())
            {
                foreach (var item in list)
                {
                    this.dic.Add(item.ControllerType.FullName, item);
                }
            }
        }

        public int Count => this.dic.Count;

        public ControllerActions this[string controllerTypeFullName]
        {
            get
            {
                ControllerActions ca;
                this.dic.TryGetValue(controllerTypeFullName, out ca);
                return ca;
            }
        }

        public IEnumerator<ControllerActions> GetEnumerator()
        {
            return this.dic.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
