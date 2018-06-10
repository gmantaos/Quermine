
![logo](Assets/logo.png)

[![ ](https://img.shields.io/nuget/v/Quermine.svg)](https://www.nuget.org/packages/Quermine)
[![ ](https://img.shields.io/nuget/dt/Quermine.svg)](https://www.nuget.org/packages/Quermine)
[![ ](https://ci.appveyor.com/api/projects/status/ahda4q6d648ahq99/branch/master?svg=true)](https://ci.appveyor.com/project/gmantaos/quermine)
[![ ](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

This library offers a significant abstraction over integrating with a relational database in your **.NET** application. What started out as a personal wrapper for convenient async operations and MySql type conversions later became an intuitive query builder and object serializer. With this library you easily connect your classes with your database tables in minutes and without writing a single query.

These abstractions aren't always free however, you cannot use the chain query building and serialization features of this library to create highly efficient queries, with complex join operations. For these cases you are still better off compiling your own. But in every other case, where you simply want to deal database tables as if they were nothing more than classes, fetching, updating and storing them again as you please, then Quermine has you covered.

The goals of this library are:

- To provide a uniform plug n' play solution for dealing with your database in an object-oriented way.
- To build your queries in a LINQ-like chaining manner, instead of with messy string injections and appendages.
- Query-free integration with relational databases for the everyday simple object-oriented cases.
- Painless extension for additional DBMS connectors and syntaxes.

#### Progress

- [x] MySql
- [x] Sqlite
- [x] SQL Server
- [ ] PostgreSql

## Installation

Get the appropriate package from NuGet

- [Quermine.MySql](https://www.nuget.org/packages/Quermine.MySql)
- [Quermine.Sqlite](https://www.nuget.org/packages/Quermine.Sqlite)
- [Quermine.SqlServer](https://www.nuget.org/packages/Quermine.SqlServer)

A fair warning though, that during the prerelease phase, CI builds of the packages are 
published without running any tests. That being said though, I've used this code quite extensively 
on a couple of apps in production - before it were a nuget package - with no significant problems.

## Usage

### MySql

```csharp
using Quermine;
using Quermine.MySql;

MySqlConnectionInfo info = new MySqlConnectionInfo("127.0.0.1", "root", "password", "database");

// You can also add additional values to the connection string
info.AddParameter("charset", "utf8")
    .AddParameter("Allow Zero Datetime", "True");

using (MySqlClient connection = await info.Connect())
{
    ...
}
```

### Sqlite

```csharp
using Quermine;
using Quermine.Sqlite;

SqliteConnectionInfo info = new SqliteConnectionInfo("/var/www/mydb.sqlite");

using (SqliteClient connection = await info.Connect())
{
    ...
}
```


### SQL Server

```csharp
using Quermine;
using Quermine.SqlServer;

SqlServerConnectionInfo info = new SqlServerConnectionInfo("127.0.0.1", "root", "password", "database");

// You can also add additional values to the connection string
info.AddParameter("Integrated Security", "True");

using (SqlServerClient connection = await info.Connect())
{
    ...
}
```

## Queries

The following examples will be using MySql, but the usage will be identical to that of the other supported types. You can of course write your own queries, like you normally would. In this case you would be taking advantage of the library's wrappers.

```csharp

Query q = Sql.Query("SELECT * FROM cats WHERE color=@color");

q.AddParameter("@color", "orange");

q.OnRow += (s, row) => {

    // Getting rows as events when they arrive.
    row.GetString("name");
    row.GetInteger("age");
    row.GetBoolean("gender");
    
};
 
// Or get the whole result set once the query is completed.
ResultSet result = await connection.Execute(q);

foreach (ResultRow row in result)
{
    ...
}
```

### NonQueries

```csharp
Query q = Sql.Query("DELETE FROM cats WHERE color='orange'");

NonQueryResult result = await connection.ExecuteNonQuery(q);

result.RowsAffected;
result.LastInsertedId;
```

### Transactions

```csharp
Query q1, q2, q3;

List<NonQueryResult> result = await connection.ExecuteTransaction(q1, q2, q3);

if (result == null)
{
    // The transsaction failed and a rollback was issued
}
else
{
    // The transaction was committed 
    foreach (NonQueryResult r in result)
    {
        // The result of each query in the order they were executed
    }
}
```

### Schemas

```csharp
List<string> tables = await connection.GetTableNames();

foreach (string table in tables)
{
    TableSchema schema = await connection.GetTableSchema(table);

    foreach (TableField field in schema)
    {
        ...
    }
}
```

## Chaining


### Select

```csharp
SelectQuery q = Sql.Select();

q.Select("name", "age")
    .From("tableName")
    .Where("age", WhereRelation.LesserThan, 10)
    .Where("name", WhereRelation.Like, "John%")
    .Where("email", ColumnCondition.IsNull)
    .Limit(5)
    .Offset(10);

await connection.Execute(q);
```

### Insert

```csharp
InsertQuery q = Sql.Insert("table_name");

q.Value("user_id", 4)
 .Value("age", 10)
 .Value("timestamp", DateTime.Now);
 
await connection.ExecuteNonQuery(q);
```

### Delete

```csharp
DeleteQuery q = Sql.Delete("table_name");

q.Where("id", 2); // Shortcut to Where("id", WhereRelation.Equal, 2)

await connection.ExecuteNonQuery(q);
```

#### WHERE clause

##### Chaining appends logical AND

```csharp
q.Where("age", WhereRelation.LesserThan, 10)
 .Where("name", WhereRelation.Like, "John%");
```

Will result in...

```mysql
WHERE age < 10 AND name LIKE 'John%'
```

##### To create nested and/or logical OR conditions create them manually

```csharp
q.Where(
    new WhereClause("age", WhereRelation.LesserThan, 10)
    | new WhereClause("name", WhereRelation.Like, "John%")
);
```

Will result in

```mysql
WHERE age < 10 OR name like 'John%'
```

Nesting works as well

```csharp
WhereClause w1, w2, w3, w4;

q.Where(
    (w1 | w2) & (w3 & w4)
);
```

## Serialization

```csharp
[DbTable("people")]
class Person
{
    [DbColumn("name")]
    string Name;

    [DbColumn("age")]
    int Age;
}
```

### Inserting

```csharp
Person p = new Person(name: "John", age: 42);

connection.Insert<Person>(p);
```

Or if you want to add parameters...
```csharp
Person p = ...;

InsertQuery<Person> q = Mysql.Insert<Person>(p);

// Turn it into a REPLACE query perhaps?
p.Replace();

// Or an INSERT IGNORE INTO query?
p.Ignore();

await connection.ExecuteNonQuery(q);
```

### Selecting

```csharp
List<Person> = await connection.Select<Person>();
```

Or if you want to add parameters...
```csharp
SelectQuery<Person> q = Sql.Select<Person>();

q.Where("name", Comparison.Equals, "John");

List<Person> result = awat connection.ExecuteNonQuery(q);
```

### Deleting

**WARNING**: `DeleteQuery` will delete ANY rows that match the given object's values.

```csharp
Person p = new Person(name: "John", age: 42);

connection.Delete<Person>(p);
```

### Updating

```csharp
Person john = new Person(name: "John", age: 42);

await connection.Update<Person>(john, p => {

    //Whatever happens to the object in this lambda will also happen to it in the database

    p.Age  = 10;

});

/*
Now john.Age is 10 and john's age in the databse is also 10
*/
```

## Known issues

#### SQL Server

- The `LastInsertedId` field on non-query results will always return `-1`, since implementing it would require injecting things into queries. If and and how this should happen needs to be carefully looked into beforehand.
