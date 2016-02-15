using System.Linq;
using System.Collections.Generic;

namespace NConfiguration.GenericView
{
	public static class CfgNodeExtensions
	{
		public static ICfgNode GetChild(this ICfgNode parent, string name)
		{
			return parent.Nested
				.Where(p => NameComparer.Equals(p.Key, name))
				.Select(p => p.Value)
				.FirstOrDefault();
		}

		/// <summary>
		/// Returns the collection of child nodes with the specified name or empty if no match is found.
		/// </summary>
		/// <param name="name">node name is not case-sensitive.</param>
		/// <returns>Returns the collection of child nodes with the specified name or empty if no match is found.</returns>
		public static IEnumerable<ICfgNode> GetCollection(this ICfgNode parent, string name)
		{
			return parent.Nested
				.Where(p => NameComparer.Equals(p.Key, name))
				.Select(p => p.Value);
		}
	}
}
