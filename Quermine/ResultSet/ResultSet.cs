using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Quermine
{
    /// <summary>
    /// Holds the schema and data of a query result.
    /// Implements ICollection.
    /// </summary>
    public class ResultSet : IEnumerable<ResultRow>
    {
        Dictionary<string, Type> schema;
        List<ResultRow> rows;
        
		/// <summary>
		/// Get the number of rows returned by the query.
		/// </summary>
        public int RowCount
        {
            get { return rows.Count; }
        }        

		/// <summary>
		/// Get a specific row of this result set by its index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ResultRow this[int index]
		{
			get { return rows[index]; }
		}

        internal ResultSet(DbDataReader reader)
        {
            schema = new Dictionary<string, Type>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Type type = reader.GetFieldType(i);

                schema.Add(
                    reader.GetName(i),
                    reader.GetFieldType(i)
                    );
            }
            
            rows = new List<ResultRow>();
        }

        internal ResultRow AddRow(DbDataReader row)
        {
            ResultRow dbRow = new ResultRow(schema, row);
            rows.Add(dbRow);
            return dbRow;
        }

		internal static async Task<ResultSet> FromReader(DbDataReader reader)
		{
			ResultSet rs = new ResultSet(reader);

			while (await reader.ReadAsync())
			{
				ResultRow row = rs.AddRow(reader);
			}

			return rs;
		}

		/// <summary>
		/// Checks whether the result of the query contains a column matching the specified key.
		/// Returns true if the key is present, even if its value is NULL.
		/// </summary>
		/// <param name="key"></param>
		public bool HasField(string key)
		{
			return schema.ContainsKey(key);
		}

        public IEnumerator<ResultRow> GetEnumerator()
        {
            return ((IEnumerable<ResultRow>)rows).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ResultRow>)rows).GetEnumerator();
        }
    }
}
