using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Collections;

namespace Queerie
{
    public class ResultRow : IEnumerable<KeyValuePair<string, object>>
    {
        Dictionary<string, object> fields;

        /// <summary>
        /// Get the underlying object by key without any cast.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            get
            {
				if (fields.ContainsKey(key))
				{
					return fields[key].GetType() == typeof(DBNull) ? null : fields[key];
				}
				else
				{
					throw new KeyNotFoundException(key);
				}
            }
        }

		public List<string> keys
		{
			get { return fields.Keys.ToList(); }
		}

        internal ResultRow(Dictionary<string, Type> schema, DbDataReader row)
        {
			fields = new Dictionary<string, object>(schema.Count);

            foreach (KeyValuePair<string, Type> field in schema)
			{
				fields.Add(
					field.Key,
					row[field.Key]
				);
			}
        }

        public string GetString(string key)
        {
            return this[key]?.ToString();
        }
        public string GetString(string key, string defaultValue)
        {
            return GetString(key) ?? defaultValue;
        }

        public double GetDouble(string key)
        {
            if (this[key] == null)
                throw new KeyNotFoundException(key);
            else
                return Convert.ToDouble(this[key]);
        }
        public double GetDouble(string key, double defaultValue)
        {
            try
            {
                defaultValue = GetDouble(key);
            } catch(Exception) { }
            return defaultValue;
        }

        public int GetInteger(string key)
        {
            if (this[key] == null)
                throw new KeyNotFoundException(key);
            else
                return Convert.ToInt32(this[key]);
        }
        public int GetInteger(string key, int defaultValue)
        {
            try
            {
                defaultValue = GetInteger(key);
            }
            catch (Exception) { }
            return defaultValue;
        }

        public bool GetBoolean(string key)
        {
            if (this[key] == null)
                throw new KeyNotFoundException(key);

            if (this[key].GetType() == typeof(int)
                || this[key].GetType() == typeof(short)
                || this[key].GetType() == typeof(byte)
                || this[key].GetType() == typeof(sbyte)
                || this[key].GetType() == typeof(long))
            {
                int val;
                try
                {
                    if (this[key].GetType() == typeof(sbyte))
                        val = Convert.ToInt16(this[key]);
                    else
                        val = (int)this[key];
                }
                catch (InvalidCastException)
                {
                    throw new InvalidCastException("Cannot cast to int: " + this[key].GetType());
                }

                if (val < 0)
                    throw new ArgumentOutOfRangeException("Cannot convert to bool: " + val);

                return val != 0;
            }

            if (this[key].GetType() == typeof(uint)
                || this[key].GetType() == typeof(ushort)
                || this[key].GetType() == typeof(ulong))
            {
                return (uint)this[key] != 0;
            }

            throw new ArgumentException("Field is not a boolean type: " + key);
        }

        public bool GetBoolean(string key, bool defaultValue)
        {
            try
            {
                defaultValue = GetBoolean(key);
            } catch (Exception) { }
            return defaultValue;
        }

		public Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object>(fields);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<string, object>>)fields).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<string, object>>)fields).GetEnumerator();
		}
	}
}
