namespace Ionix.Data
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class KeySchemaNotFoundException : Exception
    {
        public KeySchemaNotFoundException()
            : base("Key Schema Not Found")
        {

        }

        public KeySchemaNotFoundException(string message) : base(message)
        {
        }

        public KeySchemaNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }


    public static class SqlQueryHelper
    {
        public static void EnsureEntityMetaData(this IEntityMetaData metaData)
        {
            if (null == metaData)
                throw new ArgumentNullException(nameof(metaData));
            if (metaData.Properties.IsNullOrEmpty())
                throw new NullReferenceException("IEntityMetaData.Properties");
            if (String.IsNullOrEmpty(metaData.TableName))
                throw new NullReferenceException("IEntityMetaData.TableName");
        }

        public static IEntityMetaData EnsureCreateEntityMetaData<TEntity>(this IEntityMetaDataProvider provider)
        {
            if (null == provider)
                throw new ArgumentNullException(nameof(provider));

            IEntityMetaData ret = provider.CreateEntityMetaData(typeof(TEntity));
            ret.EnsureEntityMetaData();

            return ret;
        }

        //SelectById ile Kullanılıyor.
        public static IList<PropertyMetaData> OfKeys(this IEntityMetaData metaData, bool throwExcIfNoKeys)
        {
            List<PropertyMetaData> keys = metaData.Properties.Where(s => s.Schema.IsKey).ToList();
            if (!keys.IsNullOrEmpty())
            {
                keys.Sort((x, y) => x.Schema.Order.CompareTo(y.Schema.Order));
                return keys;
            }
            else
            {
                if (throwExcIfNoKeys)
                    throw new KeySchemaNotFoundException();
                return null;
            }
        }

        public static SqlQuery CreateWhereSqlByKeys(IEntityMetaData metaData, int index, char prefix, object entity)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            IList<PropertyMetaData> keySchemas = metaData.OfKeys(true);

            object[] keyValues = new object[keySchemas.Count];
            for (int j = 0; j < keySchemas.Count; ++j)
            {
                keyValues[j] = keySchemas[j].Property.GetValue(entity, null);
            }

            FilterCriteriaList list = new FilterCriteriaList(prefix);
            for (int j = 0; j < keySchemas.Count; ++j)
            {
                PropertyMetaData keySchema = keySchemas[j];
                string parameterName = metaData.GetParameterName(keySchema, index);
                list.Add(keySchema.Schema.ColumnName, parameterName, ConditionOperator.Equals, keyValues[j]);
            }
            return list.ToQuery();
        }

        //Upsert de Identity Parametre İçin Eklendi.
        public static SqlQueryParameter EnsureHasParameter(SqlQuery query, string parameterName, PropertyMetaData property, object entity)//inset de bu parametre normalde eklenmez ama upsert de update where de eklendiği için bu yapı kullanılıyor.
        {
            SqlQueryParameter identityParameter = query.Parameters.Find(parameterName);
            if (null == identityParameter)
            {
                object parValue = property.Property.GetValue(entity, null);
                identityParameter = SqlQueryParameter.Create(parameterName, property, parValue);

                query.Parameters.Add(identityParameter);
            }
            return identityParameter;
        }

        public static void SetColumnValue(DbValueSetter setter, IEntityMetaData metaData, int index, SqlQuery query, PropertyMetaData pm, object entity)//Parametewnin eklenip eklenmeyeceği bilinmdeğinden prefix ve entity verilmek zorunda.
        {
            setter.SetColumnValue(metaData, index, query, pm, entity);
        }

        public static PropertyMetaData GetPrimaryKey(this IEntityMetaData metaData)//not unique keys.
        {
            IList<PropertyMetaData> keys = metaData.OfKeys(true);
            if (keys.Count == 1)
            {
                return keys[0];
            }

            return null;//means Multiple key,  unique keys not Primary Key.
        }
    }
}