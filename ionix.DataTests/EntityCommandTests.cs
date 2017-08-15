namespace ionix.DataTests
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using ionix.Data;
    using ionix.Utils.Extensions;
    using ionix.DataTests.SqlServer;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections;

    [TestClass]
    public partial class EntityCommandTests
    {
        [TestMethod]
        public void SelectByIdTest()
        {
            Customers customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = client.Cmd.SelectById<Customers>("ALFKI");
            }

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void SelectByIdNonGenericTest()
        {
            Customers customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = (Customers)client.Cmd.SelectByIdNonGeneric(typeof(Customers), "ALFKI");
            }

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void QueryTest()
        {
            IList<Customers> customers = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customers = client.Cmd.Query<Customers>("select * from Customers".ToQuery());
            }

            Assert.IsNotNull(customers);
            Assert.AreNotEqual(customers.Count, 0);
        }

        [TestMethod]
        public void QueryNonGenericTest()
        {
            IList<Customers> customers = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customers = (IList<Customers>)client.Cmd.QueryNonGeneric(typeof(Customers), "select * from Customers".ToQuery());
            }

            Assert.IsNotNull(customers);
            Assert.AreNotEqual(customers.Count, 0);
        }

        [TestMethod]
        public void QuerySingleTest()
        {
            Customers customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = client.Cmd.QuerySingle<Customers>("select * from Customers where CustomerID=@0".ToQuery("ANATR"));
            }

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void QuerySingleNonGenericTest()
        {
            Customers customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = (Customers)client.Cmd.QuerySingleNonGeneric(typeof(Customers), "select * from Customers where CustomerID=@0".ToQuery("WOLZA"));
            }

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void SelectSingleTest()
        {
            Customers customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = client.Cmd.SelectSingle<Customers>(" where CustomerID=@0".ToQuery("ANATR"));
            }

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void SelectSingleNonGenericTest()
        {
            Customers customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = (Customers)client.Cmd.SelectSingleNonGeneric(typeof(Customers), " where CustomerID=@0".ToQuery("ANATR"));
            }

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void SelectTest()
        {
            IList<Customers> customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = client.Cmd.Select<Customers>();
            }

            Assert.IsNotNull(customer);
            Assert.AreNotEqual(customer.Count, 0);
        }

        [TestMethod]
        public void SelectNonGenericTest()
        {
            IList<Customers> customer = null;
            using (var client = ionixFactory.CreateDbClient())
            {
                customer = (IList<Customers>)client.Cmd.SelectNonGeneric(typeof(Customers), null);
            }

            Assert.IsNotNull(customer);
            Assert.AreNotEqual(customer.Count, 0);
        }

        [TestMethod]
        public void UpdateTest()
        {
            const int categoryId = 8;
            Categories c = new Categories()
            {
                CategoryID = categoryId,
                CategoryName = "CategoryName",
            };
            using (var client = ionixFactory.CreateDbClient())
            {
                int affected = client.Cmd.Update(c, p => p.CategoryName);
                Assert.AreNotEqual(affected, 0);
            }

            Assert.AreEqual(c.CategoryID, categoryId);
        }

        [TestMethod]
        public void UpdateNonGenericTest()
        {
            const int categoryId = 8;
            Categories c = new Categories()
            {
                CategoryID = categoryId,
                CategoryName = "CategoryName",
            };
            using (var client = ionixFactory.CreateDbClient())
            {
                int affected = client.Cmd.UpdateNonGeneric(c, "CategoryName");
                Assert.AreNotEqual(affected, 0);
            }

            Assert.AreEqual(c.CategoryID, categoryId);
        }

        [TestMethod]
        public void InsertTest()
        {
            Categories c = new Categories()
            {
                CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                Description = Guid.NewGuid().ToString()
            };
            using (var client = ionixFactory.CreateDbClient())
            {
                int affected = client.Cmd.Insert(c);
                Assert.AreNotEqual(affected, 0);
            }

            Assert.AreNotEqual(c.CategoryID, 0);
        }

        [TestMethod]
        public void InsertNonGenericTest()
        {
            Categories c = new Categories()
            {
                CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                Description = Guid.NewGuid().ToString()
            };
            using (var client = ionixFactory.CreateDbClient())
            {
                int affected = client.Cmd.InsertNonGeneric(c);
                Assert.AreNotEqual(affected, 0);
            }

            Assert.AreNotEqual(c.CategoryID, 0);
        }

        [TestMethod]
        public void UpsertTest()
        {
            using (var client = ionixFactory.CreateDbClient())
            {
                Categories c = client.Cmd.QuerySingle<Categories>("select top 1 * from Categories".ToQuery());
                int categoryID = c.CategoryID;

                client.Cmd.Upsert(c);

                Assert.AreEqual(categoryID, c.CategoryID);

                c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int affected = client.Cmd.Upsert(c);

                Assert.AreNotEqual(affected, 0);
                Assert.AreNotEqual(c.CategoryID, 0);
                Assert.AreNotEqual(c.CategoryID, categoryID);
            }
        }

        [TestMethod]
        public void UpsertNonGenericTest()
        {
            using (var client = ionixFactory.CreateDbClient())
            {
                Categories c = (Categories)client.Cmd.QuerySingleNonGeneric(typeof(Categories), "select top 1 * from Categories".ToQuery());
                int categoryID = c.CategoryID;

                client.Cmd.UpsertNonGeneric(c, null, null);

                Assert.AreEqual(categoryID, c.CategoryID);

                c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int affected = client.Cmd.UpsertNonGeneric(c, null, null);

                Assert.AreNotEqual(affected, 0);
                Assert.AreNotEqual(c.CategoryID, 0);
                Assert.AreNotEqual(c.CategoryID, categoryID);
            }
        }

        [TestMethod]
        public void DeleteTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                Categories c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                int affected = client.Cmd.Insert(c);

                Assert.AreNotEqual(affected, 0);
                Assert.AreNotEqual(c.CategoryID, 0);

                int count2 = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());
                Assert.AreNotEqual(count, count2);

                affected = client.Cmd.Delete(c);
                Assert.AreNotEqual(affected, 0);

                count2 = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());
                Assert.AreEqual(count, count2);

                client.Commit();
            }
        }

        [TestMethod]
        public void DeleteNonGenericTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                Categories c = new Categories()
                {
                    CategoryName = Guid.NewGuid().ToString().Substring(0, 10),
                    Description = Guid.NewGuid().ToString()
                };

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                int affected = client.Cmd.InsertNonGeneric(c);

                Assert.AreNotEqual(affected, 0);
                Assert.AreNotEqual(c.CategoryID, 0);

                int count2 = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());
                Assert.AreNotEqual(count, count2);

                affected = client.Cmd.DeleteNonGeneric(c);
                Assert.AreNotEqual(affected, 0);

                count2 = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());
                Assert.AreEqual(count, count2);

                client.Commit();
            }
        }

        [TestMethod]
        public void MultipleQuerySingleTests()
        {
            using (var c = ionixFactory.CreateDbClient())
            {
                var cmd = c.Cmd;// new EntityCommandSelect(c.DataAccess, '@');

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

                Assert.IsNotNull(order3);
            }
        }

        [TestMethod]
        public void MultipleQueryTests()
        {
            using (var c = ionixFactory.CreateDbClient())
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

                Assert.IsNotNull(order);
            }
        }


        [TestMethod]
        public void QuerySingleEntityPrimitiveTest()
        {
            using (var c = ionixFactory.CreateDbClient())
            {
                object result = c.Cmd.QuerySingle<Customers>("select top 1 * from Customers".ToQuery());

                Assert.IsTrue(result.GetType() == typeof(Customers));

                result = c.Cmd.QuerySingle<int>("select top 1 CategoryID from Categories".ToQuery());

                Assert.IsTrue(result.GetType() == typeof(int));

                result = c.Cmd.QuerySingle<string>("select top 1 CategoryName from Categories".ToQuery());

                Assert.IsTrue(result.GetType() == typeof(string));
            }
        }


        [TestMethod]
        public void QueryEntityPrimitiveTest()
        {
            using (var c = ionixFactory.CreateDbClient())
            {
                IEnumerable result = c.Cmd.Query<Customers>("select * from Customers".ToQuery());

                Assert.IsTrue(result.GetType() == typeof(List<Customers>));

                result = c.Cmd.Query<int>("select CategoryID from Categories".ToQuery());

                Assert.IsTrue(result.GetType() == typeof(List<int>));

                result = c.Cmd.Query<string>("select CategoryName from Categories".ToQuery());

                Assert.IsTrue(result.GetType() == typeof(List<string>));
            }
        }

        [TestMethod]
        public void DynamicQuerySingleTest()
        {
            using (var c = ionixFactory.CreateDbClient())
            {
                var result = c.Cmd.QuerySingle<dynamic>("select top 1 * from Customers".ToQuery());
                var result2 = c.Cmd.QuerySingle<ExpandoObject>("select top 1 * from Customers".ToQuery());

                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void DynamicQueryTest()
        {
            using (var c = ionixFactory.CreateDbClient())
            {
                var result = c.Cmd.Query<dynamic>("select * from Customers".ToQuery());
                var result2 = c.Cmd.Query<ExpandoObject>("select * from Customers".ToQuery());

                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        public void BulkCopyTest()
        {
            const int length = 100;
            List<Region> list = new List<Region>(length);
            for (int j = 0; j < length; ++j)
                list.Add(new Region() { RegionID = 1000 + j, RegionDescription = "dl" + j });

            using (var c = ionixFactory.CreateDbClient())
            {
                int count = c.Cmd.QuerySingle<int>("select count(*) from Region".ToQuery());

                c.Cmd.BulkCopy(list);

                int count2 = c.Cmd.QuerySingle<int>("select count(*) from Region".ToQuery());

                Assert.AreEqual(count + length, count2);

                int affected = c.Cmd.QuerySingle<int>("delete from Region where RegionID>@0".ToQuery(4));
                int count3 = c.Cmd.QuerySingle<int>("select count(*) from Region".ToQuery());

                Assert.AreEqual(count, count3);
            }
        }



        [TestMethod]
        public void BatchUpdateTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                var catagories = client.Cmd.Select<Categories>();

                client.Cmd.BatchUpdate(catagories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count, countAfter);

                client.Commit();
            }
        }

        [TestMethod]
        public void BatchUpdateNonGenericTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                var catagories = (IList<Categories>)client.Cmd.SelectNonGeneric(typeof(Categories), null);

                client.Cmd.BatchUpdateNonGeneric(catagories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count, countAfter);

                client.Commit();
            }
        }

        private const int BatchListLength = 3;

        [TestMethod]
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

            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                int count = client.Cmd.QuerySingle<int>("select count(*) from Territories".ToQuery());

                client.Cmd.BatchInsert(territories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Territories".ToQuery());

                Assert.AreEqual(count + territories.Count, countAfter);

                client.Commit();
            }
        }

        [TestMethod]
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

            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                int count = client.Cmd.QuerySingle<int>("select count(*) from Territories".ToQuery());

                client.Cmd.BatchInsertNonGeneric(territories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Territories".ToQuery());

                Assert.AreEqual(count + territories.Count, countAfter);

                client.Commit();
            }
        }


        [TestMethod]
        public void BatchInsertIdentityTest()
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
                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchInsert(categories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count + categories.Count, countAfter);

                foreach (Categories category in categories)
                {
                    Assert.AreNotEqual(category.CategoryID, 0);
                }

                client.Commit();
            }
        }

        [TestMethod]
        public void BatchInsertIdentityNonGenericTest()
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
                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchInsertNonGeneric(categories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count + categories.Count, countAfter);

                foreach (Categories category in categories)
                {
                    Assert.AreNotEqual(category.CategoryID, 0);
                }

                client.Commit();
            }
        }

        [TestMethod]
        public void BatchUpsertTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                var categories = client.Cmd.Query<Categories>("select top 3 * from Categories".ToQuery());

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsert(categories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count, countAfter);

                client.Commit();
            }
        }

        [TestMethod]
        public void BatchUpsertNonGenericTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                var categories = (IList<Categories>)client.Cmd.QueryNonGeneric(typeof(Categories), "select top 3 * from Categories".ToQuery());

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsertNonGeneric(categories, null, null);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count, countAfter);

                client.Commit();
            }
        }


        [TestMethod]
        public void BatchUpsertIdentityTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                var categories = client.Cmd.Query<Categories>("select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsert(categories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count + categories.Count, countAfter);

                foreach (Categories category in categories)
                {
                    Assert.AreNotEqual(category.CategoryID, 0);
                }

                client.Commit();
            }
        }

        [TestMethod]
        public void BatchUpsertIdentityNonGenericTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                var categories = (IList<Categories>)client.Cmd.QueryNonGeneric(typeof(Categories), "select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsertNonGeneric(categories, null, null);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count + categories.Count, countAfter);

                foreach (Categories category in categories)
                {
                    Assert.AreNotEqual(category.CategoryID, 0);
                }

                client.Commit();
            }
        }


        [TestMethod]
        public void BatchDeleteTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                var categories = client.Cmd.Query<Categories>("select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsert(categories);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count + categories.Count, countAfter);

                foreach (Categories category in categories)
                {
                    Assert.AreNotEqual(category.CategoryID, 0);
                }

                client.Cmd.BatchDelete(categories);

                int countAfterDelete = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count, countAfterDelete);

                client.Commit();
            }
        }

        [TestMethod]
        public void BatchDeleteNonGenericTest()
        {
            using (var client = ionixFactory.CreateTransactionalDbClient())
            {
                var categories = (IList<Categories>)client.Cmd.QueryNonGeneric(typeof(Categories), "select top 3 * from Categories".ToQuery());

                categories.ForEach((item) => item.CategoryID = -1);

                int count = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                client.Cmd.BatchUpsertNonGeneric(categories, null, null);

                int countAfter = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count + categories.Count, countAfter);

                foreach (Categories category in categories)
                {
                    Assert.AreNotEqual(category.CategoryID, 0);
                }

                client.Cmd.BatchDeleteNonGeneric(categories);

                int countAfterDelete = client.Cmd.QuerySingle<int>("select count(*) from Categories".ToQuery());

                Assert.AreEqual(count, countAfterDelete);

                client.Commit();
            }
        }
    }
}
