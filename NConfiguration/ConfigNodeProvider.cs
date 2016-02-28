using NConfiguration.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration
{
	public abstract class ConfigNodeProvider : IConfigNodeProvider
	{
		private Lazy<IReadOnlyList<KeyValuePair<string, ICfgNode>>> _list;
		private Lazy<Dictionary<string, List<ICfgNode>>> _index;

		public ConfigNodeProvider()
		{
			_list = new Lazy<IReadOnlyList<KeyValuePair<string, ICfgNode>>>(() => GetAllNodes().ToList().AsReadOnly());
			_index = new Lazy<Dictionary<string, List<ICfgNode>>>(CreateIndex);
		}

		private Dictionary<string, List<ICfgNode>> CreateIndex()
		{
			var result = new Dictionary<string, List<ICfgNode>>();

			List<ICfgNode> nodes;
			foreach (var section in _list.Value)
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

		protected abstract IEnumerable<KeyValuePair<string, ICfgNode>> GetAllNodes();

		public IReadOnlyList<KeyValuePair<string, Serialization.ICfgNode>> Items
		{
			get
			{
				return _list.Value;
			}
		}

		public IEnumerable<ICfgNode> ByName(string sectionName)
		{
			List<ICfgNode> result;
			if (_index.Value.TryGetValue(sectionName, out result))
				return result;

			return new ICfgNode[0];
		}
	}
}
