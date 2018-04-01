using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Quermine.MySql
{
    public class MySqlConnectionInfo : ConnectionInfo<MySqlClient>
	{
		public MySqlConnectionInfo(string host, string username, string password, string database, int port = 3306)
			: base(host, username, password, database, port) { }

		public override string ConnectionString => string.Format(
														"UID={0};password={1};Server={2};Port={3};database={4};"
														+ "connection timeout=30;charset=utf8",
														Username, Password, Host, Port, Database
													);

		public override async Task<MySqlClient> Connect()
		{
			MySqlClient client = new MySqlClient(this);
			await client.OpenAsync();
			return client;
		}

		public override async Task<bool> TestConnection()
		{
			using (MySqlClient client = new MySqlClient(this))
			{
				await client.OpenAsync();
				return client.State == ConnectionState.Open;
			}
		}
	}
}
