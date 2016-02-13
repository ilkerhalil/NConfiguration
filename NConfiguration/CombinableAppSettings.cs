using NConfiguration.Combination;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NConfiguration
{
	/// <summary>
	/// Store and combine application settings
	/// </summary>
	public class CombinableAppSettings : ICombinableAppSettings
	{
		public IAppSettings Settings { get; private set; }
		public ICombiner Combiner { get; private set; }

		public CombinableAppSettings(IAppSettings settings)
			: this(settings, DefaultCombiner.Instance)
		{
		}

		public CombinableAppSettings(IAppSettings settings, ICombiner combiner)
		{
			Settings = settings;
			Combiner = combiner;
		}

		public T Get<T>(string sectionName)
		{
			var cfgs = Settings.LoadCollection<T>(sectionName).GetEnumerator();

			if (!cfgs.MoveNext())
				throw new SectionNotFoundException(sectionName, typeof(T));

			T sum = cfgs.Current;

			while (cfgs.MoveNext())
				sum = Combiner.Combine<T>(sum, cfgs.Current);

			return sum;
		}

		public T TryGet<T>(string sectionName)
		{
			var cfgs = Settings.LoadCollection<T>(sectionName).GetEnumerator();

			if (!cfgs.MoveNext())
				return default(T);

			T sum = cfgs.Current;

			while (cfgs.MoveNext())
				sum = Combiner.Combine<T>(sum, cfgs.Current);

			return sum;
		}
	}
}
