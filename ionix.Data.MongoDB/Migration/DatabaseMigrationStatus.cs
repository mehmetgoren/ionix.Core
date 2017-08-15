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
			throw new Exception("Database is not the expected version, database is at version: " + databaseVersion + ", migrations are at version: " + migrationVersion);
		}

        public void ValidateMigrationsVersions()
        {
            var dbAllMigrations = GetMigrationsApplied().AsQueryable() // in memory but this will never get big enough to matter
                .OrderBy(v => v.Version).ToList();

            var incompletedVersions = dbAllMigrations.Where(m => m.CompletedOn == null).Select(m=>m.Version).ToList();
            if (incompletedVersions.Any())
            {
                throw new MigrationException($"Migrations : {string.Join(",",incompletedVersions)} are incomplete. Please contact administrator", new InvalidOperationException());
            }

            var appAllMigrations = _Runner.MigrationLocator.GetAllMigrations().OrderBy(m => m.Version).ToList();

            if (dbAllMigrations.Count > appAllMigrations.Count)
            {
                throw new MigrationException($"The number of migrations in db ({dbAllMigrations.Count}) is higher than the migration in applications ({appAllMigrations.Count}). Migrations names : {string.Join(",",dbAllMigrations.Skip(appAllMigrations.Count).Select(m=>m.Version))}", new InvalidOperationException());
            }

            for (int i = 0; i < dbAllMigrations.Count; i++)
            {
                if (dbAllMigrations[i].Version != appAllMigrations[i].Version)
                {
                    throw new MigrationException($"Conflict of migration no {i}. Versions is differents. In db is \"{dbAllMigrations[i].Version}\" application is \"{appAllMigrations[i].Version}\".", new InvalidOperationException());
                }
                if (dbAllMigrations[i].Script != appAllMigrations[i].Script)
                {
                    throw new MigrationException($"Conflict of migration no {i}. Scripts is differents. Version: '{dbAllMigrations[i].Version}'. In db script is : \n{dbAllMigrations[i].Script}\n\nin application is:\n{appAllMigrations[i].Script}", new InvalidOperationException());
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