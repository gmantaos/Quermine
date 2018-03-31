using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using System.Data;

namespace Queerie.MySql
{
	public class MySqlClient : DbClient
	{
		MySqlConnection conn;

		internal MySqlClient(string connectionString)
		{
			conn = new MySqlConnection(connectionString);
		}

		public override ConnectionState State => conn.State;

		public override void Dispose()
		{
			conn.Dispose();
		}

		protected override Task OpenAsync()
		{
			return conn.OpenAsync();
		}
	}
}
