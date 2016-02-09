using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Combination
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class CombinerAttribute: Attribute
	{
		public readonly Type CombinerType;

		public CombinerAttribute(Type combinerType)
		{
			if (combinerType == null)
				throw new ArgumentNullException("combinerType");
			CombinerType = combinerType;
		}
	}
}
