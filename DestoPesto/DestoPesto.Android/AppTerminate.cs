using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
[assembly: Xamarin.Forms.Dependency(typeof(DestoPesto.Droid.CloseApplication))]
namespace DestoPesto.Droid
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            var activity = MainActivity.mainLauncher;
                //(Activity)Forms.Context;
            activity.Finish();
        }
    }
}