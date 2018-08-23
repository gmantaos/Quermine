using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// Holds a DELETE query.
	/// </summary>
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

		/// <summary>
		/// Add table(s) to the FROM clause of this DELETE query.
		/// </summary>
		/// <param name="tables"></param>
		/// <returns></returns>
		public DeleteQuery From(params string[] tables)
		{
			fromClause += new FromClause(tables);
			return this;
		}

		/// <inheritdoc />
		public override string QueryString
		{
			get
			{
				return builder.DeleteQuery(this);
			}
		}
	}
}
