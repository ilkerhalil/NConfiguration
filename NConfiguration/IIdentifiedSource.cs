
namespace NConfiguration
{
	/// <summary>
	/// store application settings
	/// </summary>
	public interface IIdentifiedSource : IAppSettings, IConfigNodeProvider
	{
		/// <summary>
		/// source identifier the application settings
		/// </summary>
		string Identity { get; }
	}
}
