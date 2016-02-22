using NConfiguration.Combination;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Serialization
{
	public class TestMemberAttrClass
	{
		public string F1;

		[Deserializer(typeof(PlaceHolderDeserializer))]
		public string F1A;

		public string P1 { get; set; }

		[Deserializer(typeof(PlaceHolderDeserializer))]
		public string P1A { get; set; }
	}
}

