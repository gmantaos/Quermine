using System;

using Quermine;
using Quermine.MySql;

using NUnit.Framework;
using System.Collections.Generic;

namespace Quermine.Tests
{
	[TestFixture]
	public class MySqlTests
	{
		/*
		 * ConnectionInfo tests
		 */
		public static IEnumerable<object[]> ConnectionInfoTestCases()
		{
			yield return
				new object[]
				{
					new MySqlConnectionInfo("host", "username", "password", "db"),
					"UID=username;password=password;Server=host;Port=3306;database=db;"
				};
			yield return
				new object[]
				{
					new MySqlConnectionInfo("host", "username", "password", "db", 1234),
					"UID=username;password=password;Server=host;Port=1234;database=db;"
				};
			yield return
				new object[]
				{
					new MySqlConnectionInfo("host", "username", "password", "db", 1234).AddParameter("Allow Zero Datetime", true),
					"UID=username;password=password;Server=host;Port=1234;database=db;Allow Zero Datetime=True;"
				};
		}

		[Test, TestCaseSource("ConnectionInfoTestCases")]
		public void ConnectionInfo(MySqlConnectionInfo info, string expected)
		{
			Assert.AreEqual(expected, info.ConnectionString);
		}
	}
}
