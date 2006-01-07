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

	using Iesi.Collections;
	using Nullables;
	
	public abstract class ActiveRecordBaseQuery : IActiveRecordQuery
	{
		protected Type targetType;

		public ActiveRecordBaseQuery(Type type)
		{
			this.targetType = type;
		}

		public Type Target
		{
			get { return targetType; }
		}

		object IActiveRecordQuery.Execute(ISession session)
		{
			return InternalExecute(session);
		}

		[Obsolete("This method will be removed soon. Override InternalExecute instead.", true)]
		public virtual object Execute(ISession session)
		{
			throw new NotImplementedException();
		}

		protected abstract object InternalExecute(ISession session);

		/// <summary>
		/// Just a default clone implementation...
		/// </summary>
		public virtual object Clone()
		{
			return this.MemberwiseClone();
		}

		/// <summary>
		/// Converts the results stored in an <see cref="IList"/> to an
		/// strongly-typed array.
		/// </summary>
		/// <param name="t">The type of the new array</param>
		/// <param name="list">The source list</param>
		/// <param name="distinct">If true, only distinct results will be inserted in the array</param>
		/// <returns>The strongly-typed array</returns>
		protected Array GetResultsArray(Type t, IList list, bool distinct)
		{
			return GetResultsArray(t, list, null, distinct);
		}

		/// <summary>
		/// Converts the results stored in an <see cref="IList"/> to an
		/// strongly-typed array.
		/// </summary>
		/// <param name="t">The type of the new array</param>
		/// <param name="list">The source list</param>
		/// <param name="entityIndex">
		/// If the HQL clause selects more than one field, or a join is performed
		/// without using <c>fetch join</c>, the contents of the result list will
		/// be of type <c>object[]</c>. Specify which index in this array should be used to
		/// compose the new result array.
		/// </param>
		/// <param name="distinct">If true, only distinct results will be inserted in the array</param>
		/// <returns>The strongly-typed array</returns>
		protected Array GetResultsArray(Type t, IList list, NullableInt32 entityIndex, bool distinct)
		{
			// we only need to perform an additional processing if an
			// entityIndex was specified, or if distinct was chosen.
			if (distinct || entityIndex.HasValue) 
			{
				Set s = (distinct ? new ListSet() : null);

				IList newList = new ArrayList(list.Count);
				foreach (object o in list) 
				{
					object el = (!entityIndex.HasValue ? o : ((object[]) o)[entityIndex.Value]);
					if (s == null || s.Add(el))
						newList.Add(el);
				}

				list = newList;
			}

			Array a = Array.CreateInstance(t, list.Count);
			list.CopyTo(a, 0);
			return a;
		}
	}
}
