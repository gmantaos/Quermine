using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// The SELECT query builder.
	/// </summary>
	public class SelectQuery : ConditionalQuery
	{
		internal SelectClause selectClause;
		internal FromClause fromClause;
		internal uint offset;

		internal SelectQuery(QueryBuilder builder) : base(builder)
		{
			selectClause = new SelectClause();
		}

		internal SelectQuery(QueryBuilder builder, params string[] columns) : this(builder)
		{
			selectClause += new SelectClause(columns);
		}

		/// <summary>
		/// Add columns to this SELECT query.
		/// </summary>
		/// <param name="columns"></param>
		/// <returns></returns>
		public SelectQuery Select(params string[] columns)
		{
			selectClause += new SelectClause(columns);
			return this;
		}

		/// <summary>
		/// Add tables to the FROM clause of this SELECT query.
		/// </summary>
		/// <param name="tables"></param>
		/// <returns></returns>
		public SelectQuery From(params string[] tables)
		{
			fromClause += new FromClause(tables);
			return this;
		}

		/// <summary>
		/// Add an OFFSET to this SELECT query.
		/// </summary>
		/// <param name="offset"></param>
		/// <returns></returns>
		public SelectQuery Offset(uint offset)
		{
			this.offset = offset;
			return this;
		}

		public override string QueryString
		{
			get
			{
				return builder.SelectQuery(this);
			}
		}
	}
}
