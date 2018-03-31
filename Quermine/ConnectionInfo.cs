using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Quermine
{
    public abstract class ConnectionInfo<T> where T : DbClient
	{
		protected string host;
		protected string username;
		protected string password;
		protected string database;
		protected int port;

		protected abstract string ConnectionString { get; }

		internal ConnectionInfo(string host, string username, string password, string database, int port)
		{
			this.host = host;
			this.username = username;
			this.password = password;
			this.database = database;
			this.port = port;
		}

		public abstract Task<bool> TestConnection();

		public abstract Task<T> Connect();
	}
}
