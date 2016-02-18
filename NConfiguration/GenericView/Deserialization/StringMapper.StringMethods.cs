


using System;
using System.Collections.Generic;
using System.Globalization;

namespace NConfiguration.Serialization.Deserialization
{
	public partial class StringMapper
	{
		private object CreateConverterFromString(Type type)
		{
			if(type == typeof(string))
				return (Func<string, string>)ToString;
			else if(type == typeof(byte[]))
				return (Func<string, byte[]>)ToByteArray;

			else if(type == typeof(Boolean))
				return (Func<string, Boolean>)ToBoolean;
			else if(type == typeof(Boolean?))
				return (Func<string, Boolean?>)ToNBoolean;

			else if(type == typeof(Byte))
				return (Func<string, Byte>)ToByte;
			else if(type == typeof(Byte?))
				return (Func<string, Byte?>)ToNByte;

			else if(type == typeof(SByte))
				return (Func<string, SByte>)ToSByte;
			else if(type == typeof(SByte?))
				return (Func<string, SByte?>)ToNSByte;

			else if(type == typeof(Char))
				return (Func<string, Char>)ToChar;
			else if(type == typeof(Char?))
				return (Func<string, Char?>)ToNChar;

			else if(type == typeof(Int16))
				return (Func<string, Int16>)ToInt16;
			else if(type == typeof(Int16?))
				return (Func<string, Int16?>)ToNInt16;

			else if(type == typeof(Int32))
				return (Func<string, Int32>)ToInt32;
			else if(type == typeof(Int32?))
				return (Func<string, Int32?>)ToNInt32;

			else if(type == typeof(Int64))
				return (Func<string, Int64>)ToInt64;
			else if(type == typeof(Int64?))
				return (Func<string, Int64?>)ToNInt64;

			else if(type == typeof(UInt16))
				return (Func<string, UInt16>)ToUInt16;
			else if(type == typeof(UInt16?))
				return (Func<string, UInt16?>)ToNUInt16;

			else if(type == typeof(UInt32))
				return (Func<string, UInt32>)ToUInt32;
			else if(type == typeof(UInt32?))
				return (Func<string, UInt32?>)ToNUInt32;

			else if(type == typeof(UInt64))
				return (Func<string, UInt64>)ToUInt64;
			else if(type == typeof(UInt64?))
				return (Func<string, UInt64?>)ToNUInt64;

			else if(type == typeof(Single))
				return (Func<string, Single>)ToSingle;
			else if(type == typeof(Single?))
				return (Func<string, Single?>)ToNSingle;

			else if(type == typeof(Double))
				return (Func<string, Double>)ToDouble;
			else if(type == typeof(Double?))
				return (Func<string, Double?>)ToNDouble;

			else if(type == typeof(Decimal))
				return (Func<string, Decimal>)ToDecimal;
			else if(type == typeof(Decimal?))
				return (Func<string, Decimal?>)ToNDecimal;

			else if(type == typeof(TimeSpan))
				return (Func<string, TimeSpan>)ToTimeSpan;
			else if(type == typeof(TimeSpan?))
				return (Func<string, TimeSpan?>)ToNTimeSpan;

			else if(type == typeof(DateTime))
				return (Func<string, DateTime>)ToDateTime;
			else if(type == typeof(DateTime?))
				return (Func<string, DateTime?>)ToNDateTime;

			else if(type == typeof(Guid))
				return (Func<string, Guid>)ToGuid;
			else if(type == typeof(Guid?))
				return (Func<string, Guid?>)ToNGuid;

			
			return DefaultConverter(type);
		}

		
		/// <summary>
		/// Convert text to Nullable[Boolean]
		/// </summary>
		/// <param name="text">A string that represents the Boolean to convert.</param>
		/// <returns>A Boolean equivalent to the specified in text or null-value if argument not contain a text</returns>
		public Boolean? ToNBoolean(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToBoolean(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[Byte]
		/// </summary>
		/// <param name="text">A string that represents the Byte to convert.</param>
		/// <returns>A Byte equivalent to the specified in text or null-value if argument not contain a text</returns>
		public Byte? ToNByte(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToByte(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[SByte]
		/// </summary>
		/// <param name="text">A string that represents the SByte to convert.</param>
		/// <returns>A SByte equivalent to the specified in text or null-value if argument not contain a text</returns>
		public SByte? ToNSByte(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToSByte(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[Char]
		/// </summary>
		/// <param name="text">A string that represents the Char to convert.</param>
		/// <returns>A Char equivalent to the specified in text or null-value if argument not contain a text</returns>
		public Char? ToNChar(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToChar(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[Int16]
		/// </summary>
		/// <param name="text">A string that represents the Int16 to convert.</param>
		/// <returns>A Int16 equivalent to the specified in text or null-value if argument not contain a text</returns>
		public Int16? ToNInt16(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToInt16(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[Int32]
		/// </summary>
		/// <param name="text">A string that represents the Int32 to convert.</param>
		/// <returns>A Int32 equivalent to the specified in text or null-value if argument not contain a text</returns>
		public Int32? ToNInt32(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToInt32(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[Int64]
		/// </summary>
		/// <param name="text">A string that represents the Int64 to convert.</param>
		/// <returns>A Int64 equivalent to the specified in text or null-value if argument not contain a text</returns>
		public Int64? ToNInt64(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToInt64(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[UInt16]
		/// </summary>
		/// <param name="text">A string that represents the UInt16 to convert.</param>
		/// <returns>A UInt16 equivalent to the specified in text or null-value if argument not contain a text</returns>
		public UInt16? ToNUInt16(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToUInt16(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[UInt32]
		/// </summary>
		/// <param name="text">A string that represents the UInt32 to convert.</param>
		/// <returns>A UInt32 equivalent to the specified in text or null-value if argument not contain a text</returns>
		public UInt32? ToNUInt32(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToUInt32(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[UInt64]
		/// </summary>
		/// <param name="text">A string that represents the UInt64 to convert.</param>
		/// <returns>A UInt64 equivalent to the specified in text or null-value if argument not contain a text</returns>
		public UInt64? ToNUInt64(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToUInt64(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[Single]
		/// </summary>
		/// <param name="text">A string that represents the Single to convert.</param>
		/// <returns>A Single equivalent to the specified in text or null-value if argument not contain a text</returns>
		public Single? ToNSingle(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToSingle(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[Double]
		/// </summary>
		/// <param name="text">A string that represents the Double to convert.</param>
		/// <returns>A Double equivalent to the specified in text or null-value if argument not contain a text</returns>
		public Double? ToNDouble(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToDouble(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[Decimal]
		/// </summary>
		/// <param name="text">A string that represents the Decimal to convert.</param>
		/// <returns>A Decimal equivalent to the specified in text or null-value if argument not contain a text</returns>
		public Decimal? ToNDecimal(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToDecimal(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[TimeSpan]
		/// </summary>
		/// <param name="text">A string that represents the TimeSpan to convert.</param>
		/// <returns>A TimeSpan equivalent to the specified in text or null-value if argument not contain a text</returns>
		public TimeSpan? ToNTimeSpan(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToTimeSpan(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[DateTime]
		/// </summary>
		/// <param name="text">A string that represents the DateTime to convert.</param>
		/// <returns>A DateTime equivalent to the specified in text or null-value if argument not contain a text</returns>
		public DateTime? ToNDateTime(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToDateTime(text);
		}

		
		/// <summary>
		/// Convert text to Nullable[Guid]
		/// </summary>
		/// <param name="text">A string that represents the Guid to convert.</param>
		/// <returns>A Guid equivalent to the specified in text or null-value if argument not contain a text</returns>
		public Guid? ToNGuid(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return ToGuid(text);
		}

	}
}

