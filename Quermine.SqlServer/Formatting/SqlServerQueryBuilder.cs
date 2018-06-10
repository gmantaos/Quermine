using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quermine.SqlServer
{
    internal class SqlServerQueryBuilder : QueryBuilder
    {
		public override string SelectQuery(SelectQuery query)
		{
			StringBuilder str = new StringBuilder();
			str.Append(query.selectClause.ToString());
			str.Append("\n");

			str.Append(query.fromClause.ToString());
			str.Append("\n");

			str.Append(query.ConditionalQueryPart());

			return str.ToString();
		}

		public override string DeleteQuery(DeleteQuery query)
		{
			StringBuilder str = new StringBuilder();

			str.Append("DELETE ");

			if (query.limit > 0)
			{
				str.AppendFormat("TOP {0} ", query.limit);
			}

			str.Append(ModifiersQueryPart(query, true, true));

			str.Append("\n");

			str.Append(query.fromClause.ToString());
			str.Append("\n");

			str.Append(query.ConditionalQueryPart());

			return str.ToString();
		}

		public override string UpdateQuery(UpdateQuery query)
		{
			StringBuilder str = new StringBuilder();

			str.Append("UPDATE ");

			if (query.limit > 0)
			{
				str.AppendFormat("TOP {0} ", query.limit);
			}

			str.Append(ModifiersQueryPart(query, true, true));

			str.Append("\n");

			str.Append(query.tables.ToString());
			str.Append("\n");

			str.Append(query.setClause.ToString());
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

			// OFFSET
			if (cond is SelectQuery
				&& (cond.limit > 0 || (cond as SelectQuery).offset > 0))
			{
				/*
				 * An offset requires SQL Server 2012+
				 */
				if (cond.orderByClause == null)
					query.Append("ORDER BY (SELECT 0)\n");


				SelectQuery sel = cond as SelectQuery;

				query.AppendFormat("OFFSET {0} ROWS\n", sel.offset);

				// LIMIT
				if (cond.limit > 0)
				{
					query.AppendFormat("FETCH NEXT {0} ROWS ONLY\n", cond.limit);
				}
			}

			return query.ToString();
		}

		public override string CreateTableQuery(CreateTableQuery query)
		{
			StringBuilder str = new StringBuilder();

			str.AppendFormat("CREATE TABLE {0} (\n", query.tableName);

			StringBuilder fields = new StringBuilder();

			foreach (TableField field in query.fields)
			{
				if (fields.Length > 0)
					fields.AppendLine(",");

				fields.AppendFormat("\t{0}", TableField(field, true));
			}

			str.Append(fields)
			   .Append("\n);");

			return str.ToString();
		}

		public override string TableField(TableField field, bool includeKey = true)
		{
			StringBuilder str = new StringBuilder();
			str.AppendFormat("{0}", field.Name);
			str.Append(' ');
			str.Append(FieldType(field.Type));
			if (field.Length != null)
				str.AppendFormat("({0})", field.Length);

			if (field.AutoIncrement)
				str.Append(" IDENTITY(1,1)");
			else if (!field.Null)
				str.Append(" NOT NULL");
			else
				str.Append(" NULL");

			if (includeKey && field.Key.HasFlag(KeyType.Primary))
				str.Append(" PRIMARY KEY");

			if (field.Default != null)
				str.AppendFormat(" DEFAULT {0}", field.Default);

			return str.ToString();
		}
	}
}
