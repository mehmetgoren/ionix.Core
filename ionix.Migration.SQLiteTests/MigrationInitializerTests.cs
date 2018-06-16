using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ionix.Migration.SQLiteTests
{
    [TestClass]
    public class MigrationInitializerTests
    {
        [TestMethod]
        public void ExecuteTest()
        {
            ionixFactory.InitMigration();
        }
    }
}
