using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Quermine
{
    public abstract class ConnectionInfo<T> where T : DbClient
	{
		public abstract string ConnectionString { get; }

		public abstract Task<bool> TestConnection();

		public abstract Task<T> Connect();
	}
}
