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

namespace Castle.Model
{
	using System.Collections;

	using Castle.Model.Interceptor;

	/// <summary>
	/// Collection of <see cref="InterceptorReference"/>
	/// </summary>
	public class InterceptorReferenceCollection : ReadOnlyCollectionBase
	{
		public void Add(InterceptorReference interceptor)
		{
			InnerList.Add( interceptor );
		}

		public void Insert(int index, InterceptorReference interceptor)
		{
			InnerList.Insert( index, interceptor );
		}

		public bool HasInterceptors
		{
			get { return Count != 0; }
		}
	}
}
