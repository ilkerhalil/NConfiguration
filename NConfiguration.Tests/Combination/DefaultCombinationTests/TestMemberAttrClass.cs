using NConfiguration.Combination;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	public class TestMemberAttrClass
	{
		public string F1;

		[Combiner(typeof(JoinStringCombiner))]
		public string F1A;

		public string P1 { get; set; }

		[Combiner(typeof(JoinStringCombiner))]
		public string P1A { get; set; }
	}
}

