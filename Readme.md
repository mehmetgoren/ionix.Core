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
	
    var customerList = dbCmd.Query<Customers>("select * from Customers".ToQuery());
    customerList = await  dbCmd.QueryAsync<Customers>("select * from Customers".ToQuery()); 	
}
```

Select Operations #2
-----------------
```csharp
```


it supprots Sql Server, Oracle, PostgreSQL(with migration / code-first approach) and SQLite.