using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NUnit.Framework;

using Quermine;

namespace Quermine.Tests
{
	public static class TestExtensions
	{
		
		public static void AssertEqual(this Query actual, string expected)
		{
			Assert.AreEqual(Normalize(expected), Normalize(actual.ParametrizedQueryString()));
		}

		static string Normalize(string str)
		{
			return Regex.Replace(str, @"\s+", " ").Trim();
		}

		public static TestQueryProvider GetQueryProvider(this DbClient client)
		{
			if (client is MySql.MySqlClient)
			{
				return new MySqlQueryProvider();
			}
			else if (client is Sqlite.SqliteClient)
			{
				return new SqliteQueryProvider();
			}
			else if (client is SqlServer.SqlServerClient)
			{
				return new SqlServerQueryProvider();
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
