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
	public class JsonFileSettingsLoader : FileSearcher, IIncludeHandler<IncludeFileConfig>
	{
		public IIdentifiedSource LoadFile(string path)
		{
			return new JsonFileSettings(path);
		}

		public override IIdentifiedSource CreateFileSetting(string path)
		{
			return new JsonFileSettings(path);
		}

		public IEnumerable<IIdentifiedSource> TryLoad(IConfigNodeProvider owner, IncludeFileConfig includeConfig)
		{
			return CreateSettings(owner, includeConfig);
		}
	}
}

