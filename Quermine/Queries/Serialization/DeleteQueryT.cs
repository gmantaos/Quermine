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
			DbTable tableAttribute = obj.GetType()
				.GetCustomAttributes<DbTable>(true)
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
				DbField columnAttribute = field.GetCustomAttribute<DbField>(true);

				if (columnAttribute != null && columnAttribute.IsWhereCondition)
				{
					object value = field.GetValue(obj);

					Where(columnAttribute.Name, value);
				}
			}

			// Get custom properties
			PropertyInfo[] properties = obj.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				DbField columnAttribute = property.GetCustomAttribute<DbField>(true);

				if (columnAttribute != null && columnAttribute.IsWhereCondition)
				{
					object value = property.GetValue(obj);

					Where(columnAttribute.Name, value);
				}
			}
		}

	}
}
