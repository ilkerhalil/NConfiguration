using System.Linq;
using NConfiguration.Joining;
using NConfiguration.Tests.Joining;
using NConfiguration.Xml;
using NUnit.Framework;

namespace NConfiguration.Variables
{
	[TestFixture]
	public class RendererTests
	{
		[TestCase("A${empty}B", "AB")]
		[TestCase("${empty}${name2}", "value2")]
		[TestCase("${name1}text${name2}", "value1textvalue2")]
		[TestCase("${name1}${name2}", "value1value2")]
		[TestCase("A${name1}${name2}", "Avalue1value2")]
		[TestCase("A${name1}${name2}C", "Avalue1value2C")]
		[TestCase("A${name1}B${name2}C", "Avalue1Bvalue2C")]
		[TestCase("ABC${name1}DEF", "ABCvalue1DEF")]
		[TestCase("${name1}text", "value1text")]
		[TestCase("text${name1}", "textvalue1")]
		[TestCase("${name1}", "value1")]
		[TestCase("text", "text")]
		[TestCase("", "")]
		[TestCase(null, null)]
		public void Render(string sourceText, string expected)
		{
			var storage = new VariableStorage();
			storage.Set("empty", "");
			storage.Set("name1", "value1");
			storage.Set("name2", "value2");

			var renderedText = new ValueRenderer(sourceText).Render(storage);

			Assert.AreEqual(expected, renderedText);
		}

		[Test]
		public void SettingsLoader()
		{
			var storage = new VariableStorage();
			storage.Set("machineName", "TestServer");

			var loader = new SettingsLoader(storage.CfgNodeConverter);
			
			var xmlCfg = @"<?xml version='1.0' encoding='utf-8' ?>
<configuration>
	<Before Field='${machineName} ${var1}' />
	<variable name='var1' value='value1' />
	<After Field='${machineName} ${var1}' />
</configuration>".ToXmlSettings();

			var settings = loader.LoadSettings(xmlCfg).ToAppSettings();

			Assert.AreEqual("TestServer value1", settings.Get<TestConfig>("Before").Field);
			Assert.AreEqual("TestServer value1", settings.Get<TestConfig>("After").Field);
		}

		public class TestConfig
		{
			public string Field;
		}
	}
}
