using System;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NConfiguration.ExampleTypes
{
	public class CustomCombinableConfig : ICombinable_obsolete
	{
		[XmlAttribute("Field1")]
		public string Field1 { get; set; }

		public void Combine(CustomCombinableConfig other)
		{
			if (other == null)
				return;

			if (other.Field1 != null)
				Field1 += other.Field1;
		}
		
		public virtual void Combine(object other)
		{
			Combine(other as CustomCombinableConfig);
		}
	}
}

