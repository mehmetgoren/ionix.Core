namespace Ionix.Data.Tests
{
    using Ionix.Data.Tests.SqlServer;
    using System.Threading.Tasks;
    using Ionix.Data;
    using Ionix.Utils.Extensions;
    using System.Collections.Generic;
    using System;
    using Xunit;
    using FluentAssertions;

    partial class EntityCommandTests
    {
        [Fact]
        public async Task SelectByIdAsyncTest()
        {
            Customers customer = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customer = await client.Cmd.SelectByIdAsync<Customers>("ALFKI");
            }

            customer.Should().NotBeNull();
        }

        [Fact]
        public async Task SelectSingleAsyncTest()
        {
            Customers customer = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customer = await client.Cmd.SelectSingleAsync<Customers>(" where CustomerID=@0".ToQuery("ANATR"));
            }

            customer.Should().NotBeNull();
        }

        [Fact]
        public async Task SelectAsyncTest()
        {
            IList<Customers> customers = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customers = await client.Cmd.SelectAsync<Customers>();
            }

            customers.Should().NotBeNull();
            customers.Count.Should().NotBe(0);
        }

        [Fact]
        public async Task QuerySingleAsyncTest()
        {
            Customers customer = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customer = await client.Cmd.QuerySingleAsync<Customers>("select * from Customers where CustomerID=@0".ToQuery("ANATR"));
            }

            customer.Should().NotBeNull();
        }

        [Fact]
        public async Task QueryAsyncTest()
        {
            IList<Customers> customers = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customers = await client.Cmd.QueryAsync<Customers>("select * from Customers".ToQuery());
            }

            customers.Should().NotBeNull();
            customers.Count.Should().NotBe(0);
        }

        [Fact]
        public async Task MultipleQuerySingleAsyncTest()
        {
            using (var c = IonixFactory.CreateDbClient())
            {
                var cmd = c.Cmd;// new EntityCommandSelect(c.DataAccess, '@');

                SqlQuery q = @"select top 1 o.*, c.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID".ToQuery();

                var order = await cmd.QuerySingleAsync<Orders, Customers>(q);

                q = @"select top 1 o.*, c.*, e.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID
                inner join Employees e on o.EmployeeID = e.EmployeeID".ToQuery();
                var order3 = await cmd.QuerySingleAsync<Orders, Customers, Employees>(q);

                order3.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task MultipleQueryAsyncTest()
        {
            using (var c = IonixFactory.CreateDbClient())
            {
                var cmd = c.Cmd;// new EntityCommandSelect(c.DataAccess, '@');

                var provider = new DbSchemaMetaDataProvider();

                SqlQuery q = @"select o.*, c.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID".ToQuery();

                var order = await cmd.QueryAsync<Orders, Customers>(q);

                q = @"select o.*, c.*, e.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID
                inner join Employees e on o.EmployeeID = e.EmployeeID".ToQuery();
                var order3 = await cmd.QueryAsync<Orders, Customers, Employees>(q);


                //for (int j = 0; j < 1; ++j)
                //{
                //    order3 = cmd.Query<Orders, Customers, Employees>(q);
                //}

                order3.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task BulkCopyAsyncTest()
        {
            await Task.Delay(0);
            Assert.True(true);

            //test disabled.
            //const int length = 100;
            //List<Region> list = new List<Region>(length);
            //for (int j = 0; j < length; ++j)
            //    list.Add(new Region() { RegionID = 1000 + j, RegionDescription = "dl" + j });

            //using (var c = IonixFactory.CreateDbClient())
            //{
            //    int count = await c.Cmd.QuerySingleAsync<int>("select count(*) from Region".ToQuery());

            //    await c.Cmd.BulkCopyAsync(list);

            //    int count2 = await c.Cmd.QuerySingleAsync<int>("select count(*) from Region".ToQuery());

            //    Assert.AreEqual(count + length, count2);

            //    int affected = await c.Cmd.QuerySingleAsync<int>("delete from Region where RegionID>@0".ToQuery(4));
            //    int count3 = await c.Cmd.QuerySingleAsync<int>("select count(*) from Region".ToQuery());

            //    Assert.AreEqual(count, count3);
            //}
        }

        [Fact]
        public async Task UpdateAsyncTest()
        {
            const int categoryId = 8;
            Categories c = new Categories()
            {
                CategoryID = categoryId,
                CategoryName = "Async",
            };
            using (var client = IonixFactory.CreateDbClient())
            {
                int affected = await client.Cmd.UpdateAsync(c, p => p.CategoryName);
                affected.Should().NotBe(0);
            }

            c.CategoryID.Should().Be(categoryId);
        }

        [Fact]
        public async Task InsertAsyncTest()
        {
            Categories c = new Categories()
            {
                CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                Description = Guid.NewGuid().ToString()
            };
            using (var client = IonixFactory.CreateDbClient())
            {
                int affected = await client.Cmd.InsertAsync(c);
                affected.Should().NotBe(0);
            }

            c.CategoryID.Should().NotBe(0);
        }

        [Fact]
        public async Task UpsertAsyncTest()
        {
            using (var client = IonixFactory.CreateDbClient())
            {
                Categories c = await client.Cmd.QuerySingleAsync<Categories>("select top 1 * from Categories".ToQuery());
                int categoryID = c.CategoryID;

                await client.Cmd.UpsertAsync(c);

                categoryID.Should().Be(c.CategoryID);

                c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int affected = await client.Cmd.UpsertAsync(c);

                affected.Should().NotBe(0);
                c.CategoryID.Should().NotBe(0);
                categoryID.Should().NotBe(c.CategoryID);
            }
        }

        [Fact]
        public async Task DeleteAsyncTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                Categories c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                int affected = await client.Cmd.InsertAsync(c);

                affected.Should().NotBe(0);
                c.CategoryID.Should().NotBe(0);

                int count2 = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());
                count.Should().NotBe(count2);

                affected = await client.Cmd.DeleteAsync(c);
                affected.Should().NotBe(0);

                count2 = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());
                count.Should().Be(count2);

                client.Commit();
            }
        }

        [Fact]
        public async Task BatchUpdateAsyncTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                var catagories = await client.Cmd.SelectAsync<Categories>();

                await client.Cmd.BatchUpdateAsync(catagories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                count.Should().Be(countAfter);

                client.Commit();
            }
        }

        [Fact]
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

            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Territories".ToQuery());

                await client.Cmd.BatchInsertAsync(territories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Territories".ToQuery());

                countAfter.Should().Be(count + territories.Count);

                client.Commit();
            }
        }

        [Fact]
        public async Task BatchInsertIdentityAsyncTest()
        {
            var categories = new List<Categories>();
            for (int j = 0; j < BatchListLength; ++j)
                categories.Add(new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 12),
                    Description = "İşte Bu Parametresiz"
                });

            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                await client.Cmd.BatchInsertAsync(categories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count + categories.Count);

                foreach (Categories category in categories)
                {
                    category.CategoryID.Should().NotBe(0);
                }

                client.Commit();
            }
        }

        [Fact]
        public async Task BatchUpsertAsyncTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                var categories = await client.Cmd.QueryAsync<Categories>("select top 3 * from Categories".ToQuery());

                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                await client.Cmd.BatchUpsertAsync(categories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count);

                client.Commit();
            }
        }

        [Fact]
        public async Task BatchUpsertIdentityAsyncTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                var categories = await client.Cmd.QueryAsync<Categories>("select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                await client.Cmd.BatchUpsertAsync(categories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count + categories.Count);

                foreach (Categories category in categories)
                {
                    category.CategoryID.Should().NotBe(0);
                }

                client.Commit();
            }
        }


        [Fact]
        public async Task BatchDeleteAsyncTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                var categories = await client.Cmd.QueryAsync<Categories>("select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                await client.Cmd.BatchUpsertAsync(categories);

                int countAfter = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count + categories.Count);

                foreach (Categories category in categories)
                {
                    category.CategoryID.Should().NotBe(0);
                }

                await client.Cmd.BatchDeleteAsync(categories);

                int countAfterDelete = await client.Cmd.QuerySingleAsync<int>("select count(*) from Categories".ToQuery());

                countAfterDelete.Should().Be(count);

                client.Commit();
            }
        }
    }
}
