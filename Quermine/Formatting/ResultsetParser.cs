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
				Type = ParseType(field.GetString("Type")),
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

		protected virtual Type ParseType(string type)
		{
			type = type.ToLower();
			bool unsigned = type.Contains("unsigned");

			if (type.Contains("tinyint"))
				return typeof(byte);
			else if (type.Contains("smallint"))
				return unsigned ? typeof(ushort) : typeof(short);
			else if (type.Contains("bigint"))
				return unsigned ? typeof(ulong) : typeof(long);
			else if (type.Contains("int"))
				return unsigned ? typeof(uint) : typeof(int);
			else if (type.Contains("text")
				|| type.Contains("varchar"))
				return typeof(string);
			else if (type.Contains("double"))
				return typeof(double);
			else if (type.Contains("float")
				|| type.Contains("real"))
				return typeof(float);
			else if (type.Contains("decimal"))
				return typeof(decimal);
			else if (type.Contains("date")
				|| type.Contains("time"))
				return typeof(DateTime);
			else if (type.Contains("char"))
				return typeof(char);
			else if (type.Contains("blob")
				|| type.Contains("binary"))
				return typeof(byte[]);
			else
				return null; // TODO: probably do something about this
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
