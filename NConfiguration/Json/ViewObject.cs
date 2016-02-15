using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NConfiguration.GenericView;
using NConfiguration.Json.Parsing;

namespace NConfiguration.Json
{
	/// <summary>
	/// The mapping JSON-document to nodes of configuration
	/// </summary>
	public class ViewObject: ICfgNode
	{
		private IStringConverter _converter;
		private JObject _obj;

		/// <summary>
		/// The mapping JSON-document to nodes of configuration
		/// </summary>
		/// <param name="converter">string converter into a simple values</param>
		/// <param name="obj">JSON object</param>
		public ViewObject(IStringConverter converter, JObject obj)
		{
			_converter = converter;
			_obj = obj;
		}

		public IEnumerable<KeyValuePair<string, ICfgNode>> Nested
		{
			get
			{
				foreach (var el in _obj.Properties)
					foreach (var val in FlatArray(el.Value))
						yield return new KeyValuePair<string, ICfgNode>(el.Key, CreateByJsonValue(_converter, val));
			}
		}

		/// <summary>
		/// Throw NotSupportedException.
		/// </summary>
		public T As<T>()
		{
			throw new NotSupportedException("JSON document can't contain value");
		}

		internal static ICfgNode CreateByJsonValue(IStringConverter converter, JValue val)
		{
			if (val == null)
				return null;

			switch (val.Type)
			{
				case TokenType.Null:
					return new ViewPlainField(converter, null);

				case TokenType.Object:
					return new ViewObject(converter, (JObject)val);

				case TokenType.String:
				case TokenType.Boolean:
				case TokenType.Number:
					return new ViewPlainField(converter, val.ToString());
				
				default:
					throw new NotSupportedException(string.Format("JSON type {0} not supported", val.Type));
			}
		}

		internal static IEnumerable<JValue> FlatArray(JValue val)
		{
			if (val == null)
				yield break;
			
			if (val.Type != TokenType.Array)
			{
				yield return val;
				yield break;
			}

			foreach (var item in ((JArray)val).Items)
			{
				foreach (var innerItem in FlatArray(item)) //HACK: remove recursion
				{
					yield return innerItem;
				}
			}
		}
	}
}
