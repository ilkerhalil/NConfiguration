using System;
using NConfiguration.Joining;
using NConfiguration.Xml;
using NUnit.Framework;
using NConfiguration.Serialization;
using NConfiguration.Tests;

namespace NConfiguration
{
	[TestFixture]
	public class XmlSystemSettingsTests
	{
		[Test]
		public void ReadForDefaultName()
		{
			var cfg = new XmlSystemSettings("ExtConfigure").ToAppSettings().First<MyXmlConfig>();
			
			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}

		[Test]
		public void IncludeByRelativePath()
		{
			var loader = new SettingsLoader();
			loader.XmlFileByExtension();
			var systemSettings = new XmlSystemSettings("ExtConfigure");
			var settings = loader.LoadSettings(systemSettings).ToAppSettings();

			var cfg = settings.Get<MyXmlConfig>("MyCfg2");

			Assert.AreEqual("2", cfg.AttrField);
			Assert.IsNull(cfg.ElemField);
		}
	}
}

