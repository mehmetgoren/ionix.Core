ionix is a micro-orm libraray that based on .net standart 2.0 (which supports Linux, Windows and macOS).

From NuGet
----------
Install-Package ionix.Data


It' s very easy to use, here is some Northwind database examples;

Select Operations
-----------------

```csharp
using (var db = ionixFactory.CreateDbClient())
{
    var employee = db.Cmd.SelectById<Employees>(1);
    employee = await db.Cmd.SelectByIdAsync<Employees>(1);
	
	
    var customer = dbCmd.QuerySingle<Customers>("select * from Customers where CustomerID=@0".ToQuery("ANATR")); 
    customer = await dbCmd.QuerySingleAsync<Customers>("select * from Customers where CustomerID=@0".ToQuery("ANATR"));
	
    IList<Customers> customerList = dbCmd.Query<Customers>("select * from Customers".ToQuery());
    customerList = await dbCmd.QueryAsync<Customers>("select * from Customers".ToQuery());

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
    
    IList<int> regionIds = db.Cmd.Query<int>("select top 1 RegionID from Region".ToQuery());

    dynamic customers = db.Cmd.QuerySingle<dynamic>("select * from Customers t".ToQuery());
    IList<dynamic> customers = db.Cmd.Query<dynamic>("select * from Customers t".ToQuery());
}
```


it supprots Sql Server, Oracle, PostgreSQL(with migration / code-first approach) and SQLite.