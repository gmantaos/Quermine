using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Quermine.SqlServer
{
    internal class SqlServerResultsetParser : ResultsetParser
    {
		public override TableField TableField(ResultRow field)
		{

			TableField tableField = new TableField()
			{
				Name = field.GetString("TABLE_NAME"),
				Type = ParseType(field.GetString("DATA_TYPE")),
				Null = field.GetString("IS_NULLABLE").Equals("YES"),
				//Key = ParseKey(field.GetString("Key")),  ???
				Default = field["COLUMN_DEFAULT"],
				//Unsigned = field.GetString("Type").Split(' ').Contains("unsigned"),
				//Zerofill = field.GetString("Type").Split(' ').Contains("zerofill"),
				//AutoIncrement = field.GetString("Extra").Split(' ').Contains("auto_increment"),
				Length = field.GetInteger("CHARACTER_MAXIMUM_LENGTH", 0)
			};

			return tableField;
		}
	}
}
