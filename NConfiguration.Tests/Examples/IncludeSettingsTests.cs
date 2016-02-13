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
using NConfiguration.GenericView;
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
			
			var xmlFileLoader = new XmlFileSettingsLoader(Global.GenericDeserializer, Global.PlainConverter);

			var loader = new SettingsLoader(xmlFileLoader);
			loader.Loaded += (s,e) => 
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/main.config"));

			IAppSettings settings = loader.Settings;

			var addCfg = settings.TryCombine<ExampleCombineConfig>("AdditionalConfig", DefaultCombiner.Instance);

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InAppDirectory", addCfg.F);
		}

		[Test]
		public void LoadJson()
		{
			var xmlFileLoader = new XmlFileSettingsLoader(Global.GenericDeserializer, Global.PlainConverter);
			var jsonFileLoader = new JsonFileSettingsLoader(Global.GenericDeserializer, Global.PlainConverter);

			var loader = new SettingsLoader(xmlFileLoader, jsonFileLoader);
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/mainJson.config"));

			IAppSettings settings = loader.Settings;

			var addCfg = settings.TryCombine<ExampleCombineConfig>("AdditionalConfig", DefaultCombiner.Instance);

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InAppDirectory_json", addCfg.F);
		}

		[Test]
		public void AutoCombineLoad()
		{
			var xmlFileLoader = new XmlFileSettingsLoader(Global.GenericDeserializer, Global.PlainConverter);

			var loader = new SettingsLoader(xmlFileLoader);
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/autoMain.config"));

			var settings = new CombinableAppSettings(loader.Settings);

			var cfg = settings.TryGet<ChildAutoCombinableConnectionConfig>();

			var cfgs = settings.Settings.LoadCollection<ChildAutoCombinableConnectionConfig>().ToArray();

			Assert.IsNotNull(cfg);
			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;Trusted_Connection=True;Connection Timeout=60", cfg.ConnectionString);
		}

		[Test]
		public void SecureLoad()
		{
			KeyManager.Create();

			var providerLoader = new ProviderLoader();
			var xmlFileLoader = new XmlFileSettingsLoader(Global.GenericDeserializer, Global.PlainConverter);

			var loader = new SettingsLoader(xmlFileLoader);
			loader.Loaded += providerLoader.TryExtractConfigProtectedData;
			
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			loader.LoadSettings(xmlFileLoader.LoadFile("Examples/AppDirectory/secureMain.config"));

			IAppSettings settings = loader.Settings;

			var addCfg = settings.TryCombine<ExampleCombineConfig>("AdditionalConfig", DefaultCombiner.Instance);

			Assert.IsNotNull(addCfg);
			Assert.AreEqual("InUpDirectory", addCfg.F);

			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryFirst<ConnectionConfig>("MyExtConnection").ConnectionString);
			Assert.AreEqual("Server=localhost;Database=workDb;User ID=admin;Password=pass;", settings.TryFirst<ConnectionConfig>("MySecuredConnection").ConnectionString);

			KeyManager.Delete();
		}

	}
}

