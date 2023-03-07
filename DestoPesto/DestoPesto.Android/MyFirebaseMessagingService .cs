using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DestoPesto.Droid
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public MyFirebaseMessagingService()
        {

        }
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
        {
            App.Current.MainPage.DisplayAlert("OnMessageReceived", "OnMessageReceived", "OK");
        });


            Log.Debug(TAG, "From: " + message.From);
            Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
        }
        public override void OnNewToken(string p0)
        {
            base.OnNewToken(p0);
        }

    }
}