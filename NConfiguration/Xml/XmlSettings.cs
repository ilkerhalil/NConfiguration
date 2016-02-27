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
	public abstract class XmlSettings : IXmlEncryptable, IConfigNodeProvider
	{
		private readonly IDeserializer _deserializer;

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

		public IProviderCollection Providers { get; set; }

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
				yield return _deserializer.Deserialize<T>(new XmlViewNode(this, el));
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes()
		{
			if (Root == null)
				yield break;

			foreach (var at in Root.Attributes())
				yield return new KeyValuePair<string, ICfgNode>(at.Name.LocalName, new ViewPlainField(at.Value));

			foreach (var el in Root.Elements())
				yield return new KeyValuePair<string, ICfgNode>(el.Name.LocalName, new XmlViewNode(this, el));
		}
	}
}

