using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration.Serialization
{
	public interface IDeserializerFactory
	{
		object CreateInstance(Type targetType);
	}
}
