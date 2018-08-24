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
		public int? Precision;
		public bool NotNull { get; set; }
		public KeyType Key;
		public object DefaultValue;

		public bool Unsigned;
		public bool Zerofill;
		public bool AutoIncrement;

		public TableField() { }

		internal TableField(string fieldName, Type type, int? length = null, int? precision = null, FieldProperties fieldProperties = 0, object defaultValue = null)
		{
			Name = fieldName;
			Length = length;
			Precision = precision;
			Type = type;
			DefaultValue = defaultValue;
			NotNull = fieldProperties.HasFlag(FieldProperties.NotNull);
			if (fieldProperties.HasFlag(FieldProperties.PrimaryKey))
			{
				Key = KeyType.Primary;
			}
			Unsigned = fieldProperties.HasFlag(FieldProperties.Unsigned);
			Zerofill = fieldProperties.HasFlag(FieldProperties.Zerofill);
			AutoIncrement = fieldProperties.HasFlag(FieldProperties.AutoIncrement);

			if (length == null && type == typeof(string))
			{
				Length = 255;
			}

			if (fieldProperties.HasFlag(FieldProperties.AutoIncrement))
			{
				Key = KeyType.Primary;
			}

			if (fieldProperties.HasFlag(FieldProperties.PrimaryKey))
			{
				NotNull = true;
			}
		}
	}
}
