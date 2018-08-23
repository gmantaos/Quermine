using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// Holds an UPDATE query.
	/// </summary>
	public class UpdateQuery : ConditionalQuery
	{
		internal Sequence tables;
		internal SetClause setClause;

		internal UpdateQuery(QueryBuilder builder) : base(builder)
		{
		}

		internal UpdateQuery(QueryBuilder builder, params string[] tables) : this(builder)
		{
			this.tables = new Sequence(tables);
		}

		/// <summary>
		/// Add a SET directive to this directive, setting
		/// the given value to the given column.
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public UpdateQuery Set(string column, object value)
		{
			string pName = Utils.GetParameterName("set_" + column);

			setClause += new SetClause(column, pName);

			AddParameter(pName, value);
			return this;
		}

		/// <inheritdoc />
		public override string QueryString
		{
			get
			{
				return builder.UpdateQuery(this);
			}
		}
	}
}
