using NConfiguration.Combination;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	public class TestComplexClass
	{
		public string F1;
		public int F2;

		public string P1 { get; set; }
		public decimal? P2 { get; set; }
	}
}

