using NConfiguration.Combination;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	[Combiner(typeof(TestAttrClass.Combiner))]
	public class TestAttrClass
	{
		public string F1;
		public int F2;

		public class Combiner: ICombiner<TestAttrClass>
		{
			public TestAttrClass Combine(ICombiner combiner, TestAttrClass x, TestAttrClass y)
			{
				if(x == null)
					return y;
				if(y == null)
					return x;

				return new TestAttrClass()
				{
					F1 = x.F1 + y.F1,
					F2 = x.F2 + y.F2
				};
			}
		}
	}
}

