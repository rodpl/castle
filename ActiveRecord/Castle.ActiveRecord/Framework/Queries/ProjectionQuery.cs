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


#if DOTNET2

namespace Castle.ActiveRecord.Framework.Queries
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NHibernate;
	using NHibernate.Expression;
	using NHibernate.Transform;

	/// <summary>
	/// Performs a projected selection from an entity, lifting only the required fields.
	/// Similar to SELECT Id,Name FROM MyTable instead of selecting everything.
	/// It is possible to combine this with grouping. 
	/// </summary>
	/// <typeparam name="ARType">The active record entity type</typeparam>
	/// <typeparam name="TResultItem">The result value to use: object[] means returning as is</typeparam>
	/// /// <example>
	/// <code>
	/// ProjectionQuery{Post, PostTitleAndId} proj = new ProjectionQuery{Post, PostTitleAndId}(Projections.Property("Title"), Projections.Property("Id"));
	/// ICollection{PostTitleAndId} posts = proj.Execute();
	/// foreach(PostTitleAndId titleAndId in posts)
	/// {
	///		//push to site...
	/// }
	/// </code>
	/// </example>
	public class ProjectionQuery<ARType, TResultItem> : IActiveRecordQuery
	{
		IProjection[] projections;
		DetachedCriteria detachedCriteria;
		Order[] orders;

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given
		/// </summary>
		/// <param name="projections">The projections to use in the query</param>
		public ProjectionQuery(params IProjection[] projections)
		{
			this.projections = projections;
			this.orders = new Order[0];
			this.detachedCriteria = DetachedCriteria.For(Target);
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with a single projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(IProjection projection)
			: this(new IProjection[] { projection }) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the specified projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(IProjection projection1, IProjection projection2)
			: this(new IProjection[] { projection1, projection2 }) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the specified projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(IProjection projection1, IProjection projection2, IProjection projection3)
			: this(new IProjection[] { projection1, projection2, projection3 }) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		/// <param name="detachedCriteria">Criteria to select by</param>
		/// <param name="orders">The order by which to get the result</param>
		/// <param name="projections">The projections</param>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order[] orders, IProjection[] projections)
		{
			this.projections = projections;
			this.detachedCriteria = detachedCriteria;
			this.orders = orders;
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		/// <param name="detachedCriteria">Criteria to select by</param>
		/// <param name="order">The order by which to get the result</param>
		/// <param name="projections">The projections</param>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order order, params IProjection[] projections)
		{
			this.projections = projections;
			this.detachedCriteria = detachedCriteria;
			this.orders = new Order[] { order };
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with a single projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order order, IProjection projection)
			: this(detachedCriteria, order, new IProjection[] { projection }) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the specified projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order order, IProjection projection1, IProjection projection2)
			: this(detachedCriteria, order, new IProjection[] { projection1, projection2 }) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the specified projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order order, IProjection projection1, IProjection projection2, IProjection projection3)
			: this(detachedCriteria, order, new IProjection[] { projection1, projection2, projection3 }) { }


		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The results will be loaded according to the order specified
		/// </summary>
		public ProjectionQuery(Order order, IProjection[] projections)
		{
			this.projections = projections;
			this.detachedCriteria = DetachedCriteria.For(Target);
			this.orders = new Order[] { order };
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with a single projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(Order order, IProjection projection)
			: this(order, new IProjection[] { projection }) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the specified projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(Order order, IProjection projection1, IProjection projection2)
			: this(order, new IProjection[] { projection1, projection2 }) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the specified projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(Order order, IProjection projection1, IProjection projection2, IProjection projection3)
			: this(order, new IProjection[] { projection1, projection2, projection3 }) { }


		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		public ProjectionQuery(DetachedCriteria detachedCriteria, IProjection[] projections)
		{
			this.projections = projections;
			this.detachedCriteria = detachedCriteria;
			this.orders = new Order[0];
		}

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with a single projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(DetachedCriteria detachedCriteria, IProjection projection)
			: this(detachedCriteria, new IProjection[] { projection }) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the specified projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(DetachedCriteria detachedCriteria, IProjection projection1, IProjection projection2)
			: this(detachedCriteria, new IProjection[] { projection1, projection2 }) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the specified projections.
		/// </summary>
		/// <remarks>
		/// This is needed to bypass implicit conversion limitations in C# compiler
		/// </remarks>
		public ProjectionQuery(DetachedCriteria detachedCriteria, IProjection projection1, IProjection projection2, IProjection projection3)
			: this(detachedCriteria, new IProjection[] { projection1, projection2, projection3 }) { }

		/// <summary>
		/// Gets the target type of this query
		/// </summary>
		public Type Target
		{
			get { return typeof(ARType); }
		}

		/// <summary>
		/// Executes the specified query and return the results
		/// </summary>
		/// <param name="session">The session to execute the query in.</param>
		/// <returns></returns>
		object IActiveRecordQuery.Execute(ISession session)
		{
			return Execute();
		}

		/// <summary>
		/// Enumerates over the result of the query.
		/// Note: Only use if you expect most of your values to already exist in the second level cache!
		/// </summary>
		public IEnumerable Enumerate(ISession session)
		{
			return Execute();
		}

		/// <summary>
		/// Executes the specified query and return the results
		/// </summary>
		/// <returns>the result of the query</returns>
		public IList<TResultItem> Execute()
		{
			using (ISession session = GetSession())
			{
				ICriteria criteria = CreateCriteria(session);
				return criteria.List<TResultItem>();
			}
		}

		private ICriteria CreateCriteria(ISession session)
		{
			AssertAllArgumentsValid();
			ProjectionList projectionList = Projections.ProjectionList();
			foreach (IProjection projection in this.projections)
			{
				projectionList.Add(projection);
			}
			ICriteria criteria = this.detachedCriteria.GetExecutableCriteria(session);
			criteria.SetProjection(projectionList);

			if (typeof(TResultItem) != typeof(object[]))//we are not returning a tuple, so we need the result transformer
			{
				criteria.SetResultTransformer(new TypedResultTransformer<TResultItem>());
			}

			foreach (Order order in this.orders)
			{
				criteria.AddOrder(order);
			}
			return criteria;
		}

		private void AssertAllArgumentsValid()
		{
			if (this.projections == null)
			{
				throw new ArgumentNullException("projections");
			}
			if (this.orders == null)
			{
				throw new ArgumentNullException("orders");
			}
			if (this.detachedCriteria == null)
			{
				throw new ArgumentNullException("detachedCriteria");
			}
			if (this.projections.Length == 0)
			{
				throw new ActiveRecordException("Can't use projection query with zero projections!");
			}
		}

		private ISession GetSession()
		{
			return ActiveRecordBase.holder.GetSessionFactory(this.Target).OpenSession();
		}

		/// <summary>
		/// This is used to convert the resulting tuples into strongly typed objects.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private class TypedResultTransformer<T> : IResultTransformer
		{
			///<summary>
			///Convert the tuples into a strongly typed object
			///</summary>
			public object TransformTuple(object[] tuple, string[] aliases)
			{
				return Activator.CreateInstance(typeof(T), tuple);
			}

			public IList TransformList(IList collection)
			{
				return collection;
			}
		}
	}


	/// <summary>
	/// Default implemenation of ProjectionQuery that returns an Untyped object array tuples
	/// </summary>
	public class ProjectionQuery<ARType> : ProjectionQuery<ARType, object[]>
	{
		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		public ProjectionQuery(DetachedCriteria detachedCriteria, params IProjection[] projections)
			: base(detachedCriteria, projections) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The results will be loaded according to the order specified
		/// </summary>
		public ProjectionQuery(Order order, params IProjection[] projections)
			: base(order, projections) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		/// <param name="detachedCriteria">Criteria to select by</param>
		/// <param name="order">The order by which to get the result</param>
		/// <param name="projections">The projections</param>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order order, params IProjection[] projections)
			: base(detachedCriteria, order, projections) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given.
		/// The DetachedCriteria is mostly used for filtering, although it is possible to use it for ordering, limiting the 
		/// result set, etc.
		/// Note: Do not call SetProjection() on the detached criteria, since that is overwritten.
		/// </summary>
		/// <param name="detachedCriteria">Criteria to select by</param>
		/// <param name="orders">The order by which to get the result</param>
		/// <param name="projections">The projections</param>
		public ProjectionQuery(DetachedCriteria detachedCriteria, Order[] orders, params IProjection[] projections)
			: base(detachedCriteria, orders, projections) { }

		/// <summary>
		/// Create a new <see cref="ProjectionQuery{ARType,TResultItem}"/> with the given projections.
		/// At least one projections must be given
		/// </summary>
		/// <param name="projections">The projections to use in the query</param>
		public ProjectionQuery(params IProjection[] projections)
			: base(projections) { }
	}
}

#endif