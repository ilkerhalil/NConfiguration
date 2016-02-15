using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NConfiguration.GenericView
{
	/// <summary>
	/// Representation of a simple text value.
	/// </summary>
	public class ViewPlainField: ICfgNode
	{
		private string _text;
		private IStringConverter _converter;

		/// <summary>
		/// Representation of a simple text value.
		/// </summary>
		/// <param name="converter">string converte</param>
		/// <param name="text">text value</param>
		public ViewPlainField(IStringConverter converter, string text)
		{
			_converter = converter;
			_text = text;
		}

		/// <summary>
		/// Return empty collection
		/// </summary>
		public IEnumerable<ICfgNode> GetCollection(string name)
		{
			yield break;
		}

		/// <summary>
		/// Converts the value of a node in an instance of the specified type.
		/// </summary>
		/// <typeparam name="T">The required type</typeparam>
		/// <returns>The required instance</returns>
		public T As<T>()
		{
			return _converter.Convert<T>(_text);
		}

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

