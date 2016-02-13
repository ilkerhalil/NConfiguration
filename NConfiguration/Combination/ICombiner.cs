using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Combination
{
	public interface ICombiner
	{
		T Combine<T>(ICombiner combiner, T x, T y);
	}
}
