using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NConfiguration.Serialization.Enums
{
	internal interface IEnumParser<T> where T: struct
	{
		T Parse(string text);
	}
}
