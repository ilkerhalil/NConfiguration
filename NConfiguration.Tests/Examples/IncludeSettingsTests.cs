using System;
using System.Linq;
using NConfiguration.Tests;
using NUnit.Framework;
using System.Collections.Specialized;
using System.Configuration;
using NConfiguration.Xml;
using NConfiguration.Joining;
using NConfiguration.Examples;
using NConfiguration.Xml.Protected;
using NConfiguration.Serialization;
using NConfiguration.Json;
using NConfiguration.Combination;

namespace NConfiguration.Examples
{
	[TestFixture]
	public class IncludeSettingsTests
	{
		[Test]
		public void Load()
		{
			var xmlFileLoader = new XmlFileSettingsLoader();

			var loader = new SettingsLoader();
			loader.AddHandler("IncludeXmlFile", xmlFileLoader);

			loader.Loaded += (s,e) => 
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			var settings = loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/main.config"));

			var addCfg = settings.TryGet<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InAppDirectory", addCfg.F);
		}

		[Test]
		public void LoadJson()
		{
			var xmlFileLoader = new XmlFileSettingsLoader();
			var jsonFileLoader = new JsonFileSettingsLoader();

			var loader = new SettingsLoader();
			loader.AddHandler("IncludeXmlFile", xmlFileLoader);
			loader.AddHandler("IncludeJsonFile", jsonFileLoader);

			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			var settings = loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/mainJson.config"));

			var addCfg = settings.TryGet<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InAppDirectory_json", addCfg.F);
		}

		[Test]
		public void AutoCombineLoad()
		{
			var xmlFileLoader = new XmlFileSettingsLoader();

			var loader = new SettingsLoader();
			loader.AddHandler("IncludeXmlFile", xmlFileLoader);
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			var settings = loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/autoMain.config"));

			var cfg = settings.TryGet<ChildAutoCombinableConnectionConfig>();

			Assert.IsNotNull(cfg);
			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;Trusted_Connection=True;Connection Timeout=60", cfg.ConnectionString);
		}

		[Test]
		public void SecureLoad()
		{
			KeyManager.Create();

			var providerLoader = new ProviderLoader();
			var xmlFileLoader = new XmlFileSettingsLoader();

			var loader = new SettingsLoader();
			loader.AddHandler("IncludeXmlFile", xmlFileLoader);
			loader.Loaded += providerLoader.TryExtractConfigProtectedData;
			
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			var settings = loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/secureMain.config"));

			var addCfg = settings.TryGet<ExampleCombineConfig>("AdditionalConfig");

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InUpDirectory", addCfg.F);

			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryGet<ConnectionConfig>("MyExtConnection").ConnectionString);
			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryGet<ConnectionConfig>("MySecuredConnection").ConnectionString);

			KeyManager.Delete();
		}

	}
}

