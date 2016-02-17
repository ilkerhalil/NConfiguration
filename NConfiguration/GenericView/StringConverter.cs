using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Collections.Concurrent;
using System.Xml.Linq;
using NConfiguration.GenericView.Deserialization;

namespace NConfiguration.GenericView
{
	/// <summary>
	/// Converter string into a simple values
	/// </summary>
	public class StringConverter
	{
		private readonly IStringMapper _mapper;
		private readonly Func<Type, object> _creater;
		private readonly ConcurrentDictionary<Type, object> _funcMap = new ConcurrentDictionary<Type, object>();

		/// <summary>
		/// Converter string into a simple values
		/// </summary>
		public StringConverter()
			: this(new StringMapper())
		{
		}

		/// <summary>
		/// Converter string into a simple values
		/// </summary>
		/// <param name="mapper">factory to create functions of converters</param>
		public StringConverter(IStringMapper mapper)
		{
			if (mapper == null)
				throw new ArgumentNullException("mapper");
			_mapper = mapper;
			_creater = CreateFunction;
		}

		/// <summary>
		/// Set custom converter
		/// </summary>
		/// <typeparam name="T">required type</typeparam>
		/// <param name="conv">convert function</param>
		public void SetConverter<T>(Func<string, T> conv)
		{
			_funcMap[typeof(T)] = conv;
		}

		/// <summary>
		/// Returns an object of the specified type and whose value is equivalent to the specified object.
		/// </summary>
		/// <typeparam name="T">required type of object</typeparam>
		/// <param name="text">A string that represents the object to convert.</param>
		public T Convert<T>(string val)
		{
			var conv = (Func<string, T>)_funcMap.GetOrAdd(typeof(T), _creater);
			return conv(val);
		}

		private object CreateFunction(Type type)
		{
			return _mapper.CreateFunction(type);
		}
	}
}

