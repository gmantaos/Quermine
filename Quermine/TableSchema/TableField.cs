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
		public Type Type;
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
			Type = type;
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

			if (fieldTypes.HasFlag(FieldTypes.AutoIncrement))
			{
				Key = KeyType.Primary;
			}

			if (fieldTypes.HasFlag(FieldTypes.PrimaryKey))
			{
				Null = false;
			}
		}
	}
}
