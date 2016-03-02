using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Joining;
using NConfiguration.Serialization;

namespace NConfiguration.Xml
{
	public static class SettingsLoaderExtensions
	{
		public static FileSearcher XmlFileByExtension(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(XmlFileSettings.Create, "xml", "config");
			loader.AddHandler<IncludeFileConfig>("IncludeFile", searcher);
			return searcher;
		}

		public static FileSearcher XmlFileBySection(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(XmlFileSettings.Create);
			loader.AddHandler<IncludeFileConfig>("IncludeXmlFile", searcher);
			return searcher;
		}
	}
}

