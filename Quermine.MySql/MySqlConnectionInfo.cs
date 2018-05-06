using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Quermine.MySql
{
    public class MySqlConnectionInfo : ConnectionInfo<MySqlClient>
	{
		public readonly string Host;
		public readonly string Username;
		public readonly string Password;
		public readonly string Database;
		public readonly int Port;
		
		public MySqlConnectionInfo(string host, string username, string password, string database, int port)
		{
			Host = host;
			Username = username;
			Password = password;
			Database = database;
			Port = port;
		}

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
