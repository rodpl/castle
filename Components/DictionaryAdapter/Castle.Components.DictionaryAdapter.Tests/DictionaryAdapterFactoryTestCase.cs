// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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


namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Globalization;
	using NUnit.Framework;

	[TestFixture]
	public class DictionaryAdapterFactoryTestCase
	{
		private IDictionary dictionary;
		private DictionaryAdapterFactory factory;

		[SetUp]
		public void SetUp()
		{
			dictionary = new Hashtable();
			factory = new DictionaryAdapterFactory();
		}

		[Test]
		public void CreateAdapter_NoPrefixPropertiesOnly_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			Assert.IsNotNull(person);
		}

		[Test, ExpectedException(typeof(TypeLoadException))]
		public void CreateAdapter_NoPrefixWithMethod_ThrowsException()
		{
			factory.GetAdapter<IPersonWithMethod>(dictionary);
		}

		[Test]
		public void CreateAdapter_PrefixPropertiesOnly_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			Assert.IsNotNull(person);
		}

		[Test]
		public void UpdateAdapter_NoPrefix_UpdatesDictionary()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>(); // I need some friends

			Assert.AreEqual("Craig", dictionary["Name"]);
			Assert.AreEqual(37, dictionary["Age"]);
			Assert.AreEqual(new DateTime(1970, 7, 19), dictionary["DOB"]);
			Assert.AreEqual(0, ((IList<IPerson>) dictionary["Friends"]).Count);
		}

		[Test]
		public void UpdateAdapter_Prefix_UpdatesDictionary()
		{
			IPerson person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();
			Assert.AreEqual("Craig", dictionary["Name"]);
			Assert.AreEqual(37, dictionary["Age"]);
			Assert.AreEqual(new DateTime(1970, 7, 19), dictionary["DOB"]);
			Assert.AreEqual(0, ((IList<IPerson>) dictionary["Friends"]).Count);
		}

		[Test]
		public void UpdateAdapterAndRead_NoPrefix_Matches()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();

			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(37, person.Age);
			Assert.AreEqual(new DateTime(1970, 7, 19), person.DOB);
			Assert.AreEqual(0, person.Friends.Count);
		}

		[Test]
		public void UpdateAdapterAndRead_Prefix_Matches()
		{
			IPerson person = factory.GetAdapter<IPersonWithPrefix>(dictionary);
			person.Name = "Craig";
			person.Age = 37;
			person.DOB = new DateTime(1970, 7, 19);
			person.Friends = new List<IPerson>();

			Assert.AreEqual("Craig", person.Name);
			Assert.AreEqual(37, person.Age);
			Assert.AreEqual(new DateTime(1970, 7, 19), person.DOB);
			Assert.AreEqual(0, person.Friends.Count);
		}

		[Test]
		public void UpdateAdapterAndRead_PrefixOverride_Matches()
		{
			IPerson person = factory.GetAdapter<IPersonWithPrefixOverride>(dictionary);
			person.Name = "Craig";

			Assert.AreEqual("Craig", dictionary["Name"]);
		}

		[Test]
		public void UpdateAdapterAndRead_TypePrefix_Matches()
		{
			IPersonWithTypePrefixOverride person = 
				factory.GetAdapter<IPersonWithTypePrefixOverride>(dictionary);
			person.Height = 72;

			Assert.AreEqual(72, person.Height);
			Assert.AreEqual(72, dictionary["Castle.Components.DictionaryAdapter.Tests.IPersonWithTypePrefixOverride#Height"]);
		}

		[Test]
		public void ReadAdapter_NoPrefixUnitialized_ReturnsDefaults()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);

			Assert.AreEqual(default(string), person.Name);
			Assert.AreEqual(default(int), person.Age);
			Assert.AreEqual(default(DateTime), person.DOB);
			Assert.AreEqual(default(IList<IPerson>), person.Friends);
		}

		[Test]
		public void ReadAdapter_PrefixUnitialized_ReturnsDefaults()
		{
			IPerson person = factory.GetAdapter<IPersonWithPrefix>(dictionary);

			Assert.AreEqual(default(string), person.Name);
			Assert.AreEqual(default(int), person.Age);
			Assert.AreEqual(default(DateTime), person.DOB);
			Assert.AreEqual(default(IList<IPerson>), person.Friends);
		}

		[Test]
		public void UpdateAdapterAndRead_WithSeveralDifferentOverridesWithDifferentPrefixes_DictionaryKeysHaveCorrectPrefixes()
		{
			IPersonWithDeniedInheritancePrefix person = factory.GetAdapter<IPersonWithDeniedInheritancePrefix>(dictionary);

			string name = "Ming The Merciless";
			int numberOfFeet = 2;
			string hairColour = "Muddy Golden Labrador";
			string eyeColour = "The Colour Of Broken Dreams";
			int numberOfHeads = 1;
			int numberOfFingers = 3;

			person.Name = name;
			person.NumberOfFeet = numberOfFeet;
			person.HairColour = hairColour;
			person.EyeColour = eyeColour;
			person.NumberOfHeads = numberOfHeads;
			person.NumberOfFingers = numberOfFingers;

			string[] keys = new string[dictionary.Keys.Count];
			dictionary.Keys.CopyTo(keys, 0);

			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "Name"; }));
			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "NumberOfFeet"; }));
			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "Person_HairColour"; }));
			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "Person2_Eye__Colour"; }));
			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "Person2_NumberOfHeads"; }));
			Assert.IsTrue(Array.Exists(keys, delegate(string key) { return key == "NumberOfFingers"; }));

			Assert.AreEqual(name, person.Name);
			Assert.AreEqual(numberOfFeet, person.NumberOfFeet);
			Assert.AreEqual(hairColour, person.HairColour);
			Assert.AreEqual(eyeColour, person.EyeColour);
			Assert.AreEqual(numberOfHeads, person.NumberOfHeads);
			Assert.AreEqual(numberOfFingers, person.NumberOfFingers);
		}

		[Test]
		public void CreateAdapter_WithSubstitionOnProperty_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			person.First_Name = "Craig";
			Assert.AreEqual("Craig", dictionary["First Name"]);
		}

		[Test]
		public void CreateAdapter_WithSubstitionOnInterface_WorksFine()
		{
			IPersonWithDeniedInheritancePrefix person = factory.GetAdapter<IPersonWithDeniedInheritancePrefix>(dictionary);
			person.Max_Width = 22;
			Assert.AreEqual(22, dictionary["Max Width"]);
		}

		[Test]
		public void CreateAdapter_WithComponent_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IAddress mailing = person.HomeAddress;
			Assert.IsNotNull(mailing);
		}

		[Test]
		public void ReadAdapter_WithComponent_WorksFine()
		{
			dictionary["HomeAddress_Line1"] = "77 Lynwood Dr";
			dictionary["HomeAddress_City"] = "Massapequa";
			dictionary["HomeAddress_State"] = "NY";
			dictionary["HomeAddress_ZipCode"] = "11288";

			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IAddress home = person.HomeAddress;

			Assert.AreEqual(dictionary["HomeAddress_Line1"], home.Line1);
			Assert.AreEqual(dictionary["HomeAddress_City"], home.City);
			Assert.AreEqual(dictionary["HomeAddress_State"], home.State);
			Assert.AreEqual(dictionary["HomeAddress_ZipCode"], home.ZipCode);
		}

		[Test]
		public void UpdateAdapter_WithComponent_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IAddress home = person.HomeAddress;
			home.Line1 = "77 Lynwood Dr";
			home.City = "Massapequa";
			home.State = "NY";
			home.ZipCode = "11288";

			Assert.AreEqual("77 Lynwood Dr", home.Line1);
			Assert.AreEqual("Massapequa", home.City);
			Assert.AreEqual("NY", home.State);
			Assert.AreEqual("11288", home.ZipCode);
		}

		[Test]
		public void ReadAdapter_WithComponentOverrideNoPrefix_WorksFine()
		{
			dictionary["Line1"] = "139 Dartbrook";
			dictionary["City"] = "Plano";
			dictionary["State"] = "TX";
			dictionary["ZipCode"] = "75062";

			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IAddress work = person.WorkAddress;

			Assert.AreEqual(dictionary["Line1"], work.Line1);
			Assert.AreEqual(dictionary["City"], work.City);
			Assert.AreEqual(dictionary["State"], work.State);
			Assert.AreEqual(dictionary["ZipCode"], work.ZipCode);
		}

		[Test]
		public void UpdateAdapter_WithComponentOverrideNoPrefix_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IAddress work = person.WorkAddress;
			work.Line1 = "139 Dartbrook";
			work.City = "Plano";
			work.State = "TX";
			work.ZipCode = "75062";

			Assert.AreEqual("139 Dartbrook", work.Line1);
			Assert.AreEqual("Plano", work.City);
			Assert.AreEqual("TX", work.State);
			Assert.AreEqual("75062", work.ZipCode);
		}

		[Test]
		public void ReadAdapter_WithComponentOverridePrefix_WorksFine()
		{
			dictionary["Billing_Line1"] = "64 Country Rd";
			dictionary["Billing_City"] = "Miami";
			dictionary["Billing_State"] = "FL";
			dictionary["Billing_ZipCode"] = "33101";

			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IAddress billing = person.BillingAddress;

			Assert.AreEqual(dictionary["Billing_Line1"], billing.Line1);
			Assert.AreEqual(dictionary["Billing_City"], billing.City);
			Assert.AreEqual(dictionary["Billing_State"], billing.State);
			Assert.AreEqual(dictionary["Billing_ZipCode"], billing.ZipCode);
		}

		[Test]
		public void UpdateAdapter_WithComponentOverridePrefix_WorksFine()
		{
			dictionary["Billing_Line1"] = "64 Country Rd";
			dictionary["Billing_City"] = "Miami";
			dictionary["Billing_State"] = "FL";
			dictionary["Billing_ZipCode"] = "33101";

			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IAddress billing = person.BillingAddress;
			billing.Line1 = "64 Country Rd";
			billing.City = "Miami";
			billing.State = "FL";
			billing.ZipCode = "33101";
		}

		[Test]
		public void CreateAdapter_WithNestedComponent_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IAddress mailing = person.HomeAddress;
			Assert.IsNotNull(mailing.Phone);
		}

		[Test]
		public void ReadAdapter_WithNestedComponent_WorksFine()
		{
			dictionary["HomeAddress_Phone_Number"] = "212-353-1244";
			dictionary["HomeAddress_Phone_Extension"] = "245";

			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IPhone phone = person.HomeAddress.Phone;

			Assert.AreEqual(dictionary["HomeAddress_Phone_Number"], phone.Number);
			Assert.AreEqual(dictionary["HomeAddress_Phone_Extension"], phone.Extension);
		}

		[Test]
		public void UpdateAdapter_WithNestedComponent_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IPhone phone = person.HomeAddress.Phone;
			phone.Number = "212-353-1244";
			phone.Extension = "245";

			Assert.AreEqual("212-353-1244", phone.Number);
			Assert.AreEqual("245", phone.Extension);
		}

		[Test]
		public void ReadAdapter_WithNestedComponentOverrideNoPrefix_WorksFine()
		{
			dictionary["Number"] = "972-324-9821";
			dictionary["Extension"] = "300";

			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IPhone phone = person.WorkAddress.Mobile;

			Assert.AreEqual(dictionary["Number"], phone.Number);
			Assert.AreEqual(dictionary["Extension"], phone.Extension);
		}

		[Test]
		public void UpdateAdapter_WithNestedComponentOverrideNoPrefix_WorksFine()
		{
			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IPhone phone = person.HomeAddress.Mobile;
			phone.Number = "972-324-9821";
			phone.Extension = "300";

			Assert.AreEqual("972-324-9821", phone.Number);
			Assert.AreEqual("300", phone.Extension);
		}

		[Test]
		public void ReadAdapter_WithNestedComponentOverridePrefix_WorksFine()
		{
			dictionary["HomeAddress_Emr_Number"] = "911";

			IPerson person = factory.GetAdapter<IPerson>(dictionary);
			IPhone phone = person.HomeAddress.Emergency;

			Assert.AreEqual(dictionary["HomeAddress_Emr_Number"], phone.Number);
			Assert.IsNull(dictionary["HomeAddress_Emr_Extension"]);
		}

		[Test]
		public void ReadAdapter_WithDefaultConversions_WorksFine()
		{
			DateTime now = DateTime.Now;
			Guid guid = Guid.NewGuid();

			dictionary["Int"] = string.Format("{0}", 22);
			dictionary["Float"] = string.Format("{0}", 98.6);
			dictionary["Double"] = string.Format("{0}", 3.14D);
			dictionary["Decimal"] = string.Format("{0}", 100M);
			dictionary["String"] = "Hello World";
			dictionary["DateTime"] = now.ToShortDateString();
			dictionary["Guid"] = guid.ToString();

			IConversions conversions = factory.GetAdapter<IConversions>(dictionary);
			Assert.AreEqual(22, conversions.Int);
			Assert.AreEqual(98.6, conversions.Float);
			Assert.AreEqual(3.14, conversions.Double);
			Assert.AreEqual(100, conversions.Decimal);
			Assert.AreEqual("Hello World", conversions.String);
			Assert.AreEqual(now.Date, conversions.DateTime.Date);
			Assert.AreEqual(guid, conversions.Guid);
		}

		[Test]
		public void UpdateAdapter_WithDefaultConversions_WorksFine()
		{
			DateTime today = DateTime.Today;
			Guid guid = Guid.NewGuid();

			IConversionsToString conversions = factory.GetAdapter<IConversionsToString>(dictionary);
			conversions.Int = 22;
			conversions.Float = 98.6F;
			conversions.Double = 3.14;
			conversions.Decimal = 100;
			conversions.DateTime = today;
			conversions.Guid = guid;
			conversions.Phone = new Phone("2124751012", "22");

			Assert.AreEqual(string.Format("{0}", 22), dictionary["Int"]);
			Assert.AreEqual(string.Format("{0}", 98.6), dictionary["Float"]);
			Assert.AreEqual(string.Format("{0}", 3.14D), dictionary["Double"]);
			Assert.AreEqual(string.Format("{0}", 100M), dictionary["Decimal"]);
			Assert.AreEqual(today.ToShortDateString(), dictionary["DateTime"]);
			Assert.AreEqual(guid.ToString(), dictionary["Guid"]);
			Assert.AreEqual("2124751012,22", dictionary["Phone"]);
		}

		[Test]
		public void ReadAdapter_WithDefaultNullableConversions_WorksFine()
		{
			DateTime? now = DateTime.Now;
			Guid? guid = Guid.NewGuid();

			dictionary["NullInt"] = string.Format("{0}", 22);
			dictionary["NullFloat"] = string.Format("{0}", 98.6);
			dictionary["NullDouble"] = string.Format("{0}", 3.14D);
			dictionary["NullDecimal"] = string.Format("{0}", 100M);
			dictionary["NullDateTime"] = now.Value.ToShortDateString();
			dictionary["NullGuid"] = guid.ToString();

			IConversions conversions = factory.GetAdapter<IConversions>(dictionary);
			Assert.AreEqual(22, conversions.NullInt);
			Assert.AreEqual(98.6, conversions.NullFloat);
			Assert.AreEqual(3.14, conversions.NullDouble);
			Assert.AreEqual(100, conversions.NullDecimal);
			Assert.AreEqual(now.Value.Date, conversions.NullDateTime.Value.Date);
			Assert.AreEqual(guid, conversions.NullGuid);
		}

		[Test]
		public void UpdateAdapter_WithDefaultNullableConversions_WorksFine()
		{
			DateTime? today = DateTime.Today;
			Guid? guid = Guid.NewGuid();

			IConversionsToString conversions = factory.GetAdapter<IConversionsToString>(dictionary);
			conversions.NullInt = 22;
			conversions.NullFloat = 98.6F;
			conversions.NullDouble = 3.14;
			conversions.NullDecimal = 100;
			conversions.NullDateTime = today;
			conversions.NullGuid = guid;

			Assert.AreEqual(string.Format("{0}", 22), dictionary["NullInt"]);
			Assert.AreEqual(string.Format("{0}", 98.6), dictionary["NullFloat"]);
			Assert.AreEqual(string.Format("{0}", 3.14D), dictionary["NullDouble"]);
			Assert.AreEqual(string.Format("{0}", 100M), dictionary["NullDecimal"]);
			Assert.AreEqual(today.Value.ToShortDateString(), dictionary["NullDateTime"]);
			Assert.AreEqual(guid.ToString(), dictionary["NullGuid"]);
		}

		[Test]
		public void ReadAdapter_WithDefaultNullConversions_WorksFine()
		{
			IConversions conversions = factory.GetAdapter<IConversions>(dictionary);
			Assert.IsNull(conversions.NullInt);
			Assert.IsNull(conversions.NullFloat);
			Assert.IsNull(conversions.NullDouble);
			Assert.IsNull(conversions.NullDecimal);
			Assert.IsNull(conversions.NullDateTime);
			Assert.IsNull(conversions.NullGuid);
		}

		[Test]
		public void UpdateAdapter_WithDefaultNullConversions_WorksFine()
		{
			IConversionsToString conversions = factory.GetAdapter<IConversionsToString>(dictionary);
			conversions.NullInt = null;
			conversions.NullFloat = null;
			conversions.NullDecimal = null;
			conversions.NullDateTime = null;
			conversions.NullGuid = null;

			Assert.IsNull(dictionary["NullInt"]);
			Assert.IsNull(dictionary["NullFloat"]);
			Assert.IsNull(dictionary["NullDouble"]);
			Assert.IsNull(dictionary["NullDecimal"]);
			Assert.IsNull(dictionary["NullDateTime"]);
			Assert.IsNull(dictionary["NullGuid"]);
		}

		[Test]
		public void ReadAdapter_WithStringLists_WorksFine()
		{
			dictionary["Names"] = "Craig,Brenda,Kaitlyn,Lauren,Matthew";
			dictionary["Ages"] = "37,36,5,3,1";

			IStringLists lists = factory.GetAdapter<IStringLists>(dictionary);

			IList<string> names = lists.Names;
			Assert.IsNotNull(names);
			Assert.AreEqual(5, names.Count);
			Assert.AreEqual("Craig", names[0]);
			Assert.AreEqual("Brenda", names[1]);
			Assert.AreEqual("Kaitlyn", names[2]);
			Assert.AreEqual("Lauren", names[3]);
			Assert.AreEqual("Matthew", names[4]);

			IList<int> ages = lists.Ages;
			Assert.AreEqual(5, ages.Count);
			Assert.AreEqual(37, ages[0]);
			Assert.AreEqual(36, ages[1]);
			Assert.AreEqual(5, ages[2]);
			Assert.AreEqual(3, ages[3]);
			Assert.AreEqual(1, ages[4]);
		}

		[Test]
		public void UpdateAdapter_WithStringLists_WorksFine()
		{
			IStringLists lists = factory.GetAdapter<IStringLists>(dictionary);

			IList<string> names = lists.Names;
			names.Add("Craig");
			names.Add("Brenda");
			names.Add("Kaitlyn");
			names.Add("Lauren");
			names.Add("Matthew");

			Assert.AreEqual("Craig,Brenda,Kaitlyn,Lauren,Matthew", dictionary["Names"]);

			IList<int> ages = new List<int>();
			ages.Add(37);
			ages.Add(36);
			ages.Add(5);
			ages.Add(3);
			ages.Add(1);
			lists.Ages = ages;

			Assert.AreEqual("37,36,5,3,1", dictionary["Ages"]);
		}
	}

	public interface IPhone
	{
		string Number { get; set; }
		string Extension { get; set; }
	}

	public class Phone : IPhone
	{
		private string number;
		private string extension;

		public Phone()
		{	
		}

		public Phone(string number, string extension)
		{
			this.number = number;
			this.extension = extension;
		}

		public string Extension
		{
			get { return extension; }
			set { extension = value; }
		}

		public string Number
		{
			get { return number; }
			set { number = value; }
		}
	}

	public interface IAddress
	{
		string Line1 { get; set; }
		string Line2 { get; set; }
		string City { get; set; }
		string State { get; set; }
		string ZipCode { get; set; }

		[DictionaryComponent]
		IPhone Phone { get; }

		[DictionaryComponent(NoPrefix = true)]
		IPhone Mobile { get; }

		[DictionaryComponent(Prefix = "Emr_")]
		IPhone Emergency { get; }
	}

	public class Address : IAddress
	{
		private string line1;
		private string line2;
		private string city;
		private string state;
		private string zipCode;
		private IPhone phone;
		private IPhone mobile;
		private IPhone emergency;

		public string Line1
		{
			get { return line1; }
			set { line1 = value; }
		}

		public string Line2
		{
			get { return line2; }
			set { line2 = value; }
		}

		public string City
		{
			get { return city; }
			set { city = value; }
		}

		public string State
		{
			get { return state; }
			set { state = value; }
		}

		public string ZipCode
		{
			get { return zipCode; }
			set { zipCode = value; }
		}

		public IPhone Phone
		{
			get
			{
				if (phone == null)
				{
					phone = new Phone();
				}
				return phone;
			}
		}

		public IPhone Mobile
		{
			get
			{
				if (mobile == null)
				{
					mobile = new Phone();
				}
				return mobile;
			}
		}


		public IPhone Emergency
		{
			get
			{
				if (emergency == null)
				{
					emergency = new Phone();
				}
				return emergency;
			}
		}
	}

	public interface IPerson
	{
		string Name { get; set; }

		int Age { get; set; }

		DateTime DOB { get; set; }

		IList<IPerson> Friends { get; set; }

		[DictionaryKeySubstitution("_", " ")]
		string First_Name { get; set; }

		[DictionaryComponent]
		IAddress HomeAddress { get; set; }

		[DictionaryComponent(NoPrefix = true)]
		IAddress WorkAddress { get; set; }

		[DictionaryComponent(Prefix = "Billing_")]
		IAddress BillingAddress { get; set; }
	}

	public interface IPersonWithoutPrefix : IPerson
	{
		int NumberOfFeet { get; set; }
	}

	[DictionaryKeyPrefix("Person_")]
	public interface IPersonWithPrefix : IPersonWithoutPrefix
	{
		string HairColour { get; set; }
	}

	[DictionaryKeyPrefix("Person2_")]
	public interface IPersonWithPrefixOverride : IPersonWithPrefix
	{
		[DictionaryKey("Eye__Colour")]
		string EyeColour { get; set; }
	}

	[DictionaryTypeKeyPrefix]
	public interface IPersonWithTypePrefixOverride : IPersonWithPrefixOverride
	{
		int Height { get; set; }
	}

	public interface IPersonWithPrefixOverrideFurtherOverride : IPersonWithPrefixOverride
	{
		int NumberOfHeads { get; set; }
	}

	[DictionaryKeyPrefix("")]
	[DictionaryKeySubstitution("_", " ")]
	public interface IPersonWithDeniedInheritancePrefix : IPersonWithPrefixOverrideFurtherOverride
	{
		int NumberOfFingers { get; set; }

		int Max_Width { get; set; }
	}

	public interface IPersonWithMethod : IPerson
	{
		void Run();
	}

	public interface IConversions
	{
		int Int { get; set; }
		float Float { get; set; }
		double Double { get; set; }
		decimal Decimal { get; set; }
		String String { get; set; }
		DateTime DateTime { get; set; }
		Guid Guid { get; set; }
		int? NullInt { get; set; }
		float? NullFloat { get; set; }
		double? NullDouble { get; set; }
		DateTime? NullDateTime { get; set; }
		Guid? NullGuid { get; set; }
		decimal? NullDecimal { get; set; }
	}

	[DictionaryStringValues]
	public interface IConversionsToString : IConversions
	{
		[TypeConverter(typeof(PhoneConverter))]
		Phone Phone { get; set; }
	}

	public interface IStringLists
	{
		[DictionaryStringList]
		IList<string> Names { get; }

		[DictionaryStringList]
		IList<int> Ages { get; set; }
	}

	public class PhoneConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context,
		                                    Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context,
		                                   CultureInfo culture, object value)
		{
			if (value is string)
			{
				string[] fields = ((string) value).Split(new char[] {','});
				return new Phone(fields[0], fields[1]);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context,
		                                 CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ((Phone) value).Number + "," + ((Phone) value).Extension;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
