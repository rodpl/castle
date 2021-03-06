// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections;

	public class ConstructorCollection : CollectionBase
	{
		public void Add(ConstructorEmitter constructor)
		{
			InnerList.Add(constructor);
		}

		public ConstructorEmitter this[int index]
		{
			get { return (ConstructorEmitter) base.InnerList[index]; }
		}
	}
}