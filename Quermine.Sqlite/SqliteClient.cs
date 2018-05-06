using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

		internal override QueryBuilder Builder => new SqliteQueryBuilder();

		public override void Dispose()
		{
			conn.Dispose();
		}

		public override async Task<ResultSet> Execute(Query query)
		{
			using (SQLiteCommand cmd = GetCommand(query))
			{
				cmd.Connection = conn;
				DbDataReader reader = await cmd.ExecuteReaderAsync();
				ResultSet result = new ResultSet(reader);

				while (await reader.ReadAsync())
				{
					ResultRow row = result.AddRow(reader);
					query.Row(row);
				}
				return result;
			}
		}

		public override async Task<NonQueryResult> ExecuteNonQuery(Query query)
		{
			using (SQLiteCommand cmd = GetCommand(query))
			{
				cmd.Connection = conn;
				int rowsAffected = await cmd.ExecuteNonQueryAsync();
				long lastInsertedId = await GetLastInsertedId();
				return new NonQueryResult(rowsAffected, lastInsertedId);
			}
		}

		public async Task<long> GetLastInsertedId()
		{
			using (SQLiteCommand cmd = new SQLiteCommand(@"SELECT last_insert_rowid()"))
			{
				cmd.Connection = conn;
				return (long)await cmd.ExecuteScalarAsync();
			}
		}

		public override async Task<List<NonQueryResult>> ExecuteTransaction(IsolationLevel isolationLevel, params Query[] queries)
		{
			List<NonQueryResult> results = new List<NonQueryResult>();

			SQLiteTransaction transaction = conn.BeginTransaction(isolationLevel);

			foreach (Query query in queries)
			{
				try
				{
					NonQueryResult res = await ExecuteNonQuery(query);

					results.Add(res);
				}
				catch (Exception)
				{
					transaction.Rollback();
					return null;
				}
			}

			transaction.Commit();
			return results;
		}

		public override async Task<List<string>> GetTableNames()
		{
			Query query = Sql.Select("name")
							 .From("sqlite_master")
							 .Where("type", "table");

			ResultSet tables = await Execute(query);

			return tables.Select(row => row.GetString("name")).ToList();
		}

		public override async Task<TableSchema> GetTableSchema(string table)
		{
			Query query = Sql.Query(string.Format("PRAGMA table_info({0});", table));
			return new TableSchema(new SqliteResultsetParser(), await Execute(query));
		}

		internal override Task OpenAsync()
		{
			return conn.OpenAsync();
		}

		SQLiteCommand GetCommand(Query query)
		{
			SQLiteCommand cmd = new SQLiteCommand(query.QueryString);
			foreach (KeyValuePair<string, object> param in query.Parameters())
			{
				cmd.Parameters.Add(new SQLiteParameter(param.Key, param.Value));
			}
			return cmd;
		}
	}
}
