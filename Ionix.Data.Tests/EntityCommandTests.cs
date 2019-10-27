namespace Ionix.DataTests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using Ionix.Data;
    using Ionix.Utils.Extensions;
    using Ionix.DataTests.SqlServer;
    using System.Collections;
    using IonixIonix.DataTests.SqlServer;
    using Xunit;
    using FluentAssertions;

    public partial class EntityCommandTests
    {
        [Fact]
        public void SelectByIdTest()
        {
            Customers customer = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customer = client.Cmd.SelectById<Customers>("ALFKI");
            }

            customer.Should().NotBeNull();
        }

        [Fact]
        public void SelectByIdNonGenericTest()
        {
            Customers customer = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customer = (Customers)client.Cmd.SelectByIdNonGeneric(typeof(Customers), "ALFKI");
            }

            customer.Should().NotBeNull();
        }

        [Fact]
        public void QueryTest()
        {
            IList<Customers> customers = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customers = client.Cmd.Query<Customers>("select * from Customers".ToQuery());
            }

            customers.Should().NotBeNull();
            customers.Count.Should().NotBe(0);
        }

        [Fact]
        public void QueryNonGenericTest()
        {
            IList<Customers> customers = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customers = (IList<Customers>)client.Cmd.QueryNonGeneric(typeof(Customers), "select * from Customers".ToQuery());
            }

            customers.Should().NotBeNull();
            customers.Count.Should().NotBe(0);
        }

        [Fact]
        public void QuerySingleTest()
        {
            Customers customer = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customer = client.Cmd.QuerySingle<Customers>("select * from Customers where CustomerID=@0".ToQuery("ANATR"));
            }

            customer.Should().NotBeNull();
        }

        [Fact]
        public void QuerySingleNonGenericTest()
        {
            Customers customer = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customer = (Customers)client.Cmd.QuerySingleNonGeneric(typeof(Customers), "select * from Customers where CustomerID=@0".ToQuery("WOLZA"));
            }

            customer.Should().NotBeNull();
        }

        [Fact]
        public void SelectSingleTest()
        {
            Customers customer = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customer = client.Cmd.SelectSingle<Customers>(" where CustomerID=@0".ToQuery("ANATR"));
            }

            customer.Should().NotBeNull();
        }

        [Fact]
        public void SelectSingleNonGenericTest()
        {
            Customers customer = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customer = (Customers)client.Cmd.SelectSingleNonGeneric(typeof(Customers), " where CustomerID=@0".ToQuery("ANATR"));
            }

            customer.Should().NotBeNull();
        }

        [Fact]
        public void SelectTest()
        {
            IList<Customers> customers = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customers = client.Cmd.Select<Customers>();
            }

            customers.Should().NotBeNull();
            customers.Count.Should().NotBe(0);
        }

        [Fact]
        public void SelectNonGenericTest()
        {
            IList<Customers> customers = null;
            using (var client = IonixFactory.CreateDbClient())
            {
                customers = (IList<Customers>)client.Cmd.SelectNonGeneric(typeof(Customers), null);
            }

            customers.Should().NotBeNull();
            customers.Count.Should().NotBe(0);
        }

        [Fact]
        public void UpdateTest()
        {
            const int categoryId = 8;
            Categories c = new Categories()
            {
                CategoryID = categoryId,
                CategoryName = "CategoryName",
            };
            using (var client = IonixFactory.CreateDbClient())
            {
                int affected = client.Cmd.Update(c, p => p.CategoryName);
                affected.Should().NotBe(0);
            }

            c.CategoryID.Should().Be(categoryId);
        }

        [Fact]
        public void UpdateNonGenericTest()
        {
            const int categoryId = 8;
            Categories c = new Categories()
            {
                CategoryID = categoryId,
                CategoryName = "CategoryName",
            };
            using (var client = IonixFactory.CreateDbClient())
            {
                int affected = client.Cmd.UpdateNonGeneric(c, "CategoryName");
                affected.Should().NotBe(0);
            }

            c.CategoryID.Should().Be(categoryId);
        }

        [Fact]
        public void InsertTest()
        {
            Categories c = new Categories()
            {
                CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                Description = Guid.NewGuid().ToString()
            };
            using (var client = IonixFactory.CreateDbClient())
            {
                int affected = client.Cmd.Insert(c);
                affected.Should().NotBe(0);
            }

            c.CategoryID.Should().NotBe(0);
        }

        [Fact]
        public void InsertNonGenericTest()
        {
            Categories c = new Categories()
            {
                CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                Description = Guid.NewGuid().ToString()
            };
            using (var client = IonixFactory.CreateDbClient())
            {
                int affected = client.Cmd.InsertNonGeneric(c);
                affected.Should().NotBe(0);
            }

            c.CategoryID.Should().NotBe(0);
        }

        [Fact]
        public void UpsertTest()
        {
            using (var client = IonixFactory.CreateDbClient())
            {
                Categories c = client.Cmd.QuerySingle<Categories>("select top 1 * from Categories".ToQuery());
                int categoryID = c.CategoryID;

                client.Cmd.Upsert(c);

                categoryID.Should().Be(c.CategoryID);

                c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int affected = client.Cmd.Upsert(c);

                affected.Should().NotBe(0);
                c.CategoryID.Should().NotBe(0);
                c.CategoryID.Should().NotBe(categoryID);
            }
        }

        [Fact]
        public void UpsertNonGenericTest()
        {
            using (var client = IonixFactory.CreateDbClient())
            {
                Categories c = (Categories)client.Cmd.QuerySingleNonGeneric(typeof(Categories), "select top 1 * from Categories".ToQuery());
                int categoryID = c.CategoryID;

                client.Cmd.UpsertNonGeneric(c, null, null);

                categoryID.Should().Be(c.CategoryID);

                c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int affected = client.Cmd.UpsertNonGeneric(c, null, null);

                affected.Should().NotBe(0);
                c.CategoryID.Should().NotBe(0);
                c.CategoryID.Should().NotBe(categoryID);
            }
        }

        [Fact]
        public void DeleteTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                Categories c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                int affected = client.Cmd.Insert(c);

                affected.Should().NotBe(0);
                c.CategoryID.Should().NotBe(0);

                int count2 = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());
                count.Should().NotBe(count2);

                affected = client.Cmd.Delete(c);
                affected.Should().NotBe(0);

                count2 = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());
                count.Should().Be(count2);

                client.Commit();
            }
        }

        [Fact]
        public void DeleteNonGenericTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                Categories c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                int affected = client.Cmd.InsertNonGeneric(c);

                affected.Should().NotBe(0);
                c.CategoryID.Should().NotBe(0);

                int count2 = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());
                count.Should().NotBe(count2);

                affected = client.Cmd.DeleteNonGeneric(c);
                affected.Should().NotBe(0);

                count2 = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());
                count.Should().Be(count2);

                client.Commit();
            }
        }

        [Fact]
        public void MultipleQuerySingleTest()
        {
            using (var c = IonixFactory.CreateDbClient())
            {
                var cmd = c.Cmd;

                SqlQuery q = @"select top 1 o.*, c.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID".ToQuery();

                var order = cmd.QuerySingle<Orders, Customers>(q);

                //for (int j = 0; j < 1; ++j)
                //{
                //    order = cmd.QuerySingle<Orders, Customers>(q);
                //}

                //order = cmd.QuerySingle<Orders, Customers>(q);


                q = @"select top 1 o.*, c.*, e.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID
                inner join Employees e on o.EmployeeID = e.EmployeeID".ToQuery();
                var order3 = cmd.QuerySingle<Orders, Customers, Employees>(q);

                order3.Should().NotBeNull();
            }
        }

        [Fact]
        public void MultipleQueryLeftJoinTest()
        {
            using (var c = IonixFactory.CreateDbClient())
            {
                var cmd = c.Cmd;

                SqlQuery q = @"select t1.*, t2.* from Employees t1
                left join Employees t2 on t1.EmployeeID=t2.ReportsTo".ToQuery();

                var order = cmd.Query<Employees, Employees>(q);

                //for (int j = 0; j < 1; ++j)
                //{
                //    order = cmd.QuerySingle<Orders, Customers>(q);
                //}

                //order = cmd.QuerySingle<Orders, Customers>(q);


                // q = @"select top 1 o.*, c.*, e.* from Orders o
                // inner join Customers c on o.CustomerID = c.CustomerID
                // inner join Employees e on o.EmployeeID = e.EmployeeID".ToQuery();
                //var order3 = cmd.QuerySingle<Orders, Customers, Employees>(q);

                order.Should().NotBeNull();
            }
        }

        [Fact]
        public void MultipleQueryTest()
        {
            using (var c = IonixFactory.CreateDbClient())
            {
                var cmd = c.Cmd;// new EntityCommandSelect(c.DataAccess, '@');

                var provider = new DbSchemaMetaDataProvider();

                SqlQuery q = @"select o.*, c.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID".ToQuery();

                var order = cmd.Query<Orders, Customers>(q);

                q = @"select o.*, c.*, e.* from Orders o
                inner join Customers c on o.CustomerID = c.CustomerID
                inner join Employees e on o.EmployeeID = e.EmployeeID".ToQuery();
                var order3 = cmd.Query<Orders, Customers, Employees>(q);


                //for (int j = 0; j < 1; ++j)
                //{
                //    order3 = cmd.Query<Orders, Customers, Employees>(q);
                //}

                order.Should().NotBeNull();
            }
        }


        [Fact]
        public void QuerySingleEntityPrimitiveTest()
        {
            using (var c = IonixFactory.CreateDbClient())
            {
                object result = c.Cmd.QuerySingle<Customers>("select top 1 * from Customers".ToQuery());

                result.GetType().Should().Be(typeof(Customers));

                result = c.Cmd.QuerySingle<int>("select top 1 CategoryID from Categories".ToQuery());

                result.GetType().Should().Be(typeof(int));

                result = c.Cmd.QuerySingle<string>("select top 1 CategoryName from Categories".ToQuery());

                result.GetType().Should().Be(typeof(string));
            }
        }


        [Fact]
        public void QueryEntityPrimitiveTest()
        {
            using (var c = IonixFactory.CreateDbClient())
            {
                IEnumerable result = c.Cmd.Query<Customers>("select * from Customers".ToQuery());

                result.GetType().Should().Be(typeof(List<Customers>));

                result = c.Cmd.Query<int>("select CategoryID from Categories".ToQuery());

                result.GetType().Should().Be(typeof(List<int>));

                result = c.Cmd.Query<string>("select CategoryName from Categories".ToQuery());

                result.GetType().Should().Be(typeof(List<string>));
            }
        }

        [Fact]
        public void DynamicQuerySingleTest()
        {
            using (var c = IonixFactory.CreateDbClient())
            {
                var result = c.Cmd.QuerySingle<dynamic>("select top 1 * from Customers".ToQuery());
                var result2 = c.Cmd.QuerySingle<ExpandoObject>("select top 1 * from Customers".ToQuery());

                Assert.NotNull(result);
                result2.Should().NotBeNull();
            }
        }

        [Fact]
        public void DynamicQueryTest()
        {
            using (var c = IonixFactory.CreateDbClient())
            {
                var result = c.Cmd.Query<dynamic>("select * from Customers".ToQuery());
                var result2 = c.Cmd.Query<ExpandoObject>("select * from Customers".ToQuery());

                Assert.NotNull(result);
                result2.Should().NotBeNull();
            }
        }

        [Fact]
        public void BulkCopyTest()
        {
            const int length = 100;
            List<Region> list = new List<Region>(length);
            for (int j = 0; j < length; ++j)
                list.Add(new Region() { RegionID = 1000 + j, RegionDescription = "dl" + j });

            using (var c = IonixFactory.CreateDbClient())
            {
                int count = c.Cmd.QuerySingle<int>("select count(*) from Region".ToQuery());

                c.Cmd.BulkCopy(list);

                int count2 = c.Cmd.QuerySingle<int>("select count(*) from Region".ToQuery());

                count2.Should().Be(count + length);

                int affected = c.Cmd.QuerySingle<int>("delete from Region where RegionID>@0".ToQuery(4));
                int count3 = c.Cmd.QuerySingle<int>("select count(*) from Region".ToQuery());
               
                count.Should().Be(count3);
            }
        }



        [Fact]
        public void BatchUpdateTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                var catagories = client.Cmd.Select<Categories>();

                client.Cmd.BatchUpdate(catagories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                count.Should().Be(countAfter);

                client.Commit();
            }
        }

        [Fact]
        public void BatchUpdateNonGenericTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                var catagories = (IList<Categories>)client.Cmd.SelectNonGeneric(typeof(Categories), null);

                client.Cmd.BatchUpdateNonGeneric(catagories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                count.Should().Be(countAfter);

                client.Commit();
            }
        }

        private const int BatchListLength = 3;

        [Fact]
        public void BatchInsertTest()
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
                int count = client.Cmd.QuerySingle<int>("select count(*) from Territories".ToQuery());

                client.Cmd.BatchInsert(territories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Territories".ToQuery());

                countAfter.Should().Be(count + territories.Count);

                client.Commit();
            }
        }

        [Fact]
        public void BatchInsertNonGenericTest()
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
                int count = client.Cmd.QuerySingle<int>("select count(*) from Territories".ToQuery());

                client.Cmd.BatchInsertNonGeneric(territories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Territories".ToQuery());

                countAfter.Should().Be(count + territories.Count);

                client.Commit();
            }
        }


        [Fact]
        public void BatchInsertIdentityTest()
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
                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchInsert(categories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count + categories.Count);

                foreach (Categories category in categories)
                {
                    category.CategoryID.Should().NotBe(0);
                }

                client.Commit();
            }
        }

        [Fact]
        public void BatchInsertIdentityNonGenericTest()
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
                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchInsertNonGeneric(categories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count + categories.Count);

                foreach (Categories category in categories)
                {
                    category.CategoryID.Should().NotBe(0);
                }

                client.Commit();
            }
        }

        [Fact]
        public void BatchUpsertTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                var categories = client.Cmd.Query<Categories>("select top 3 * from Categories".ToQuery());

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsert(categories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count);

                client.Commit();
            }
        }

        [Fact]
        public void BatchUpsertNonGenericTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                var categories = (IList<Categories>)client.Cmd.QueryNonGeneric(typeof(Categories), "select top 3 * from Categories".ToQuery());

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsertNonGeneric(categories, null, null);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count);

                client.Commit();
            }
        }


        [Fact]
        public void BatchUpsertIdentityTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                var categories = client.Cmd.Query<Categories>("select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsert(categories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count + categories.Count);

                foreach (Categories category in categories)
                {
                    category.CategoryID.Should().NotBe(0);
                }

                client.Commit();
            }
        }

        [Fact]
        public void BatchUpsertIdentityNonGenericTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                var categories = (IList<Categories>)client.Cmd.QueryNonGeneric(typeof(Categories), "select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsertNonGeneric(categories, null, null);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count + categories.Count);

                foreach (Categories category in categories)
                {
                    category.CategoryID.Should().NotBe(0);
                }

                client.Commit();
            }
        }


        [Fact]
        public void BatchDeleteTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                var categories = client.Cmd.Query<Categories>("select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsert(categories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count + categories.Count);

                foreach (Categories category in categories)
                {
                    category.CategoryID.Should().NotBe(0);
                }

                client.Cmd.BatchDelete(categories);

                int countAfterDelete = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                countAfterDelete.Should().Be(count);

                client.Commit();
            }
        }

        [Fact]
        public void BatchDeleteNonGenericTest()
        {
            using (var client = IonixFactory.CreateTransactionalDbClient())
            {
                var categories = (IList<Categories>)client.Cmd.QueryNonGeneric(typeof(Categories), "select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsertNonGeneric(categories, null, null);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                countAfter.Should().Be(count + categories.Count);

                foreach (Categories category in categories)
                {
                    category.CategoryID.Should().NotBe(0);
                }

                client.Cmd.BatchDeleteNonGeneric(categories);

                int countAfterDelete = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                countAfterDelete.Should().Be(count);

                client.Commit();
            }
        }
    }
}
