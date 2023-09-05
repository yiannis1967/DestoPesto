using Android.Media;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Android.Provider.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(DestoPesto.Droid.DeviceCore))]

namespace DestoPesto.Droid
{
    /// <MetaDataID>{9197f2b7-0392-426b-b818-566c2f0fa1b9}</MetaDataID>

    public class DeviceCore : IDevice
    {

        public bool IsBackgroundServiceStarted
        {
            get
            {

                if (ForegroundServiceManager == null)
                    return false;
                else
                    return ForegroundServiceManager.IsServiceStarted;

            }
        }



        public bool RunInBackground(Action action, BackgroundServiceState serviceState)
        {
            if (ForegroundServiceManager != null)
            {
                return ForegroundServiceManager.Run(action, serviceState);

            }
            return false;
        }

        public void StopBackgroundService()
        {
            if (ForegroundServiceManager != null)
                ForegroundServiceManager.Stop();
        }

        public Task<PermissionStatus> RemoteNotificationsPermissionsRequest()
        {
            return MainActivity.NotificationPermissionsRequest();
      

        }


        public Task<PermissionStatus> RemoteNotificationsPermissionsCheck()
        {




            if ((int)Build.VERSION.SdkInt < 33)
                return Task.FromResult(PermissionStatus.Granted);

            if (MainActivity.CheckSelfPermission(Android.Manifest.Permission.PostNotifications) == Android.Content.PM.Permission.Granted)
                return Task.FromResult(PermissionStatus.Granted);
            else
                return Task.FromResult(PermissionStatus.Denied);


        }

        public void PermissionsGranted()
        {
            MainActivity?.InitAfterPermissionsGranted();
        }

        static internal string m_androidId;

        static internal string m_OldandroidId = "";

        public static MyForeGroundService ForegroundServiceManager { get; internal set; }
        public static MainActivity MainActivity { get; internal set; }
    }


}
