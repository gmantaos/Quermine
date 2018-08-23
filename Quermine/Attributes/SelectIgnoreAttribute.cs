using System;
using System.Collections.Generic;
using System.Text;

namespace Quermine
{
	/// <summary>
	/// Indicate that this field or property should not be included in the 
	/// construction of SELECT queries or in the object's deserialization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class SelectIgnoreAttribute : Attribute
    {
    }
}
