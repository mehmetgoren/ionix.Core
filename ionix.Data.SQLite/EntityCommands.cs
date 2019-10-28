namespace Ionix.Data.SQLite
{
    using Utils.Extensions;
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
            EntitySqlQueryBuilderInsert builder = new EntitySqlQueryBuilderInsert() { InsertFields = this.InsertFields };
            PropertyMetaData identity;
            SqlQuery query = builder.CreateQuery(entity, metaData, 0, out identity);

            return (query, identity);
        }
        public override int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
            (SqlQuery query, PropertyMetaData identity) = this.Prepare(entity, metaData);

            if (null != identity)
            {
                object value = base.DataAccess.ExecuteScalar(query);
                identity.Property.SetValueConvertSafely(entity, value);

                return value.IsNull() ? 0 : 1;
            }
            else
            {
                return base.DataAccess.ExecuteNonQuery(query);
            }
        }
        public override async Task<int> ExecuteAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();
            (SqlQuery query, PropertyMetaData identity) = this.Prepare(entity, metaData);

            if (null != identity)
            {
                object value = await base.DataAccess.ExecuteScalarAsync(query);
                identity.Property.SetValueConvertSafely(entity, value);

                return value.IsNull() ? 0 : 1;
            }
            else
            {
                return await base.DataAccess.ExecuteNonQueryAsync(query);
            }
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

        public override int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            EntityCommandUpdate updateCmd = new EntityCommandUpdate(base.DataAccess) { UpdatedFields = this.UpdatedFields };

            int ret = updateCmd.Execute(entity, provider);
            if (ret == 0)// if not update then insert.
            {
                EntityCommandInsert insertCmd = new EntityCommandInsert(base.DataAccess) { InsertFields = this.InsertFields };
                return insertCmd.Execute(entity, provider);
            }

            return ret;
        }
        public override async Task<int> ExecuteAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider)
        {
            EntityCommandUpdate updateCmd = new EntityCommandUpdate(base.DataAccess) { UpdatedFields = this.UpdatedFields };

            int ret = await updateCmd.ExecuteAsync(entity, provider);
            if (ret == 0)// if not update then insert.
            {
                EntityCommandInsert insertCmd = new EntityCommandInsert(base.DataAccess) { InsertFields = this.InsertFields };
                return await insertCmd.ExecuteAsync(entity, provider);
            }

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
