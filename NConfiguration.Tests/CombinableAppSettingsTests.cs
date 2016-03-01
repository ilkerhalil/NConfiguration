using System;
using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;
using NConfiguration.Xml;
using System.IO;
using NConfiguration.Serialization;
using NConfiguration.Tests;
using NConfiguration.Examples;
using NConfiguration.ExampleTypes;

namespace NConfiguration
{
	[TestFixture]
	public class CombinableAppSettingsTests
	{
		[Test]
		public void ImplementInterfaceCombiner()
		{
			var settings = @"<Config>
<Combinable Field1='val1' />
<Combinable Field1='val2' />
<Combinable Field1='val3' />
</Config>".ToXmlSettings().AsSingleSettings();

			var connCfg = settings.Get<CustomCombinableConfig>("Combinable");

			Assert.That(connCfg.Field1, Is.EqualTo("val1val2val3"));
		}

		[Test]
		public void DefaultCombiner()
		{
			var settings = @"<Config>
<Combinable Field1='val1' />
<Combinable Field1='val2' />
<Combinable Field1='val3' />
</Config>".ToXmlSettings().AsSingleSettings();


			var connCfg = settings.Get<CombinableConfig>("Combinable");

			Assert.That(connCfg.Field1, Is.EqualTo("val3"));
		}
	}
}

