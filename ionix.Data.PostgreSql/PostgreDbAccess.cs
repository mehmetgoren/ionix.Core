namespace ionix.Data.PostgreSql
{
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

    public class PostgreDbAccess : DbAccess
    {
        public PostgreDbAccess(DbConnection connection) 
            : base(connection)
        {

        }

        public override IDataReader CreateDataReader(SqlQuery query, CommandBehavior behavior)
        {
            DbDataReader reader = (DbDataReader)base.CreateDataReader(query, behavior);

            return new PostgreDataReader(reader);
        }

        public override async Task<DbDataReader> CreateDataReaderAsync(SqlQuery query, CommandBehavior behavior)
        {
            DbDataReader reader = await  base.CreateDataReaderAsync(query, behavior);

            return new PostgreDataReader(reader);
        }
    }
}
