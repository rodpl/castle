// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	
	using NHibernate;
	using NHibernate.Expression;
	
	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// Allow programmers to use the 
	/// ActiveRecord functionality without direct reference
	/// to <see cref="ActiveRecordBase"/>
	/// </summary>
	public class ActiveRecordMediator
	{
		/// <summary>
		/// Invokes the specified delegate passing a valid 
		/// NHibernate session. Used for custom NHibernate queries.
		/// </summary>
		/// <param name="targetType">The target ActiveRecordType</param>
		/// <param name="call">The delegate instance</param>
		/// <param name="instance">The ActiveRecord instance</param>
		/// <returns>Whatever is returned by the delegate invocation</returns>
		public static object Execute(Type targetType, NHibernateDelegate call, object instance)
		{
			return ActiveRecordBase.Execute(targetType, call, instance);
		}

		/// <summary>
		/// Finds an object instance by its primary key.
		/// </summary>
		/// <param name="targetType">The AR subclass type</param>
		/// <param name="id">ID value</param>
		/// <param name="throwOnNotFound"><c>true</c> if you want an exception to be thrown
		/// if the object is not found</param>
		/// <exception cref="ObjectNotFoundException">if <c>throwOnNotFound</c> is set to 
		/// <c>true</c> and the row is not found</exception>
		public static object FindByPrimaryKey(Type targetType, object id, bool throwOnNotFound)
		{
			return ActiveRecordBase.FindByPrimaryKey(targetType, id, throwOnNotFound);
		}

		/// <summary>
		/// Finds an object instance by its primary key.
		/// </summary>
		/// <param name="targetType">The AR subclass type</param>
		/// <param name="id">ID value</param>
		public static object FindByPrimaryKey(Type targetType, object id)
		{
			return FindByPrimaryKey(targetType, id, true);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="orders">The sort order - used to determine which record is the first one</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static object FindFirst(Type targetType, Order[] orders, params ICriterion[] criterias)
		{
			return ActiveRecordBase.FindFirst(targetType, orders, criterias);
		}

		/// <summary>
		/// Searches and returns the first row.
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static object FindFirst(Type targetType, params ICriterion[] criterias)
		{
			return FindFirst(targetType, null, criterias);
		}

		/// <summary>
		/// Searches and returns the a row. If more than one is found, 
		/// throws <see cref="ActiveRecordException"/>
		/// </summary>
		/// <param name="targetType">The target type</param>
		/// <param name="criterias">The criteria expression</param>
		/// <returns>A <c>targetType</c> instance or <c>null</c></returns>
		public static object FindOne(Type targetType, params ICriterion[] criterias)
		{
			return ActiveRecordBase.FindOne(targetType, criterias);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static Array SlicedFindAll(Type targetType, int firstResult, int maxresults,
		                                  Order[] orders, params ICriterion[] criterias)
		{
			return ActiveRecordBase.SlicedFindAll(targetType, firstResult,
			                                      maxresults, orders, criterias);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		public static Array SlicedFindAll(Type targetType, int firstResult, int maxresults, params ICriterion[] criterias)
		{
			return SlicedFindAll(targetType, firstResult, maxresults, null, criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public static Array FindAll(Type targetType)
		{
			return FindAll(targetType, (Order[]) null);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using sort orders and criterias.
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="orders"></param>
		/// <param name="criterias"></param>
		/// <returns></returns>
		public static Array FindAll(Type targetType, Order[] orders, params ICriterion[] criterias)
		{
			return ActiveRecordBase.FindAll(targetType, orders, criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criterias.
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="criterias"></param>
		/// <returns></returns>
		public static Array FindAll(Type targetType, params ICriterion[] criterias)
		{
			return FindAll(targetType, null, criterias);
		}

		/// <summary>
		/// Deletes all entities of the specified type (and their inheritors)
		/// </summary>
		/// <param name="type">The type.</param>
		public static void DeleteAll(Type type)
		{
			ActiveRecordBase.DeleteAll(type);
		}

		/// <summary>
		/// Deletes all entities of specified type that match the HQL where clause
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="where">The where.</param>
		public static void DeleteAll(Type type, string where)
		{
			ActiveRecordBase.DeleteAll(type, where);
		}

		/// <summary>
		/// Enumerates the query.
		/// Note: Only use if you expect most of the values to be on the second level cache
		/// </summary>
		/// <param name="q">The query</param>
		/// <returns></returns>
		public static IEnumerable EnumerateQuery(IActiveRecordQuery q)
		{
			return ActiveRecordBase.EnumerateQuery(q);
		}

		/// <summary>
		/// Executes the query
		/// </summary>
		/// <param name="q">The query</param>
		/// <returns></returns>
		public static object ExecuteQuery(IActiveRecordQuery q)
		{
			return ActiveRecordBase.ExecuteQuery(q);
		}

		/// <summary>
		/// Saves the instance to the database
		/// </summary>
		/// <param name="instance"></param>
		public static void Save(object instance)
		{
			ActiveRecordBase.Save(instance);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance"></param>
		public static void Create(object instance)
		{
			ActiveRecordBase.Create(instance);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance"></param>
		public static void Update(object instance)
		{
			ActiveRecordBase.Update(instance);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance"></param>
		public static void Delete(object instance)
		{
			ActiveRecordBase.Delete(instance);
		}
		
		/// <summary>
		/// Refresh the instance from the database.
		/// </summary>
		/// <param name="instance">The ActiveRecord instance to be reloaded</param>
		public static void Refresh(object instance)
		{
			ActiveRecordBase.Refresh(instance);
		}

		/// <summary>
		/// Testing hock only.
		/// </summary>
		public static ISessionFactoryHolder GetSessionFactoryHolder()
		{
			return ActiveRecordBase.holder;
		}
	}
}