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
	public class DbFieldAttribute : Attribute
	{
		/// <summary>
		/// The name of the field.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">The name of the field.</param>
		public DbFieldAttribute(string name = null)
		{
			Name = name;
		}
	}
}
