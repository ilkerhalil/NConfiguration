using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfiguration.Serialization;
using NConfiguration.Json.Parsing;

namespace NConfiguration.Json
{
	public abstract class JsonSettings : IAppSettings
	{
		private readonly IDeserializer _deserializer;

		public JsonSettings(IDeserializer deserializer)
		{
			_deserializer = deserializer;
		}

		protected abstract IEnumerable<JValue> GetValue(string name);

		/// <summary>
		/// Returns a collection of instances of configurations
		/// </summary>
		/// <typeparam name="T">type of instance of configuration</typeparam>
		/// <param name="name">section name</param>
		public IEnumerable<T> LoadCollection<T>(string name)
		{
			foreach(var val in GetValue(name))
				foreach(var item in ViewObject.FlatArray(val))
					yield return _deserializer.Deserialize<T>(ViewObject.CreateByJsonValue(item));
		}
	}
}
