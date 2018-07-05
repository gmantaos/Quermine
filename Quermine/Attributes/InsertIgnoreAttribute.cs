using System;
using System.Collections.Generic;
using System.Text;

namespace Quermine
{
	/// <summary>
	/// Inidcate that this field or property should not be included in 
	/// constructing INSERT values. Useful for fields of Auto-Increment
	/// SQL columns.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class InsertIgnoreAttribute : Attribute
    {
    }
}
