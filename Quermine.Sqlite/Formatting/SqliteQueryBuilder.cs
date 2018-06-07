using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Quermine.Sqlite
{
    internal class SqliteQueryBuilder : QueryBuilder
	{
		public override string CreateTableQuery(CreateTableQuery query)
		{
			StringBuilder str = new StringBuilder();

			str.AppendFormat("CREATE TABLE \"{0}\" (\n", query.tableName);

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
			str.AppendFormat("\"{0}\"", field.Name);
			str.Append(' ');
			str.Append(FieldType(field.Type));
			if (field.Length != null)
				str.AppendFormat("({0})", field.Length);

			if (includeKey && field.Key.HasFlag(KeyType.Primary))
				str.Append(" PRIMARY KEY");

			if (field.AutoIncrement)
				str.Append(" AUTOINCREMENT");
			else if (!field.Null)
				str.Append(" NOT NULL");
			else
				str.Append(" NULL");

			if (field.Default != null)
				str.AppendFormat(" DEFAULT {0}", field.Default);
			
			return str.ToString();
		}

		protected override string FieldType(Type type)
		{
			if (type == typeof(int))
				return "INTEGER";
			else if (type == typeof(uint))
				return "INTEGER";
			else
				return base.FieldType(type);
		}
	}
}
