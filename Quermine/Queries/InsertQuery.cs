using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
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

		public InsertQuery Value(string column, object value)
		{
			string pName = Utils.GetParameterName("set_" + column);

			columns += new Sequence("`" + column + "`");
			values += new Sequence(pName);
			AddParameter(pName, value);

			return this;
		}

		public InsertQuery Replace()
		{
			replace = true;
			return this;
		}

		public InsertQuery Replace(bool enabled)
		{
			replace = enabled;
			return this;
		}

		public override string QueryString
		{
			get
			{
				return builder.InsertQuery(this);
			}
		}
	}
}
