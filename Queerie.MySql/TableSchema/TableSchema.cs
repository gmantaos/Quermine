using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queerie.MySql
{
	public class TableSchema : Abstract.TableSchema<TableField>
	{
		internal TableSchema(ResultSet table) : base(table) { }

		protected override TableField GetTableField(ResultRow row)
		{
			return new TableField(row);
		}
	}
}
