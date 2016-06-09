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
			cd.SetDeserializer((context, node) => context.Deserialize<string>(node) + "_1");

			var item = cd.Deserialize<Test>(root);

			Assert.That(item.Text, Is.EqualTo("textF1_1"));
			Assert.That(item.Number, Is.EqualTo(123));
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
		public void SequentialConverters()
		{
			var root =
@"<Root Text='text' Number='123' />".ToXmlView();

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance)
				.Convert<string>(_ => _ + "_S")
				.Convert<Test>(_ =>
				{
					_.Text += "_T";
					return _;
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
@"<Root Text='{$x}_{$y}' Number='1200'>
	<RecursiveInner Text='i_{$x}_{$y}' Number='1300'/>
</Root>
".ToXmlView();

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance);
			cd1.SetDeserializer((context, node) => context.Deserialize<string>(node).Replace("{$x}", "X"));
			cd1.SetDeserializer((context, node) => context.Deserialize<int>(node) + 1);
			cd1.SetDeserializer((context, node) =>
			{
				var result = context.Deserialize<OuterTest>(node);
				result.Text += "_markCh1";
				return result;
			});

			var cd2 = new ChildDeserializer(cd1);
			cd2.SetDeserializer((context, node) => context.Deserialize<string>(node).Replace("{$y}", "Y"));
			cd2.SetDeserializer((context, node) =>
			{
				var result = context.Deserialize<OuterTest>(node);
				result.Text += "_markCh2";
				return result;
			});

			var item = cd2.Deserialize<OuterTest>(root);

			Assert.That(item.Text, Is.EqualTo("X_Y_markCh1_markCh2"));
			Assert.That(item.Number, Is.EqualTo(1201));

			Assert.That(item.RecursiveInner.Text, Is.EqualTo("i_X_Y_markCh1_markCh2"));
			Assert.That(item.RecursiveInner.Number, Is.EqualTo(1301));
		}

		[Test]
		public void SpecialContextOverride()
		{
			var root =
@"<Root Text='text_{$x}_{$y}_{$z}' Number='1'>
	<Inner Text='text_{$x}_{$y}_{$z}' Number='1'/>
	<RecursiveInner Text='text_{$x}_{$y}_{$z}' Number='1'/>
</Root>
".ToXmlView();

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance)
				.Convert<string>(_ => _.Replace("{$x}", "X"))
				.CurrentOverride<Test>(current => new ChildDeserializer(current)
					.Convert<string>(_ => _.Replace("{$y}", "Y"))
					.Convert<int>(_ => _ * 3));

			var cd2 = new ChildDeserializer(cd1)
				.Convert<string>(_ => _.Replace("{$z}", "Z"))
				.Convert<int>(_ => _ * 7);

			var item = cd2.Deserialize<OuterTest>(root);

			Assert.That(item.Text, Is.EqualTo("text_X_{$y}_Z"));
			Assert.That(item.Inner.Text, Is.EqualTo("text_X_Y_Z"));
			Assert.That(item.RecursiveInner.Text, Is.EqualTo("text_X_{$y}_Z"));

			Assert.That(item.Number, Is.EqualTo(7));
			Assert.That(item.Inner.Number, Is.EqualTo(3 * 7));
			Assert.That(item.RecursiveInner.Number, Is.EqualTo(7));
		}

		[Test]
		public void ReplaceSpecifyType()
		{
			var root =
@"<Root Text='text_{$x}_{$z}' Number='100'>
	<Inner Text='text_{$x}_{$z}' Number='100'/>
	<RecursiveInner Text='text_{$x}_{$z}' Number='100'/>
</Root>
".ToXmlView();

			var cdR = new ChildDeserializer(DefaultDeserializer.Instance)
				.Convert<string>(_ => _.Replace("{$x}", "Xr").Replace("{$z}", "Zr"));

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance)
				.Convert<int>(_ => _ + 1)
				.Convert<string>(_ => _.Replace("{$x}", "X"))
				.SetDeserializer((context, node) => cdR.Deserialize<Test>(node));

			var cd2 = new ChildDeserializer(cd1)
				.Convert<string>(_ => _.Replace("{$z}", "Z"));

			var item = cd2.Deserialize<OuterTest>(root);

			Assert.That(item.Text, Is.EqualTo("text_X_Z"));
			Assert.That(item.Inner.Text, Is.EqualTo("text_Xr_Zr"));
			Assert.That(item.RecursiveInner.Text, Is.EqualTo("text_X_Z"));

			Assert.That(item.Number, Is.EqualTo(101));
			Assert.That(item.Inner.Number, Is.EqualTo(100));
			Assert.That(item.RecursiveInner.Number, Is.EqualTo(101));
		}

		[Test]
		public void OverrideContextInSpecifyType()
		{
			var root =
@"<Root Text='text_{$x}_{$z}' Number='100'>
	<Inner Text='text_{$x}_{$z}' Number='100'/>
	<RecursiveInner Text='text_{$x}_{$z}' Number='100'>
		<Inner Text='text_{$x}_{$z}' Number='100'/>
	</RecursiveInner>
</Root>
".ToXmlView();

			var cd1 = new ChildDeserializer(DefaultDeserializer.Instance)
				.Convert<int>(_ => _ + 1)
				.Convert<string>(_ => _.Replace("{$x}", "X"))

				.CurrentOverride<Test>((current) => new ChildDeserializer(current)
					.SetDeserializer((context, node) => DefaultDeserializer.Instance.Deserialize<string>(node).Replace("{$x}", "Xreplaced").Replace("{$z}", "Zreplaced")));

			var cd2 = new ChildDeserializer(cd1)
				.Convert<string>(_ => _.Replace("{$z}", "Z"));

			var item = cd2.Deserialize<OuterTest>(root);

			Assert.That(item.Text, Is.EqualTo("text_X_Z"));
			Assert.That(item.Inner.Text, Is.EqualTo("text_Xreplaced_Zreplaced"));
			Assert.That(item.RecursiveInner.Text, Is.EqualTo("text_X_Z"));
			Assert.That(item.RecursiveInner.Inner.Text, Is.EqualTo("text_Xreplaced_Zreplaced"));

			Assert.That(item.Number, Is.EqualTo(101));
			Assert.That(item.Inner.Number, Is.EqualTo(101));
			Assert.That(item.RecursiveInner.Number, Is.EqualTo(101));
			Assert.That(item.RecursiveInner.Inner.Number, Is.EqualTo(101));
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
