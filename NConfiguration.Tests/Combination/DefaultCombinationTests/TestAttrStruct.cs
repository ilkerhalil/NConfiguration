using NConfiguration.Combination;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	[Combiner(typeof(TestAttrStruct.Combiner))]
	public struct TestAttrStruct
	{
		public string F1;
		public int F2;

		public class Combiner : ICombiner<TestAttrStruct>
		{
			public TestAttrStruct Combine(ICombiner combiner, TestAttrStruct x, TestAttrStruct y)
			{
				x.F1 += y.F1;
				x.F2 += y.F2;
				return x;
			}
		}
	}
}

