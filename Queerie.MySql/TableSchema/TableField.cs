using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queerie.MySql
{
	public class TableField : Abstract.TableField
	{
		internal TableField(ResultRow field)
		{
			Name = field.GetString("Field");
			Type = field.GetString("Type").Split('(')[0];
			if (field.GetString("Type").Split('(').Length > 1)
			{
				int closingPar = field.GetString("Type").Split('(')[1].IndexOf(')');
				Length = int.Parse(field.GetString("Type").Split('(')[1].Substring(0, closingPar));
			}
			Null = field.GetString("Null").Equals("YES");
			Key = ParseKey(field.GetString("Key"));
			Default = field["Default"];
			Unsigned = field.GetString("Type").Split(' ').Contains("unsigned");
			Zerofill = field.GetString("Type").Split(' ').Contains("zerofill");
			AutoIncrement = field.GetString("Extra").Split(' ').Contains("auto_increment");
		}

		public override string ToString(bool includeKey = true)
		{
			StringBuilder field = new StringBuilder();
			field.AppendFormat("`{0}`", Name);
			field.Append(' ');
			field.Append(Type);
			if (Length != null)
				field.AppendFormat("({0})", Length);
			if (Unsigned)
				field.Append(" UNSIGNED");
			if (Zerofill)
				field.Append(" ZEROFILL");
			if (!Null)
				field.Append(" NOT NULL");
			if (AutoIncrement)
				field.Append(" AUTO_INCREMENT");
			if (Default != null)
				field.AppendFormat(" DEFAULT {0}", Default);
			if (includeKey && Key.HasFlag(KeyType.Primary))
				field.Append(" PRIMARY KEY");
			return field.ToString();
		}
	}
}
