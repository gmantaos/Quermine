using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Collections;

namespace Queerie
{
    /// <summary>
    /// Holds the schema and data of a query result.
    /// Implements ICollection.
    /// </summary>
    public class ResultSet : IEnumerable<ResultRow>
    {
        Dictionary<string, Type> schema;
        List<ResultRow> rows;
        
        public int RowCount
        {
            get { return rows.Count; }
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
