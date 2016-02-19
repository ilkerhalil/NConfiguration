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
	public class XmlFileSettingsLoader : FileSearcher
	{
		public XmlFileSettingsLoader(IDeserializer deserializer)
			: base(deserializer)
		{
		}

		public IIdentifiedSource LoadFile(string fileName)
		{
			return new XmlFileSettings(fileName, Deserializer);
		}

		/// <summary>
		/// name of including configuration
		/// </summary>
		public override string Tag
		{
			get
			{
				return "XmlFile";
			}
		}

		public override IIdentifiedSource CreateFileSetting(string path)
		{
			return new XmlFileSettings(path, Deserializer);
		}
	}
}

