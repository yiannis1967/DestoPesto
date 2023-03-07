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
    [BroadcastReceiver(Enabled = true)]
   
    public class BrodcastHandler : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {

            NotificationManager manager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            if (manager.GetActiveNotifications().Length > 0)
            {

                context.StopService(new Intent(context, typeof(BackgroundService)));
                context.StartService(new Intent(context, typeof(BackgroundService)));

            }



         




        }


    }
   
}
