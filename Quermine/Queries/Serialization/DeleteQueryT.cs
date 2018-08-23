using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Quermine
{
	/// <summary>
	/// Holds a DELETE query that got constructed as the result
	/// of the serialization of the given type's field and property
	/// types that carry the DbField attribute.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DeleteQuery<T> : DeleteQuery
	{

		internal DeleteQuery(QueryBuilder builder, T obj) : base(builder)
		{
			// Get table name
			DbTableAttribute tableAttribute = obj.GetType()
				.GetCustomAttributes<DbTableAttribute>(true)
				.FirstOrDefault();

			if (tableAttribute != null)
			{
				From(tableAttribute.Name);
			}
			else
			{
				From(obj.GetType().Name);
			}

			List<MemberInfo> members = obj.GetType().GetValueMembers();
			foreach (MemberInfo member in members)
			{
				Type memberType = member.GetUnderlyingType();

				DbFieldAttribute columnAttribute = member.GetCustomAttribute<DbFieldAttribute>(true);
				WhereIgnoreAttribute whereIgnore = member.GetCustomAttribute<WhereIgnoreAttribute>(true);

				if (columnAttribute != null && whereIgnore == null)
				{
					object value = member.GetValue(obj);

					if (columnAttribute.ValidFormatter(memberType))
					{
						value = columnAttribute.FormatGetValue(memberType, value);
					}

					Where(columnAttribute.Name, value);
				}
			}
		}

	}
}
