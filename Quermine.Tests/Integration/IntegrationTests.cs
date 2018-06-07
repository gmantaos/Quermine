using System;
using System.IO;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

using NUnit.Framework;

using Quermine;

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
			yield return Credentials.Sqlite().Connect().Result;
			yield return Credentials.MySql().Connect().Result;
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
			await client.ExecuteNonQuery("DROP TABLE IF EXISTS `people`");

			CreateTableQuery query = client.GetQueryProvider().CreateTable("people");
			query.Field<int>("id", fieldTypes: FieldTypes.PrimaryKey | FieldTypes.AutoIncrement)
				 .Field<string>("name")
				 .Field<DateTime>("birthday");

			Console.WriteLine(query.QueryString);

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
			Assert.AreEqual(1, r1.LastInsertedId);

			NonQueryResult r2 = await client.ExecuteNonQuery(q2);
			Assert.AreEqual(1, r2.RowsAffected);
			Assert.AreEqual(2, r2.LastInsertedId);

			NonQueryResult r3 = await client.ExecuteNonQuery(q3);
			Assert.AreEqual(1, r3.RowsAffected);
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

			Assert.AreEqual(4, result.LastInsertedId);
			Assert.AreEqual(1, result.RowsAffected);

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
	}
}
