using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Quermine
{
	public class UpdateQuery<T> : UpdateQuery where T : new()
	{
		internal UpdateQuery(QueryBuilder builder) : base(builder)
		{
			// Get table name
			DbTable tableAttribute = typeof(T)
				.GetCustomAttributes<DbTable>(true)
				.FirstOrDefault();

			if (tableAttribute != null)
			{
				tables += new Sequence(tableAttribute.Name);
			}
			else
			{
				tables += new Sequence(typeof(T).Name);
			}
		}

		public UpdateQuery<T> Where(T obj)
		{
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

			return this;
		}

		public UpdateQuery<T> Set(T obj)
		{
			// Get custom fields
			FieldInfo[] fields = obj.GetType().GetFields();
			foreach (FieldInfo field in fields)
			{
				DbField columnAttribute = field.GetCustomAttribute<DbField>(true);

				if (columnAttribute != null)
				{
					object value = field.GetValue(obj);

					Set(columnAttribute.Name, value);
				}
			}

			// Get custom properties
			PropertyInfo[] properties = obj.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				DbField columnAttribute = property.GetCustomAttribute<DbField>(true);

				if (columnAttribute != null)
				{
					object value = property.GetValue(obj);

					Set(columnAttribute.Name, value);
				}
			}

			return this;
		}
	}
}
