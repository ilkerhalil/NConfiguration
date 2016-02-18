using NConfiguration.Xml;
using NUnit.Framework;
using NConfiguration.Serialization;
using NConfiguration.Tests;

namespace NConfiguration.Ini
{
	[TestFixture]
	public class ParsingTests
	{
		[TestCase("")]
		[TestCase(@" ")]
		[TestCase(@";kjhsdufi
;svfvsr

;svsevsev")]
		[TestCase(@"


")]
		[TestCase(";uiuhoioh")]
		[TestCase(";;")]
		public void Empty(string text)
		{
			var ini = text.ToIniSections();

			Assert.That(ini, Is.Empty);
		}

		[Test]
		public void EmptySections()
		{
			var ini = @";tuytih h
[s1] ;uiuo
;hkjh
[s2]

".ToIniSections();

			Assert.That(ini, Is.Not.Empty);
			Assert.That(ini[0].Name, Is.EqualTo("s1"));
			Assert.That(ini[0].Pairs, Is.Empty);
			Assert.That(ini[1].Name, Is.EqualTo("s2"));
			Assert.That(ini[1].Pairs, Is.Empty);
		}

		[Test]
		public void SimpleValues()
		{
			var ini = @"[s1]
key1 = value1
key2 =	value2	;werr
key3= value3;ygwsofw
key4=value4 
".ToIniSections();

			Assert.That(ini, Is.Not.Empty);
			Assert.That(ini[0].Name, Is.EqualTo("s1"));
			Assert.That(ini[0].Pairs[0].Key, Is.EqualTo("key1"));
			Assert.That(ini[0].Pairs[0].Value, Is.EqualTo("value1"));
			Assert.That(ini[0].Pairs[1].Key, Is.EqualTo("key2"));
			Assert.That(ini[0].Pairs[1].Value, Is.EqualTo("value2	"));
			Assert.That(ini[0].Pairs[2].Key, Is.EqualTo("key3"));
			Assert.That(ini[0].Pairs[2].Value, Is.EqualTo("value3"));
			Assert.That(ini[0].Pairs[3].Key, Is.EqualTo("key4"));
			Assert.That(ini[0].Pairs[3].Value, Is.EqualTo("value4 "));
		}

		[Test]
		public void QuotedValues()
		{
			var ini = @"[s1]
key1 = ""value1""
key2 =	""value2	"";werr
key3=    ""value3;ygwsofw
key4=value4"" 
key4 = ""xxx""""XXX""""xxx""
".ToIniSections();

			Assert.That(ini, Is.Not.Empty);
			Assert.That(ini[0].Name, Is.EqualTo("s1"));
			Assert.That(ini[0].Pairs[0].Key, Is.EqualTo("key1"));
			Assert.That(ini[0].Pairs[0].Value, Is.EqualTo("value1"));
			Assert.That(ini[0].Pairs[1].Key, Is.EqualTo("key2"));
			Assert.That(ini[0].Pairs[1].Value, Is.EqualTo("value2	"));
			Assert.That(ini[0].Pairs[2].Key, Is.EqualTo("key3"));
			Assert.That(ini[0].Pairs[2].Value, Is.EqualTo(@"value3;ygwsofw
key4=value4"));
			Assert.That(ini[0].Pairs[3].Key, Is.EqualTo("key4"));
			Assert.That(ini[0].Pairs[3].Value, Is.EqualTo("xxx\"XXX\"xxx"));
		}

	}
}

