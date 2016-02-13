using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfiguration.GenericView.Deserialization;
using System.Collections.Concurrent;

namespace NConfiguration.Combination
{
	public class ChildCombiner: ICombiner
	{
		private ICombiner _parent;
		private Dictionary<Type, object> _funcMap = new Dictionary<Type, object>();

		public ChildCombiner(ICombiner parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			_parent = parent;
		}

		/// <summary>
		/// Set custom combiner
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="combine">combine function</param>
		public void SetCombiner<T>(Combine<T> combine)
		{
			_funcMap[typeof(T)] = combine;
		}

		/// <summary>
		/// Set custom combiner
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="combiner">combiner</param>
		public void SetCombiner<T>(ICombiner<T> combiner)
		{
			SetCombiner<T>(combiner.Combine);
		}

		public T Combine<T>(ICombiner context, T x, T y)
		{
			object combine;
			if (_funcMap.TryGetValue(typeof(T), out combine))
				return ((Combine<T>)combine)(this, x, y);

			return _parent.Combine(context, x, y);
		}
	}
}
