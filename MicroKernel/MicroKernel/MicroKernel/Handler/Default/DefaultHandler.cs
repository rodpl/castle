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

namespace Castle.MicroKernel.Handler.Default
{
	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Concerns;
	using Castle.MicroKernel.Factory.Default;

	/// <summary>
	/// Summary description for DefaultHandler.
	/// </summary>
	public class DefaultHandler : BaseHandler
	{
		public DefaultHandler( IComponentModel model ) : base( model )
		{
		}

		protected override void CreateComponentFactoryAndLifestyleManager()
		{
			IComponentFactory delegateFactory = new BaseComponentFactory( 
				Kernel, this, ComponentModel, m_serv2handler );

			if (Kernel is IAvalonKernel)
			{
				IAvalonKernel kernel = Kernel as IAvalonKernel;

				IConcern commissionChain = kernel.Concerns.GetCommissionChain( kernel );
				IConcern decommissionChain = kernel.Concerns.GetDecommissionChain( kernel );

				ConcernChainComponentFactory factory = 
					new ConcernChainComponentFactory( 
						commissionChain, decommissionChain, 
						ComponentModel, delegateFactory );

				delegateFactory = factory;
			}

			m_lifestyleManager = 
				Kernel.LifestyleManagerFactory.Create( 
					delegateFactory, ComponentModel );
		}
	}
}
