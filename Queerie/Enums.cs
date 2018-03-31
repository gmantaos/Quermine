﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queerie
{
	public enum WhereRelation
	{
		/// <summary>
		/// =
		/// </summary>
		Equal,

		/// <summary>
		/// &lt;&gt;
		/// </summary>
		NotEqual,

		/// <summary>
		/// &lt;
		/// </summary>
		LesserThan,

		/// <summary>
		/// &lt;=
		/// </summary>
		UpTo,

		/// <summary>
		/// &gt;
		/// </summary>
		BiggerThan,

		/// <summary>
		/// &gt;=
		/// </summary>
		AtLeast,

		/// <summary>
		/// LIKE
		/// </summary>
		Like
	}

	public enum ColumnCondition
	{
		/// <summary>
		/// IS NULL
		/// </summary>
		IsNull,

		/// <summary>
		/// IS NOT NULL
		/// </summary>
		NotNull
	}

	public enum KeyType
	{
		None,

		/// <summary>
		/// PRI
		/// </summary>
		Primary
	}

	[Flags]
	public enum FieldTypes
	{
		None			= 0,
		Unsigned		= 1 << 0,
		Zerofill		= 1 << 1,
		NotNull			= 1 << 2,
		PrimaryKey		= 1 << 3,
		AutoIncrement	= 1 << 4
	}
}
