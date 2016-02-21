using NConfiguration.Combination;
using NConfiguration.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class TypeAttributeTests
	{
		[Test]
		public void DeserializerForClass()
		{
			var root =
@"<Root F1='textF1' F2='123' />".ToXmlView();

			var item = DefaultDeserializer.Instance.Deserialize<TestAttrClass>(root);

			Assert.That(item.F1, Is.EqualTo("textF1attr"));
			Assert.That(item.F2, Is.EqualTo(133));
		}

		[Test]
		public void GenericDeserializerForClass()
		{
			var root =
@"<Root F1='textF1' F2='123' />".ToXmlView();

			var item = DefaultDeserializer.Instance.Deserialize<TestGenericAttrClass>(root);

			Assert.That(item.F1, Is.EqualTo("textF1attr"));
			Assert.That(item.F2, Is.EqualTo(133));
		}
	}
}
