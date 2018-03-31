using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queerie.Abstract
{
	public abstract class TableField
	{
		public string Name { get; protected set; }
		public string Type { get; protected set; }
		public int? Length { get; protected set; }
		public bool Null { get; protected set; }
		public KeyType Key { get; protected set; }
		public object Default { get; protected set; }

		public bool Unsigned { get; protected set; }
		public bool Zerofill { get; protected set; }
		public bool AutoIncrement { get; protected set; }

		protected TableField() { }

		internal TableField(string fieldName, Type type, int? length = null, FieldTypes fieldTypes = 0, object defaultVal = null)
		{
			Name = fieldName;
			Length = length;
			Type = ParseType(type);
			Default = defaultVal;
			Null = !fieldTypes.HasFlag(FieldTypes.NotNull);
			if (fieldTypes.HasFlag(FieldTypes.PrimaryKey))
			{
				Key = KeyType.Primary;
			}
			Unsigned = fieldTypes.HasFlag(FieldTypes.Unsigned);
			Zerofill = fieldTypes.HasFlag(FieldTypes.Zerofill);
			AutoIncrement = fieldTypes.HasFlag(FieldTypes.AutoIncrement);
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

		protected virtual string ParseType(Type type)
		{
			if (type == typeof(byte))
				return "TINYINT";
			else if (type == typeof(short))
				return "SMALLINT";
			else if (type == typeof(int))
				return "INT";
			else if (type == typeof(uint))
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

		public abstract string ToString(bool includeKey = true);
	}
}
