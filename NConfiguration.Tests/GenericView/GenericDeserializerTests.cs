using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class GenericDeserializerTests
	{
		public class PrimitiveTypeCollection
		{
			public string TextField;
			public string TextProp {get; set;}
			public TestEn EnProp { get; set; }
			public TestEn? NEnProp { get; set; }
			public bool BoolField;
			public bool BoolProp { get; set; }
			public bool? NBoolField;
			public bool? NBoolProp { get; set; }
			public byte ByteField;
			public byte ByteProp { get; set; }
			public byte? NByteField;
			public byte? NByteProp { get; set; }
		}

		public enum TestEn
		{
			One,
			Two
		}

		[Test]
		public void ParsePrimitiveType()
		{
			var root = @"<Config
	TextField='val1' TextProp='val2'
	BoolField='+' BoolProp='false'
	NBoolField='T' NBoolProp=''
	ByteField='123' ByteProp='12'
	NByteProp=''
	EnProp='One'
	NEnProp='Two'
><NByteField>0</NByteField></Config>".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<PrimitiveTypeCollection>(root);

			Assert.AreEqual("val1", tc.TextField);
			Assert.AreEqual("val2", tc.TextProp);

			Assert.AreEqual(true, tc.BoolField);
			Assert.AreEqual(false, tc.BoolProp);
			Assert.AreEqual(true, tc.NBoolField);
			Assert.AreEqual(null, tc.NBoolProp);
			Assert.AreEqual(123, tc.ByteField);
			Assert.AreEqual(12, tc.ByteProp);
			Assert.AreEqual(0, tc.NByteField);
			Assert.AreEqual(null, tc.NByteProp);
			Assert.AreEqual(TestEn.One, tc.EnProp);
			Assert.AreEqual(TestEn.Two, tc.NEnProp);
		}

		public class ComplexTest
		{
			public int[] Array { get; set; }
			public IEnumerable<sbyte> Coll { get; set; }
			public ComplexTest Inner { get; set; }
			public List<ComplexTest> InnerList { get; set; }
		}

		[Test]
		public void ParseEmptyCollections()
		{
			var root =
@"<Root></Root>".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<ComplexTest>(root);

			CollectionAssert.IsEmpty(tc.Array);
			CollectionAssert.IsEmpty(tc.Coll);
			Assert.IsNull(tc.Inner);
			CollectionAssert.IsEmpty(tc.InnerList);
		}

		[Test]
		public void ParseArray1()
		{
			var root =
@"<Root Array='123'></Root>".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<ComplexTest>(root);

			CollectionAssert.AreEqual(new int[] { 123 }, tc.Array);
		}

		[Test]
		public void ParseArray2()
		{
			var root =
@"<root array='123'><array>345</array></root>".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<ComplexTest>(root);

			CollectionAssert.AreEqual(new int[] { 123, 345 }, tc.Array);
		}

		[Test]
		public void ParseCollection()
		{
			var root =
@"<root coll='5'><coll>-5</coll></root>".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<ComplexTest>(root);

			CollectionAssert.AreEqual(new sbyte[] { 5, -5 }, tc.Coll);
		}

		[Test]
		public void ParseInner()
		{
			var root =
@"<root><inner coll='5'><coll>-5</coll></inner></root>".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<ComplexTest>(root);

			Assert.NotNull(tc.Inner);
			CollectionAssert.AreEqual(new sbyte[] { 5, -5 }, tc.Inner.Coll);
		}

		[Test]
		public void ParseInnerList()
		{
			var root =
@"<root><innerlist coll='5'><coll>-5</coll></innerlist><innerlist coll='6'><coll>-6</coll></innerlist></root>".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<ComplexTest>(root);

			CollectionAssert.AreEqual(new sbyte[] { 5, -5 }, tc.InnerList[0].Coll);
			CollectionAssert.AreEqual(new sbyte[] { 6, -6 }, tc.InnerList[1].Coll);
		}
	}
}
