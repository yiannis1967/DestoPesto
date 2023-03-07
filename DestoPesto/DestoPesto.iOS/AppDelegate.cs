using System;
using System.Collections.Generic;
using System.Linq;
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
            global::Xamarin.Forms.Forms.Init();

            Maps.iOS.CustomMapRenderer.Init();
            Firebase.Core.App.Configure();
            Dictionary<string,object> m_options= new Dictionary<string,object>();
            foreach(var key in options.Keys)
            {
                m_options[key.ToString()]=options[key];
            }
            
            var formsApp = new App();
            formsApp.Options=m_options;

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


            string webClientID = "959003601559-v7ft2g3pr4augp8jus4k61bmoooe37h4.apps.googleusercontent.com";

            Authentication.iOS.Authentication.Init(webClientID);
            //"apps.googleusercontent.com.241222885422-bquei744e1i8q3h0r82k7fm31fbuej7m"


            //OOAdvantech.iOS.DeviceOOAdvantechCore.InitFirebase(fcmToken, webClientID);


            //OOAdvantech.iOS.DeviceOOAdvantechCore.SetFirebaseToken(fcmToken);



            // TODO: If necessary send token to application server.
            // Note: This callback is fired at each app startup and whenever a new token is generated.
        }

    }
}
