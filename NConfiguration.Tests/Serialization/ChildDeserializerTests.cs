using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class ChildDeserializerTests
	{
		[Test]
		public void OneOverride()
		{
			var root =
@"<Root Text='textF1' Number='123' />".ToXmlView();

			var cd = new ChildDeserializer(DefaultDeserializer.Instance);
			cd.SetDeserializer((context, node) => context.Deserialize<int>(node) + 1);

			var item = cd.Deserialize<Test>(root);

			Assert.That(item.Text, Is.EqualTo("textF1"));
			Assert.That(item.Number, Is.EqualTo(124));
		}

		[Test]
		public void MixedOverrides()
		{
			var root =
@"<Root Text='textF1' Number='123' />".ToXmlView();

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance);
			cd1.SetDeserializer((context, node) => context.Deserialize<int>(node) + 1);

			var cd2 = new ChildDeserializer(cd1);
			cd2.SetDeserializer((context, node) => context.Deserialize<string>(node) + "1");

			var item = cd2.Deserialize<Test>(root);

			Assert.That(item.Text, Is.EqualTo("textF11"));
			Assert.That(item.Number, Is.EqualTo(124));
		}

		[Test]
		public void TwoOverrides()
		{
			var root =
@"<Root Text='text' Number='123' />".ToXmlView();

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance);
			cd1.SetDeserializer((context, node) => context.Deserialize<string>(node).Replace("t", "xx"));

			var cd2 = new ChildDeserializer(cd1);
			cd2.SetDeserializer((context, node) => context.Deserialize<string>(node).Replace("xxx", "O"));

			var item = cd2.Deserialize<Test>(root);

			Assert.That(item.Text, Is.EqualTo("xxeO"));
			Assert.That(item.Number, Is.EqualTo(123));
		}

		public class Test
		{
			public string Text;

			public int Number;
		}
	}
}
