using System;
using System.Text;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Quermine
{
	/// <summary>
	/// The primary connection object.
	/// </summary>
    public abstract class DbClient : IDisposable
	{
		#region Internal

		internal abstract Task OpenAsync();

		internal abstract QueryBuilder Builder { get; }

		#endregion

		#region Abstract

		/// <summary>
		/// The current state of the underlying connection of this client.
		/// </summary>
		public abstract ConnectionState State { get; }

		/// <summary>
		/// Close the connection and dispose of the connection object.
		/// </summary>
		public abstract void Dispose();

		/// <summary>
		/// Execute a query asynchronously and get the result.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public abstract Task<ResultSet> Execute(Query query);

		/// <summary>
		/// Execute a non-query asynchronously.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public abstract Task<NonQueryResult> ExecuteNonQuery(Query query);

		/// <summary>
		/// Execute a scalar query asynchronously and get the returned value.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public abstract Task<object> ExecuteScalar(Query query);

		/// <summary>
		/// Execute a transaction asynchronously, one query at a time, in the order that they are passed
		/// to this method. If any query raises an exception, the transaction will be rolled back as you would
		/// manually do and the exception will be thrown again.
		/// </summary>
		/// <param name="isolationLevel"></param>
		/// <param name="queries"></param>
		/// <returns></returns>
		public abstract Task<List<NonQueryResult>> ExecuteTransaction(IsolationLevel isolationLevel, params Query[] queries);

		/// <summary>
		/// Get the schema of a table asynchonously.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public abstract Task<TableSchema> GetTableSchema(string table);

		/// <summary>
		/// Asynchronously get a list of the names of all tables in 
		/// the currently selected database.
		/// </summary>
		/// <returns></returns>
		public abstract Task<List<string>> GetTableNames();

		#endregion

		#region Wrappers

		/// <summary>
		/// Execute a query asynchronously and get the result.
		/// </summary>
		/// <param name="commandString"></param>
		/// <returns></returns>
		public async Task<ResultSet> Execute(string commandString)
		{
			Query query = new Query(Builder, commandString);
			return await Execute(query);
		}

		/// <summary>
		/// Execute a non-query asynchronously.
		/// </summary>
		/// <param name="commandString"></param>
		/// <returns></returns>
		public async Task<NonQueryResult> ExecuteNonQuery(string commandString)
		{
			Query query = new Query(Builder, commandString);
			return await ExecuteNonQuery(query);
		}

		/// <summary>
		/// Execute a scalar query asynchronously and get the returned value.
		/// </summary>
		/// <param name="commandString"></param>
		/// <returns></returns>
		public async Task<object> ExecuteScalar(string commandString)
		{
			Query query = new Query(Builder, commandString);
			return await ExecuteScalar(query);
		}

		/// <summary>
		/// Execute a scalar query asynchronously and get the returned value after casting it to the type T.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public async Task<T> ExecuteScalar<T>(Query query)
		{
			return (T)(await ExecuteScalar(query));
		}

		/// <summary>
		/// Execute a scalar query asynchronously and get the returned value after casting it to the type T.
		/// </summary>
		/// <param name="commandString"></param>
		/// <returns></returns>
		public async Task<T> ExecuteScalar<T>(string commandString)
		{
			Query query = new Query(Builder, commandString);
			return (T)(await ExecuteScalar(query));
		}

		/// <summary>
		/// Execute a query asynchronously and deserialize each row of the result set
		/// into an object of type T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		public virtual async Task<List<T>> Execute<T>(Query query) where T : new()
		{
			ResultSet result = await Execute(query);

			ResultSerializer<T> serializer = new ResultSerializer<T>(Builder);
			List<T> resultObjects = new List<T>();

			foreach (ResultRow row in result)
			{
				resultObjects.Add(await serializer.Deserialize(this, row));
			}

			return resultObjects;
		}

		/// <summary>
		/// Execute a transaction asynchronously, one query at a time, in the order that they are passed
		/// to this method.
		/// </summary>
		/// <param name="queries"></param>
		/// <returns></returns>
		public Task<List<NonQueryResult>> ExecuteTransaction(params Query[] queries)
		{
			return ExecuteTransaction(IsolationLevel.Unspecified, queries);
		}

		/// <summary>
		/// Insert the given serializable object into the DB.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns>The number of rows affected</returns>
		public Task<NonQueryResult> Insert<T>(T obj)
		{
			InsertQuery<T> query = new InsertQuery<T>(Builder, obj);
			return ExecuteNonQuery(query);
		}

		/// <summary>
		/// Insert all of the given serializable objects into the DB.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objects"></param>
		/// <returns></returns>
		public async Task<List<NonQueryResult>> InsertAll<T>(IEnumerable<T> objects)
		{
			List<NonQueryResult> results = new List<NonQueryResult>();
			foreach (T obj in objects)
			{
				results.Add(await Insert(obj));
			}
			return results;
		}

		/// <summary>
		/// Asynchronously select all of the rows in the table that T specifies
		/// and serialize the result into a list of T objects.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public Task<List<T>> Select<T>() where T : new()
		{
			SelectQuery<T> query = new SelectQuery<T>(Builder);
			return Execute<T>(query);
		}

		/// <summary>
		/// Delete any rows from the DB that match the values of the given object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns>The number of rows affected</returns>
		public Task<NonQueryResult> Delete<T>(T obj)
		{
			DeleteQuery<T> query = new DeleteQuery<T>(Builder, obj);
			return ExecuteNonQuery(query);
		}

		/// <summary>
		/// This method will update the object using the given method, and will also apply these updates to the database row of this object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="updateFunc"></param>
		/// <returns></returns>
		public Task<NonQueryResult> Update<T>(T obj, Func<T, T> updateFunc) where T : new()
		{
			UpdateQuery<T> query = new UpdateQuery<T>(Builder);

			query.Where(obj);

			obj = updateFunc(obj);

			query.Set(obj);

			return ExecuteNonQuery(query);
		}

		/// <summary>
		/// This method will update the object using the given method, and will also apply these updates to the database row of this object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="updateFunc"></param>
		/// <returns></returns>
		public Task<NonQueryResult> Update<T>(T obj, Action<T> updateFunc) where T : new()
		{
			return Update<T>(obj, o =>
			{
				updateFunc(o);
				return o;
			});
		}

		/// <summary>
		/// Execute a DROP TABLE IF EXISTS query.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public virtual async Task DropTableIfExists(string tableName)
		{
			await ExecuteNonQuery("DROP TABLE IF EXISTS " + tableName);
		}

		#endregion
	}
}
