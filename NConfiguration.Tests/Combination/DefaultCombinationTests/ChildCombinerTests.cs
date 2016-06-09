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

		[Test]
		public void InnerOverrides()
		{
			var x = new TestClass()
			{
				F1 = "xF1",
				Inner = new TestClass()
				{
					F1 = "xiF1"
				}
			};

			var y = new TestClass()
			{
				F1 = "yF1",
				Inner = new TestClass()
				{
					F1 = "yiF1"
				}
			};

			var childCombiner = new ChildCombiner(DefaultCombiner.Instance);
			childCombiner.SetCombiner<TestClass>((ctx, prev, next) => prev == null ? next : next == null ? prev : new TestClass()
			{
				F1 = prev.F1 + "," + next.F1,
				Inner = ctx.CurrentCombine(prev.Inner, next.Inner)
			});

			var combined = childCombiner.Combine(x, y);

			Assert.That(combined.F1, Is.EqualTo("xF1,yF1"));
			Assert.That(combined.Inner.F1, Is.EqualTo("xiF1,yiF1"));
		}

		[Test]
		public void ModifyCombined()
		{
			var x = new TestClass()
			{
				F1 = "xF1",
				Inner = new TestClass()
				{
					F1 = "xiF1"
				}
			};

			var y = new TestClass()
			{
				F1 = "yF1",
				Inner = new TestClass()
				{
					F1 = "yiF1"
				}
			};

			var childCombiner = new ChildCombiner(DefaultCombiner.Instance);
			childCombiner.SetCombiner<TestClass>((ctx, prev, next) => prev == null ? next : next == null ? prev : new TestClass()
			{
				F1 = prev.F1 + "," + next.F1,
				Inner = ctx.CurrentCombine(prev.Inner, next.Inner)
			});
 
			var childCombiner2 = new ChildCombiner(childCombiner)
				.Modify<TestClass>(_ => _.F1 += " combined");
			
			var combined = childCombiner2.Combine(x, y);

			Assert.That(combined.F1, Is.EqualTo("xF1,yF1 combined"));
			Assert.That(combined.Inner.F1, Is.EqualTo("xiF1,yiF1 combined"));
		}

		public class TestClass
		{
			public string F1;
			public TestClass Inner;
		}
	}
}
