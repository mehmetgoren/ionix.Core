namespace Ionix.DataTests
{
    using Ionix.DataTests.SqlServer;
    using Ionix.Data;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using System;
    using Xunit;
    using IonixIonix.DataTests.SqlServer;
    using FluentAssertions;

    public class DbAccessTests
    {
        [Fact]
        public void ExecuteNonQueryTest()
        {
            int result = -1;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = dbAccess.ExecuteNonQuery(@"update Region set RegionDescription=@RegionDescription 
                where RegionDescription=@RegionDescription".ToQuery2(new { RegionDescription = "Eastern" }));
            }

            result.Should().BeGreaterThan(-1);
        }

        [Fact]
        public async Task ExecuteNonQueryAsyncTest()
        {
            int result = -1;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = await dbAccess.ExecuteNonQueryAsync(@"update Region set RegionDescription=@RegionDescription 
                where RegionDescription=@RegionDescription".ToQuery2(new { RegionDescription = "Eastern" }));
            }

            result.Should().BeGreaterThan(-1);
        }

        [Fact]
        public void ExecuteScalarTest()
        {
            int result = 0;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = dbAccess.ExecuteScalar<int>("select top 1 RegionID from Region".ToQuery());
            }

            result.Should().NotBe(0);
        }

        [Fact]
        public async Task ExecuteScalarAsyncTest()
        {
            int result = 0;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = await dbAccess.ExecuteScalarAsync<int>("select RegionID from Region".ToQuery());
            }

            result.Should().NotBe(0);
        }

        [Fact]
        public void ExecuteScalarListTest()
        {
            IList<int> result = null;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = dbAccess.ExecuteScalarList<int>("select RegionID from Region".ToQuery());
            }

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteScalarListAsyncTest()
        {
            IList<int> result = null;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = await dbAccess.ExecuteScalarListAsync<int>("select RegionID from Region".ToQuery());
            }

            result.Should().NotBeNull();
        }

        [Fact]
        public void QueryDataTableTest()
        {
            DataTable result = null;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = dbAccess.QueryDataTable("select * from Customers t".ToQuery());
            }

            result.Should().NotBeNull();
        }

        [Fact]
        public void QueryExpandoListTest()
        {
            IList<dynamic> result = null;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = dbAccess.Query("select * from Customers t".ToQuery());
            }

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task QueryExpandoListAsyncTest()
        {
            IList<dynamic> result = null;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = await dbAccess.QueryAsync("select * from Customers t".ToQuery());
            }

            result.Should().NotBeNull();
        }


        [Fact]
        public async Task QueryDataTableAsyncTest()
        {
            DataTable result = null;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = await dbAccess.QueryDataTableAsync("select * from Customers t".ToQuery());
            }

            result.Should().NotBeNull();
        }

        [Fact]
        public void QuerySingleTest()
        {
            dynamic result = null;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = dbAccess.QuerySingle("select * from Customers t where t.CustomerID=@0".ToQuery("ALFKI"));
            }

            result.Should().NotBeNull();
        }


        [Fact]
        public async Task QuerySingleAsyncTest()
        {
            dynamic result = null;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = await dbAccess.QuerySingleAsync("select * from Customers t where t.CustomerID=@0".ToQuery("ALFKI"));
            }

            result.Should().NotBeNull();
        }

        [Fact]
        public void QueryTest()
        {
            IList<dynamic> result = null;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = dbAccess.Query("select * from Customers t".ToQuery());
            }

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task QueryAsyncTest()
        {
            IList<dynamic> result = null;
            using (var dbAccess = IonixFactory.CreatDataAccess())
            {
                result = await dbAccess.QueryAsync("select * from Customers t".ToQuery());
            }

            result.Should().NotBeNull();
        }

        [Fact]
        public void EventsTest()
        {
            string pre = null, comp = null;

            using (var dbAccess = IonixFactory.CreatDataAccess())
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

            pre.Should().NotBe(comp);
        }

        [Fact]
        public void TransactionTest()
        {
            string orginalValue = null, modifiedVAlue = Guid.NewGuid().ToString().PadRight(50, '0'), rollBackValue = null, commitVAlue = null;
            int id = 1;

            using (var dbAccess = IonixFactory.CreateTransactionalDataAccess())
            {
                orginalValue = dbAccess.ExecuteScalar<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));

                dbAccess.ExecuteNonQuery("update Region set RegionDescription=@0 where RegionID=@1".ToQuery(modifiedVAlue, id));
                dbAccess.Rollback();

                rollBackValue = dbAccess.ExecuteScalar<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));
            }

            using (var dbAccess = IonixFactory.CreateTransactionalDataAccess())
            {
                dbAccess.ExecuteNonQuery("update Region set RegionDescription=@0 where RegionID=@1".ToQuery(modifiedVAlue, id));
                dbAccess.Commit();

                commitVAlue = dbAccess.ExecuteScalar<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));
            }

            orginalValue.Should().Be(rollBackValue);
            modifiedVAlue.Should().Be(commitVAlue);
        }

        [Fact]
        public async Task TransactionAsyncTest()
        {
            string orginalValue = null, modifiedVAlue = Guid.NewGuid().ToString().PadRight(50, '0'), rollBackValue = null, commitVAlue = null;
            int id = 1;

            using (var dbAccess = IonixFactory.CreateTransactionalDataAccess())
            {
                orginalValue = await dbAccess.ExecuteScalarAsync<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));

                await dbAccess.ExecuteNonQueryAsync("update Region set RegionDescription=@0 where RegionID=@1".ToQuery(modifiedVAlue, id));
                dbAccess.Rollback();

                rollBackValue = await dbAccess.ExecuteScalarAsync<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));
            }

            using (var dbAccess = IonixFactory.CreateTransactionalDataAccess())
            {
                await dbAccess.ExecuteNonQueryAsync("update Region set RegionDescription=@0 where RegionID=@1".ToQuery(modifiedVAlue, id));
                dbAccess.Commit();

                commitVAlue = await dbAccess.ExecuteScalarAsync<string>("select t.RegionDescription from Region t where t.RegionID=@0".ToQuery(id));
            }

            orginalValue.Should().Be(rollBackValue);
            modifiedVAlue.Should().Be(commitVAlue);        }

        //[Fact]
        //public void AutoCloseCommandDataReaderGetDbDataReaderTest()
        //{
        //    IDataReader innerReader = null;
        //    using (var dbAccess = IonixFactory.CreateTransactionalDataAccess())
        //    {
        //        using (var reader = dbAccess.CreateDataReader("select * from Customers".ToQuery()))
        //        {
        //          // innerReader = ((AutoCloseCommandDataReader)reader).Concrete.GetData(0);
        //           innerReader = reader.GetData(0);
        //           innerReader.Dispose();
        //        }
        //    }

        //     innerReader.Should().NotBeNull();
        //}
    }
}
