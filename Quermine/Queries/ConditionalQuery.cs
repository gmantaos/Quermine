using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// Represents a query that supports WHERE, ORDER BY and LIMIT.
	/// </summary>
	public abstract class ConditionalQuery : Query
	{
		internal WhereClause whereClause;
		internal OrderByClause orderByClause;
		internal uint limit;

		internal ConditionalQuery(QueryBuilder builder) : base(builder)
		{

		}
		
		/// <summary>
		/// Add conditions to the ORDER BY clause of this query.
		/// </summary>
		/// <param name="columns"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Append a new conition with a logical AND and cast to another query type
		/// </summary>
		/// <typeparam name="Q"></typeparam>
		/// <param name="column"></param>
		/// <param name="relation"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Q Where<Q>(string column, WhereRelation relation, object value) where Q : ConditionalQuery
		{
			return (Q)Where(new WhereClause(column, relation, value));
		}

		/// <summary>
		/// Append a new sub-query conition with a logical AND 
		/// </summary>
		/// <param name="column"></param>
		/// <param name="relation"></param>
		/// <param name="subQuery"></param>
		/// <returns></returns>
		public ConditionalQuery Where(string column, WhereRelation relation, Query subQuery)
		{
			return Where(new WhereClause(column, relation, subQuery));
		}

		/// <summary>
		/// Append a new sub-query conition with a logical AND and cast to another query type
		/// </summary>
		/// <typeparam name="Q"></typeparam>
		/// <param name="column"></param>
		/// <param name="relation"></param>
		/// <param name="subQuery"></param>
		/// <returns></returns>
		public Q Where<Q>(string column, WhereRelation relation, Query subQuery) where Q : ConditionalQuery
		{
			return (Q)Where(new WhereClause(column, relation, subQuery));
		}

		/// <summary>
		/// Append a new conition with a logical AND
		/// </summary>
		/// <param name="whereClauseString"></param>
		/// <returns></returns>
		public ConditionalQuery Where(string whereClauseString)
		{
			return Where(new WhereClause(whereClauseString));
		}

		/// <summary>
		/// Append a new conition with a logical AND and cast to another query type
		/// </summary>
		/// <typeparam name="Q"></typeparam>
		/// <param name="whereClauseString"></param>
		/// <returns></returns>
		public Q Where<Q>(string whereClauseString) where Q : ConditionalQuery
		{
			return (Q)Where(new WhereClause(whereClauseString));
		}

		/// <summary>
		/// Shortcut to where equals
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public ConditionalQuery Where(string column, object value)
		{
			return Where(column, WhereRelation.Equal, value);
		}

		/// <summary>
		/// Shortcut to where equals
		/// </summary>
		/// <typeparam name="Q"></typeparam>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Q Where<Q>(string column, object value) where Q : ConditionalQuery
		{
			return (Q)Where(column, WhereRelation.Equal, value);
		}

		/// <summary>
		/// Append a new conition with a logical AND
		/// </summary>
		/// <param name="column"></param>
		/// <param name="condition"></param>
		/// <returns></returns>
		public ConditionalQuery Where(string column, ValueCondition condition)
		{
			return Where(new WhereClause(column, condition));
		}

		/// <summary>
		/// Append a new conition with a logical AND and cast to another query type
		/// </summary>
		/// <typeparam name="Q"></typeparam>
		/// <param name="column"></param>
		/// <param name="condition"></param>
		/// <returns></returns>
		public Q Where<Q>(string column, ValueCondition condition) where Q : ConditionalQuery
		{
			return (Q)Where(new WhereClause(column, condition));
		}

		/// <summary>
		/// Add a limit to the number of results returned by this query.
		/// <para>For example, this adds LIMIT to a MySql query and TOP to an MsSql query.</para>
		/// </summary>
		/// <param name="limit"></param>
		/// <returns></returns>
		public ConditionalQuery Limit(uint limit) 
		{
			this.limit = limit;
			return this;
		}

		/// <summary>
		/// Add a limit to the number of results returned by this query.
		/// <para>For example, this adds LIMIT to a MySql query and TOP to an MsSql query.</para>
		/// </summary>
		/// <typeparam name="Q"></typeparam>
		/// <param name="limit"></param>
		/// <returns></returns>
		public Q Limit<Q>(uint limit) where Q : ConditionalQuery
		{
			return (Q)Limit(limit);
		}

		internal string ConditionalQueryPart()
		{
			return builder.ConditionalQueryPart(this);
		}
	}
}
