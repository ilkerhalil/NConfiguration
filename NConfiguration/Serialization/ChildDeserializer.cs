using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace NConfiguration.Serialization
{
	public delegate T ChildDeserializeDefinition<T>(ChildDeserializer.Context context, ICfgNode node);

	public sealed class ChildDeserializer: IDeserializer
	{
		private readonly IDeserializer _parent;
		private readonly Dictionary<Type, object> _funcMap = new Dictionary<Type, object>();

		public ChildDeserializer(IDeserializer parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			_parent = parent;
		}

		public class Context
		{
			public IDeserializer Parent { get; private set; }
			public IDeserializer Current { get; private set; }

			internal Context(IDeserializer parent, IDeserializer current)
			{
				Parent = parent;
				Current = current;
			}

			public T Deserialize<T>(ICfgNode node)
			{
				return Parent.Deserialize<T>(Current, node);
			}

			public T Deserialize<T>(IDeserializer current, ICfgNode node)
			{
				return Parent.Deserialize<T>(current, node);
			}
		}

		/// <summary>
		/// Set custom deserializer
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="deserialize">deserialize function</param>
		public void SetDeserializer<T>(ChildDeserializeDefinition<T> deserialize)
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
			ChildDeserializeDefinition<T> func = (context, node) => deserializer.Deserialize(context.Current, node);
			_funcMap[typeof(T)] = func;
		}

		public T Deserialize<T>(IDeserializer context, ICfgNode node)
		{
			object deserialize;
			if (_funcMap.TryGetValue(typeof (T), out deserialize))
			{
				return ((ChildDeserializeDefinition<T>)deserialize)(new Context(_parent, context), node);
			}
			
			return _parent.Deserialize<T>(context, node);
		}
	}
}
