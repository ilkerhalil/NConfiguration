using System;
using Configuration.Xml.Joining;
using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;
using Configuration.Xml;
using Configuration.Joining;
using Configuration.Examples;
using Configuration.Xml.Protected;
using Configuration.GenericView;

namespace Configuration
{
	[TestFixture]
	public class IncludeSettingsTests
	{
		[Test]
		public void Load()
		{
			var loader = new SettingsLoader();
			var xmlFileLoader = new XmlFileSettingsLoader(new XmlViewConverter());

			loader.Including += xmlFileLoader.ResolveXmlFile;
			loader.Loaded += (s,e) => 
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			xmlFileLoader.LoadFile(loader, "Examples/AppDirectory/main.config");

			IAppSettings settings = loader.Settings;

			var addCfg = settings.TryLoad<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InAppDirectory", addCfg.F);
		}

		[Test]
		public void SecureLoad()
		{
			KeyManager.Create();

			var providerLoader = new ProviderLoader();
			var loader = new SettingsLoader();
			var xmlFileLoader = new XmlFileSettingsLoader(new XmlViewConverter());

			loader.Including += xmlFileLoader.ResolveXmlFile;
			loader.Loaded += providerLoader.TryExtractConfigProtectedData;
			
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			xmlFileLoader.LoadFile(loader, "Examples/AppDirectory/secureMain.config");

			IAppSettings settings = loader.Settings;

			var addCfg = settings.TryLoad<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InUpDirectory", addCfg.F);

			var extConn = settings.TryLoad<ConnectionConfig>("MyExtConnection");

			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryLoad<ConnectionConfig>("MyExtConnection").ConnectionString);
			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryLoad<ConnectionConfig>("MySecuredConnection").ConnectionString);

			KeyManager.Delete();
		}

	}
}

