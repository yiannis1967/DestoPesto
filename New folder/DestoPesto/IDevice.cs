using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DestoPesto
{
    /// <MetaDataID>{48d05848-b872-46b6-9b65-5a423dc01fab}</MetaDataID>
    public interface IDevice
    {
        //  Task<string> SendMessage(string xmlMessage);

        bool isGPSEnabled();

        Task<PermissionStatus> RemoteNotificationsPermissionsRequest();

        
        Task<PermissionStatus> RemoteNotificationsPermissionsCheck();


        bool IsBackgroundServiceStarted { get; }
        

        bool RunInBackground(Action action, BackgroundServiceState serviceState);

        void StopBackgroundService();

        Task PermissionsGranted();

        string DeviceID { get; }

        double  GetOrientation(Stream stream);

        Task<FileResult> CapturePhotoAsync();

    }
}


