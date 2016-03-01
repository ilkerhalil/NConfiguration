using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Joining;
using NConfiguration.Serialization;

namespace NConfiguration.Ini
{
	public static class SettingsLoaderExtensions
	{
		public static SettingsLoader IncludeIniFiles(this SettingsLoader loader)
		{
			loader.AddHandler<IncludeFileConfig>("IncludeIniFile", IniFileSettingsLoader.Instance);

			return loader;
		}
	}
}

