using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queerie.Abstract
{
	public abstract class TableSchema<T> : IEnumerable<T> where T : TableField
	{
		IEnumerable<T> fields;

		TableSchema()
		{
			fields = new List<T>();
		}

		internal TableSchema(ResultSet table)
		{
			fields = table.Select(field => GetTableField(field));
		}

		protected abstract T GetTableField(ResultRow row);

		public IEnumerator<T> GetEnumerator()
		{
			return fields.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return fields.GetEnumerator();
		}
	}
}
