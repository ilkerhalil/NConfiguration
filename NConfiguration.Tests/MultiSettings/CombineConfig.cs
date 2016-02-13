using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Combination;

namespace NConfiguration
{
	public class CombineConfig : ICombinable, ICombinable<CombineConfig>
	{
		[XmlAttribute]
		public string F = null;

		public void Combine(ICombiner combiner, object other)
		{
			Combine(combiner, other as CombineConfig);
		}

		public void Combine(ICombiner combiner, CombineConfig other)
		{
			if (other == null)
				return;

			F = other.F ?? F;
		}
	}
}

