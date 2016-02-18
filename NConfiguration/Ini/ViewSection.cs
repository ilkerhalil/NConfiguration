using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using NConfiguration.Serialization;
using System.Collections.Specialized;

namespace NConfiguration.Ini
{
	/// <summary>
	/// The mapping section in INI-document to nodes of configuration
	/// </summary>
	public class ViewSection: ICfgNode
	{
		private List<KeyValuePair<string, string>> _pairs;

		/// <summary>
		/// The mapping INI-document to nodes of configuration
		/// </summary>
		/// <param name="converter">string converter into a simple values</param>
		/// <param name="section">section in INI-document</param>
		public ViewSection(Section section)
		{
			_pairs = section.Pairs;
		}

		public string Text
		{
			get
			{
				throw new NotSupportedException("section can't contain value");
			}
		}

		/// <summary>
		/// Gets all the child nodes with their names.
		/// </summary>
		public IEnumerable<KeyValuePair<string, ICfgNode>> Nested
		{
			get
			{
				foreach (var pair in _pairs)
					yield return new KeyValuePair<string, ICfgNode>(pair.Key, new ViewPlainField(pair.Value));
			}
		}
	}
}

