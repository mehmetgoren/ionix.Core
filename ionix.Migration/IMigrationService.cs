namespace ionix.Migration
{
    using System.Collections.Generic;
    using Data;

    public interface IMigrationService
    {
        IMigrationBuilder MigrationBuilder { get; }

        IColumnDbTypeResolver ColumnDbTypeResolver { get; }

        bool IsDatabaseVersionTableCreated(ICommandAdapter cmd);
        IEnumerable<DatabaseVersionBase> GetDatabaseVersionList(ICommandAdapter cmd);
        DatabaseVersionBase GetLastDatabaseVersion(ICommandAdapter cmd);


        DatabaseVersionBase CreateDatabaseVersion();
    }
}
