using System;
using System.Collections.Generic;
using System.Text;

namespace Quermine.SqlServer
{
    public static class QueryProvider
    {
		static SqlServerQueryBuilder builder = new SqlServerQueryBuilder();

		public static Query Query(string queryString)
		{
			return new Query(builder, queryString);
		}

		public static CreateTableQuery CreateTable(string tableName)
		{
			return new CreateTableQuery(builder, tableName);
		}

		public static CreateTableQuery<T> CreateTable<T>() where T : new()
		{
			return new CreateTableQuery<T>(builder);
		}

		public static DeleteQuery Delete(string table)
		{
			return new DeleteQuery(builder, table);
		}

		public static DeleteQuery<T> Delete<T>(T obj)
		{
			return new DeleteQuery<T>(builder, obj);
		}

		public static InsertQuery Insert(string table)
		{
			return new InsertQuery(builder, table);
		}

		public static InsertQuery<T> Insert<T>(T obj)
		{
			return new InsertQuery<T>(builder, obj);
		}

		public static SelectQuery Select()
		{
			return new SelectQuery(builder);
		}

		public static SelectQuery Select(params string[] columns)
		{
			return new SelectQuery(builder, columns);
		}

		public static SelectQuery<T> Select<T>() where T: new()
		{
			return new SelectQuery<T>(builder);
		}

		public static UpdateQuery Update(params string[] tables)
		{
			return new UpdateQuery(builder, tables);
		}
	}
}
