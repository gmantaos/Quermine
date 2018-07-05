﻿using System;
using System.IO;
using System.Text;
using System.Data;
using System.Linq;
using System.Diagnostics;
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
			Console.WriteLine(q.ParametrizedQueryString());

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
	}
}