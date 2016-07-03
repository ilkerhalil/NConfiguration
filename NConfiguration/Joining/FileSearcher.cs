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
	public sealed class FileSearcher: IIncludeHandler<IncludeFileConfig>
	{
		private readonly Func<string, IIdentifiedSource> _creater;
		private readonly HashSet<string> _validExtensions;

		public FileSearcher(Func<string, IIdentifiedSource> creater, params string[] validExtensions)
		{
			_creater = creater;
			_validExtensions = new HashSet<string>(validExtensions.Select(_ => "." + _), NameComparer.Instance);
		}

		public event EventHandler<FindingSettingsArgs> FindingSettings;

		private void OnFindingSettings(IConfigNodeProvider source, IncludeFileConfig cfg, string searchPath)
		{
			var copy = FindingSettings;
			if (copy != null)
				copy(this, new FindingSettingsArgs(source, cfg, searchPath));
		}

		public IEnumerable<IIdentifiedSource> TryLoad(IConfigNodeProvider owner, IncludeFileConfig cfg, string searchPath)
		{
			var filePath = cfg.Path;

			if (_validExtensions.Count != 0 && !_validExtensions.Contains(Path.GetExtension(filePath)))
				yield break;

			if (Path.IsPathRooted(filePath))
			{
				OnFindingSettings(owner, cfg, null);

				if (!File.Exists(filePath) && !cfg.Required)
					yield break;

				yield return _creater(filePath);
				yield break;
			}

			string basePath;

			if(filePath[0] == '~')
			{
				if(searchPath == null || !Path.IsPathRooted(searchPath))
					throw new InvalidOperationException(
						string.Format("path '{0}' required rooted search path. But was define '{1}'", filePath, searchPath));

				filePath = filePath.Substring(2); // remove ~ and path separator
				basePath = searchPath;
			}
			else
			{
				var rpo = owner as IFilePathOwner;
				if (rpo == null)
					throw new InvalidOperationException("can not be searched for a relative path because the settings do not provide an absolute path");
				basePath = rpo.Path;
			}

			OnFindingSettings(owner, cfg, basePath);
			var found = SearchSettings(basePath, filePath, cfg.Search);

			if (found.Count == 0)
			{
				if (cfg.Required)
					throw new ApplicationException(string.Format("configuration file '{0}' not found in '{1}'", filePath, basePath));

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
						var item = _creater(fullPath);
						result.Add(item);
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

