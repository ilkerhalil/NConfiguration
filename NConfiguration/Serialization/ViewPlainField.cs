using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NConfiguration.Serialization
{
	/// <summary>
	/// Representation of a simple text value.
	/// </summary>
	public class ViewPlainField: ICfgNode
	{
		/// <summary>
		/// Representation of a simple text value.
		/// </summary>
		/// <param name="converter">string converte</param>
		/// <param name="text">text value</param>
		public ViewPlainField(string text)
		{
			Text = text;
		}

		public string Text { get; private set; }

		/// <summary>
		/// Return empty collection
		/// </summary>
		public IEnumerable<KeyValuePair<string, ICfgNode>> Nested
		{
			get
			{
				yield break;
			}
		}
	}
}

