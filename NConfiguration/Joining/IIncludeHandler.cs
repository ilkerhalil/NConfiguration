using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration.Joining
{
	public interface IIncludeHandler<T>
	{
		IEnumerable<IIdentifiedSource> TryLoad(IConfigNodeProvider owner, T includeConfig, string searchPath);
	}
}
