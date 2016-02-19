using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using NConfiguration.Xml;
using NConfiguration.Serialization;
using NConfiguration.Tests;

namespace NConfiguration
{
	public class XmlStringSettings : XmlSettings, IIdentifiedSource
	{
		private readonly XElement _root;
		private readonly string _hash;

		public XmlStringSettings(string text)
			: base(DefaultDeserializer.Instance)
		{
			_hash = text.GetHashCode().ToString();
			_root = XDocument.Parse(text).Root;
		}

		protected override XElement Root
		{
			get
			{
				return _root;
			}
		}

		public string Identity
		{
			get
			{
				return _hash;
			}
		}
	}
}

