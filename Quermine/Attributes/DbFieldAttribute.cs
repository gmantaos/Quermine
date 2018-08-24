using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// Attribute used to map a field or property to a column in a database table.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class DbFieldAttribute : Attribute
	{
		/// <summary>
		/// The name of the field.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The custom formatter to use when setting and getting values from this field.
		/// </summary>
		public Type FormatWith;

		/// <summary>
		/// The length of the field on the database column. 
		/// Used when generating a CREATE TABLE query.
		/// <para>A negative value indicates default.</para>
		/// </summary>
		public int Length = -1;

		/// <summary>
		/// The precision of the field on the database column. 
		/// Used when generating a CREATE TABLE query.
		/// <para>A negative value indicates default.</para>
		/// </summary>
		public int Precision = -1;

		/// <summary>
		/// Additional field properties that define this column 
		/// in the database.
		/// Used when generating a CREATE TABLE query.
		/// </summary>
		public FieldProperties FieldProprties = 0;

		/// <summary>
		/// The default value of this column in the database.
		/// Used when generating a CREATE TABLE query.
		/// </summary>
		public object DefaultValue = null;

		/// <summary>
		/// Map this field or property to a column in the database table.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		public DbFieldAttribute(string name = null)
		{
			Name = name;
		}

		internal bool ValidFormatter(Type targetMemberType)
		{
			if (FormatWith == null)
				return false;

			IEnumerable<Type> intf = from interfaceType in FormatWith.GetInterfaces()
									 where interfaceType.IsGenericType
									 let baseInterface = interfaceType.GetGenericTypeDefinition()
									 where baseInterface == typeof(IValueFormatter<>)
									 select interfaceType;

			if (intf.Count() == 0)
			{
				throw new ArgumentException("Type assigned to DbField.Formatter does not implement IValueFormatter: " + FormatWith);
			}
			else if (FormatWith.IsGenericType)
			{
				throw new ArgumentException("Type assigned to DbField.Formatter must not be generic: " + FormatWith);
			}
			else if (FormatWith.GetConstructors().Length > 0				// Has a constructor
				&& FormatWith.GetConstructor(Type.EmptyTypes) == null    // ... with parameters
				&& !FormatWith.IsValueType)								// and it's not a struct
			{
				throw new ArgumentException("Type assigned to DbField.Formatter must provide a public parameterless constructor: " + FormatWith);
			}
			else
			{
				return true;
			}
		}

		internal object FormatSetValue(Type targetMemberType, object value)
		{
			object formatter = Activator.CreateInstance(FormatWith);

			MethodInfo setMethod = FormatWith.GetMethod("SetValue");

			return setMethod.Invoke(formatter, new object[] { value });
		}

		internal object FormatGetValue(Type targetMemberType, object value)
		{
			object formatter = Activator.CreateInstance(FormatWith);

			MethodInfo getMethod = FormatWith.GetMethod("GetValue");

			return getMethod.Invoke(formatter, new object[] { value });
		}
	}
}
