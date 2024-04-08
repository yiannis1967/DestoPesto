using Xamarin.Forms;
using DestoPesto.iOS;
using Foundation;

[assembly: Dependency (typeof (BaseUrl_iOS))]

namespace DestoPesto.iOS 
{
	public class BaseUrl_iOS : IBaseUrl 
	{
		public string Get () 
		{
			return NSBundle.MainBundle.BundlePath;
		}
	}
}