using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;
using System.Globalization;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class DeserializePrimitiveTypes
	{
		public class PrimitiveTypeContainer
		{
			public string Text {get; set;}
			public bool? Bool { get; set; }
			public Byte? Byte { get; set; }
			public SByte? SByte { get; set; }
			public Char? Char { get; set; }
			public Int16? Int16 { get; set; }
			public Int32? Int32 { get; set; }
			public Int64? Int64 { get; set; }
			public UInt16? UInt16 { get; set; }
			public UInt32? UInt32 { get; set; }
			public UInt64? UInt64 { get; set; }
			public Single? Single { get; set; }
			public Double? Double { get; set; }
			public Decimal? Decimal { get; set; }
			public TimeSpan? TimeSpan { get; set; }
			public DateTime? DateTime { get; set; }
			public Guid? Guid { get; set; }

			public ByteEn? ByteEn { get; set; }
			public SByteEn? SByteEn { get; set; }
			public ShortEn? ShortEn { get; set; }
			public UShortEn? UShortEn { get; set; }
			public IntEn? IntEn { get; set; }
			public UIntEn? UIntEn { get; set; }
			public LongEn? LongEn { get; set; }
			public ULongEn? ULongEn { get; set; }
		}

		public enum ByteEn: byte
		{
			One,
			Two
		}

		public enum SByteEn : sbyte
		{
			One,
			Two
		}

		public enum ShortEn : short
		{
			One,
			Two
		}

		public enum UShortEn : ushort
		{
			One,
			Two
		}

		public enum IntEn : int
		{
			One,
			Two
		}

		public enum UIntEn : uint
		{
			One,
			Two
		}

		public enum LongEn : long
		{
			One,
			Two
		}

		public enum ULongEn : ulong
		{
			One,
			Two
		}

		[Test]
		public void ParseNotEmpty()
		{
			var root = @"<Config
			Text='text'
			Bool='+'
			Byte='120'
			SByte='-120'
			Char='Q'
			Int16='1'
			Int32='1'
			Int64='1'
			UInt16='1'
			UInt32='1'
			UInt64='1'
			Single='1'
			Double='1'
			Decimal='1'
			TimeSpan='0:0:0'
			DateTime='2006-10-26'
			Guid='0FB12DA2-A8BD-4BF7-8507-9ECA5771FD3B'
			ByteEn='One'
			SByteEn='One'
			ShortEn='One'
			UShortEn='One'
			IntEn='One'
			UIntEn='One'
			LongEn='One'
			ULongEn='One' />".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<PrimitiveTypeContainer>(root);

			Assert.AreEqual("text", tc.Text);
			Assert.AreEqual(true, tc.Bool);
			Assert.AreEqual(120, tc.Byte);
			Assert.AreEqual(-120, tc.SByte);
			Assert.AreEqual('Q', tc.Char);
			Assert.AreEqual(1, tc.Int16);
			Assert.AreEqual(1, tc.Int32);
			Assert.AreEqual(1, tc.Int64);
			Assert.AreEqual(1, tc.UInt16);
			Assert.AreEqual(1, tc.UInt32);
			Assert.AreEqual(1, tc.UInt64);
			Assert.AreEqual(1, tc.Single);
			Assert.AreEqual(1, tc.Double);
			Assert.AreEqual(1, tc.Decimal);
			Assert.AreEqual(TimeSpan.Zero, tc.TimeSpan);
			Assert.AreEqual(DateTime.Parse("2006-10-26", CultureInfo.InvariantCulture), tc.DateTime);
			Assert.AreEqual(Guid.Parse("0FB12DA2-A8BD-4BF7-8507-9ECA5771FD3B"), tc.Guid);
			Assert.AreEqual(ByteEn.One, tc.ByteEn);
			Assert.AreEqual(SByteEn.One, tc.SByteEn);
			Assert.AreEqual(ShortEn.One, tc.ShortEn);
			Assert.AreEqual(UShortEn.One, tc.UShortEn);
			Assert.AreEqual(IntEn.One, tc.IntEn);
			Assert.AreEqual(UIntEn.One, tc.UIntEn);
			Assert.AreEqual(LongEn.One, tc.LongEn);
			Assert.AreEqual(ULongEn.One, tc.ULongEn);
		}

		[Test]
		public void ParseEmpty()
		{
			var root = @"<Config
			Bool=''
			Byte=''
			SByte=''
			Char=''
			Int16=''
			Int32=''
			Int64=''
			UInt16=''
			UInt32=''
			UInt64=''
			Single=''
			Double=''
			Decimal=''
			TimeSpan=''
			DateTime=''
			Guid=''
			ByteEn=''
			SByteEn=''
			ShortEn=''
			UShortEn=''
			IntEn=''
			UIntEn=''
			LongEn=''
			ULongEn=''/>".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<PrimitiveTypeContainer>(root);

			Assert.IsNull(tc.Text);
			Assert.IsNull(tc.Bool);
			Assert.IsNull(tc.Byte);
			Assert.IsNull(tc.SByte);
			Assert.IsNull(tc.Char);
			Assert.IsNull(tc.Int16);
			Assert.IsNull(tc.Int32);
			Assert.IsNull(tc.Int64);
			Assert.IsNull(tc.UInt16);
			Assert.IsNull(tc.UInt32);
			Assert.IsNull(tc.UInt64);
			Assert.IsNull(tc.Single);
			Assert.IsNull(tc.Double);
			Assert.IsNull(tc.Decimal);
			Assert.IsNull(tc.TimeSpan);
			Assert.IsNull(tc.DateTime);
			Assert.IsNull(tc.Guid);
			Assert.IsNull(tc.ByteEn);
			Assert.IsNull(tc.SByteEn);
			Assert.IsNull(tc.ShortEn);
			Assert.IsNull(tc.UShortEn);
			Assert.IsNull(tc.IntEn);
			Assert.IsNull(tc.UIntEn);
			Assert.IsNull(tc.LongEn);
			Assert.IsNull(tc.ULongEn);
		}
	}
}
