﻿using System;
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

		Dictionary<string, object> parameters = new Dictionary<string, object>();
		
		public MySqlConnectionInfo(string host, string username, string password, string database, int port = 3306)
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
					string.Format("UID={0};password={1};Server={2};Port={3};database={4};",
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

		/// <summary>
		/// Add an additional parameter to the connection string, which will be appended
		/// in the form of KEY=VALUE;
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public MySqlConnectionInfo AddParameter(string key, object value)
		{
			parameters.Add(key, value);
			return this;
		}
	}
}
