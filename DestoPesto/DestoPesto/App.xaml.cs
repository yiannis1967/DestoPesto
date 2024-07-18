using DestoPesto.Models;
using DestoPesto.Services;
using DestoPesto.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Globalization;
using PCLStorage;
using Rg.Plugins.Popup.Services;
using System.Reflection;
using LocalNotifications;
using System.ComponentModel;
using Authentication;

namespace DestoPesto
{
    /// <MetaDataID>{fd109209-cefc-4ba1-b347-41e4cec0d7e3}</MetaDataID>
    public partial class App : Application, INotifyPropertyChanged
    {
        internal string Version;

        public App(string version)
        {
            //123uu
            InitializeComponent();
            Version = version;
            DependencyService.Register<MockDataStore>();
            Authentication.DeviceAuthentication.AuthStateChanged += DeviceAuthentication_AuthStateChanged;
            var locationInUsePermisionstask = Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            locationInUsePermisionstask.Wait();
            var locationInUsePermisions = locationInUsePermisionstask.Result;

            var cameraInUsePermisionstask = Permissions.CheckStatusAsync<Permissions.Camera>();
            cameraInUsePermisionstask.Wait();
            var cameraInUsePermisions = cameraInUsePermisionstask.Result;




            MainPage = new AppShell();



            //DestoPesto.Images.iconSplash.png
            var names = typeof(App).Assembly.GetManifestResourceNames();
        }

        AuthUser user = null;


        private async void DeviceAuthentication_AuthStateChanged(object sender, Authentication.AuthUser e)
        {



            if (Authentication.DeviceAuthentication.AuthUser != null)
            {
                user = Authentication.DeviceAuthentication.AuthUser;
                var device = Xamarin.Forms.DependencyService.Get<IDevice>();
                
                if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Granted)
                    JsonHandler.SignIn(_FirbaseMessgesToken,true);
                else
                    JsonHandler.SignIn(_FirbaseMessgesToken, true);


                Xamarin.Forms.Application.Current.Properties["user_id"] = Authentication.DeviceAuthentication.AuthUser.User_ID;
            }
            else
            {
                if (user != null)
                {
                    Xamarin.Forms.Application.Current.Properties["user_id"] = "";
                }
            }

            getLocation();
        }

        CancellationTokenSource cts;
        Location LastSubmittedLocation = new Location();
        internal System.Collections.ObjectModel.ObservableCollection<DamageData> SubmittedDamage;
        internal System.Collections.ObjectModel.ObservableCollection<DamageData> SubmittedDamageUser;

        static public bool ShowAll = false;

        public async Task getLocation()
        {

            if (!DependencyService.Get<IDevice>().isGPSEnabled())
            {
                //    await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ExitText, DestoPesto.Properties.Resources.TokenExpiredText, DestoPesto.Properties.Resources.Oktext);
                return;
            }



            var locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            MessagingCenter.Unsubscribe<string>(this, "GetData");
            MessagingCenter.Subscribe<string>(this, "GetData", async (value) =>
            {
                if (value == "1")
                {
                    locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    //else

                    if (locationInUsePermisions != PermissionStatus.Granted)
                        return;

                    var req = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                    cts = new CancellationTokenSource();
                    var loc = await Geolocation.GetLocationAsync(req, cts.Token);
                    try
                    {


                        if (SubmittedDamage == null)
                            SubmittedDamage = await JsonHandler.GetDamages(false, loc.Latitude, loc.Longitude, 20000.0);
                        LastSubmittedLocation = loc;

                    }
                    catch (Exception ex)
                    {

                    }
                    //SubmittedDamage = JsonHandler.damageData;

                    //Call GetUserSubmission
                    try
                    {
                        if (SubmittedDamageUser == null)
                            SubmittedDamageUser = await JsonHandler.GetDamages(true, loc.Latitude, loc.Longitude, 20000.0);
                    }
                    catch (Exception ex)
                    {

                    }
                    //SubmittedDamageUser = JsonHandler.damageData;
                    MessagingCenter.Send<App, ObservableCollection<DamageData>>(App.Current as App, "LocList", SubmittedDamageUser);
                }
            });

            if (MainPage == null)
                return;

            locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            //else

            if (locationInUsePermisions != PermissionStatus.Granted)
                return;



            try
            {






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
                        try
                        {
                            if (SubmittedDamage == null)
                                SubmittedDamage = await JsonHandler.GetDamages(false, location.Latitude, location.Longitude, 481.0);
                        }
                        catch (Exception ex)
                        {

                        }
                        LastSubmittedLocation = location;
                        //SubmittedDamage = JsonHandler.damageData;

                        //Call GetUserSubmission
                        try
                        {
                            if (SubmittedDamageUser == null)
                                SubmittedDamageUser = await JsonHandler.GetDamages(true, location.Latitude, location.Longitude, 481.0);
                        }
                        catch (Exception ex)
                        {

                        }
                        //                        SubmittedDamageUser = JsonHandler.damageData;
                        MessagingCenter.Send<App, ObservableCollection<DamageData>>(App.Current as App, "LocList", SubmittedDamageUser);

                    }

                }
                else
                {

                    //Call GetSubmission
                    try
                    {

                        SubmittedDamage = await JsonHandler.GetDamages(false, location.Latitude, location.Longitude, 20000.0);
                    }
                    catch (Exception ex)
                    {

                    }
                    LastSubmittedLocation = location;
                    SubmittedDamage = JsonHandler.damageData;

                    //Call GetUserSubmission
                    try
                    {

                        SubmittedDamageUser = await JsonHandler.GetDamages(true, location.Latitude, location.Longitude, 20000.0);
                    }
                    catch (Exception ex)
                    {

                    }
                    //SubmittedDamageUser = JsonHandler.damageData;
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
            catch (Exception errr)
            {

            }
            //  Toast.MakeText(this, "Latitude: " +position.Latitude.ToString() + " Longitude: " + position.Longitude.ToString(),ToastLength.Short).Show();

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

                                JsonHandler.ShowNotification(DestoPesto.Properties.Resources.LocalNotificationTitle, DestoPesto.Properties.Resources.LocalNotificationDec);





                                return;


                            }
                        }
                        else
                        {
                            JsonHandler.ShowNotification(DestoPesto.Properties.Resources.LocalNotificationTitle, DestoPesto.Properties.Resources.LocalNotificationDec);
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

        public bool LocationPermisionsChecked { get; private set; }
        public static Dictionary<string, string> IntentExtras { get; set; }
        public Dictionary<string, object> Options { get; set; }


        string _FirbaseMessgesToken;
        public string FirbaseMessgesToken
        {
            get => _FirbaseMessgesToken;
            set
            {
                _FirbaseMessgesToken = value;
       

            }
        }

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

        bool Initialezed;
        protected override async void OnStart()
        {

            var uri = JsonHandler.getUri();
            var device = Xamarin.Forms.DependencyService.Get<IDevice>();
            var RemoteNotificationsPermissionsTask = device.RemoteNotificationsPermissionsCheck();
            RemoteNotificationsPermissionsTask.Wait();
            var RemoteNotificationsPermissions = RemoteNotificationsPermissionsTask.Result;
            if (RemoteNotificationsPermissions == PermissionStatus.Granted)
            {
                if (!Initialezed)
                    await device.PermissionsGranted();
                Initialezed = true;
            }

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public void StartFGService()
        {
            JsonHandler.SubmitTripFilesTask();
        }

        public async void DispayMessage(IDictionary<string, string> data)
        {


            string description;
            data.TryGetValue("Description", out description);

            string messageID;
            data.TryGetValue("MessageID", out messageID);
            if (messageID != null && messageID.IndexOf("Contest_") == 0)
                return;

            string submisionThumb;
            if (!data.ContainsKey("SubmisionThumb"))
                return;

            data.TryGetValue("SubmisionThumb", out submisionThumb);
            if(string.IsNullOrWhiteSpace(submisionThumb)) 
                return;

            string comments;
            data.TryGetValue("Comments", out comments);

            await PopupNavigation.Instance.PushAsync(new SubmisionPopupPage(description, submisionThumb, comments));


            try
            {
                try
                {
                    var location = await Geolocation.GetLastKnownLocationAsync();
                    //if (SubmittedDamageUser == null)
                    SubmittedDamageUser = await JsonHandler.GetDamages(true, location.Latitude, location.Longitude, 20000.0);
                }
                catch (Exception ex)
                {
                }
                MessagingCenter.Send<App, ObservableCollection<DamageData>>(App.Current as App, "LocList", SubmittedDamageUser);

            }
            catch (Exception error)
            {

            }


        }

        internal void RemoveUserSubmittedDamage(DamageData damageData)
        {
            SubmittedDamageUser.Remove(damageData);

            OnPropertyChanged(nameof(SubmittedDamageUser));
        }
    }
}
