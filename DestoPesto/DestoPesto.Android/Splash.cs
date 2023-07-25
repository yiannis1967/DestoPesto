using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DestoPesto.Droid
{

    /// <MetaDataID>{100481b8-85be-46de-b328-e98db5af6037}</MetaDataID>
    [Activity(Theme = "@style/splashTheme", MainLauncher = true, NoHistory = true)]
    public class Splash : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Intent?.Extras?.KeySet() != null)
            {
                if (App.IntentExtras == null)
                    App.IntentExtras = new Dictionary<string, string>();
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                    App.IntentExtras[key] = value;

                }
            }


            // Create your application here
            StartActivity(typeof(MainActivity));
        }

    }
}