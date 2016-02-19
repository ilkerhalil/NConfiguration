using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Serialization
{
	public delegate T Deserialize<T>(IDeserializer context, ICfgNode node);

	public class DefaultDeserializer: IDeserializer
	{
		private static readonly IDeserializer _instance = new DefaultDeserializer();

		public static IDeserializer Instance
		{
			get { return _instance; }
		}

		private DefaultDeserializer()
		{
		}

		public T Deserialize<T>(IDeserializer context, ICfgNode node)
		{
			return Cache<T>.DeserializeLazy.Value(context, node);
		}

		private static class Cache<T>
		{
			public static readonly Lazy<Deserialize<T>> DeserializeLazy = new Lazy<Deserialize<T>>(DeserializeCreater);

			private static Deserialize<T> DeserializeCreater()
			{
				return (Deserialize<T>)BuildUtils.CreateFunction(typeof(T));
			}
		}
	}
}
