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

namespace Castle.CastleOnRails.Engine
{
	using System;
	using System.Web;

	using Castle.CastleOnRails.Engine.Adapters;

	using Castle.CastleOnRails.Framework;
	using Castle.CastleOnRails.Framework.Internal;

	/// <summary>
	/// Extends the <see cref="ProcessEngine"/> and implements 
	/// <see cref="IHttpHandler"/> to dispatch the web
	/// requests. 
	/// </summary>
	public class RailsHttpHandler : ProcessEngine, IHttpHandler
	{
		private String _url;

		public RailsHttpHandler( String virtualRootDir, String url, IViewEngine viewEngine, 
			IControllerFactory controllerFactory, IFilterFactory filterFactory)
			: base(virtualRootDir, controllerFactory, viewEngine, filterFactory)
		{
			_url = url;
		}

		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			Process(new RailsEngineContextAdapter(context, _url));
		}

		public bool IsReusable
		{
			get { return true; }
		}

		#endregion
	}
}
