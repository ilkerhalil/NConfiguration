using NConfiguration.Serialization;
using System;

namespace NConfiguration.Serialization.Deserialization
{
	public interface IGenericMapper
	{
		object CreateFunction(Type targetType, IGenericDeserializer deserializer, StringConverter converter);
	}
}