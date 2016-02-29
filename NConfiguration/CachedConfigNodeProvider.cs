using NConfiguration.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration
{
	public abstract class CachedConfigNodeProvider : BaseConfigNodeProvider
	{
		private Lazy<IReadOnlyList<KeyValuePair<string, ICfgNode>>> _list;
		private Lazy<Dictionary<string, List<ICfgNode>>> _index;

		public CachedConfigNodeProvider()
		{
			_list = new Lazy<IReadOnlyList<KeyValuePair<string, ICfgNode>>>(() => GetAllNodes().ToList().AsReadOnly());
			_index = new Lazy<Dictionary<string, List<ICfgNode>>>(CreateIndex);
		}

		protected abstract IEnumerable<KeyValuePair<string, ICfgNode>> GetAllNodes();

		public override IReadOnlyList<KeyValuePair<string, Serialization.ICfgNode>> Items { get { return _list.Value; } }

		protected override Dictionary<string, List<ICfgNode>> Index { get { return _index.Value; } }
	}
}
