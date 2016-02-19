using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using NConfiguration.Xml;
using NConfiguration.Serialization;
using NConfiguration.Tests;
using System.Collections.Generic;

namespace NConfiguration.Ini
{
	public class IniStringSettings : IniSettings
	{
		private readonly List<Section> _sections;

		public IniStringSettings(string text)
			: base(DefaultDeserializer.Instance)
		{
			_sections = Section.Parse(text);
		}

		protected override IEnumerable<Section> Sections
		{
			get
			{
				return _sections;
			}
		}
	}
}

