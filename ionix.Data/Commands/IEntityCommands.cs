namespace Ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IEntityCommandExecute
    {
        int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
        Task<int> ExecuteAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider);

    }
    public abstract class EntityCommandExecute : IEntityCommandExecute
    {
        protected EntityCommandExecute(IDbAccess dataAccess)
        {
            if (null == dataAccess)
                throw new ArgumentNullException(nameof(dataAccess));

            this.DataAccess = dataAccess;
        }
        public IDbAccess DataAccess { get; }

        public abstract int Execute<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
        public abstract Task<int> ExecuteAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
    }

    public interface IEntityCommandUpdate
    {
        HashSet<string> UpdatedFields { get; set; }
        int Update<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
        Task<int> UpdateAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
    }
    public interface IEntityCommandInsert
    {
        HashSet<string> InsertFields { get; set; }
        int Insert<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
        Task<int> InsertAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
    }
    public interface IEntityCommandUpsert
    {
        HashSet<string> UpdatedFields { get; set; }
        HashSet<string> InsertFields { get; set; }
        int Upsert<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
        Task<int> UpsertAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
    }
    public interface IEntityCommandDelete
    {
        int Delete<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
        Task<int> DeleteAsync<TEntity>(TEntity entity, IEntityMetaDataProvider provider);
    }
}
