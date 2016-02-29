using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfiguration.Serialization;
using NConfiguration.Json.Parsing;

namespace NConfiguration.Json
{
	public abstract class JsonSettings : CachedConfigNodeProvider, IAppSettings
	{
		private readonly IDeserializer _deserializer;

		public JsonSettings(IDeserializer deserializer)
		{
			_deserializer = deserializer;
		}

		protected abstract JObject Root { get; }

		/// <summary>
		/// Returns a collection of instances of configurations
		/// </summary>
		/// <typeparam name="T">type of instance of configuration</typeparam>
		/// <param name="name">section name</param>
		public IEnumerable<T> LoadCollection<T>(string name)
		{
			foreach(var val in Root.Properties.Where(p => NameComparer.Equals(p.Key, name)).Select(p => p.Value))
				foreach(var item in ViewObject.FlatArray(val))
					yield return _deserializer.Deserialize<T>(ViewObject.CreateByJsonValue(item));
		}

		protected override IEnumerable<KeyValuePair<string, ICfgNode>> GetAllNodes()
		{
			foreach (var pair in Root.Properties)
				foreach (var item in ViewObject.FlatArray(pair.Value))
					yield return new KeyValuePair<string, ICfgNode>(pair.Key, ViewObject.CreateByJsonValue(item));
		}
	}
}
