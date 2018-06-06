using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quermine.Sqlite;

namespace Quermine.Tests
{
	public class SqliteQueryProvider : QueryProvider
	{
		public override Query Query(string queryString)
		{
			return Sql.Query(queryString);
		}

		public override CreateTableQuery CreateTable(string tableName)
		{
			return Sql.CreateTable(tableName);
		}

		public override DeleteQuery Delete(string table)
		{
			return Sql.Delete(table);
		}

		public override DeleteQuery<T> Delete<T>(T obj)
		{
			return Sql.Delete<T>(obj);
		}

		public override InsertQuery Insert(string table)
		{
			return Sql.Insert(table);
		}

		public override InsertQuery<T> Insert<T>(T obj)
		{
			return Sql.Insert<T>(obj);
		}

		public override SelectQuery Select()
		{
			return Sql.Select();
		}

		public override SelectQuery Select(params string[] columns)
		{
			return Sql.Select(columns);
		}

		public override SelectQuery<T> Select<T>()
		{
			return Sql.Select<T>();
		}

		public override UpdateQuery Update(params string[] tables)
		{
			return Sql.Update(tables);
		}
	}
}
