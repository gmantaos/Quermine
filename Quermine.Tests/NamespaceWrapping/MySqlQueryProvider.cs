using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quermine.MySql;

namespace Quermine.Tests
{
	public class MySqlQueryProvider : TestQueryProvider
	{
		public override Query Query(string queryString)
		{
			return QueryProvider.Query(queryString);
		}

		public override CreateTableQuery CreateTable(string tableName)
		{
			return QueryProvider.CreateTable(tableName);
		}

		public override CreateTableQuery<T> CreateTable<T>()
		{
			return QueryProvider.CreateTable<T>();
		}

		public override DeleteQuery Delete(string table)
		{
			return QueryProvider.Delete(table);
		}

		public override DeleteQuery<T> Delete<T>(T obj)
		{
			return QueryProvider.Delete<T>(obj);
		}

		public override InsertQuery Insert(string table)
		{
			return QueryProvider.Insert(table);
		}

		public override InsertQuery<T> Insert<T>(T obj)
		{
			return QueryProvider.Insert<T>(obj);
		}

		public override SelectQuery Select()
		{
			return QueryProvider.Select();
		}

		public override SelectQuery Select(params string[] columns)
		{
			return QueryProvider.Select(columns);
		}

		public override SelectQuery<T> Select<T>()
		{
			return QueryProvider.Select<T>();
		}

		public override UpdateQuery Update(params string[] tables)
		{
			return QueryProvider.Update(tables);
		}
	}
}
