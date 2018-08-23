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

		/// <summary>
		/// Add a field to this CREATE TABLE query.
		/// </summary>
		/// <typeparam name="T">The type of the field.</typeparam>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="length">The length of the field.</param>
		/// <param name="fieldProperties">Additional properties that define the field.</param>
		/// <param name="defaultVal">The field's default value.</param>
		/// <returns></returns>
		public CreateTableQuery Field<T>(string fieldName, int? length = null, FieldProperties fieldProperties = 0, object defaultVal = null)
		{
			fields.Add(new TableField(fieldName, typeof(T), length, fieldProperties, defaultVal));
			return this;
		}

		/// <summary>
		/// Add a field to this CREATE TABLE query.
		/// </summary>
		/// <param name="fieldName">The name of the field.</param>
		/// <param name="type">The type of the field.</param>
		/// <param name="length">The length of the field.</param>
		/// <param name="fieldProperties">Additional properties that define the field.</param>
		/// <param name="defaultVal">The field's default value.</param>
		public CreateTableQuery Field(string fieldName, Type type, int? length = null, FieldProperties fieldProperties = 0, object defaultVal = null)
		{
			fields.Add(new TableField(fieldName, type, length, fieldProperties, defaultVal));
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
