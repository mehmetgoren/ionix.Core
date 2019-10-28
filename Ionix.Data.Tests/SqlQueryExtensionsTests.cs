namespace Ionix.Data.Tests
{
    using FluentAssertions;
    using Ionix.Data;
    using System.Linq;
    using Xunit;

    public class SqlQueryExtensionsTests
    {
        [Fact]
        public void ToQueryTest()
        {
            var q = "select * from Categories t where t.CategoryName like '%@0%'".ToQuery("ct");

            q.Parameters.First().ParameterName.Should().Be("0");

            q = "select * from Categories t where t.CategoryName like '%@CategoryName%'".ToQuery2(new { CategoryName = "ct" });

            q.Parameters.First().ParameterName.Should().Be("CategoryName");
        }
    }
}
