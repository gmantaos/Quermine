using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;

namespace Quermine.Sqlite
{
	public class SqliteClient : DbClient
	{
		SqliteConnectionInfo connectionInfo;
		SQLiteConnection conn;

		internal SqliteClient(SqliteConnectionInfo connectionInfo)
		{
			this.connectionInfo = connectionInfo;
			conn = new SQLiteConnection(connectionInfo.ConnectionString);
		}

		public override ConnectionState State => conn.State;

		internal override QueryBuilder Builder => throw new NotImplementedException();

		public override void Dispose()
		{
			conn.Dispose();
		}

		public override Task<ResultSet> Execute(Query query)
		{
			throw new NotImplementedException();
		}

		public override Task<List<T>> Execute<T>(SelectQuery<T> query)
		{
			throw new NotImplementedException();
		}

		public override Task<NonQueryResult> ExecuteNonQuery(Query query)
		{
			throw new NotImplementedException();
		}

		public override Task<List<NonQueryResult>> ExecuteTransaction(IsolationLevel isolationLevel, params Query[] queries)
		{
			throw new NotImplementedException();
		}

		public override Task<List<string>> GetTableNames()
		{
			throw new NotImplementedException();
		}

		public override Task<TableSchema> GetTableSchema(string table)
		{
			throw new NotImplementedException();
		}

		internal override Task OpenAsync()
		{
			return conn.OpenAsync();
		}
	}
}
