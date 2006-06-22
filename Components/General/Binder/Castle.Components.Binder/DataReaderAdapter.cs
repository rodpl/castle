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
	using System.Collections;
	using System.Collections.Specialized;
	using System.ComponentModel;
	using System.Data;

	public class DataReaderAdapter : IBindingDataSourceNode
	{
		private readonly IDataReader reader;
		private bool readSuccessful;

		public DataReaderAdapter(IDataReader reader)
		{
			this.reader = reader;

			ReadRow();
		}

		#region IBindingDataSourceNode

		public IBindingDataSourceNode ObtainNode(String name)
		{
			return this;
		}

		public String GetEntryValue(String name)
		{
			throw new NotImplementedException();
		}

		public Object GetEntryValue(String name, Type desiredType, out bool conversionSucceeded)
		{
			int ordinal = reader.GetOrdinal(name);

			if (ordinal < 0 || reader.IsDBNull(ordinal))
			{
				conversionSucceeded = false; 
				return null;
			}

			conversionSucceeded = true;

			if (desiredType == typeof(Int16))
			{
				return reader.GetInt16(ordinal);
			}
			else if (desiredType == typeof(Int32))
			{
				return reader.GetInt32(ordinal);
			}
			else if (desiredType == typeof(Int64))
			{
				return reader.GetInt64(ordinal);
			}
			else if (desiredType == typeof(String))
			{
				String value = reader.GetString(ordinal);
				
				if (value != null)
				{
					value = value.Trim();
				}
				
				return value;
			}
			else if (desiredType == typeof(Boolean))
			{
				return reader.GetBoolean(ordinal);
			}
			else if (desiredType == typeof(Byte))
			{
				return reader.GetByte(ordinal);
			}
			else if (desiredType == typeof(Char))
			{
				return reader.GetChar(ordinal);
			}
			else if (desiredType == typeof(DateTime))
			{
				return reader.GetDateTime(ordinal);
			}
			else if (desiredType == typeof(Decimal))
			{
				return reader.GetDecimal(ordinal);
			}
			else if (desiredType == typeof(Double))
			{
				return reader.GetDouble(ordinal);
			}
			else if (desiredType == typeof(Single))
			{
				return reader.GetFloat(ordinal);
			}
			else if (desiredType.IsEnum)
			{
				Type underlyingType = Enum.GetUnderlyingType(desiredType);

				if (underlyingType == typeof(byte))
				{
					byte val = reader.GetByte(ordinal);

					return Enum.ToObject(desiredType, val);
				}
				else if (underlyingType == typeof(Int16))
				{
					short val = reader.GetInt16(ordinal);

					return Enum.ToObject(desiredType, val);
				}
				else if (underlyingType == typeof(Int32))
				{
					int val = reader.GetInt32(ordinal);

					return Enum.ToObject(desiredType, val);
				}
				else if (underlyingType == typeof(Int64))
				{
					long val = reader.GetInt64(ordinal);
				
					return Enum.ToObject(desiredType, val);
				}
				else
				{
					throw new NotImplementedException("Only Int16, Int32 and Int64 " + 
						"enums types are supported");
				}
			}
			else if (desiredType == typeof(Guid))
			{
				return reader.GetGuid(ordinal);
			}
			else
			{
				object val = reader.GetValue(ordinal);
				
				TypeConverter conv = TypeDescriptor.GetConverter(desiredType);
				
				Type sourceType = (val != null ? val.GetType() : typeof(String));
				
				if (conv != null && conv.CanConvertFrom(sourceType))
				{
					return conv.ConvertFrom(val);
				}
				
				return val;
			}
		}

		public String GetMetaEntryValue(String name)
		{
			return null;
		}

		public IBindingDataSourceNode[] IndexedNodes
		{
			get { return BuildNodes(); }
		}

		public bool IsIndexed
		{
			get { return true; }
		}

		public bool CanConvert
		{
			get { return true; }
		}

		public bool ShouldIgnore
		{
			get { return false; }
		}

		public bool CanHandleNested
		{
			get { return false; }
		}

		#endregion

		#region IDictionary

		public bool Contains(object key)
		{
			throw new NotImplementedException();
		}

		public void Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public void Remove(object key)
		{
			throw new NotImplementedException();
		}

		public ICollection Keys
		{
			get { throw new NotImplementedException(); }
		}

		public ICollection Values
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsFixedSize
		{
			get { throw new NotImplementedException(); }
		}

		public object this[object key]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		#endregion

		#region ICollection

		public IEnumerator GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		public bool HasRow
		{
			get { return readSuccessful; }
		}

		private void ReadRow()
		{
			readSuccessful = reader.Read();
		}

		private IBindingDataSourceNode[] BuildNodes()
		{
			string[] fields = GetFields();
			int[] indexesToSkip = FindDuplicateFields(fields);

			ArrayList nodes = new ArrayList();

			int row = 0;

			while(readSuccessful)
			{
				DataSourceNode node = new DataSourceNode(row.ToString());

				for(int i=0; i<reader.FieldCount; i++)
				{
					// Is in the skip list?
					if (Array.IndexOf(indexesToSkip, i) >= 0) continue;
					
					// Is null?
					if (reader.IsDBNull(i)) continue;

					node.ProcessEntry(fields[i], reader.GetValue(i).ToString());
				}

				nodes.Add(node);
			
				ReadRow(); row++;
			}

			return (IBindingDataSourceNode[]) nodes.ToArray(typeof(IBindingDataSourceNode));
		}

		/// <summary>
		/// Check the fields for duplicates.
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		/// <remarks>
		/// I have to add this check as some stored procedures
		/// return duplicate columns (doh!) and this isn't good
		/// for the binder.
		/// </remarks>
		private int[] FindDuplicateFields(string[] fields)
		{
			HybridDictionary dict = new HybridDictionary(fields.Length, true);
			ArrayList duplicateList = new ArrayList();
			
			for(int i=0; i < fields.Length; i++)
			{
				String field = fields[i];
				
				if (dict.Contains(field))
				{
					duplicateList.Add(i);
					continue;
				}	
				
				dict.Add(field, String.Empty);
			}
			
			return (int[]) duplicateList.ToArray(typeof(int));
		}

		private string[] GetFields()
		{
			String[] fields = new String[reader.FieldCount];
	
			for(int i=0; i<reader.FieldCount; i++)
			{
				fields[i] = reader.GetName(i);
			}

			return fields;
		}
	}
}
