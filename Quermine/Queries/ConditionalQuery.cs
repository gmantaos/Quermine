using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	public abstract class ConditionalQuery : Query
	{
		internal WhereClause whereClause;
		internal OrderByClause orderByClause;
		internal int limit;

		internal ConditionalQuery(QueryBuilder builder) : base(builder)
		{

		}
		
		public ConditionalQuery OrderBy(params string[] columns)
		{
			orderByClause += new OrderByClause(columns);
			return this;
		}

		/// <summary>
		/// Append a new conition with a logical AND
		/// <para>To append with OR, use new WhereClause(...)</para>
		/// </summary>
		/// <param name="clause"></param>
		public ConditionalQuery Where(WhereClause clause)
		{
			AddParameters(clause.Parameters());
			whereClause &= clause;
			return this;
		}

		/// <summary>
		/// Append a new conition with a logical AND
		/// <para>To append with OR, use new WhereClause(...)</para>
		/// </summary>
		/// <param name="column"></param>
		/// <param name="relation"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public ConditionalQuery Where(string column, WhereRelation relation, object value)
		{
			return Where(new WhereClause(column, relation, value));
		}

		public Q Where<Q>(string column, WhereRelation relation, object value) where Q : ConditionalQuery
		{
			return (Q)Where(new WhereClause(column, relation, value));
		}

		public ConditionalQuery Where(string whereClauseString)
		{
			return Where(new WhereClause(whereClauseString));
		}

		public Q Where<Q>(string whereClauseString) where Q : ConditionalQuery
		{
			return (Q)Where(new WhereClause(whereClauseString));
		}

		/// <summary>
		/// Shortcut to Equals Where
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public ConditionalQuery Where(string column, object value)
		{
			return Where(column, WhereRelation.Equal, value);
		}

		public Q Where<Q>(string column, object value) where Q : ConditionalQuery
		{
			return (Q)Where(column, WhereRelation.Equal, value);
		}

		public ConditionalQuery Where(string column, ColumnCondition condition)
		{
			return Where(new WhereClause(column, condition));
		}

		public Q Where<Q>(string column, ColumnCondition condition) where Q : ConditionalQuery
		{
			return (Q)Where(new WhereClause(column, condition));
		}

		public ConditionalQuery Limit(int limit) 
		{
			this.limit = limit;
			return this;
		}

		public Q Limit<Q>(int limit) where Q : ConditionalQuery
		{
			return (Q)Limit(limit);
		}

		internal string ConditionalQueryPart()
		{
			return builder.ConditionalQueryPart(this);
		}
	}
}
