using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using NConfiguration.Xml.Protected;
using NConfiguration.Serialization;
using System.Collections.Generic;
using NConfiguration.Json.Parsing;
using NConfiguration.Tests;

namespace NConfiguration.Json
{
	public class JsonStringSettings : JsonSettings
	{
		private readonly JObject _obj;

		public JsonStringSettings(string text)
		{
			var val = JValue.Parse(text);
			if (val.Type != TokenType.Object)
				throw new FormatException("required json object in content");

			_obj = (JObject)val;
		}

		protected override JObject Root
		{
			get { return _obj; }
		}
	}
}

