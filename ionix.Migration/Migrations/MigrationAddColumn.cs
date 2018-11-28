namespace ionix.Migration
{
    using ionix.Data;
    using ionix.Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;

    public abstract class MigrationAddColumn : Migration
    {
        protected MigrationAddColumn(MigrationVersion version)
            : base(version) { }


        protected abstract IEnumerable<Type> GetMigrationTypes();//örneğin assembly versin.

        private readonly Lazy<IMigrationService> migrationService = new Lazy<IMigrationService>(Injector.GetInstance<IMigrationService>);
        protected IMigrationService MigrationService => migrationService.Value;

        private IEnumerable<Type> GetMigrationTypesWithCheckTableAttributes() => this.GetMigrationTypes()?.Where(p => p.GetCustomAttribute<TableAttribute>() != null && p.GetCustomAttribute<MigrationVersionAttribute>() != null);

        public override SqlQuery GenerateQuery()
        {
            var migrationTypes = this.GetMigrationTypesWithCheckTableAttributes();
            if (!migrationTypes.IsEmptyList())
                return this.MigrationService.MigrationSqlQueryBuilder.AddColumn(migrationTypes, MigrationAddColumnDbSchemaMetaDataProvider.Create(this.Version), this.MigrationService.ColumnDbTypeResolver);

            return new SqlQuery();
        }

        public override void Sync(ICommandAdapter cmd)
        {
            SqlQuery query = this.GenerateQuery();
            if (null != query && !query.IsEmpty())
            {
                cmd.Factory.DataAccess.ExecuteNonQuery(this.GenerateQuery());
            }
        }
    }
}
