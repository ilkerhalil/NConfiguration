using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Serialization
{
	[Deserializer(typeof(TestAttrClass.Deserializer))]
	public class TestAttrClass
	{
		public string F1;
		public int F2;

		public class Deserializer : IDeserializer<TestAttrClass>
		{
			public TestAttrClass Deserialize(IDeserializer context, ICfgNode cfgNode)
			{
				return new TestAttrClass()
				{
					F1 = context.Deserialize<string>(cfgNode.NestedByName("F1").FirstOrDefault()) + "attr",
					F2 = context.Deserialize<int>(cfgNode.NestedByName("F2").FirstOrDefault()) + 10
				};
			}
		}
	}
}

