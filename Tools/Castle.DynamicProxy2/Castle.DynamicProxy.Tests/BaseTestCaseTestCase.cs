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

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Reflection.Emit;
	using Castle.Core.Interceptor;
	using NUnit.Framework;

	[TestFixture]
	public class BaseTestCaseTestCase : BasePEVerifyTestCase
	{
		public override void TearDown()
		{
			ResetGeneratorAndBuilder(); // we call TearDown ourselves in these test cases
			base.TearDown();
		}

		[Test]
		public void TearDown_DoesNotSaveAnything_IfNoProxyGenerated()
		{
			string path = ModuleScope.DEFAULT_FILE_NAME;
			if (File.Exists(path))
				File.Delete(path);

			base.TearDown();

			Assert.IsFalse(File.Exists(path));
		}

		[Test]
		public void TearDown_SavesAssembly_IfProxyGenerated()
		{
			string path = ModuleScope.DEFAULT_FILE_NAME;
			if (File.Exists(path))
				File.Delete(path);

			generator.CreateClassProxy(typeof (object), new StandardInterceptor());

			base.TearDown();
			Assert.IsTrue(File.Exists(path));
		}

		[Test]
		[ExpectedException(typeof (AssertionException))]
		public void TearDown_FindsVerificationErrors()
		{
			ModuleBuilder moduleBuilder = generator.ProxyBuilder.ModuleScope.ObtainDynamicModule(true);
			TypeBuilder invalidType = moduleBuilder.DefineType("InvalidType");
			MethodBuilder invalidMethod = invalidType.DefineMethod("InvalidMethod", MethodAttributes.Public);
			invalidMethod.GetILGenerator().Emit(OpCodes.Ldnull); // missing RET statement

			invalidType.CreateType();

			if (!IsVerificationDisabled)
				Console.WriteLine("This next test case is expected to yield a verification error.");

			base.TearDown();
		}

		[Test]
		public void DisableVerification_DisablesVerificationForTestCase()
		{
			DisableVerification();
			TearDown_FindsVerificationErrors();
		}

		[Test]
		public void DisableVerification_ResetInNextTestCase1()
		{
			Assert.IsFalse(IsVerificationDisabled);
			DisableVerification();
			Assert.IsTrue(IsVerificationDisabled);
		}

		[Test]
		public void DisableVerification_ResetInNextTestCase2()
		{
			Assert.IsFalse(IsVerificationDisabled);
			DisableVerification();
			Assert.IsTrue(IsVerificationDisabled);
		}
	}
}