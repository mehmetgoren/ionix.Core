namespace Ionix.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICommandAdapter
    {
        ICommandFactory Factory { get; }
        TypeConversionMode ConversionMode { get; set; }

        TEntity SelectById<TEntity>(params object[] idValues);
        Task<TEntity> SelectByIdAsync<TEntity>(params object[] idValues);

        TEntity SelectSingle<TEntity>(SqlQuery extendedQuery);
        Task<TEntity> SelectSingleAsync<TEntity>(SqlQuery extendedQuery);

        IList<TEntity> Select<TEntity>(SqlQuery extendedQuery);
        Task<IList<TEntity>> SelectAsync<TEntity>(SqlQuery extendedQuery);

        TEntity QuerySingle<TEntity>(SqlQuery query);
        Task<TEntity> QuerySingleAsync<TEntity>(SqlQuery query);

        (TEntity1, TEntity2) QuerySingle<TEntity1, TEntity2>(SqlQuery query);
        Task<(TEntity1, TEntity2)> QuerySingleAsync<TEntity1, TEntity2>(SqlQuery query);

        (TEntity1, TEntity2, TEntity3) QuerySingle<TEntity1, TEntity2, TEntity3>(SqlQuery query);
        Task<(TEntity1, TEntity2, TEntity3)> QuerySingleAsync<TEntity1, TEntity2, TEntity3>(SqlQuery query);

        (TEntity1, TEntity2, TEntity3, TEntity4) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4>(SqlQuery query);
        Task<(TEntity1, TEntity2, TEntity3, TEntity4)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4>(SqlQuery query);

        (TEntity1, TEntity2, TEntity3, TEntity4, TEntity5) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(SqlQuery query);
        Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(SqlQuery query);

        (TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(SqlQuery query);
        Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(SqlQuery query);

        (TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7) QuerySingle<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(SqlQuery query);
        Task<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)> QuerySingleAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(SqlQuery query);


        IList<TEntity> Query<TEntity>(SqlQuery query);
        Task<IList<TEntity>> QueryAsync<TEntity>(SqlQuery query);

        IList<(TEntity1, TEntity2)> Query<TEntity1, TEntity2>(SqlQuery query);
        Task<IList<(TEntity1, TEntity2)>> QueryAsync<TEntity1, TEntity2>(SqlQuery query);

        IList<(TEntity1, TEntity2, TEntity3)> Query<TEntity1, TEntity2, TEntity3>(SqlQuery query);
        Task<IList<(TEntity1, TEntity2, TEntity3)>> QueryAsync<TEntity1, TEntity2, TEntity3>(SqlQuery query);

        IList<(TEntity1, TEntity2, TEntity3, TEntity4)> Query<TEntity1, TEntity2, TEntity3, TEntity4>(SqlQuery query);
        Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4>(SqlQuery query);

        IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(SqlQuery query);
        Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5>(SqlQuery query);

        IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(SqlQuery query);
        Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(SqlQuery query);

        IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)> Query<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(SqlQuery query);
        Task<IList<(TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7)>> QueryAsync<TEntity1, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(SqlQuery query);




        int Update<TEntity>(TEntity entity, params string[] updatedFields);
        Task<int> UpdateAsync<TEntity>(TEntity entity, params string[] updatedFields);

        int Insert<TEntity>(TEntity entity, params string[] insertFields);
        Task<int> InsertAsync<TEntity>(TEntity entity, params string[] insertFields);

        int Upsert<TEntity>(TEntity entity, string[] updatedFields, string[] insertFields);
        Task<int> UpsertAsync<TEntity>(TEntity entity, string[] updatedFields, string[] insertFields);

        int Delete<TEntity>(TEntity entity);
        Task<int> DeleteAsync<TEntity>(TEntity entity);

        int BatchUpdate<TEntity>(IEnumerable<TEntity> entityList, params string[] updatedFields);
        Task<int> BatchUpdateAsync<TEntity>(IEnumerable<TEntity> entityList, params string[] updatedFields);

        int BatchInsert<TEntity>(IEnumerable<TEntity> entityList, params string[] insertFields);
        Task<int> BatchInsertAsync<TEntity>(IEnumerable<TEntity> entityList, params string[] insertFields);

        int BatchUpsert<TEntity>(IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFields);
        Task<int> BatchUpsertAsync<TEntity>(IEnumerable<TEntity> entityList, string[] updatedFields, string[] insertFields);

        int BatchDelete<TEntity>(IEnumerable<TEntity> entityList);
        Task<int> BatchDeleteAsync<TEntity>(IEnumerable<TEntity> entityList);

        void BulkCopy<TEntity>(IEnumerable<TEntity> entityList);
        Task BulkCopyAsync<TEntity>(IEnumerable<TEntity> entityList);
    }
}
