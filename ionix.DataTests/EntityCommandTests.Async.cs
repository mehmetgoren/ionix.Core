namespace ionix.DataTests
{
    using ionix.DataTests.SqlServer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;
    using ionix.Data;
    using ionix.Utils.Extensions;
    using System.Collections.Generic;
    using System;

    partial class EntityCommandTests
    {
        [TestMethod]
        public async Task SelectByIdAsyncTest()
        {
            Customers customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = await client.Cmd.SelectByIdAsync<Customers>("ALFKI");
            }

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public async Task SelectSingleAsyncTest()
        {
            Customers customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = await client.Cmd.SelectSingleAsync<Customers>(" where CustomerID=@0".ToQuery("ANATR"));
            }

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public async Task SelectAsyncTest()
        {
            IList<Customers> customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = await client.Cmd.SelectAsync<Customers>();
            }

            Assert.IsNotNull(customer);
            Assert.AreNotEqual(customer.Count, 0);
        }

        [TestMethod]
        public async Task QuerySingleAsyncTest()
        {
            Customers customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = await client.Cmd.QuerySingleAsync<Customers>("select * from Customers where CustomerID=@0".ToQuery("ANATR"));
            }

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public async Task QueryTestAsync()
        {
            IList<Customers> customers = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customers = await client.Cmd.QueryAsync<Customers>("select * from Customers".ToQuery());
            }

            Assert.IsNotNull(customers);
            Assert.AreNotEqual(customers.Count, 0);
        }

        [TestMethod]
        public async Task BulkCopyAsyncTest()
        {
            const int length = 100;
            List<Region> list = new List<Region>(length);
            for (int j = 0; j < length; ++j)
                list.Add(new Region() { RegionID = 1000 + j, RegionDescription = "dl" + j });

            using (var c = ionixFactory.CreateDbClient())
            {
                int count = await c.Cmd.QuerySingleAsync<int>("select count(*) from Region".ToQuery());

                await c.Cmd.BulkCopyAsync(list);

                int count2 = await c.Cmd.QuerySingleAsync<int>("select count(*) from Region".ToQuery());

                Assert.AreEqual(count + length, count2);

                int affected = await c.Cmd.QuerySingleAsync<int>("delete from Region where RegionID>@0".ToQuery(4));
                int count3 = await c.Cmd.QuerySingleAsync<int>("select count(*) from Region".ToQuery());

                Assert.AreEqual(count, count3);
            }
        }

        [TestMethod]
        public async Task UpdateAsyncTest()
        {
            const int categoryId = 8;
            Categories c = new Categories()
            {
                CategoryID = categoryId,
                CategoryName = "Async",
            };
            using (var client = ionixFactory.CreateDbClient())
            {
                int affected = await client.Cmd.UpdateAsync(c, p => p.CategoryName);
                Assert.AreNotEqual(affected, 0);
            }

            Assert.AreEqual(c.CategoryID, categoryId);
        }

        [TestMethod]
        public async Task InsertAsyncTest()
        {
            Categories c = new Categories()
            {
                CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                Description = Guid.NewGuid().ToString()
            };
            using (var client = ionixFactory.CreateDbClient())
            {
                int affected = await client.Cmd.InsertAsync(c);
                Assert.AreNotEqual(affected, 0);
            }

            Assert.AreNotEqual(c.CategoryID, 0);
        }

        [TestMethod]
        public async Task UpsertAsyncTest()
        {
            using (var client = ionixFactory.CreateDbClient())
            {
                Categories c = await client.Cmd.QuerySingleAsync<Categories>("select top 1 * from Categories".ToQuery());
                int categoryID = c.CategoryID;

                await client.Cmd.UpsertAsync(c);

                Assert.AreEqual(categoryID, c.CategoryID);

                c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int affected = await client.Cmd.UpsertAsync(c);

                Assert.AreNotEqual(affected, 0);
                Assert.AreNotEqual(c.CategoryID, 0);
                Assert.AreNotEqual(c.CategoryID, categoryID);
            }
        }

        [TestMethod]
        public async Task DeleteAsyncTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                Categories c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                int affected = await client.Cmd.InsertAsync(c);

                Assert.AreNotEqual(affected, 0);
                Assert.AreNotEqual(c.CategoryID, 0);

                int count2 = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());
                Assert.AreNotEqual(count, count2);

                affected = await client.Cmd.DeleteAsync(c);
                Assert.AreNotEqual(affected, 0);

                count2 = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());
                Assert.AreEqual(count, count2);

                client.Commit();
            }
        }

        [TestMethod]
        public async Task BatchUpdateAsyncTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                var catagories = await client.Cmd.SelectAsync<Categories>();

                await client.Cmd.BatchUpdateAsync(catagories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count, countAfter);

                client.Commit();
            }
        }

        [TestMethod]
        public async Task BatchInsertAsyncTest()
        {
            var territories = new List<Territories>();
            for (int j = 0; j < BatchListLength; ++j)
                territories.Add(new Territories()
                {
                    TerritoryID = Guid.NewGuid().ToString().Substring(0, 20),
                    TerritoryDescription = Guid.NewGuid().ToString(),
                    RegionID = 1
                });

            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Territories".ToQuery());

                await client.Cmd.BatchInsertAsync(territories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Territories".ToQuery());

                Assert.AreEqual(count + territories.Count, countAfter);

                client.Commit();
            }
        }

        [TestMethod]
        public async Task BatchInsertIdentityAsyncTest()
        {
            var categories = new List<Categories>();
            for (int j = 0; j < BatchListLength; ++j)
                categories.Add(new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 12),
                    Description = "İşte Bu Parametresiz"
                });

            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                await client.Cmd.BatchInsertAsync(categories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count + categories.Count, countAfter);

                foreach (Categories category in categories)
                {
                    Assert.AreNotEqual(category.CategoryID, 0);
                }

                client.Commit();
            }
        }

        [TestMethod]
        public async Task BatchUpsertAsyncTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                var categories = await client.Cmd.QueryAsync<Categories>("select top 3 * from Categories".ToQuery());

                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                await client.Cmd.BatchUpsertAsync(categories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count, countAfter);

                client.Commit();
            }
        }

        [TestMethod]
        public async Task BatchUpsertIdentityAsyncTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                var categories = await client.Cmd.QueryAsync<Categories>("select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                await client.Cmd.BatchUpsertAsync(categories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count + categories.Count, countAfter);

                foreach (Categories category in categories)
                {
                    Assert.AreNotEqual(category.CategoryID, 0);
                }

                client.Commit();
            }
        }


        [TestMethod]
        public async Task BatchDeleteAsyncTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                var categories = await client.Cmd.QueryAsync<Categories>("select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                await client.Cmd.BatchUpsertAsync(categories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count + categories.Count, countAfter);

                foreach (Categories category in categories)
                {
                    Assert.AreNotEqual(category.CategoryID, 0);
                }

                await client.Cmd.BatchDeleteAsync(categories);

                int countAfterDelete = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count, countAfterDelete);

                client.Commit();
            }
        }

      //  tüm testler debug modda incelenecek ve sql çıktılarına bakılacak, repository ler eklenecek. oracle, postgres ve sqllite kütüphaneleri eklenecek.
    }
}
