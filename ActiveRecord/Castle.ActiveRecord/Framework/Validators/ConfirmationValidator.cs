// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Validators
{
	using System;
	using System.Reflection;


	/// <summary>
	/// Validates that a property and a matching property are the same.
	/// This it used when you need to accept two identical inputs from the user, for instnace, 
	/// a password and its confirmation.
	/// </summary>
	public class ConfirmationValidator : AbstractValidator
	{
		private String _confirmationFieldOrProperty;

		/// <summary>
		/// Initializes a new instance of the <see cref="ConfirmationValidator"/> class.
		/// </summary>
		/// <param name="confirmationFieldOrProperty">The confirmation field or property.</param>
		public ConfirmationValidator(String confirmationFieldOrProperty)
		{
			_confirmationFieldOrProperty = confirmationFieldOrProperty;
		}

		/// <summary>
		/// Check that the confirmation property has the same value as this property.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public override bool Perform(object instance, object fieldValue)
		{
			object confValue = GetFieldOrPropertyValue(instance);

			if (confValue == null && (fieldValue == null || fieldValue.ToString().Length == 0))
			{
				return true;
			}
			else if (confValue == null)
			{
				return false;
			}

			if (fieldValue == null && (confValue == null || confValue.ToString().Length == 0))
			{
				return true;
			}
			else if (fieldValue == null)
			{
				return false;
			}

			return confValue.Equals(fieldValue);
		}

		private object GetFieldOrPropertyValue(object instance)
		{
			PropertyInfo pi = instance.GetType().GetProperty(_confirmationFieldOrProperty, BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.Public);
			
			if (pi == null)
			{
				FieldInfo fi = instance.GetType().GetField(_confirmationFieldOrProperty, BindingFlags.DeclaredOnly|BindingFlags.Instance|BindingFlags.Public);

				if (fi != null)
				{
					return fi.GetValue( instance );
				}
			}
			else
			{
				return pi.GetValue(instance, null);
			}

			throw new ValidationException("No public instance field or property named " + _confirmationFieldOrProperty + " for type " + instance.GetType().FullName);
		}

		/// <summary>
		/// Builds the default error message.
		/// </summary>
		/// <returns></returns>
		protected override string BuildErrorMessage()
		{
			return String.Format("Field {0} doesn't match with confirmation.", Property.Name);
		}
	}
}
