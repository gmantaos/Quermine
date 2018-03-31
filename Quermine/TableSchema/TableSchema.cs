using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	public class TableSchema : IEnumerable<TableField>
	{
		IEnumerable<TableField> fields;

		TableSchema()
		{
			fields = new List<TableField>();
		}

		internal TableSchema(ResultsetParser parser, ResultSet table) : this()
		{
			fields = table.Select(field => parser.TableField(field));
		}

		public IEnumerator<TableField> GetEnumerator()
		{
			return fields.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return fields.GetEnumerator();
		}
	}
}
