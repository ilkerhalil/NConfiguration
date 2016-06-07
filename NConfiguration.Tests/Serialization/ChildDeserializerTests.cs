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
		public void SequentialOverride()
		{
			var root =
@"<Root Text='text' Number='123' />".ToXmlView();

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance);
			cd1.SetDeserializer((context, node) => context.Deserialize<string>(node) + "_S");
			cd1.SetDeserializer((context, node) =>
			{
				var result = context.Deserialize<Test>(node);
				result.Text += "_T";
				return result;
			});


			var item = cd1.Deserialize<Test>(root);

			Assert.That(item.Text, Is.EqualTo("text_S_T"));
		}

		[Test]
		public void TwoOverrides()
		{
			var root =
@"<Root Text='text' Number='123' />".ToXmlView();

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance);
			cd1.SetDeserializer((context, node) => context.Deserialize<string>(node) + "_markS1");

			var cd2 = new ChildDeserializer(cd1);
			cd2.SetDeserializer((context, node) => context.Deserialize<string>(node) + "_markS2");

			var item = cd2.Deserialize<Test>(root);

			Assert.That(item.Text, Is.EqualTo("text_markS1_markS2"));
			Assert.That(item.Number, Is.EqualTo(123));
		}

		[Test]
		public void InnerOverrides()
		{
			var root =
@"<Root Text='text' Number='1200'>
	<Inner Text='itext' Number='1300'/>
</Root>
".ToXmlView();

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance);
			cd1.SetDeserializer((context, node) => context.Deserialize<string>(node) + "_markS1");
			cd1.SetDeserializer((context, node) => context.Deserialize<int>(node) + 1);

			var cd2 = new ChildDeserializer(cd1);
			cd2.SetDeserializer((context, node) => context.Deserialize<string>(node) + "_markS2");

			var item = cd2.Deserialize<OuterTest>(root);

			Assert.That(item.Text, Is.EqualTo("text_markS1_markS2"));
			Assert.That(item.Number, Is.EqualTo(1201));

			Assert.That(item.Inner.Text, Is.EqualTo("itext_markS1_markS2"));
			Assert.That(item.Inner.Number, Is.EqualTo(1301));

		}

		[Test]
		public void InnerRecursiveOverrides()
		{
			var root =
@"<Root Text='text' Number='1200'>
	<RecursiveInner Text='itext' Number='1300'/>
</Root>
".ToXmlView();

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance);
			cd1.SetDeserializer((context, node) => context.Deserialize<string>(node).Replace("t", "xx"));
			cd1.SetDeserializer((context, node) => context.Deserialize<int>(node) + 1);
			cd1.SetDeserializer((context, node) =>
			{
				var result = context.Deserialize<OuterTest>(node);
				result.Text += "_markCh1";
				return result;
			});

			var cd2 = new ChildDeserializer(cd1);
			cd2.SetDeserializer((context, node) => context.Deserialize<string>(node).Replace("xxx", "O"));
			cd2.SetDeserializer((context, node) =>
			{
				var result = context.Deserialize<OuterTest>(node);
				result.Text += "_markCh2";
				return result;
			});

			var item = cd2.Deserialize<OuterTest>(root);

			Assert.That(item.Text, Is.EqualTo("xxeO_markCh1_markCh2"));
			Assert.That(item.Number, Is.EqualTo(1201));

			Assert.That(item.RecursiveInner.Text, Is.EqualTo("ixxeO_markCh1_markCh2"));
			Assert.That(item.RecursiveInner.Number, Is.EqualTo(1301));

		}

		public class Test
		{
			public string Text;

			public int Number;
		}

		public class OuterTest
		{
			public string Text;

			public int Number;

			public Test Inner;

			public OuterTest RecursiveInner;
		}
	}
}
