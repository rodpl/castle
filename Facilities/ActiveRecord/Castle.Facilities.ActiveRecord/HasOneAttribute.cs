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

namespace Castle.Facilities.ActiveRecord
{
	using System;

	/// <summary>
	/// Summary description for HasOneAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class HasOneAttribute : Attribute
	{
		private Type _mapType;
		private string _cascade;
		private string _constrained;
		private string _outerJoin;

		public HasOneAttribute()
		{
		}

		public HasOneAttribute( Type mapType )
		{
			this._mapType = mapType;
		}

		public Type MapType
		{
			get { return _mapType; }
			set { _mapType = value; }
		}

		public string Cascade
		{
			get { return _cascade; }
			set { _cascade = value; }
		}

		public string Constrained
		{
			get { return _constrained; }
			set { _constrained = value; }
		}

		public string OuterJoin
		{
			get { return _outerJoin; }
			set { _outerJoin = value; }
		}
	}
}
