using NConfiguration.Combination;
using NConfiguration.Serialization;
using System.Collections.Generic;

namespace NConfiguration
{
	public interface IConfigNodeProvider
	{
		IEnumerable<KeyValuePair<string, ICfgNode>> GetNodes();
	}
}
