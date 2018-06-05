ionix is a micro-orm library that based on .net standart 2.0 (which supports Linux, Windows and macOS).

From NuGet
----------
Install-Package ionix.Data


It' s very easy to use, here is some Northwind database examples:

Select Operations
-----------------

```csharp
using (var db = ionixFactory.CreateDbClient())
{
    var employee = db.Cmd.SelectById<Employees>(1);
    employee = await db.Cmd.SelectByIdAsync<Employees>(1);	
	
    var employee = dbCmd.QuerySingle<Employees>("select * from Employees where EmployeeID=@0".ToQuery(1)); 
    employee = await dbCmd.QuerySingleAsync<Employee>("select * from Employees where EmployeeID=@0".ToQuery(1));
	
    IList<Employees> employeeList = dbCmd.Query<Employees>("select * from Employees".ToQuery());
    employeeList = await dbCmd.QueryAsync<Employees>("select * from Employees".ToQuery());

    var q = @"select o.*, c.*, e.* from Orders o
              inner join Customers c on o.CustomerID = c.CustomerID
              inner join Employees e on o.EmployeeID = e.EmployeeID".ToQuery();
    var models = db.Cmd.Query<Orders, Customers, Employees>(q);
    models = await db.Cmd.QueryAsync<Orders, Customers, Employees>(q); 	
}
```

Select Operations #2
-----------------

```csharp
using (var db = ionixFactory.CreateDbClient())
{
    int regionId = db.Cmd.QuerySingle<int>("select top 1 RegionID from Region".ToQuery());
    
    IList<int> regionIds = db.Cmd.Query<int>("select RegionID from Region".ToQuery());

    dynamic customers = db.Cmd.QuerySingle<dynamic>("select top 1 * from Customers t".ToQuery());
    IList<dynamic> customers = db.Cmd.Query<dynamic>("select * from Customers t".ToQuery());

    IList<Categories> categories = (IList<Categories>)client.Cmd.QueryNonGeneric(typeof(Categories), "select top 3 * from Categories".ToQuery());
}
```

Update Operations
-----------------
```csharp
using (var db = ionixFactory.CreateTransactionalDbClient())
{
    Categories c = new Categories()
    {
        CategoryID = 8,
        CategoryName = "CategoryName",
    };

    int affected = db.Cmd.Update(c);
    affected = db.Cmd.Update(c, p => p.CategoryName);
    affected = await db.Cmd.UpdateAsync(c);

    IList<Categories> catagories = client.Cmd.Select<Categories>();
    db.Cmd.BatchUpdate(catagories);
    db.Cmd.BatchUpdate(catagories, p => p.CategoryName);
    await db.Cmd.BatchUpdateAsync(catagories);
     
    db.Commit();
}
```

Insert Operations
-----------------
```csharp
using (var db = ionixFactory.CreateTransactionalDbClient())
{
    Categories c = new Categories()
    {
        CategoryID = 0,
        CategoryName = "CategoryName",
    };

    int affected = db.Cmd.Insert(c);
    affected = await db.Cmd.InsertAsync(c);

    IList<Categories> catagories = client.Cmd.Select<Categories>();
    categories.ForEach((item) => item.CategoryID = 0);
    db.Cmd.BatchInsert(catagories);
    await db.Cmd.BatchInsertAsync(catagories);
     
    db.Commit();
}
```

Upsert Operations
-----------------
```csharp
using (var db = ionixFactory.CreateTransactionalDbClient())
{
    Categories c = new Categories()
    {
        CategoryID = 0,
        CategoryName = "CategoryName",
    };

    int affected = db.Cmd.Upsert(c);
    affected = db.Cmd.Upsert(c, p => p.CategoryName);
    affected = await db.Cmd.UpsertAsync(c);

    IList<Categories> catagories = client.Cmd.Select<Categories>();
    categories.ForEach((item) => item.CategoryID = 0);
    db.Cmd.BatchUpsert(catagories);
    db.Cmd.BatchUpsert(catagories, p => p.CategoryName);
    await db.Cmd.BatchUpsertAsync(catagories);
     
    db.Commit();
}
```

Delete Operations
-----------------
```csharp
using (var db = ionixFactory.CreateDbClient())
{
    Categories c = new Categories()
    {
        CategoryID = 3
    };

    int affected = db.Cmd.Delete(c);
    affected = await db.Cmd.DeleteAsync(c);
}
```

BulkCopy Operations
-------------------
```csharp
const int length = 1000000;
List<Region> list = new List<Region>(length);
for (int j = 0; j < length; ++j)
     list.Add(new Region() { RegionID = 1000 + j, RegionDescription = "dl" + j });

using (var db = ionixFactory.CreateDbClient())
{
    db.Cmd.BulkCopy(list);
    async db.Cmd.BulkCopyAsync(list); 
}
```

Which databases are supported?
------------------------------
Sql Server, Oracle, PostgreSQL (with migration / code-first approach) and SQLite.

You can check the [ionix.DataTests](https://github.com/mehmetgoren/ionix.Core/tree/master/ionix.DataTests) project out for more details.