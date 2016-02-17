using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using NConfiguration.GenericView.Deserialization;

namespace NConfiguration.GenericView
{
	[TestFixture]
	public class XmlDeserializerTests
	{
		public class XmlMapper: GenericMapper
		{
			public override ComplexFunctionBuilder CreateComplexFunctionBuilder(Type targetType, IGenericDeserializer deserializer, StringConverter converter)
			{
				if (BuildToolkit.XmlAvailable(targetType) == AttributeState.NotImplemented)
					throw new NotImplementedException();
				return new ComplexFunctionBuilder(targetType, deserializer, converter, XmlFieldReader);
			}
		}

		public class BadType1
		{
			[XmlArray]
			public int[] Array { get; set; }
		}

		public class BadType2
		{
			[XmlAnyElement]
			public XmlElement AnyElement { get; set; }
		}

		[Test]
		[ExpectedException(typeof(NotImplementedException))]
		public void ParseBadType1()
		{
			var root = @"<Root></Root>".ToXmlView();
			var d = new GenericDeserializer(new XmlMapper());

			d.Deserialize<BadType1>(root);
		}

		[Test]
		[ExpectedException(typeof(NotImplementedException))]
		public void ParseBadType2()
		{
			var root = @"<Root></Root>".ToXmlView();
			var d = new GenericDeserializer(new XmlMapper());

			d.Deserialize<BadType2>(root);
		}

		public class GoodType
		{
			[XmlIgnore]
			public bool Ignored;
			[XmlAttribute("xmlNInt")]
			public int? NInt;
			[XmlElement("xmlInner")]
			public GoodType Inner { get; set; }
			[XmlElement("xmlInnerList")]
			public List<GoodType> InnerList { get; set; }
			[XmlAttribute("xmlBlob")]
			public byte[] Blob;
		}

		[Test]
		public void ParseGoodType1()
		{
			var root = 
@"<Root Ignored='true'><xmlNInt>123</xmlNInt></Root>".ToXmlView();
			var d = new GenericDeserializer(new XmlMapper());

			var t = d.Deserialize<GoodType>(root);

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
			var d = new GenericDeserializer(new XmlMapper());

			var t = d.Deserialize<GoodType>(root);

			Assert.IsFalse(t.Ignored);
			Assert.IsNotNull(t.Inner);
			Assert.AreEqual(321, t.Inner.NInt);
		}

		[Test]
		public void ParseGoodType_Blob()
		{
			var root =
@"<Root xmlBlob='MTIz'></Root>".ToXmlView();
			var d = new GenericDeserializer(new XmlMapper());

			var t = d.Deserialize<GoodType>(root);

			Assert.That(t.Blob, Is.EquivalentTo(new byte[] {49, 50, 51}));
		}
	}
}
