using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using DestoPesto.Models;
using DestoPesto.Services;
using PCLStorage;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace DestoPesto.Droid
{
    [Service(Label = "BackgroundService")]
    class BackgroundService : Service

    {
        BrodcastHandler reciever = new BrodcastHandler();

        List<Location> locationList = new List<Location>();


        bool Continue = true;
        public override IBinder OnBind(Intent intent)
        {

            throw new NotImplementedException();
        }
        public override void OnTaskRemoved(Intent rootIntent)
        {
            Intent intent = new Intent("com.android.ServiceStopped");
            SendBroadcast(intent);
        }

        public override void OnDestroy()

        {
            UnregisterReceiver(reciever);
            // int count =locationList.Count;


            t.Enabled = false;
            t.Stop();
            StopForeground(true);
            StopSelf();

            Continue = false;
            base.OnDestroy();
        }
        CancellationTokenSource cts;
        System.Collections.ObjectModel.ObservableCollection<DamageData> SubmittedDamage;
        System.Collections.ObjectModel.ObservableCollection<DamageData> SubmittedDamageUser;
        Location LastSubmittedLocation = new Location();
        bool a = false;

        async Task getLocation()
        {

            var locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            var locationPermisions = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (locationInUsePermisions != PermissionStatus.Granted)
            {
                return;
            }

            MessagingCenter.Unsubscribe<string>(this, "GetData");
            MessagingCenter.Subscribe<string>(this, "GetData", async (value) =>
            {
                if (value == "1")
                {
                    var req = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                    cts = new CancellationTokenSource();
                    var loc = await Geolocation.GetLocationAsync(req, cts.Token);
                    await JsonHandler.GetDamages(false, loc.Latitude, loc.Longitude, 20000.0);
                    LastSubmittedLocation = loc;
                    SubmittedDamage = JsonHandler.damageData;

                    //Call GetUserSubmission
                    await JsonHandler.GetDamages(true, loc.Latitude, loc.Longitude, 20000.0);
                    SubmittedDamageUser = JsonHandler.damageData;
                    MessagingCenter.Send<App, ObservableCollection<DamageData>>(App.Current as App, "LocList", SubmittedDamageUser);
                }
            }
            );
            try
            {


                //  if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {

                    //     return;

                }

                if (!a)
                {

                }
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
                {

                }

                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);
                if (location == null)
                {
                    location = await Geolocation.GetLastKnownLocationAsync();

                    if (location == null)
                    {
                        return;
                    }
                }
                else
                {


                }
                #region UpdateDamageData
                if (LastSubmittedLocation != null)
                {

                    double Distance = LocationExtensions.CalculateDistance(LastSubmittedLocation, location, DistanceUnits.Kilometers);
                    if (Distance > 20)
                    {
                        //Call GetSubmission
                        await JsonHandler.GetDamages(false, location.Latitude, location.Longitude, 20000.0);
                        LastSubmittedLocation = location;
                        SubmittedDamage = JsonHandler.damageData;

                        //Call GetUserSubmission
                        await JsonHandler.GetDamages(true, location.Latitude, location.Longitude, 20000.0);
                        SubmittedDamageUser = JsonHandler.damageData;
                        MessagingCenter.Send<App, ObservableCollection<DamageData>>(App.Current as App, "LocList", SubmittedDamageUser);

                    }

                }
                else
                {

                    //Call GetSubmission
                    await JsonHandler.GetDamages(false, location.Latitude, location.Longitude, 20000.0);
                    LastSubmittedLocation = location;
                    SubmittedDamage = JsonHandler.damageData;

                    //Call GetUserSubmission
                    await JsonHandler.GetDamages(true, location.Latitude, location.Longitude, 20000.0);
                    SubmittedDamageUser = JsonHandler.damageData;
                    MessagingCenter.Send<App, ObservableCollection<DamageData>>(App.Current as App, "LocList", SubmittedDamageUser);
                }
                #endregion


                if (SubmittedDamageUser != null)
                {
                    await GenerateLocalNotificationAsync(SubmittedDamageUser, location);
                }




                //    var locator = CrossGeolocator.Current;
                // locator.DesiredAccuracy = 25;

                //  var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

                //  locationList.Add(new Tuple<double, double>(position.Latitude, position.Longitude));
                //


            }
            catch
            {

            }
            //  Toast.MakeText(this, "Latitude: " +position.Latitude.ToString() + " Longitude: " + position.Longitude.ToString(),ToastLength.Short).Show();

        }

        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(Constants.ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        System.Timers.Timer t = new System.Timers.Timer();
        // This is any integer value unique to the application.
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        private void runAsForeground()
        {
            try
            {
                //NotificationChannel channel = new NotificationChannel("ServiceChannel", "Topos mou Tracking", NotificationImportance.Max);

                //NotificationManager manager = GetSystemService(Context.NotificationService) as NotificationManager;

                //manager.CreateNotificationChannel(channel);

                //Notification notification = new Notification.Builder(this, MainActivity.CHANNEL_ID)
                //        .SetOngoing(true)
                //        .SetContentTitle("toposmou")
                //        .SetContentText("gggggg")
                //        .Build();



                int notifyID = 1;
                string CHANNEL_ID = "my_channel_01";// The id of the channel. 
                                                    //CharSequence name = getString(R.string.channel_name);

                var notification = new Notification.Builder(this)
                    .SetContentTitle("toposmou")//Resources.GetString(Resource.String.app_name))
                    .SetContentText("gggggg") //Resources.GetString(Resource.String.notification_text))
                    .SetSmallIcon(Resource.Drawable.iconDark)
                    .SetChannelId(CHANNEL_ID)
                    .SetContentIntent(BuildIntentToShowMainActivity())
                    .SetOngoing(true)
                    //.AddAction(BuildRestartTimerAction())
                    //.AddAction(BuildStopServiceAction())
                    .Build();



                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);




            }
            catch (Exception ex)
            {

            }
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            RegisterReceiver(reciever, new IntentFilter("com.android.ServiceStopped"));
            locationList.Clear();

            t.Interval = 15000;
            t.Elapsed -= T_ElapsedAsync;
            t.Elapsed += T_ElapsedAsync;
            t.Enabled = true;

            //   Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            //   {
            // getLocation();
            runAsForeground();
            T_ElapsedAsync(null, null);

            //   });

            return base.OnStartCommand(intent, flags, startId);





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



        private async void T_ElapsedAsync(object sender, ElapsedEventArgs e)
        {

            await getLocation();

        }


        #region StoreLoaclNotificationTime

        public async Task CreateFolder()
        {
            try
            {
                IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;

                ExistenceCheckResult folderExist = await rootFolder.CheckExistsAsync("DataFolder");
                if (folderExist == ExistenceCheckResult.FolderExists)
                {
                    return;

                }
                else if (folderExist == ExistenceCheckResult.NotFound)
                {
                    IFolder folder = await rootFolder.CreateFolderAsync("DataFolder", CreationCollisionOption.OpenIfExists);

                }

                IFolder folder2 = await rootFolder.CreateFolderAsync("DataFolder",
                    CreationCollisionOption.OpenIfExists);
                ExistenceCheckResult fileExist = await folder2.CheckExistsAsync("LocalNotificationTime.txt");

                if (fileExist == ExistenceCheckResult.FileExists)
                {
                    return;
                }
                else if (fileExist == ExistenceCheckResult.NotFound)
                {

                    IFile file = await folder2.CreateFileAsync("LocalNotificationTime" + ".txt", CreationCollisionOption.ReplaceExisting);

                    string data = "";

                    await file.WriteAllTextAsync(data);

                }


            }
            catch (Exception e)
            {
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

                string data = DateTime.Now.ToString("yyyy|MM|dd|hh:mm:ss"+"!"+location.Latitude+"*"+location.Longitude);

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

        #endregion

    }

    public static class Constants
    {

        public const int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        public const string SERVICE_STARTED_KEY = "has_service_been_started";
        public const string BROADCAST_MESSAGE_KEY = "broadcast_message";
        public const string NOTIFICATION_BROADCAST_ACTION = "DestoPesto.Notification.Action";

        public const string ACTION_START_SERVICE = "DestoPesto.action.START_SERVICE";
        public const string ACTION_STOP_SERVICE = "DestoPesto.action.STOP_SERVICE";
        public const string ACTION_RESTART_TIMER = "DestoPesto.action.RESTART_TIMER";
        public const string ACTION_MAIN_ACTIVITY = "DestoPesto.action.MAIN_ACTIVITY";

    }
}