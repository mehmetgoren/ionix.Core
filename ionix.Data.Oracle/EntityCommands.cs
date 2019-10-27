namespace Ionix.Data.Oracle
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class EntityCommandUpdate : EntityCommandExecute, IEntityCommandUpdate
    {
        public EntityCommandUpdate(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> UpdatedFields { get; set; }

        int IEntityCommandUpdate.Update<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.Execute(entity, provider);
        }
        Task<int> IEntityCommandUpdate.UpdateAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.ExecuteAsync(entity, provider);
        }

        private SqlQuery CreateQuery(object entity, IEntityMetaData metaData)
        {
            EntitySqlQueryBuilderUpdate builder = new EntitySqlQueryBuilderUpdate() { UpdatedFields = this.UpdatedFields };
            SqlQuery query = builder.CreateQuery(entity, metaData, 0);
            return query;
        }
        public override int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            if (null == entity)
                return 0;

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
            SqlQuery query = this.CreateQuery(entity, metaData);

            return base.DataAccess.ExecuteNonQuery(query);
        }
        public override Task<int> ExecuteAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            if (null == entity)
                return Task.FromResult(0);

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
            SqlQuery query = this.CreateQuery(entity, metaData);

            return base.DataAccess.ExecuteNonQueryAsync(query);
        }
    }

    public class EntityCommandInsert : EntityCommandExecute, IEntityCommandInsert
    {
        public EntityCommandInsert(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> InsertFields { get; set; }

        int IEntityCommandInsert.Insert<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.Execute(entity, provider);
        }
        Task<int> IEntityCommandInsert.InsertAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.ExecuteAsync(entity, provider);
        }

        private (SqlQuery, PropertyMetaData) Prepare(object entity, IEntityMetaData metaData)
        {
            EntitySqlQueryBuilderInsert builder = new EntitySqlQueryBuilderInsert(base.DataAccess, false) { InsertFields = this.InsertFields };
            PropertyMetaData sequenceIdentity;
            SqlQuery query = builder.CreateQuery(entity, metaData, 0, out sequenceIdentity);

            return (query, sequenceIdentity);
        }
        internal static void SetIdentityValue(object entity, IEntityMetaData metaData, SqlQuery query, PropertyMetaData sequenceIdentity)
        {
            if (null != sequenceIdentity)
            {
                string parameterName = metaData.GetParameterName(sequenceIdentity, 0);
                object identityValue = query.Parameters.Find(parameterName).Value;
                sequenceIdentity.Property.SetValue(entity, identityValue, null);
            }
        }

        public override int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider)//for Single
        {
            if (null == entity)
                return 0;

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            (SqlQuery query, PropertyMetaData sequenceIdentity) = this.Prepare(entity, metaData);
            int ret = base.DataAccess.ExecuteNonQuery(query);
            SetIdentityValue(entity, metaData, query, sequenceIdentity);

            return ret;
        }

        public override async Task<int> ExecuteAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider)//for Single
        {
            if (null == entity)
                return 0;

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            (SqlQuery query, PropertyMetaData sequenceIdentity) = this.Prepare(entity, metaData);
            int ret = await base.DataAccess.ExecuteNonQueryAsync(query);
            SetIdentityValue(entity, metaData, query, sequenceIdentity);

            return ret;
        }
    }

    public class EntityCommandUpsert : EntityCommandExecute, IEntityCommandUpsert
    {
        public EntityCommandUpsert(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> UpdatedFields { get; set; }

        public HashSet<string> InsertFields { get; set; }

        int IEntityCommandUpsert.Upsert<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.Execute(entity, provider);
        }
        Task<int> IEntityCommandUpsert.UpsertAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.ExecuteAsync(entity, provider);
        }

        private (SqlQuery, PropertyMetaData) Prepare(object entity, IEntityMetaData metaData)
        {
            EntitySqlQueryBuilderUpsert builder = new EntitySqlQueryBuilderUpsert(this.DataAccess, false) { UpdatedFields = this.UpdatedFields, InsertFields = this.InsertFields };
            PropertyMetaData sequenceIdentity;
            SqlQuery query = builder.CreateQuery(entity, metaData, 0, out sequenceIdentity);

            return (query, sequenceIdentity);
        }
        public override int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            if (null == entity)
                return 0;

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            (SqlQuery query, PropertyMetaData sequenceIdentity) = this.Prepare(entity, metaData);
            int ret = base.DataAccess.ExecuteNonQuery(query);
            EntityCommandInsert.SetIdentityValue(entity, metaData, query, sequenceIdentity);

            return ret;
        }
        public override async Task<int> ExecuteAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            if (null == entity)
                return 0;

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            (SqlQuery query, PropertyMetaData sequenceIdentity) = this.Prepare(entity, metaData);
            int ret = await base.DataAccess.ExecuteNonQueryAsync(query);
            EntityCommandInsert.SetIdentityValue(entity, metaData, query, sequenceIdentity);

            return ret;
        }
    }

    public class EntityCommandDelete : EntityCommandExecute, IEntityCommandDelete
    {
        public EntityCommandDelete(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        int IEntityCommandDelete.Delete<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.Execute(entity, provider);
        }
        Task<int> IEntityCommandDelete.DeleteAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            return this.ExecuteAsync(entity, provider);
        }

        private static SqlQuery CreateQuery(object entity, IEntityMetaData metaData)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));

            EntitySqlQueryBuilderDelete builder = new EntitySqlQueryBuilderDelete();
            SqlQuery query = builder.CreateQuery(entity, metaData, 0);

            return query;
        }
        public override int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            if (null == entity)
                return 0;

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
            SqlQuery query = CreateQuery(entity, metaData);

            return base.DataAccess.ExecuteNonQuery(query);
        }

        public override Task<int> ExecuteAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            if (null == entity)
                return Task.FromResult(0);

            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
            SqlQuery query = CreateQuery(entity, metaData);

            return base.DataAccess.ExecuteNonQueryAsync(CreateQuery(entity, metaData));
        }
    }
}
