using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Quermine
{
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
			FieldInfo[] fields = obj.GetType().GetFields();
			foreach (FieldInfo field in fields)
			{
				DbFieldAttribute columnAttribute = field.GetCustomAttribute<DbFieldAttribute>(true);
				InsertIgnoreAttribute insertIgnore = field.GetCustomAttribute<InsertIgnoreAttribute>(true);

				if (columnAttribute != null && insertIgnore == null)
				{
					object value = field.GetValue(obj);

					Value(columnAttribute.Name, value);
				}
			}

			// Get custom properties
			PropertyInfo[] properties = obj.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				DbFieldAttribute columnAttribute = property.GetCustomAttribute<DbFieldAttribute>(true);
				InsertIgnoreAttribute insertIgnore = property.GetCustomAttribute<InsertIgnoreAttribute>(true);

				if (columnAttribute != null && insertIgnore == null)
				{
					object value = property.GetValue(obj);

					Value(columnAttribute.Name, value);
				}
			}
		}

	}
}
