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
	public class JsonFileSettingsLoader : FileSearcher
	{
		public JsonFileSettingsLoader(IDeserializer deserializer)
			: base(deserializer)
		{
		}

		public IIdentifiedSource LoadFile(string path)
		{
			return new JsonFileSettings(path, Deserializer);
		}

		/// <summary>
		/// name of including configuration
		/// </summary>
		public override string Tag
		{
			get
			{
				return "JsonFile";
			}
		}

		public override IIdentifiedSource CreateFileSetting(string path)
		{
			return new JsonFileSettings(path, Deserializer);
		}
	}
}

