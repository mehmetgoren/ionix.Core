namespace ionix.Migration
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using Data;
    using System.Linq;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Reflection;

    //Migration000 ve MigrationCreateTable' ın ikisinin de buna ihtiyacı var.
    //MigrationVersionAttribute attributu' u nu Migration000' ıun da
    public abstract class MigrationCreateTable : Migration
    {
        protected MigrationCreateTable(MigrationVersion version)
            : base(version) { }


        protected abstract IEnumerable<Type> GetMigrationTypes();//örneğin assembly versin.

        private readonly Lazy<IMigrationService> migrationService = new Lazy<IMigrationService>(Injector.GetInstance<IMigrationService>);
        protected IMigrationService MigrationService => migrationService.Value;



        private IEnumerable<Type> GetMigrationTypesWithCheckTableAttributes() => this.GetMigrationTypes()?.Where(p => p.GetCustomAttribute<TableAttribute>() != null);

        public override SqlQuery GenerateQuery()
        {
            var migrationTypes = this.GetMigrationTypesWithCheckTableAttributes();
            if (!migrationTypes.IsEmptyList())
            {
                List<Type> filteredList = new List<Type>();
                foreach (Type migrationType in migrationTypes)
                {
                    bool flag = false;
                    var migrationVersionAttr = migrationType.GetCustomAttribute<MigrationVersionAttribute>();
                    if (null != migrationVersionAttr && !String.IsNullOrEmpty(migrationVersionAttr.MigrationVersion))
                    {
                        string migrationVersion = migrationVersionAttr.MigrationVersion;
                        flag = !String.IsNullOrEmpty(migrationVersion) && new MigrationVersion(migrationVersion) == this.Version;
                    }
                    if (flag)
                        filteredList.Add(migrationType);
                }

                if (!filteredList.IsEmptyList())
                {
                    return this.MigrationService.MigrationSqlQueryBuilder.CreateTable(filteredList, MigrationCreateTableDbSchemaMetaDataProvider.Create(this.Version), this.MigrationService.ColumnDbTypeResolver);
                }


                //return this.MigrationService.MigrationSqlQueryBuilder.CreateTable(migrationTypes, MigrationCreateTableDbSchemaMetaDataProvider.Create(this.Version), this.MigrationService.ColumnDbTypeResolver);
            }

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
