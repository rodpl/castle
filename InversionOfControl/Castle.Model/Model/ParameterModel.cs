// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Model
{
	using System;

	/// <summary>
	/// Represents a parameter. Usually the parameter
	/// comes from the external world, ie, an external configuration.
	/// </summary>
	public class ParameterModel
	{
		private String _name;
		private String _value;

		public ParameterModel( String name, String value )
		{
			_name = name;
			_value = value;
		}

		public String Name
		{
			get { return _name; }
		}

		public String Value
		{
			get { return _value; }
		}
	}
}
