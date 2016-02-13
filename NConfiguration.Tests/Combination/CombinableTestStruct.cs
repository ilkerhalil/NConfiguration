using System;

namespace NConfiguration.Combination
{
	public struct CombinableTestStruct: ICombinable
	{
		public string Text;

		public void Combine(ICombiner combiner, object other)
		{
			if (other == null)
				return;

			var otherText = ((CombinableTestStruct) other).Text;

			if (otherText != null)
				Text = otherText + Text;
		}
	}
}

