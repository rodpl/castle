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

namespace Castle.MicroKernel.Subsystems.Configuration
{
	using System;

	using Apache.Avalon.Framework;

	/// <summary>
	/// Summary description for IConfigurationManager.
	/// </summary>
	public interface IConfigurationManager : IKernelSubsystem
	{
		/// <summary>
		/// Implementation should return a configuration for 
		/// the component.
		/// </summary>
		/// <param name="componentName"></param>
		/// <returns></returns>
		IConfiguration GetConfiguration( String componentName );

		/// <summary>
		/// Implementation should associate a configuration for
		/// the component name.
		/// </summary>
		/// <param name="componentName"></param>
		/// <param name="configuration"></param>
		void Add( String componentName, IConfiguration configuration );

		/// <summary>
		/// Returns configurations available.
		/// </summary>
		IConfiguration[] Configurations
		{
			get;
		}
	}
}
