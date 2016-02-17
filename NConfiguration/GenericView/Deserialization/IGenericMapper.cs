using System;

namespace NConfiguration.GenericView.Deserialization
{
	public interface IGenericMapper
	{
		object CreateFunction(Type targetType, IGenericDeserializer deserializer, StringConverter converter);
	}
}