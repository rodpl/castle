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

namespace Castle.MicroKernel.Test.Components
{
	using System;

	using Apache.Avalon.Framework;

	/// <summary>
	/// Summary description for AvalonSpamService2.
	/// </summary>
	[AvalonComponent("spamservice2", Lifestyle.Singleton)]
	[AvalonService( typeof(ISpamService) )]
	public class AvalonSpamService2 : ISpamService, IInitializable
	{
		public IMailService m_mailService;

		public AvalonSpamService2(IMailService mailservice)
		{
			m_mailService = mailservice;
		}

		#region ISpamService Members

		public void AnnoyPeople(String contents)
		{
			if (m_mailService == null)
			{
				throw new Exception("Dependency not satisfied.");
			}		
		}

		#endregion

		#region IInitializable Members

		public void Initialize()
		{
		}

		#endregion
	}
}
