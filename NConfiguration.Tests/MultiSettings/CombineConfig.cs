using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NConfiguration
{
	public class CombineConfig : ICombinable_obsolete
	{
		[XmlAttribute]
		public string F = null;

		public void Combine(object other)
		{
			Combine(other as CombineConfig);
		}

		public void Combine(CombineConfig other)
		{
			if (other == null)
				return;

			F = other.F ?? F;
		}
	}
}

