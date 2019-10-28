namespace Ionix.Data
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Threading.Tasks;

    public enum TypeConversionMode
    {
        DoNotConvert = 0,
        Convert,
        ConvertSafely
    }

    public interface IEntityCommandSelect//Select ler Entity üzerinden otomatik yazılan Select ifadeleri. Query ise custom için.
    {
        TypeConversionMode ConversionMode { get; set; }

        TEntity SelectById<TEntity>(IEntityMetaDataProvider provider, params object[] keys);
        Task<TEntity> SelectByIdAsync<TEntity>(IEntityMetaDataProvider provider, params object[] keys);

        TEntity SelectSingle<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery);
        Task<TEntity> SelectSingleAsync<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery);

        IList<TEntity> Select<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery);
        Task<IList<TEntity>> SelectAsync<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery);

        TEntity QuerySingle<TEntity>(IEntityMetaDataProvider provider, SqlQuery query);//Property adı kolondan farklı olan durumlar için IEntityMetaDataProvider provider eklendi.
        Task<TEntity> QuerySingleAsync<TEntity>(IEntityMetaDataProvider provider, SqlQuery query);//Property adı kolondan farklı olan durumlar için IEntityMetaDataProvider provider eklendi.

        (TEntity1, TEntity2) QuerySingle<TEntity1, TEntity2>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<(TEntity1, TEntity2)> QuerySingleAsync<TEntity1, TEntity2>(IEntityMetaDataProvider provider, SqlQuery query);

        (TEntity1, TEntity2, TEntity3) QuerySingle<TEntity1, TEntity2, TEntity3>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<(TEntity1, TEntity2, TEntity3)> QuerySingleAsync<TEntity1, TEntity2, TEntity3>(IEntityMetaDataProvider provider, SqlQuery query);

        (TEntity1, TEntity2, TEntity3, TEntity4) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<(TEntity1, TEntity2, TEntity3, TEntity4)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4>(IEntityMetaDataProvider provider, SqlQuery query);

        (TEntity1, TEntity2, TEntity3, TEntity4, TEntity5) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(IEntityMetaDataProvider provider, SqlQuery query);

        (TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(IEntityMetaDataProvider provider, SqlQuery query);

        (TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(IEntityMetaDataProvider provider, SqlQuery query);



        IList<TEntity> Query<TEntity>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<IList<TEntity>> QueryAsync<TEntity>(IEntityMetaDataProvider provider, SqlQuery query);

        IList<(TEntity1, TEntity2)> Query<TEntity1, TEntity2>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<IList<(TEntity1, TEntity2)>> QueryAsync<TEntity1, TEntity2>(IEntityMetaDataProvider provider, SqlQuery query);

        IList<(TEntity1, TEntity2, TEntity3)> Query<TEntity1, TEntity2, TEntity3>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<IList<(TEntity1, TEntity2, TEntity3)>> QueryAsync<TEntity1, TEntity2, TEntity3>(IEntityMetaDataProvider provider, SqlQuery query);

        IList<(TEntity1, TEntity2, TEntity3, TEntity4)> Query<TEntity1, TEntity2, TEntity3, TEntity4>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4>(IEntityMetaDataProvider provider, SqlQuery query);

        IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(IEntityMetaDataProvider provider, SqlQuery query);

        IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(IEntityMetaDataProvider provider, SqlQuery query);

        IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(IEntityMetaDataProvider provider, SqlQuery query);
        Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(IEntityMetaDataProvider provider, SqlQuery query);
    }

    public partial class EntityCommandSelect : IEntityCommandSelect
    {
        public EntityCommandSelect(IDbAccess dataAccess, char parameterPrefix)
        {
            if (null == dataAccess)
                throw new ArgumentNullException(nameof(dataAccess));

            this.DataAccess = dataAccess;
            this.ParameterPrefix = parameterPrefix;
        }

        public IDbAccess DataAccess { get; }

        public char ParameterPrefix { get; }

        public TypeConversionMode ConversionMode { get; set; }

        private enum MapType
        {
            Select,
            Query
        }

        private void SetValue(object entity, object dbValue, PropertyInfo pi)
        {
            if (dbValue == DBNull.Value)
            {
                pi.SetValue(entity, null, null);
            }
            else
            {
                switch (ConversionMode)
                {
                    case TypeConversionMode.DoNotConvert:
                        pi.SetValue(entity, dbValue, null);
                        break;
                    case TypeConversionMode.Convert:
                        pi.SetValueConvert(entity, dbValue);
                        break;
                    case TypeConversionMode.ConvertSafely:
                        pi.SetValueConvertSafely(entity, dbValue);
                        break;
                    default:
                        throw new NotSupportedException(ConversionMode.ToString());
                }
            }
        }

        private void Map<TEntity>(TEntity entity, IEntityMetaData metaData, IDataReader dr, MapType mapType)
        {
            switch (mapType)
            {
                case MapType.Select:
                    foreach (PropertyMetaData md in metaData.Properties)
                    {
                        string columnName = md.Schema.ColumnName;
                        PropertyInfo pi = md.Property;
                        if (pi.GetSetMethod() != null)
                        {
                            object dbValue = dr[columnName];

                            SetValue(entity, dbValue, pi);
                        }
                    }
                    break;
                case MapType.Query:
                    int fieldCount = dr.FieldCount;
                    for (int j = 0; j < fieldCount; ++j)
                    {
                        string columnName = dr.GetName(j);
                        PropertyMetaData md = metaData[columnName];// metaData.Properties.FirstOrDefault(p => String.Equals(columnName, p.Schema.ColumnName));
                        if (null != md)
                        {
                            PropertyInfo pi = md.Property;
                            if (pi.GetSetMethod() != null)
                            {
                                object dbValue = dr[j];

                                SetValue(entity, dbValue, pi);
                            }
                        }
                    }
                    break;
                default:
                    throw new NotSupportedException(mapType.ToString());
            }
        }

        private TEntity ReadEntity<TEntity>(IEntityMetaData metaData, SqlQuery query, MapType mapType)
        {
            IDataReader dr = null;
            try
            {
                dr = this.DataAccess.CreateDataReader(query, CommandBehavior.SingleRow);

                if (dr.Read())
                {
                    TEntity entity = Activator.CreateInstance<TEntity>();
                    this.Map<TEntity>(entity, metaData, dr, mapType);
                    return entity;
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return default(TEntity);
        }
        private async Task<TEntity> ReadEntityAsync<TEntity>(IEntityMetaData metaData, SqlQuery query, MapType mapType)
        {
            IDataReader dr = null;
            try
            {
                dr = await this.DataAccess.CreateDataReaderAsync(query, CommandBehavior.SingleRow);

                if (dr.Read())
                {
                    TEntity entity = Activator.CreateInstance<TEntity>();
                    this.Map<TEntity>(entity, metaData, dr, mapType);
                    return entity;
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return default(TEntity);
        }

        private IList<TEntity> ReadEntityList<TEntity>(IEntityMetaData metaData, SqlQuery query, MapType mapType)
        {
            List<TEntity> ret = new List<TEntity>();

            IDataReader dr = null;
            try
            {
                dr = this.DataAccess.CreateDataReader(query, CommandBehavior.Default);
                while (dr.Read())
                {
                    TEntity entity = Activator.CreateInstance<TEntity>();
                    this.Map<TEntity>(entity, metaData, dr, mapType);
                    ret.Add(entity);
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }
        private async Task<IList<TEntity>> ReadEntityListAsync<TEntity>(IEntityMetaData metaData, SqlQuery query, MapType mapType)
        {
            List<TEntity> ret = new List<TEntity>();

            IDataReader dr = null;
            try
            {
                dr = await this.DataAccess.CreateDataReaderAsync(query, CommandBehavior.Default);
                while (dr.Read())
                {
                    TEntity entity = Activator.CreateInstance<TEntity>();
                    this.Map<TEntity>(entity, metaData, dr, mapType);
                    ret.Add(entity);
                }
            }
            finally
            {
                if (dr != null) dr.Dispose();
            }

            return ret;
        }

        private (IEntityMetaData, SqlQuery) PrepareSelectById<TEntity>(IEntityMetaDataProvider provider, object[] keys)
        {
            if (keys.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(keys));

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQueryBuilderSelect builder = new SqlQueryBuilderSelect(metaData);
            SqlQuery query = builder.ToQuery();//Select sql yazıldı.

            FilterCriteriaList filters = new FilterCriteriaList(this.ParameterPrefix);

            IList<PropertyMetaData> keySchemas = metaData.OfKeys(true);//Order a göre geldiği için böyle.
            if (keySchemas.Count != keys.Length)
                throw new InvalidOperationException("Keys and Valus count does not match");

            int index = -1;
            foreach (PropertyMetaData keyProperty in keySchemas)
            {
                string parameterName = metaData.GetParameterName(keyProperty, 0);
                filters.Add(keyProperty.Schema.ColumnName, parameterName, ConditionOperator.Equals, keys[++index]);
            }

            query.Combine(filters.ToQuery());//Where ifadesi oluşturuldu. Eğer ki

            return (metaData, query);

        }
        public virtual TEntity SelectById<TEntity>(IEntityMetaDataProvider provider, params object[] keys)
        {
            (IEntityMetaData metaData, SqlQuery query) = this.PrepareSelectById<TEntity>(provider, keys);

            return this.ReadEntity<TEntity>(metaData, query, MapType.Select);
        }
        public virtual Task<TEntity> SelectByIdAsync<TEntity>(IEntityMetaDataProvider provider, params object[] keys)
        {
            (IEntityMetaData metaData, SqlQuery query) = this.PrepareSelectById<TEntity>(provider, keys);

            return this.ReadEntityAsync<TEntity>(metaData, query, MapType.Select);
        }


        private static (IEntityMetaData, SqlQuery) PrepareSelect<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQueryBuilderSelect builder = new SqlQueryBuilderSelect(metaData);
            SqlQuery query = builder.ToQuery();
            if (null != extendedQuery)
                query.Combine(extendedQuery);

            return (metaData, query);
        }
        public virtual TEntity SelectSingle<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery)
        {
            (IEntityMetaData metaData, SqlQuery query) = PrepareSelect<TEntity>(provider, extendedQuery);

            return this.ReadEntity<TEntity>(metaData, query, MapType.Select);
        }
        public virtual Task<TEntity> SelectSingleAsync<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery)
        {
            (IEntityMetaData metaData, SqlQuery query) = PrepareSelect<TEntity>(provider, extendedQuery);

            return this.ReadEntityAsync<TEntity>(metaData, query, MapType.Select);
        }


        public virtual IList<TEntity> Select<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery)
        {
            (IEntityMetaData metaData, SqlQuery query) = PrepareSelect<TEntity>(provider, extendedQuery);

            return this.ReadEntityList<TEntity>(metaData, query, MapType.Select);
        }
        public virtual Task<IList<TEntity>> SelectAsync<TEntity>(IEntityMetaDataProvider provider, SqlQuery extendedQuery)
        {
            (IEntityMetaData metaData, SqlQuery query) = PrepareSelect<TEntity>(provider, extendedQuery);

            return this.ReadEntityListAsync<TEntity>(metaData, query, MapType.Select);
        }


        private static IEntityMetaData PrepareQuery<TEntity>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            if (null == query)
                throw new ArgumentNullException(nameof(query));

            return provider.EnsureCreateEntityMetaData<TEntity>();
        }
        public virtual TEntity QuerySingle<TEntity>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            return this.ReadEntity<TEntity>(PrepareQuery<TEntity>(provider, query), query, MapType.Query);
        }
        public virtual Task<TEntity> QuerySingleAsync<TEntity>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            return this.ReadEntityAsync<TEntity>(PrepareQuery<TEntity>(provider, query), query, MapType.Query);
        }


        public virtual IList<TEntity> Query<TEntity>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            return this.ReadEntityList<TEntity>(PrepareQuery<TEntity>(provider, query), query, MapType.Query);
        }
        public virtual Task<IList<TEntity>> QueryAsync<TEntity>(IEntityMetaDataProvider provider, SqlQuery query)
        {
            return this.ReadEntityListAsync<TEntity>(PrepareQuery<TEntity>(provider, query), query, MapType.Query);
        }
    }
}
