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

namespace Castle.CastleOnRails.Framework.Tests
{
	using System;

	using Castle.CastleOnRails.Engine;

	using Castle.CastleOnRails.Framework.Internal;
	using Castle.CastleOnRails.Framework.Tests.Controllers;

	public class FakeControllerFactory : IControllerFactory
	{
		public FakeControllerFactory()
		{
		}

		public Controller GetController(UrlInfo info)
		{
			if ("home".Equals(info.Controller))
			{
				return new HomeController();
			}
			else if ("filtered".Equals(info.Controller))
			{
				return new FilteredController();
			}
			else if ("filtered2".Equals(info.Controller))
			{
				return new Filtered2Controller();
			}
			else if ("exception".Equals(info.Controller))
			{
				return new ExceptionController();
			}
			else if ("layout".Equals(info.Controller))
			{
				return new LayoutController();
			}

			throw new RailsException("Unknown controller");
		}

		public void Release(Controller controller)
		{

		}
	}
}
