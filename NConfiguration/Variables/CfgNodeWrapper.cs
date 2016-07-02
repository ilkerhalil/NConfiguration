using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration.Variables
{
	internal class CfgNodeWrapper: CfgNode
	{
		private readonly ICfgNode _wrapped;
		private readonly VariableStorage _variableStorage;

		public CfgNodeWrapper(ICfgNode wrapped, VariableStorage variableStorage)
		{
			_wrapped = wrapped;
			_variableStorage = variableStorage;
		}

		public override string GetNodeText()
		{
			return new ValueRenderer(_wrapped.Text).Render(_variableStorage);
		}

		public override IEnumerable<KeyValuePair<string, ICfgNode>> GetNestedNodes()
		{
			return _wrapped.Nested.Select(
				pair => new KeyValuePair<string, ICfgNode>(pair.Key, new CfgNodeWrapper(pair.Value, _variableStorage)));
		}
	}
}
