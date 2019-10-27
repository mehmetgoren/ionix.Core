namespace Ionix.Data.PostgreSql.BulkCopy
{
    using Npgsql;

    public class CommandFactoryWithBulkCopy : CommandFactory
    {
        public CommandFactoryWithBulkCopy(IDbAccess dataAccess)
            : base(dataAccess)
        {
        }

        public override IBulkCopyCommand CreateBulkCopyCommand()
        {
            DbAccess dbAccess = (DbAccess)base.DataAccess;
            NpgsqlConnection conn = (NpgsqlConnection)dbAccess.Connection;
            return new BulkCopyCommand(conn);
        }
    }
}
