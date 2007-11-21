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
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Reflection;

	/// <summary>
	/// Describes a dictionary property.
	/// </summary>
	public class PropertyDescriptor : IDictionaryKeyBuilder,
	                                  IDictionaryPropertyGetter,
	                                  IDictionaryPropertySetter
	{
		private readonly PropertyInfo property;
		private ICollection<IDictionaryPropertyGetter> getters;
		private ICollection<IDictionaryPropertySetter> setters;
		private ICollection<IDictionaryKeyBuilder> keyBuilders;
		private TypeConverter typeConverter;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyDescriptor"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		public PropertyDescriptor(PropertyInfo property)
		{
			this.property = property;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyDescriptor"/> class
		/// with an existing property descriptor.
		/// </summary>
		/// <param name="other">The other descriptor.</param>
		public PropertyDescriptor(PropertyDescriptor other)
		{
			property = other.property;
			AddKeyBuilders(other.keyBuilders);
			AddGetters(other.getters);
			AddSetters(other.setters);
		}

		/// <summary>
		/// Gets the property name.
		/// </summary>
		public string PropertyName
		{
			get { return Property.Name; }
		}

		/// <summary>
		/// Gets the property type.
		/// </summary>
		public Type PropertyType
		{
			get { return Property.PropertyType; }
		}

		/// <summary>
		/// Gets the property.
		/// </summary>
		/// <value>The property.</value>
		public PropertyInfo Property
		{
			get { return property; }
		}

		/// <summary>
		/// Gets the type converter.
		/// </summary>
		/// <value>The type converter.</value>
		public TypeConverter TypeConverter
		{
			get
			{
				if (typeConverter == null)
				{
					Type converterType = AttributesUtil.GetTypeConverter(property);

					if (converterType != null)
					{
						typeConverter = (TypeConverter) Activator.CreateInstance(converterType);
					}
					else
					{
						typeConverter = TypeDescriptor.GetConverter(PropertyType);
					}
				}

				return typeConverter;
			}
		}

		/// <summary>
		/// Gets or sets the key builders.
		/// </summary>
		/// <value>The key builders.</value>
		public ICollection<IDictionaryKeyBuilder> KeyBuilders
		{
			get { return keyBuilders; }
			set { keyBuilders = value; }
		}

		/// <summary>
		/// Gets or sets the setter.
		/// </summary>
		/// <value>The setter.</value>
		public ICollection<IDictionaryPropertySetter> Setters
		{
			get { return setters; }
			set { setters = value; }
		}

		/// <summary>
		/// Gets or sets the getter.
		/// </summary>
		/// <value>The getter.</value>
		public ICollection<IDictionaryPropertyGetter> Getters
		{
			get { return getters; }
			set { getters = value; }
		}

		#region IDictionaryKeyBuilder Members

		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public string GetKey(IDictionary dictionary, string key,
		                     PropertyDescriptor descriptor)
		{
			if (keyBuilders != null)
			{
				foreach(IDictionaryKeyBuilder builder in keyBuilders)
				{
					key = builder.GetKey(dictionary, key, this);
				}
			}

			if (descriptor != null)
			{
				key = descriptor.GetKey(dictionary, key, null);
			}

			return key;
		}

		/// <summary>
		/// Adds the key builder.
		/// </summary>
		/// <param name="builder">The builder.</param>
		public void AddKeyBuilder(IDictionaryKeyBuilder builder)
		{
			if (keyBuilders == null)
			{
				keyBuilders = new List<IDictionaryKeyBuilder>();
			}
			keyBuilders.Add(builder);
		}

		/// <summary>
		/// Adds the key builders.
		/// </summary>
		/// <param name="builders">The builders.</param>
		public void AddKeyBuilders(ICollection<IDictionaryKeyBuilder> builders)
		{
			if (keyBuilders == null)
			{
				keyBuilders = builders;
			}
			else if (builders != null)
			{
				foreach(IDictionaryKeyBuilder builder in builders)
				{
					keyBuilders.Add(builder);
				}
			}
		}

		#endregion

		#region IDictionaryPropertyGetter Members

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="storedValue">The stored value.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public object GetPropertyValue(
			IDictionaryAdapterFactory factory, IDictionary dictionary,
			string key, object storedValue, PropertyDescriptor descriptor)
		{
			if (getters != null)
			{
				foreach(IDictionaryPropertyGetter getter in getters)
				{
					storedValue = getter.GetPropertyValue(
						factory, dictionary, key, storedValue, this);
				}
			}

			if (descriptor != null)
			{
				storedValue = descriptor.GetPropertyValue(
					factory, dictionary, key, storedValue, null);
			}

			return storedValue;
		}

		/// <summary>
		/// Adds the dictionary getter.
		/// </summary>
		/// <param name="getter">The getter.</param>
		public void AddGetter(IDictionaryPropertyGetter getter)
		{
			if (getters == null)
			{
				getters = new List<IDictionaryPropertyGetter>();
			}
			getters.Add(getter);
		}

		/// <summary>
		/// Adds the dictionary getters.
		/// </summary>
		/// <param name="gets">The getters.</param>
		public void AddGetters(ICollection<IDictionaryPropertyGetter> gets)
		{
			if (getters == null)
			{
				getters = gets;
			}
			else if (gets != null)
			{
				foreach(IDictionaryPropertyGetter getter in gets)
				{
					getters.Add(getter);
				}
			}
		}

		#endregion

		#region IDictionaryPropertySetter Members

		/// <summary>
		/// Sets the property value.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public object SetPropertyValue(
			IDictionaryAdapterFactory factory, IDictionary dictionary,
			string key, object value, PropertyDescriptor descriptor)
		{
			if (setters != null)
			{
				foreach(IDictionaryPropertySetter setter in setters)
				{
					value = setter.SetPropertyValue(
						factory, dictionary, key, value, this);
				}
			}

			if (descriptor != null)
			{
				value = descriptor.SetPropertyValue(
					factory, dictionary, key, value, null);
			}

			return value;
		}

		/// <summary>
		/// Adds the dictionary setter.
		/// </summary>
		/// <param name="setter">The setter.</param>
		public void AddSetter(IDictionaryPropertySetter setter)
		{
			if (setters == null)
			{
				setters = new List<IDictionaryPropertySetter>();
			}
			setters.Add(setter);
		}

		/// <summary>
		/// Adds the dictionary setters.
		/// </summary>
		/// <param name="sets">The setters.</param>
		public void AddSetters(ICollection<IDictionaryPropertySetter> sets)
		{
			if (setters == null)
			{
				setters = sets;
			}
			else if (sets != null)
			{
				foreach(IDictionaryPropertySetter setter in sets)
				{
					setters.Add(setter);
				}
			}
		}

		#endregion
	}
}