using System;
using System.IO;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

using NUnit.Framework;

using Quermine;
using Quermine.Sqlite;

namespace Quermine.Tests
{
	[TestFixture]
	public class SqliteIntegrationTests
	{
		SqliteConnectionInfo info;
		SqliteClient client;

		public SqliteIntegrationTests()
		{
			string dbPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Samples/test_data.sqlite");

			info = new SqliteConnectionInfo(dbPath);
		}

		[SetUp]
		public async Task Init()
		{
			client = await info.Connect();
		}

		[TearDown]
		public void Cleanup()
		{
			client.Dispose();
		}

		[Test]
		public async Task Connection()
		{
			Assert.AreEqual(ConnectionState.Open, client.State);
		}

		[Test]
		public async Task TestConnection()
		{
			Assert.AreEqual(true, await info.TestConnection());
		}

		[Test]
		public async Task SelectAll()
		{
			Query query = Sql.Query("SELECT * FROM people");

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
		}

		[Test]
		public async Task InsertOne()
		{
			Query query = Sql.Query("INSERT INTO people (name) VALUES ('Steve')");

			NonQueryResult result = await client.ExecuteNonQuery(query);

			Assert.AreEqual(4, result.LastInsertedId);
			Assert.AreEqual(1, result.RowsAffected);
		}
	}
}
