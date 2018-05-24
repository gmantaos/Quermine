using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Quermine.SqlServer
{
    public class SqlServerConnectionInfo : ConnectionInfo<SqlServerClient>
	{
		public readonly string Host;
		public readonly string Username;
		public readonly string Password;
		public readonly string Database;
		public readonly int Port;
		
		public SqlServerConnectionInfo(string host, string username, string password, string database, int port)
		{
			Host = host;
			Username = username;
			Password = password;
			Database = database;
			Port = port;
		}

		public override string ConnectionString => string.Format(
														"User ID={0};Password={1};Data Source=={2},{3};Initial Catalog={4};",
														Username, Password, Host, Port, Database
													);

		public override async Task<SqlServerClient> Connect()
		{
			SqlServerClient client = new SqlServerClient(this);
			await client.OpenAsync();
			return client;
		}

		public override async Task<bool> TestConnection()
		{
			using (SqlServerClient client = new SqlServerClient(this))
			{
				await client.OpenAsync();
				return client.State == ConnectionState.Open;
			}
		}
	}
}
