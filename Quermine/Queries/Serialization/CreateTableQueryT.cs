using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Quermine
{
	/// <summary>
	/// Holds a CREATE TABLE query that got constructed as the result
	/// of the serialization of the given type's field and property
	/// types that carry the DbField attribute.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CreateTableQuery<T> : CreateTableQuery where T : new()
	{
		internal CreateTableQuery(QueryBuilder builder) : base(builder, Utils.GetTableName<T>())
		{
			foreach (MemberInfo member in typeof(T).GetValueMembers())
			{
				Type memberType = member.GetUnderlyingType();

				DbFieldAttribute columnAttribute = member.GetCustomAttribute<DbFieldAttribute>(true);
				CreateIgnoreAttribute createIgnore = member.GetCustomAttribute<CreateIgnoreAttribute>(true);

				if (columnAttribute != null && createIgnore == null)
				{
					Field(columnAttribute.Name, memberType, columnAttribute.Length, columnAttribute.FieldProprties, columnAttribute.DefaultValue);
				}
			}
		}
	}
}
