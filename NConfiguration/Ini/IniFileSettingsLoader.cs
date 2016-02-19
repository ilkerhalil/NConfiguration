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
	public class IniFileSettingsLoader : FileSearcher
	{
		public IniFileSettingsLoader(IDeserializer deserializer)
			: base(deserializer)
		{
		}

		public IIdentifiedSource LoadFile(string path)
		{
			return new IniFileSettings(path, Deserializer);
		}

		/// <summary>
		/// name of including configuration
		/// </summary>
		public override string Tag
		{
			get
			{
				return "IniFile";
			}
		}

		public override IIdentifiedSource CreateFileSetting(string path)
		{
			return new IniFileSettings(path, Deserializer);
		}
	}
}

