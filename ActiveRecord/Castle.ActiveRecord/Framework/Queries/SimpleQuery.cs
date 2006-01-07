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

namespace Castle.ActiveRecord.Queries
{
	using System;
	
	using NHibernate;

	public class SimpleQuery : ActiveRecordBaseQuery
	{
		string hql;
		Type returnType;
		object[] parameters;

		public SimpleQuery(Type targetType, Type returnType, string hql, params object[] parameters)
			: base(targetType)
		{
			this.returnType = returnType;
			this.hql = hql;
			this.parameters = parameters;
		}

		public SimpleQuery(Type returnType, string hql, params object[] parameters)
			: this(returnType, returnType, hql, parameters)
		{
		}

		public string Query
		{
			get { return hql; }
		}

		protected override object InternalExecute(ISession session)
		{
			IQuery q = session.CreateQuery(hql);

			if (parameters != null)
				for (int i=0; i < parameters.Length; i++)
					q.SetParameter(i, parameters[i]);

			return GetResultsArray(returnType, q.List(), false);
		}
	}
}
