using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using NConfiguration.Xml.Protected;
using NConfiguration.Serialization;
using System.Collections.Generic;
using NConfiguration.Json.Parsing;
using NConfiguration.Monitoring;
using System.Text;

namespace NConfiguration.Json
{
	public class JsonFileSettings : JsonSettings, IFilePathOwner, IIdentifiedSource, IChangeable
	{
		private readonly JObject _obj;
		private readonly FileMonitor _fm;

		public JsonFileSettings(string fileName, IDeserializer deserializer)
			: base(deserializer)
		{
			try
			{
				fileName = System.IO.Path.GetFullPath(fileName);
				var content = File.ReadAllBytes(fileName);

				var val = JValue.Parse(Encoding.UTF8.GetString(content));
				if (val.Type != TokenType.Object)
					throw new FormatException("required json object in content");

				_obj = (JObject)val;

				Identity = this.GetIdentitySource(fileName);
				Path = System.IO.Path.GetDirectoryName(fileName);
				_fm = this.GetMonitoring(fileName, content);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		protected override IEnumerable<JValue> GetValue(string name)
		{
			return _obj.Properties
				.Where(p => NameComparer.Equals(p.Key, name))
				.Select(p => p.Value);
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

