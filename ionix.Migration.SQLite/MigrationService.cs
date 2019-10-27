namespace Ionix.Migration.SQLite
{
    using System.Collections.Generic;
    using Data;

    public class MigrationService : IMigrationService
    {
        public static readonly MigrationService Instance = new MigrationService();
        private MigrationService()
        {

        }

        public IMigrationSqlQueryBuilder MigrationSqlQueryBuilder => SQLite.MigrationSqlQueryBuilder.Instance;

        public IColumnDbTypeResolver ColumnDbTypeResolver => SQLite.ColumnDbTypeResolver.Instance;

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
            return cmd.SelectSingle<DatabaseVersion>(" ORDER BY Version DESC LIMIT 1".ToQuery());
        }

        public bool IsDatabaseVersionTableCreated(ICommandAdapter cmd)
        {
            SqlQuery q = @"SELECT COUNT(*) FROM ( SELECT name FROM sqlite_master WHERE type='table' AND name=@0) T;".ToQuery("DatabaseVersion");

            return cmd.QuerySingle<int>(q) > 0;
        }  
    }
}

