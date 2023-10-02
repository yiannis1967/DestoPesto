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

        string _DeviceID;
        public string DeviceID
        {
            get
            {
                if (_DeviceID==null)
                    _DeviceID=GetDeviceUniqueID();
                return _DeviceID;
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

        public static string GetDeviceUniqueID()
        {
            // If all else fails, if the user does have lower than API 9 (lowerba
            // than Gingerbread), has reset their phone or 'Secure.ANDROID_ID'
            // returns 'null', then simply the ID returned will be solely based
            // off their Android device information. This is where the collisions
            // can happen.
            // Thanks http://www.pocketmagic.net/?p=1662!
            // Try not to use DISPLAY, HOST or ID - these items could change.
            // If there are collisions, there will be overlapping data
            String m_szDevIDShort = "35" +
                    (Build.Board.Length % 10)
                    + (Build.Brand.Length % 10)
                    + (Build.CpuAbi.Length % 10)
                    + (Build.Device.Length % 10)
                    + (Build.Manufacturer.Length % 10)
                    + (Build.Model.Length % 10)
                    + (Build.Product.Length % 10);

            // Thanks to @Roman SL!
            // http://stackoverflow.com/a/4789483/950427
            // Only devices with API >= 9 have android.os.Build.SERIAL
            // http://developer.android.com/reference/android/os/Build.html#SERIAL
            // If a user upgrades software or roots their phone, there will be a duplicate entry
            String serial = null;
            try
            {
                serial = Build.Serial;

                // Go ahead and return the serial for api => 9
                return new Java.Util.UUID(m_szDevIDShort.GetHashCode(), serial.GetHashCode()).ToString();
            }
            catch (Exception e)
            {
                // String needs to be initialized
                serial = "serial"; // some value
            }

            // Thanks @Joe!
            // http://stackoverflow.com/a/2853253/950427
            // Finally, combine the values we have found by using the UUID class to create a unique identifier

            // DebugLog..LOGE(new UUID(m_szDevIDShort.hashCode(), serial.hashCode()).toString());

            return new Java.Util.UUID(m_szDevIDShort.GetHashCode(), serial.GetHashCode()).ToString();
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

        public Task PermissionsGranted()
        {
            MainActivity?.InitAfterPermissionsGranted();
            return Task.FromResult(true);

        }

        static internal string m_androidId;

        static internal string m_OldandroidId = "";

        public static MyForeGroundService ForegroundServiceManager { get; internal set; }
        public static MainActivity MainActivity { get; internal set; }
    }


}
