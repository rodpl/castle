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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	using System.Reflection;

	using NHibernate.Cfg;

	using Castle.Model.Configuration;

	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// Performs the initialization of the framework
	/// </summary>
	public class ActiveRecordStarter
	{
		/// <summary>
		/// Initialize tha mappings using the configuration and 
		/// the list of types
		/// </summary>
		public static void Initialize( IConfigurationSource source, params Type[] types )
		{
			if (source == null) throw new ArgumentNullException("source");
			if (types == null) throw new ArgumentNullException("types");

			// First initialization
			SessionFactoryHolder holder = new SessionFactoryHolder();
			ActiveRecordBase._holder = holder;

			// Base configuration
			SetUpConfiguration(source, typeof(ActiveRecordBase), holder);

			NHibernateMappingEngine engine = new NHibernateMappingEngine();

			foreach( Type type in types )
			{
				if ( !typeof(ActiveRecordBase).IsAssignableFrom( type ) )
				{
					continue;
				}

				SetUpConfiguration(source, type, holder);

				Configuration cfg = holder.GetConfiguration( holder.GetRootType(type) );

				if (!type.IsAbstract)
				{
					String xml = engine.CreateMapping(type, types);
					if (xml != String.Empty) cfg.AddXmlString(xml);
				}
			}
		}

		/// <summary>
		/// Initialize tha mappings using the configuration and 
		/// checking all the types on the specified <c>Assembly</c>
		/// </summary>
		public static void Initialize( Assembly assembly, IConfigurationSource source )
		{
			Type[] types = assembly.GetExportedTypes();

			ArrayList list = new ArrayList();

			foreach( Type type in types )
			{
				if ( !typeof(ActiveRecordBase).IsAssignableFrom( type ) )
				{
					continue;
				}

				list.Add(type);
			}

			Initialize( source, (Type[]) list.ToArray( typeof(Type) ) );
		}

		/// <summary>
		/// Initializes the framework reading the configuration from
		/// the <c>AppDomain</c> and checking all the types on the executing <c>Assembly</c>
		/// </summary>
		public static void Initialize()
		{
			IConfigurationSource source = System.Configuration.ConfigurationSettings.GetConfig("activerecord") as IConfigurationSource;
			
			if (source == null)
			{
				String message = "Could not obtain configuration from the AppDomain config file." + 
					" Sorry, but you have to fill the configuration or provide a " + 
					"IConfigurationSource instance yourself.";
				throw new System.Configuration.ConfigurationException(message);
			}
			
			Initialize( Assembly.GetExecutingAssembly(), source );
		}

		private static Configuration CreateConfiguration(IConfiguration config)
		{
			Configuration cfg = new Configuration();

			foreach(String key in config.Attributes.AllKeys)
			{
				cfg.Properties.Add(key, config.Attributes[key]);
			}

			return cfg;
		}

		private static void SetUpConfiguration(IConfigurationSource source, Type type, SessionFactoryHolder holder)
		{
			IConfiguration config = source.GetConfiguration(type);
	
			if (config != null)
			{
				Configuration nconf = CreateConfiguration(config);
				holder.Register( type, nconf );
			}
		}
	}
}
