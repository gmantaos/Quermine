using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	internal class SetClause
	{
		Sequence clauses;

		SetClause(Sequence clauses)
		{
			this.clauses = clauses;
		}

		internal SetClause(string column, string value)
		{
			clauses = new Sequence(column + " = " + value);
		}

		public override string ToString()
		{
			return "SET " + clauses.ToString();
		}

		public static SetClause operator +(SetClause f1, SetClause f2)
		{
			if (f1 == null)
				return f2;
			if (f2 == null)
				return f1;
			return new SetClause(f1.clauses + f2.clauses);
		}
	}
}
