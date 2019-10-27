namespace Ionix.Migration.PostgreSql
{
    using System.Collections.Generic;
    using Data;

    public class MigrationService : IMigrationService
    {
        public static readonly MigrationService Instance = new MigrationService();
        private MigrationService()
        {

        }

        public IMigrationSqlQueryBuilder MigrationSqlQueryBuilder => PostgreSql.MigrationSqlQueryBuilder.Instance;

        public IColumnDbTypeResolver ColumnDbTypeResolver => PostgreSql.ColumnDbTypeResolver.Instance;

        public DatabaseVersionBase CreateDatabaseVersion()
        {
            return new DatabaseVersion();
        }

        public IEnumerable<DatabaseVersionBase> GetDatabaseVersionList(ICommandAdapter cmd)
        {
            return cmd.Select<DatabaseVersion>();
        }

        public DatabaseVersionBase GetLastDatabaseVersion(ICommandAdapter cmd)
        {
            return cmd.SelectSingle<DatabaseVersion>(" ORDER BY version DESC LIMIT 1".ToQuery());
        }

        public bool IsDatabaseVersionTableCreated(ICommandAdapter cmd)
        {
            SqlQuery q = @"SELECT EXISTS (
                           SELECT 1
                           FROM   information_schema.tables 
                           WHERE  table_schema = 'public'
                           AND    table_name = 'database_version'
                           );".ToQuery();

            return cmd.QuerySingle<bool>(q);
        }  
    }
}

