using System;

using Quermine;
using Quermine.MySql;

using NUnit.Framework;

namespace Quermine.Tests
{
	[TestFixture]
	public class MySqlTests
	{
		[Test]
		public void TestMethod1()
		{
			SelectQuery q = Sql.Select();

			q.From("table1 T1", "table2 T2")
				.Where("T1.length", WhereRelation.NotEqual, 4)
				.OrderBy("T2.name DESC")
				.Limit(10);

			Console.WriteLine(q.ParametrizedQueryString());
		}
	}
}
