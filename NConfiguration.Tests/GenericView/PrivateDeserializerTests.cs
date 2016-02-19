using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class PrivateDeserializerTests
	{
		public class NoInjectType
		{
			public string PubText;
			private string PrivProp { get; set; }
			private string PrivField;

			public string getPrivProp { get { return PrivProp; } }
			public string getPrivField { get { return PrivField; } }
		}

		public class PrivateCtorType
		{
			private PrivateCtorType()
			{
			}

			public string PubText;
		}

		public class InjectPropertyType
		{
			public string PubText;
			[DataMember]
			private string PrivProp { get; set; }
			private string PrivField;

			public string getPrivProp { get { return PrivProp; } }
			public string getPrivField { get { return PrivField; } }
		}

		public class InjectFieldType
		{
			public string PubText;
			private string PrivProp { get; set; }
			[DataMember]
			private string PrivField;

			public string getPrivProp { get { return PrivProp; } }
			public string getPrivField { get { return PrivField; } }
		}

		public class InjectAsSerializationType
		{
			public string PubText;

			[DataMember]
			private string PrivField;

			[XmlElement]
			private string PrivProp { get; set; }

			public string getPrivProp { get { return PrivProp; } }
			public string getPrivField { get { return PrivField; } }
		}

		ICfgNode _root = @"<Config
	PubText='PubText'
	PrivField='PrivField'
	PrivProp='PrivProp' />".ToXmlView();

		[Test]
		public void NoInject()
		{
			var tc = DefaultDeserializer.Instance.Deserialize<NoInjectType>(_root);

			Assert.AreEqual("PubText", tc.PubText);
			Assert.AreEqual(null, tc.getPrivField);
			Assert.AreEqual(null, tc.getPrivProp);
		}

		[Test]
		public void InjectField()
		{
			var tc = DefaultDeserializer.Instance.Deserialize<InjectFieldType>(_root);

			Assert.AreEqual("PubText", tc.PubText);
			Assert.AreEqual("PrivField", tc.getPrivField);
			Assert.AreEqual(null, tc.getPrivProp);
		}

		[Test]
		public void InjectProperty()
		{
			var tc = DefaultDeserializer.Instance.Deserialize<InjectPropertyType>(_root);

			Assert.AreEqual("PubText", tc.PubText);
			Assert.AreEqual(null, tc.getPrivField);
			Assert.AreEqual("PrivProp", tc.getPrivProp);
		}

		[Test]
		public void InjectAsSerialization()
		{
			var tc = DefaultDeserializer.Instance.Deserialize<InjectAsSerializationType>(_root);

			Assert.AreEqual("PubText", tc.PubText);
			Assert.AreEqual("PrivField", tc.getPrivField);
			Assert.AreEqual(null, tc.getPrivProp);
		}

		[Test]
		public void PrivateCtor()
		{
			var tc = DefaultDeserializer.Instance.Deserialize<PrivateCtorType>(_root);

			Assert.AreEqual("PubText", tc.PubText);
		}
	}
}
