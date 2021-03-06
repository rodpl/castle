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

namespace Castle.MonoRail.Views.AspView.Compiler.StatementProcessors
{
	using OutputMethodGenerators;

	public class SharpStatementProcessor : IStatementProcessor
	{
		public StatementInfo GetInfoFor(string statement)
		{
			string content = statement.Trim().Substring(1).Trim();

			return new StatementInfo(
				new EncodedOutputMethodGenerator(), content);
		}

		public bool CanHandle(string statement)
		{
			statement = statement.Trim();
			return statement.StartsWith("#");
		}
	}
}