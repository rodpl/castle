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

namespace Castle.ActiveRecord.Generator.Components
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord.Generator.Components.Database;


	[Serializable]
	public class Project
	{
		private String _name;
		private IList _databases = new ArrayList();
		private IList _descriptors = new ArrayList();
		private IDictionary _dbDef2ArBase = new Hashtable();

		public Project(string name)
		{
			_name = name;
		}

		public Project() : this("not named yet")
		{
		}

		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public void AddActiveRecordDescriptor( IActiveRecordDescriptor descriptor )
		{
			if (descriptor == null) throw new ArgumentNullException("descriptor");
			if (_descriptors.Contains(descriptor)) throw new ArgumentException("Already exists");

			_descriptors.Add(descriptor);
		}

		public void AddDatabaseDefinition(DatabaseDefinition dbDef)
		{
			if (dbDef == null) throw new ArgumentNullException("dbDef");

			ActiveRecordBaseDescriptor baseDesc;
			
			if (_dbDef2ArBase.Count == 0)
			{
				baseDesc = new ActiveRecordBaseDescriptor("ActiveRecordBase");
			}
			else
			{
				String name = dbDef.Alias.Replace(" ","") + "Base";
				baseDesc = new ActiveRecordBaseDescriptor(name);
			}

			dbDef.ActiveRecordBaseDescriptor = baseDesc;
			AddActiveRecordDescriptor(baseDesc);

			_databases.Add(dbDef);
			
			// Just an optimization for later
			_dbDef2ArBase[dbDef] = baseDesc;
		}

		public IList Databases
		{
			get { return ArrayList.ReadOnly(_databases); }
		}

		public IList Descriptors
		{
			get { return ArrayList.ReadOnly(_descriptors); }
		}

		public IDictionary BaseClasses
		{
			get { return _dbDef2ArBase; }
		}
	}
}
