namespace Ionix.Data
{
    using System;
    using System.Reflection;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    using Ionix.Utils.Extensions;

    public abstract class EntityMetaDataProviderBase : IEntityMetaDataProvider
    {
        private static readonly IDictionary<Type, Dictionary<Type, IEntityMetaData>> _cache = new Dictionary<Type, Dictionary<Type, IEntityMetaData>>();//new ConcurrentDictionary<Type, IEntityMetaData>();// new Dictionary<Type, IEntityMetaData>(); 

        protected virtual bool IsMapped(PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;

            if (ReflectionExtensions.IsEnumerable(pi.PropertyType))
                return false;

            return ReflectionExtensions.IsPrimitiveType(pi.PropertyType);
        }
        protected abstract void SetExtendedSchema(SchemaInfo schema, PropertyInfo pi);
        protected virtual void SetExtendedMetaData(EntityMetaData metaData) { }

        private SchemaInfo FromPropertyInfo(PropertyInfo pi)
        {
            if (!this.IsMapped(pi))
                return null;

            Type propertyType = pi.PropertyType;

            bool nullableTypeDetected = propertyType.IsNullableType();

            SchemaInfo schema = new SchemaInfo(pi.Name, nullableTypeDetected ? propertyType.GetTypeInfo().GetGenericArguments()[0] : propertyType);

            if (nullableTypeDetected)
                schema.IsNullable = true;
            else
            {
                TypeInfo propertyTypeInfo = propertyType.GetTypeInfo();
                if (propertyTypeInfo.IsClass)
                    schema.IsNullable = true;
                else if (propertyTypeInfo.IsValueType)
                    schema.IsNullable = false;
            }

            bool hasSetMethod = pi.SetMethod != null;
            if (!hasSetMethod)
                schema.ReadOnly = true;

            this.SetExtendedSchema(schema, pi);

            return schema;
        }

        private static readonly object syncRoot = new object();
        public IEntityMetaData CreateEntityMetaData(Type entityType)
        {
            lock (syncRoot)
            {
                Type derivedType = this.GetType();
                Dictionary<Type, IEntityMetaData> tempCache;
                if (!_cache.TryGetValue(derivedType, out tempCache))
                {
                    tempCache = new Dictionary<Type, IEntityMetaData>();
                    _cache.Add(derivedType, tempCache);
                }

                IEntityMetaData metaData;
                if (!tempCache.TryGetValue(entityType, out metaData))
                {
                    int order = 0;
                    EntityMetaData temp = new EntityMetaData(entityType);
                    foreach (PropertyInfo pi in entityType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        ++order;
                        SchemaInfo schema = this.FromPropertyInfo(pi);
                        if (null == schema) //NotMapped.
                            continue;

                        schema.Order = order; //Order parametere isimlarine ardışıklık için çok önemli. Oyüzden base de atamasını yaptık.
                        schema.Lock();
                        temp.Add(schema, pi);
                    }

                    this.SetExtendedMetaData(temp);

                    tempCache.Add(entityType, temp);
                    metaData = temp;
                }

                return metaData;
            }
        }
    }

    public class EntityMetaDataProvider : EntityMetaDataProviderBase
    {
        //this is not a singleton type but you can use this reference to avoid multiple unnecessary instance.
        public static readonly EntityMetaDataProvider Instance = new EntityMetaDataProvider();

        protected override void SetExtendedSchema(SchemaInfo schema, PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<KeyAttribute>() != null)
            {
                schema.IsKey = true;
            }

            if (pi.GetCustomAttribute<RequiredAttribute>() != null)
            {
                schema.IsNullable = false;
            }
            var sla = pi.GetCustomAttribute<StringLengthAttribute>();
            if (null != sla)
            {
                schema.MaxLength = sla.MaximumLength;
            }
        }
    }

    //DbSchemaAttribute ile kullanılacak custom yapılar için.
    public class DbSchemaMetaDataProvider : EntityMetaDataProviderBase
    {
        //this is not a singleton type but you can use this reference to avoid multiple unnecessary instance.
        public static readonly DbSchemaMetaDataProvider Instance = new DbSchemaMetaDataProvider();

        protected override void SetExtendedSchema(SchemaInfo schema, PropertyInfo pi)
        {
            DbSchemaAttribute att = pi.GetCustomAttribute<DbSchemaAttribute>();
            if (null != att)
            {
                if (!String.IsNullOrEmpty(att.ColumnName))
                    schema.ColumnName = att.ColumnName;
                schema.DatabaseGeneratedOption = att.DatabaseGeneratedOption;
                schema.DefaultValue = att.DefaultValue;
                schema.IsKey = att.IsKey;
                schema.MaxLength = att.MaxLength;

                if (schema.IsKey)
                    schema.IsNullable = false;
                else if (att.isNullable.IsDirty)//Ensure it is changes by user. Because the metadataprovider set the value properly.
                    schema.IsNullable = att.IsNullable;

                if (att.readOnly.IsDirty)//Ensure it is changes by user.
                    schema.ReadOnly = att.ReadOnly;

                schema.SqlValueType = att.SqlValueType;
            }
        }
    }
}