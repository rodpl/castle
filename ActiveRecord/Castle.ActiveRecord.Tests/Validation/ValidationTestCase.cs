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

namespace Castle.ActiveRecord.Tests.Validation
{
	using System;

	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Validation.Model;

	[TestFixture]
	public class ValidationTestCase
	{
		[Test]
		public void IsValid()
		{
			User user = new User();

			Assert.IsFalse(user.IsValid()); 

			user.Name = "hammett";
			user.Login = "hammett";
			user.Password = "123";
			user.ConfirmationPassword = "123";
			user.Email = "hammett@gmail.com";

			Assert.IsTrue(user.IsValid()); 
		}

		[Test]
		public void ErrorMessages()
		{
			User user = new User();

			Assert.IsFalse(user.IsValid()); 
			Assert.AreEqual(5, user.ValidationErrorMessages.Length);
			Assert.AreEqual("Login is not optional.", user.ValidationErrorMessages[0]);
			Assert.AreEqual("Name is not optional.", user.ValidationErrorMessages[1]);
			Assert.AreEqual("Email is not optional.", user.ValidationErrorMessages[2]);
			Assert.AreEqual("Password is not optional.", user.ValidationErrorMessages[3]);
			Assert.AreEqual("ConfirmationPassword is not optional.", user.ValidationErrorMessages[4]);

			user.Name = "hammett";
			user.Login = "hammett";
			user.Email = "hammett@gmail.com";
			user.Password = "123x";
			user.ConfirmationPassword = "123";
			
			Assert.IsFalse(user.IsValid()); 
			Assert.AreEqual(1, user.ValidationErrorMessages.Length);
			Assert.AreEqual("Field Password doesn't match with confirmation.", user.ValidationErrorMessages[0]);

			user.Password = "123";

			Assert.IsTrue(user.IsValid()); 
			Assert.AreEqual(0, user.ValidationErrorMessages.Length);
		}
	}
}
