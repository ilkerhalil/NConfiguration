using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Xml.Protected
{
	/// <summary>
	/// This configuration can decrypt XML section
	/// </summary>
	public interface IXmlEncryptable : IConfigNodeProvider
	{
		/// <summary>
		/// the collection providers to decrypt XML sections
		/// </summary>
		IProviderCollection Providers { get; set; }
	}
}
