using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Serialization
{
	public interface IDeserializer<T>
	{
		T Deserialize(IDeserializer context, ICfgNode cfgNode);
	}
}
