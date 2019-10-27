namespace Ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBatchCommandExecute
    {
        int Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
        Task<int> ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
    }
    public abstract class BatchCommandExecute : IBatchCommandExecute
    {
        protected BatchCommandExecute(IDbAccess dataAccess)
        {
            if (null == dataAccess)
                throw new ArgumentNullException(nameof(dataAccess));

            this.DataAccess = dataAccess;
        }
        public IDbAccess DataAccess { get; }

        public abstract int Execute<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
        public abstract Task<int> ExecuteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
    }

    public interface IBatchCommandUpdate
    {
        HashSet<string> UpdatedFields { get; set; }
        int Update<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
        Task<int> UpdateAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
    }

    public interface IBatchCommandInsert
    {
        HashSet<string> InsertFields { get; set; }
        int Insert<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
        Task<int> InsertAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
    }

    public interface IBatchCommandUpsert
    {
        HashSet<string> UpdatedFields { get; set; }
        HashSet<string> InsertFields { get; set; }
        int Upsert<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
        Task<int> UpsertAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
    }

    public interface IBatchCommandDelete
    {
        int Delete<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
        Task<int> DeleteAsync<TEntity>(IEnumerable<TEntity> entityList, IEntityMetaDataProvider provider);
    }
}
