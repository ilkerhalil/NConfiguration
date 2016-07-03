using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Collections.Specialized;

namespace NConfiguration.Joining
{
	public class FindingSettingsArgs : EventArgs
	{
		public IConfigNodeProvider Source { get; private set; }
		public IncludeFileConfig IncludeFile { get; private set; }
		public string SearchPath { get; private set; }

		public FindingSettingsArgs(IConfigNodeProvider source, IncludeFileConfig cfg, string searchPath)
		{
			Source = source;
			IncludeFile = cfg;
			SearchPath = searchPath;
		}
	}
}

