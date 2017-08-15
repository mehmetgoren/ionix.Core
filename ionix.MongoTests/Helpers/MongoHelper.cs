namespace ionix.MongoTests
{
    using ionix.Data.Mongo.Migration;
    using System;
    using System.Reflection;

    //Todo: Index için Migration sınıfları ekle.
    public static class MongoHelper
    {
        public static bool InitializeMongo(Assembly asm, string connectionString, string databaseName)
        {
            if (null != asm && !String.IsNullOrEmpty(connectionString) && !String.IsNullOrEmpty(databaseName))
            {
                var runner = new MigrationRunner(connectionString, databaseName);

                runner.MigrationLocator.LookForMigrationsInAssembly(asm);
                // runner.MigrationLocator.LookForMigrationsInAssemblyOfType<Migration1>();

                runner.DatabaseStatus.ValidateMigrationsVersions();

                runner.UpdateToLatest();
                return true;
            }

            return false;
        }
    }
}
