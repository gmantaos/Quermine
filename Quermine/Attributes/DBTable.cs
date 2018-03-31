using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class DbTable : Attribute
	{
		/// <summary>
		/// The name of the table.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The name of the table.
		/// </summary>
		/// <param name="name"></param>
		public DbTable(string name)
		{
			Name = name;
		}
	}
}
