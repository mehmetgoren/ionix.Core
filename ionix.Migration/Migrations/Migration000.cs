namespace ionix.Migration
{
    using System;
    using System.Collections.Generic;
    using Data;

    public sealed class Migration000 : MigrationCreateTable
    {
        public const string VersionNo = "0.0.0";

        public Migration000() :
            base(VersionNo)
        {
        }

        public override void Sync(ICommandAdapter cmd)
        {
            DateTime start = DateTime.Now;
            base.Sync(cmd);

            var model = Injector.GetInstance<IMigrationService>().CreateDatabaseVersion();
            model.SetValuesFrom(this);
            model.StartedOn = start;
            model.CompletedOn = DateTime.Now;

            cmd.InsertNonGeneric(model);
        }

        protected override IEnumerable<Type> GetMigrationTypes()
        {
            return new[] { Injector.GetInstance<IMigrationService>().CreateDatabaseVersion().GetType() };
        }
    }
}
