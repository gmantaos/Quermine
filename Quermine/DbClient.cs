using System;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Quermine
{
    public abstract class DbClient : IDisposable
	{
		internal abstract Task OpenAsync();

		/// <summary>
		/// The current state of the underlying connection of this client.
		/// </summary>
		public abstract ConnectionState State { get; }

		public abstract void Dispose();

		public abstract Task<ResultSet> Execute(Query query);

		public abstract Task<NonQueryResult> ExecuteNonQuery(Query query);

		public abstract Task<List<T>> Execute<T>(SelectQuery<T> query) where T : new();

		public abstract Task<List<NonQueryResult>> ExecuteTransaction(IsolationLevel isolationLevel, params Query[] queries);

		public abstract Task<TableSchema> GetTableSchema(string table);

		public Task ExecuteTransaction(params Query[] queries)
		{
			return ExecuteTransaction(IsolationLevel.Unspecified, queries);
		}
	}
}
