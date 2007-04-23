// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

#if DOTNET2

namespace Castle.ActiveRecord.Tests
{
	using Castle.ActiveRecord.Tests.Model.CompositeUserType;
	using NUnit.Framework;

	[TestFixture]
	public class CompositeUserTypeTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void CompositeUserTypeConfig()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Citizen));

			Recreate();

			Citizen c = new Citizen();

			c.Name = new string[] {"Jonh", "Doe"};
			c.ManufacturerName = new string[] {"Acme", "Inc"};

			c.Create();

			Citizen loaded = Citizen.Find(c.Id);

			Assert.IsNotNull(loaded);
			Assert.AreEqual("Jonh", c.Name[0]);
			Assert.AreEqual("Doe", c.Name[1]);
			Assert.AreEqual("Acme", c.ManufacturerName[0]);
		}
	}
}

#endif