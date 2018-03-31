using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	internal class FromClause
	{
		Sequence tables;

		public FromClause(params string[] tables)
		{
			this.tables = new Sequence(tables);
		}

		FromClause(Sequence tables)
		{
			this.tables = tables;
		}

		public override string ToString()
		{
			return "FROM " + tables.ToString();
		}

		public static FromClause operator +(FromClause f1, FromClause f2)
		{
			if (f1 == null)
				return f2;
			if (f2 == null)
				return f1;
			return new FromClause(f1.tables + f2.tables);
		}
	}
}
