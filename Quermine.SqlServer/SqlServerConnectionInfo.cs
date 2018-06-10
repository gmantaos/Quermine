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

		Dictionary<string, object> parameters = new Dictionary<string, object>();

		public SqlServerConnectionInfo(string host, string username, string password, string database, int port = 1433)
		{
			Host = host;
			Username = username;
			Password = password;
			Database = database;
			Port = port;
		}

		public override string ConnectionString
		{
			get
			{
				StringBuilder str = new StringBuilder(
					string.Format(
						"UID={0};Password={1};Server={2},{3};Database={4};",
						Username, Password, Host, Port, Database
					)
				);

				foreach (KeyValuePair<string, object> param in parameters)
				{
					str.AppendFormat("{0}={1};", param.Key, param.Value);
				}

				return str.ToString();
			}
		}

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
				try
				{
					await client.OpenAsync();
					return client.State == ConnectionState.Open;
				}
				catch
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Add an additional parameter to the connection string, which will be appended
		/// in the form of KEY=VALUE;
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public SqlServerConnectionInfo AddParameter(string key, object value)
		{
			parameters.Add(key, value);
			return this;
		}
	}
}
