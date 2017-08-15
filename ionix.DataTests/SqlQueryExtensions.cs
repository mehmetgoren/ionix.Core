namespace ionix.DataTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ionix.Data;
    using System.Linq;

    [TestClass]
    public class SqlQueryExtensions
    {
        [TestMethod]
        public void ToQueryTest()
        {
            var q = "select * from Categories t where t.CategoryName like '%@0%'".ToQuery("ct");

            Assert.IsTrue(q.Parameters.First().ParameterName == "0");

            q = "select * from Categories t where t.CategoryName like '%@CategoryName%'".ToQuery2(new { CategoryName = "ct" });

            Assert.IsTrue(q.Parameters.First().ParameterName == "CategoryName");

        }
    }
}
