using System;
using System.Collections.Generic;
using System.Text;

namespace Quermine.SqlServer
{
    internal class SqlServerQueryBuilder : QueryBuilder
    {
		public override string SelectQuery(SelectQuery query)
		{
			StringBuilder str = new StringBuilder();
			str.Append("SELECT ");

			// TOP
			if (query.limit > 0)
			{
				str.AppendFormat("TOP {0} ", query.limit);
			}

			str.Append(query.selectClause.ColumnSequence);
			str.Append("\n");

			str.Append(query.fromClause.ToString());
			str.Append("\n");

			str.Append(query.ConditionalQueryPart());

			return str.ToString();
		}

		public override string ConditionalQueryPart(ConditionalQuery cond)
		{
			StringBuilder query = new StringBuilder();

			// WHERE
			if (cond.whereClause != null)
			{
				query.Append("WHERE ");
				query.Append(cond.whereClause.ToString());
				query.Append("\n");
			}

			// ORDER BY
			if (cond.orderByClause != null)
			{
				query.Append(cond.orderByClause.ToString());
				query.Append("\n");
			}

			return query.ToString();
		}
	}
}
