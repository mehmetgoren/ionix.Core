namespace Ionix.Data
{
    using System;
    using System.Data;
    using System.Threading.Tasks;

    public interface IDbAccess : IDisposable
    {
        int CommandTimeout { get; set; }

        int ExecuteNonQuery(SqlQuery query);
        Task<int> ExecuteNonQueryAsync(SqlQuery query);

        object ExecuteScalar(SqlQuery query);
        Task<object> ExecuteScalarAsync(SqlQuery query);

        AutoCloseCommandDataReader CreateDataReader(SqlQuery query, CommandBehavior behavior);
        Task<AutoCloseCommandDataReader> CreateDataReaderAsync(SqlQuery query, CommandBehavior behavior);
    }

    public interface ITransactionalDbAccess : IDbAccess, IDbTransaction
    {

    }
}
