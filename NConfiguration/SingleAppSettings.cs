using NConfiguration.Combination;
using NConfiguration.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace NConfiguration
{
	public class SingleAppSettings : IAppSettings_new
	{
		private Dictionary<string, List<ICfgNode>> _sections = new Dictionary<string, List<ICfgNode>>(NameComparer.Instance);

		public SingleAppSettings(IEnumerable<KeyValuePair<string, ICfgNode>> sections)
			:this(sections, DefaultDeserializer.Instance, DefaultCombiner.Instance)
		{
		}

		public SingleAppSettings(IEnumerable<KeyValuePair<string, ICfgNode>> sections, IDeserializer deserializer, ICombiner combiner)
		{
			Deserializer = deserializer;
			Combiner = combiner;

			List<ICfgNode> nodes;
			foreach(var section in sections)
			{
				if(!_sections.TryGetValue(section.Key, out nodes))
				{
					nodes = new List<ICfgNode>();
					_sections.Add(section.Key, nodes);
				}
				nodes.Add(section.Value);
			}
		}

		public IEnumerable<ICfgNode> GetSection(string sectionName)
		{
			List<ICfgNode> nodes;
			if (_sections.TryGetValue(sectionName, out nodes))
				return nodes;

			return Enumerable.Empty<ICfgNode>();
		}

		public IDeserializer Deserializer { get; private set; }

		public ICombiner Combiner { get; private set; }
	}
}
