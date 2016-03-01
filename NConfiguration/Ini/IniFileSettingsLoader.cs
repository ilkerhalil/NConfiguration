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
	public class IniFileSettingsLoader : FileSearcher, IIncludeHandler<IncludeFileConfig>
	{
		public static readonly IniFileSettingsLoader Instance = new IniFileSettingsLoader();

		public IIdentifiedSource LoadFile(string path)
		{
			return new IniFileSettings(path);
		}

		public override IIdentifiedSource CreateFileSetting(string path)
		{
			return new IniFileSettings(path);
		}

		public IEnumerable<IIdentifiedSource> TryLoad(IConfigNodeProvider owner, IncludeFileConfig includeConfig)
		{
			return CreateSettings(owner, includeConfig);
		}
	}
}

