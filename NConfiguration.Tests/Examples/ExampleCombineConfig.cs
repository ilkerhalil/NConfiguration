using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Combination;

namespace NConfiguration.Examples
{
	public class ExampleCombineConfig : ICombinable, ICombinable<ExampleCombineConfig>
	{
		[XmlAttribute]
		public string F = null;

		public void Combine(ICombiner combiner, object other)
		{
			Combine(combiner, other as ExampleCombineConfig);
		}

		public void Combine(ICombiner combiner, ExampleCombineConfig other)
		{
			if (other == null)
				return;

			F = other.F ?? F;
		}
	}
}

