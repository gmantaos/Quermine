using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Quermine.SqlServer
{
	public class SqlServerClient : DbClient
	{
		SqlServerConnectionInfo connectionInfo;
		SqlConnection conn;

		internal SqlServerClient(SqlServerConnectionInfo connectionInfo)
		{
			this.connectionInfo = connectionInfo;
			conn = new SqlConnection(connectionInfo.ConnectionString);
		}

		public override ConnectionState State => conn.State;

		internal override QueryBuilder Builder => new SqlServerQueryBuilder();

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
			using (SqlCommand cmd = GetCommand(query))
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
			using (SqlCommand cmd = GetCommand(query))
			{
				cmd.Connection = conn;
				int rowsAffected = await cmd.ExecuteNonQueryAsync();
				return new NonQueryResult(rowsAffected, -1);
			}
		}

		public override async Task<List<NonQueryResult>> ExecuteTransaction(IsolationLevel isolationLevel, params Query[] queries)
		{
			List<NonQueryResult> results = new List<NonQueryResult>();
			SqlTransaction transaction = conn.BeginTransaction(isolationLevel);

			foreach (Query query in queries)
			{
				try
				{
					SqlCommand cmd = GetCommand(query);
					cmd.Connection = conn;

					int rowsAffected = await cmd.ExecuteNonQueryAsync();
					NonQueryResult res = new NonQueryResult(rowsAffected, -1);

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
			Query query = Sql.Select("*")
							 .From("INFORMATION_SCHEMA.COLUMNS")
							 .Where("TABLE_NAME", table)
							 .OrderBy("ORDINAL_POSITION");

			return new TableSchema(new SqlServerResultsetParser(), await Execute(query));
		}

		SqlCommand GetCommand(Query query)
		{
			SqlCommand cmd = new SqlCommand(query.QueryString);
			foreach (KeyValuePair<string, object> param in query.Parameters())
			{
				cmd.Parameters.Add(new SqlParameter(param.Key, param.Value));
			}
			return cmd;
		}

		public override async Task<List<string>> GetTableNames()
		{
			Query query = Sql.Select("TABLE_NAME")
			                 .From("INFORMATION_SCHEMA.TABLES")
							 .Where("TABLE_TYPE", "BASE TABLE");

			ResultSet tables = await Execute(query);

			return tables.Select(row => row.GetString("table_name")).ToList();
		}

		public override async Task<object> ExecuteScalar(Query query)
		{
			using (SqlCommand cmd = GetCommand(query))
			{
				cmd.Connection = conn;
				return await cmd.ExecuteScalarAsync();
			}
		}
	}
}
