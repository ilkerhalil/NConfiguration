using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Serialization;

namespace NConfiguration.Joining
{
	public class SettingsLoader
	{
		private MultiSettings _settings;
		private Dictionary<string, ISettingsFactory> _tagMap = new Dictionary<string, ISettingsFactory>(NameComparer.Instance);
		private HashSet<IdentityKey> _loaded = new HashSet<IdentityKey>();

		public SettingsLoader(params ISettingsFactory[] factories)
			: this(new MultiSettings(), factories)
		{
		}

		public SettingsLoader(MultiSettings settings, params ISettingsFactory[] factories)
		{
			_settings = settings;
			AddFactory(factories);
		}

		public MultiSettings Settings
		{
			get { return _settings; }
		}

		public void AddFactory(ISettingsFactory factory)
		{
			_tagMap[factory.Tag] = factory;
		}

		public void AddFactory(string name, ISettingsFactory factory)
		{
			_tagMap[name] = factory;
		}
	
		public void AddFactory(params ISettingsFactory[] factories)
		{
			foreach(var f in factories)
				_tagMap[f.Tag] = f;
		}

		public event EventHandler<LoadedEventArgs> Loaded;

		private void OnLoaded(IIdentifiedSource settings)
		{
			var copy = Loaded;
			if (copy != null)
				copy(this, new LoadedEventArgs(settings));
		}

		public SettingsLoader LoadSettings(IIdentifiedSource setting)
		{
			if (CheckLoaded(setting))
				return this;
			
			_settings.Add(setting);
			OnLoaded(setting);
			IncludeSettings(setting);
			return this;
		}

		private void IncludeSettings(IIdentifiedSource source)
		{
			var includeRoot = source.TryFirst<ICfgNode>("Include", false);
			if(includeRoot == null)
				return;

			foreach (var incNode in includeRoot.Nested)
			{
				if (NameComparer.Equals(incNode.Key, "FinalSearch"))
					continue;

				ISettingsFactory factory;
				if(!_tagMap.TryGetValue(incNode.Key, out factory))
					throw new InvalidOperationException(string.Format("unknown include type '{0}'", incNode.Key));

				foreach (var incSetting in factory.CreateSettings(source, incNode.Value))
					LoadSettings(incSetting);
			}
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

