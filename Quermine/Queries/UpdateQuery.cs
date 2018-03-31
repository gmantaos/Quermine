using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
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

		public UpdateQuery Set(string column, object value)
		{
			string pName = Utils.GetParameterName("set_" + column);

			setClause += new SetClause(column, pName);

			AddParameter(pName, value);
			return this;
		}

		public override string QueryString
		{
			get
			{
				return builder.UpdateQuery(this);
			}
		}
	}
}
