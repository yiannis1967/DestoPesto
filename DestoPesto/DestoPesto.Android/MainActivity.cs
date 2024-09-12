using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Facebook;
using Android.Gms.Common;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading;
using Android.Content;
using System.Threading.Tasks;
using DestoPesto.Services;
using DestoPesto.Models;
using System.Collections.ObjectModel;
using AndroidX.Core.Content;
using System.Collections.Generic;
using PCLStorage;
using System.Globalization;
using Java.Security;
using Android.Util;
using static Android.Content.PM.PackageManager;
using Firebase.Iid;
using LocalNotifications;
using Firebase.Messaging;
using System.Linq;

namespace DestoPesto.Droid
{
    
    [Activity(Label = "Δες το, Πες το", Icon = "@drawable/ic_launcher", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.Gms.Tasks.IOnSuccessListener
    {
        public MainActivity()
        {

        }
        static MainActivity()
        {
            App.StartTime = DateTime.UtcNow;
        }
        static ICallbackManager CallbackManager;
        FirebaseAuthEvents FirebaseAuthEvents = new FirebaseAuthEvents();
        MyAccessTokenTracker myAccessTokenTracker;
        App App;
        protected override async void OnCreate(Bundle savedInstanceState)
        {



            base.OnCreate(savedInstanceState);

            try
            {

                //  var sds = LogDebug.Current.ReadLog();

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
                Xamarin.FormsMaps.Init(this, savedInstanceState);




                //LogDebug.Current.Log(new List<string>() { "Step 1"});

                Rg.Plugins.Popup.Popup.Init(this);

                App = new App("");
                LoadApplication(App);
                
                DeviceCore.MainActivity = this;

                Task.Run(() =>
                {
                    string webClientID = "959003601559-9ljm2ph745o1s8h78p1luiq3fioham75.apps.googleusercontent.com";

                    Authentication.Android.FirebaseAuthentication.Init(this, webClientID);
                    FirebaseMessaging.Instance?.GetToken()?.AddOnSuccessListener(this, this);
                    DeviceCore.ForegroundServiceManager = new Droid.MyForeGroundService();

                    if (Intent?.Extras?.KeySet() != null)
                    {
                        if (App.IntentExtras == null)
                            App.IntentExtras = new Dictionary<string, string>();
                        foreach (var key in Intent.Extras.KeySet())
                        {
                            var value = Intent.Extras.GetString(key);
                            App.IntentExtras[key] = value;
                        }
                    }

                });


                

                //LogDebug.Current.Log(new List<string>() { "Step 2" });


                
                //PrintHashKey(this);




                //string AppVersion = Properties.Resources.Version + " " + Android.App.Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Android.App.Application.Context.ApplicationContext.PackageName, 0).VersionCode.ToString();

                //App = new App(AppVersion);


                //LogDebug.Current.Log(new List<string>() { "Step 3" });
               


                // LogDebug.Current.Log(new List<string>() { "Step 4" });

              
                //InitAfterPermissionsRequest();//  LogDebug.Current.Log(new List<string>() { "Step 5" });
                //var token = await Task<string>.Run(() =>
                //{
                //    return FirebaseInstanceId.Instance.GetToken("959003601559", "FCM");
                //});

                


                //MessagingCenter.Unsubscribe<string>(this, "backgroundService");
                //MessagingCenter.Subscribe<string>(this, "backgroundService", async (value) =>
                //{
                //    if (value == "1")
                //    {
                //        var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                //        CancellationTokenSource cts = new CancellationTokenSource();
                //        var location = await Geolocation.GetLocationAsync(request, cts.Token);
                //        StartService(new Intent(this, typeof(BackgroundService)));
                //    }
                //    else
                //    {
                //        StopService(new Intent(this, typeof(BackgroundService)));
                //    }
                //}
                //);

                ////////////Droid.ForeGroundService.ServiceState serviceState = new LocationService.ServiceState()
                ////////////{
                ////////////    NotificationTitle = "Δέστω πέστω",
                ////////////    NotificationContentText = "The started service is running.",
                ////////////    NotificationSmallIcon = Resource.Drawable.icon,
                ////////////    StopServiceCommandTitle = "Stop Service",
                ////////////    StopServiceCommandIcon = Resource.Drawable.icon,
                ////////////    StopActionID = "DestoPesto.action.STOP_SERVICE",
                ////////////    DelayBetweenLogMessage =  5000, // milliseconds,
                ////////////    ServiceRunningNotificationID = 10000,
                ////////////    ActionsMainActivity = "DestoPesto.action.MAIN_ACTIVITY",
                ////////////    ServiceStartedKey = "has_service_been_started",
                ////////////    Terminate = false,

                ////////////};
                // serviceState.Runnable = new Action(async () =>
                //{
                //    do
                //    {
                //        await App.getLocation();
                //        System.Threading.Thread.Sleep(15000);

                //    } while (!serviceState.Terminate);
                //});



                //////////new Droid.LocationService().StartForegroundService(this, Constants.ACTION_START_SERVICE, serviceState);

               // LoadApplication(App);


            }
            catch (Exception error)
            {
                // LogDebug.Current.Log(new List<string>() { error.Message, error.StackTrace });
            }
        }

        public void InitAfterPermissionsGranted()
        {
            IsPlayServicesAvailable();
            CreateNotificationChannel();
            App.StartFGService();
            CreateNotificationFromIntent(Intent);
        }

        public static Activity mainLauncher = null;
        protected override void OnNewIntent(Intent intent)
        {
            CreateNotificationFromIntent(intent);
        }
        protected override void OnStart()
        {
            try
            {
                
                base.OnStart();

            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine(error.StackTrace);
                
            }



        }


        void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.GetStringExtra(LocalNotifications.Droid.AndroidNotificationManager.TitleKey);
                string message = intent.GetStringExtra(LocalNotifications.Droid.AndroidNotificationManager.MessageKey);

                DependencyService.Get<INotificationManager>().ReceiveNotification(title, message);
            }
        }


        public void OnSuccess(Java.Lang.Object result)
        {
            string token = result.ToString();
            if (App.Current is App)
                (App.Current as App).FirbaseMessgesToken = token;


        }

        //CancellationTokenSource cts;
        //Location LastSubmittedLocation = new Location();
        //System.Collections.ObjectModel.ObservableCollection<DamageData> SubmittedDamage;
        //System.Collections.ObjectModel.ObservableCollection<DamageData> SubmittedDamageUser;
        //async Task getLocation()
        //{
        //    MessagingCenter.Unsubscribe<string>(this, "GetData");
        //    MessagingCenter.Subscribe<string>(this, "GetData", async (value) =>
        //    {
        //        if (value == "1")
        //        {
        //            var req = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
        //            cts = new CancellationTokenSource();
        //            var loc = await Geolocation.GetLocationAsync(req, cts.Token);
        //            await JsonHandler.GetDamages(false, loc.Latitude, loc.Longitude, 20000.0);
        //            LastSubmittedLocation = loc;
        //            SubmittedDamage = JsonHandler.damageData;

        //            //Call GetUserSubmission
        //            await JsonHandler.GetDamages(true, loc.Latitude, loc.Longitude, 20000.0);
        //            SubmittedDamageUser = JsonHandler.damageData;
        //            MessagingCenter.Send<App, ObservableCollection<DamageData>>(App.Current as App, "LocList", SubmittedDamage);
        //        }
        //    });
        //    try
        //    {


        //        //  if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        //        {

        //            //     return;

        //        }


        //        //if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.AccessFineLocation) == Permission.Granted)
        //        //{

        //        //}

        //        var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
        //        cts = new CancellationTokenSource();
        //        var location = await Geolocation.GetLocationAsync(request, cts.Token);
        //        if (location == null)
        //        {
        //            location = await Geolocation.GetLastKnownLocationAsync();

        //            if (location == null)
        //            {
        //                return;
        //            }
        //        }
        //        else
        //        {


        //        }
        //        #region UpdateDamageData
        //        if (LastSubmittedLocation != null)
        //        {

        //            double Distance = LocationExtensions.CalculateDistance(LastSubmittedLocation, location, DistanceUnits.Kilometers);
        //            if (Distance > 20)
        //            {
        //                //Call GetSubmission
        //                await JsonHandler.GetDamages(false, location.Latitude, location.Longitude, 20000.0);
        //                LastSubmittedLocation = location;
        //                SubmittedDamage = JsonHandler.damageData;

        //                //Call GetUserSubmission
        //                await JsonHandler.GetDamages(true, location.Latitude, location.Longitude, 20000.0);
        //                SubmittedDamageUser = JsonHandler.damageData;
        //                MessagingCenter.Send<App, ObservableCollection<DamageData>>(App.Current as App, "LocList", SubmittedDamage);

        //            }

        //        }
        //        else
        //        {

        //            //Call GetSubmission
        //            await JsonHandler.GetDamages(false, location.Latitude, location.Longitude, 20000.0);
        //            LastSubmittedLocation = location;
        //            SubmittedDamage = JsonHandler.damageData;

        //            //Call GetUserSubmission
        //            await JsonHandler.GetDamages(true, location.Latitude, location.Longitude, 20000.0);
        //            SubmittedDamageUser = JsonHandler.damageData;
        //            MessagingCenter.Send<App, ObservableCollection<DamageData>>(App.Current as App, "LocList", SubmittedDamage);
        //        }
        //        #endregion


        //        if (SubmittedDamageUser != null)
        //        {
        //            await GenerateLocalNotificationAsync(SubmittedDamageUser, location);
        //        }




        //        //    var locator = CrossGeolocator.Current;
        //        // locator.DesiredAccuracy = 25;

        //        //  var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

        //        //  locationList.Add(new Tuple<double, double>(position.Latitude, position.Longitude));
        //        //


        //    }
        //    catch
        //    {

        //    }
        //    //  Toast.MakeText(this, "Latitude: " +position.Latitude.ToString() + " Longitude: " + position.Longitude.ToString(),ToastLength.Short).Show();

        //}


        string msgText;
        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))

                    msgText = GoogleApiAvailability.Instance.GetErrorString(resultCode);
                else
                {
                    msgText = "This device is not supported";
                    Finish();
                }
                return false;
            }
            else
            {
                msgText = "Google Play Services is available.";
                return true;
            }
        }

        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;
        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID,
                                                  "FCM Notifications",
                                                  NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
        private async Task GenerateLocalNotificationAsync(System.Collections.ObjectModel.ObservableCollection<DamageData> data, Location _location)
        {

            List<Location> _locList = JsonHandler.getLocationsFromReports(data);

            for (int i = 0; i < _locList.Count; i++)
            {
                double Distance = LocationExtensions.CalculateDistance(_locList[i], _location, DistanceUnits.Kilometers);
                if (Distance < 5)
                {
                    try
                    {
                        await ReadTripFile("LocalNotificationTime.txt");
                        if (FileData != "")
                        {
                            var Data = FileData.Split('!');
                            var loc = Data[1].Split('*');
                            double latitude = Convert.ToDouble(loc[0]);
                            double longitude = Convert.ToDouble(loc[1]);

                            DateTime t1 = DateTime.ParseExact(Data[0], "yyyy|MM|dd|hh:mm:ss", CultureInfo.InvariantCulture);


                            string dt = DateTime.Now.ToString("yyyy|MM|dd|hh:mm:ss");

                            DateTime t2 = DateTime.ParseExact(dt, "yyyy|MM|dd|hh:mm:ss", CultureInfo.InvariantCulture);

                            TimeSpan ts = t2.TimeOfDay - t1.TimeOfDay;
                            double Dis = LocationExtensions.CalculateDistance(new Location(latitude, longitude), _location, DistanceUnits.Kilometers);
                            if (ts.TotalHours > 24 || (Dis > 20))
                            {
                                await BuildTripFile(_location);
                                //Generate Local Notification

                                JsonHandler.ShowNotification(Properties.Resources.LocalNotificationTitle, Properties.Resources.LocalNotificationDec);

                                return;


                            }
                        }
                        else
                        {
                            JsonHandler.ShowNotification(Properties.Resources.LocalNotificationTitle, Properties.Resources.LocalNotificationDec);
                            await BuildTripFile(_location);
                            return;
                        }
                    }
                    catch (Exception ex)

                    {
                    }

                }
            }
        }


        public async Task BuildTripFile(Location location)
        {
            try
            {
                IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;

                IFolder folder = await rootFolder.CreateFolderAsync("DataFolder",
                    CreationCollisionOption.OpenIfExists);
                //    IFile file = await folder.CreateFileAsync(DateTime.Now.ToString("yyyy|MM|dd|hh:mm:ss") + ".txt", CreationCollisionOption.ReplaceExisting);

                IFile file = await folder.CreateFileAsync("LocalNotificationTime" + ".txt", CreationCollisionOption.ReplaceExisting);

                string data = DateTime.Now.ToString("yyyy|MM|dd|hh:mm:ss" + "!" + location.Latitude + "*" + location.Longitude);

                await file.WriteAllTextAsync(data);
            }
            catch (Exception e)

            {
                String a = e.Message;
            }
        }
        public string FileData = "";
        public async Task ReadTripFile(String FileName)
        {
            try
            {
                IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;

                IFolder folder = await rootFolder.CreateFolderAsync("DataFolder",
                    CreationCollisionOption.OpenIfExists);

                ExistenceCheckResult fileExist = await folder.CheckExistsAsync(FileName);

                if (fileExist == ExistenceCheckResult.FileExists)
                {
                    IFile file = await folder.GetFileAsync(FileName);

                    String text = await file.ReadAllTextAsync();

                    FileData = text;
                }
                else if (fileExist == ExistenceCheckResult.NotFound)
                {

                    IFile file = await folder.CreateFileAsync("LocalNotificationTime" + ".txt", CreationCollisionOption.ReplaceExisting);

                    string data = "";

                    await file.WriteAllTextAsync(data);

                }

            }
            catch (Exception e)

            {
                string a = e.Message;
            }
        }
     

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Authentication.Android.FirebaseAuthentication.OnActivityResult(requestCode, resultCode, data);


        }


        public static void PrintHashKey(ContextWrapper pContext)
        {

            try
            {


                PackageInfo info = pContext.PackageManager.GetPackageInfo(pContext.PackageName, Android.Content.PM.PackageInfoFlags.Signatures);

                foreach (var signature in info.Signatures)
                {
                    MessageDigest md5 = MessageDigest.GetInstance("MD5");
                    MessageDigest sha = MessageDigest.GetInstance("SHA");
                    MessageDigest sha256 = MessageDigest.GetInstance("SHA256");

                    sha.Update(signature.ToByteArray());
                    //Log.Debug("KeyHash:", Base64.EncodeToString(md.Digest(), Base64.Default));
                    var s = Base64.EncodeToString(sha.Digest(), Base64Flags.Default);
                }
            }
            catch (NameNotFoundException e)
            {

            }
            catch (NoSuchAlgorithmException e)
            {

            }

            //try
            //{
            //    PackageInfo info = pContext.getPackageManager().getPackageInfo(pContext.getPackageName(), PackageManager.GET_SIGNATURES);
            //    for (Signature signature : info.signatures)
            //    {
            //        MessageDigest md = MessageDigest.getInstance("SHA");
            //        md.update(signature.toByteArray());
            //        String hashKey = new String(Base64.encode(md.digest(), 0));
            //        Log.i(TAG, "printHashKey() Hash Key: " + hashKey);
            //    }
            //}
            //catch (NoSuchAlgorithmException e)
            //{
            //    Log.e(TAG, "printHashKey()", e);
            //}
            //catch (Exception e)
            //{
            //    Log.e(TAG, "printHashKey()", e);
            //}
        }

        internal const int requestCodeStart = 12000;

        static int requestCode = requestCodeStart;

        internal static int NextRequestCode()
        {
            if (++requestCode >= 12999)
                requestCode = requestCodeStart;

            return requestCode;
        }
        static readonly Dictionary<int, TaskCompletionSource<PermissionStatus>> requests =
                  new Dictionary<int, TaskCompletionSource<PermissionStatus>>();

        static readonly object locker = new object();
        internal Task<PermissionStatus> NotificationPermissionsRequest()
        {

            string[] notiPermission =
     {
                Android.Manifest.Permission.PostNotifications
            };

            //if ((int)Build.VERSION.SdkInt < 33)
            //    return Task.FromResult(PermissionStatus.Granted);
            
            if (CheckSelfPermission(Android.Manifest.Permission.PostNotifications) != Android.Content.PM.Permission.Granted)
            {

                TaskCompletionSource<PermissionStatus> tcs;

                int notificationRequestCode = 0;
                lock (locker)
                {
                    tcs = new TaskCompletionSource<PermissionStatus>();

                    notificationRequestCode = NextRequestCode();

                    requests.Add(notificationRequestCode, tcs);
                }

                if (!MainThread.IsMainThread)
                    throw new PermissionException("Permission request must be invoked on main thread.");

                RequestPermissions(notiPermission, notificationRequestCode);
                return tcs.Task;
                
            }
            else
                return Task.FromResult(PermissionStatus.Granted);


        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            if(permissions.Contains( Android.Manifest.Permission.PostNotifications))
            {
                lock (locker)
                {
                    if (requests.ContainsKey(requestCode))
                    {
                        if (grantResults.Any(g => g == Android.Content.PM.Permission.Denied))
                            requests[requestCode].TrySetResult(PermissionStatus.Denied);
                        else
                            requests[requestCode].TrySetResult(PermissionStatus.Granted);
                        
                        requests.Remove(requestCode);
                    }
                }
            }
            else
            {
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        //internal static void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        //{
        //    lock (locker)
        //    {
        //        if (requests.ContainsKey(requestCode))
        //        {
        //            var result = new PermissionResult(permissions, grantResults);
        //            requests[requestCode].TrySetResult(result);
        //            requests.Remove(requestCode);
        //        }
        //    }
        //}
    }
}