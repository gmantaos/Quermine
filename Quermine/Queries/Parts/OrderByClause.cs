using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	public class OrderByClause
	{
		Sequence columns;

		public OrderByClause(params string[] columns)
		{
			this.columns = new Sequence(columns);
		}

		OrderByClause(Sequence columns)
		{
			this.columns = columns;
		}

		public override string ToString()
		{
			return "ORDER BY " + columns.ToString();
		}

		public static OrderByClause operator +(OrderByClause s1, OrderByClause s2)
		{
			if (s1 == null)
				return s2;
			if (s2 == null)
				return s1;
			return new OrderByClause(s1.columns + s2.columns);
		}
	}
}
