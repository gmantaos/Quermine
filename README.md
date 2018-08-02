
<h1 align="center">
  <img src="Assets/logo.png">
</h1>

<p align="center">
<a href="https://www.nuget.org/packages/Quermine">
  <img src="https://img.shields.io/nuget/v/Quermine.svg">
</a>
<a href="https://www.nuget.org/packages/Quermine">
  <img src="https://img.shields.io/nuget/dt/Quermine.svg">
</a>
<a href="https://ci.appveyor.com/project/gmantaos/quermine">
  <img src="https://ci.appveyor.com/api/projects/status/ahda4q6d648ahq99/branch/master?svg=true">
</a>
<a href="LICENSE">
  <img src="https://img.shields.io/badge/license-MIT-blue.svg">
</a>
</p>

This library offers a significant abstraction over integrating with a relational database in your **.NET** application. What started out as a personal wrapper for convenient async operations and MySql type conversions later became an intuitive query builder and object serializer. With this library you easily connect your classes with your database tables in minutes and without writing almost any queries.

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

Or if the database file doesn't exist yet.

```csharp
SqliteConnectionInfo info = new SqliteConnectionInfo("/var/www/mydb.sqlite");

info.Create();
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

// The transaction was committed 
foreach (NonQueryResult r in result)
{
    // The result of each query in the order they were executed
}
```

A failing transaction will throw the exception that was raised.

```csharp
try
{
	await connection.ExecuteTransaction(q1, q2, q3);
}
catch(Exception ex)
{
	// Transaction was rolled-back automatically
	Console.WriteLine(ex);
}
```

### Schemas

```csharp
List<string> tables = await connection.GetTableNames();

foreach (string table in tables)
{
    TableSchema schema = await connection.GetTableSchema(table);

    // Get specific fields by name
    TableField nameField = schema["name"];

    // ... or iterate over them
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
    [DbField("name")]
    string Name;

    [DbField("age")]
    int Age;
}
```

You can deserialize any query into your object.

```csharp
Query q = Sql.Query("SELECT name, age FROM other_table");

List<Person> people = await connection.Execute<Person>(q);
```

Or you can let the library construct the queries, as shown in the sections below.

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

### Ignoring fields

There are cases when certain fields should participate in `SELECT` queries, for retrieving objects, 
but not in other kinds of queries. Such cases for example are the following:

- **Auto-Increment IDs**: A field mapped to such a column in a table should generally not be specified manually when inserting an object, or attempted to be updated. To achieve this you can use the `InsertIgnore` and `UpdateIgnore` attributes.
- **Non-Distinctive Types:** Queries like `DELETE`, `UPDATE` and `SELECT` should use distinctive fields of an object in their `WHERE` clauses, like an ID or a name. Searching by values that don't define the object or values that are hard to compare - like floating point numbers for example - can lead to unintended side effects. To exclude such fields from `WHERE` clauses use the `WhereIgnore` attribute.

```csharp
[DbTable("people")]
class Person
{
    [DbField("id"), InsertIgnore, UpdateIgnore]
    int ID;

    [DbField("name")]
    string Name;
	
    [DbField("savings"), WhereIgnore]
    double Savings;
}
```

### Custom value formatting

Another common use case is having to convert or transform values, between serializable members and the database. Such instances may require a simple type cast, or even custom parsing of the given values to generate entirely new objects. To achieve this, you can create custom formatters that implement the `IValueFormatter<T>` interface, like in the following examples.

```csharp
// Treating an integer in the database as a double in the object
class RoundFormatter : IValueFormatter<double>
{
    public object GetValue(double val)
    {
        return (int)Math.Round(val);
    }

    public double SetValue(object val)
    {
        return (double)val;
    }
}

// Treating a string field in the database as a byte array in the object
class AsciiFormatter : IValueFormatter<byte[]>
{
    public object GetValue(byte[] val)
    {
        return Encoding.ASCII.GetString(val);
    }

    public byte[] SetValue(object val)
    {
        return Encoding.ASCII.GetBytes(val.ToString());
    }
}

// Having a birthday in the database but wanting an age in the class
class AgeFormatter : IValueFormatter<int>
{
    public object GetValue(int val)
    {
        throw new NotImplementedException();
    }

    public int SetValue(object val)
    {
        DateTime birthday = (DateTime)val;
	
        return YearsBetween(birthday, DateTime.Now);
    }
}
```

`GetValue` is used when *reading* the value of a member, in order to place the converted value in a query, and `SetValue` is used to convert values fetched from the database before *writing* them on the member.

```csharp
[DbTable("books")]
class Book
{
    [DbField("description", FormatWith = typeof(AsciiFormatter))]
    byte[] AsciiDescription;    // a string in the database
    
    [DbField("savings", FormatWith = typeof(RoundFormatter))]
    double Savings;             // an integer in the database
    
    [DbField("release_date", FormatWith = typeof(AgeFormatter))]
    int Age;			// a DateTime in the database
}
```

## Known issues and limitations

#### SQL Server

- The `LastInsertedId` field on non-query results will always return `-1`, since implementing it would require injecting things into queries. If and and how this should happen needs to be carefully looked into beforehand.
- The `AutoIncrement`, `Unsigned`, `Zerofill` and `Key` fields of a `TableField` will be non-indicative of the real value.

#### SQLite

- The `AutoIncrement` and `NotNull` fields of a `TableField` will be non-indicative of the real value.



