using NConfiguration.Combination;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	[Combiner(typeof(TestGenericAttrClass.Combiner<>))]
	public class TestGenericAttrClass
	{
		public string F1;
		public int F2;

		public class Combiner<T> : ICombiner<T> where T : TestGenericAttrClass, new()
		{
			public T Combine(ICombiner combiner, T x, T y)
			{
				if(x == null)
					return y;
				if(y == null)
					return x;

				return new T()
				{
					F1 = x.F1 + y.F1,
					F2 = x.F2 + y.F2
				};
			}
		}
	}
}

