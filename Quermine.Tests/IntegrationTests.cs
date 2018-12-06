using System;
using System.IO;
using System.Text;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using NUnit.Framework;

using ChanceNET;

namespace Quermine.Tests
{
	[TestFixture]
	public class IntegrationTests
	{
		public IntegrationTests()
		{
		}

		public static IEnumerable<DbClient> DbClientTestCases()
		{
			if (Credentials.Sqlite().TestConnection().Result)
				yield return Credentials.Sqlite().Connect().Result;

			if (Credentials.MySql().TestConnection().Result)
				yield return Credentials.MySql().Connect().Result;

			if (Credentials.SqlServer().TestConnection().Result)
				yield return Credentials.SqlServer().Connect().Result;
		}

		[Test, TestCaseSource("DbClientTestCases")]
		public void Connection(DbClient client)
		{
			Assert.AreEqual(ConnectionState.Open, client.State);

			client.Dispose();
		}

		[Test, Order(1), TestCaseSource("DbClientTestCases")]
		public async Task CreateTable (DbClient client)
		{
			await client.DropTableIfExists("people");

			CreateTableQuery query = client.GetQueryProvider().CreateTable("people");
			query.Field<int>("id", fieldProperties: FieldProperties.PrimaryKey | FieldProperties.AutoIncrement)
				 .Field<string>("name")
				 .Field<DateTime>("birthday");

			NonQueryResult res = await client.ExecuteNonQuery(query);

			client.Dispose();
		}

		[Test, Order(2), TestCaseSource("DbClientTestCases")]
		public async Task Insert(DbClient client)
		{
			Query q1 = client.GetQueryProvider().Insert("people")
				.Value("name", "John").Value("birthday", DateTime.Parse("2018-05-26 16:22:54"));
			Query q2 = client.GetQueryProvider().Insert("people")
				.Value("name", "Mary").Value("birthday", DateTime.Parse("2018-05-26 16:23:04"));
			Query q3 = client.GetQueryProvider().Insert("people")
				.Value("name", "Pete").Value("birthday", DateTime.Parse("2018-05-26 16:23:07"));

			NonQueryResult r1 = await client.ExecuteNonQuery(q1);
			Assert.AreEqual(1, r1.RowsAffected);

			if (!(client is SqlServer.SqlServerClient))
				Assert.AreEqual(1, r1.LastInsertedId);

			NonQueryResult r2 = await client.ExecuteNonQuery(q2);
			Assert.AreEqual(1, r2.RowsAffected);

			if (!(client is SqlServer.SqlServerClient))
				Assert.AreEqual(2, r2.LastInsertedId);

			NonQueryResult r3 = await client.ExecuteNonQuery(q3);
			Assert.AreEqual(1, r3.RowsAffected);

			if (!(client is SqlServer.SqlServerClient))
				Assert.AreEqual(3, r3.LastInsertedId);

			client.Dispose();
		}

		[Test, Order(3), TestCaseSource("DbClientTestCases")]
		public async Task SelectAll(DbClient client)
		{
			Query query = client.GetQueryProvider().Query("SELECT * FROM people");

			ResultSet rs = await client.Execute(query);

			Assert.AreEqual(3, rs.RowCount);

			Assert.AreEqual(1, rs[0].GetInteger("id"));
			Assert.AreEqual("John", rs[0].GetString("name"));
			Assert.AreEqual(DateTime.Parse("2018-05-26 16:22:54"), rs[0].GetDateTime("birthday"));

			Assert.AreEqual(2, rs[1].GetInteger("id"));
			Assert.AreEqual("Mary", rs[1].GetString("name"));
			Assert.AreEqual(DateTime.Parse("2018-05-26 16:23:04"), rs[1].GetDateTime("birthday"));

			Assert.AreEqual(3, rs[2].GetInteger("id"));
			Assert.AreEqual("Pete", rs[2].GetString("name"));
			Assert.AreEqual(DateTime.Parse("2018-05-26 16:23:07"), rs[2].GetDateTime("birthday"));

			client.Dispose();
		}

		[Test, Order(4), TestCaseSource("DbClientTestCases")]
		public async Task InsertOne(DbClient client)
		{
			Query query = client.GetQueryProvider().Query("INSERT INTO people (name) VALUES ('Steve')");

			NonQueryResult result = await client.ExecuteNonQuery(query);

			Assert.AreEqual(1, result.RowsAffected);

			if (!(client is SqlServer.SqlServerClient))
				Assert.AreEqual(4, result.LastInsertedId);

			client.Dispose();
		}

		[Test, Order(5), TestCaseSource("DbClientTestCases")]
		public async Task SelectFour(DbClient client)
		{
			Query query = client.GetQueryProvider().Query("SELECT * FROM people");

			ResultSet rs = await client.Execute(query);

			Assert.AreEqual(4, rs.RowCount);

			client.Dispose();
		}

		[Test, Order(6), TestCaseSource("DbClientTestCases")]
		public async Task DeleteOne(DbClient client)
		{
			Query query = client.GetQueryProvider().Delete("people").Where("id", 4);

			NonQueryResult result = await client.ExecuteNonQuery(query);
			
			Assert.AreEqual(1, result.RowsAffected);

			client.Dispose();
		}

		[Test, Order(7), TestCaseSource("DbClientTestCases")]
		public async Task SelectThree(DbClient client)
		{
			Query query = client.GetQueryProvider().Query("SELECT * FROM people");

			ResultSet rs = await client.Execute(query);

			Assert.AreEqual(3, rs.RowCount);

			client.Dispose();
		}

		[Test, Order(8), TestCaseSource("DbClientTestCases")]
		public async Task SelectTwoChain(DbClient client)
		{
			Query query = client.GetQueryProvider()
								.Select("id", "name", "birthday")
								.From("people")
								.Where("id", WhereRelation.AtLeast, 2);


			ResultSet rs = await client.Execute(query);

			Assert.AreEqual(2, rs.RowCount);

			Assert.AreEqual(2, rs[0].GetInteger("id"));
			Assert.AreEqual("Mary", rs[0].GetString("name"));

			Assert.AreEqual(3, rs[1].GetInteger("id"));
			Assert.AreEqual("Pete", rs[1].GetString("name"));

			client.Dispose();
		}

		[Test, Order(9), TestCaseSource("DbClientTestCases")]
		public async Task UpdateOne(DbClient client)
		{
			UpdateQuery query = client.GetQueryProvider().Update("people");

			query.Where("id", 2);
			query.Set("name", "Pepe");

			NonQueryResult res = await client.ExecuteNonQuery(query);
			Assert.AreEqual(1, res.RowsAffected);

			client.Dispose();
		}

		[Test, Order(10), TestCaseSource("DbClientTestCases")]
		public async Task Scalar(DbClient client)
		{
			Query query = client.GetQueryProvider()
								.Select("name")
								.From("people")
								.Where("id", 2);

			string actual = await client.ExecuteScalar<string>(query);

			Assert.AreEqual("Pepe", actual);

			client.Dispose();
		}

		[Test, Order(11), TestCaseSource("DbClientTestCases")]
		public async Task ScalarDateTime(DbClient client)
		{
			Query query = client.GetQueryProvider()
								.Select("birthday")
								.From("people")
								.Where("id", 1);

			DateTime expected = DateTime.Parse("2018-05-26 16:22:54");
			DateTime actual = await client.ExecuteScalar<DateTime>(query);

			Assert.AreEqual(expected, actual);

			client.Dispose();
		}

		[Test, Order(12), TestCaseSource("DbClientTestCases")]
		public async Task SerializeInsert(DbClient client)
		{
			Person p = new Person()
			{
				Name = "Mark",
				Birthday = DateTime.Now.Date
			};

			InsertQuery<Person> q = client.GetQueryProvider().Insert<Person>(p);

			NonQueryResult res = await client.Insert(p);

			Assert.AreEqual(1, res.RowsAffected);

			if (!(client is SqlServer.SqlServerClient))
				Assert.AreEqual(5, res.LastInsertedId);

			client.Dispose();
		}

		[Test, Order(13), TestCaseSource("DbClientTestCases")]
		public async Task SerializeSelect(DbClient client)
		{
			SelectQuery<Person> query = client.GetQueryProvider().Select<Person>();
			query.Where("id", 5);

			Person p = (await client.Execute<Person>(query)).FirstOrDefault();

			Assert.AreEqual(5, p.ID);
			Assert.AreEqual("Mark", p.Name);
			Assert.AreEqual(DateTime.Now.Date, p.Birthday);

			client.Dispose();
		}

		[Test, Order(14), TestCaseSource("DbClientTestCases")]
		public async Task SerializeUpdate(DbClient client)
		{
			Person p = new Person()
			{
				ID = 5,
				Name = "Mark",
				Birthday = DateTime.Now.Date
			};

			DateTime newBd = DateTime.Now.Date.AddDays(1);

			await client.Update(p, mark =>
			{
				mark.Birthday = newBd;
			});

			DateTime bd = await client.ExecuteScalar<DateTime>("SELECT birthday FROM people WHERE id=5");

			Assert.AreEqual(newBd, bd);

			client.Dispose();
		}

		[Test, Order(15), TestCaseSource("DbClientTestCases")]
		public async Task Transaction(DbClient client)
		{
			Person p = new Person()
			{
				Name = "Jimothy",
				Birthday = DateTime.Now.Date
			};

			Query q1 = client.GetQueryProvider().Insert(p);
			Query q2 = client.GetQueryProvider().Update("people")
			                                    .Set("name", "Lance")
												.Where("id", 6);

			List<NonQueryResult> res = await client.ExecuteTransaction(q1, q2);

			Assert.AreEqual(2, res.Count);
			Assert.AreEqual(1, res[0].RowsAffected);
			Assert.AreEqual(1, res[1].RowsAffected);

			if (!(client is SqlServer.SqlServerClient))
			{
				Assert.AreEqual(6, res[0].LastInsertedId);
			}

			string name = await client.ExecuteScalar<string>("SELECT name FROM people WHERE id=6");

			Assert.AreEqual("Lance", name);

			client.Dispose();
		}

		[Test, Order(16), TestCaseSource("DbClientTestCases")]
		public async Task TableNames(DbClient client)
		{
			List<string> tables = await client.GetTableNames();

			Assert.IsTrue(tables.Contains("people"));

			client.Dispose();
		}
		

		[Test, Order(17), TestCaseSource("DbClientTestCases")]
		public async Task TableSchema(DbClient client)
		{
			TableSchema schema = await client.GetTableSchema("people");

			Assert.IsFalse(schema == null);

			Assert.AreEqual(3, schema.Count());

			TableField id = schema["id"];
			Assert.IsFalse(id == null);
			Assert.AreEqual("id", id.Name);

			if (!(client is SqlServer.SqlServerClient))
			{
				Assert.AreEqual(KeyType.Primary, id.Key);
			}

			if (!(client is Sqlite.SqliteClient)
				&& !(client is SqlServer.SqlServerClient))
			{
				Assert.AreEqual(true, id.AutoIncrement);
				Assert.AreEqual(true, id.NotNull);
			}

			TableField name = schema["name"];
			Assert.IsFalse(name == null);
			Assert.AreEqual(typeof(string), name.Type);

			TableField bd = schema["birthday"];
			Assert.IsFalse(bd == null);
			Assert.AreEqual(typeof(DateTime), bd.Type);

			client.Dispose();
		}

		[Test, Order(18), TestCaseSource("DbClientTestCases")]
		public async Task SetValueFormatting(DbClient client)
		{
			List<Person> people = await client.Select<Person>();

			foreach (Person p in people)
			{
				string ascii = Encoding.ASCII.GetString(p.NameAscii);

				Assert.AreEqual(p.Name, ascii);
			}

			client.Dispose();
		}

		[Test, Order(19), TestCaseSource("DbClientTestCases")]
		public async Task GetValueFormatting(DbClient client)
		{
			PersonName p = new PersonName()
			{
				NameAscii = Encoding.ASCII.GetBytes("Cool Dude")
			};

			NonQueryResult res = await client.Insert(p);

			Assert.AreEqual(1, res.RowsAffected);
			if (!(client is SqlServer.SqlServerClient))
			{
				Assert.AreEqual(7, res.LastInsertedId);
			}

			string name = await client.ExecuteScalar<string>("SELECT name FROM people WHERE id=7");

			Assert.AreEqual("Cool Dude", name);

			client.Dispose();
		}

		[Test, TestCaseSource("DbClientTestCases")]
		public async Task CreateTableT(DbClient client)
		{
			await client.DropTableIfExists("books");

			Query query = client.GetQueryProvider().CreateTable<Book>();

			await client.ExecuteNonQuery(query);

			List<string> tableNames = await client.GetTableNames();

			Assert.IsTrue(tableNames.Contains("books"));

			TableSchema schema = await client.GetTableSchema("books");

			TableField id = schema["id"];
			Assert.IsFalse(id == null);
			Assert.AreEqual("id", id.Name);
			Assert.AreEqual(typeof(int), id.Type);
			if (!(client is SqlServer.SqlServerClient))
			{
				Assert.AreEqual(KeyType.Primary, id.Key);
			}

			TableField title = schema["title"];
			Assert.AreEqual("title", title.Name);
			Assert.AreEqual(12, title.Length);
			Assert.AreEqual(typeof(string), title.Type);

			TableField year = schema["year"];
			Assert.AreEqual("year", year.Name);
			Assert.AreEqual(typeof(int), year.Type);

			TableField release = schema["release"];
			Assert.AreEqual("release", release.Name);
			Assert.AreEqual(typeof(DateTime), release.Type);
		}
		
		public async Task DataTest(DbClient client)
		{
			await client.DropTableIfExists("books");

			Query query = client.GetQueryProvider().CreateTable<Book>();

			await client.ExecuteNonQuery(query);

			Chance chance = new Chance();

			List<Book> books = chance.N(20, () => chance.Object<Book>());

			List<NonQueryResult> res = await client.InsertAll(books);

			Assert.AreEqual(20, res.Count);

			List<Book> dbBooks = await client.Select<Book>();

			Assert.AreEqual(20, dbBooks.Count);

			foreach (Book book in books)
			{
				Book dbBook = dbBooks.First(b => book.ID == b.ID);

				Assert.AreEqual(book.Title, dbBook.Title);
				Assert.AreEqual(book.Year, dbBook.Year);
				Assert.AreEqual(book.Release, dbBook.Release);
			}
		}
	}
}
