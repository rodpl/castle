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
	using System.Reflection;

	public class Validator
	{
		private readonly IValidatorRegistry registry;

		public Validator(IValidatorRegistry registry)
		{
			this.registry = registry;
		}

		public bool IsValid(object obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");

			bool isValid = true;

			foreach(IValidator validator in GetValidators(obj))
			{
				if (!validator.IsValid(obj))
				{
					isValid = false;
				}
			}

			return isValid;
		}

		public IValidator[] GetValidators(Type parentType, PropertyInfo property)
		{
			if (parentType == null) throw new ArgumentNullException("parentType");
			if (property == null) throw new ArgumentNullException("property");

			return registry.GetValidators(parentType, property);
		}

		private IValidator[] GetValidators(object obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");

			return registry.GetValidators(obj.GetType());
		}
	}
}
