using System;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Quermine.Sqlite
{
	public class SqliteConnectionInfo : ConnectionInfo<SqliteClient>
	{
		public readonly string DataSource;
		public readonly int Version = 3;

		public override string ConnectionString => string.Format("Data Source={0};Version={1};", DataSource, Version);

		public SqliteConnectionInfo(string dataSource, int version = 3)
		{
			DataSource = dataSource;
			Version = version;
		}

		public override async Task<SqliteClient> Connect()
		{
			SqliteClient client = new SqliteClient(this);
			await client.OpenAsync();
			return client;
		}

		public override async Task<bool> TestConnection()
		{
			using (SqliteClient client = new SqliteClient(this))
			{
				await client.OpenAsync();
				return client.State == ConnectionState.Open;
			}
		}

		public SqliteConnectionInfo Create()
		{
			SQLiteConnection.CreateFile(DataSource);
			return this;
		}
	}
}
