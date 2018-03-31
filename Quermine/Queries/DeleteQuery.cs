using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	public class DeleteQuery : ConditionalQuery
	{
		internal FromClause fromClause;

		internal DeleteQuery(QueryBuilder builder) : base(builder)
		{

		}

		internal DeleteQuery(QueryBuilder builder, string table) : this(builder)
		{
			fromClause = new FromClause(table);
		}

		public DeleteQuery From(params string[] tables)
		{
			fromClause += new FromClause(tables);
			return this;
		}

		public override string QueryString
		{
			get
			{
				return builder.DeleteQuery(this);
			}
		}
	}
}
