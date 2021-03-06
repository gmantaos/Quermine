﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Quermine
{
	/// <summary>
	/// Holds an INSERT query that got constructed as the result
	/// of the serialization of the given type's field and property
	/// types that carry the DbField attribute.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class InsertQuery<T> : InsertQuery
	{

		internal InsertQuery(QueryBuilder builder, T obj) : base(builder)
		{
			// Get table name
			DbTableAttribute tableAttribute = obj.GetType()
				.GetCustomAttributes<DbTableAttribute>(true)
				.FirstOrDefault();
			
			if (tableAttribute != null)
			{
				table = tableAttribute.Name;
			}
			else
			{
				table = obj.GetType().Name;
			}

			// Get custom fields
			List<MemberInfo> members = obj.GetType().GetValueMembers();
			foreach (MemberInfo member in members)
			{
				Type memberType = member.GetUnderlyingType();

				DbFieldAttribute columnAttribute = member.GetCustomAttribute<DbFieldAttribute>(true);
				InsertIgnoreAttribute insertIgnore = member.GetCustomAttribute<InsertIgnoreAttribute>(true);

				if (columnAttribute != null && insertIgnore == null)
				{
					object value = member.GetValue(obj);

					if (columnAttribute.ValidFormatter(memberType))
					{
						value = columnAttribute.FormatGetValue(memberType, value);
					}

					Value(columnAttribute.Name, value);
				}
			}
		}

	}
}
