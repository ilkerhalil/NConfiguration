using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using NConfiguration.Joining;
using NConfiguration.Serialization;

namespace NConfiguration.Joining
{
	public abstract class FileSearcher : ISettingsFactory
	{
		protected FileSearcher(IDeserializer deserializer)
		{
			Deserializer = deserializer;
		}

		public IDeserializer Deserializer { get; private set; }

		/// <summary>
		/// name of including configuration
		/// </summary>
		public abstract string Tag { get; }

		public abstract IIdentifiedSource CreateFileSetting(string path);

		public event EventHandler<FindingSettingsArgs> FindingSettings;

		private void OnFindingSettings(IAppSettings source, IncludeFileConfig cfg)
		{
			var copy = FindingSettings;
			if (copy != null)
				copy(this, new FindingSettingsArgs(source, cfg));
		}

		/// <summary>
		/// creates a collection of includes configuration
		/// </summary>
		/// <param name="source">parent settings</param>
		/// <param name="config">include configuration node</param>
		/// <returns>collection of includes configuration</returns>
		public IEnumerable<IIdentifiedSource> CreateSettings(IAppSettings source, ICfgNode config)
		{
			var rpo = source as IFilePathOwner;
			var cfg = Deserializer.Deserialize<IncludeFileConfig>(config);

			OnFindingSettings(source, cfg);

			if (Path.IsPathRooted(cfg.Path))
			{
				if (!File.Exists(cfg.Path) && !cfg.Required)
					yield break;

				yield return CreateFileSetting(cfg.Path);
				yield break;
			}

			// relative path
			if (rpo == null)
				throw new InvalidOperationException("can not be searched for a relative path because the settings do not provide an absolute path");

			var found = SearchSettings(rpo.Path, cfg.Path, cfg.Search);

			if (found.Count == 0)
			{
				if (cfg.Required)
					throw new ApplicationException(string.Format("configuration file '{0}' not found in '{1}'", cfg.Path, rpo.Path));

				yield break;
			}

			if (cfg.Include == IncludeMode.First)
				yield return found.First();
			else if (cfg.Include == IncludeMode.Last)
				yield return found.Last();
			else
				foreach (var item in found)
					yield return item;
		}

		private List<IIdentifiedSource> SearchSettings(string basePath, string fileName, SearchMode mode)
		{
			var result = new List<IIdentifiedSource>();

			if(basePath == null)
				return result;

			if (mode == SearchMode.Up)
				basePath = Path.GetDirectoryName(basePath);

			if(basePath == null)
				return result;

			while(true)
			{
				try
				{
					if (!Directory.Exists(basePath))
						break;
					var fullPath = Path.Combine(basePath, fileName);

					if (File.Exists(fullPath))
					{
						var item = CreateFileSetting(fullPath);
						result.Add(item);

						if (item.TryFirst<IncludeConfig>(true).FinalSearch)
							break;
					}

					if (mode == SearchMode.Exact)
						break;

					basePath = Path.GetDirectoryName(basePath);
				}
				catch (UnauthorizedAccessException)
				{
					continue;
				}
			}

			return result;
		}
	}
}

