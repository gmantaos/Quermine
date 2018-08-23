using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// Holds an INSERT query.
	/// </summary>
	public class InsertQuery : Query
	{
		internal string table;
		internal Sequence columns;
		internal Sequence values;
		internal bool replace = false;

		internal InsertQuery(QueryBuilder builder) : base(builder)
		{

		}

		internal InsertQuery(QueryBuilder builder, string table) : this(builder)
		{
			this.table = table;
		}

		/// <summary>
		/// Add a value meant for the given column to this INSERT query.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public InsertQuery Value(string column, object value)
		{
			string pName = Utils.GetParameterName("set_" + column);

			columns += new Sequence(column);
			values += new Sequence(pName);
			AddParameter(pName, value);

			return this;
		}

		/// <summary>
		/// Change this query into a REPLACE query.
		/// </summary>
		/// <returns></returns>
		public InsertQuery Replace()
		{
			replace = true;
			return this;
		}

		/// <summary>
		/// Enable or disable the REPLACE directive fr this query.
		/// </summary>
		/// <param name="enabled"></param>
		/// <returns></returns>
		public InsertQuery Replace(bool enabled)
		{
			replace = enabled;
			return this;
		}

		/// <inheritdoc />
		public override string QueryString
		{
			get
			{
				return builder.InsertQuery(this);
			}
		}
	}
}
