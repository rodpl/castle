// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Windsor.Proxy
{
	using System;

	using Castle.DynamicProxy;

	using Castle.Model.Interceptor;

	/// <summary>
	/// Represents an ordered chain of <see cref="IMethodInterceptor"/>
	/// implementations.
	/// </summary>
	[Serializable]
	public sealed class InterceptorChain : IMethodInterceptor, IInterceptor
	{
		private IMethodInterceptor[] _interceptors;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="interceptors"></param>
		public InterceptorChain(IMethodInterceptor[] interceptors)
		{
			if (interceptors == null) throw new ArgumentNullException("interceptors");

			_interceptors = interceptors;
		}

		/// <summary>
		/// Executes the method Intercept on the chain.
		/// </summary>
		/// <param name="invocation"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
#if DEBUG
			// Sanity check
			if (!(invocation is DefaultMethodInvocation))
			{
				throw new Exception("Expected 'invocation' to be an instance of DefaultMethodInvocation");
			}
#endif

			DefaultMethodInvocation minvocation = invocation as DefaultMethodInvocation;
			minvocation.Reset();

			if (!minvocation.IsInitialized)
			{
				minvocation.InterceptorChain = _interceptors;
			}

			return minvocation.Proceed( args );
		}

		public object Intercept(IInvocation invocation, params object[] args)
		{
			throw new NotImplementedException();
		}
	}
}
