
using DestoPesto.iOS;
using Firebase.CloudMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;


[assembly: Xamarin.Forms.Dependency(typeof(DestoPesto.iOS.DeviceCore))]

namespace DestoPesto.iOS
{
    /// <MetaDataID>{9197f2b7-0392-426b-b818-566c2f0fa1b9}</MetaDataID>


    public class DeviceCore : IDevice
    {

        public bool IsBackgroundServiceStarted
        {
            get
            {
                if (Background != null && Background.Status == TaskStatus.Running)
                    return true;

                return false;

            }
        }

        public static AppDelegate AppDelegate { get; internal set; }

        Task Background;

        public bool RunInBackground(Action action, BackgroundServiceState serviceState)
        {



            if (Background == null || Background.Status != TaskStatus.Running)
            {
                Background = null;
                Background = Task.Run(() =>
                {
                    action.Invoke();
                });
                return true;
            }
            return false;
        }

        public Task<PermissionStatus> RemoteNotificationsPermissionsCheck()
        {
            TaskCompletionSource<PermissionStatus> taskCompletionSource = new TaskCompletionSource<PermissionStatus>();
            UNUserNotificationCenter.Current.GetNotificationSettings((UNNotificationSettings settings) =>
            {
                if (settings.AlertSetting == UNNotificationSetting.Enabled)
                {
                    taskCompletionSource.SetResult(PermissionStatus.Granted);
                }
                else if (settings.AlertSetting == UNNotificationSetting.NotSupported)
                {
                    taskCompletionSource.SetResult(PermissionStatus.Denied);
                }
                else if (settings.AlertSetting == UNNotificationSetting.Disabled)
                {
                    taskCompletionSource.SetResult(PermissionStatus.Disabled);
                }
                else
                    taskCompletionSource.SetResult(PermissionStatus.Denied);




            });

            return taskCompletionSource.Task;

        }

        public Task<PermissionStatus> RemoteNotificationsPermissionsRequest()
        {
            TaskCompletionSource<PermissionStatus> taskCompletionSource = new TaskCompletionSource<PermissionStatus>();
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {


                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                {
                    if(granted)
                        taskCompletionSource.SetResult(PermissionStatus.Granted);
                    else
                        taskCompletionSource.SetResult(PermissionStatus.Disabled);


                    Console.WriteLine(granted);
                });



                // For iOS 10 display notification (sent via APNS)
                //UNUserNotificationCenter.Current.Delegate = AppDelegate;

                // For iOS 10 data message (sent via FCM)
                //Messaging.SharedInstance.Delegate = AppDelegate;
            }
            else
            {
                taskCompletionSource.SetResult(PermissionStatus.Granted);

                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                taskCompletionSource.SetResult(PermissionStatus.Granted);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();
            return taskCompletionSource.Task;
        }


        public void StopBackgroundService()
        {

        }

        public async Task PermissionsGranted()
        {

            await DeviceCore.AppDelegate.RegisterForRemoteNotifications();

            return;
   

        }

        static internal string m_androidId;

        static internal string m_OldandroidId = "";




    }


}
