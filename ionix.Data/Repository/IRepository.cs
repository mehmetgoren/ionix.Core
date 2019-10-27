namespace Ionix.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> : IDisposable
    {
        //Props
        ICommandAdapter Cmd { get; }
        IDbAccess DataAccess { get; }
        //

        //Evets
        event EventHandler<PreExecuteCommandEventArgs<TEntity>> PreExecuteCommand;
        event EventHandler<ExecuteCommandCompleteEventArgs<TEntity>> ExecuteCommandCompleted;
        //

        //Select
        TEntity SelectById(params object[] keys);
        Task<TEntity> SelectByIdAsync(params object[] keys);

        TEntity SelectSingle(SqlQuery extendedQuery);
        Task<TEntity> SelectSingleAsync(SqlQuery extendedQuery);

        IList<TEntity> Select(SqlQuery extendedQuery);
        Task<IList<TEntity>> SelectAsync(SqlQuery extendedQuery);

        TEntity QuerySingle(SqlQuery query);
        Task<TEntity> QuerySingleAsync(SqlQuery query);

        IList<TEntity> Query(SqlQuery query);
        Task<IList<TEntity>> QueryAsync(SqlQuery query);
        //

        //Entity
        int Update(TEntity entity, params string[] updatedFields);
        Task<int> UpdateAsync(TEntity entity, params string[] updatedFields);

        int Insert(TEntity entity, params string[] insertFields);
        Task<int> InsertAsync(TEntity entity, params string[] insertFields);

        int Upsert(TEntity entity, string[] updatedFields, string[] insertFields);
        Task<int> UpsertAsync(TEntity entity, string[] updatedFields, string[] insertFields);

        int Delete(TEntity entity);
        Task<int> DeleteAsync(TEntity entity);
        //

        //Batch
        int BatchUpdate(IEnumerable<TEntity> entityList, params string[] updatedFields);
        Task<int> BatchUpdateAsync(IEnumerable<TEntity> entityList, params string[] updatedFields);

        int BatchInsert(IEnumerable<TEntity> entityList, params string[] insertFields);
        Task<int> BatchInsertAsync(IEnumerable<TEntity> entityList, params string[] insertFields);

        int BatchUpsert(IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFields);
        Task<int> BatchUpsertAsync(IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFields);

        int BatchDelete(IEnumerable<TEntity> entityList);
        Task<int> BatchDeleteAsync(IEnumerable<TEntity> entityList);
        //
    }
}
