using NConfiguration.Joining;
using NConfiguration.Xml;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration.Tests.Joining
{
	[TestFixture]
	public class SettingsLoaderTests
	{
		[Test]
		public void IncludeInMiddle()
		{
			var loader = new SettingsLoader();
			loader.XmlFileBySection();

			var settings = loader.LoadSettings(XmlFileSettings.Create("Joining/AppDirectory/main.config"));

			Assert.That(settings.LoadSections<AdditionalConfig>().Select(_ => _.F), Is.EquivalentTo(new[] { "InMainPre", "InAdditional", "InMainPost" }));
		}
	}
}
