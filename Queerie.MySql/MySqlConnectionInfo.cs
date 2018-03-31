using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Queerie.MySql
{
    public class MySqlConnectionInfo : ConnectionInfo<MySqlClient>
	{
		public MySqlConnectionInfo(string host, string username, string password, string database, int port = 3306)
			: base(host, username, password, database, port) { }

		protected override string ConnectionString => string.Format(
														"UID={0};password={1};Server={2};Port={3};database={4};"
														+ "connection timeout=30;charset=utf8",
														username, password, host, port, database
													);

		public override Task<MySqlClient> Connect()
		{
			throw new NotImplementedException();
		}

		public override Task<bool> TestConnection()
		{
			throw new NotImplementedException();
		}
	}
}
