using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NConfiguration.Combination
{
	[TestFixture]
	public class SimplyTests
	{
		[TestCase(0, 0, 0)]
		[TestCase(0, 5, 5)]
		[TestCase(5, 0, 5)]
		[TestCase(5, 8, 8)]
		public void CombineInt(int x, int y, int res)
		{
			Assert.That(DefaultCombiner.Instance.Combine(x, y), Is.EqualTo(res));
		}

		[TestCase("A", "B", "B")]
		[TestCase(null, "B", "B")]
		[TestCase("A", null, "A")]
		[TestCase(null, null, null)]
		public void CombineString(string x, string y, string res)
		{
			Assert.That(DefaultCombiner.Instance.Combine(x, y), Is.EqualTo(res));
		}

		[TestCase(0, 0, 0)]
		[TestCase(0, 5, 5)]
		[TestCase(5, 0, 0)]
		[TestCase(5, 8, 8)]
		[TestCase(null, 5, 5)]
		[TestCase(5, null, 5)]
		[TestCase(null, null, null)]
		public void CombineNInt(int? x, int? y, int? res)
		{
			Assert.That(DefaultCombiner.Instance.Combine(x, y), Is.EqualTo(res));
		}

		[TestCase("A,B", "B", "B")]
		[TestCase("A,B", "", "A,B")]
		[TestCase("", "A,B", "A,B")]
		[TestCase("", "", "")]
		public void CombineArray(string x, string y, string res)
		{
			var xColl = x.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();
			var yColl = y.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToArray();

			var combine = string.Join(",", DefaultCombiner.Instance.Combine(xColl, yColl));

			Assert.That(combine, Is.EqualTo(res));
		}
	}
}
