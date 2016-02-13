using NConfiguration.Combination;
using System;
using System.Collections.Generic;

namespace NConfiguration
{
	/// <summary>
	/// Store and combine application settings
	/// </summary>
	public interface ICombinableAppSettings
	{
		IAppSettings Settings { get; }

		/// <summary>
		/// Load the configuration object by name.
		/// </summary>
		/// <param name='settings'>instance of application settings</param>
		/// <param name='sectionName'>Section name.</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		T Get<T>(string sectionName);

		/// <summary>
		/// Trying to load the configuration object by name.
		/// </summary>
		/// <param name='settings'>instance of application settings</param>
		/// <param name='sectionName'>Section name.</param>
		/// <typeparam name='T'>type of configuration</typeparam>
		T TryGet<T>(string sectionName);
	}
}
