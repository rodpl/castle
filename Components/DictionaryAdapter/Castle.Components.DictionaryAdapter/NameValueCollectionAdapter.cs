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

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections.Specialized;

	public class NameValueCollectionAdapter : AbstractDictionaryAdapter
	{
		private readonly NameValueCollection nameValues;

		public NameValueCollectionAdapter(NameValueCollection nameValues)
		{
			this.nameValues = nameValues;
		}

		public override bool IsReadOnly
		{
			get { return false; }
		}

		public override bool Contains(object key)
		{
			return Array.IndexOf(nameValues.AllKeys, key) >= 0;
		}

		public override object this[object key]
		{
			get { return nameValues[key.ToString()]; }
			set
			{
				String val = (value != null) ? value.ToString() : null;
				nameValues[key.ToString()] = val;
			}
		}
		
		public static NameValueCollectionAdapter Adapt(NameValueCollection nameValues)
		{
			return new NameValueCollectionAdapter(nameValues);
		}
	}
}