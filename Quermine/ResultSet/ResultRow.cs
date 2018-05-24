using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Collections;

namespace Quermine
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

		/// <summary>
		/// Get a list of the column keys in the current ResultSet.
		/// </summary>
		public List<string> Fields
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

		/// <summary>
		/// Checks whether the result of the query contains a column matching the specified key.
		/// Returns true if the key is present, even if its value is NULL.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool HasField(string key)
		{
			return fields.ContainsKey(key);
		}

		/// <summary>
		/// Get the value of a column converted to a string.
		/// <para>Throws a KeyNotFoundException if they key is not found in the result.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetString(string key)
		{
			if (this[key] == null)
				throw new KeyNotFoundException(key);
			else
				return this[key]?.ToString();
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
        public string GetString(string key, string defaultValue)
        {
            return GetString(key) ?? defaultValue;
        }
		
		/// <summary>
		/// Get the value of a column converted to a double.
		/// <para>Throws a KeyNotFoundException if they key is not found in the result.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public double GetDouble(string key)
        {
            return Convert.ToDouble(this[key]);
        }


		/// <summary>
		/// Get the value of a column converted to a double, returning a default value instead if the original value is NULL.
		/// <para>Throws a KeyNotFoundException if they key is not found in the result.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public double GetDouble(string key, double defaultValue)
        {
			if (this[key] == null)
				return defaultValue;

			return GetDouble(key);
		}

		/// <summary>
		/// Get the value of a column converted to an integer.
		/// <para>Throws a KeyNotFoundException if they key is not found in the result.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public int GetInteger(string key)
        {
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


		/// <summary>
		/// Get the value of a column converted to a boolean.
		/// <para>A numeric database value can be treated as a boolean, with a zero value to be considered false and a positive value to be considered true.</para>
		/// <para>Throws an ArgumentOutOfRangeException if the value is negative.</para>
		/// <para>Throws a KeyNotFoundException if they key is not found in the result.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Get the value of a column converted to a DateTime.
		/// <para>Throws a KeyNotFoundException if they key is not found in the result.</para>
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public DateTime GetDateTime(string key)
		{
			return Convert.ToDateTime(this[key]);
		}

		public DateTime GetDateTime(string key, DateTime defaultValue)
		{
			try
			{
				defaultValue = GetDateTime(key);
			}
			catch (Exception) { }
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
