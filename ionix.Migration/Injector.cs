namespace Ionix.Migration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    //polymorphism yerine kullanılması için eklendi ve NigrationBase' den 
    //protected abstract IMigrationBuilder MigrationBuilder { get; } protected abstract IColumnDbTypeResolver ColumnDbTypeResolver { get; }
    //methodları kaldırıldı. Ayrıca DatabaseMigrationStatus' de base class olmadı.
    public static class Injector
    {
        private static readonly IDictionary<Type, Func<object>> registrations = new ConcurrentDictionary<Type, Func<object>>();

        public static void Register<TService, TImpl>() where TImpl : TService
        {
            registrations.Add(typeof(TService), () => GetInstance(typeof(TImpl)));
        }

        public static void Register<TService>(Func<TService> instanceCreator)
        {
            registrations.Add(typeof(TService), () => instanceCreator());
        }

        public static void RegisterSingleton<TService>(TService instance)
        {
            registrations.Add(typeof(TService), () => instance);
        }

        public static void RegisterSingleton<TService>(Func<TService> instanceCreator)
        {
            var lazy = new Lazy<TService>(instanceCreator);
            Register<TService>(() => lazy.Value);
        }

        public static object GetInstance(Type serviceType)
        {
            Func<object> creator;
            if (registrations.TryGetValue(serviceType, out creator)) return creator();
            else if (!serviceType.IsAbstract) return CreateInstance(serviceType);
            else throw new InvalidOperationException("No registration for " + serviceType);
        }

        public static T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        private static object CreateInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().Single();
            var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType);
            var dependencies = parameterTypes.Select(t => GetInstance(t)).ToArray();
            return Activator.CreateInstance(implementationType, dependencies);
        }
    }
}
