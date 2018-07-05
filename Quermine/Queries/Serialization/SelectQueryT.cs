using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;

namespace Quermine
{
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
			
		}
	}
}
