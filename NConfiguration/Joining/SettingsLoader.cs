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
	public sealed class SettingsLoader
	{
		public delegate ICfgNode CfgNodeConverter(string name, ICfgNode candidate);

		private delegate IEnumerable<IIdentifiedSource> Include(IConfigNodeProvider target, IDeserializer deserializer, ICfgNode config, string searchPath);
	
		private readonly CfgNodeConverter _cfgNodeConverter;

		private readonly Dictionary<string, List<Include>> _includeHandlers = new Dictionary<string, List<Include>>(NameComparer.Instance);

		public SettingsLoader()
		{
			_cfgNodeConverter = (_, node) => node;
		}

		public SettingsLoader(CfgNodeConverter cfgNodeConverter)
		{
			_cfgNodeConverter = cfgNodeConverter;
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
			handlers.Add((target, deserializer, cfgNode, searchPath) => handler.TryLoad(target, deserializer.Deserialize<T>(cfgNode), searchPath));
		}

		public event EventHandler<LoadedEventArgs> Loaded;

		private void OnLoaded(IIdentifiedSource settings)
		{
			var copy = Loaded;
			if (copy != null)
				copy(this, new LoadedEventArgs(settings));
		}

		public ChangeableConfigNodeProvider LoadSettings(IIdentifiedSource setting)
		{
			return LoadSettings(setting, DefaultDeserializer.Instance);
		}

		public ChangeableConfigNodeProvider LoadSettings(IIdentifiedSource setting, IDeserializer deserializer, string searchPath = null)
		{
			if(searchPath == null)
			{
				var rpo = setting as IFilePathOwner;
				if (rpo != null)
					searchPath = rpo.Path;
			}

			var context = new Context(deserializer, searchPath);

			context.FirstChange.Observe(setting as IChangeable);
			OnLoaded(setting);
			context.CheckLoaded(setting);

			return new ChangeableConfigNodeProvider(ScanInclude(setting, context), context.FirstChange);
		}

		private IEnumerable<KeyValuePair<string, ICfgNode>> ScanInclude(IConfigNodeProvider source, Context context)
		{
			foreach(var pair in source.Items)
			{
				var configName = pair.Key;

				if (NameComparer.Equals(configName, AppSettingExtensions.IdentitySectionName) ||
					NameComparer.Equals(configName, FileMonitor.ConfigSectionName))
					continue;

				var configNode = _cfgNodeConverter(configName, pair.Value);
				if (configNode == null)
					continue;

				List<Include> hadlers;
				if (!_includeHandlers.TryGetValue(configName, out hadlers))
				{
					yield return new KeyValuePair<string, ICfgNode>(configName, configNode);
					continue;
				}

				var includeSettings = hadlers
					.Select(_ => _(source, context.Deserializer, configNode, context.SearchPath))
					.FirstOrDefault(_ => _ != null);

				if (includeSettings == null)
					throw new NotSupportedException("any registered handlers returned null");

				var includeSettingsArray = includeSettings.ToArray();
				var includeRequired = context.Deserializer.Deserialize<RequiredContainConfig>(configNode).Required;

				if(includeRequired && includeSettingsArray.Length == 0)
					throw new ApplicationException(string.Format("include setting from section '{0}' not found", configName));

				foreach (var cnProvider in includeSettingsArray)
				{
					if (context.CheckLoaded(cnProvider))
						continue;

					OnLoaded(cnProvider);
					context.FirstChange.Observe(cnProvider as IChangeable);

					foreach (var includePair in ScanInclude(cnProvider, context))
						yield return includePair;
				}
			}
		}

		internal class RequiredContainConfig
		{
			[DataMember(Name = "Required", IsRequired = false)]
			public bool Required { get; set; }
		}

		private class Context
		{
			public readonly string SearchPath;
			public readonly IDeserializer Deserializer;
			public readonly FirstChange FirstChange = new FirstChange();

			private readonly HashSet<IdentityKey> _loaded = new HashSet<IdentityKey>();

			public Context(IDeserializer deserializer, string searchPath)
			{
				Deserializer = deserializer;
				SearchPath = searchPath;
			}

			public bool CheckLoaded(IIdentifiedSource settings)
			{
				var key = new IdentityKey(settings.GetType(), settings.Identity);
				return !_loaded.Add(key);
			}

			private class IdentityKey : IEquatable<IdentityKey>
			{
				private readonly Type _type;
				private readonly string _id;

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
}

