using System.Collections.Generic;

namespace NConfiguration.GenericView
{
	/// <summary>
	/// A node in the document of configuration
	/// </summary>
	public interface ICfgNode
	{
		/// <summary>
		/// Gets all the child nodes with their names.
		/// </summary>
		IEnumerable<KeyValuePair<string, ICfgNode>> Nested { get; }

		/// <summary>
		/// Converts the value of a node in an instance of the specified type.
		/// </summary>
		/// <typeparam name="T">The required type</typeparam>
		/// <returns>The required instance</returns>
		T As<T>();
	}
}
