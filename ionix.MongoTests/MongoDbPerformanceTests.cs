//namespace ionix.MongoTests
//{
//    using MongoDB.Driver;
//    using MongoDB.Driver.Linq;
//    using ionix.Data.MongoDB;
//    using Microsoft.VisualStudio.TestTools.UnitTesting;

//    [TestClass]
//    public class MongoDbPerformanceTests
//    {
//        static MongoDbPerformanceTests()
//        {
//            MongoClientProxy.SetConnectionString(MongoTests.MongoAddress);
//        }

//        [TestMethod]
//        public void InitMongoDbPerformanceTests()
//        {
//            long cout = Mongo.Cmd.Count<Person>();
//        }

//        [TestMethod]
//        public void IndexContainsSearchTest()
//        {
//            var result = Mongo.Cmd.AsQueryable<Person>().Where(p => p.Name.Contains("bbbb")).ToList();

//            Assert.IsNotNull(result);
//        }
//    }
//}
