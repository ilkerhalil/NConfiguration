using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NConfiguration.Serialization
{
	public class FieldFunctionInfo
	{
		public Type ResultType { get; private set; }
		public object[] CustomAttributes { get; private set; }

		public string Name { get; set; }
		public bool Required { get; set; }

		public FieldFunctionInfo(FieldInfo fi)
		{
			ResultType = fi.FieldType;
			Name = fi.Name;
			CustomAttributes = fi.GetCustomAttributes(true);
		}

		public FieldFunctionInfo(PropertyInfo pi)
		{
			ResultType = pi.PropertyType;
			Name = pi.Name;
			CustomAttributes = pi.GetCustomAttributes(true);
		}
	}
}
