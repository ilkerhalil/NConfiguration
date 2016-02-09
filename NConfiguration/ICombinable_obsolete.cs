using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration
{
	/// <summary>
	/// object implementing this interface can be reduced at boot
	/// </summary>
	public interface ICombinable_obsolete
	{
		/// <summary>
		/// combine by other instance
		/// </summary>
		/// <param name="other">The object to combine with the current object.</param>
		void Combine(object other);
	}
}
