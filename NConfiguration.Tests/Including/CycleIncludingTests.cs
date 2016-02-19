using System;
using NConfiguration.Tests;
using NUnit.Framework;
using System.Linq;
using NConfiguration.Xml;
using System.Collections.Generic;
using NConfiguration.Joining;
using NConfiguration.Json;
using NConfiguration.Ini;
using System.IO;
using NConfiguration.Serialization;

namespace NConfiguration.Including
{
	[TestFixture]
	public class CycleIncludingTests
	{
		[TestCase("xmlPartial.xml")]
		[TestCase("jsonPartial.json")]
		public void DefaultIdentity(string file)
		{
			var settings = LoadSettings(file);

			Assert.AreEqual("ini", settings.First<OneField>("iniPartialConfig").Field);
			Assert.AreEqual("json", settings.First<OneField>("jsonPartialConfig").Field);
			Assert.AreEqual("xml", settings.First<OneField>("xmlPartialConfig").Field);
		}

		[TestCase("xmlPartial_id1.xml", "json id1", "xml id1")]
		[TestCase("jsonPartial_id1.json", "json id1", "xml id1")]
		[TestCase("xmlPartial_id2.xml", "json id1", "xml id2")]
		[TestCase("jsonPartial_id2.json", "json id2", "xml id1")]
		public void CustomIdentity(string file, string jsonId, string xmlId)
		{
			var settings = LoadSettings(file);

			Assert.AreEqual("ini id1", settings.First<OneField>("iniPartialConfig").Field);
			Assert.AreEqual(jsonId, settings.First<OneField>("jsonPartialConfig").Field);
			Assert.AreEqual(xmlId, settings.First<OneField>("xmlPartialConfig").Field);
		}

		private static IAppSettings LoadSettings(string file)
		{
			var xmlFileLoader = new XmlFileSettingsLoader(DefaultDeserializer.Instance);
			var jsonFileLoader = new JsonFileSettingsLoader(DefaultDeserializer.Instance);
			var iniFileLoader = new IniFileSettingsLoader(DefaultDeserializer.Instance);

			var loader = new SettingsLoader(xmlFileLoader, jsonFileLoader, iniFileLoader);
			loader.Loaded += (s, e) =>
			{
				Console.WriteLine("Loaded: {0} ({1})", e.Settings.GetType(), e.Settings.Identity);
			};

			if (Path.GetExtension(file) == ".xml")
				loader.LoadSettings(xmlFileLoader.LoadFile(Path.Combine("Including", file)));
			else
				loader.LoadSettings(jsonFileLoader.LoadFile(Path.Combine("Including", file)));

			return loader.Settings;
		}

		public class OneField
		{
			public string Field { get; set; }
		}
	}
}

