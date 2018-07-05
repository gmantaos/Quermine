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
			DbTableAttribute tableAttribute = typeof(T)
				.GetCustomAttributes<DbTableAttribute>(true)
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

			return this;
		}

		public UpdateQuery<T> Set(T obj)
		{
			// Get custom fields
			FieldInfo[] fields = obj.GetType().GetFields();
			foreach (FieldInfo field in fields)
			{
				DbFieldAttribute columnAttribute = field.GetCustomAttribute<DbFieldAttribute>(true);
				UpdateIgnoreAttribute updateIgnore = field.GetCustomAttribute<UpdateIgnoreAttribute>(true);

				if (columnAttribute != null && updateIgnore == null)
				{
					object value = field.GetValue(obj);

					Set(columnAttribute.Name, value);
				}
			}

			// Get custom properties
			PropertyInfo[] properties = obj.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
			{
				DbFieldAttribute columnAttribute = property.GetCustomAttribute<DbFieldAttribute>(true);
				UpdateIgnoreAttribute updateIgnore = property.GetCustomAttribute<UpdateIgnoreAttribute>(true);

				if (columnAttribute != null && updateIgnore == null)
				{
					object value = property.GetValue(obj);

					Set(columnAttribute.Name, value);
				}
			}

			return this;
		}
	}
}
