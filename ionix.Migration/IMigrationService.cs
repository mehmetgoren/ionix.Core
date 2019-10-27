namespace Ionix.Migration
{
    using System.Collections.Generic;
    using Data;

    public interface IMigrationService
    {
        IMigrationSqlQueryBuilder MigrationSqlQueryBuilder { get; }

        IColumnDbTypeResolver ColumnDbTypeResolver { get; }

        bool IsDatabaseVersionTableCreated(ICommandAdapter cmd);
        IEnumerable<DatabaseVersionBase> GetDatabaseVersionList(ICommandAdapter cmd);
        DatabaseVersionBase GetLastDatabaseVersion(ICommandAdapter cmd);


        DatabaseVersionBase CreateDatabaseVersion();
    }
}
