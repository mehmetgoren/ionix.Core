namespace Ionix.MongoTests
{
    using Ionix.Data.Mongo.Migration;
    using MongoDB.Driver;
    using System.Reflection;

    //Todo: Index için Migration sınıfları ekle.
    public static class MongoHelper
    {
        public static bool InitializeMigration(Assembly asm, IMongoDatabase db)
        {
            if (null != asm && null != db)
            {
                var runner = new MigrationRunner(db);

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
