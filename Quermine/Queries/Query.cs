using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quermine
{
    public class Query
    {
		string queryString;
		Dictionary<string, object> parameters;

		internal QueryBuilder builder;

		internal bool lowPriority;
		internal bool ignore;

		public event EventHandler<ResultRow> OnRow;

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

		public Query AddParameter(string name, object value)
		{
			parameters.Add(name, value);
			return this;
		}

		public Query AddParameter(KeyValuePair<string, object> param)
		{
			parameters.Add(param.Key, param.Value);
			return this;
		}

		public Query AddParameters(Dictionary<string, object> parameters)
		{
			parameters.ToList().ForEach(p => AddParameter(p));
			return this;
		}

		public Query LowPriority(bool enabled)
		{
			lowPriority = enabled;
			return this;
		}

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
	}
}
