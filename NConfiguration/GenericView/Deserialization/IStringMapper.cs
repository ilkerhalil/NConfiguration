using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Collections.Concurrent;
using System.Xml.Linq;

namespace NConfiguration.Serialization.Deserialization
{
	/// <summary>
	/// Allows you to create delegates to convert a string to an instance of a specified type.
	/// </summary>
	public interface IStringMapper
	{
		/// <summary>
		/// Creates a delegate to convert a string to an instance of a specified type.
		/// </summary>
		/// <param name="type">type of the desired object</param>
		/// <returns>instance of Func[string, type]</returns>
		object CreateFunction(Type type);
	}
}

