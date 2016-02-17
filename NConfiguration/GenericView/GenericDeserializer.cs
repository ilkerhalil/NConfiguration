using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfiguration.GenericView.Deserialization;
using System.Collections.Concurrent;

namespace NConfiguration.GenericView
{
	public class GenericDeserializer: IGenericDeserializer
	{
		private StringConverter _converter;
		private IGenericMapper _mapper;
		private readonly Func<Type, object> _creater;
		private ConcurrentDictionary<Type, object> _funcMap = new ConcurrentDictionary<Type, object>();

		public GenericDeserializer()
			: this(new GenericMapper())
		{
		}

		public GenericDeserializer(IGenericMapper mapper)
		{
			if (mapper == null)
				throw new ArgumentNullException("mapper");

			_converter =  new StringConverter();
			_mapper = mapper;
			_creater = CreateFunction;
		}

		/// <summary>
		/// Set custom deserializer
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="conv">deserialize function</param>
		public void SetDeserializer<T>(Func<ICfgNode, T> conv)
		{
			_funcMap[typeof(T)] = conv;
		}

		public T Deserialize<T>(ICfgNode cfgNode)
		{
			//if (GenericMapper.IsPrimitive(typeof(T)))
			//	return _converter.Convert<T>(cfgNode.As<string>());

			return ((Func<ICfgNode, T>)GetFunction(typeof(T)))(cfgNode);
		}

		private object GetFunction(Type type)
		{
			return _funcMap.GetOrAdd(type, _creater);
		}

		private object CreateFunction(Type type)
		{
			return _mapper.CreateFunction(type, this, _converter);
		}
	}
}
