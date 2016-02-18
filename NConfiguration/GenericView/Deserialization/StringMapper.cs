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
	/// Support types: Boolean, Byte, SByte, Char, Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Decimal, TimeSpan, DateTime, Guid
	///  and their nullable variants.
	/// </summary>
	public partial class StringMapper : IStringMapper
	{
		private CultureInfo _ci;

		/// <summary>
		/// Specified CultureInfo
		/// </summary>
		public CultureInfo CurrentCulture
		{
			get
			{
				return _ci;
			}
		}

		/// <summary>
		/// Allows you to create delegates to convert a string to an instance of a specified type in culture-independent settings.
		/// Support types: Boolean, Byte, SByte, Char, Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, TimeSpan, DateTime, Guid
		///  and their nullable variants.
		/// </summary>
		public StringMapper()
			:this(CultureInfo.InvariantCulture)
		{
		}

		/// <summary>
		/// Allows you to create delegates to convert a string to an instance of a specified type in specified culture settings.
		/// Support types: Boolean, Byte, SByte, Char, Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, TimeSpan, DateTime, Guid
		///  and their nullable variants.
		/// </summary>
		/// <param name="ci">Specified culture settings.</param>
		public StringMapper(CultureInfo ci)
		{
			if (ci == null)
				throw new ArgumentNullException("ci");

			_ci = ci;
		}

		/// <summary>
		/// Creates a delegate to convert a string to an instance of a specified type.
		/// </summary>
		/// <param name="type">Type of the desired object.</param>
		/// <returns>Instance of Func[string, type].</returns>
		public virtual object CreateFunction(Type type)
		{
			if (type == typeof(string))
				return CreateNativeConverter();

			if (type.IsEnum)
				return CreateStringToEnum(type);

			var undtype = Nullable.GetUnderlyingType(type);
			if (undtype != null && undtype.IsEnum) // is nullable enum
				return CreateStringToNullableEnum(type, undtype);

			return CreateConverterFromString(type);
		}

		private object DefaultConverter(Type type)
		{
			var mi = typeof(StringMapper).GetMethod("DefaultConverter", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(type);
			var funcType = typeof(Func<,>).MakeGenericType(typeof(string), type);
			return Delegate.CreateDelegate(funcType, mi);
		}

		private static T DefaultConverter<T>(string val)
		{
			return (T)Convert.ChangeType(val, typeof(T));
		}

		private object CreateNativeConverter()
		{
			var mi = typeof(StringMapper).GetMethod("NativeConverter", BindingFlags.NonPublic | BindingFlags.Static);
			var funcType = typeof(Func<string, string>);
			return Delegate.CreateDelegate(funcType, mi);
		}

		private static string NativeConverter(string val)
		{
			return val;
		}

		private object CreateStringToEnum(Type type)
		{
			var mi = typeof(EnumHelper<>).MakeGenericType(type).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);
			var funcType = typeof(Func<,>).MakeGenericType(typeof(string), type);
			return Delegate.CreateDelegate(funcType, mi);
		}

		private object CreateStringToNullableEnum(Type type, Type underlyingType)
		{
			var mi = GetType().GetMethod("ToNullableEnum", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(underlyingType);
			var funcType = typeof(Func<,>).MakeGenericType(typeof(string), type);
			return Delegate.CreateDelegate(funcType, mi);
		}

		public static T? ToNullableEnum<T>(string text) where T: struct
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;

			return EnumHelper<T>.Parse(text);
		}

		/// <summary>
		/// Converts the specified string, which encodes binary data as base-64 digits, to an equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="text">The string to convert.</param>
		/// <returns>An array of 8-bit unsigned integers that is equivalent to text.</returns>
		public Byte[] ToByteArray(string text)
		{
			return System.Convert.FromBase64String(text);
		}

		/// <summary>
		/// Converts the string representation of a char to its Char equivalent.
		/// </summary>
		/// <param name="text">A string that represents the char to convert.</param>
		/// <returns>A Char equivalent to the char contained in text.</returns>
		public Char ToChar(string text)
		{
			if (text.Length != 1)
				throw new ArgumentOutOfRangeException("text", "must contain only one char");
			return text[0];
		}

		/// <summary>
		/// Converts the string representation of a number to its 16-bit unsigned integer equivalent.
		/// </summary>
		/// <param name="text">A string that represents the number to convert.</param>
		/// <returns>A 16-bit unsigned integer equivalent to the number contained in text.</returns>
		public UInt16 ToUInt16(string text)
		{
			return UInt16.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 16-bit signed integer equivalent.
		/// </summary>
		/// <param name="text">A string containing a number to convert.</param>
		/// <returns>A 16-bit signed integer equivalent to the number contained in text.</returns>
		public Int16 ToInt16(string text)
		{
			return Int16.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 32-bit unsigned integer equivalent.
		/// </summary>
		/// <param name="text">A string representing the number to convert.</param>
		/// <returns>A 32-bit unsigned integer equivalent to the number contained in text.</returns>
		public UInt32 ToUInt32(string text)
		{
			return UInt32.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 32-bit signed integer equivalent.
		/// </summary>
		/// <param name="text">A string containing a number to convert.</param>
		/// <returns>A 32-bit signed integer equivalent to the number contained in text.</returns>
		public Int32 ToInt32(string text)
		{
			return Int32.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 64-bit unsigned integer equivalent.
		/// </summary>
		/// <param name="text">A string that represents the number to convert.</param>
		/// <returns>A 64-bit unsigned integer equivalent to the number contained in text.</returns>
		public UInt64 ToUInt64(string text)
		{
			return UInt64.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 64-bit signed integer equivalent.
		/// </summary>
		/// <param name="text">A string containing a number to convert.</param>
		/// <returns>A 64-bit signed integer equivalent to the number contained in text.</returns>
		public Int64 ToInt64(string text)
		{
			return Int64.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its single-precision floating-point number equivalent.
		/// </summary>
		/// <param name="text">A string that contains a number to convert.</param>
		/// <returns>A single-precision floating-point number equivalent to the numeric value or symbol specified in text.</returns>
		public Single ToSingle(string text)
		{
			return Single.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its double-precision floating-point number equivalent.
		/// </summary>
		/// <param name="text">A string that contains a number to convert.</param>
		/// <returns>A double-precision floating-point number that is equivalent to the numeric value or symbol specified in text.</returns>
		public Double ToDouble(string text)
		{
			return Double.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its System.Decimal number equivalent.
		/// </summary>
		/// <param name="text">A string that contains a number to convert.</param>
		/// <returns>A System.Decimal number that is equivalent to the numeric value or symbol specified in text.</returns>
		public Decimal ToDecimal(string text)
		{
			return Decimal.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 8-bit signed integer equivalent.
		/// </summary>
		/// <param name="text">A string that represents a number to convert. The string is interpreted using the System.Globalization.NumberStyles.Integer style.</param>
		/// <returns>An 8-bit signed integer that is equivalent to the number contained in the text parameter.</returns>
		public SByte ToSByte(string text)
		{
			return SByte.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its System.Byte equivalent.
		/// </summary>
		/// <param name="text">A string containing a number to convert. The string is interpreted using the System.Globalization.NumberStyles.Integer style.</param>
		/// <returns>The System.Byte value equivalent to the number contained in text.</returns>
		public Byte ToByte(string text)
		{
			return Byte.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a time interval to its System.TimeSpan equivalent.
		/// </summary>
		/// <param name="text">A string that specifies the time interval to convert.</param>
		/// <returns>A time interval that corresponds to text.</returns>
		public TimeSpan ToTimeSpan(string text)
		{
			return TimeSpan.Parse(text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a GUID to the equivalent System.Guid structure.
		/// </summary>
		/// <param name="text">The GUID to convert.</param>
		/// <returns>A structure that contains the value that was parsed.</returns>
		public Guid ToGuid(string text)
		{
			return Guid.Parse(text);
		}

		/// <summary>
		/// Converts the specified string representation of a date and time to its System.DateTime equivalent.
		/// </summary>
		/// <param name="text">A string containing a date and time to convert.</param>
		/// <returns>A System.DateTime equivalent to the date and time contained in text</returns>
		public DateTime ToDateTime(string text)
		{
			return DateTime.Parse(text, _ci,
				DateTimeStyles.AdjustToUniversal |
				DateTimeStyles.AllowWhiteSpaces |
				DateTimeStyles.AssumeUniversal |
				DateTimeStyles.NoCurrentDateDefault);
		}

		/// <summary>
		/// returns the origin string
		/// </summary>
		public string ToString(string text)
		{
			return text;
		}

		private static Dictionary<string, bool> _booleanMap = new Dictionary<string, bool>(NameComparer.Instance)
		{
			{"true", true},
			{"yes", true},
			{"1", true},
			{"+", true},
			{"t", true},
			{"y", true},
			{"false", false},
			{"no", false},
			{"0", false},
			{"-", false},
			{"f", false},
			{"n", false}
		};

		/// <summary>
		/// Converts the specified string representation of a logical value to its System.Boolean equivalent.
		/// Support formats: true/false, t/f, yes/no, y/n, 1/0, +/- in case insensitivity.
		/// </summary>
		/// <param name="text">A string containing the value to convert.</param>
		public bool ToBoolean(string text)
		{
			bool result;
			if (_booleanMap.TryGetValue(text, out result))
				return result;

			throw new FormatException(string.Format("can not convert '{0}' to a boolean type", text));
		}
	}
}

