using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quermine
{
    internal class ResultsetParser
    {
		public virtual TableField TableField(ResultRow field)
		{

			TableField tableField = new TableField()
			{
				Name = field.GetString("Field"),
				Type = field.GetString("Type").Split('(')[0],
				Null = field.GetString("Null").Equals("YES"),
				Key = ParseKey(field.GetString("Key")),
				Default = field["Default"],
				Unsigned = field.GetString("Type").Split(' ').Contains("unsigned"),
				Zerofill = field.GetString("Type").Split(' ').Contains("zerofill"),
				AutoIncrement = field.GetString("Extra").Split(' ').Contains("auto_increment"),
			};

			if (field.GetString("Type").Split('(').Length > 1)
			{
				int closingPar = field.GetString("Type").Split('(')[1].IndexOf(')');
				tableField.Length = int.Parse(field.GetString("Type").Split('(')[1].Substring(0, closingPar));
			}

			return tableField;
		}

		protected virtual KeyType ParseKey(string key)
		{
			switch (key)
			{
				case "PRI":
					return KeyType.Primary;
				default:
					return KeyType.None;
			}
		}
	}
}
