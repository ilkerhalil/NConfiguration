using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfiguration.GenericView.Deserialization;
using System.Collections.Concurrent;

namespace NConfiguration.Combination
{
	public delegate T Combine<T>(ICombiner combiner, T x, T y);

	public class DefaultCombiner: ICombiner
	{
		private static readonly ICombiner _instance = new DefaultCombiner();

		public static ICombiner Instance
		{
			get { return _instance; }
		}

		private DefaultCombiner()
		{
		}

		public T Combine<T>(ICombiner context, T x, T y)
		{
			return Cache<T>.CombineLazy.Value(context, x, y);
		}

		private static class Cache<T>
		{
			public static readonly Lazy<Combine<T>> CombineLazy = new Lazy<Combine<T>>(CombineCreater);

			private static Combine<T> CombineCreater()
			{
				return (Combine<T>)BuildUtils.CreateFunction(typeof (T));
			}
		}
	}
}
