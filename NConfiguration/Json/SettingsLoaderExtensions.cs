using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Joining;
using NConfiguration.Serialization;

namespace NConfiguration.Json
{
	public static class SettingsLoaderExtensions
	{
		public static FileSearcher JsonFileByExtension(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(JsonFileSettings.Create, "js", "json");
			loader.AddHandler<IncludeFileConfig>("IncludeFile", searcher);
			return searcher;
		}

		public static FileSearcher JsonFileBySection(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(JsonFileSettings.Create);
			loader.AddHandler<IncludeFileConfig>("IncludeJsonFile", searcher);
			return searcher;
		}
	}
}

