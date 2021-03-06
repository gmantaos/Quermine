﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quermine
{
    internal class QueryBuilder
    {
		public virtual string Query(Query query)
		{
			return query.QueryString;
		}

		public virtual string SelectQuery(SelectQuery query)
		{
			StringBuilder str = new StringBuilder();
			str.Append(query.selectClause.ToString());
			str.Append("\n");

			str.Append(query.fromClause.ToString());
			str.Append("\n");

			str.Append(query.ConditionalQueryPart());

			// OFFSET
			if (query.offset > 0)
			{
				str.AppendFormat("OFFSET {0}", query.offset);
				str.Append("\n");
			}

			return str.ToString();
		}

		public virtual string CreateTableQuery(CreateTableQuery query)
		{
			StringBuilder str = new StringBuilder();

			str.AppendFormat("CREATE TABLE `{0}` (\n", query.tableName);

			foreach (TableField field in query.fields)
			{
				str.AppendFormat("\t{0},\n", TableField(field, false));
			}

			StringBuilder keys = new StringBuilder();
			foreach (TableField key in query.fields.Where(f => f.Key.HasFlag(KeyType.Primary)))
			{
				if (keys.Length > 0)
					keys.Append(", ");
				keys.AppendFormat("`{0}`", key.Name);
			}

			if (keys.Length > 0)
				str.AppendFormat("\tPRIMARY KEY ({0})", keys.ToString());

			str.Append("\n);");
			return str.ToString();
		}

		public virtual string DeleteQuery(DeleteQuery query)
		{
			StringBuilder str = new StringBuilder();

			str.Append("DELETE ");

			str.Append(ModifiersQueryPart(query, true, true));

			str.Append("\n");

			str.Append(query.fromClause.ToString());
			str.Append("\n");

			str.Append(query.ConditionalQueryPart());

			return str.ToString();
		}

		public virtual string UpdateQuery(UpdateQuery query)
		{
			StringBuilder str = new StringBuilder();

			str.Append("UPDATE ");

			str.Append(ModifiersQueryPart(query, true, true));

			str.Append("\n");

			str.Append(query.tables.ToString());
			str.Append("\n");

			str.Append(query.setClause.ToString());
			str.Append("\n");

			str.Append(query.ConditionalQueryPart());

			return str.ToString();
		}

		public virtual string InsertQuery(InsertQuery query)
		{
			StringBuilder str = new StringBuilder();
			str.Append(query.replace ? "REPLACE" : "INSERT");
			str.Append(" ");

			str.Append(ModifiersQueryPart(query, true, !query.replace));

			str.Append("\n");

			str.Append("INTO ");
			str.Append(query.table);
			str.Append("\n");

			str.AppendFormat("({0})", query.columns.ToString());
			str.Append("\n");

			str.Append("VALUES ");

			str.AppendFormat("({0})", query.values.ToString());
			str.Append("\n");

			return str.ToString();
		}

		public virtual string ConditionalQueryPart(ConditionalQuery cond)
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

			// LIMIT
			if (cond.limit > 0)
			{
				query.AppendFormat("LIMIT {0}", cond.limit);
				query.Append("\n");
			}

			return query.ToString();
		}

		public virtual string TableField(TableField field, bool includeKey = true)
		{
			StringBuilder str = new StringBuilder();
			str.AppendFormat("`{0}`", field.Name);
			str.Append(' ');
			str.Append(FieldType(field.Type));

			str.Append(FieldLength(field));

			if (field.Unsigned)
				str.Append(" UNSIGNED");
			if (field.Zerofill)
				str.Append(" ZEROFILL");
			if (field.NotNull)
				str.Append(" NOT NULL");
			else
				str.Append(" NULL");
			if (field.AutoIncrement)
				str.Append(" AUTO_INCREMENT");
			if (field.DefaultValue != null)
				str.AppendFormat(" DEFAULT {0}", field.DefaultValue);
			if (includeKey && field.Key.HasFlag(KeyType.Primary))
				str.Append(" PRIMARY KEY");
			return str.ToString();
		}

		protected string FieldLength(TableField field)
		{
			StringBuilder str = new StringBuilder();
			if (field.Length != null && field.Length > 0)
			{
				str.Append("(")
				   .Append(field.Length);

				if (field.Precision != null && field.Precision > 0)
				{
					str.Append(",")
					   .Append(field.Precision);
				}

				str.Append(")");
			}
			return str.ToString();
		}

		protected virtual string FieldType(Type type)
		{
			if (type == typeof(byte))
				return "TINYINT";
			else if (type == typeof(short))
				return "SMALLINT";
			else if (type == typeof(int))
				return "INT";
			else if (type == typeof(uint))
				return "INT";
			else if (type == typeof(long))
				return "INT";
			else if (type == typeof(double))
				return "DOUBLE";
			else if (type == typeof(float))
				return "FLOAT";
			else if (type == typeof(decimal))
				return "DECIMAL";
			else if (type == typeof(DateTime))
				return "DATETIME";
			else if (type == typeof(char))
				return "CHAR";
			else if (type == typeof(string))
				return "VARCHAR";
			else if (type == typeof(byte[]))
				return "BLOB";
			else
				throw new ArgumentException("Cannot convert to Sql type: " + type.ToString());
		}

		protected string ModifiersQueryPart(Query query, bool lowPriority, bool ignore)
		{
			StringBuilder str = new StringBuilder();

			if (query.lowPriority && lowPriority)
				str.Append("LOW_PRIORITY ");
			if (query.ignore && ignore)
				str.Append("IGNORE ");

			return str.ToString();
		}
	}
}
