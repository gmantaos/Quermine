using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quermine.MySql;
using Quermine.Sqlite;
using Quermine.SqlServer;

namespace Quermine.Tests
{
	public static class Credentials
	{
		public static MySqlConnectionInfo MySql()
		{
			return new MySqlConnectionInfo(
				Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "127.0.0.1",
				Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root",
				Environment.GetEnvironmentVariable("MYSQL_PASS") ?? "Password12!",
				"quermine"
			);
		}
	}
}
