using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Quermine
{
    internal static class Extensions
    {
		const BindingFlags BINDING_FLAGS = BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic
		                                 | BindingFlags.Instance | BindingFlags.Static;

		public static List<MemberInfo> GetValueMembers(this Type type)
		{
			List<MemberInfo> members = type.GetFields(BINDING_FLAGS).Cast<MemberInfo>().ToList();
			IEnumerable<PropertyInfo> properties = type.GetProperties(BINDING_FLAGS).Where(p => p.CanRead);

			members.AddRange(properties);

			return members;
		}

		public static List<MemberInfo> SetValueMembers(this Type type)
		{
			List<MemberInfo> members = type.GetFields(BINDING_FLAGS).Cast<MemberInfo>().ToList();
			IEnumerable<PropertyInfo> properties = type.GetProperties(BINDING_FLAGS).Where(p => p.CanWrite);

			members.AddRange(properties);

			return members;
		}

		public static Type GetUnderlyingType(this MemberInfo member)
		{
			return (member is PropertyInfo) ? (member as PropertyInfo).PropertyType : (member as FieldInfo).FieldType;
		}

		public static object GetValue(this MemberInfo member, object obj)
		{
			if (member is FieldInfo)
			{
				return (member as FieldInfo).GetValue(obj);
			}
			else if (member is PropertyInfo)
			{
				return (member as PropertyInfo).GetValue(obj);
			}
			else
			{
				throw new InvalidCastException("MemberInfo.SetValue: member is neither a field or a property");
			}
		}
		public static void SetValue(this MemberInfo member, object obj, object value)
		{
			if (member is FieldInfo)
			{
				(member as FieldInfo).SetValue(obj, value);
			}
			else if (member is PropertyInfo)
			{
				(member as PropertyInfo).SetValue(obj, value);
			}
			else
			{
				throw new InvalidCastException("MemberInfo.SetValue: member is neither a field or a property");
			}
		}
	}
}
