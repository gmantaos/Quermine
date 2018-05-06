using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace Quermine.MySql
{
	public class MySqlClient : DbClient
	{
		MySqlConnectionInfo connectionInfo;
		MySqlConnection conn;

		internal MySqlClient(MySqlConnectionInfo connectionInfo)
		{
			this.connectionInfo = connectionInfo;
			conn = new MySqlConnection(connectionInfo.ConnectionString);
		}

		public override ConnectionState State => conn.State;

		internal override QueryBuilder Builder => new MysqlQueryBuilder();

		public override void Dispose()
		{
			conn.Dispose();
		}

		internal override Task OpenAsync()
		{
			return conn.OpenAsync();
		}

		public override async Task<ResultSet> Execute(Query query)
		{
			using (MySqlCommand cmd = GetCommand(query))
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
			using (MySqlCommand cmd = GetCommand(query))
			{
				cmd.Connection = conn;
				int rowsAffected = await cmd.ExecuteNonQueryAsync();
				return new NonQueryResult(rowsAffected, cmd.LastInsertedId);
			}
		}

		public override async Task<List<NonQueryResult>> ExecuteTransaction(IsolationLevel isolationLevel, params Query[] queries)
		{
			List<NonQueryResult> results = new List<NonQueryResult>();
			MySqlTransaction transaction = await conn.BeginTransactionAsync(isolationLevel);

			foreach (Query query in queries)
			{
				try
				{
					MySqlCommand cmd = GetCommand(query);
					cmd.Connection = conn;

					int rowsAffected = await cmd.ExecuteNonQueryAsync();
					NonQueryResult res = new NonQueryResult(rowsAffected, cmd.LastInsertedId);

					results.Add(res);
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					return null;
				}
			}

			transaction.Commit();
			return results;
		}

		public override async Task<TableSchema> GetTableSchema(string table)
		{
			Query query = Sql.Query(string.Format("describe {0};", table));
			return new TableSchema(new MysqlResultsetParser(), await Execute(query));
		}

		MySqlCommand GetCommand(Query query)
		{
			MySqlCommand cmd = new MySqlCommand(query.QueryString);
			foreach (KeyValuePair<string, object> param in query.Parameters())
			{
				cmd.Parameters.Add(new MySqlParameter(param.Key, param.Value));
			}
			return cmd;
		}

		public override async Task<List<string>> GetTableNames()
		{
			Query query = Sql.Select("table_name")
								.From("information_schema.tables")
								.Where("table_schema", connectionInfo.Database);

			ResultSet tables = await Execute(query);

			return tables.Select(row => row.GetString("table_name")).ToList();
		}
	}
}
