using System;
using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;
using NConfiguration.Xml;
using System.IO;
using NConfiguration.Serialization;
using NConfiguration.Tests;

namespace NConfiguration
{
	[TestFixture]
	public class XmlFileSettingsTests
	{
		private IAppSettings _settings;
		
		[OneTimeSetUp]
		public void SetUp()
		{
			_settings = new XmlFileSettings("testConfig1.xml".ResolveTestPath()).ToAppSettings();
		}
		
		[Test]
		public void ReadForDefaultName()
		{
			var cfg = _settings.First<MyXmlConfig>();
			
			Assert.AreEqual("attr field text", cfg.AttrField);
			Assert.AreEqual("elem field text", cfg.ElemField);
		}
		
		[Test]
		public void SectionNotFound()
		{
			Assert.Throws<SectionNotFoundException>(() => _settings.First<MyXmlConfig>("MyCfg3"));
		}
		
		[Test]
		public void ReadNullSection()
		{
			var cfg = _settings.TryFirst<MyXmlConfig>("MyCfg3");
			Assert.IsNull(cfg);
		}
		
		[Test]
		public void ReadForSpecifiedName()
		{
			var cfg = _settings.First<MyXmlConfig>("MyCfg2");
			
			Assert.AreEqual("2", cfg.AttrField);
		}
	}
}

