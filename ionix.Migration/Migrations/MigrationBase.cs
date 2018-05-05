namespace ionix.Migration
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using Data;

    public abstract class MigrationBase : Migration
    {
        protected MigrationBase(MigrationVersion version)
            : base(version) { }


        protected abstract IEnumerable<Type> GetMigrationTypes();//örneğin assembly versin.

        private readonly Lazy<IMigrationService> migrationService = new Lazy<IMigrationService>(Injector.GetInstance<IMigrationService>);
        protected IMigrationService MigrationService => migrationService.Value;

        public override SqlQuery GenerateQuery()
        {
            var migrationTypes = this.GetMigrationTypes();
            if (!migrationTypes.IsEmptyList())
                return this.MigrationService.MigrationBuilder.Build(migrationTypes, this.MigrationService.ColumnDbTypeResolver);

            return new SqlQuery();
        }

        public override void Sync(ICommandAdapter cmd)
        {
            SqlQuery query = this.GenerateQuery();
            if (null == query || query.IsEmpty())
            {
                throw new MigrationException($"{this.GetType()}.{nameof(GenerateQuery)} shouldn't returns null or empty query.");
            }

            cmd.Factory.DataAccess.ExecuteNonQuery(this.GenerateQuery());
        }

    }
}
