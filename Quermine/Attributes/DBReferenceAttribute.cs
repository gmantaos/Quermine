using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// This attribute indicates that this field should be serialized from another DB table
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class DbReferenceAttribute : Attribute
	{
		/// <summary>
		/// The column of this table whose value is pointing to row(s) of the other table.
		/// </summary>
		public readonly string Column;

		/// <summary>
		/// The column of the other table.
		/// </summary>
		public readonly string ForeignColumn;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="column">The column of this table whose value is pointing to row(s) of the other table.</param>
		/// <param name="foreignColumn">The column of the other table.</param>
		public DbReferenceAttribute(string column, string foreignColumn)
		{
			Column = column;
			ForeignColumn = foreignColumn;
		}
	}
}
