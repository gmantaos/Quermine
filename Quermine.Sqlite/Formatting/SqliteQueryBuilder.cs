using System;
using System.Collections.Generic;
using System.Text;

namespace Quermine.Sqlite
{
    internal class SqliteQueryBuilder : QueryBuilder
	{
		public override string TableField(TableField field, bool includeKey = true)
		{
			StringBuilder str = new StringBuilder();
			str.AppendFormat("`{0}`", field.Name);
			str.Append(' ');
			str.Append(field.Type);
			if (field.Length != null)
				str.AppendFormat("({0})", field.Length);
			if (field.Unsigned)
				str.Append(" UNSIGNED");
			if (field.Zerofill)
				str.Append(" ZEROFILL");
			if (!field.Null)
				str.Append(" NOT NULL");
			else
				str.Append(" NULL");
			if (field.AutoIncrement)
				str.Append(" AUTOINCREMENT");
			if (field.Default != null)
				str.AppendFormat(" DEFAULT {0}", field.Default);
			if (includeKey && field.Key.HasFlag(KeyType.Primary))
				str.Append(" PRIMARY KEY");
			return str.ToString();
		}
	}
}
