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

namespace Castle.MicroKernel.Lifestyle
{
	using System;

	/// <summary>
	/// Summary description for AbstractLifestyleManager.
	/// </summary>
	public abstract class AbstractLifestyleManager : ILifestyleManager
	{
		protected IComponentActivator _componentFactory;

		public virtual void Init(IComponentActivator componentActivator)
		{
			_componentFactory = componentActivator;
		}

		public virtual object Resolve()
		{
			return _componentFactory.Create();
		}

		public virtual void Release(object instance)
		{
			_componentFactory.Destroy( instance );
		}	

		public abstract void Dispose();
	}
}
