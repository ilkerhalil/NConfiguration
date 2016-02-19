using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NConfiguration.Combination
{
	[TestFixture]
	public class ComplexTests
	{
		[Test]
		public void CombineStruct()
		{
			var prev = new TestStruct()
			{
				PString = "prev", FString1 = "prev", FString2 = "prev", FInt = 1, NInt = 0
			};
			
			var next = new TestStruct()
			{
				PString = "next", FString1 = "next", FString2 = null, FInt = 0, NInt = null
			};

			var result = DefaultCombiner.Instance.Combine(prev, next);
			
			Assert.That(result.PString, Is.EqualTo("next"));
			Assert.That(result.FString1, Is.EqualTo("next"));
			Assert.That(result.FString2, Is.EqualTo("prev"));
			Assert.That(result.FInt, Is.EqualTo(1));
			Assert.That(result.NInt, Is.EqualTo(0));
		}

		[Test]
		public void CombineClass()
		{
			var prev = new TestClass()
			{
				PString = "prev",
				FString1 = "prev",
				FString2 = "prev",
				FInt = 1,
				NInt = 0,
				Array1 = new int[]{1, 2, 3},
				Array2 = new int[]{1, 2, 3}
			};

			var next = new TestClass()
			{
				PString = "next",
				FString1 = "next",
				FString2 = null,
				FInt = 0,
				NInt = null,
				Array1 = null,
				Array2 = new int[]{3, 3, 3}
			};

			var result = DefaultCombiner.Instance.Combine(prev, next);

			Assert.That(result.PString, Is.EqualTo("next"));
			Assert.That(result.FString1, Is.EqualTo("next"));
			Assert.That(result.FString2, Is.EqualTo("prev"));
			Assert.That(result.FInt, Is.EqualTo(1));
			Assert.That(result.NInt, Is.EqualTo(0));
			Assert.That(result.Array1, Is.EquivalentTo(new int[] { 1, 2, 3 }));
			Assert.That(result.Array2, Is.EquivalentTo(new int[] { 3, 3, 3 }));
		}

		[Test]
		public void CombineIgnoreAttr()
		{
			var prev = new TestAttrClass()
			{
				PString = null,
				FString1 = null,
			};

			var next = new TestAttrClass()
			{
				PString = "next",
				FString1 = "next",
			};

			var result = DefaultCombiner.Instance.Combine(prev, next);

			Assert.That(result.PString, Is.EqualTo("next"));
			Assert.That(result.FString1, Is.EqualTo(null));
		}

		[TestCase(true, false, true)]
		[TestCase(false, true, true)]
		[TestCase(false, false, false)]
		public void CombineNullClass(bool prevExist, bool nextExist, bool resultExist)
		{
			var prev = new TestClass()
			{
				PString = "prev"
			};
			if (!prevExist)
				prev = null;

			var next = new TestClass()
			{
				PString = "next"
			};
			if (!nextExist)
				next = null;

			var result = DefaultCombiner.Instance.Combine(prev, next);

			if(resultExist)
			{
				if(prevExist)
					Assert.That(result.PString, Is.EqualTo("prev"));
				if(nextExist)
					Assert.That(result.PString, Is.EqualTo("next"));
			}
			else
				Assert.That(result, Is.Null);
		}

		[TestCase("A", "B", "BA")]
		[TestCase(null, "B", "B")]
		[TestCase("A", null, "A")]
		[TestCase(null, null, null)]
		public void ContainCombinableClass(string x, string y, string res)
		{
			var prev = new CombinableContainer()
			{
				ClassField = x == null ? null : (new CombinableTestClass() { Text = x })
			};

			var next = new CombinableContainer()
			{
				ClassField = y == null ? null : (new CombinableTestClass() { Text = y })
			};

			var field = DefaultCombiner.Instance.Combine(prev, next).ClassField;
			var test = field == null ? null : field.Text;

			Assert.That(test, Is.EqualTo(res));
		}

		[TestCase("A", "B", "BA")]
		[TestCase(null, "B", "B")]
		[TestCase("A", null, "A")]
		[TestCase(null, null, null)]
		public void ContainCombinableStruct(string x, string y, string res)
		{
			var prev = new CombinableContainer()
			{
				StructField = new CombinableTestStruct() { Text = x }
			};

			var next = new CombinableContainer()
			{
				StructField = new CombinableTestStruct() { Text = y }
			};

			var test = DefaultCombiner.Instance.Combine(prev, next).StructField.Text;

			Assert.That(test, Is.EqualTo(res));
		}


		public class CombinableContainer
		{
			public CombinableTestClass ClassField;

			public CombinableTestStruct StructField;
		}
	}
}
