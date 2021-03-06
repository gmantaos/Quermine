﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Quermine
{
	/// <summary>
	/// Indicate that this field or property should not be included in the 
	/// construction of WHERE conditions.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class WhereIgnoreAttribute : Attribute
    {
    }
}
