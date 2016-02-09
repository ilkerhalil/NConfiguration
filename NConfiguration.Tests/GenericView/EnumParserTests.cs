using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;
using NConfiguration.GenericView.Deserialization;

namespace NConfiguration.GenericView
{
	[TestFixture]
	public class EnumParserTests
	{
		public enum ByteEn: byte
		{
			One = 1,
			Two = 2,
			Max = 255
		}

		[TestCase("One", ByteEn.One)]
		[TestCase("Two", ByteEn.Two)]
		[TestCase("Max", ByteEn.Max)]
		[TestCase("ONE", ByteEn.One)]
		[TestCase("TWO", ByteEn.Two)]
		[TestCase("MAX", ByteEn.Max)]
		[TestCase("1", ByteEn.One)]
		[TestCase("2", ByteEn.Two)]
		[TestCase("255", ByteEn.Max)]
		[TestCase("0x01", ByteEn.One)]
		[TestCase("0x02", ByteEn.Two)]
		[TestCase("0xff", ByteEn.Max)]
		public void SuccessByteParse(string text, ByteEn expected)
		{
			Assert.AreEqual(expected, EnumHelper<ByteEn>.Parse(text));
		}

		[TestCase("One1")]
		[TestCase("3")]
		[TestCase("Max,One")]
		[TestCase("0")]
		[ExpectedException(typeof(FormatException))]
		public void FailByteParse(string text)
		{
			EnumHelper<ByteEn>.Parse(text);
		}

		[Flags]
		public enum FByteEn : byte
		{
			None = 0,
			R = 0x01,
			G = 0x02,
			B = 0x04,
			A = 0x08,
			W = R | G | B,
		}

		[TestCase("A", FByteEn.A)]
		[TestCase("B", FByteEn.B)]
		[TestCase("B,R", FByteEn.B|FByteEn.R)]
		[TestCase("G", FByteEn.G)]
		[TestCase("W", FByteEn.W)]
		[TestCase("R,B,G", FByteEn.W)]
		[TestCase("R", FByteEn.R)]
		[TestCase("None", FByteEn.None)]
		[TestCase("8", FByteEn.A)]
		[TestCase("0x04", FByteEn.B)]
		[TestCase("5", FByteEn.B | FByteEn.R)]
		[TestCase("2", FByteEn.G)]
		[TestCase("7", FByteEn.W)]
		[TestCase("5,G", FByteEn.W)]
		[TestCase("", FByteEn.None)]
		[TestCase("0", FByteEn.None)]
		public void SuccessFlagByteParse(string text, FByteEn expected)
		{
			Assert.AreEqual(expected, EnumHelper<FByteEn>.Parse(text));
		}

		[TestCase("One1")]
		[TestCase("300")]
		[TestCase("Max,One")]
		[TestCase("0xFFFF")]
		[ExpectedException(typeof(FormatException))]
		public void FailFlagByteParse(string text)
		{
			EnumHelper<FByteEn>.Parse(text);
		}
	}
}
