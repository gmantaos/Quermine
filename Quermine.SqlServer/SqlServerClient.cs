﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Quermine.SqlServer
{
	/// <inheritdoc />
	public class SqlServerClient : DbClient
	{
		readonly SqlServerConnectionInfo connectionInfo;
		SqlConnection conn;

		internal SqlServerClient(SqlServerConnectionInfo connectionInfo)
		{
			this.connectionInfo = connectionInfo;
			conn = new SqlConnection(connectionInfo.ConnectionString);
		}

		/// <inheritdoc />
		public override ConnectionState State => conn.State;

		internal override QueryBuilder Builder => new SqlServerQueryBuilder();

		/// <inheritdoc />
		public override void Dispose()
		{
			conn.Dispose();
		}

		internal override Task OpenAsync()
		{
			return conn.OpenAsync();
		}

		/// <inheritdoc />
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

				reader.Close();

				return result;
			}
		}

		/// <inheritdoc />
		public override async Task<NonQueryResult> ExecuteNonQuery(Query query)
		{
			using (SqlCommand cmd = GetCommand(query))
			{
				cmd.Connection = conn;
				int rowsAffected = await cmd.ExecuteNonQueryAsync();
				return new NonQueryResult(rowsAffected, -1);
			}
		}

		/// <inheritdoc />
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
					cmd.Transaction = transaction;

					int rowsAffected = await cmd.ExecuteNonQueryAsync();
					NonQueryResult res = new NonQueryResult(rowsAffected, -1);

					results.Add(res);
				}
				catch (Exception ex)
				{
					transaction.Rollback();

					throw ex;
				}
			}

			transaction.Commit();
			return results;
		}

		/// <inheritdoc />
		public override async Task<TableSchema> GetTableSchema(string table)
		{
			Query query = QueryProvider.Select("*")
							 .From("INFORMATION_SCHEMA.COLUMNS")
							 .Where("TABLE_NAME", table)
							 .OrderBy("ORDINAL_POSITION");

			ResultSet rs = await Execute(query);

			TableSchema schema = new TableSchema(new SqlServerResultsetParser(), rs);

			return schema;
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

		/// <inheritdoc />
		public override async Task<List<string>> GetTableNames()
		{
			Query query = QueryProvider.Select("TABLE_NAME")
			                 .From("INFORMATION_SCHEMA.TABLES")
							 .Where("TABLE_TYPE", "BASE TABLE");

			ResultSet tables = await Execute(query);

			return tables.Select(row => row.GetString("TABLE_NAME")).ToList();
		}

		/// <inheritdoc />
		public override async Task<object> ExecuteScalar(Query query)
		{
			using (SqlCommand cmd = GetCommand(query))
			{
				cmd.Connection = conn;
				return await cmd.ExecuteScalarAsync();
			}
		}

		/// <inheritdoc />
		public override Task DropTableIfExists(string tableName)
		{
			return ExecuteNonQuery(string.Format("IF OBJECT_ID('{0}', 'U') IS NOT NULL DROP TABLE {0}", tableName));
		}
	}
}
