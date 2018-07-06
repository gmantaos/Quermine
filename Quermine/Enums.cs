using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// A comparison between two values.
	/// </summary>
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
		Like,

		/// <summary>
		/// EXISTS
		/// </summary>
		Exists,

		/// <summary>
		/// NOT EXISTS
		/// </summary>
		NotExists,

		/// <summary>
		/// IN
		/// </summary>
		In,

		/// <summary>
		/// NOT IN
		/// </summary>
		NotIn
	}

	/// <summary>
	/// A condition of a column's value.
	/// </summary>
	public enum ValueCondition
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

	/// <summary>
	/// Specifying whether a table column participates in the table's key.
	/// </summary>
	public enum KeyType
	{
		/// <summary>
		/// Normal field
		/// </summary>
		None,

		/// <summary>
		/// PRI
		/// </summary>
		Primary
	}

	/// <summary>
	/// Additional properties to assign to a field. Not all of them are available on every supported DBMS.
	/// </summary>
	[Flags]
	public enum FieldTypes
	{
		/// <summary>
		/// No additional properties.
		/// </summary>
		None			= 0,

		/// <summary>
		/// Specifying an usigned numeric field.
		/// </summary>
		Unsigned		= 1 << 0,

		/// <summary>
		/// Visual alignment of numeric values by filling with zeroes.
		/// </summary>
		Zerofill		= 1 << 1,

		/// <summary>
		/// Null values are not allowed in this field.
		/// </summary>
		NotNull			= 1 << 2,

		/// <summary>
		/// This field should participate in the table's primary key.
		/// </summary>
		PrimaryKey		= 1 << 3,

		/// <summary>
		/// Enable auto-incrementing assignment to this field. Be aware that some DBMS require the field to also be a primery key.
		/// </summary>
		AutoIncrement	= 1 << 4
	}
}
