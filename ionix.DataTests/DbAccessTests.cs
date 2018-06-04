namespace ionix.DataTests
{
    using ionix.DataTests.SqlServer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ionix.Data;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using System;

    [TestClass]
    public class DbAccessTests
    {
        [TestMethod]
        public void ExecuteNonQueryTest()
        {
            int result = -1;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = dbAccess.ExecuteNonQuery(@"update Region set RegionDescription=@RegionDescription 
                where RegionDescription=@RegionDescription".ToQuery2(new { RegionDescription = "Eastern" }));
            }

            Assert.IsTrue(result > -1);
        }

        [TestMethod]
        public async Task ExecuteNonQueryAsyncTest()
        {
            int result = -1;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = await dbAccess.ExecuteNonQueryAsync(@"update Region set RegionDescription=@RegionDescription 
                where RegionDescription=@RegionDescription".ToQuery2(new { RegionDescription = "Eastern" }));
            }

            Assert.IsTrue(result > -1);
        }

        [TestMethod]
        public void ExecuteScalarTest()
        {
            int result = 0;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = dbAccess.ExecuteScalar<int>("select top 1 RegionID from Region".ToQuery());
            }

            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public async Task ExecuteScalarAsyncTest()
        {
            int result = 0;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = await dbAccess.ExecuteScalarAsync<int>("select RegionID from Region".ToQuery());
            }

            Assert.AreNotEqual(result, 0);
        }

        [TestMethod]
        public void ExecuteScalarListTest()
        {
            IList<int> result = null;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = dbAccess.ExecuteScalarList<int>("select RegionID from Region".ToQuery());
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ExecuteScalarListAsyncTest()
        {
            IList<int> result = null;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = await dbAccess.ExecuteScalarListAsync<int>("select RegionID from Region".ToQuery());
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void QueryDataTableTest()
        {
            DataTable result = null;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = dbAccess.QueryDataTable("select * from Customers t".ToQuery());
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void QueryExpandoListTest()
        {
            IList<dynamic> result = null;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = dbAccess.Query("select * from Customers t".ToQuery());
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task QueryExpandoListAsyncTest()
        {
            IList<dynamic> result = null;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = await dbAccess.QueryAsync("select * from Customers t".ToQuery());
            }

            Assert.IsNotNull(result);
        }


        [TestMethod]
        public async Task QueryDataTableAsyncTest()
        {
            DataTable result = null;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = await dbAccess.QueryDataTableAsync("select * from Customers t".ToQuery());
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void QuerySingleTest()
        {
            dynamic result = null;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = dbAccess.QuerySingle("select * from Customers t where t.CustomerID=@0".ToQuery("ALFKI"));
            }

            Assert.IsNotNull(result);
        }


        [TestMethod]
        public async Task QuerySingleAsyncTest()
        {
            dynamic result = null;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = await dbAccess.QuerySingleAsync("select * from Customers t where t.CustomerID=@0".ToQuery("ALFKI"));
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void QueryTest()
        {
            IList<dynamic> result = null;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = dbAccess.Query("select * from Customers t".ToQuery());
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task QueryAsyncTest()
        {
            IList<dynamic> result = null;
            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                result = await dbAccess.QueryAsync("select * from Customers t".ToQuery());
            }

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void EventsTest()
        {
            string pre = null, comp = null;

            using (var dbAccess = ionixFactory.CreatDataAccess())
            {
                var derived = (DbAccess)dbAccess;

                derived.PreExecuteSql += (e) =>
                {
                    pre = e.Query.ToString();
                };

                derived.ExecuteSqlComplete += (e) =>
                {
                    comp = e.Query.ToString();
                };

                var result = dbAccess.Query("select top 1 * from Customers t".ToQuery());
            }

            Assert.AreEqual(pre, comp);
        }

        [TestMethod]
        public void TransactionTest()
        {
            string orginalValue = null, modifiedVAlue = Guid.NewGuid().ToString().PadRight(50, '0'), rollBackValue = null, commitVAlue = null;
            int id = 1;

            using (var dbAccess = ionixFactory.CreateTransactionalDataAccess())
            {
                orginalValue = dbAccess.ExecuteScalar<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));

                dbAccess.ExecuteNonQuery("update Region set RegionDescription=@0 where RegionID=@1".ToQuery(modifiedVAlue, id));
                dbAccess.Rollback();

                rollBackValue = dbAccess.ExecuteScalar<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));
            }

            using (var dbAccess = ionixFactory.CreateTransactionalDataAccess())
            {
                dbAccess.ExecuteNonQuery("update Region set RegionDescription=@0 where RegionID=@1".ToQuery(modifiedVAlue, id));
                dbAccess.Commit();

                commitVAlue = dbAccess.ExecuteScalar<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));
            }

            Assert.IsTrue(orginalValue == rollBackValue && modifiedVAlue == commitVAlue);
        }

        [TestMethod]
        public async Task TransactionAsyncTest()
        {
            string orginalValue = null, modifiedVAlue = Guid.NewGuid().ToString().PadRight(50, '0'), rollBackValue = null, commitVAlue = null;
            int id = 1;

            using (var dbAccess = ionixFactory.CreateTransactionalDataAccess())
            {
                orginalValue = await dbAccess.ExecuteScalarAsync<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));

                await dbAccess.ExecuteNonQueryAsync("update Region set RegionDescription=@0 where RegionID=@1".ToQuery(modifiedVAlue, id));
                dbAccess.Rollback();

                rollBackValue = await dbAccess.ExecuteScalarAsync<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));
            }

            using (var dbAccess = ionixFactory.CreateTransactionalDataAccess())
            {
                await dbAccess.ExecuteNonQueryAsync("update Region set RegionDescription=@0 where RegionID=@1".ToQuery(modifiedVAlue, id));
                dbAccess.Commit();

                commitVAlue = await dbAccess.ExecuteScalarAsync<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));
            }

            Assert.IsTrue(orginalValue == rollBackValue && modifiedVAlue == commitVAlue);
        }

        //[TestMethod]
        //public void AutoCloseCommandDataReaderGetDbDataReaderTest()
        //{
        //    IDataReader innerReader = null;
        //    using (var dbAccess = ionixFactory.CreateTransactionalDataAccess())
        //    {
        //        using (var reader = dbAccess.CreateDataReader("select * from Customers".ToQuery()))
        //        {
        //          // innerReader = ((AutoCloseCommandDataReader)reader).Concrete.GetData(0);
        //           innerReader = reader.GetData(0);
        //           innerReader.Dispose();
        //        }
        //    }

        //    Assert.IsNotNull(innerReader);
        //}
    }
}
