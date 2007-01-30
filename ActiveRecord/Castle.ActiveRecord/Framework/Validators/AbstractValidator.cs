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

namespace Castle.ActiveRecord.Framework.Validators
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Abstract <see cref="IValidator"/> implementation
	/// </summary>
	[Serializable]
	public abstract class AbstractValidator : IValidator
	{
		private String errorMessage;
		private PropertyInfo property;

		/// <summary>
		/// Obtains the value of a property or field on a specific instance.
		/// </summary>
		/// <param name="instance">The instance to inspect.</param>
		/// <param name="fieldOrPropertyName">The name of the field or property to inspect.</param>
		/// <returns></returns>
		public object GetFieldOrPropertyValue(object instance, string fieldOrPropertyName)
		{
			PropertyInfo pi = instance.GetType().GetProperty(fieldOrPropertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

			if (pi == null)
			{
				FieldInfo fi = instance.GetType().GetField(fieldOrPropertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

				if (fi != null)
				{
					return fi.GetValue(instance);
				}
			}
			else
			{
				return pi.GetValue(instance, null);
			}

			throw new ValidationException("No public instance field or property named " + fieldOrPropertyName + " for type " + instance.GetType().FullName);
		}

		/// <summary>
		/// Implementors should perform any initialization logic
		/// </summary>
		/// <param name="property">The target property</param>
		public void Initialize(PropertyInfo property)
		{
			this.property = property;

			if (errorMessage == null)
			{
				errorMessage = BuildErrorMessage();
			}
		}

		/// <summary>
		/// The target property
		/// </summary>
		public PropertyInfo Property
		{
			get { return property; }
		}

		/// <summary>
		/// The error message to be displayed if the validation fails
		/// </summary>
		public String ErrorMessage
		{
			get { return errorMessage; }
			set { errorMessage = value; }
		}

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public bool Perform(object instance)
		{
			PropertyInfo instanceProp = instance.GetType().GetProperty(Property.Name);
			return this.Perform(instance, instanceProp.GetValue(instance, null));
		}

		/// <summary>
		/// Implementors should perform the actual validation upon
		/// the property value
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public abstract bool Perform(object instance, object fieldValue);

		/// <summary>
		/// Builds the error message.
		/// </summary>
		/// <returns></returns>
		protected abstract string BuildErrorMessage();
	}
}
