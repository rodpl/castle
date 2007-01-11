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

namespace Castle.Components.Validator
{
	using System;
	using System.Collections;

	/// <summary>
	/// Ensures that a property was 
	/// filled with some value
	/// </summary>
	[Serializable]
	public class NonEmptyValidator : AbstractValidator
	{
		/// <summary>
		/// Check that this property has a value that is not null or empty (if string)
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			return fieldValue != null && fieldValue.ToString().Length != 0;
		}

		public override bool SupportWebValidation
		{
			get { return true; }
		}

		public override void ApplyWebValidation(WebValidationConfiguration config, InputElementType inputType,
												IWebValidationGenerator generator, IDictionary attributes)
		{
			generator.SetAsRequired(BuildErrorMessage());
		}

		protected override string MessageKey
		{
			get { return MessageConstants.IsRequiredMessage; }
		}
	}
}