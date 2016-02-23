using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Serialization
{
	internal class NameComparer // UNDONE move to NConfiguration ns
	{
		public static readonly IEqualityComparer<string> Instance = StringComparer.OrdinalIgnoreCase;

		public static bool Equals(string x, string y)
		{
			return Instance.Equals(x, y);
		}
	}
}
