namespace ionix.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

    public interface IDbAccess : IDisposable
    {
        int ExecuteNonQuery(SqlQuery query);
        Task<int> ExecuteNonQueryAsync(SqlQuery query);

        object ExecuteScalar(SqlQuery query);
        Task<object> ExecuteScalarAsync(SqlQuery query);

        IDataReader CreateDataReader(SqlQuery query, CommandBehavior behavior);
        Task<DbDataReader> CreateDataReaderAsync(SqlQuery query, CommandBehavior behavior);
    }

    public interface ITransactionalDbAccess : IDbAccess, IDbTransaction
    {

    }
}
