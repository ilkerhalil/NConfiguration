using NConfiguration.Combination;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	[TestFixture]
	public class ClassAttributeTests
	{
		[Test]
		public void ClassAttribute()
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
	}


}
