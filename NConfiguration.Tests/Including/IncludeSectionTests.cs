using System;
using NConfiguration.Tests;
using NUnit.Framework;
using System.Linq;
using NConfiguration.Xml;
using System.Collections.Generic;
using NConfiguration.Joining;
using NConfiguration.Serialization;

namespace NConfiguration.Including
{
	[TestFixture]
	public class IncludeSectionTests
	{
		private static string _xmlFrm = @"<?xml version='1.0' encoding='utf-8' ?>
<Config>
	{0}
</Config>";

		[TestCase("<Include FinalSearch='true'/>", true)]
		[TestCase("<Include FinalSearch='false'/>", false)]
		[TestCase("<Include />", false)]
		[TestCase("", false)]
		public void FinalSearch(string elText, bool expected)
		{
			var settings = string.Format(_xmlFrm, elText).ToXmlSettings();

			var cfg = settings.TryFirst<IncludeConfig>(true);
			
			Assert.AreEqual(expected, cfg.FinalSearch);
		}

		private class MockFactory : ISettingsFactory
		{
			public List<IncludeFileConfig> Configs = new List<IncludeFileConfig>();

			public string Tag
			{
				get { return "XmlFile"; }
			}

			public IEnumerable<IIdentifiedSource> CreateSettings(IAppSettings source, ICfgNode config)
			{
				Configs.Add(DefaultDeserializer.Instance.Deserialize<IncludeFileConfig>(config));
				yield break;
			}


		}
		
		[Test]
		public void GetFiles()
		{
			var settings = @"<?xml version='1.0' encoding='utf-8' ?>
<Config>
	<Include FinalSearch='true'>
		<XmlFile Path='file1'/>
		<XmlFile Path='file2' Search='Exact' Include='All' Required='false'/>
		<XmlFile Path='file3' Search='All' Include='First' Required='true'/>
	</Include>
</Config>".ToXmlSettings();

			var mock = new MockFactory();

			SettingsLoader loader = new SettingsLoader(mock);
			loader.LoadSettings(settings);

			var files = mock.Configs;

			CollectionAssert.IsNotEmpty(files);
			
			Assert.AreEqual(3, files.Count);
			
			Assert.AreEqual("file1", files[0].Path);
			Assert.AreEqual(SearchMode.All, files[0].Search);
			Assert.AreEqual(IncludeMode.All, files[0].Include);
			Assert.IsFalse(files[0].Required);

			Assert.AreEqual("file2", files[1].Path);
			Assert.AreEqual(SearchMode.Exact, files[1].Search);
			Assert.AreEqual(IncludeMode.All, files[1].Include);
			Assert.IsFalse(files[1].Required);

			Assert.AreEqual("file3", files[2].Path);
			Assert.AreEqual(SearchMode.All, files[2].Search);
			Assert.AreEqual(IncludeMode.First, files[2].Include);
			Assert.IsTrue(files[2].Required);
		}
	}
}

