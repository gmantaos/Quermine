using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// Attribute used for mapping a class to a database table.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class DbTableAttribute : Attribute
	{
		/// <summary>
		/// The name of the table.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// Map this class to a table in the database.
		/// </summary>
		/// <param name="name">The name of the table in the database.</param>
		public DbTableAttribute(string name)
		{
			Name = name;
		}
	}
}
