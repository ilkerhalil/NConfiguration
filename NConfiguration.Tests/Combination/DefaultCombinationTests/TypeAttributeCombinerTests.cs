using NConfiguration.Combination;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	[TestFixture]
	public class TypeAttributeCombinerTests
	{
		[Test]
		public void SimpleCombinerForClass()
		{
			var x = new TestAttrClass()
			{
				F1 = "xF1",
				F2 = 1
			};

			var y = new TestAttrClass()
			{
				F1 = "yF1",
				F2 = 2
			};

			var combined = DefaultCombiner.Instance.Combine(x, y);

			Assert.That(combined.F1, Is.EqualTo("xF1yF1"));
			Assert.That(combined.F2, Is.EqualTo(3));
		}

		[Test]
		public void GenericCombinerForClass()
		{
			var x = new TestGenericAttrClass()
			{
				F1 = "xF1",
				F2 = 1
			};

			var y = new TestGenericAttrClass()
			{
				F1 = "yF1",
				F2 = 2
			};

			var combined = DefaultCombiner.Instance.Combine(x, y);

			Assert.That(combined.F1, Is.EqualTo("xF1yF1"));
			Assert.That(combined.F2, Is.EqualTo(3));
		}

		[Test]
		public void GenericCombinerForStruct()
		{
			var x = new TestAttrStruct()
			{
				F1 = "xF1",
				F2 = 1
			};

			var y = new TestAttrStruct()
			{
				F1 = "yF1",
				F2 = 2
			};

			var combined = DefaultCombiner.Instance.Combine(x, y);

			Assert.That(combined.F1, Is.EqualTo("xF1yF1"));
			Assert.That(combined.F2, Is.EqualTo(3));

			var ncombined = DefaultCombiner.Instance.Combine<TestAttrStruct?>(x, y);

			Assert.That(ncombined.Value.F1, Is.EqualTo("xF1yF1"));
			Assert.That(ncombined.Value.F2, Is.EqualTo(3));
		}
	}
}
