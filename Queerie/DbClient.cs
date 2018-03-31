using System;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Queerie
{
    public abstract class DbClient : IDisposable
	{
		public abstract ConnectionState State { get; }

		public abstract Task OpenAsync();

		public abstract void Dispose();
	}
}
