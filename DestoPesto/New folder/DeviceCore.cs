
using CoreLocation;
using DestoPesto.iOS;
using Firebase.CloudMessaging;
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
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


        string _DeviceID;
        public string DeviceID
        {
            get
            {
                if (_DeviceID == null)
                    _DeviceID = GetDeviceUniqueID();
                return _DeviceID;
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
                    if (granted)
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


        public double GetOrientation(Stream stream)
        {
            var imageData = NSData.FromStream(stream);
            var uIImage = UIImage.LoadFromData(imageData);




            if (uIImage.Orientation == UIImageOrientation.Right)
                return 90;
            if (uIImage.Orientation == UIImageOrientation.Left)
                return -90;
            if (uIImage.Orientation == UIImageOrientation.Down)
                return 180;

            if (uIImage.Orientation == UIImageOrientation.Up)
                return 360;

            return 0;


        }
        public void StopBackgroundService()
        {

        }
        public static String GetDeviceUniqueID()
        {

            string id = UIKit.UIDevice.CurrentDevice?.IdentifierForVendor?.AsString();
            return id;

        }
        public async Task PermissionsGranted()
        {

            await DeviceCore.AppDelegate.RegisterForRemoteNotifications();

            return;


        }

        static internal string m_androidId;

        static internal string m_OldandroidId = "";






        private UIImagePickerController _picker;

        public async Task<FileResult> CapturePhotoAsync()
        {
            TaskCompletionSource<FileResult> taskCompletionSource = new TaskCompletionSource<FileResult>();

            if (await Permissions.RequestAsync<Permissions.Camera>() != PermissionStatus.Granted)
            {
                taskCompletionSource.TrySetException(new PermissionException("Camera permission not granted"));
                return await taskCompletionSource.Task;
            }

            //Create an image picker object
            _picker = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.Camera
            };

            //Make sure we can find the top most view controller to launch the camera
            UIViewController vc = Platform.GetCurrentUIViewController();

            vc.PresentViewController(_picker, true, null);
            _picker.FinishedPickingMedia += async (_, e) =>
            {
                // get the original image
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;

                string documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string jpgFilename = Path.Combine(documentsDirectory, $"{Guid.NewGuid()}.jpg");
                NSData imgData = originalImage.AsJPEG();

                await vc.DismissViewControllerAsync(true);

                if (imgData.Save(jpgFilename, false, out NSError err))
                {
                    taskCompletionSource.TrySetResult(new FileResult(jpgFilename));
                }
                else
                {
                    taskCompletionSource.TrySetException(new Exception("Unable to save the image" + err));
                }

                _picker.DismissViewController(true, null);
            };
            _picker.Canceled += async (_, e) =>
            {
                await vc.DismissViewControllerAsync(true);
                taskCompletionSource.TrySetResult(null);
                _picker?.Dispose();
                _picker = null;
            };

            return await taskCompletionSource.Task;
        }

        public bool isGPSEnabled()
        {
            if (CLLocationManager.Status == CLAuthorizationStatus.Denied)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private class CameraDelegate : UIImagePickerControllerDelegate
        {
            public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
            {
                picker.DismissModalViewController(true);
            }
        }
    }


}


