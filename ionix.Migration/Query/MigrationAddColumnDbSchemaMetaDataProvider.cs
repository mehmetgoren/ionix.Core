namespace ionix.Migration
{
    using ionix.Data;
    using System.Reflection;

    //if you will need a something like donotincludemigration attribute, use this provider also.
    public sealed class MigrationAddColumnDbSchemaMetaDataProvider
        : DbSchemaMetaDataProvider
    {
        public static MigrationAddColumnDbSchemaMetaDataProvider Create(MigrationVersion migrationVersion) => new MigrationAddColumnDbSchemaMetaDataProvider(migrationVersion);

        private MigrationVersion Version { get; }

        private MigrationAddColumnDbSchemaMetaDataProvider(MigrationVersion version)
        {
            this.Version = version;
            this.DoNotCache = true;
        }


        protected override bool IsMapped(PropertyInfo pi)
        {
            bool ret = base.IsMapped(pi);
            if (ret)
            {
                var attr = pi.GetCustomAttribute<MigrationVersionAttribute>();
                ret = null != attr && this.Version.ToString() == attr.MigrationVersion;
            }

            return ret;
        }
    }
}
