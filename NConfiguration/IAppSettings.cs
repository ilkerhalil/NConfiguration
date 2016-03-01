using NConfiguration.Combination;
using NConfiguration.Serialization;
using System.Collections.Generic;

namespace NConfiguration
{
	/// <summary>
	/// Store application settings
	/// </summary>
	public interface IAppSettings //UNDONE rename
	{
		IConfigNodeProvider Nodes { get; }

		IDeserializer Deserializer { get; }

		ICombiner Combiner { get; }
	}
}
