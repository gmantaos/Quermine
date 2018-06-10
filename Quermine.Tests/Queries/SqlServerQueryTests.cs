using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NUnit.Framework;

using Quermine.SqlServer;

namespace Quermine.Tests
{
	[TestFixture]
	public class SqlServerQueryTests
	{
		/*
		 * SELECT queries
		 */
		public static IEnumerable<object[]> SelectQueryTestCases()
		{
			yield return 
				new object[] 
				{
					Sql.Query("SELECT * FROM tname"),
					"SELECT * FROM tname"
				};
			yield return 
				new object[] 
				{
					Sql.Query("SELECT col1, col2 FROM tname"),
					"SELECT col1, col2 FROM tname"
				};
			yield return
				new object[] 
				{
					Sql.Query("SELECT col1, col2 FROM tname WHERE col1=@p").AddParameter("@p", 123),
					"SELECT col1, col2 FROM tname WHERE col1='123'"
				};
			yield return
				new object[]
				{
					Sql.Select("col1", "col2").From("tname"),
					"SELECT col1, col2 FROM tname"
				};
			yield return
				new object[]
				{
					Sql.Select("col1", "col2").From("tname").Limit(12),
					"SELECT col1, col2 FROM tname ORDER BY (SELECT 0) OFFSET 0 ROWS FETCH NEXT 12 ROWS ONLY"
				};
			yield return
				new object[]
				{
					Sql.Select("col1", "col2").From("tname").Offset(12),
					"SELECT col1, col2 FROM tname ORDER BY (SELECT 0) OFFSET 12 ROWS"
				};
			yield return
				new object[]
				{
					Sql.Select("col1", "col2").From("tname").Offset(12).Limit(13),
					"SELECT col1, col2 FROM tname ORDER BY (SELECT 0) OFFSET 12 ROWS  FETCH NEXT 13 ROWS ONLY"
				};
		}

		/*
		 * INSERT queries
		 */
		public static IEnumerable<object[]> InsertQueryTestCases()
		{
			yield return
				new object[]
				{
					Sql.Insert("tname").Value("col1", 123).Value("col2", "enlo"),
					"INSERT INTO tname (col1, col2) VALUES ('123', 'enlo')"
				};
			yield return
				new object[]
				{
					Sql.Insert("tname").Value("col1", 123).Value("col2", "enlo").Replace(false),
					"INSERT INTO tname (col1, col2) VALUES ('123', 'enlo')"
				};
			yield return
				new object[]
				{
					Sql.Insert("tname").Value("col1", 123).Value("col2", "enlo").Replace(),
					"REPLACE INTO tname (col1, col2) VALUES ('123', 'enlo')"
				};
			yield return
				new object[]
				{
					Sql.Insert("tname").Value("col1", 123).Value("col2", "enlo").Ignore(true),
					"INSERT IGNORE INTO tname (col1, col2) VALUES ('123', 'enlo')"
				};
		}

		/*
		 * UPDATE queries
		 */
		public static IEnumerable<object[]> UpdateQueryTestCases()
		{
			yield return
				new object[]
				{
					Sql.Update("tname").Set("col1", 123),
					"UPDATE tname SET col1 = '123'"
				};
			yield return
				new object[]
				{
					Sql.Update("tname T", "tname T2").Set("T.col1", 123),
					"UPDATE tname T, tname T2 SET T.col1 = '123'"
				};
			yield return
				new object[]
				{
					Sql.Update("tname").Set("col1", 123).Set("col2", "enlo"),
					"UPDATE tname SET col1 = '123', col2 = 'enlo'"
				};
			yield return
				new object[]
				{
					Sql.Update("tname").Set("col1", 123).Set("col2", "enlo").Limit(12),
					"UPDATE TOP 12 tname SET col1 = '123', col2 = 'enlo'"
				};
			yield return
				new object[]
				{
					Sql.Update("tname").Set("col1", 123).Set("col2", "enlo").Ignore(),
					"UPDATE IGNORE tname SET col1 = '123', col2 = 'enlo'"
				};
			yield return
				new object[]
				{
					Sql.Update("tname").Set("col1", 123).Set("col2", "enlo").Limit(12).Ignore(),
					"UPDATE TOP 12 IGNORE tname SET col1 = '123', col2 = 'enlo'"
				};
		}

		[Test, TestCaseSource("SelectQueryTestCases")]
		public void SelectQueries(Query query, string expected)
		{
			query.AssertEqual(expected);
		}

		[Test, TestCaseSource("InsertQueryTestCases")]
		public void InsertQueries(Query query, string expected)
		{
			query.AssertEqual(expected);
		}

		[Test, TestCaseSource("UpdateQueryTestCases")]
		public void UpdateQueries(Query query, string expected)
		{
			query.AssertEqual(expected);
		}
	}
}
