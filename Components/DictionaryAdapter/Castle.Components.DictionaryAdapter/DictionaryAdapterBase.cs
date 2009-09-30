﻿// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;

	public abstract partial class DictionaryAdapterBase : IDictionaryAdapter
	{
		public DictionaryAdapterBase(Type type, IDictionaryAdapterFactory factory,
									 IDictionary dictionary, PropertyDescriptor descriptor)
		{
			Type = type;
			Factory = factory;
			Dictionary = dictionary;
			Descriptor = descriptor;
			State = new Hashtable();

			IsEditable = typeof(IEditableObject).IsAssignableFrom(type);
			WantsPropertyChangeNotification = typeof(INotifyPropertyChanged).IsAssignableFrom(type);

			if (IsEditable)
			{
				updates = new Dictionary<string, object>();
			}
		}

		public Type Type { get; private set; }

		public IDictionary State { get; private set; }

		public IDictionary Dictionary { get; private set;  }

		public PropertyDescriptor Descriptor { get; private set; }

		public IDictionaryAdapterFactory Factory { get; private set; }

		public abstract IDictionary<string, PropertyDescriptor> Properties { get; }

		public abstract void FetchProperties();

		public virtual object GetProperty(string propertyName)
		{
			PropertyDescriptor descriptor;
			if (Properties.TryGetValue(propertyName, out descriptor))
			{
				object propertyValue;
				if (GetEditedProperty(propertyName, out propertyValue))
				{
					return propertyValue;
				}
				return descriptor.GetPropertyValue(this, propertyName, null, Descriptor);
			}

			return null;
		}

		public virtual bool SetProperty(string propertyName, ref object value)
		{
			bool stored = false;

			PropertyDescriptor descriptor;
			if (Properties.TryGetValue(propertyName, out descriptor))
			{
				if (EditProperty(propertyName, value))
				{
					return true;
				}

				var trackPropertyChange = TrackPropertyChange(propertyName);
				try
				{
					stored = descriptor.SetPropertyValue(this, propertyName, ref value, Descriptor);
				}
				finally
				{
					if (stored && trackPropertyChange != null)
					{
						trackPropertyChange.Dispose();
					}
				}
			}

			return stored;
		}
	}
}
