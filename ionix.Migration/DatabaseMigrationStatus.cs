namespace ionix.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;

    public class DatabaseMigrationStatus
    {
        private readonly MigrationRunner runner;


        public DatabaseMigrationStatus(MigrationRunner runner)
        {
            this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        public IEnumerable<DatabaseVersionBase> GetDatabseVersions()//İlk migration tablosu burada oluşyor.
        {
            IMigrationService service = Injector.GetInstance<IMigrationService>();
            if (!service.IsDatabaseVersionTableCreated(this.runner.Cmd))
            {
                new Migration000().Sync(this.runner.Cmd);
            }

            return service.GetDatabaseVersionList(this.runner.Cmd).OrderBy(p => p.Version);
        }

        public DatabaseVersionBase GetLastGetDatabseVersions()
        {
            IMigrationService service = Injector.GetInstance<IMigrationService>();
            return service.GetLastDatabaseVersion(this.runner.Cmd);
        }

        public bool IsNotLatestVersion()
        {
            return this.runner.MigrationReflection.LatestVersion()
                   != GetVersion();
        }

        public void ThrowIfNotLatestVersion()
        {
            if (!this.IsNotLatestVersion())
                return;

            var databaseVersion = this.GetVersion();
            var migrationVersion = this.runner.MigrationReflection.LatestVersion();
            throw new Exception("Database is not the expected version, database is at version: " + databaseVersion + ", migrations are at version: " + migrationVersion);
        }

        public void ValidateMigrationsVersions()
        {
            var dbAllMigrations = this.GetDatabseVersions().Where(p => p.Version != Migration000.VersionNo).ToList();//.AsQueryable() // in memory but this will never get big enough to matter
               // .OrderBy(v => v.Version).ToList();

            var incompletedVersions = dbAllMigrations.Where(p => p.Version != Migration000.VersionNo && p.CompletedOn == null).Select(m => m.Version).ToList();
            if (incompletedVersions.Any())
            {
                throw new MigrationException($"Migrations : {string.Join(",", incompletedVersions)} are incomplete. Please contact administrator. (means you' ve been fucked)");
            }

            var appAllMigrations = this.runner.MigrationReflection.GetAllMigrations().OrderBy(m => m.Version).ToList();

            if (dbAllMigrations.Count > appAllMigrations.Count)
            {
                throw new MigrationException($"The number of migrations in db ({dbAllMigrations.Count}) is higher than the migration in applications ({appAllMigrations.Count}). Migrations names : {string.Join(",", dbAllMigrations.Skip(appAllMigrations.Count).Select(m => m.Version))}");
            }

            for (int i = 0; i < dbAllMigrations.Count; i++)
            {
                if (dbAllMigrations[i].Version != appAllMigrations[i].Version)
                {
                    throw new MigrationException($"Conflict script has been detected! at {i}. Versions is different. The Db' script is : \"{dbAllMigrations[i].Version}\" and the application's is \"{appAllMigrations[i].Version}\".");
                }
                if (dbAllMigrations[i].Script != appAllMigrations[i].GenerateQuery().ToString())
                {
                    throw new MigrationException($"Conflict script has been detected! at {i}.  Version: '{dbAllMigrations[i].Version}'. The Db' script is : \n{dbAllMigrations[i].Script}\n\nin and the application's is:\n{appAllMigrations[i].GenerateQuery()}");
                }
            }
        }

        public virtual MigrationVersion GetVersion()
        {
            var lastAppliedMigration = this.GetLastGetDatabseVersions();
            return lastAppliedMigration == null
                       ? MigrationVersion.Default()
                       : new MigrationVersion(lastAppliedMigration.Version);
        }

        public DatabaseVersionBase StartMigration(Migration migration)
        {
            var appliedMigration = Injector.GetInstance<IMigrationService>().CreateDatabaseVersion();//new DatabaseVersion(migration);
            appliedMigration.SetValuesFrom(migration);

            appliedMigration.StartedOn = DateTime.Now;
            this.runner.Cmd.InsertNonGeneric(appliedMigration);//Eğer sıkıntı çıkarsa service e taşı.
            return appliedMigration;
        }

        public void CompleteMigration(DatabaseVersionBase appliedMigration)
        {
            appliedMigration.CompletedOn = DateTime.Now;
            this.FindOneAndReplace(appliedMigration);
        }

        public void FindOneAndReplace(DatabaseVersionBase appliedMigration)
        {
            this.runner.Cmd.UpdateNonGeneric(appliedMigration);//Eğer sıkıntı çıkarsa service e taşı.
        }
    }
}
