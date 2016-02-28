using NConfiguration.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration
{
	public class ConfigNodeProvider2 : IConfigNodeProvider
	{
		private IReadOnlyList<KeyValuePair<string, ICfgNode>> _items;
		private Dictionary<string, List<ICfgNode>> _index;

		public ConfigNodeProvider2(IReadOnlyList<KeyValuePair<string, ICfgNode>> items)
		{
			Items = items;
			_index = CreateIndex();
		}

		private Dictionary<string, List<ICfgNode>> CreateIndex()
		{
			var result = new Dictionary<string, List<ICfgNode>>();

			List<ICfgNode> nodes;
			foreach (var section in Items)
			{
				if (!result.TryGetValue(section.Key, out nodes))
				{
					nodes = new List<ICfgNode>();
					result.Add(section.Key, nodes);
				}
				nodes.Add(section.Value);
			}

			return result;
		}

		public IReadOnlyList<KeyValuePair<string, Serialization.ICfgNode>> Items {get; private set;}

		public IEnumerable<ICfgNode> ByName(string sectionName)
		{
			List<ICfgNode> result;
			if (_index.TryGetValue(sectionName, out result))
				return result;

			return new ICfgNode[0];
		}
	}
}
