using DestoPesto.Views;
using DestoPesto.Services;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DestoPesto.Models;
using Authentication;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using Rg.Plugins.Popup.Services;

namespace DestoPesto
{
    public partial class App : Application
    {
        private AuthUser user;

        public static Dictionary<string, string> IntentExtras { get; set; }

        public static DateTime StartTime { get; set; }

        public UserSubmissions UserSubmissions { get; set; } = new UserSubmissions();

        public App(string appVersion)
        {
            InitializeComponent();



            //DependencyService.Register<MockDataStore>();
            Authentication.DeviceAuthentication.AuthStateChanged += DeviceAuthentication_AuthStateChanged;
            //var locationInUsePermisionstask = Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            //locationInUsePermisionstask.Wait();
            //var locationInUsePermisions = locationInUsePermisionstask.Result;

            //var cameraInUsePermisionstask = Permissions.CheckStatusAsync<Permissions.Camera>();
            //cameraInUsePermisionstask.Wait();
            //var cameraInUsePermisions = cameraInUsePermisionstask.Result;
            var appShell = new AppShell();


            appShell.CurrentItem = appShell.Items[0]; // main paige
            //////appShell.CurrentItem = appShell.Items[2]; // login paige

            if (Authentication.DeviceAuthentication.AuthUser == null && (!Application.Current.Properties.ContainsKey("user_id") || string.IsNullOrWhiteSpace(Application.Current.Properties["user_id"] as string)))
            {
                appShell.CurrentItem = appShell.Items[1]; // login paige
            }
            else
                appShell.LoginItem.IsVisible = false;

            appShell.Navigating += AppShell_Navigating;

            MainPage = appShell;
            //MainPage = new MainPage_s();


        }

        private void AppShell_Navigating(object sender, ShellNavigatingEventArgs e)
        {

        }

        private async void DeviceAuthentication_AuthStateChanged(object sender, Authentication.AuthUser e)
        {

            
            if (Authentication.DeviceAuthentication.AuthUser != null)
            {
                user = Authentication.DeviceAuthentication.AuthUser;
                if (Shell.Current?.CurrentItem == Shell.Current?.Items[1]) // login paige
                {
                    if (MainPage is AppShell)
                    {
                        Shell.Current.CurrentItem = Shell.Current.Items[0];
                        (Shell.Current as AppShell).LoginItem.IsVisible = false;



                    }

                }

                Application.Current.Properties["user_id"] = Authentication.DeviceAuthentication.AuthUser.User_ID;
                await Application.Current.SavePropertiesAsync();
                //var device = Xamarin.Forms.DependencyService.Get<IDevice>();

                Task.Run(() =>
                {
                    JsonHandler.SignIn(_FirbaseMessgesToken, true);
                });




            }
            else
            {
                if (user != null)
                {
                    Xamarin.Forms.Application.Current.Properties["user_id"] = "";
                }
            }

            //GetLocation();

        }

        string _FirbaseMessgesToken;
        public string FirbaseMessgesToken
        {
            get => _FirbaseMessgesToken;
            set
            {
                _FirbaseMessgesToken = value;
            }
        }

        public Dictionary<string, object> Options { get; set; }

        protected override void OnStart()
        {

            //GetLocation();

        }


        protected override void OnSleep()
        {
            var tt = Application.Current.Properties;

            System.Diagnostics.Debug.WriteLine(tt.Count);
        }

        protected override void OnResume()
        {
            //GetLocation();
        }

        public void StartFGService()
        {
            JsonHandler.SubmitTripFilesTask();
        }
        internal System.Collections.ObjectModel.ObservableCollection<DamageData> SubmittedDamage;
        internal System.Collections.ObjectModel.ObservableCollection<DamageData> SubmittedDamageUser;

        internal void RemoveUserSubmittedDamage(DamageData damageData)
        {
            SubmittedDamageUser.Remove(damageData);

            OnPropertyChanged(nameof(SubmittedDamageUser));
        }

        CancellationTokenSource cts;
        Location LastSubmittedLocation = new Location();
        //        public async Task GetLocation()
        //        {

        //            if (!DependencyService.Get<IDevice>().isGPSEnabled())
        //                return;

        //            if (MainPage == null)
        //                return;

        //            var locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        //            if (locationInUsePermisions != PermissionStatus.Granted)
        //                return;



        //            cts = new CancellationTokenSource();
        //            Location location=new Location();
        //            try
        //            {
        //                location = await Geolocation.GetLocationAsync();
        //                if (location == null)
        //                {
        //                    location = await Geolocation.GetLastKnownLocationAsync();
        //                    if (location == null)
        //                        return;
        //                }
        //            }
        //            catch ( Exception error)
        //            {


        //            }

        //            await Task.Run(async () =>
        //            {

        //                try
        //                {



        //#if DEBUG
        //                    location = new Location(37.933357, 23.534217);
        //#endif

        //                    #region UpdateDamageData
        //                    if (LastSubmittedLocation == null)
        //                        LastSubmittedLocation = new Location();

        //                    double Distance = LocationExtensions.CalculateDistance(LastSubmittedLocation, location, DistanceUnits.Kilometers);
        //                    if (Distance > 40)
        //                    {
        //                        do
        //                        {
        //                            try
        //                            {
        //                                if (SubmittedDamage == null)
        //                                    SubmittedDamage = await JsonHandler.GetDamages(false, location.Latitude, location.Longitude, 500);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                            }
        //                            LastSubmittedLocation = location;
        //                            try
        //                            {
        //                                if (SubmittedDamageUser == null)
        //                                    SubmittedDamageUser = await JsonHandler.GetDamages(true, location.Latitude, location.Longitude, 500);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                            } 
        //                            System.Threading.Thread.Sleep(5000);

        //                        } while (SubmittedDamage == null);
        //                    }


        //                    #endregion
        //                }
        //                catch (Exception errr)
        //                {
        //                }



        //            });

        //        }


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
            if (string.IsNullOrWhiteSpace(submisionThumb))
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


    }
}
