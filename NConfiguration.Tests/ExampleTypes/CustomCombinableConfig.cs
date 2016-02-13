using System;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Combination;

namespace NConfiguration.ExampleTypes
{
	public class CustomCombinableConfig : ICombinable, ICombinable<CustomCombinableConfig>
	{
		[XmlAttribute("Field1")]
		public string Field1 { get; set; }

		public void Combine(ICombiner combiner, CustomCombinableConfig other)
		{
			if (other == null)
				return;

			if (other.Field1 != null)
				Field1 += other.Field1;
		}

		public virtual void Combine(ICombiner combiner, object other)
		{
			Combine(combiner, other as CustomCombinableConfig);
		}
	}
}

