using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// Represents a column in a database table.
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
		public Type Formatter;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">The name of the field.</param>
		public DbFieldAttribute(string name = null)
		{
			Name = name;
		}

		internal bool ValidFormatter(Type targetMemberType)
		{
			if (Formatter == null)
				return false;

			IEnumerable<Type> intf = from interfaceType in Formatter.GetInterfaces()
									 where interfaceType.IsGenericType
									 let baseInterface = interfaceType.GetGenericTypeDefinition()
									 where baseInterface == typeof(IValueFormatter<>)
									 select interfaceType;

			if (intf.Count() == 0)
			{
				throw new ArgumentException("Type assigned to DbField.Formatter does not implement IValueFormatter: " + Formatter);
			}
			else if (Formatter.IsGenericType)
			{
				throw new ArgumentException("Type assigned to DbField.Formatter must not be generic: " + Formatter);
			}
			else if (Formatter.GetConstructors().Length > 0				// Has a constructor
				&& Formatter.GetConstructor(Type.EmptyTypes) == null    // ... with parameters
				&& !Formatter.IsValueType)								// and it's not a struct
			{
				throw new ArgumentException("Type assigned to DbField.Formatter must provide a public parameterless constructor: " + Formatter);
			}
			else
			{
				return true;
			}
		}

		internal object FormatSetValue(Type targetMemberType, object value)
		{
			object formatter = Activator.CreateInstance(Formatter);

			MethodInfo setMethod = Formatter.GetMethod("SetValue");

			return setMethod.Invoke(formatter, new object[] { value });
		}

		internal object FormatGetValue(Type targetMemberType, object value)
		{
			object formatter = Activator.CreateInstance(Formatter);

			MethodInfo getMethod = Formatter.GetMethod("GetValue");

			return getMethod.Invoke(formatter, new object[] { value });
		}
	}
}
