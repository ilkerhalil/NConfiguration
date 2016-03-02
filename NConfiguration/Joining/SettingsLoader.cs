using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Serialization;
using NConfiguration.Combination;
using NConfiguration.Monitoring;
using System.Runtime.Serialization;

namespace NConfiguration.Joining
{
	public class SettingsLoader
	{
		private delegate IEnumerable<IIdentifiedSource> Include(IConfigNodeProvider target, ICfgNode config);

		private readonly Dictionary<string, List<Include>> _includeHandlers = new Dictionary<string, List<Include>>(NameComparer.Instance);
		private readonly IDeserializer _deserializer;

		private HashSet<IdentityKey> _loaded = new HashSet<IdentityKey>();
		private MultiSettings _result;

		public SettingsLoader()
			: this(DefaultDeserializer.Instance)
		{
		}

		public SettingsLoader(IDeserializer deserializer)
		{
			_deserializer = deserializer;
		}

		public void AddHandler<T>(IIncludeHandler<T> handler)
		{
			AddHandler(typeof(T).GetSectionName(), handler);
		}

		public void AddHandler<T>(string sectionName, IIncludeHandler<T> handler)
		{
			List<Include> handlers;
			if (!_includeHandlers.TryGetValue(sectionName, out handlers))
			{
				handlers = new List<Include>();
				_includeHandlers.Add(sectionName, handlers);
			}
			handlers.Add((target, cfgNode) => handler.TryLoad(target, _deserializer.Deserialize<T>(cfgNode)));
		}

		public event EventHandler<LoadedEventArgs> Loaded;

		private void OnLoaded(IIdentifiedSource settings)
		{
			var copy = Loaded;
			if (copy != null)
				copy(this, new LoadedEventArgs(settings));
		}

		public MultiSettings LoadSettings(IIdentifiedSource setting)
		{
			_result = new MultiSettings();
			_result.Deserializer = _deserializer;
			_result.Combiner = DefaultCombiner.Instance;

			_loaded = new HashSet<IdentityKey>();

			CheckLoaded(setting);
			OnLoaded(setting);
			_result.Observe(setting as IChangeable);

			_result.Nodes = new DefaultConfigNodeProvider(ScanInclude(setting).ToList());

			return _result;
		}

		private IEnumerable<KeyValuePair<string, ICfgNode>> ScanInclude(IIdentifiedSource source)
		{
			foreach(var pair in source.Items)
			{
				if (NameComparer.Equals(pair.Key, AppSettingExtensions.IdentitySectionName) ||
					NameComparer.Equals(pair.Key, FileMonitor.ConfigSectionName))
					continue;

				List<Include> hadlers;
				if (!_includeHandlers.TryGetValue(pair.Key, out hadlers))
				{
					yield return pair;
					continue;
				}

				var includeSettings = hadlers
					.Select(_ => _(source, pair.Value))
					.FirstOrDefault(_ => _ != null);

				if (includeSettings == null)
					throw new NotSupportedException("any registered handlers returned null");

				var includeSettingsArray = includeSettings.ToArray();
				var includeRequired = DefaultDeserializer.Instance.Deserialize<RequiredContainConfig>(pair.Value).Required;

				if(includeRequired && includeSettingsArray.Length == 0)
					throw new ApplicationException(string.Format("include setting from section '{0}' not found", pair.Key));

				foreach (var cnProvider in includeSettingsArray)
				{
					if (CheckLoaded(cnProvider))
						continue;

					OnLoaded(cnProvider);
					_result.Observe(cnProvider as IChangeable);

					foreach(var includePair in ScanInclude(cnProvider))
						yield return includePair;
				}
			}
		}

		internal class RequiredContainConfig
		{
			[DataMember(Name = "Required", IsRequired = false)]
			public bool Required { get; set; }
		}

		private bool CheckLoaded(IIdentifiedSource settings)
		{
			var key = new IdentityKey(settings.GetType(), settings.Identity);
			return !_loaded.Add(key);
		}

		private class IdentityKey
		{
			private Type _type;
			private string _id;

			public IdentityKey(Type type, string id)
			{
				_type = type;
				_id = id;
			}

			public bool Equals(IdentityKey other)
			{
				if (other == null)
					return false;

				if (_type != other._type)
					return false;

				return string.Equals(_id, other._id, StringComparison.InvariantCulture);
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as IdentityKey);
			}

			public override int GetHashCode()
			{
				return _type.GetHashCode() ^ _id.GetHashCode();
			}
		}

	}
}

