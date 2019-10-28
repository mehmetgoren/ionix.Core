namespace Ionix.Data.SqlServer
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

        private SqlQuery CreateBatchUpdateQuery<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQuery batchQuery = new SqlQuery();

            batchQuery.Text.Append(GlobalInternal.SqlServerBeginStatement);

            int index = 0;
            EntitySqlQueryBuilderUpdate updateBuilder = new EntitySqlQueryBuilderUpdate();
            updateBuilder.UpdatedFields = this.UpdatedFields;

            foreach (TEntity entity in entityList)
            {
                batchQuery.Text.AppendLine();
                batchQuery.Combine(updateBuilder.CreateQuery(entity, metaData, index++));
            }

            batchQuery.Text.AppendLine();
            batchQuery.Text.Append(GlobalInternal.SqlServerEndStatement);

            return batchQuery;
        }

        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return 0;

            SqlQuery batchQuery = this.CreateBatchUpdateQuery(entityList, provider);
            return base.DataAccess.ExecuteNonQuery(batchQuery);
        }
        public override Task<int> ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return Task.FromResult(0);

            SqlQuery batchQuery = this.CreateBatchUpdateQuery(entityList, provider);
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

        private (SqlQuery, PropertyMetaData, List<string>) CreateBatchInsertQuery<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQuery batchQuery = new SqlQuery();

            batchQuery.Text.Append(GlobalInternal.SqlServerBeginStatement);

            int index = 0;
            List<string> outParameterNames = new List<string>();
            EntitySqlQueryBuilderInsert insertBuilder = new EntitySqlQueryBuilderInsert();
            insertBuilder.InsertFields = this.InsertFields;

            PropertyMetaData identity = null; //Tek Bir Tane Identity var sadece paremetre ismi değişiyor.
            foreach (TEntity entity in entityList)
            {
                batchQuery.Text.AppendLine();

                batchQuery.Combine(insertBuilder.CreateQuery(entity, metaData, index, out identity));
                if (null != identity)
                {
                    string parameterName = metaData.GetParameterName(identity, index);
                    outParameterNames.Add(parameterName);
                }

                ++index;
            }

            batchQuery.Text.AppendLine();
            batchQuery.Text.Append(GlobalInternal.SqlServerEndStatement);

            return (batchQuery, identity, outParameterNames);
        }

        internal static void SetOutputParametersValues<TEntity>(PropertyMetaData identity, IEnumerable<TEntity> entityList, SqlQuery batchQuery
            , List<string> outParameterNames)
        {
            if (null != identity)//outParameterNames.Count must be equal to entityList.Count.
            {
                int index = -1;
                foreach (TEntity entity in entityList)
                {
                    string outParameterName = outParameterNames[++index];
                    object identityValue = batchQuery.Parameters.Find(outParameterName).Value;
                    identity.Property.SetValue(entity, identityValue, null);
                }
            }
        }

        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return 0;

            (SqlQuery batchQuery, PropertyMetaData identity, List<string> outParameterNames) = this.CreateBatchInsertQuery(entityList, provider);
            int ret = base.DataAccess.ExecuteNonQuery(batchQuery);
            SetOutputParametersValues(identity, entityList, batchQuery, outParameterNames);

            return ret;
        }

        public override async Task<int> ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return 0;

            (SqlQuery batchQuery, PropertyMetaData identity, List<string> outParameterNames) = this.CreateBatchInsertQuery(entityList, provider);
            int ret = await base.DataAccess.ExecuteNonQueryAsync(batchQuery);//await çünkü dış parametreleri set ediyoruz.
            SetOutputParametersValues(identity, entityList, batchQuery, outParameterNames);

            return ret;
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

        private (SqlQuery, PropertyMetaData, List<string>) CreateBatchUpsertQuery<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            IEntityMetaData metaData = provider.EnsureCreateEntityMetaData<TEntity>();

            SqlQuery batchQuery = new SqlQuery();

            batchQuery.Text.Append(GlobalInternal.SqlServerBeginStatement);


            int index = 0;
            List<string> outParameterNames = new List<string>();
            EntitySqlQueryBuilderUpsert upsertBuilder = new EntitySqlQueryBuilderUpsert();
            upsertBuilder.UpdatedFields = this.UpdatedFields;
            upsertBuilder.InsertFields = this.InsertFields;

            PropertyMetaData identity = null; //Tek Bir Tane Identity var sadece paremetre ismi değişiyor.
            foreach (TEntity entity in entityList)
            {
                batchQuery.Text.AppendLine();

                batchQuery.Combine(upsertBuilder.CreateQuery(entity, metaData, index, out identity));
                if (null != identity)
                {
                    string parameterName = metaData.GetParameterName(identity, index);
                    outParameterNames.Add(parameterName);
                }

                ++index;
            }

            batchQuery.Text.AppendLine();
            batchQuery.Text.Append(GlobalInternal.SqlServerEndStatement);

            return (batchQuery, identity, outParameterNames);
        }

        public override int Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return 0;

            (SqlQuery batchQuery, PropertyMetaData identity, List<string> outParameterNames) = this.CreateBatchUpsertQuery(entityList, provider);
            int ret = base.DataAccess.ExecuteNonQuery(batchQuery);
            BatchCommandInsert.SetOutputParametersValues(identity, entityList, batchQuery, outParameterNames);

            return ret;
        }

        public override async Task<int> ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider)
        {
            if (entityList.IsNullOrEmpty())
                return 0;

            (SqlQuery batchQuery, PropertyMetaData identity, List<string> outParameterNames) = this.CreateBatchUpsertQuery(entityList, provider);
            int ret = await base.DataAccess.ExecuteNonQueryAsync(batchQuery);//await çünkü dış parametreleri set ediyoruz.
            BatchCommandInsert.SetOutputParametersValues(identity, entityList, batchQuery, outParameterNames);

            return ret;
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

            batchQuery.Text.Append(GlobalInternal.SqlServerBeginStatement);

            int index = 0;
            EntitySqlQueryBuilderDelete deleteBuilder = new EntitySqlQueryBuilderDelete();

            foreach (TEntity entity in entityList)
            {
                batchQuery.Text.AppendLine();
                batchQuery.Combine(deleteBuilder.CreateQuery(entity, metaData, index++));
            }

            batchQuery.Text.AppendLine();
            batchQuery.Text.Append(GlobalInternal.SqlServerEndStatement);

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
