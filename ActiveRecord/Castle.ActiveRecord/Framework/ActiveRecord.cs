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

#if dotNet2
namespace Castle.ActiveRecord
{
	using System;
	using System.Text;
	using System.Collections.Generic;

	using NHibernate.Expression;

	public static class ActiveRecord<T> where T : ActiveRecordBase
	{
		public static T Find(object id)
		{
			return (T)ActiveRecordBase.FindByPrimaryKey(typeof(T), id, true);
		}

		public static T TryFind(object id)
		{
			return (T)ActiveRecordBase.FindByPrimaryKey(typeof(T), id, false);
		}
		
		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll(typeof(T));
		}

		public static T[] FindAll()
		{
			return (T[])ActiveRecordBase.FindAll(typeof(T));
		}

		public static T[] FindAll(params ICriterion[] criterias)
		{
			return (T[])ActiveRecordBase.FindAll(typeof(T), criterias);
		}

		public static T[] FindAll(ICriterion criteria, int firstResult, int maxresults)
		{
			return FindAll(new ICriterion[] { criteria }, firstResult, maxresults);
		}

		public static T[] FindAll(ICriterion[] criterias,int firstResult, int maxresults)
		{
			return (T[])ActiveRecordBase.SlicedFindAll(typeof(T), firstResult, maxresults, criterias);
		}

		public static T[] FindAll(ICriterion[] criterias, Order[] orders)
		{
			return (T[])ActiveRecordBase.FindAll(typeof(T), orders, criterias);
		}

		public static T[] FindAll( ICriterion[] criterias, Order[] orders,int firstResult, int maxresults)
		{
			return (T[]) ActiveRecordBase.SlicedFindAll(typeof(T),firstResult, maxresults,orders,criterias);
		}
	}
}
#endif