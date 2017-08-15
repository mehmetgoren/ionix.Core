namespace ionix.Rest
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NotAuthorizeAttribute : Attribute
    {

    }

    public sealed class ActionMethodInfo
    {
        public MethodInfo MethodIndo { get; set; }
        public bool NotAuthorize { get; set; }
    }


    //Bu Kısım Arayüz Tarafında Map Edilip Delsert işlemi gerçekleşecek. Reflection Helper.
    public sealed class ControllerActions : IEnumerable<ActionMethodInfo>
    {
        public Type ControllerType { get; private set; }
        public bool NotAutharize { get; private set; }

        private readonly Dictionary<string, ActionMethodInfo> actionMethods;//string actionName

        //Default reflection Kullanımu
        public ControllerActions(Type controllerType, IEnumerable<MethodInfo> actionMethods)
            : this(controllerType)
        {
            if (actionMethods.IsEmptyList())
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

            this.ControllerType = controllerType;

            NotAuthorizeAttribute naa = controllerType.GetCustomAttribute<NotAuthorizeAttribute>();
            this.NotAutharize = naa != null;

            this.actionMethods = new Dictionary<string, ActionMethodInfo>();
        }

        //role için eklendi
        public void AddMethodInfo(MethodInfo info)
        {
            if (null == info)
                throw new ArgumentNullException(nameof(info));

            NotAuthorizeAttribute naa = info.GetCustomAttribute<NotAuthorizeAttribute>();

            this.actionMethods[info.Name] = new ActionMethodInfo() { MethodIndo = info, NotAuthorize = naa != null };
        }

        public ActionMethodInfo this[string actionName]
        {
            get
            {
                ActionMethodInfo action;
                this.actionMethods.TryGetValue(actionName, out action);
                return action;
            }
        }

        public override string ToString()
        {
            return this.ControllerType.FullName;
        }

        public IEnumerator<ActionMethodInfo> GetEnumerator()
        {
            return this.actionMethods.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public interface IReflectController
    {
        IEnumerable<ControllerActions> FindByAssemblies(IEnumerable<Assembly> assemblies);
    }

    public class ReflectController : IReflectController
    {
        public Type ControllerBaseType { get; } = typeof(ControllerBase);

        public Type ActionBaseType { get; } = typeof(IActionResult);

        private HashSet<MethodInfo> GetActionMethods(Type controllerType)
        {
            HashSet<MethodInfo> ret = new HashSet<MethodInfo>();
            foreach (MethodInfo mi in controllerType.GetMethods())
            {
                if (this.ActionBaseType.IsAssignableFrom(mi.ReturnType))//if method is action method
                {
                    // NotAuthorizeAttribute naa = mi.GetCustomAttribute<NotAuthorizeAttribute>();
                    //  if (naa == null)
                    ret.Add(mi);
                }
            }
            return ret;
        }
        public virtual IEnumerable<ControllerActions> FindByAssemblies(IEnumerable<Assembly> assemblies)
        {
            List<ControllerActions> ret = new List<ControllerActions>();
            if (!assemblies.IsEmptyList())
            {
                foreach (Assembly asm in assemblies)
                {
                    foreach (Type type in asm.GetTypes())
                    {
                        if (!type.IsAbstract && this.ControllerBaseType.IsAssignableFrom(type))
                        {
                            //NotAuthorizeAttribute naa = type.GetCustomAttribute<NotAuthorizeAttribute>();
                            //if (null == naa)
                            AuthorizeAttribute aa = type.GetCustomAttribute<AuthorizeAttribute>(true);//Eğerki Authorize Attribute u Yok ise Boşuna DB ye eklemesin
                            if (null != aa)
                            {
                                var actionMethods = this.GetActionMethods(type);
                                if (!actionMethods.IsEmptyList())
                                    ret.Add(new ControllerActions(type, actionMethods));
                            }
                        }
                    }
                }
            }

            return ret;
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

        public static ControllerActionsList Create<TReflectControllerApi, TReflectControllerMvc>(IEnumerable<Assembly> assemblies)//Clear UnusedRecords da kullanılıyor
            where TReflectControllerApi : IReflectController, new()
            where TReflectControllerMvc : IReflectController, new()
        {
            TReflectControllerApi rc = new TReflectControllerApi();
            var list = rc.FindByAssemblies(assemblies).ToList();

            TReflectControllerMvc mc = new TReflectControllerMvc();
            list.AddRange(mc.FindByAssemblies(assemblies));

            return new ControllerActionsList(list);
        }


        private readonly Dictionary<string, ControllerActions> dic;//string controllerName
        private ControllerActionsList(IEnumerable<ControllerActions> list)
        {
            this.dic = new Dictionary<string, ControllerActions>();

            if (!list.IsEmptyList())
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
