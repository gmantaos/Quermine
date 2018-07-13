using System;
using System.Collections.Generic;
using System.Text;

namespace Quermine
{
	/// <summary>
	/// Implementing this interface allows for the creation of custom formatters,
	/// which can provide custom type conversions during serialization.
	/// </summary>
	/// <typeparam name="T"></typeparam>
    interface IValueFormatter<T>
    {
		/// <summary>
		/// Formatting provider to be called when reading a serializable field so that its value can paticipate in a query.
		/// </summary>
		/// <param name="val">The value of the field.</param>
		/// <returns>The value to place in the query.</returns>
		object GetValue(T val);

		/// <summary>
		/// Formatting provider to be called when setting the value of a serializable field with a value
		/// retrieved from the database.
		/// </summary>
		/// <param name="val">The value from the database.</param>
		/// <returns>The final value to set on the field.</returns>
		T SetValue(object val);
    }
}
