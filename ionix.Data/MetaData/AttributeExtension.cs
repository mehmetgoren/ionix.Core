namespace Ionix.Data
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;
    using Utils.Extensions;

    public static class AttributeExtension
    {
        public static string GetTableName(Type entityType)
        {
            if (null == entityType)
                throw new ArgumentNullException(nameof(entityType));

            TableAttribute ta = entityType.GetTypeInfo().GetCustomAttribute<TableAttribute>();
            if (null != ta)
                return String.IsNullOrEmpty(ta.Schema) ? ta.Name : ta.Schema + '.' + ta.Name;

            return entityType.FullName; //schema, user name gibi yapıları namespace yap.
        }

        public static string GetTableName<TEntity>()
        {
            return GetTableName(typeof(TEntity));
        }
    }
}
