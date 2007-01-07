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

	public class DateValidator : AbstractValidator
	{
		/// <summary>
		/// Gets a value indicating whether this validator supports web validation.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if web validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		public override bool SupportWebValidation
		{
			get { return true; }
		}

		/// <summary>
		/// Applies the web validation by setting up one or
		/// more input rules on <see cref="IWebValidationGenerator"/>.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="attributes">The attributes.</param>
		public override void ApplyWebValidation(WebValidationConfiguration config, InputElementType inputType,
		                                        IWebValidationGenerator generator, IDictionary attributes)
		{
			// TODO: web validation for date
			// generator.SetDigitsOnly(BuildErrorMessage());
		}

		public override bool IsValid(object instance, object fieldValue)
		{
			if (fieldValue == null) return false;

			try
			{
				Convert.ToDateTime(fieldValue.ToString());

				return true;
			}
			catch(Exception)
			{
				return false;
			}
		}

		protected override string BuildErrorMessage()
		{
			return String.Format(GetResourceForCurrentCulture().GetString(MessageConstants.InvalidDateMessage), Name);
		}
	}
}