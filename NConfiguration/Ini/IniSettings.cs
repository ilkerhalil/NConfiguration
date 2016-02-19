using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfiguration.Serialization;

namespace NConfiguration.Ini
{
	public abstract class IniSettings : IAppSettings
	{
		private readonly IDeserializer _deserializer;

		public IniSettings(IDeserializer deserializer)
		{
			_deserializer = deserializer;
		}

		protected abstract IEnumerable<Section> Sections { get; }

		/// <summary>
		/// Returns a collection of instances of configurations
		/// </summary>
		/// <typeparam name="T">type of instance of configuration</typeparam>
		/// <param name="sectionName">section name</param>
		public IEnumerable<T> LoadCollection<T>(string sectionName)
		{
			foreach (var section in Sections)
			{
				if (NameComparer.Equals(section.Name, sectionName))
					yield return _deserializer.Deserialize<T>(new ViewSection(section));

				if (section.Name == string.Empty)
					foreach(var pair in section.Pairs.Where(p => NameComparer.Equals(p.Key, sectionName)))
						yield return _deserializer.Deserialize<T>(new ViewPlainField(pair.Value));
			}
		}
	}
}
