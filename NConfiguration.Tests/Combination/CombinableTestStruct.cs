using System;

namespace NConfiguration.Combination
{
	public struct CombinableTestStruct: ICombinable_obsolete
	{
		public string Text;

		public void Combine(object other)
		{
			if (other == null)
				return;

			var otherText = ((CombinableTestStruct) other).Text;

			if (otherText != null)
				Text = otherText + Text;

			
		}
	}
}

