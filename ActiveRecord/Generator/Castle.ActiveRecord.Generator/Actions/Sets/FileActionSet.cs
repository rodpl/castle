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

namespace Castle.ActiveRecord.Generator.Actions
{
	using System;
	using System.Windows.Forms;


	public class FileActionGroup : IActionSet
	{
		private ProjectNewAction newAction;
		private ProjectOpenAction openAction;
		private ProjectSaveAction saveAction;

		public FileActionGroup()
		{
		}

		#region IAction Members

		public void Init(Model model)
		{
			newAction = new ProjectNewAction();
			openAction = new ProjectOpenAction();
			saveAction = new ProjectSaveAction();

			newAction.Init(model);
			openAction.Init(model);
			saveAction.Init(model);
		}

		public void Install(IWorkspace workspace)
		{
			MenuItem item = new MenuItem("P&roject");
			workspace.MainMenu.MenuItems.Add(item);

			newAction.Install(workspace, item, null);			
			openAction.Install(workspace, item, null);
			saveAction.Install(workspace, item, null);
		}

		#endregion
	}
}
