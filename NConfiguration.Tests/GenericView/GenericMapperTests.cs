using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NUnit.Framework;
using NConfiguration.Serialization.Deserialization;

namespace NConfiguration.Serialization
{
	[TestFixture]
	public class GenericMapperTests
	{
		public struct EmptyStruct
		{
		}

		public class EmptyClass
		{
		}

		[Test]
		public void CreateFunctionForEmptyStruct()
		{
			var mapper = new GenericMapper();
			var func = (Func<ICfgNode, EmptyStruct>)mapper.CreateFunction(typeof(EmptyStruct), null, new StringConverter());
			var result = func(null);

			Assert.AreEqual(new EmptyStruct(), result);
		}

		[Test]
		public void CreateFunctionForEmptyClass()
		{
			var mapper = new GenericMapper();
			var func = (Func<ICfgNode, EmptyClass>)mapper.CreateFunction(typeof(EmptyClass), null, new StringConverter());
			var result = func(null);

			Assert.NotNull(result);
		}

		public struct TestStruct
		{
			public string TextField;
			public string TextProp {get; set;}
		}

		public class TestClass
		{
			public string TextField;
			public string TextProp { get; set; }
		}

		[Test]
		public void CreateFunctionForTestStruct()
		{
			var mapper = new GenericMapper();
			mapper.CreateFunction(typeof(TestStruct), null, new StringConverter());
		}

		[Test]
		public void CreateFunctionForTestClass()
		{
			var mapper = new GenericMapper();
			mapper.CreateFunction(typeof(TestClass), null, new StringConverter());
		}
	}
}
