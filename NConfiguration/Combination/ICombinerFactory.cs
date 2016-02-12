using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration.Combination
{
	public interface ICombinerFactory
	{
		object CreateInstance(Type targetType);
	}
}
