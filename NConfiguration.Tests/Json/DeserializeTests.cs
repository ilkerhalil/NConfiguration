using NConfiguration.Xml;
using NUnit.Framework;
using NConfiguration.Serialization;
using NConfiguration.Tests;
using System;
using NConfiguration.Json.Parsing;
using System.Linq;
using System.Collections.Generic;

namespace NConfiguration.Json
{
	[TestFixture]
	public class DeserializeTests
	{
		JsonStringSettings settings = new JsonStringSettings(@"{
	""Section1"":
	{
		""Item1"":""item1.value1"",
		""Item2"": 1231232,
		""nullItem"":null,
		""Item3"":""09d89405-59b4-4a4b-af2e-c8a134a4860a""
	},
	""Section2"":
	{
		""Item1"": [ 1, 2 ],
		""Item1"": null,
		""Item1"": [ null, [ 1, 2 ]]
	},
	""Section3"":
	{
		""Item1"":""item1.value1"",
		""Item1"": [ 1, 2 ],
		""Item1"": null,
		""Item1"": [ null, [ 1, 2 ]]
	}}");

		public class Section1
		{
			public string item1 { get; set; }
			public long item2 { get; set; }
			public int? nullItem { get; set; }
			public Guid item3 { get; set; }
		}

		public class Section2
		{
			public List<int?> Item1 { get; set; }
		}

		public class Section3
		{
			public List<string> Item1 { get; set; }
		}

		[Test]
		public void Simple()
		{
			var section = settings.First<Section1>("Section1");
			Assert.That(section.item1, Is.EqualTo("item1.value1"));
			Assert.That(section.item2, Is.EqualTo(1231232));
			Assert.That(section.nullItem, Is.Null);
			Assert.That(section.item3, Is.EqualTo(new Guid("09d89405-59b4-4a4b-af2e-c8a134a4860a")));
		}

		[Test]
		public void MultiIntArray()
		{
			var section = settings.First<Section2>("Section2");
			Assert.That(section.Item1, Is.EquivalentTo(new int?[] { 1, 2, null, null, 1, 2}));
		}

		[Test]
		public void MultiStringArray()
		{
			var section = settings.First<Section3>("Section3");
			Assert.That(section.Item1, Is.EquivalentTo(new string[] {"item1.value1", "1", "2", null, null, "1", "2" }));
		}
	}
}

