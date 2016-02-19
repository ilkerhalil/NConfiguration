using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using System.Runtime.Serialization;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class XmlDeserializerTests
	{
		public class GoodType
		{
			[IgnoreDataMember]
			public bool Ignored;
			[DataMember(Name = "xmlNInt")]
			public int? NInt;
			[DataMember(Name = "xmlInner")]
			public GoodType Inner { get; set; }
			[DataMember(Name = "xmlInnerList")]
			public List<GoodType> InnerList { get; set; }
			[DataMember(Name = "xmlBlob")]
			public byte[] Blob;
		}

		[Test]
		public void ParseGoodType1()
		{
			var root = 
@"<Root Ignored='true'><xmlNInt>123</xmlNInt></Root>".ToXmlView();

			var t = DefaultDeserializer.Instance.Deserialize<GoodType>(root);

			Assert.IsFalse(t.Ignored);
			Assert.AreEqual(123, t.NInt);
			Assert.IsNull(t.Inner);
			Assert.AreEqual(0, t.InnerList.Count);
		}

		[Test]
		public void ParseGoodType2()
		{
			var root = 
@"<Root Ignored='1'><xmlInner xmlNInt='321'></xmlInner></Root>".ToXmlView();

			var t = DefaultDeserializer.Instance.Deserialize<GoodType>(root);

			Assert.IsFalse(t.Ignored);
			Assert.IsNotNull(t.Inner);
			Assert.AreEqual(321, t.Inner.NInt);
		}

		[Test]
		public void ParseGoodType_Blob()
		{
			var root =
@"<Root xmlBlob='MTIz'></Root>".ToXmlView();

			var t = DefaultDeserializer.Instance.Deserialize<GoodType>(root);

			Assert.That(t.Blob, Is.EquivalentTo(new byte[] {49, 50, 51}));
		}
	}
}
