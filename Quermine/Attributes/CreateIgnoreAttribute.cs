using System;
using System.Collections.Generic;
using System.Text;

namespace Quermine
{
	/// <summary>
	/// Inidcate that this field or property should not be included in 
	/// constructing CREATE TABLE queries.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class CreateIgnoreAttribute : Attribute
    {
    }
}
