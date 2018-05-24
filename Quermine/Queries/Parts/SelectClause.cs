using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	internal class SelectClause
	{
		Sequence columns;

		internal string ColumnSequence => columns.ToString();

		public SelectClause(params string[] columns)
		{
			this.columns = new Sequence(columns);
		}

		SelectClause(Sequence columns)
		{
			this.columns = columns;
		}

		public override string ToString()
		{
			string cols = columns.ToString();
			return "SELECT " + (string.IsNullOrWhiteSpace(cols) ? "*" : cols);
		}

		public static SelectClause operator +(SelectClause s1, SelectClause s2)
		{
			if (s1 == null)
				return s2;
			if (s2 == null)
				return s1;
			return new SelectClause(s1.columns + s2.columns);
		}
	}
}
