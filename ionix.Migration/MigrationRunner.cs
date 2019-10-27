namespace Ionix.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;

    public class MigrationRunner
    {
        public MigrationRunner(ICommandAdapter cmd)
        {
            this.Cmd = cmd ?? throw new ArgumentNullException(nameof(cmd));
            this.DatabaseStatus = new DatabaseMigrationStatus(this);
            this. MigrationReflection = new MigrationReflection();
        }

        public ICommandAdapter Cmd { get; }
        public MigrationReflection MigrationReflection { get; }
        public DatabaseMigrationStatus DatabaseStatus { get; }

        private string Database
        {
            get
            {
                if (this.Cmd.Factory.DataAccess is DbAccess dbAccess)
                {
                    var conn = dbAccess.Connection;// .Client.Settings.Server;

                    return $"{conn.Database}@{conn.DataSource}"; 
                }
                return "DbAccess casting failed, could not identify server info";
            }
        }

        public void UpdateToLatest()
        {
            Console.WriteLine(WhatWeAreUpdating() + " to latest...");
            this.UpdateTo(this.MigrationReflection.LatestVersion());
        }

        private string WhatWeAreUpdating()
        {
            return $"Updating server(s) {this.Database}";
        }


        protected void ApplyMigrations(IEnumerable<Migration> migrations)
        {
            migrations.ToList()
                      .ForEach(ApplyMigration);
        }


        protected void ApplyMigration(Migration migration)
        {
            Console.WriteLine(new { Message = "Applying migration", migration.Version, migration.Description, DatabaseName = this.Database });

            var appliedMigration = this.DatabaseStatus.StartMigration(migration);
            try
            {
                migration.Sync(this.Cmd);
            }
            catch (Exception exception)
            {
                appliedMigration.Exception = exception.ToString();

                try
                {
                    this.DatabaseStatus.FindOneAndReplace(appliedMigration);
                }
                catch (Exception exn)
                {
                    Console.WriteLine(exn);
                }

                OnMigrationException(migration, exception);

            }
            this.DatabaseStatus.CompleteMigration(appliedMigration);
        }

        protected void OnMigrationException(Migration migration, Exception exception)
        {
            var message = new
            {
                Message = "Migration failed to be applied: " + exception.Message,
                migration.Version,
                Name = migration.GetType(),
                migration.Description,
                DatabaseName = this.Database
            };
            Console.WriteLine(message);
            throw new MigrationException(message.ToString(), exception);
        }

        public void UpdateTo(MigrationVersion updateToVersion)
        {
            var currentVersion = this.DatabaseStatus.GetLastGetDatabseVersions();
            Console.WriteLine(new
            {
                Message = WhatWeAreUpdating(),
                currentVersion,
                updateToVersion,
                DatabaseName = this.Database
            });

            var migrations = this.MigrationReflection.GetMigrationsAfter(currentVersion)
                                             .Where(m => m.Version <= updateToVersion);

            ApplyMigrations(migrations);
        }
    }
}
