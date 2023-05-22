using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using DestoPesto.Services;
using System.Collections.Generic;
using DestoPesto.Models;
using System.Collections.ObjectModel;
using System.IO;

using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Windows.Input;
using Maps;
using Rg.Plugins.Popup.Services;
using System.Reflection;
using PCLStorage;
using System.Xml.Linq;
//using Rg.Plugins.Popup.Services;

namespace DestoPesto.Views
{
    public partial class AboutPage : ContentPage
    {
        //public TapGestureRecognizer location_tap = new TapGestureRecognizer();
        public string WebAPIkey = "AIzaSyCH9F_m6KO7_1BB3NN0eiSjN9_d99bRjsk";
        public ObservableCollection<DamageData> ReportedDamagePins = new ObservableCollection<DamageData>();
        String userEmail = "";
        String userId = "";
        String deviceId = "";
        List<CategoryButton> CategoryButtons = new List<CategoryButton>();
        public ICommand SettingCommand { protected set; get; }
        public bool LocationPermisionsChecked { get; private set; }

        protected override void OnDisappearing()
        {
            AboutPage.LatVisibleRegion= map.VisibleRegion;
            base.OnDisappearing();
        }

        public AboutPage()
        {

            InitializeComponent();
            map.IsVisible=false;
            JsonHandler.CreateFolder();
            //    location_tap.Tapped += Location_tap_Tapped;
            // imgpin.GestureRecognizers.Add(location_tap);
            //  lblclickpin.GestureRecognizers.Add(location_tap);
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            Authentication.DeviceAuthentication.AuthStateChanged += DeviceAuthentication_AuthStateChanged;

            
            SetCatagoryButtons();
            MessagingCenter.Send<string>("1", "backgroundService");
            MessagingCenter.Subscribe<App, ObservableCollection<DamageData>>(App.Current, "LocList", (snd, arg) =>
         {



             //     var secondNotFirst = arg.Except(ReportedDamagePins).ToList();

             //  if (secondNotFirst.Any())

             ReportedDamagePins = arg;
             // if (ReportedDamagePins.Count > 0)
             {
                 DrawPinsOnMap(arg);
             }
         });

            SettingCommand = new Command(async () =>
            {

                //SamplingGeolocation();
                //Printer.Discover();
                var settinPage = new ItemsPage();
                Navigation.PushAsync(settinPage);

            });
            BindingContext = this;

            Authentication.DeviceAuthentication.AuthStateChanged += DeviceAuthentication_AuthStateChanged;
        }

        private void DeviceAuthentication_AuthStateChanged(object sender, Authentication.AuthUser e)
        {
            getLocation();
        
                
        }



        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            //SetCatagoryButtons();
            //var profiles = Connectivity.ConnectionProfiles;
            ////if (profiles.Contains(ConnectionProfile.WiFi))
            ////{
            ////    await JsonHandler.SubmitTripFiles();

            ////}


            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                MessagingCenter.Send<string>("1", "GetData");
            }
        }

        private void _refreshToolBarItem_Clicked(object sender, EventArgs e)
        {

        }


        public string _AppVersion;
        public string AppVersion
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_AppVersion))
                    //_AppVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionCode;
                    //Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;

                    _AppVersion = "Jan23.1";
                return _AppVersion;
            }
        }

        public static MapSpan LatVisibleRegion { get; private set; }

        Dictionary<Button, Catagories> CatagoriesDictionry = new Dictionary<Button, Catagories>();
        async void SetCatagoryButtons()
        {
            Grid grid = (Grid)FindByName("GridBtn");
            Grid gridMain = (Grid)FindByName("GridMain");
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            CategoryButtons.Clear();

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                gridMain.RowDefinitions[0].Height = new GridLength(7, GridUnitType.Star);
                Label lbl = new Label();
                lbl.TextColor = Color.FromHex("#89BB29");
                lbl.Text = "No Internet Connection!!!";
                lbl.FontSize = 30;
                grid.Children.Add(lbl, 0, 0);

                return;

            }

            await JsonHandler.GetCatagories();
            ObservableCollection<Catagories> catagories = JsonHandler.catagories;



            double rows = catagories.Count / 3.0;
            int rowCount = Convert.ToInt32(Math.Ceiling(rows));
            for (int i = 0; i < rowCount; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }


            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            if (rowCount == 1)
            {
                gridMain.RowDefinitions[0].Height = new GridLength(7, GridUnitType.Star);
            }
            if (rowCount == 2)
            {
                gridMain.RowDefinitions[0].Height = new GridLength(4, GridUnitType.Star);
            }
            if (rowCount == 3)
            {
                gridMain.RowDefinitions[0].Height = new GridLength(3, GridUnitType.Star);
            }

            for (int x = 0; x < rowCount; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    // Insert button into grid  
                    CategoryButton cb = new CategoryButton();
                    Button b = cb.button;
                    b.Clicked += B_Clicked;
                    b.BackgroundColor = Color.FromHex("#89BB29");

                    cb.label.TextColor = Color.White;
                    b.IsVisible = false;
                    b.CornerRadius = 10;
                    grid.Children.Add(cb, y, x);
                    CategoryButtons.Add(cb);
                }
            }
            string svg = @"  <svg 
                                                        xmlns=""https://www.w3.org/2000/svg"" viewBox=""0 0 24 24""
                                                        fill=""#5f6368"">
                                                        <path
                                                            d=""M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"">
                                                        </path>
                                                        <path fill=""none"" d=""M0 0h24v24H0z""></path>
</svg>";

            for (int i = 0; i < catagories.Count; i++)
            {
                //CategoryButtons[i].button.Text = catagories[i].description;
                CategoryButtons[i].label.Text = catagories[i].description;
                CategoryButtons[i].BindingContext = catagories[i];
                CatagoriesDictionry[CategoryButtons[i].button] = catagories[i];
                CategoryButtons[i].button.IsVisible = true;
                CategoryButtons[i].button.CornerRadius = 5;
                //CategoryButtons[i].VerticalOptions = LayoutOptions.CenterAndExpand;
                //CategoryButtons[i].button.ImageSource=Xamarin.Forms.Svg.SvgImageSource.FromSvgResource(svg);
                //CategoryButtons[i].img.Source=Xamarin.Forms.Svg.SvgImageSource.FromSvgResource(svg);

                //CategoryButtons[i].button.FontFamily = "Inter";

                //CategoryButtons[i].FontSize = 12;
                //CategoryButtons[i].label.FontSize = 12;
                //CategoryButtons[i].TextColor = catagories[i].textColor;


                Color bkColor = Color.FromHex("#89BB29");
                try
                {
                    var bkcolorstr = new ColorTypeConverter();
                    if (catagories[i].color != "grey")
                        bkColor = (Color)bkcolorstr.ConvertFromInvariantString(catagories[i].color);
                }
                catch { }


                CategoryButtons[i].button.BackgroundColor = bkColor;


            }



        }

        private async Task getLocation()
        {
            var locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            //else
            //var locationPermisions = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (!LocationPermisionsChecked && locationInUsePermisions != PermissionStatus.Granted)
            {
                LocationPermisionsChecked = true;
                //if (!App.Settings.DontAskmeforLocationAgain)
                if (await DisplayAlert(DestoPesto.Properties.Resources.ApplicationName, DestoPesto.Properties.Resources.LocationPrompt, DestoPesto.Properties.Resources.TurnOn, DestoPesto.Properties.Resources.TurnOff))
                {
                    try
                    {
                        if (locationInUsePermisions != PermissionStatus.Granted)
                        {
                            //if (DeviceInfo.Version >= version)
                            if (locationInUsePermisions != PermissionStatus.Granted)
                            {
                                locationInUsePermisions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                                if (locationInUsePermisions != PermissionStatus.Granted)
                                    LocationPermisionsChecked = true;
                            }
                            else
                                LocationPermisionsChecked = true;

                            if (locationInUsePermisions == PermissionStatus.Granted)
                                locationInUsePermisions = await Permissions.RequestAsync<Permissions.LocationAlways>();
                        }

                    }
                    finally
                    {
                    }
                }
                //        else
                //            App.Settings.DontAskmeforLocationAgain = true;

                LocationPermisionsChecked = true;
            }



            try
            {
                locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                //locationPermisions = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                if (locationInUsePermisions != PermissionStatus.Granted)
                    return;


                var location = await Geolocation.GetLocationAsync();
                if (location != null)
                {

                    var zoomLevel = 15; // between 1 and 18
                    var latlongdegrees = 360 / (Math.Pow(2, zoomLevel));
                    if (LatVisibleRegion!=null)
                        latlongdegrees=LatVisibleRegion.LatitudeDegrees;

                    MapSpan mapSpan = new MapSpan(new Position(location.Latitude, location.Longitude), latlongdegrees, latlongdegrees);


                    var h = map.Height;
                    var w = map.Width;
                    

                    //if (LatVisibleRegion!=null)
                    //    map.MoveToRegion(LatVisibleRegion);
                    //else
                        map.MoveToRegion(mapSpan);

                    if (Authentication.DeviceAuthentication.AuthUser != null)
                        MessagingCenter.Send<string>("1", "GetData");




                }
            }
            catch (Exception ex)
            {


            }
        }

        private void B_Clicked(object sender, EventArgs e)
        {
            String text = (sender as Button).Text;

            Location_tap_Tapped(CatagoriesDictionry[(sender as Button)]);
        }

        void DrawPinsOnMap(ObservableCollection<DamageData> _pinLoc)
        {

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Code to run on the main thread

                try
                {
                    if (map.CustomPins == null)
                        map.CustomPins = new List<PinEx>();
                    map.MapElements.Clear();
                    map.Pins.Clear();
                    for (int i = 0; i < ReportedDamagePins.Count; i++)
                    {
                        try
                        {
                            var date = _pinLoc[i].firstDateReported.Split('T');
                            PinEx pin = new PinEx
                            {
                                Label = _pinLoc[i].CategoryName,
                                Url = _pinLoc[i].MarkIconUri,// Services.JsonHandler.GetCatagoryMarkIconUri( category. "https://asfameazure.blob.core.windows.net/images/fast-food.png",
                                Address = _pinLoc[i].numberOfUsers + " since " + date[0],
                                StyleId = _pinLoc[i].id,
                                Type = PinType.Generic,
                                Position = new Position(_pinLoc[i].lat, _pinLoc[i].lng),
                                Name = _pinLoc[i].numberOfUsers + " since " + date[0]


                            };
                            if (pin.Label == null)
                                continue;
                            pin.InfoWindowClicked += Pin_MarkerClicked;
                            map.CustomPins.Add(pin);
                            map.Pins.Add(pin);
                        }
                        catch (Exception error)
                        {

                            
                        }
                    }

                }
                catch (Exception e)
                {

                }
            });

        }



        protected override async void OnAppearing()
        {
            //(App.Current as App).getLocation();
            base.OnAppearing();
           

            var locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (locationInUsePermisions==PermissionStatus.Granted)
            {
                try
                {
                    var location = await Geolocation.GetLocationAsync();
                    if (location != null)
                    {
                        map.HasZoomEnabled = true;
                        

                        var zoomLevel = 15; // between 1 and 18
                        var latlongdegrees = 360 / (Math.Pow(2, zoomLevel));
                        if (LatVisibleRegion!=null)
                            latlongdegrees=LatVisibleRegion.LatitudeDegrees;

                        MapSpan mapSpan = new MapSpan(new Position(location.Latitude, location.Longitude), latlongdegrees, latlongdegrees);
                        var h = map.Height;
                        var w = map.Width;
                     
                        //    map.MoveToRegion(LatVisibleRegion);
                        //else
                            map.MoveToRegion(mapSpan);
                        map.IsVisible=true;
                    }
                }
                catch (Exception error)
                {

                    throw;
                }
            }


            if ((App.Current as App).IntentExtras != null)
            {
                foreach (var entry in (App.Current as App).IntentExtras)
                {
                    if (entry.Key == "MessageID")
                    {

                        string description;
                        (App.Current as App).IntentExtras.TryGetValue("Description", out description);
                        string submisionThumb;
                        (App.Current as App).IntentExtras.TryGetValue("SubmisionThumb", out submisionThumb);
                        string comments;
                        (App.Current as App).IntentExtras.TryGetValue("Comments", out comments);

                        (App.Current as App).IntentExtras.Clear();
                        await PopupNavigation.Instance.PushAsync(new SubmisionPopupPage(description, submisionThumb, comments));

                        break;
                    }
                    //DisplayAlert("Notification", $"{entry.Key} : {entry.Value}", "OK");
                }
            }
            if (Authentication.DeviceAuthentication.AuthUser == null)
            {
                await Shell.Current.Navigation.PushAsync(new LoginPage());
                return;
            }

        }

        private async void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
        {
            e.HideInfoWindow = true;
            string pinName = ((Pin)sender).Label;
            string pinid = ((Pin)sender).StyleId;
            bool res = await DisplayAlert(Properties.Resources.ReportFix, $":{pinName}", StringResource.YesText, StringResource.NoText);


            if (res)
            {

                FixdDamage fix = new FixdDamage();
                fix.id = pinid;
                DateTime dt = DateTime.Now;
                string month = dt.Month.ToString();
                if (dt.Month < 10)
                {
                    month = "0" + month;


                }
                string day = dt.Day.ToString();
                if (dt.Day < 10)
                {
                    day = "0" + day;


                }
                string hour = dt.Hour.ToString();
                if (dt.Hour < 10)
                {
                    hour = "0" + hour;


                }
                string min = dt.Minute.ToString();
                if (dt.Minute < 10)
                {
                    min = "0" + min;


                }
                fix.fixedDate = dt.Year + "-" + month + "-" + day + "T00:" + hour + ":" + min + ".315Z";
                //await getUserData();
                if (Authentication.DeviceAuthentication.AuthUser == null)
                {
                    //await Shell.Current.Navigation.PushAsync(new LoginPage());
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }

                string sss = Authentication.DeviceAuthentication.IDToken;
                fix.userId = userEmail;
                await JsonHandler.PutSubmission(fix);

            }

        }

        private async Task getUserData()
        {
            if (Authentication.DeviceAuthentication.AuthUser == null)
            {
                //await Shell.Current.Navigation.PushAsync(new LoginPage());
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }


            // var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));


            try
            {
                ////This is the saved firebaseauthentication that was saved during the time of login
                //var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                ////Here we are Refreshing the token
                //var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
                //Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
                //Now lets grab user information

                //userEmail = savedfirebaseauth.User.Email;
                //userId = savedfirebaseauth.User.Email;
                //deviceId = savedfirebaseauth.User.LocalId;
                userEmail = Authentication.DeviceAuthentication.AuthUser.Email;
                userId = Authentication.DeviceAuthentication.AuthUser.User_ID;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await App.Current.MainPage.DisplayAlert("Alert", "Oh no !  Token expired", "OK");
            }
        }
        private async void Location_tap_Tapped(Catagories selectedCatagory)
        {

            //JsonHandler.ShowNotification("Arion", "Hello World");
            //return; 
            
            //var locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            //if(locationInUsePermisions==PermissionStatus.Denied)
            //{
            //  var resulht=   await Permissions.RequestAsync<Permissions.StorageWrite>();

            //    return;
            //}



            //try
            //{
            //    IUploadData uploadData = DependencyService.Get<IUploadData>();
            //    var names = typeof(JsonHandler).Assembly.GetManifestResourceNames();
            //    var image = typeof(JsonHandler).Assembly.GetManifestResourceStream("DestoPesto.TestPhoto.JPG");


            //    IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;
            //    IFolder folder = await rootFolder.CreateFolderAsync("SubFolder", CreationCollisionOption.OpenIfExists);
            //    string fileName = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
            //    IFile imgFile = await folder.CreateFileAsync(fileName + ".jpg", CreationCollisionOption.ReplaceExisting);

            //    var imgFileStream = await imgFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);
            //    image.Position = 0;
            //    image.CopyTo(imgFileStream);
            //    fileName = imgFile.Path;
            //    imgFileStream.Close();



            //    uploadData.PostSubmissionWithImage(JsonHandler.getUri() + "api/Submissions/iOSUploadImage/", fileName, image);
            //    return;
            //}
            //catch (Exception error)
            //{

            //    return;
            //}
            //await getUserData();
            if (Authentication.DeviceAuthentication.AuthUser == null)
            {
                await Shell.Current.Navigation.PushAsync(new LoginPage());

                return;
            }
            var stt = Authentication.DeviceAuthentication.IDToken;
            string dfd = Authentication.DeviceAuthentication.AuthUser.ExpirationTime.ToString();
            //if (selectedCatagory.code=="9")
            //{

            //}
            //else
            Stream stream = null;



            FileResult result = await MediaPicker.CapturePhotoAsync();
            //FileResult result = await MediaPicker.PickPhotoAsync();//.CapturePhotoAsync();

            if (result != null)
            {
                string base64 = "";
                stream = await result.OpenReadAsync();
                //if (stream != null)
                //{
                //    using (MemoryStream memory = new MemoryStream())
                //    {
                //        stream.CopyTo(memory);
                //        byte[] bytes = memory.ToArray();
                //        //byte[] resizedImage = await ImageResizer.ResizeImage(bytes, 1200, 1200);
                //        //base64 = System.Convert.ToBase64String(resizedImage);
                //        base64 = System.Convert.ToBase64String(bytes);
                //    }
                //}
            }

            //{
            //    var names = typeof(JsonHandler).Assembly.GetManifestResourceNames();
            //    stream = typeof(JsonHandler).Assembly.GetManifestResourceStream("DestoPesto.TestPhoto.JPG");
            //}


            if (stream != null)
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                CancellationTokenSource cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                DateTime dt = DateTime.Now;
                string month = dt.Month.ToString();
                if (dt.Month < 10)
                {
                    month = "0" + month;


                }
                string day = dt.Day.ToString();
                if (dt.Day < 10)
                {
                    day = "0" + day;


                }
                string hour = dt.Hour.ToString();
                if (dt.Hour < 10)
                {
                    hour = "0" + hour;


                }
                string min = dt.Minute.ToString();
                if (dt.Minute < 10)
                {
                    min = "0" + min;


                }
                PostSubmission post = new PostSubmission();

                post.dateTime = dt.Year + "-" + month + "-" + day + "T00:" + hour + ":" + min + ".315Z";
                //post.dateTime = dt.ToString();
                post.UserEmail = userEmail;
                post.userId = userEmail;
                post.lat = location.Latitude;
                post.lng = location.Longitude;
                post.category = selectedCatagory.code;
                post.comments = "Reported";
                //post.photo = base64;
                post.DeviceId = deviceId;
                post.DeviceModel = Xamarin.Essentials.DeviceInfo.Model;
                post.DeviceName = Xamarin.Essentials.DeviceInfo.Name;
                post.DeviceManufacturer = Xamarin.Essentials.DeviceInfo.Manufacturer;
                post.DevicePlatform = Xamarin.Essentials.DeviceInfo.Platform.ToString();
                post.DeviceIdiom = Xamarin.Essentials.DeviceInfo.Idiom.ToString();


                var onlyWifi = Preferences.Get("onlyWifi", false);

                if (!onlyWifi)
                {
                    try
                    {
                        JsonHandler.SuspendBKService = true;
                        string fileName = await JsonHandler.BuildTripFile(post, stream);
                        if (await JsonHandler.PostSubmissionWithImage(post, stream))
                        {
                            await JsonHandler.DeleteTripFile(fileName);
                            await JsonHandler.DeleteTripFile(fileName.Replace(".jpg", ".txt"));

                        }
                    }
                    finally
                    {
                        JsonHandler.SuspendBKService = false;
                    }

                }
                else
                {
                    var profiles = Connectivity.ConnectionProfiles;
                    if (profiles.Contains(ConnectionProfile.WiFi))
                    {
                        try
                        {
                            JsonHandler.SuspendBKService = true;
                            string fileName = await JsonHandler.BuildTripFile(post, stream);

                            if (await JsonHandler.PostSubmissionWithImage(post, stream))
                            {
                                await JsonHandler.DeleteTripFile(fileName);
                                await JsonHandler.DeleteTripFile(fileName.Replace(".jpg", ".txt"));

                            }
                        }
                        finally
                        {
                            JsonHandler.SuspendBKService = false;
                        }

                    }
                    else
                    {
                        await JsonHandler.BuildTripFile(post, stream);
                    }


                }


                //  await App.Current.MainPage.DisplayAlert("Alert", action, "Ok");

            }

        }


        private async void map_PropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {
            
            //var location = await Geolocation.GetLastKnownLocationAsync();
            //Position position = new Position(location.Latitude, -location.Latitude);
            //MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            //map.MoveToRegion(mapSpan);
        }

        private void map_MapClicked(object sender, MapClickedEventArgs e)
        {


        }


    }
}