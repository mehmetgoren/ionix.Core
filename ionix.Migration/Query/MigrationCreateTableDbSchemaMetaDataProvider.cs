namespace ionix.Migration
{
    using ionix.Data;
    using System;
    using System.Reflection;

    //if you will need a something like donotincludemigration attribute, use this provider also.
    public sealed class MigrationCreateTableDbSchemaMetaDataProvider
        : DbSchemaMetaDataProvider
    {
        public static MigrationCreateTableDbSchemaMetaDataProvider Create(MigrationVersion migrationVersion) => new MigrationCreateTableDbSchemaMetaDataProvider(migrationVersion);

        private readonly MigrationVersion migrationVersion;
        private MigrationCreateTableDbSchemaMetaDataProvider(MigrationVersion migrationVersion)
        {
            this.migrationVersion = migrationVersion;
        }

        protected override bool IsMapped(PropertyInfo pi)
        {
            bool ret = base.IsMapped(pi);
            if (ret)
            {
                var migVer = pi.GetCustomAttribute<MigrationVersionAttribute>();
                if (null != migVer)
                {
                    if (migVer.MigrationVersion == this.migrationVersion.ToString())
                        throw new InvalidOperationException($"Both the table({pi.DeclaringType}) and the column({pi.Name}) have same migration version number.");

                    ret = false;
                }
            }

            return ret; 
        }
    }
}
