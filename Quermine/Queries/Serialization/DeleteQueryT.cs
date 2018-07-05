using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Quermine
{
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

			// Get custom fields
			FieldInfo[] fields = obj.GetType().GetFields();
			foreach (FieldInfo field in fields)
			{
				DbFieldAttribute columnAttribute = field.GetCustomAttribute<DbFieldAttribute>(true);
				WhereIgnoreAttribute whereIgnore = field.GetCustomAttribute<WhereIgnoreAttribute>(true);

				if (columnAttribute != null && whereIgnore == null)
				{
					object value = field.GetValue(obj);

					Where(columnAttribute.Name, value);
				}
			}

			// Get custom properties
			PropertyInfo[] properties = obj.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				DbFieldAttribute columnAttribute = property.GetCustomAttribute<DbFieldAttribute>(true);
				WhereIgnoreAttribute whereIgnore = property.GetCustomAttribute<WhereIgnoreAttribute>(true);

				if (columnAttribute != null && whereIgnore == null)
				{
					object value = property.GetValue(obj);

					Where(columnAttribute.Name, value);
				}
			}
		}

	}
}
