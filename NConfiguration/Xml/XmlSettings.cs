using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Xml.Serialization;
using NConfiguration.Xml.Protected;
using System.Security.Cryptography;
using NConfiguration.Serialization;

namespace NConfiguration.Xml
{
	/// <summary>
	/// This settings loaded from a XML document
	/// </summary>
	public abstract class XmlSettings : IXmlEncryptable, IAppSettings
	{
		private static readonly XNamespace cryptDataNS = XNamespace.Get("http://www.w3.org/2001/04/xmlenc#");

		private readonly IDeserializer _deserializer;
		private IProviderCollection _providers = null;

		/// <summary>
		/// XML root element that contains all the configuration section
		/// </summary>
		protected abstract XElement Root { get; }

		/// <summary>
		/// This settings loaded from a XML document
		/// </summary>
		public XmlSettings(IDeserializer deserializer)
		{
			_deserializer = deserializer;
		}

		private XElement Decrypt(XElement el)
		{
			if (el == null)
				return null;

			var attr = el.Attribute("configProtectionProvider");
			if (attr == null)
				return el;

			if (_providers == null)
				throw new InvalidOperationException("protection providers not configured");

			var provider = _providers.Get(attr.Value);
			if (provider == null)
				throw new InvalidOperationException(string.Format("protection provider `{0}' not found", attr.Value));

			var encData = el.Element(cryptDataNS + "EncryptedData");
			if (encData == null)
				throw new FormatException(string.Format("element `EncryptedData' not found in element `{0}'", el.Name));

			var xmlEncData = encData.ToXmlElement();
			XmlElement xmlData;

			try
			{
				xmlData = (XmlElement)provider.Decrypt(xmlEncData);
			}
			catch (SystemException sex)
			{
				throw new CryptographicException(string.Format("can't decrypt the configuration section `{0}'", encData.Name), sex);
			}

			return xmlData.ToXElement();
		}

		/// <summary>
		/// Sets the collection providers to decrypt XML sections
		/// </summary>
		public void SetProviderCollection(IProviderCollection collection)
		{
			_providers = collection;
		}

		/// <summary>
		/// Returns a collection of instances of configurations
		/// </summary>
		/// <typeparam name="T">type of instance of configuration</typeparam>
		/// <param name="name">section name</param>
		public IEnumerable<T> LoadCollection<T>(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			if (Root == null)
				yield break;

			foreach (var at in Root.Attributes().Where(a => NameComparer.Equals(name, a.Name.LocalName)))
				yield return _deserializer.Deserialize<T>(new ViewPlainField(at.Value));


			foreach (var el in Root.Elements().Where(e => NameComparer.Equals(name, e.Name.LocalName)))
				yield return _deserializer.Deserialize<T>(new XmlViewNode(Decrypt(el)));
		}
	}
}

