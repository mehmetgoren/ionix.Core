using FluentAssertions;
using Xunit;

namespace Ionix.Migration.SQLiteTests
{
    public class MigrationInitializerTests
    {
        [Fact]
        public void ExecuteTest()
        {
            IonixFactory.InitMigration(false);
            true.Should().BeTrue();
        }
    }
}
