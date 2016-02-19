using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using NConfiguration.Xml.Protected;
using NConfiguration.Serialization;
using System.Collections.Generic;
using NConfiguration.Ini.Parsing;
using NConfiguration.Monitoring;
using System.Text;

namespace NConfiguration.Ini
{
	public class IniFileSettings : IniSettings, IFilePathOwner, IIdentifiedSource, IChangeable
	{
		private readonly List<Section> _sections;
		private readonly FileMonitor _fm;

		public IniFileSettings(string fileName, IDeserializer deserializer)
			: base(deserializer)
		{
			try
			{
				fileName = System.IO.Path.GetFullPath(fileName);
				var content = File.ReadAllBytes(fileName);
				
				var context = new ParseContext();
				context.ParseSource(Encoding.UTF8.GetString(content));
				_sections = new List<Section>(context.Sections);

				Identity = this.GetIdentitySource(fileName);
				Path = System.IO.Path.GetDirectoryName(fileName);
				_fm = this.GetMonitoring(fileName, content);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		protected override IEnumerable<Section> Sections
		{
			get
			{
				return _sections;
			}
		}

		/// <summary>
		/// source identifier the application settings
		/// </summary>
		public string Identity {get; private set;}

		/// <summary>
		/// Directory containing the configuration file
		/// </summary>
		public string Path {get; private set;}

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

