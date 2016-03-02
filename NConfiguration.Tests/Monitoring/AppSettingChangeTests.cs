using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NConfiguration.Xml;
using NConfiguration.Tests;
using NConfiguration.Examples;
using NConfiguration.Joining;
using NConfiguration.Serialization;

namespace NConfiguration.Monitoring
{
	[TestFixture]
	public class AppSettingChangebleTests
	{
		private string _xmlCfgAutoOrigin = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
	<WatchFile Mode=""Auto"" />
	<AdditionalConfig F=""Origin"" />
</configuration>";

		private string _xmlCfgAutoModify = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
	<WatchFile Mode=""Auto"" />
	<AdditionalConfig F=""Modify"" />
</configuration>";

		[Test]
		public void AutoChange()
		{
			string cfgFile = Path.GetTempFileName();
			File.WriteAllText(cfgFile, _xmlCfgAutoOrigin);

			var settings = new XmlFileSettings(cfgFile).AsSingleSettings();
			
			var wait = new ManualResetEvent(false);
			settings.Changed += (a, e) => { wait.Set(); };

			var t = Task.Factory.StartNew(() =>
			{
				File.WriteAllText(cfgFile, _xmlCfgAutoModify);
			}, TaskCreationOptions.LongRunning);

			Task.WaitAll(t);

			Assert.IsTrue(wait.WaitOne(10000), "10 sec elapsed");

			settings = new XmlFileSettings(cfgFile).AsSingleSettings();
			Assert.That(settings.First<ExampleCombineConfig>("AdditionalConfig").F, Is.EqualTo("Modify"));
		}

		private string _xmlCfgMain = @"<?xml version='1.0' encoding='utf-8' ?>
<configuration>
	<WatchFile Mode='Auto' />
	<AdditionalConfig F='InMain'/>
	<IncludeXmlFile Path='{0}' Search='Exact' Include='First' Required='true'/>
</configuration>";

		[Test]
		public void MultiChange()
		{
			string cfgMainFile = Path.GetTempFileName();
			string cfgAdditionalFile = Path.GetTempFileName();


			File.WriteAllText(cfgAdditionalFile, _xmlCfgAutoOrigin);
			File.WriteAllText(cfgMainFile, string.Format(_xmlCfgMain, cfgAdditionalFile));

			var loader = new SettingsLoader();
			loader.XmlFileBySection();
			var settings = loader.LoadSettings(new XmlFileSettings(cfgMainFile));

			var wait = new ManualResetEvent(false);
			settings.Changed += (s, e) => { wait.Set(); };

			var t = Task.Factory.StartNew(() =>
			{
				File.WriteAllText(cfgAdditionalFile, _xmlCfgAutoModify);
			}, TaskCreationOptions.LongRunning);

			Task.WaitAll(t);

			Assert.IsTrue(wait.WaitOne(10000), "10 sec elapsed");
		}
	}
}
