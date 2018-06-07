using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Quermine.Sqlite
{
    internal class SqliteResultsetParser : ResultsetParser
	{
		public override TableField TableField(ResultRow field)
		{
			TableField tableField = new TableField()
			{
				Name = field.GetString("name"),
				Type = ParseType(field.GetString("type")),
				Null = field.GetInteger("notnull") == 0,
				Key = field.GetInteger("pk") == 1 ? KeyType.Primary : KeyType.None,
				Default = field["dflt_value"],
				Unsigned = field.GetString("type").Split(' ').Contains("unsigned"),
				Zerofill = field.GetString("type").Split(' ').Contains("zerofill"),
				AutoIncrement = false
			};

			if (field.GetString("type").Split('(').Length > 1)
			{
				int closingPar = field.GetString("type").Split('(')[1].IndexOf(')');
				tableField.Length = int.Parse(field.GetString("type").Split('(')[1].Substring(0, closingPar));
			}

			return tableField;
		}
	}
}
