using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;
using System.Xml.Linq;

namespace NConfiguration.GenericView
{
	[TestFixture]
	public class XmlViewTests
	{
		[Test]
		public void GenericNavigate()
		{
			var root = _doc1.ToXmlView();
			Assert.IsNull(root.GetChild("noitem"));

			Assert.IsNotNull(root.GetChild("item1"));
			Assert.AreEqual("item2.value2item3.value1", root.Text);
			Assert.AreEqual("item1.value1", root.GetChild("item1").Text);

			CollectionAssert.AreEqual(
				new[] { "item2.value1", "item2.value2" },
				root.GetCollection("item2").Select(n => n.Text));
		}

		private XDocument _doc1 = @"<?xml version='1.0' encoding='utf-8' ?>
<Config Item1='item1.value1' Item2='item2.value1'>
	<Item2 Item3='item3.value3'>item2.value2</Item2>
	<Item3>item3.value1</Item3>
</Config>".ToXDocument();

		[TestCase(true, "True")]
		[TestCase(true, "true")]
		[TestCase(false, "False")]
		[TestCase(false, "false")]
		[TestCase(true, "t")]
		[TestCase(true, "y")]
		[TestCase(false, "f")]
		[TestCase(false, "n")]
		[TestCase(true, "+")]
		[TestCase(true, "1")]
		[TestCase(false, "-")]
		[TestCase(false, "0")]
		[TestCase(null, "")]
		public void ParseBoolean(object expected, string text)
		{
			var root = string.Format("<Config>{0}</Config>", text).ToXmlView();
			Assert.AreEqual(expected, _converter.Convert<bool?>(root.Text));
		}

		private StringConverter _converter = new StringConverter();
	}
}
