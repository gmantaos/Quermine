using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Quermine
{
    public abstract class ConnectionInfo<T> where T : DbClient
	{
		public readonly string Host;
		public readonly string Username;
		public readonly string Password;
		public readonly string Database;
		public readonly int Port;

		public abstract string ConnectionString { get; }

		internal ConnectionInfo(string host, string username, string password, string database, int port)
		{
			this.Host = host;
			this.Username = username;
			this.Password = password;
			this.Database = database;
			this.Port = port;
		}

		public abstract Task<bool> TestConnection();

		public abstract Task<T> Connect();
	}
}
