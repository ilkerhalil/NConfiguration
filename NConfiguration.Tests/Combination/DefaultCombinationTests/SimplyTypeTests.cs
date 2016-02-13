using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfiguration.Combination;
using NUnit.Framework;

namespace NConfiguration.Tests.Combination.DefaultCombinationTests
{
	[TestFixture]
	public class SimplyTypeTests
	{
		[Test, TestCaseSource("CombineCases")]
		public void Combine(Type t, object x, object y, object result)
		{
			var combined = typeof (ICombiner).GetMethod("Combine")
				.MakeGenericMethod(t)
				.Invoke(DefaultCombiner.Instance, new[] {DefaultCombiner.Instance, x, y });

			Assert.That(combined, Is.EqualTo(result));
		}

		internal static IEnumerable<IEnumerable<object[]>> TypedCombineCases()
		{
			var mi = typeof(SimplyTypeTests).GetMethod("GenericStructCases");
			foreach (var t in NumTypes())
			{
				var numCases = (IEnumerable<object[]>)
					mi.MakeGenericMethod(t).Invoke(null, new[] { Convert.ChangeType(1, t), Convert.ChangeType(2, t) });

				yield return numCases;
			}

			yield return GenericClassCases("one", "two");
			yield return GenericCases<bool?>(true, false);
			yield return BoolCases();
			yield return GenericStructCases(TestEn.One, TestEn.Two);
			yield return
				GenericStructCases(Guid.Parse("925E6C4A-C88A-44C4-B4FE-6CC42579886F"),
					Guid.Parse("825E6C4A-C88A-44C4-B4FE-6CC42579886F"));
			yield return GenericStructCases('A', 'B');
			
			yield return GenericStructCases(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
			yield return GenericStructCases(new DateTime(2016, 1, 9), new DateTime(2016, 1, 10));
			yield return GenericStructCases(
				new DateTimeOffset(new DateTime(2016, 1, 9), TimeSpan.FromHours(1)),
				new DateTimeOffset(new DateTime(2016, 1, 9), TimeSpan.FromHours(2)));
		}

		internal static IEnumerable<object[]> CombineCases()
		{
			return TypedCombineCases().SelectMany(_ => _.ToArray());
		}

		internal static IEnumerable<Type> NumTypes()
		{
			return new[] {typeof (ushort), typeof (uint), typeof (ulong), typeof(short), typeof(int),
				typeof(long), typeof(sbyte), typeof(float), typeof(decimal),
				typeof(double), typeof(byte) };
		}

		public static IEnumerable<object[]> GenericStructCases<T>(T item1, T item2) where T: struct
		{
			yield return new object[] { typeof(T), item1, default(T), item1 };
			yield return new object[] { typeof(T), item1, item2, item2 };
			yield return new object[] { typeof(T), default(T), item2, item2 };
			yield return new object[] { typeof(T), default(T), default(T), default(T) };

			yield return new object[] { typeof(T?), (T?)item1, null, (T?)item1 };
			yield return new object[] { typeof(T?), (T?)item1, (T?)item2, (T?)item2 };
			yield return new object[] { typeof(T?), null, (T?)item2, (T?)item2 };
			yield return new object[] { typeof(T?), null, null, null };
		}

		internal static IEnumerable<object[]> GenericClassCases<T>(T item1, T item2) where T : class
		{
			yield return new object[] { typeof(T), item1, null, item1 };
			yield return new object[] { typeof(T), item1, item2, item2 };
			yield return new object[] { typeof(T), null, item2, item2 };
			yield return new object[] { typeof(T), null, null, null };
		}

		internal static IEnumerable<object[]> GenericCases<T>(T item1, T item2)
		{
			yield return new object[] { typeof(T), item1, default(T), item1 };
			yield return new object[] { typeof(T), item1, item2, item2 };
			yield return new object[] { typeof(T), default(T), item2, item2 };
			yield return new object[] { typeof(T), default(T), default(T), default(T) };
		}

		internal static IEnumerable<object[]> BoolCases()
		{
			yield return new object[] { typeof(bool), true, true, true };
			yield return new object[] { typeof(bool), false, false, false };
			yield return new object[] { typeof(bool), true, false, true };
			yield return new object[] { typeof(bool), false, true, true };
		}

		public enum TestEn
		{
			Default = 0,
			One,
			Two
		}

	}
}
