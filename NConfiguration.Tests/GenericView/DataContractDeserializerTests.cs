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
	public class DataContractDeserializerTests
	{
		public class TestType1
		{
			[IgnoreDataMember]
			public bool Ignored;

			[DataMember]
			public int? NInt1;
			[DataMember(IsRequired = true)]
			public int? NInt2;
			[DataMember(IsRequired = false)]
			public int? NInt3;

			[DataMember]
			public int Int1;
			[DataMember(IsRequired = true)]
			public int Int2;
			[DataMember(IsRequired = false)]
			public int Int3;

			[DataMember]
			public ICfgNode CfgNode1 { get; set; }
			[DataMember(IsRequired = true)]
			public ICfgNode CfgNode2 { get; set; }

			[DataMember]
			public TestType2 Inner1 { get; set; }
			[DataMember(IsRequired = true)]
			public TestType2 Inner2 { get; set; }
		}

		public class TestType2
		{
			[DataMember(IsRequired = true)]
			public int? NInt;
		}

		[Test]
		public void BadParsePath()
		{
			try
			{
				var root =
	@"<Root NInt2='1' Int2='1'><CfgNode2 /><Inner1 /></Root>".ToXmlView();

				DefaultDeserializer.Instance.Deserialize<TestType1>(root);
			}
			catch (DeserializeChildException ex)
			{
				var fullPath = string.Join("/", ex.FullPath);
				Assert.That(fullPath, Is.EqualTo("Inner1/NInt"));
				Assert.That(ex.Reason, Is.InstanceOf<FormatException>());
			}
		}

		[Test]
		[ExpectedException(typeof(DeserializeChildException))]
		public void BadParse1()
		{
			var root =
@"<Root></Root>".ToXmlView();

			DefaultDeserializer.Instance.Deserialize<TestType1>(root);
		}

		[Test]
		[ExpectedException(typeof(DeserializeChildException))]
		public void BadParse2()
		{
			var root =
@"<Root NInt2=''></Root>".ToXmlView();

			DefaultDeserializer.Instance.Deserialize<TestType1>(root);
		}

		[Test]
		[ExpectedException(typeof(DeserializeChildException))]
		public void BadParse3()
		{
			var root =
@"<Root NInt2='' Int2=''></Root>".ToXmlView();

			DefaultDeserializer.Instance.Deserialize<TestType1>(root);
		}

		[Test]
		public void RequiredNullable()
		{
			var root =
@"<Root NInt=''></Root>".ToXmlView();

			var t = DefaultDeserializer.Instance.Deserialize<TestType2>(root);

			Assert.AreEqual(null, t.NInt);
		}

		[Test]
		public void EmptyParse()
		{
			var root =
@"<root ignored='true' nint2='' int2='123' cfgNode2='' ><inner2 NInt=''/></root>".ToXmlView();

			var t = DefaultDeserializer.Instance.Deserialize<TestType1>(root);

			Assert.AreEqual(false, t.Ignored);

			Assert.AreEqual(null, t.Inner1);
			Assert.NotNull(t.Inner2);
			Assert.AreEqual(123, t.Int2);
			Assert.AreEqual(null, t.NInt2);
		}
	}
}
