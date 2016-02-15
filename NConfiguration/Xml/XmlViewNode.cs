using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using NConfiguration.GenericView;

namespace NConfiguration.Xml
{
	/// <summary>
	/// The mapping XML-document to nodes of configuration
	/// </summary>
	public class XmlViewNode: ICfgNode
	{
		private XElement _element;
		private IStringConverter _converter;

		/// <summary>
		/// The mapping XML-document to nodes of configuration
		/// </summary>
		/// <param name="converter">string converter into a simple values</param>
		/// <param name="element">XML element</param>
		public XmlViewNode(IStringConverter converter, XElement element)
		{
			_converter = converter;
			_element = element;
		}

		/// <summary>
		/// Converts the value of a node in an instance of the specified type.
		/// </summary>
		/// <typeparam name="T">The required type</typeparam>
		/// <returns>The required instance</returns>
		public T As<T>()
		{
			return _converter.Convert<T>(_element.Value);
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> Nested
		{
			get
			{
				foreach (var attr in _element.Attributes())
					yield return new KeyValuePair<string, ICfgNode>(attr.Name.LocalName, new ViewPlainField(_converter, attr.Value));

				foreach (var el in _element.Elements())
					yield return new KeyValuePair<string, ICfgNode>(el.Name.LocalName, new XmlViewNode(_converter, el));
			}
		}

	}
}

