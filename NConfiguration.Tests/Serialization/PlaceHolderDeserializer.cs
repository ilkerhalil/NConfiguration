using NConfiguration.Combination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration.Serialization
{
	public class PlaceHolderDeserializer : IDeserializer<string>
	{
		public string Combine(ICombiner combiner, string x, string y)
		{
			return (x ?? "null") + "," + (y ?? "null");
		}

		public string Deserialize(IDeserializer context, ICfgNode cfgNode)
		{
			return context.Deserialize<string>(cfgNode)
				.Replace("${machineName}", Environment.MachineName)
				.Replace("${userName}", Environment.UserName);
		}
	}
}
