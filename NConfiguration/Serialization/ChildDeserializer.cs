using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace NConfiguration.Serialization
{
	public class ChildDeserializer: IDeserializer
	{
		private IDeserializer _parent;
		private Dictionary<Type, object> _funcMap = new Dictionary<Type, object>();

		public ChildDeserializer(IDeserializer parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			_parent = parent;
		}

		/// <summary>
		/// Set custom deserializer
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="deserialize">deserialize function</param>
		public void SetDeserializer<T>(Deserialize<T> deserialize)
		{
			_funcMap[typeof(T)] = deserialize;
		}

		/// <summary>
		/// Set custom deserializer
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="deserializer">deserializer</param>
		public void SetDeserializer<T>(IDeserializer<T> deserializer)
		{
			_funcMap[typeof(T)] = (Deserialize<T>)deserializer.Deserialize;
		}

		public T Deserialize<T>(IDeserializer context, ICfgNode node)
		{
			object deserialize;
			if (_funcMap.TryGetValue(typeof(T), out deserialize))
				return ((Deserialize<T>)deserialize)(this, node);

			return _parent.Deserialize<T>(context, node);
		}
	}
}
