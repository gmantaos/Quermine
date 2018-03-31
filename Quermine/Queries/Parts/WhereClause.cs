﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	public class WhereClause
	{
		string whereClauseString;
		Dictionary<string, object> parameters;

		private WhereClause()
		{
			parameters = new Dictionary<string, object>();
		}

		public WhereClause(string whereClauseString) : this()
		{
			this.whereClauseString = whereClauseString;
		}

		private WhereClause(string whereClauseString, Dictionary<string, object> parameters)
		{
			this.whereClauseString = whereClauseString;
			this.parameters = parameters;
		}

		public WhereClause(string column, WhereRelation relation, object value) : this()
		{
			string pName = Utils.GetParameterName("where_" + column);

			whereClauseString = string.Format("{0} {1} {2}",
				column, GetSymbol(relation), pName
				);

			parameters.Add(pName, value);
		}

		/// <summary>
		/// Shortcut to Equals WhereClause
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		public WhereClause(string column, object value) : this(column, WhereRelation.Equal, value)
		{

		}

		public WhereClause(string column, ColumnCondition condition) : this()
		{
			whereClauseString = string.Format("{0} {1}",
				column, GetSymbol(condition)
				);
		}

		public WhereClause And(WhereClause clause)
		{
			return this & clause;
		}

		public WhereClause Or(WhereClause clause)
		{
			return this | clause;
		}

		public static WhereClause operator &(WhereClause w1, WhereClause w2)
		{
			return MergeClauses(w1, w2, "AND");
		}

		public static WhereClause operator |(WhereClause w1, WhereClause w2)
		{
			return MergeClauses(w1, w2, "OR");
		}

		static WhereClause MergeClauses(WhereClause w1, WhereClause w2, string relation)
		{
			if (w1 == null)
				return w2;
			if (w2 == null)
				return w2;

			string clause = string.Format("{0} {1} {2}", w1.ToString(), relation, w2.ToString());
			Dictionary<string, object> parameters = new Dictionary<string, object>(w1.parameters);

			w2.parameters.ToList().ForEach(x => parameters.Add(x.Key, x.Value));

			return new WhereClause(clause, parameters);
		}

		public override string ToString()
		{
			return string.Format("({0})", whereClauseString);
		}

		internal Dictionary<string, object> Parameters()
		{
			return parameters;
		}

		static string GetSymbol(WhereRelation relation)
		{
			switch (relation)
			{
				case WhereRelation.Equal:
					return "=";

				case WhereRelation.NotEqual:
					return "<>";

				case WhereRelation.LesserThan:
					return "<";

				case WhereRelation.UpTo:
					return "<=";

				case WhereRelation.BiggerThan:
					return ">";

				case WhereRelation.AtLeast:
					return ">=";

				case WhereRelation.Like:
					return "LIKE";

				default:
					throw new ArgumentException("Unknown WHERE operator: " + relation);
			}
		}

		static string GetSymbol(ColumnCondition condition)
		{
			switch (condition)
			{
				case ColumnCondition.IsNull:
					return "IS NULL";

				case ColumnCondition.NotNull:
					return "IS NOT NULL";

				default:
					throw new ArgumentException("Unknown WHERE operator: " + condition);
			}
		}
	}
}
