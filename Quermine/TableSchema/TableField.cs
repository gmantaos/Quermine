using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	public class TableField
	{
		public string Name;
		public string Type;
		public int? Length;
		public bool Null;
		public KeyType Key;
		public object Default;

		public bool Unsigned;
		public bool Zerofill;
		public bool AutoIncrement;

		public TableField() { }

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

			if (length == null && type == typeof(string))
			{
				Length = 255;
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
	}
}
