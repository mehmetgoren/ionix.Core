namespace Ionix.Data.PostgreSql
{
    using Utils.Extensions;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class BatchCommandUpdate : BatchCommandExecute, IBatchCommandUpdate
    {
        public BatchCommandUpdate(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> UpdatedFields { get; set; }

        int IBatchCommandUpdate.Update<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            return this.Execute<TEntity>(entityList, provider);
        }
        Task<int> IBatchCommandUpdate.UpdateAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            return this.ExecuteAsync<TEntity>(entityList, provider);
        }

        private SqlQuery CreateUpdateQuery<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQuery batchQuery = new SqlQuery();

            batchQuery.Text.Append(GlobalInternal.BeginStatement);

            int index = 0;
            EntitySqlQueryBuilderUpdate updateBuilder = new EntitySqlQueryBuilderUpdate() { UseSemicolon = true };
            updateBuilder.UpdatedFields = this.UpdatedFields;

            foreach (TEntity entity in entityList)
            {
                batchQuery.Text.AppendLine();

                batchQuery.Combine(updateBuilder.CreateQuery(entity, metaData, index++));
            }

            batchQuery.Text.AppendLine();
            batchQuery.Text.Append(GlobalInternal.EndStatement);

            return batchQuery;
        }
        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return 0;

            SqlQuery batchQuery = this.CreateUpdateQuery(entityList, provider);

            return base.DataAccess.ExecuteNonQuery(batchQuery);
        }
        public override Task<int> ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return Task.FromResult(0);

            SqlQuery batchQuery = this.CreateUpdateQuery(entityList, provider);

            return base.DataAccess.ExecuteNonQueryAsync(batchQuery);
        }
    }

    public class BatchCommandInsert : BatchCommandExecute, IBatchCommandInsert
    {
        public BatchCommandInsert(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> InsertFields { get; set; }

        int IBatchCommandInsert.Insert<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            return this.Execute(entityList, provider);
        }
        Task<int> IBatchCommandInsert.InsertAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            return this.ExecuteAsync(entityList, provider);
        }

        private SqlQuery CreateInsertQuery<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQuery batchQuery = new SqlQuery();

            batchQuery.Text.Append(GlobalInternal.BeginStatement);


            int index = 0;
            EntitySqlQueryBuilderInsert insertBuilder = new EntitySqlQueryBuilderInsert() { UseSemicolon = true };
            insertBuilder.InsertFields = this.InsertFields;

            PropertyMetaData sequenceIdentity; //Postgres Output parametreyi desteklemediği için öylesine konuldu.
            foreach (TEntity entity in entityList)
            {
                batchQuery.Text.AppendLine();

                batchQuery.Combine(insertBuilder.CreateQuery(entity, metaData, index++, out sequenceIdentity));
            }

            batchQuery.Text.AppendLine();
            batchQuery.Text.Append(GlobalInternal.EndStatement);

            return batchQuery;
        }
        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return 0;

            SqlQuery batchQuery = this.CreateInsertQuery(entityList, provider);
            return base.DataAccess.ExecuteNonQuery(batchQuery);
        }
        public override Task<int> ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return Task.FromResult(0);

            SqlQuery batchQuery = this.CreateInsertQuery(entityList, provider);
            return base.DataAccess.ExecuteNonQueryAsync(batchQuery);
        }
    }

    public class BatchCommandUpsert : BatchCommandExecute, IBatchCommandUpsert
    {
        public BatchCommandUpsert(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        public HashSet<string> UpdatedFields { get; set; }

        public HashSet<string> InsertFields { get; set; }

        int IBatchCommandUpsert.Upsert<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            return this.Execute(entityList, provider);
        }
        Task<int> IBatchCommandUpsert.UpsertAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            return this.ExecuteAsync(entityList, provider);
        }

        private EntityCommandUpsert CreateUpsertCommand()
        {
            EntityCommandUpsert upsertCommand = new EntityCommandUpsert(base.DataAccess);
            upsertCommand.UpdatedFields = this.UpdatedFields;
            upsertCommand.InsertFields = this.InsertFields;

            return upsertCommand;
        }
        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return 0;

            int sum = 0;
            EntityCommandUpsert upsertCommand = this.CreateUpsertCommand();

            foreach (TEntity entity in entityList)
            {
                sum += upsertCommand.Execute(entity, provider);
            }
            return sum;
        }
        public override async Task<int> ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return 0;

            int sum = 0;
            EntityCommandUpsert upsertCommand = this.CreateUpsertCommand();

            foreach (TEntity entity in entityList)
            {
                sum += await upsertCommand.ExecuteAsync(entity, provider);
            }
            return sum;
        }
    }


    public class BatchCommandDelete : BatchCommandExecute, IBatchCommandDelete
    {
        public BatchCommandDelete(IDbAccess dataAccess)
            : base(dataAccess)
        { }

        int IBatchCommandDelete.Delete<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            return this.Execute(entityList, provider);
        }

        Task<int> IBatchCommandDelete.DeleteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            return this.ExecuteAsync(entityList, provider);
        }

        private SqlQuery CreateBatchDeleteQuery<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQuery batchQuery = new SqlQuery();

            batchQuery.Text.Append(GlobalInternal.BeginStatement);

            int index = 0;
            EntitySqlQueryBuilderDelete deleteBuilder = new EntitySqlQueryBuilderDelete() { UseSemicolon = true };

            foreach (TEntity entity in entityList)
            {
                batchQuery.Text.AppendLine();
                batchQuery.Combine(deleteBuilder.CreateQuery(entity, metaData, index++));
            }

            batchQuery.Text.AppendLine();
            batchQuery.Text.Append(GlobalInternal.EndStatement);

            return batchQuery;
        }
        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return 0;

            SqlQuery batchQuery = this.CreateBatchDeleteQuery(entityList, provider);
            return base.DataAccess.ExecuteNonQuery(batchQuery);
        }

        public override Task<int> ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return Task.FromResult(0);

            SqlQuery batchQuery = this.CreateBatchDeleteQuery(entityList, provider);
            return base.DataAccess.ExecuteNonQueryAsync(batchQuery);
        }
    }
}
