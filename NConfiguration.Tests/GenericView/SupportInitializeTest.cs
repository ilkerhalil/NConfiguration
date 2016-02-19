using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class SupportInitializeTest
	{
		public class SupportInitializeType : ISupportInitialize
		{
			public string Text {get; set;}

			[IgnoreDataMember]
			public string OnBeginText = null;

			[IgnoreDataMember]
			public bool OnBegin = false;

			[IgnoreDataMember]
			public string OnEndText = null;

			[IgnoreDataMember]
			public bool OnEnd = false;

			public void BeginInit()
			{
				OnBeginText = Text;
				OnBegin = true;
			}

			public void EndInit()
			{
				OnEndText = Text;
				OnEnd = true;
			}
		}

		[Test]
		public void ParseNotEmpty()
		{
			var root = @"<Config Text='text'/>".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<SupportInitializeType>(root);

			Assert.IsTrue(tc.OnBegin);
			Assert.IsTrue(tc.OnEnd);
			Assert.IsNull(tc.OnBeginText);
			Assert.AreEqual("text", tc.OnEndText);
			Assert.AreEqual("text", tc.Text);
		}

		[Test]
		public void ParseNull()
		{
			var root = @"<Config />".ToXmlView();

			var tc = DefaultDeserializer.Instance.Deserialize<SupportInitializeType>(root);

			Assert.IsTrue(tc.OnBegin);
			Assert.IsTrue(tc.OnEnd);
			Assert.IsNull(tc.OnBeginText);
			Assert.IsNull(tc.OnEndText);
			Assert.IsNull(tc.Text);
		}
	}
}
