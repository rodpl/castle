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
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Enumeration used to mark the component's lifestyle.
	/// </summary>
	public enum LifestyleType
	{
		/// <summary>
		/// No lifestyle specified.
		/// </summary>
		Undefined,
		/// <summary>
		/// Singleton components are instantiated once, and shared
		/// between all clients.
		/// </summary>
		Singleton,
		/// <summary>
		/// Thread components have a unique instance per thread.
		/// </summary>
		Thread,
		/// <summary>
		/// Transient components are created on demand.
		/// </summary>
		Transient,
		
		Custom
	}

	/// <summary>
	/// Enumeration to define the component's activation policy.
	/// </summary>
	public enum ActivationType
	{
		/// <summary>
		/// No activation policy specified.
		/// </summary>
		Undefined,
		/// <summary>
		/// The component should be activated as soon as the container is ready.
		/// </summary>
		Start,
		/// <summary>
		/// The component should be activated only if requested.
		/// </summary>
		OnDemand
	}



	/// <summary>
	/// Summary description for ComponentModel.
	/// </summary>
	public class ComponentModel
	{
		private String _name;
		private Type _service;
		private Type _implementation;
		private LifestyleType _lifestyleType;
		private Type _customLifestyle;
		private ConstructorCandidateCollection _constructors;
		private PropertySetCollection _properties;
		private LifecycleStepCollection _lifecycleSteps;
		private IDictionary _extended;

		public ComponentModel(String name, Type service, Type implementation)
		{
			_name = name;
			_service = service;
			_implementation = implementation;
			_lifestyleType = LifestyleType.Undefined;

			_extended = new HybridDictionary(true);
			_constructors = new ConstructorCandidateCollection();
			_properties = new PropertySetCollection();
			_lifecycleSteps = new LifecycleStepCollection();
		}

		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public Type Service
		{
			get { return _service; }
			set { _service = value; }
		}

		public Type Implementation
		{
			get { return _implementation; }
			set { _implementation = value; }
		}

		public IDictionary ExtendedProperties
		{
			get { return _extended; }
		}

		public ConstructorCandidateCollection Constructors
		{
			get { return _constructors; }
		}

		public PropertySetCollection Properties
		{
			get { return _properties; }
		}

		public LifecycleStepCollection LifecycleSteps
		{
			get { return _lifecycleSteps; }
		}

		public LifestyleType LifestyleType
		{
			get { return _lifestyleType; }
			set { _lifestyleType = value; }
		}

		public Type CustomLifestyle
		{
			get { return _customLifestyle; }
			set { _customLifestyle = value; }
		}

		public void Freeze()
		{
			// TODO: Freeze
		}
	}
}
