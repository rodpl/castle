// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.Db4oIntegration.Tests.Components
{
	using Db4objects.Db4o;
	using Db4objects.Db4o.Query;

	public class BeerDao
	{
		private readonly IObjectContainer _objContainer;

		public BeerDao(IObjectContainer objContainer)
		{
			_objContainer = objContainer;
		}

		public virtual void Create(Beer beer)
		{
			_objContainer.Store(beer);
		}

		public virtual void Remove(Beer beer)
		{
			_objContainer.Delete(beer);
		}

		public virtual Beer Load(object id)
		{
			IQuery query = _objContainer.Query();

			query.Constrain(typeof(Beer));
			query.Descend("_id").Constrain(id);

			if (query.Execute().Count == 0)
			{
				return null;
			}
			else
			{
				return (Beer) query.Execute().Next();
			}
		}

		public virtual IObjectSet FindAll()
		{
			return _objContainer.QueryByExample(typeof(Beer));
		}
	}
}
