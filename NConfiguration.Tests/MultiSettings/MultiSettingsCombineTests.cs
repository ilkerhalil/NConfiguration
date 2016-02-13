using System;
using NUnit.Framework;
using NConfiguration.Joining;
using NConfiguration.Combination;
namespace NConfiguration
{
	[TestFixture]
	public class MultiSettingsCombineTests: MultiSettingsBase
	{
		[Test]
		public void ForwardReplace()
		{
			var s = new MultiSettings();

			s.Add(GetXmlSettings("ACfg_FA"));
			s.Add(GetXmlSettings("ACfg"));


			var cfg = s.Combine<CombineConfig>("ACfg", DefaultCombiner.Instance);
			Assert.AreEqual("A", cfg.F);
		}
		
		[TestCase(null, "Empty", "Empty", "ACfg")]
		[TestCase("A", "Empty", "Empty", "ACfg_FA")]
		[TestCase("B", "Empty", "Empty", "ACfg_FB")]
		[TestCase(null, "Empty", "ACfg", "Empty")]
		[TestCase(null, "Empty", "ACfg", "ACfg")]
		[TestCase("A", "Empty", "ACfg", "ACfg_FA")]
		[TestCase("B", "Empty", "ACfg", "ACfg_FB")]
		[TestCase("A", "Empty", "ACfg_FA", "Empty")]
		[TestCase("A", "Empty", "ACfg_FA", "ACfg")]
		[TestCase("A", "Empty", "ACfg_FA", "ACfg_FA")]
		[TestCase("B", "Empty", "ACfg_FA", "ACfg_FB")]
		[TestCase("B", "Empty", "ACfg_FB", "Empty")]
		[TestCase("B", "Empty", "ACfg_FB", "ACfg")]
		[TestCase("A", "Empty", "ACfg_FB", "ACfg_FA")]
		[TestCase("B", "Empty", "ACfg_FB", "ACfg_FB")]

		[TestCase(null, "ACfg", "Empty", "Empty")]
		[TestCase(null, "ACfg", "Empty", "ACfg")]
		[TestCase("A", "ACfg", "Empty", "ACfg_FA")]
		[TestCase("B", "ACfg", "Empty", "ACfg_FB")]
		[TestCase(null, "ACfg", "ACfg", "Empty")]
		[TestCase(null, "ACfg", "ACfg", "ACfg")]
		[TestCase("A", "ACfg", "ACfg", "ACfg_FA")]
		[TestCase("B", "ACfg", "ACfg", "ACfg_FB")]
		[TestCase("A", "ACfg", "ACfg_FA", "Empty")]
		[TestCase("A", "ACfg", "ACfg_FA", "ACfg")]
		[TestCase("A", "ACfg", "ACfg_FA", "ACfg_FA")]
		[TestCase("B", "ACfg", "ACfg_FA", "ACfg_FB")]
		[TestCase("B", "ACfg", "ACfg_FB", "Empty")]
		[TestCase("B", "ACfg", "ACfg_FB", "ACfg")]
		[TestCase("A", "ACfg", "ACfg_FB", "ACfg_FA")]
		[TestCase("B", "ACfg", "ACfg_FB", "ACfg_FB")]

		[TestCase("A", "ACfg_FA", "Empty", "Empty")]
		[TestCase("A", "ACfg_FA", "Empty", "ACfg")]
		[TestCase("A", "ACfg_FA", "Empty", "ACfg_FA")]
		[TestCase("B", "ACfg_FA", "Empty", "ACfg_FB")]
		[TestCase("A", "ACfg_FA", "ACfg", "Empty")]
		[TestCase("A", "ACfg_FA", "ACfg", "ACfg")]
		[TestCase("A", "ACfg_FA", "ACfg", "ACfg_FA")]
		[TestCase("B", "ACfg_FA", "ACfg", "ACfg_FB")]
		[TestCase("A", "ACfg_FA", "ACfg_FA", "Empty")]
		[TestCase("A", "ACfg_FA", "ACfg_FA", "ACfg")]
		[TestCase("A", "ACfg_FA", "ACfg_FA", "ACfg_FA")]
		[TestCase("B", "ACfg_FA", "ACfg_FA", "ACfg_FB")]
		[TestCase("B", "ACfg_FA", "ACfg_FB", "Empty")]
		[TestCase("B", "ACfg_FA", "ACfg_FB", "ACfg")]
		[TestCase("A", "ACfg_FA", "ACfg_FB", "ACfg_FA")]
		[TestCase("B", "ACfg_FA", "ACfg_FB", "ACfg_FB")]

		[TestCase("B", "ACfg_FB", "Empty", "Empty")]
		[TestCase("B", "ACfg_FB", "Empty", "ACfg")]
		[TestCase("A", "ACfg_FB", "Empty", "ACfg_FA")]
		[TestCase("B", "ACfg_FB", "Empty", "ACfg_FB")]
		[TestCase("B", "ACfg_FB", "ACfg", "Empty")]
		[TestCase("B", "ACfg_FB", "ACfg", "ACfg")]
		[TestCase("A", "ACfg_FB", "ACfg", "ACfg_FA")]
		[TestCase("B", "ACfg_FB", "ACfg", "ACfg_FB")]
		[TestCase("A", "ACfg_FB", "ACfg_FA", "Empty")]
		[TestCase("A", "ACfg_FB", "ACfg_FA", "ACfg")]
		[TestCase("A", "ACfg_FB", "ACfg_FA", "ACfg_FA")]
		[TestCase("B", "ACfg_FB", "ACfg_FA", "ACfg_FB")]
		[TestCase("B", "ACfg_FB", "ACfg_FB", "Empty")]
		[TestCase("B", "ACfg_FB", "ACfg_FB", "ACfg")]
		[TestCase("A", "ACfg_FB", "ACfg_FB", "ACfg_FA")]
		[TestCase("B", "ACfg_FB", "ACfg_FB", "ACfg_FB")]
		public void Replace(string expected, params string[] confFiles)
		{
			var s = new MultiSettings();

			foreach(var name in confFiles)
				s.Add(GetXmlSettings(name));

			var cfg = s.Combine<CombineConfig>("ACfg", DefaultCombiner.Instance);
			Assert.AreEqual(expected, cfg.F);
		}

		[Test]
		public void ReplaceNull()
		{
			var s = new MultiSettings();

			for(int i =0; i<3 ; i++)
				s.Add(GetXmlSettings("Empty"));

			var cfg = s.TryCombine<CombineConfig>("ACfg", DefaultCombiner.Instance);
			Assert.IsNull(cfg);
		}
	}
}

