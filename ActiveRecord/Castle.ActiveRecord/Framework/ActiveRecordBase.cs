// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using Castle.ActiveRecord.Framework;
	using NHibernate;
	using NHibernate.Expression;

	/// <summary>
	/// Allow custom executions using the NHibernate's ISession.
	/// </summary>
	public delegate object NHibernateDelegate(ISession session, object instance);

	/// <summary>
	/// Base class for all ActiveRecord classes. Implements 
	/// all the functionality to simplify the code on the 
	/// subclasses.
	/// </summary>
	public abstract class ActiveRecordBase
	{
		/// <summary>
		/// Constructs an ActiveRecordBase subclass.
		/// </summary>
		public ActiveRecordBase()
		{
		}

		#region Overridable Hooks

		/// <summary>
		/// Hook to change the object state
		/// before saving it.
		/// </summary>
		/// <param name="state"></param>
		/// <returns>Return <c>true</c> if you have changed the state. <c>false</c> otherwise</returns>
		protected internal virtual bool BeforeSave(IDictionary state)
		{
			return false;
		}

		/// <summary>
		/// Hook to transform the read data 
		/// from the database before populating 
		/// the object instance
		/// </summary>
		/// <param name="adapter"></param>
		/// <returns>Return <c>true</c> if you have changed the state. <c>false</c> otherwise</returns>
		protected internal virtual bool BeforeLoad(IDictionary adapter)
		{
			return false;
		}

		/// <summary>
		/// Hook to perform additional tasks 
		/// before removing the object instance representation
		/// from the database.
		/// </summary>
		/// <param name="adapter"></param>
		protected internal virtual void BeforeDelete(IDictionary adapter)
		{
		}

		#endregion

		#region Base static methods

		/// <summary>
		/// Invokes the specified delegate passing a valid 
		/// NHibernate session. Used for custom NHibernate queries.
		/// </summary>
		/// <param name="targetType">The target ActiveRecordType</param>
		/// <param name="call">The delegate instance</param>
		/// <param name="instance">The ActiveRecord instance</param>
		/// <returns>Whatever is returned by the delegate invocation</returns>
		protected static object Execute(Type targetType, NHibernateDelegate call, object instance)
		{
			return DomainModel.Execute(targetType,call,instance);
		}

		internal protected static object FindByPrimaryKey(Type targetType, object id, bool throwOnNotFound)
		{
			return DomainModel.FindByPrimaryKey(targetType,id, throwOnNotFound);	
		}

		/// <summary>
		/// Finds an object instance by a unique ID
		/// </summary>
		/// <param name="targetType">The AR subclass type</param>
		/// <param name="id">ID value</param>
		/// <returns></returns>
		protected static object FindByPrimaryKey(Type targetType, object id)
		{
			return FindByPrimaryKey(targetType, id, true);
		}

		/// <summary>
		/// Returns all instances found for the specified type.
		/// </summary>
		/// <param name="targetType"></param>
		/// <returns></returns>
		protected static Array FindAll(Type targetType)
		{
			return FindAll(targetType, (Order[]) null);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		internal protected static Array SlicedFindAll(Type targetType, int firstResult, int maxresults, Order[] orders, params ICriterion[] criterias)
		{
			return DomainModel.SlicedFindAll(targetType,firstResult,maxresults,orders,criterias);
		}

		/// <summary>
		/// Returns a portion of the query results (sliced)
		/// </summary>
		internal protected static Array SlicedFindAll(Type targetType, int firstResult, int maxresults, params ICriterion[] criterias)
		{
			return SlicedFindAll(targetType, firstResult, maxresults, null, criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using sort orders and criterias.
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="orders"></param>
		/// <param name="criterias"></param>
		/// <returns></returns>
		internal protected static Array FindAll(Type targetType, Order[] orders, params ICriterion[] criterias)
		{
			return DomainModel.FindAll(targetType,orders,criterias);
		}

		/// <summary>
		/// Returns all instances found for the specified type 
		/// using criterias.
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="criterias"></param>
		/// <returns></returns>
		internal protected static Array FindAll(Type targetType, params ICriterion[] criterias)
		{
			return FindAll(targetType, null, criterias);
		}

		internal protected static void DeleteAll(Type type)
		{
			DomainModel.DeleteAll(type);
		}

		/// <summary>
		/// Saves the instance to the database
		/// </summary>
		/// <param name="instance"></param>
		protected static void Save(object instance)
		{
			DomainModel.Save(instance);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		/// <param name="instance"></param>
		protected static void Create(object instance)
		{
			DomainModel.Create(instance);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		/// <param name="instance"></param>
		protected static void Update(object instance)
		{
			DomainModel.Update(instance);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		/// <param name="instance"></param>
		protected static void Delete(object instance)
		{
			DomainModel.Delete(instance);
		}

		#endregion

		#region Base methods

		/// <summary>
		/// Invokes the specified delegate passing a valid 
		/// NHibernate session. Used for custom NHibernate queries.
		/// </summary>
		/// <param name="call">The delegate instance</param>
		/// <returns>Whatever is returned by the delegate invocation</returns>
		protected internal object Execute(NHibernateDelegate call)
		{
			return DomainModel.Execute( this.GetType(), call, this );
		}

		/// <summary>
		/// Saves the instance information to the database.
		/// May Create or Update the instance depending 
		/// on whether it has a valid ID.
		/// </summary>
		public virtual void Save()
		{
			DomainModel.Save(this);
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		public virtual void Create()
		{
			DomainModel.Create(this);
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		public virtual void Update()
		{
			DomainModel.Update(this);
		}

		/// <summary>
		/// Deletes the instance from the database.
		/// </summary>
		public virtual void Delete()
		{
			DomainModel.Delete(this);
		}

		#endregion
	}
}