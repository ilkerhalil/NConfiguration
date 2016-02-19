using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using NConfiguration.Xml.Protected;
using NConfiguration.Serialization;
using NConfiguration.Monitoring;

namespace NConfiguration.Xml
{
	/// <summary>
	/// settings loaded from a file
	/// </summary>
	public class XmlFileSettings : XmlSettings, IFilePathOwner, IIdentifiedSource, IChangeable
	{
		private readonly XElement _root;
		private readonly FileMonitor _fm;

		/// <summary>
		/// settings loaded from a file
		/// </summary>
		/// <param name="fileName">file name</param>
		/// <param name="converter"></param>
		/// <param name="deserializer">deserializer</param>
		public XmlFileSettings(string fileName, IDeserializer deserializer)
			: base(deserializer)
		{
			try
			{
				fileName = System.IO.Path.GetFullPath(fileName);
				var content = File.ReadAllBytes(fileName);

				_root = XDocument.Load(new MemoryStream(content)).Root;
				Identity = this.GetIdentitySource(fileName);
				Path = System.IO.Path.GetDirectoryName(fileName);
				_fm = this.GetMonitoring(fileName, content);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		/// <summary>
		/// XML root element that contains all the configuration section
		/// </summary>
		protected override XElement Root
		{
			get
			{
				return _root;
			}
		}

		/// <summary>
		/// source identifier the application settings
		/// </summary>
		public string Identity { get; private set; }

		/// <summary>
		/// Directory containing the configuration file
		/// </summary>
		public string Path { get; private set; }

		/// <summary>
		/// Instance changed.
		/// </summary>
		public event EventHandler Changed
		{
			add
			{
				if (_fm != null)
					_fm.Changed += value;
			}
			remove
			{
				if (_fm != null)
					_fm.Changed -= value;
			}
		}
	}
}

