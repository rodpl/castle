using System;

namespace Castle.Facilities.ActiveRecord
{
	/// <summary>
	/// Summary description for PropertyAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyAttribute : Attribute
	{
		private string _column;
		private string _update;
		private string _insert;
		private string _formula;
		private string _length;
		private string _notNull;

		public PropertyAttribute()
		{
		}

		public string NotNull
		{
			get { return _notNull; }
			set { _notNull = value; }
		}

		public string Length
		{
			get { return _length; }
			set { _length = value; }
		}

		public string Column
		{
			get { return _column; }
			set { _column = value; }
		}

		public string Update
		{
			get { return _update; }
			set { _update = value; }
		}

		public string Insert
		{
			get { return _insert; }
			set { _insert = value; }
		}

		public string Formula
		{
			get { return _formula; }
			set { _formula = value; }
		}
	}
}
