using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.GenericView.Deserialization
{
	public class GenericMapper : IGenericMapper
	{
		private readonly HashSet<Type> _primitiveTypes = new HashSet<Type>
			{
				typeof(String), typeof(Boolean), typeof(Char),
				typeof(Byte), typeof(SByte),
				typeof(Int16), typeof(Int32), typeof(Int64), typeof(UInt16), typeof(UInt32), typeof(UInt64),
				typeof(Single), typeof(Double), typeof(Decimal),
				typeof(TimeSpan), typeof(DateTime), typeof(Guid),
				typeof(byte[])
			};

		public GenericMapper()
		{
		}

		public virtual bool IsPrimitive(Type type)
		{
			var ntype = Nullable.GetUnderlyingType(type);
			if(ntype != null) // is Nullable<>
				type = ntype;

			if (type.IsEnum)
				return true;

			return _primitiveTypes.Contains(type);
		}

		public bool IsCollection(Type type)
		{
			if (!type.IsGenericType)
				return false;

			var genType = type.GetGenericTypeDefinition();

			return genType == typeof(List<>)
				|| genType == typeof(IList<>)
				|| genType == typeof(ICollection<>)
				|| genType == typeof(IEnumerable<>);
		}

		public object CreateFunction(Type targetType, IGenericDeserializer deserializer)
		{
			if (targetType == typeof(ICfgNode))
				return BuildToolkit.CreateNativeFunction();

			if(IsPrimitive(targetType))
				return BuildToolkit.CreatePrimitiveFunction(targetType);

			if(targetType.IsArray ||
				IsCollection(targetType))
				throw new ArgumentOutOfRangeException(string.Format("type '{0}' is collection", targetType.FullName));

			return CreateComplexFunctionBuilder(targetType, deserializer).Compile();
		}

		public virtual ComplexFunctionBuilder CreateComplexFunctionBuilder(Type targetType, IGenericDeserializer deserializer)
		{
			Action<FieldFunctionInfo> cffi;

			if (BuildToolkit.DataContractAvailable(targetType) == AttributeState.Found)
				cffi = DataContractFieldReader; // DataContract deserialize
			else if (BuildToolkit.XmlAvailable(targetType) == AttributeState.Found)
				cffi = XmlFieldReader; // Xml deserialize
			else
				cffi = NativeNameFieldReader; // Native name deserialize

			return new ComplexFunctionBuilder(targetType, deserializer, cffi);
		}

		public void NativeNameFieldReader(FieldFunctionInfo ffi)
		{
			if (!ffi.IsPublic)
			{
				ffi.Ignore = true;
				return;
			}

			ffi.Function = DefaultFunctionType(ffi.ResultType);
		}

		private FieldFunctionType DefaultFunctionType(Type type)
		{
			if (IsPrimitive(type))
				return FieldFunctionType.Primitive;
			else if(type.IsArray)
				return FieldFunctionType.Array;
			else if (IsCollection(type))
				return FieldFunctionType.Collection;
			else
				return FieldFunctionType.Complex;
		}

		public void DataContractFieldReader(FieldFunctionInfo ffi)
		{
			if (ffi.CustomAttributes.Any(o => o is IgnoreDataMemberAttribute))
			{
				ffi.Ignore = true;
				return;
			}

			ffi.Function = DefaultFunctionType(ffi.ResultType);

			var dmAttr = ffi.CustomAttributes.Select(o => o as DataMemberAttribute).FirstOrDefault(a => a != null);

			if (dmAttr == null && !ffi.IsPublic)
			{
				ffi.Ignore = true;
				return;
			}

			if (dmAttr != null && !string.IsNullOrWhiteSpace(dmAttr.Name))
				ffi.Name = dmAttr.Name;

			if(dmAttr != null)
				ffi.Required = dmAttr.IsRequired;
		}

		public void XmlFieldReader(FieldFunctionInfo ffi)
		{
			if (ffi.CustomAttributes.Any(o => o is XmlIgnoreAttribute))
			{
				ffi.Ignore = true;
				return;
			}

			ffi.Function = DefaultFunctionType(ffi.ResultType);

			var attrAttr = ffi.CustomAttributes.Select(o => o as XmlAttributeAttribute).FirstOrDefault(a => a != null);
			var elAttr = ffi.CustomAttributes.Select(o => o as XmlElementAttribute).FirstOrDefault(a => a != null);

			if (attrAttr == null && elAttr == null && !ffi.IsPublic)
			{
				ffi.Ignore = true;
				return;
			}

			if (attrAttr != null && elAttr != null)
				throw new ArgumentOutOfRangeException(string.Format("found XmlAttributeAttribute and XmlElementAttribute for member '{0}'", ffi.Name));

			if (attrAttr != null && !string.IsNullOrWhiteSpace(attrAttr.AttributeName))
				ffi.Name = attrAttr.AttributeName;

			if (elAttr != null && !string.IsNullOrWhiteSpace(elAttr.ElementName))
				ffi.Name = elAttr.ElementName;
		}
	}
}
