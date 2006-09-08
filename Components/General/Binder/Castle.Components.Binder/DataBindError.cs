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

namespace Castle.Components.Binder
{
	using System;

	/// <summary>
	/// Represents an error that occurred when trying to 
	/// databind a property of an instance.
	/// </summary>
	public class DataBindError
	{
		private readonly String parent;
		private readonly String property;
		private readonly Exception exception;

		public DataBindError(String parent, String property) : this(parent, property, null)
		{
		}

		public DataBindError(String parent, String property, Exception exception)
		{
			this.parent = parent;
			this.property = property;
			this.exception = exception;
		}

		public String Key
		{
			get { return parent + "." + Property; }
		}

		public String Parent
		{
			get { return parent; }
		}

		public String Property
		{
			get { return property; }
		}

		public Exception Exception
		{
			get { return exception; }
		}

		public override String ToString()
		{
			if (Exception != null)
			{
				return Exception.Message;
			}

			return "BindError." + Key;
		}
	}
}