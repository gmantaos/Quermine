using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// Holds the schema of a table from the database, in the form of an enumeration of its fields.
	/// </summary>
	public class TableSchema : IEnumerable<TableField>
	{
		internal List<TableField> fields;

		/// <summary>
		/// Get a field of the table's schema by its name, returning null if the field
		/// is not present in the schema.
		/// The match is case sensitive.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public TableField this[string field]
		{
			get
			{
				return fields.FirstOrDefault(f => f.Name == field);
			}
		}

		/// <summary>
		/// Initialize an empty table schema.
		/// </summary>
		public TableSchema()
		{
			fields = new List<TableField>();
		}

		internal TableSchema(ResultsetParser parser, ResultSet table) : this()
		{
			fields = table.Select(field => parser.TableField(field)).ToList();
		}

		/// <summary>
		/// Get the enumerator of the list of fields.
		/// </summary>
		/// <returns></returns>
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
