using System;
using Xamarin.Forms;
using DestoPesto.Droid;

[assembly: Dependency (typeof (BaseUrl_Android))]
namespace DestoPesto.Droid 
{
	public class BaseUrl_Android : IBaseUrl 
	{
		public string Get () 
		{
			return "file:///android_asset/";
		}
	}
}