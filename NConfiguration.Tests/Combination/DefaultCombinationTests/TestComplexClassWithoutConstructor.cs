using NConfiguration.Combination;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	public class TestComplexClassWithoutConstructor
	{
		public TestComplexClassWithoutConstructor(string f1, int f2, string p1, decimal? p2)
		{
			F1 = f1;
			F2 = f2;
			P1 = p1;
			P2 = p2;
		}

		public string F1;
		public int F2;

		public string P1 {get; private set;}
		public decimal? P2  {get; private set;}
	}
}

