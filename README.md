
![logo](Assets/logo.png)


This library offers a significant abstraction over integrating with a relational database in your **.NET** application. What started out as a personal wrapper for convenient async operations and MySql type conversions later became an intuitive query builder and object serializer. With this library you easily connect your classes with your database tables in minutes and without writing a single query.

These abstractions aren't always free however, you cannot use the chain query building and serialization features of this library to create highly efficient queries, with complex join operations. For these cases you are still better off compiling your own. But in every other case, where you simply want to deal database tables as if they were nothing more than classes, fetching, updating and storing them again as you please, then Quermine has you covered.

## Usage

Simply instantiate the appropriate client for your database type. The following examples will be using MySql, but the usage will be identical to that of the other supported types.

```csharp
using Quermine;
using Quermine.MySql;

MysqlConnectionInfo info = new MysqlConnectionInfo("127.0.0.1", "root", "password", "database");

using (DbClient connection = await info.Connect())
{
    ...
}
```

You can of course write your own queries, like you normally would. In this case you would be taking advantage of the library's wrappers.

```csharp

Query q = MySql.Query("SELECT * FROM cats WHERE color=@color");

q.AddParameter("@color", "orange");

q.OnRow += (s, row) => {

    // Getting rows as events when they arrive.
    row.GetString("name");
    row.GetInteger("age");
    row.GetBoolean("gender");
    
};
 
// Or get the whole result set once the query is completed.
ResultSet result = await connection.Execute(q);

foreach (ResultRow Row in result)
{
    ...
}
```

### NonQueries

```csharp
Query q = MySql.Query("DELETE FROM cats WHERE color='organge'");

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

## Chaining


### Select

```csharp
SelectQuery q = MySql.Select();

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
InsertQuery q = MySql.Insert("table_name");

q.Value("user_id", 4)
 .Value("age", 10)
 .Value("timestamp", DateTime.Now);
 
await connection.ExecuteNonQuery(q);
```

### Delete

```csharp
DeleteQuery q = MySql.Delete("table_name");

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
SelectQuery<Person> q = MySql.Select<Person>();

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

### // TODO

- Support "ON DUPLICATE KEY UPDATE"