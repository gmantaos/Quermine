using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quermine
{
	/// <summary>
	/// Holds any query. Can be executed by calling Execute() or ExecuteNonQuery()
	/// on the respective client.
	/// </summary>
    public class Query
    {
		string queryString;
		Dictionary<string, object> parameters;

		internal QueryBuilder builder;

		internal bool lowPriority;
		internal bool ignore;

		/// <summary>
		/// Event that will fire asynchronously for each row that is fetched from a query.
		/// </summary>
		public event EventHandler<ResultRow> OnRow;

		/// <summary>
		/// Returns the full query string that this query object currently holds.
		/// </summary>
		public virtual string QueryString
		{
			get { return queryString; }
		}

		internal Query(QueryBuilder builder)
		{
			this.builder = builder;
			parameters = new Dictionary<string, object>();
		}

		internal Query(QueryBuilder builder, string queryString) : this(builder)
		{
			this.queryString = queryString;
		}

		/// <summary>
		/// Add a named parameter to his query.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Query AddParameter(string name, object value)
		{
			parameters.Add(name, value);
			return this;
		}

		/// <summary>
		/// Add a named parameter to this query in the form of a key-value pair.
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		public Query AddParameter(KeyValuePair<string, object> param)
		{
			parameters.Add(param.Key, param.Value);
			return this;
		}

		/// <summary>
		/// Add multiple parameters to this query.
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public Query AddParameters(Dictionary<string, object> parameters)
		{
			parameters.ToList().ForEach(p => AddParameter(p));
			return this;
		}

		/// <summary>
		/// Enable the LOW PRIORITY flag for this query, if it supported
		/// by the underlying db server.
		/// </summary>
		/// <returns></returns>
		public Query LowPriority()
		{
			lowPriority = true;
			return this;
		}

		/// <summary>
		/// Enable or disable the LOW PRIORITY flag for this query, 
		/// if it supported by the underlying db server.
		/// </summary>
		/// <param name="enabled"></param>
		/// <returns></returns>
		public Query LowPriority(bool enabled)
		{
			lowPriority = enabled;
			return this;
		}

		/// <summary>
		/// Enable the IGNORE flag for this query, if it supported
		/// by the underlying db server.
		/// </summary>
		/// <returns></returns>
		public Query Ignore()
		{
			ignore = true;
			return this;
		}

		/// <summary>
		/// Enable or disable the IGNORE flag for this query, 
		/// if it supported by the underlying db server.
		/// </summary>
		/// <param name="enabled"></param>
		/// <returns></returns>
		public Query Ignore(bool enabled)
		{
			ignore = enabled;
			return this;
		}

		internal Dictionary<string, object> Parameters()
		{
			return parameters;
		}

		internal void Row(ResultRow row)
		{
			OnRow?.Invoke(this, row);
		}

		/// <summary>
		/// This method is for testing purposes only.
		/// Stay far, far away from it.
		/// </summary>
		/// <returns></returns>
		internal string ParametrizedQueryString()
		{
			string str = QueryString;

			foreach (KeyValuePair<string, object> parameter in parameters)
			{
				str = str.Replace(parameter.Key, "'" + parameter.Value + "'");
			}

			return str;
		}
	}
}
