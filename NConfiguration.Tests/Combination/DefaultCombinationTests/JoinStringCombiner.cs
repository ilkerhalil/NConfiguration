using NConfiguration.Combination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	public class JoinStringCombiner : ICombiner<string>
	{
		public string Combine(ICombiner combiner, string x, string y)
		{
			return (x ?? "null") + "," + (y ?? "null");
		}
	}
}
