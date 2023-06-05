using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using Java.Lang;
using Java.Sql;

using Xamarin.Essentials;
//using Exception = System.Exception;

namespace DestoPesto.Droid
{
    /// <summary>
    /// This is a sample started service. When the service is started, it will log a string that details how long 
    /// the service has been running (using Android.Util.Log). This service displays a notification in the notification
    /// tray while the service is active.
    /// </summary>
    /// <MetaDataID>{a8667a5a-c1e7-4e98-802e-27a52e6b7657}</MetaDataID>

    public class ForegroundService : Service
    {
        static readonly string TAG = typeof(ForegroundService).FullName;



        public static bool isStarted;
        Handler handler;



        static List<string> StartCommands = new List<string>();
        static List<string> StopCommands = new List<string>();
        static Dictionary<string, ServiceState> ServiceStates = new Dictionary<string, ServiceState>();
        public void StartForegroundService(Context packageContext, string action, ServiceState serviceState)
        {
            StartCommands.Add(action);
            ServiceStates[action] = serviceState;
            var startServiceIntent = new Intent(Platform.CurrentActivity, GetType());
            startServiceIntent.SetAction(action);
            Platform.CurrentActivity.StartService(startServiceIntent);

        }

        public void StopForegroundService(Context packageContext, string action)
        {
            StopCommands.Add(action);
            var stopServiceIntent = new Intent(packageContext, GetType());
            stopServiceIntent.SetAction(action);
            Platform.CurrentActivity.StopService(stopServiceIntent);

        }


        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(TAG, "OnCreate: the service is initializing.");

            handler = new Handler();

            // This Action is only for demonstration purposes.

        }


        ServiceState State;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                if (StartCommands.Contains(intent.Action))
                {
                    State = ServiceStates[intent.Action];
                    if (isStarted)
                    {
                        //Log.Info(TAG, "OnStartCommand: The service is already running.");
                    }
                    else
                    {
                        //Log.Info(TAG, "OnStartCommand: The service is starting.");
                        try
                        {

                            RegisterForegroundService(State);

                        }
                        catch (Java.Lang.Exception error)
                        {
                            throw;
                        }

                        Runnable = new Action(() =>
                        {
                            Task.Run(() =>
                            {
                                State.Runnable.Invoke();
                                StopForeground(true);
                                StopSelf();
                                isStarted = false;
                                State.Terminate = true;
                                if (State.BackgroundServiceState != null)
                                    State.BackgroundServiceState.Terminate = true;

                            });
                        });



                        handler.PostDelayed(Runnable, State.DelayBetweenLogMessage);
                        isStarted = true;
                    }
                }
                else if (StopCommands.Contains(intent.Action))
                {
                    Log.Info(TAG, "OnStartCommand: The service is stopping.");

                    StopForeground(true);
                    StopSelf();
                    isStarted = false;
                    State.Terminate = true;
                    if (State.BackgroundServiceState != null)
                        State.BackgroundServiceState.Terminate = true;

                }


            }
            catch (Java.Lang.Exception error)
            {

                throw;
            }
            // This tells Android not to restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }


        public override IBinder OnBind(Intent intent)
        {
            // Return null because this is a pure started service. A hybrid service would return a binder that would
            // allow access to the GetFormattedStamp() method.
            return null;
        }


        public override void OnDestroy()
        {
            // We need to shut things down.

            Log.Info(TAG, "OnDestroy: The started service is shutting down.");

            // Stop the handler.
            handler.RemoveCallbacks(State.Runnable);

            // Remove the notification from the status bar.
            var notificationManager = (Android.App.NotificationManager)GetSystemService(NotificationService);
            notificationManager.Cancel(State.ServiceRunningNotificationID);
            isStarted = false;
            base.OnDestroy();
        }

        public Action Runnable { get; internal set; }

        public class ServiceState
        {
            public string NotificationTitle { get; set; }
            public string NotificationContentText { get; set; }
            public int NotificationSmallIcon { get; set; }
            public string StopServiceCommandTitle { get; set; }
            public int StopServiceCommandIcon { get; set; }
            public Action Runnable { get; set; }
            public bool Terminate { get; set; }
            public string StopActionID { get; set; }
            public int DelayBetweenLogMessage { get; set; }
            public int ServiceRunningNotificationID { get; set; }
            public string ActionsMainActivity { get; set; }
            public string ServiceStartedKey { get; set; }
            public BackgroundServiceState BackgroundServiceState { get; set; }
        }

        void RegisterForegroundService(ServiceState notificationData)
        {
            string NOTIFICATION_CHANNEL_ID = "com.Your.project.id";
            NotificationChannel chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, "Your Channel Name", Android.App.NotificationImportance.High);

            Android.App.NotificationManager manager = (Android.App.NotificationManager)GetSystemService(Context.NotificationService);

            manager.CreateNotificationChannel(chan);

            NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);

            Notification notification = notificationBuilder.SetOngoing(true)
               .SetContentTitle(notificationData.NotificationTitle)
                .SetContentText(notificationData.NotificationContentText)
                .SetSmallIcon(notificationData.NotificationSmallIcon)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .AddAction(BuildStopServiceAction(notificationData))
                .SetSilent(true)
                .Build();

            StopCommands.Add(notificationData.StopActionID);

            const int Service_Running_Notification_ID = 936;
            StartForeground(Service_Running_Notification_ID, notification);
        }




        /// <summary>
        /// Builds a PendingIntent that will display the main activity of the app. This is used when the 
        /// user taps on the notification; it will take them to the main activity of the app.
        /// </summary>
        /// <returns>The content intent.</returns>
        PendingIntent BuildIntentToShowMainActivity()
        {

            var notificationIntent = new Intent(this, Xamarin.Essentials.Platform.CurrentActivity.GetType());
            notificationIntent.SetAction(State.ActionsMainActivity);// Constants.ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(State.ServiceStartedKey, true);


            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent|PendingIntentFlags.Immutable);
            return pendingIntent;
        }



        /// <summary>
        /// Builds the Notification.Action that will allow the user to stop the service via the
        /// notification in the status bar
        /// </summary>
        /// <returns>The stop service action.</returns>
        NotificationCompat.Action BuildStopServiceAction(ServiceState serviceState)
        {

            var stopServiceIntent = new Intent(this, GetType());
            stopServiceIntent.SetAction(serviceState.StopActionID);
            var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, PendingIntentFlags.Immutable);

            var builder = new NotificationCompat.Action.Builder(serviceState.StopServiceCommandIcon,
                                                        serviceState.StopServiceCommandTitle,
                                                          stopServicePendingIntent);
            return builder.Build();

        }
    }



}