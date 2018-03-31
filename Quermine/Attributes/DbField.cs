using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// Represents a column in a database table.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class DbField : Attribute
	{
		/// <summary>
		/// The name of the field.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// Whether this value should be included in the WHERE condition when serializing into 
		/// a DELETE or an UPDATE query.
		/// </summary>
		public readonly bool IsWhereCondition;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">The name of the field.</param>
		/// <param name="isWhereCondition">Whether this value should be included in the WHERE condition when serializing into </param>
		public DbField(string name = null, bool isWhereCondition = true)
		{
			Name = name;
			IsWhereCondition = isWhereCondition;
		}
	}
}
