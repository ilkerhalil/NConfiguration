using NConfiguration.Combination;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	[TestFixture]
	public class AutoCombinerTests
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

			var combined = DefaultCombiner.Instance.Combine(x, y);

			Assert.That(combined.F1, Is.EqualTo("xF1"));
			Assert.That(combined.F2, Is.EqualTo(2));
			Assert.That(combined.P1, Is.EqualTo("yP1"));
			Assert.That(combined.P2, Is.EqualTo(1));
		}

		[Test]
		public void TestComplexClassWithoutConstructor()
		{
			var x = new TestComplexClassWithoutConstructor("xF1", 1, "xP1", 1);
			var y = new TestComplexClassWithoutConstructor(null, 2, "yP1", null);

			var combined = DefaultCombiner.Instance.Combine(x, y);

			Assert.That(combined.F1, Is.EqualTo("xF1"));
			Assert.That(combined.F2, Is.EqualTo(2));
			Assert.That(combined.P1, Is.EqualTo("yP1"));
			Assert.That(combined.P2, Is.EqualTo(1));
		}


		[Test]
		public void TestMemberAttrClass()
		{
			var x = new TestMemberAttrClass()
			{
				F1 = "xF1",
				F1A = "xF1A",
				P1 = "xP1",
				P1A = "xP1A"
			};

			var y = new TestMemberAttrClass()
			{
				F1 = "yF1",
				F1A = "yF1A",
				P1 = "yP1",
				P1A = "yP1A"
			};

			var combined = DefaultCombiner.Instance.Combine(x, y);

			Assert.That(combined.F1, Is.EqualTo("yF1"));
			Assert.That(combined.F1A, Is.EqualTo("xF1A,yF1A"));
			Assert.That(combined.P1, Is.EqualTo("yP1"));
			Assert.That(combined.P1A, Is.EqualTo("xP1A,yP1A"));
		}
	}
}
