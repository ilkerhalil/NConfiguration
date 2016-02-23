using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using NConfiguration.Serialization;

namespace NConfiguration.Xml
{
	/// <summary>
	/// The mapping XML-document to nodes of configuration
	/// </summary>
	public class XmlViewNode : CfgNode
	{
		private XElement _element;

		/// <summary>
		/// The mapping XML-document to nodes of configuration
		/// </summary>
		/// <param name="converter">string converter into a simple values</param>
		/// <param name="element">XML element</param>
		public XmlViewNode(XElement element)
		{
			_element = element;
		}

		public override string GetNodeText()
		{
			return _element.Value;
		}

		public override IEnumerable<KeyValuePair<string, ICfgNode>> GetNestedNodes()
		{
			foreach (var attr in _element.Attributes())
				yield return new KeyValuePair<string, ICfgNode>(attr.Name.LocalName, new ViewPlainField(attr.Value));

			foreach (var el in _element.Elements())
				yield return new KeyValuePair<string, ICfgNode>(el.Name.LocalName, new XmlViewNode(el));
		}
	}
}

