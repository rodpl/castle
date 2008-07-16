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

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	using System.Collections.Generic;

	public class WithComponentAndSections : AspViewBase
	{
		public override void Render()
		{
			Output(@"Parent
");
			InvokeViewComponent("MyComponent", null,
				new KeyValuePair<string, ViewComponentSectionRendereDelegate>[] { new KeyValuePair<string, ViewComponentSectionRendereDelegate>("section1", MyComponent1_section1), new KeyValuePair<string, ViewComponentSectionRendereDelegate>("section2", MyComponent1_section2) });
			Output(@"
Parent");
		}


		internal void MyComponent1_section1()
		{
			Output(@"section1");
		}

		internal void MyComponent1_section2()
		{
			Output(@"section2");
		}

		protected override string ViewDirectory
		{
			get { return ""; }
		}

		protected override string ViewName
		{
			get { throw new System.Exception("The method or operation is not implemented."); }
		}
	}
}
