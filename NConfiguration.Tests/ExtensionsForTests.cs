using System.Xml;
using System.Xml.Linq;
using NConfiguration.Xml.Protected;
using NConfiguration.Xml;
using NConfiguration.Serialization;
using NConfiguration.Tests;
using NConfiguration.Ini;
using System.Collections.Generic;

namespace NConfiguration
{
	public static class ExtensionsForTests
	{
		public static ICfgNode ToXmlView(this XDocument doc)
		{
			return new XmlViewNode(doc.Root);
		}

		public static IIdentifiedSource ToXmlSettings(this string text)
		{
			return new XmlStringSettings(text);
		}

		public static IniStringSettings ToIniSettings(this string text)
		{
			return new IniStringSettings(text);
		}

		public static List<Section> ToIniSections(this string text)
		{
			return Section.Parse(text);
		}

		public static IIdentifiedSource ToXmlSettings(this string text, IProviderCollection providers)
		{
			var settings = new XmlStringSettings(text);
			settings.SetProviderCollection(providers);
			return settings;
		}

		public static XmlElement ToXmlElement(this string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			return doc.DocumentElement;
		}

		public static XDocument ToXDocument(this string xml)
		{
			return XDocument.Parse(xml);
		}

		public static ICfgNode ToXmlView(this string xml)
		{
			return ToXmlView(XDocument.Parse(xml));
		}
	}
}



