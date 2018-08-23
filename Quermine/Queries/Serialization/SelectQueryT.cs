using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace Quermine
{
	/// <summary>
	/// Holds a SELECT query that got constructed as the result
	/// of the serialization of the given type's field and property
	/// types that carry the DbField attribute.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SelectQuery<T> : SelectQuery where T : new()
	{
		internal SelectQuery(QueryBuilder builder) : base(builder)
		{
			// Get table name
			DbTableAttribute tableAttribute = typeof(T)
				.GetCustomAttributes<DbTableAttribute>(true)
				.FirstOrDefault();

			if (tableAttribute != null)
			{
				From(tableAttribute.Name);
			}
			else
			{
				From(typeof(T).Name);
			}
			
			foreach (MemberInfo member in typeof(T).GetValueMembers())
			{
				Type memberType = member.GetUnderlyingType();

				DbFieldAttribute columnAttribute = member.GetCustomAttribute<DbFieldAttribute>(true);
				SelectIgnoreAttribute selectIgnore = member.GetCustomAttribute<SelectIgnoreAttribute>(true);

				if (columnAttribute != null && selectIgnore == null)
				{
					Select(columnAttribute.Name);
				}
			}
		}
	}
}
