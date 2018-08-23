using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine.Tests
{
	public abstract class TestQueryProvider
	{
		public abstract Query Query(string queryString);

		public abstract CreateTableQuery CreateTable(string tableName);

		public abstract DeleteQuery Delete(string table);

		public abstract DeleteQuery<T> Delete<T>(T obj);

		public abstract InsertQuery Insert(string table);

		public abstract InsertQuery<T> Insert<T>(T obj);

		public abstract SelectQuery Select();

		public abstract SelectQuery Select(params string[] columns);

		public abstract SelectQuery<T> Select<T>() where T : new();

		public abstract UpdateQuery Update(params string[] tables);
	}
}
