using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quermine
{
	/// <summary>
	/// Represents a CREATE TABLE query.
	/// </summary>
	public class CreateTableQuery : Query
	{
		internal string tableName;
		internal List<TableField> fields;

		internal CreateTableQuery(QueryBuilder builder, string tableName) : base(builder)
		{
			this.tableName = tableName;
			fields = new List<TableField>();
		}

		public CreateTableQuery Field<T>(string fieldName, int? length = null, FieldTypes fieldTypes = 0, object defaultVal = null)
		{
			fields.Add(new TableField(fieldName, typeof(T), length, fieldTypes, defaultVal));
			return this;
		}

		public override string QueryString
		{
			get
			{
				return builder.CreateTableQuery(this);
			}
		}
	}
}
