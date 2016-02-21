using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Serialization
{
	[Deserializer(typeof(TestGenericAttrClass.Deserializer<>))]
	public class TestGenericAttrClass
	{
		public string F1;
		public int F2;

		public class Deserializer<T> : IDeserializer<T> where T : TestGenericAttrClass, new()
		{
			T IDeserializer<T>.Deserialize(IDeserializer context, ICfgNode cfgNode)
			{

				return new T()
				{
					F1 = context.Deserialize<string>(cfgNode.NestedByName("F1").FirstOrDefault()) + "attr",
					F2 = context.Deserialize<int>(cfgNode.NestedByName("F2").FirstOrDefault()) + 10
				};
			}
		}
	}
}

