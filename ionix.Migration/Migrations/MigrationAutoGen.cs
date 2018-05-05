namespace ionix.Migration
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Data;

    public abstract class MigrationAutoGen : MigrationBase//it also checks version.
    {
        protected MigrationAutoGen(MigrationVersion version) 
            : base(version) { }


        public override SqlQuery GenerateQuery()
        {
            var migrationTypes = this.GetMigrationTypes();
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
                    return this.MigrationService.MigrationBuilder.Build(filteredList, this.MigrationService.ColumnDbTypeResolver);
                }

            }

            return new SqlQuery();
        }
    }
}
