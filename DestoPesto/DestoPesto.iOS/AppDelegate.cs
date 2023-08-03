using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BackgroundTasks;
using DestoPesto.Services;
using Firebase.CloudMessaging;
using Foundation;
using UIKit;
using UserNotifications;

namespace DestoPesto.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            Facebook.CoreKit.ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);

            Maps.iOS.CustomMapRenderer.Init();
            Firebase.Core.App.Configure();



            Dictionary<string, object> m_options = new Dictionary<string, object>();
            //if (options != null)
            //{
            //    foreach (var key in options.Keys)
            //    {
            //        m_options[key.ToString()] = options[key];
            //    }
            //}

            string text = DeviceApplication.Current.ReadLog();
            System.Diagnostics.Debug.Write(text);
             

            var formsApp = new App("");
            formsApp.Options = m_options;
            BGTaskScheduler.Shared.Register(UploadTaskId, null, task => HandleUpload(task as BGAppRefreshTask));

            LoadApplication(formsApp);
            RegisterForRemoteNotifications();
            return base.FinishedLaunching(app, options);
        }

        private void RegisterForRemoteNotifications()
        {
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                {
                    Console.WriteLine(granted);
                });

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                // For iOS 10 data message (sent via FCM)
                Messaging.SharedInstance.Delegate = this;
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Firebase.CloudMessaging.Messaging messaging, string fcmToken)
        {
            var esnn = Firebase.Core.App.DefaultInstance;
            Console.WriteLine($"Firebase registration token: {fcmToken}");
            if (App.Current is App)
                (App.Current as App).FirbaseMessgesToken = fcmToken;


            string webClientID = "959003601559-v7ft2g3pr4augp8jus4k61bmoooe37h4.apps.googleusercontent.com";

            Authentication.iOS.Authentication.Init(webClientID);
            //"apps.googleusercontent.com.241222885422-bquei744e1i8q3h0r82k7fm31fbuej7m"


            //OOAdvantech.iOS.DeviceOOAdvantechCore.InitFirebase(fcmToken, webClientID);


            //OOAdvantech.iOS.DeviceOOAdvantechCore.SetFirebaseToken(fcmToken);



            // TODO: If necessary send token to application server.
            // Note: This callback is fired at each app startup and whenever a new token is generated.
        }
        public static string UploadTaskId { get; } = "com.arion.destopesto.ios.upload";
        public static NSString RefreshSuccessNotificationName { get; } = new NSString($"{UploadTaskId}.success");


        public override void DidEnterBackground(UIApplication application)
        {
            if (JsonHandler.HasTripDamages())
                ScheduleUploadTask();
        }

        void HandleUpload(BGAppRefreshTask task)
        {
            DeviceApplication.Current.Log(new List<string> { "HandleUpload" });
            if (JsonHandler.HasTripDamages())
                ScheduleUploadTask();

            //JsonHandler.SubmitNextTripDamage();

            //task.ExpirationHandler = () => operations.CancelOperations();

            //operations.FetchLatestPosts(task);
        }


        void ScheduleUploadTask()
        {
            DeviceApplication.Current.Log(new List<string> { "ScheduleUploadTask" });
            NSNotificationCenter.DefaultCenter.AddObserver(RefreshSuccessNotificationName, UploadSuccess);

            var request = new BGAppRefreshTaskRequest(UploadTaskId)
            {
                EarliestBeginDate = (NSDate)DateTime.Now.AddMinutes(10) // Fetch no earlier than 15 minutes from now
            };

            BGTaskScheduler.Shared.Submit(request, out NSError error);

            if (error != null)
                Debug.WriteLine($"Could not schedule app refresh: {error}");
        }
        void UploadSuccess(NSNotification notification)
        {
            NSNotificationCenter.DefaultCenter.RemoveObserver(RefreshSuccessNotificationName);
            var task = notification.Object as BGAppRefreshTask;
            task?.SetTaskCompleted(true);
        }

        public override void WillTerminate(UIApplication uiApplication)
        {
            var notification = new UILocalNotification();

            // set the fire date (the date time in which it will fire)
            notification.FireDate = NSDate.FromTimeIntervalSinceNow(5);

            // configure the alert
            notification.AlertAction = "View Alert";
            notification.AlertBody = "Your one minute alert has fired!";

            // modify the badge
            notification.ApplicationIconBadgeNumber = 1;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            // schedule it
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);




            base.WillTerminate(uiApplication);
        }

    }


    public class DeviceApplication
    {



        static DeviceApplication _Current = new DeviceApplication();
        public static DeviceApplication Current
        {
            get
            {
                return _Current;
            }
        }

        List<string> CachedLines = new List<string>();


        public void Log(List<string> lines)
        {
            foreach (var line in lines.ToList())
            {
                var index = lines.IndexOf(line);
                lines[index]=DateTime.Now.ToString()+" : "+line;
            }
            lock (this)
            {

                CachedLines.AddRange(lines);
                int count = 5;
                //do
                //{
                try
                {

                    const string errorFileName = "Common.log";
                    var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                    var errorFilePath = Path.Combine(libraryPath, errorFileName);
                    File.AppendAllLines(errorFilePath, CachedLines);

                    var liness = File.ReadAllLines(errorFilePath);
                    CachedLines.Clear();
                    return;
                }
                catch (Exception error)
                {
                    // System.Threading.Thread.Sleep(200);

                }
                //    count--;
                //} while (count>0);     
            }
        }

        public string ReadLog()
        {
            lock (this)
            {
                const string errorFileName = "Common.log";
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                if (File.Exists(errorFilePath))
                    return File.ReadAllText(errorFilePath);
                else
                    return "";
            }
        }

        public void ClearLog()
        {
            lock (this)
            {
                const string errorFileName = "Common.log";
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = Path.Combine(libraryPath, errorFileName);
                File.Delete(errorFilePath);
            }
        }

    }
}
