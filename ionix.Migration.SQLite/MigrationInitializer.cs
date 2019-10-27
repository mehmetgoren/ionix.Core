namespace Ionix.Migration.SQLite
{
    using System;

    public class MigrationInitializer : MigrationInitializerBase
    {
        public MigrationInitializer()
        {
            this.CheckTransactionalDbAccess = false;
        }
        public MigrationInitializer(Action backup)
            : base(backup)
        {
            this.CheckTransactionalDbAccess = false;
        }

        protected override void RegisterMigrationServices()
        {
            Injector.RegisterSingleton<IMigrationService>(MigrationService.Instance);
        }
    }
}
