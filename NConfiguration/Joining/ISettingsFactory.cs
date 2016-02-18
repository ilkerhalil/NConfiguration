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
	/// <summary>
	/// The interface for creating instances IIdentifiedSource
	/// </summary>
	public interface ISettingsFactory
	{
		/// <summary>
		/// name of including configuration
		/// </summary>
		string Tag { get; }
		/// <summary>
		/// creates a collection of includes configuration
		/// </summary>
		/// <param name="source">parent settings</param>
		/// <param name="config">include configuration node</param>
		/// <returns>collection of includes configuration</returns>
		IEnumerable<IIdentifiedSource> CreateSettings(IAppSettings source, ICfgNode config);
	}
}

