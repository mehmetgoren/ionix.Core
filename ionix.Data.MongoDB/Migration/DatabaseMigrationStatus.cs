namespace ionix.Data.Mongo.Migration
{
	using System;
	using System.Linq;
	using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class DatabaseMigrationStatus
	{
		private readonly MigrationRunner _Runner;

		public string VersionCollectionName = "DatabaseVersion";

		public DatabaseMigrationStatus(MigrationRunner runner)
		{
			_Runner = runner;
		}

		public virtual IMongoCollection<AppliedMigration> GetMigrationsApplied()
		{
			return _Runner.Database.GetCollection<AppliedMigration>(VersionCollectionName);
		}

		public virtual bool IsNotLatestVersion()
		{
			return _Runner.MigrationLocator.LatestVersion()
			       != GetVersion();
		}

		public virtual void ThrowIfNotLatestVersion()
		{
			if (!IsNotLatestVersion())
			{
				return;
			}
			var databaseVersion = GetVersion();
			var migrationVersion = _Runner.MigrationLocator.LatestVersion();
		    throw new Exception($"Migration versions are not match. The database' s version is '{databaseVersion}', and  migration's version is '{migrationVersion }'");
        }

        public void ValidateMigrationsVersions()
        {
            var dbAllMigrations = GetMigrationsApplied().AsQueryable() 
                .OrderBy(v => v.Version).ToList();

            var incompletedVersions = dbAllMigrations.Where(m => m.CompletedOn == null).Select(m=>m.Version).ToList();
            if (incompletedVersions.Any())
            {
                throw new MigrationException($"Some Migrations : {string.Join(",",incompletedVersions)} are incomplete.");
            }

            var appAllMigrations = _Runner.MigrationLocator.GetAllMigrations().OrderBy(m => m.Version).ToList();

            if (dbAllMigrations.Count > appAllMigrations.Count)
            {
                throw new MigrationException($"the Migrations count in db ({dbAllMigrations.Count}) is higher than application migration count ({appAllMigrations.Count}). Migrations names : {string.Join(",",dbAllMigrations.Skip(appAllMigrations.Count).Select(m=>m.Version))}");
            }

            for (int i = 0; i < dbAllMigrations.Count; i++)
            {
                if (dbAllMigrations[i].Version != appAllMigrations[i].Version)
                {
                    throw new MigrationException($"A migration conflict has been detected at index: {i}. The db's version is \"{dbAllMigrations[i].Version}\" and application's version is \"{appAllMigrations[i].Version}\".");
                }
                if (dbAllMigrations[i].Script != appAllMigrations[i].Script)
                {
                    throw new MigrationException($"A migration conflict script has been detected at index: {i}. The db's version: '{dbAllMigrations[i].Version}'. and application's version is \n{dbAllMigrations[i].Script}\n\nin application is:\n{appAllMigrations[i].Script}");
                }
            }
        }

        public virtual MigrationVersion GetVersion()
		{
			var lastAppliedMigration = GetLastAppliedMigration();
			return lastAppliedMigration == null
			       	? MigrationVersion.Default()
			       	: lastAppliedMigration.Version;
		}

		public virtual AppliedMigration GetLastAppliedMigration()
		{
			return GetMigrationsApplied().AsQueryable() // in memory but this will never get big enough to matter
				.OrderByDescending(v => v.Version)
				.FirstOrDefault();
		}

		public virtual AppliedMigration StartMigration(Migration migration)
		{
			var appliedMigration = new AppliedMigration(migration);
			GetMigrationsApplied().InsertOne(appliedMigration);
			return appliedMigration;
		}

		public virtual void CompleteMigration(AppliedMigration appliedMigration)
		{
			appliedMigration.CompletedOn = DateTime.Now;
            FindOneAndReplace(appliedMigration);
           // GetMigrationsApplied().FindOneAndReplace
              //  (Builders<AppliedMigration>.Filter.Eq(p => p.Version, appliedMigration.Version), appliedMigration);
                // appliedMigration, appliedMigration);
		}

        public void FindOneAndReplace(AppliedMigration appliedMigration)
        {
            GetMigrationsApplied().FindOneAndReplace
              (Builders<AppliedMigration>.Filter.Eq(p => p.Version, appliedMigration.Version), appliedMigration);
        }
	}
}