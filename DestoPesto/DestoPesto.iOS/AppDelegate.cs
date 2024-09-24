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
using ObjCRuntime;
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

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

           // Errorlog.Current.ClearLog();

        

            global::Rg.Plugins.Popup.Popup.Init();
            global::Xamarin.Forms.Forms.Init();
            Facebook.CoreKit.ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);

            Maps.iOS.CustomMapRenderer.Init();
            Firebase.Core.App.Configure();

            DeviceCore.AppDelegate = this;

            

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
            //RegisterForRemoteNotifications();
            

            formsApp.StartFGService();
            var result= base.FinishedLaunching(app, options);

            string webClientID = "959003601559-v7ft2g3pr4augp8jus4k61bmoooe37h4.apps.googleusercontent.com";
            if (!FirebaseAuthInitilized)
            {
                Authentication.iOS.Authentication.Init(webClientID);
                FirebaseAuthInitilized = true;
            }
            return result;



        }
        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            
            if( unobservedTaskExceptionEventArgs.Exception is Exception)
               Errorlog.Current.Log(new System.Collections.Generic.List<string>() { "Unobserved Task Exception:"+ (unobservedTaskExceptionEventArgs.Exception as Exception).Message, (unobservedTaskExceptionEventArgs.Exception as Exception).StackTrace });
            var error = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            Errorlog.Current.Log(new System.Collections.Generic.List<string>() { "Unobserved Task Exception:"+ error.Message, error.StackTrace });
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            if (unhandledExceptionEventArgs.ExceptionObject is Exception)
                Errorlog.Current.Log(new System.Collections.Generic.List<string>() { "Unobserved Task Exception:"+ (unhandledExceptionEventArgs.ExceptionObject as Exception).Message, (unhandledExceptionEventArgs.ExceptionObject as Exception).StackTrace });

            var error = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            Errorlog.Current.Log(new System.Collections.Generic.List<string>() { "Unhandled Exception:"+ error.Message, error.StackTrace });
        }

        bool RemoteNotificationsIsRegister = false;
        internal Task<bool> RegisterForRemoteNotifications()
        {

            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            if (RemoteNotificationsIsRegister)
                return Task<bool>.FromResult(true);

            RemoteNotificationsIsRegister = true;
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                // For iOS 10 data message (sent via FCM)
                Messaging.SharedInstance.Delegate = this;

                // iOS 10 or later
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                {

                    Console.WriteLine(granted);
                    if (granted)
                    {
                        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                        {
                            UIApplication.SharedApplication.RegisterForRemoteNotifications();

                            try
                            {
                                var fcmToken = Firebase.CloudMessaging.Messaging.SharedInstance.FcmToken;
                                if (App.Current is App)
                                    (App.Current as App).FirbaseMessgesToken = fcmToken;
                            }
                            catch (Exception errolr)
                            {

                            }

                            taskCompletionSource.SetResult(true);

                          
                        });
                        
                        
                    }
                    else
                        taskCompletionSource.SetResult(true);
                });
                
                return taskCompletionSource.Task;
                /*
                UNUserNotificationCenter.Current.GetNotificationSettings((UNNotificationSettings settings) =>
                {
                    if(settings.AlertSetting==UNNotificationSetting.Enabled)
                    {

                    }

                });*/


            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
                return Task<bool>.FromResult(true);
            }

            

            /*
            var fcmToken = Messaging.SharedInstance.FcmToken ?? "";
            if (!string.IsNullOrWhiteSpace(fcmToken))
            {


                if (App.Current is App)
                    (App.Current as App).FirbaseMessgesToken = fcmToken;


                string webClientID = "959003601559-v7ft2g3pr4augp8jus4k61bmoooe37h4.apps.googleusercontent.com";

                if (!FirebaseAuthInitilized)
                {
                    Authentication.iOS.Authentication.Init(webClientID);
                    FirebaseAuthInitilized = true;
                }
            }*/


        }


        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {


            try
            {

                Dictionary<string, string> messageProperties = userInfo.ToDictionary<KeyValuePair<NSObject, NSObject>, string, string>(item => item.Key as NSString, item => item.Value.ToString());
                (App.Current as App).DispayMessage(messageProperties);

            }
            catch (Exception ex)
            {

            }

            //base.DidReceiveRemoteNotification(application, userInfo, completionHandler);

                //caNxuyw3YE9TnnlUT3vJno:APA91bH1UndH5QUPnKVnyOBZDkd5VR3uRY5kGuaMzWnb78FksJBPC_Yu0j3sgSTqFZ3PS9_yb4Tq_YWy5g5hPVqBgzmNhUNI7nQO16ZUmvHuL3nb6eMdqvL-XNV7WktYBRw-amAwhCaB
        }

        bool FirebaseAuthInitilized = false;

        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Firebase.CloudMessaging.Messaging messaging, string fcmToken)
        {
            var esnn = Firebase.Core.App.DefaultInstance;
            Console.WriteLine($"Firebase registration token: {fcmToken}");
            if (App.Current is App)
                (App.Current as App).FirbaseMessgesToken = fcmToken;


         

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

            var device = Xamarin.Forms.DependencyService.Get<IDevice>();
            if (device.IsBackgroundServiceStarted)
            {
                var notification = new UILocalNotification();

                // set the fire date (the date time in which it will fire)
                notification.FireDate = NSDate.FromTimeIntervalSinceNow(3);

                // configure the alert
                notification.AlertAction = DestoPesto.Properties.Resources.AlertText;
                notification.AlertBody = DestoPesto.Properties.Resources.NotificationContentText;

                // modify the badge
                notification.ApplicationIconBadgeNumber = 1;

                // set the sound to be the default sound
                notification.SoundName = UILocalNotification.DefaultSoundName;

                // schedule it
                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                System.Threading.Thread.Sleep(1000);
            }

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
