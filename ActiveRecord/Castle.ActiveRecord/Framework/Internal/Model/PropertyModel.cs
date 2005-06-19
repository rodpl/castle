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

namespace Castle.ActiveRecord.Framework.Internal
{
	using System;
	using System.Reflection;


	public class PropertyModel : IModelNode
	{
		private readonly PropertyInfo prop;
		private readonly PropertyAttribute att;

		public PropertyModel(PropertyInfo prop, PropertyAttribute att)
		{
			this.prop = prop;
			this.att = att;
		}

		public PropertyInfo Property
		{
			get { return prop; }
		}

		public PropertyAttribute PropertyAtt
		{
			get { return att; }
		}

		#region IModelNode Members

		public String ToXml()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IVisitable Members

		public void Accept(IVisitor visitor)
		{
			visitor.VisitProperty(this);
		}

		#endregion
	}
}
