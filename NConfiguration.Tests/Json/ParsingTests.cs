using NConfiguration.Xml;
using NUnit.Framework;
using NConfiguration.Serialization;
using NConfiguration.Tests;
using System;
using NConfiguration.Json.Parsing;
using System.Linq;

namespace NConfiguration.Json
{
	[TestFixture]
	public class ParsingTests
	{
		[TestCase(@"[""Unclosed array""")]
		[TestCase(@"{unquoted_key: ""keys must be quoted""}")]
		[TestCase(@"[""extra comma"",]")]
		[TestCase(@"[""double extra comma"",,]")]
		[TestCase(@"[   , ""<-- missing value""]")]
		[TestCase(@"[""Comma after the close""],")]
		[TestCase(@"[""Extra close""]]")]
		[TestCase(@"{""Extra comma"": true,}")]
		[TestCase(@"{""Extra value after close"": true} ""misplaced quoted value""")]
		[TestCase(@"{""Illegal expression"": 1 + 2}")]
		[TestCase(@"{""Illegal invocation"": alert()}")]
		[TestCase(@"{""Numbers cannot have leading zeroes"": 013}")]
		[TestCase(@"{""Numbers cannot be hex"": 0x14}")]
		[TestCase(@"[""Illegal backslash escape: \x15""]")]
		[TestCase(@"[\naked]")]
		[TestCase(@"[""Illegal backslash escape: \017""]")]
		[TestCase(@"{""Missing colon"" null}")]
		[TestCase(@"{""Double colon"":: null}")]
		[TestCase(@"{""Comma instead of colon"", null}")]
		[TestCase(@"[""Colon instead of comma"": false]")]
		[TestCase(@"[""Bad value"", truth]")]
		[TestCase(@"['single quote']")]
		[TestCase(@"[""	tab	character	in	string	""]")]
		[TestCase(@"[""tab\   character\   in\  string\  ""]")]
		[TestCase(@"[""line
break""]")]
		[TestCase(@"[""line\
break""]")]
		[TestCase(@"[0e]")]
		[TestCase(@"[0e+]")]
		[TestCase(@"[0e+-1]")]
		[TestCase(@"{""Comma instead if closing brace"": true,")]
		[TestCase(@"[""mismatch""}")]
		[ExpectedException(typeof(FormatException))]
		public void BadParse(string text)
		{
			JValue.Parse(text);
		}

		string _text1 = @"[
    ""JSON Test Pattern pass1"",
    {""object with 1 member"":[""array with 1 element""]},
    {},
    [],
    -42,
    true,
    false,
    null,
    {
        ""integer"": 1234567890,
        ""real"": -9876.543210,
        ""e"": 0.123456789e-12,
        ""E"": 1.234567890E+34,
        """":  23456789012E66,
        ""zero"": 0,
        ""one"": 1,
        ""space"": "" "",
        ""quote"": ""\"""",
        ""backslash"": ""\\"",
        ""controls"": ""\b\f\n\r\t"",
        ""slash"": ""/ & \/"",
        ""alpha"": ""abcdefghijklmnopqrstuvwyz"",
        ""ALPHA"": ""ABCDEFGHIJKLMNOPQRSTUVWYZ"",
        ""digit"": ""0123456789"",
        ""0123456789"": ""digit"",
        ""special"": ""`1~!@#$%^&*()_+-={':[,]}|;.</>?"",
        ""hex"": ""\u0123\u4567\u89AB\uCDEF\uabcd\uef4A"",
        ""true"": true,
        ""false"": false,
        ""null"": null,
        ""array"":[  ],
        ""object"":{  },
        ""address"": ""50 St. James Street"",
        ""url"": ""http://www.JSON.org/"",
        ""comment"": ""// /* <!-- --"",
        ""# -- --> */"": "" "",
        "" s p a c e d "" :[1,2 , 3

,

4 , 5        ,          6           ,7        ],""compact"":[1,2,3,4,5,6,7],
        ""jsontext"": ""{\""object with 1 member\"":[\""array with 1 element\""]}"",
        ""quotes"": ""&#34; \u0022 %22 0x22 034 &#x22;"",
        ""\/\\\""\uCAFE\uBABE\uAB98\uFCDE\ubcda\uef4A\b\f\n\r\t`1~!@#$%^&*()_+-=[]{}|;:',./<>?""
: ""A key can be any string""
    },
    0.5 ,98.6
,
99.44
,

1066,
1e1,
0.1e1,
1e-1,
1e00,2e+00,2e-00
,""rosebud""]";

		[Test]
		public void SuccessParse1()
		{
			var rootArr = (JArray)JValue.Parse(_text1);

			Assert.That(rootArr.Items[0].ToString(), Is.EqualTo("JSON Test Pattern pass1"));
			Assert.That(((JArray)(((JObject)rootArr.Items[1])["object with 1 member"])).Items[0].ToString(),
				Is.EqualTo("array with 1 element"));
			Assert.That(((JObject)rootArr.Items[2]).Properties, Is.Empty);
			Assert.That(((JArray)rootArr.Items[3]).Items, Is.Empty);
			Assert.That(rootArr.Items[4].ToString(), Is.EqualTo("-42"));
			Assert.That(rootArr.Items[5].ToString(), Is.EqualTo("true"));
			Assert.That(rootArr.Items[6].ToString(), Is.EqualTo("false"));
			Assert.That(rootArr.Items[7].ToString(), Is.EqualTo("null"));

			var o8 = (JObject)rootArr.Items[8];

			Assert.That(o8["integer"].ToString(), Is.EqualTo("1234567890"));
			Assert.That(o8["real"].ToString(), Is.EqualTo("-9876.543210"));
			Assert.That(o8["e"].ToString(), Is.EqualTo("0.123456789e-12"));
			Assert.That(o8["E"].ToString(), Is.EqualTo("1.234567890E+34"));
			Assert.That(o8[""].ToString(), Is.EqualTo("23456789012E66"));
			Assert.That(o8["zero"].ToString(), Is.EqualTo("0"));
			Assert.That(o8["one"].ToString(), Is.EqualTo("1"));
			Assert.That(o8["space"].ToString(), Is.EqualTo(" "));
			Assert.That(o8["quote"].ToString(), Is.EqualTo("\""));
			Assert.That(o8["backslash"].ToString(), Is.EqualTo("\\"));
			Assert.That(o8["controls"].ToString(), Is.EqualTo("\b\f\n\r\t"));
			Assert.That(o8["slash"].ToString(), Is.EqualTo("/ & /"));
			Assert.That(o8["alpha"].ToString(), Is.EqualTo("abcdefghijklmnopqrstuvwyz"));
			Assert.That(o8["ALPHA"].ToString(), Is.EqualTo("ABCDEFGHIJKLMNOPQRSTUVWYZ"));
			Assert.That(o8["digit"].ToString(), Is.EqualTo("0123456789"));
			Assert.That(o8["0123456789"].ToString(), Is.EqualTo("digit"));
			Assert.That(o8["special"].ToString(), Is.EqualTo("`1~!@#$%^&*()_+-={':[,]}|;.</>?"));
			Assert.That(o8["hex"].ToString(), Is.EqualTo("\u0123\u4567\u89AB\uCDEF\uabcd\uef4A"));
			Assert.That(o8["true"].ToString(), Is.EqualTo("true"));
			Assert.That(o8["false"].ToString(), Is.EqualTo("false"));
			Assert.That(o8["null"].ToString(), Is.EqualTo("null"));
			Assert.That(o8["array"], Is.InstanceOf<JArray>());
			Assert.That(o8["object"], Is.InstanceOf<JObject>());
			Assert.That(o8["address"].ToString(), Is.EqualTo("50 St. James Street"));
			Assert.That(o8["url"].ToString(), Is.EqualTo("http://www.JSON.org/"));
			Assert.That(o8["comment"].ToString(), Is.EqualTo("// /* <!-- --"));
			Assert.That(o8["# -- --> */"].ToString(), Is.EqualTo(" "));
			Assert.That(((JArray)o8[" s p a c e d "]).Items.Select(i => i.ToString()), Is.EquivalentTo(Enumerable.Range(1, 7).Select(i => i.ToString())));
			Assert.That(((JArray)o8["compact"]).Items.Select(i => i.ToString()), Is.EquivalentTo(Enumerable.Range(1, 7).Select(i => i.ToString())));
			Assert.That(o8["jsontext"].ToString(), Is.EqualTo(@"{""object with 1 member"":[""array with 1 element""]}"));
			Assert.That(o8["quotes"].ToString(), Is.EqualTo("&#34; \u0022 %22 0x22 034 &#x22;"));
			Assert.That(o8["/\\\"\uCAFE\uBABE\uAB98\uFCDE\ubcda\uef4A\b\f\n\r\t`1~!@#$%^&*()_+-=[]{}|;:',./<>?"].ToString(), Is.EqualTo("A key can be any string"));

			Assert.That(rootArr.Items[9].ToString(), Is.EqualTo("0.5"));
			Assert.That(rootArr.Items[10].ToString(), Is.EqualTo("98.6"));
			Assert.That(rootArr.Items[11].ToString(), Is.EqualTo("99.44"));
			Assert.That(rootArr.Items[12].ToString(), Is.EqualTo("1066"));
			Assert.That(rootArr.Items[13].ToString(), Is.EqualTo("1e1"));
			Assert.That(rootArr.Items[14].ToString(), Is.EqualTo("0.1e1"));
			Assert.That(rootArr.Items[15].ToString(), Is.EqualTo("1e-1"));
			Assert.That(rootArr.Items[16].ToString(), Is.EqualTo("1e00"));
			Assert.That(rootArr.Items[17].ToString(), Is.EqualTo("2e+00"));
			Assert.That(rootArr.Items[18].ToString(), Is.EqualTo("2e-00"));
			Assert.That(rootArr.Items[19].ToString(), Is.EqualTo("rosebud"));
		}

		string _text2 = @"[[[[[[[[[[[[[[[[[[[""Too deep""]]]]]]]]]]]]]]]]]]]";

		[Test]
		public void SuccessParse2()
		{
			var rootVal = JValue.Parse(_text2);

			Assert.That(rootVal.Type, Is.EqualTo(TokenType.Array));
			
			var arr = (JArray)rootVal;

			for(int i=0;i<18; i++)
			{
				Assert.That(arr.Items.Count, Is.EqualTo(1));
				arr = (JArray)arr.Items[0];
			}

			Assert.That(arr.Items.Count, Is.EqualTo(1));
			Assert.That(arr.Items[0].Type, Is.EqualTo(TokenType.String));
			Assert.That(((JString)arr.Items[0]).Value, Is.EqualTo("Too deep"));
		}

		string _text3 = @"{
    ""JSON Test Pattern pass3"": {
        ""The outermost value"": ""must be an object or array."",
        ""In this test"": ""It is an object.""
    }
}";

		[Test]
		public void SuccessParse3()
		{
			var rootVal = JValue.Parse(_text3);
			Assert.That(rootVal.Type, Is.EqualTo(TokenType.Object));

			var obj = (JObject)rootVal;

			Assert.That(obj.Properties[0].Key, Is.EqualTo("JSON Test Pattern pass3"));
			Assert.That(obj.Properties[0].Value.Type, Is.EqualTo(TokenType.Object));

			obj = (JObject)obj.Properties[0].Value;
			Assert.That(obj.Properties[0].Key, Is.EqualTo("The outermost value"));
			Assert.That(obj.Properties[1].Key, Is.EqualTo("In this test"));

			Assert.That(((JString)obj.Properties[1].Value).Value, Is.EqualTo("It is an object."));
		}
		
	}
}

