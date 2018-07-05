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
			List<MemberInfo> members = obj.GetType().GetValueMembers();
			foreach (MemberInfo member in members)
			{
				DbFieldAttribute columnAttribute = member.GetCustomAttribute<DbFieldAttribute>(true);
				WhereIgnoreAttribute whereIgnore = member.GetCustomAttribute<WhereIgnoreAttribute>(true);

				if (columnAttribute != null && whereIgnore == null)
				{
					object value = member.GetValue(obj);

					Where(columnAttribute.Name, value);
				}
			}

			return this;
		}

		public UpdateQuery<T> Set(T obj)
		{
			List<MemberInfo> members = obj.GetType().GetValueMembers();
			foreach (MemberInfo member in members)
			{
				DbFieldAttribute columnAttribute = member.GetCustomAttribute<DbFieldAttribute>(true);
				UpdateIgnoreAttribute updateIgnore = member.GetCustomAttribute<UpdateIgnoreAttribute>(true);

				if (columnAttribute != null && updateIgnore == null)
				{
					object value = member.GetValue(obj);

					Set(columnAttribute.Name, value);
				}
			}

			return this;
		}
	}
}
