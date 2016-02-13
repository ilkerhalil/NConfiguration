using NConfiguration.Combination;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	[TestFixture]
	public class ChildCombinerTests
	{
		[Test]
		public void TestComplexClass()
		{
			var x = new TestComplexClass()
			{
				F1 = "xF1",
				F2 = 1,
				P1 = "xP1",
				P2 = 1
			};

			var y = new TestComplexClass()
			{
				F1 = null,
				F2 = 2,
				P1 = "yP1",
				P2 = null
			};

			var childCombiner = new ChildCombiner(DefaultCombiner.Instance);
			childCombiner.SetCombiner<string>((ctx, prev, next) => (prev ?? "null") + "," + (next ?? "null"));
			childCombiner.SetCombiner<int>((ctx, prev, next) => prev + next + 10);

			var combined = childCombiner.Combine(x, y);

			Assert.That(combined.F1, Is.EqualTo("xF1,null"));
			Assert.That(combined.F2, Is.EqualTo(13));
			Assert.That(combined.P1, Is.EqualTo("xP1,yP1"));
			Assert.That(combined.P2, Is.EqualTo(1));
		}
	}
}
