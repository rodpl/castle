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

namespace Castle.ActiveRecord
{
	using System;

	/// <summary>
	/// Define how broken relations should be handled.
	/// </summary>
	[Serializable]
	public enum NotFoundBehaviour
	{
		/// <summary>
		/// Throw an exception when the relation is broken.
		/// </summary>
		Default,
		
		/// <summary>
		/// Throw an exception when the relation is broken.
		/// </summary>
		/// <remarks>this is the default behaviour</remarks>
		Exception,

		/// <summary>
		/// Ignore the broken relation and update
		/// the FK to null on the next save.
		/// </summary>
		Ignore
	}
	
	/// <summary>
	/// Maps a one to one association.
	/// </summary>
	/// <example>
	/// <code>
	/// public class Post : ActiveRecordBase
	/// {
	/// 		...
	/// 
	/// 		[BelongsTo("blogid")]
	/// 		public Blog Blog
	/// 		{
	/// 		get { return _blog; }
	/// 		set { _blog = value; }
	/// 		}
	/// 	</code>
	/// </example>
	/// <remarks>
	/// Please note that the 'blogid' foreign key lies on the 'Post' table.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class BelongsToAttribute : WithAccessAttribute
	{
		private Type type;
		private String column;
		private String[] compositeKeyColumns;
		private bool update = true;
		private bool insert = true;
		private bool notnull;
		private bool unique;
		private OuterJoinEnum outerJoin = OuterJoinEnum.Auto;
		private CascadeEnum cascade = CascadeEnum.None;
		private NotFoundBehaviour notFoundBehaviour = NotFoundBehaviour.Default;

		/// <summary>
		/// Initializes a new instance of the <see cref="BelongsToAttribute"/> class.
		/// </summary>
		public BelongsToAttribute()
		{
		}

		/// <summary>
		/// Indicates the name of the column to be used on the association.
		/// Usually the name of the foreign key field on the underlying database.
		/// </summary>
		public BelongsToAttribute(String column)
		{
			this.column = column;
		}

		/// <summary>
		/// Defines the target type of the association. It's usually inferred from the property type.
		/// </summary>
		public Type Type
		{
			get { return type; }
			set { type = value; }
		}

		/// <summary>
		/// Defines the column used by association (usually a foreign key)
		/// </summary>
		public String Column
		{
			get { return column; }
			set { column = value; }
		}
	    
		/// <summary>
		/// Defines the Composite Key columns used by association (aka Natural Keys).
		/// </summary>
		public String[] CompositeKeyColumns
		{
			get { return compositeKeyColumns; }
			set { compositeKeyColumns = value; }
		}

		/// <summary>
		/// Defines the cascading behavior of this association.
		/// </summary>
		public CascadeEnum Cascade
		{
			get { return cascade; }
			set { cascade = value; }
		}

		/// <summary>
		/// Defines the outer join behavior of this association.
		/// </summary>
		public OuterJoinEnum OuterJoin
		{
			get { return outerJoin; }
			set { outerJoin = value; }
		}

		/// <summary>
		/// Set to <c>false</c> to ignore this association when updating entities of this ActiveRecord class.
		/// </summary>
		public bool Update
		{
			get { return update; }
			set { update = value; }
		}

		/// <summary>
		/// Set to <c>false</c> to ignore this association when inserting entities of this ActiveRecord class.
		/// </summary>
		public bool Insert
		{
			get { return insert; }
			set { insert = value; }
		}

		/// <summary>
		/// Indicates whether this association allows nulls or not.
		/// </summary>
		public bool NotNull
		{
			get { return notnull; }
			set { notnull = value; }
		}

		/// <summary>
		/// Indicates whether this association is unique.
		/// </summary>
		public bool Unique
		{
			get { return unique; }
			set { unique = value; }
		}

		/// <summary>
		/// Gets or sets the way broken relations are handled.
		/// </summary>
		/// <value>The behaviour.</value>
		public NotFoundBehaviour NotFoundBehaviour
		{
			get { return notFoundBehaviour; }
			set { notFoundBehaviour = value; }
		}
	}
}
