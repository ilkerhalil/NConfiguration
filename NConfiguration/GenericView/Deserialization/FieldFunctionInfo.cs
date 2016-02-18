using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NConfiguration.Serialization.Deserialization
{
	public class FieldFunctionInfo
	{
		public Type ResultType { get; private set; }
		public bool IsPublic { get; private set; }
		public object[] CustomAttributes { get; private set; }

		public FieldFunctionType Function { get; set; }
		public string Name { get; set; }
		public bool Required { get; set; }
		public bool Ignore { get; set; }

		public FieldFunctionInfo(FieldInfo fi)
		{
			ResultType = fi.FieldType;
			Name = fi.Name;
			IsPublic = fi.IsPublic;
			CustomAttributes = fi.GetCustomAttributes(true);
		}

		public FieldFunctionInfo(PropertyInfo pi)
		{
			ResultType = pi.PropertyType;
			Name = pi.Name;
			IsPublic = pi.CanWrite && pi.GetSetMethod(true).IsPublic;
			CustomAttributes = pi.GetCustomAttributes(true);
		}
	}
}
