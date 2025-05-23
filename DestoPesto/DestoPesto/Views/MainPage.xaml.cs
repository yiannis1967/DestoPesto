﻿using Authentication;
using DestoPesto.Models;
using DestoPesto.Services;
using Maps;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

using Xamarin.Forms.Xaml;


namespace DestoPesto.Views
{
    /// <MetaDataID>{65a085db-d7f3-4dbc-a6b9-889368b766eb}</MetaDataID>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();


            //MunicipalityStats = new MunicipalityStatsVM(Newtonsoft.Json.JsonConvert.DeserializeObject<MunicipalityStats>($"{{\r\n  \"subs\": 261,\r\n  \"_fixed\": 9,\r\n  \"perc\": 3,\r\n  \"average_repair_days\": 56,\r\n  \"unfixed_since\": \"2024-02-02T00:00:00\",\r\n  \"unfixed_days\": 318,\r\n  \"email\": \"mayor@piraeus.gov.gr\",\r\n  \"date\": \"2024-12-16T00:00:00\",\r\n  \"ranking\": 12, \r\n\"validMunicipalities\":150, \r\n}}"));

            BindingContext = this;

        }


        public event PropertyChangedEventHandler PropertyChanged;
        private MapEx map;

        public bool MapIsVisible { get; private set; }

        public bool BrowserIsVisible
        {
            get
            {
                return !MapIsVisible;
            }
        }


        public static MapSpan LatVisibleRegion { get; private set; }
        public AuthUser CurrentUser { get; private set; }
        public bool NoInternetConnection { get; private set; }


        protected async override void OnAppearing()
        {
            base.OnAppearing();



            //this.DisplayAlert("MainPage Loading time", (DateTime.UtcNow - App.StartTime).TotalSeconds.ToString(), "OK");

            if (InitTask != null)
                return;






            InitTask = Task.Run(async () =>
               {
                   //System.Threading.Thread.Sleep(5000);
                   MainThread.BeginInvokeOnMainThread(async () =>
                  {


                      var device = Xamarin.Forms.DependencyService.Get<IDevice>();
                      if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Denied)
                      {
                          var result = await device.RemoteNotificationsPermissionsRequest();
                          //if (result == PermissionStatus.Disabled)
                          //{
                          //    await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ExitText, DestoPesto.Properties.Resources.TokenExpiredText, DestoPesto.Properties.Resources.Oktext);
                          //    return;
                          //}
                      }
                      else if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Disabled)
                      {
                          // await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ExitText, DestoPesto.Properties.Resources.TokenExpiredText, DestoPesto.Properties.Resources.Oktext);
                          //return;
                      }


                      var locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                      if (locationInUsePermisions == PermissionStatus.Granted && DependencyService.Get<IDevice>().isGPSEnabled())
                      {
                          InitMap();
                      }
                      else
                      {
                          locationInUsePermisions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                          if (locationInUsePermisions != PermissionStatus.Granted)
                              MapIsVisible = false;
                          else
                              InitMap();
                      }

                      if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Granted&& locationInUsePermisions==PermissionStatus.Granted)
                          device.PermissionsGranted();
                      // Code to run on the main thread  

                  });

                   WebClient client = new WebClient();
                   await JsonHandler.GetCachedCategories();

                   if (JsonHandler.catagories?.Count > 0)
                   {


                       MainThread.BeginInvokeOnMainThread(async () =>
                       {
                           try
                           {
                               await SetCategoryButtons();
                           }
                           catch (Exception error)
                           {
                           }
                       });
                   }




                   try
                   {
                       if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                       {
                           await JsonHandler.GetCategories();

                           foreach (var category in JsonHandler.catagories)
                           {

                               var iconBuffer = client.DownloadData(category.IconUrl);

                               string iconFileName = $"icon_{category.id}.png";
                               var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                               var filePath = Path.Combine(libraryPath, iconFileName);
                               File.WriteAllBytes(filePath, iconBuffer);


                               iconBuffer = client.DownloadData(category.markIconUrl);

                               iconFileName = $"m_icon_{category.id}.png";
                               libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                               filePath = Path.Combine(libraryPath, iconFileName);
                               File.WriteAllBytes(filePath, iconBuffer);

                               //var stream = new MemoryStream(System.IO.File.ReadAllBytes(filePath));
                               //Func<Stream> getStream = () => { return stream; };
                               //category.ImageSource = StreamImageSource.FromStream(getStream);// { Stream = token => Task.Run(, token); };
                           }

                       }
                       MainThread.BeginInvokeOnMainThread(async () =>
                       {
                           try
                           {
                               await SetCategoryButtons();
                           }
                           catch (Exception error)
                           {
                           }
                       });

                   }
                   catch (Exception ex)
                   {

                   }



                   try
                   {
                       App App = DestoPesto.App.Current as App;
                       //await PopupNavigation.Instance.PushAsync(new SubmisionPopupPage("description", "submisionThumb", "comments", null));

                       if (/*(App.Current as App)*/DestoPesto.App.IntentExtras != null)
                       {
                           foreach (var entry in /*(App.Current as App)*/DestoPesto.App.IntentExtras)
                           {
                               if (entry.Key == "MessageID")
                               {

                                   string description;
                                   /*(App.Current as App)*/
                                   DestoPesto.App.IntentExtras.TryGetValue("Description", out description);
                                   string submisionThumb;
                                   /*(App.Current as App)*/
                                   DestoPesto.App.IntentExtras.TryGetValue("SubmisionThumb", out submisionThumb);
                                   string comments;
                                   /*(App.Current as App)*/
                                   DestoPesto.App.IntentExtras.TryGetValue("Comments", out comments);

                                   string messageID;
                                   DestoPesto.App.IntentExtras.TryGetValue("MessageID", out messageID);
                                   if (messageID != null && messageID.IndexOf("Contest_") == 0)
                                       return;

                                   string imageUrl;
                                   DestoPesto.App.IntentExtras.TryGetValue("ImageUrl", out imageUrl);
                                   if (submisionThumb == null)
                                       submisionThumb = imageUrl;

                                   ShareTextRequest shareTextRequest = null;

                                   string shareText;
                                   bool share = false;
                                   DestoPesto.App.IntentExtras.TryGetValue("Share", out shareText);
                                   if (shareText == "true")
                                   {
                                       share = true;

                                       string share_Text;
                                       DestoPesto.App.IntentExtras.TryGetValue("Share_Text", out share_Text);
                                       string share_Subject;
                                       DestoPesto.App.IntentExtras.TryGetValue("Share_Subject", out share_Subject);
                                       string share_Title;
                                       DestoPesto.App.IntentExtras.TryGetValue("Share_Title", out share_Title);
                                       string share_Uri;
                                       DestoPesto.App.IntentExtras.TryGetValue("Share_Uri", out share_Uri);
                                       shareTextRequest = new ShareTextRequest
                                       {
                                           Subject = share_Subject,
                                           Text = share_Text,
                                           Title = share_Title,
                                           Uri = share_Uri
                                       };

                                   }



                                   /*(App.Current as App)*/
                                   DestoPesto.App.IntentExtras.Clear();
                                   await PopupNavigation.Instance.PushAsync(new SubmisionPopupPage(description, submisionThumb, comments, shareTextRequest));

                                   break;
                               }
                               //DisplayAlert("Notification", $"{entry.Key} : {entry.Value}", "OK");
                           }
                       }


                   }
                   catch (Exception error)
                   {


                   }



                   if (_MobileHomePage == null)
                   {

                       string mainIntroHtml = null;
                       const string errorFileName = "MainIntro.json";
                       var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                       var filePath = Path.Combine(libraryPath, errorFileName);

                       if (File.Exists(filePath))
                           _MobileHomePage = File.ReadAllText(filePath);



                       MainThread.BeginInvokeOnMainThread(async () =>
                       {
                           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MobileHomePage)));
                       });



                       try
                       {
                           if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                           {
                               _MobileHomePage = client.DownloadString(Properties.Resources.HomeScreenMobileLink);

                               File.WriteAllText(filePath, _MobileHomePage);
                               MainThread.BeginInvokeOnMainThread(async () =>
                               {
                                   PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MobileHomePage)));
                               });

                           }


                       }
                       catch (Exception ex)
                       {

                       }
                   }




               });


            //this.DisplayAlert("MainPage Loading time", (DateTime.UtcNow - App.StartTime).TotalSeconds.ToString(), "OK");
        }

        private async Task InitMap()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MapIsVisible)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrowserIsVisible)));


            map = new MapEx() { HasScrollEnabled = true, MapType = MapType.Street, HasZoomEnabled = true, IsShowingUser = true };
            map.IsVisible = false;
            MapContent.Content = map;
            //map.MapClicked += map_MapClicked;
            map.PropertyChanged += map_PropertyChangedAsync;


            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
#if DEBUG
                location = new Xamarin.Essentials.Location(35.32476085169625, 25.130670035857452);
#endif
                if (location != null && map != null)
                {
                    map.HasZoomEnabled = true;


                    var zoomLevel = 15; // between 1 and 18
                    var latlongdegrees = 360 / (Math.Pow(2, zoomLevel));
                    if (LatVisibleRegion != null)
                        latlongdegrees = LatVisibleRegion.LatitudeDegrees;

                    MapSpan mapSpan = new MapSpan(new Position(location.Latitude, location.Longitude), latlongdegrees, latlongdegrees);
                    var h = map.Height;
                    var w = map.Width;

                    //    map.MoveToRegion(LatVisibleRegion);
                    //else
                    map.MoveToRegion(mapSpan);
                    map.IsVisible = true;
                    MapIsVisible = true;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MapIsVisible)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrowserIsVisible)));

                }
            }
            catch (Exception error)
            {


            }

            MapIsVisible = true;
        }

        CancellationTokenSource tokenSource;

        double _UploadOpacity;

        public double UploadOpacity
        {
            get => _UploadOpacity; // 1;
            set
            {
                _UploadOpacity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UploadOpacity)));
            }
        }


        public bool VisibleMunicipality
        {
            get => !string.IsNullOrWhiteSpace(MunicipalityStats?.MunicipalityName);
        }





        private async void map_PropertyChangedAsync(object sender, PropertyChangedEventArgs e)
        {




            if (e.PropertyName == "VisibleRegion")
            {

                try
                {
                    if (MunicipalityStats == null)
                    {
                        var location = await Geolocation.GetLastKnownLocationAsync();
#if DEBUG
                        location = new Xamarin.Essentials.Location(35.32476085169625, 25.130670035857452);
#endif
                        var municipalityStats = await JsonHandler.GetMunicipalityStats(location.Latitude, location.Longitude);
                        if (municipalityStats != null && municipalityStats.Subs > 0)
                        {
                            MunicipalityStats = new MunicipalityStatsVM(municipalityStats);
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MunicipalityStats)));
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibleMunicipality)));
                        }
                    }
                }
                catch (Exception error)
                {
                }

                tokenSource?.Cancel();

                tokenSource = new CancellationTokenSource();
                tokenSource?.Token.ThrowIfCancellationRequested();
                var meters = map.VisibleRegion?.Radius.Meters;
                if (meters.HasValue)
                {
                    try
                    {
                        await Task.Delay(5000, tokenSource.Token);

                        GetDamages(meters.Value);
                    }
                    catch (Exception error)
                    {
                    }
                }


            }

            //var location = await Geolocation.GetLastKnownLocationAsync();
            //Position position = new Position(location.Latitude, -location.Latitude);
            //MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            //map.MoveToRegion(mapSpan);
        }


        double prevRad = 0;
        private void GetDamages(double rad)
        {
            if (rad > prevRad && rad < JsonHandler.MaxRadToMapinMeters)
            {




                Task.Run(async () =>
                 {
                     var location = await Geolocation.GetLastKnownLocationAsync();

#if DEBUG
                     location = new Xamarin.Essentials.Location(35.32476085169625, 25.130670035857452);
#endif
                     //Call GetSubmission
                     try
                     {
                         (App.Current as App).SubmittedDamage = await JsonHandler.GetDamages(false, location.Latitude, location.Longitude, rad);
                         //if (JsonHandler.MunicipalityStats != null)
                         //{


                         //    MunicipalityStats = new MunicipalityStatsVM(JsonHandler.MunicipalityStats);
                         //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MunicipalityStats)));
                         //}
                     }
                     catch (Exception ex)
                     {
                     }
                     try
                     {
                         if ((App.Current as App).SubmittedDamageUser == null)
                             (App.Current as App).SubmittedDamageUser = await JsonHandler.GetDamages(true, location.Latitude, location.Longitude, rad);
                     }
                     catch (Exception ex)
                     {
                     }

                     MainThread.BeginInvokeOnMainThread(() =>
                     {
                         if (ShowAll)
                             DrawPinsOnMap((App.Current as App).SubmittedDamage);
                         else
                             DrawPinsOnMap((App.Current as App).SubmittedDamageUser);
                     });

                     prevRad = rad;
                 });
            }





        }


        List<CategoryButton> CategoryButtons = new List<CategoryButton>();
        Dictionary<Button, Catagories> CatagoriesDictionry = new Dictionary<Button, Catagories>();



        async Task SetCategoryButtons()
        {
            Grid grid = (Grid)FindByName("GridBtn");
            Grid gridMain = (Grid)FindByName("GridMain");
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            CategoryButtons.Clear();

            //if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            //{
            //    if (!this.NoInternetConnection)
            //    {
            //        this.NoInternetConnection = true;
            //        await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ApplicationName, Properties.Resources.NoInternetText, null);
            //        this.NoInternetConnection = false;
            //    }



            //    return;

            //}


            ObservableCollection<Catagories> categories = JsonHandler.catagories;



            double rows = categories.Count / 3.0;
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
                    CategoryButton cb = new CategoryButton(12);
                    Button b = cb.button;
                    if (b == null)
                    {
                        continue;
                    }
                    b.Clicked += B_Clicked;
                    b.BackgroundColor = Color.FromHex("#89BB29");

                    cb.label.TextColor = Color.White;
                    b.IsVisible = false;
                    b.CornerRadius = 10;
                    grid.Children.Add(cb, y, x);
                    CategoryButtons.Add(cb);
                }
            }


            for (int i = 0; i < categories.Count; i++)
            {
                //CategoryButtons[i].button.Text = catagories[i].description;
                CategoryButtons[i].label.Text = categories[i].description;

                if (System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName.ToLower() != "ell")
                    CategoryButtons[i].label.Text = categories[i].description_en;


                CategoryButtons[i].BindingContext = categories[i];
                CatagoriesDictionry[CategoryButtons[i].button] = categories[i];
                CategoryButtons[i].button.IsVisible = true;
                CategoryButtons[i].button.CornerRadius = 5;



                Color bkColor = Color.FromHex("#89BB29");
                try
                {
                    var bkcolorstr = new ColorTypeConverter();
                    if (categories[i].color != "grey")
                        bkColor = (Color)bkcolorstr.ConvertFromInvariantString(categories[i].color);
                }
                catch { }


                CategoryButtons[i].button.BackgroundColor = bkColor;


            }



        }




        private string _MobileHomePage;
        public string MobileHomePage
        {
            get
            {

                return _MobileHomePage;
            }
        }

        public Task InitTask { get; private set; }
        public bool ShowAll { get; private set; }
        public ObservableCollection<DamageData> ReportedDamagePins { get; private set; }
        public string CurrentContestTitle { get; private set; }

        private void B_Clicked(object sender, EventArgs e)
        {


            Location_tap_Tapped(CatagoriesDictionry[(sender as Button)]);
        }

        private async void Location_tap_Tapped(Catagories selectedCatagory)
        {
            if (!DependencyService.Get<IDevice>().isGPSEnabled())
            {
                await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ApplicationName, DestoPesto.Properties.Resources.LocationServicesOff, DestoPesto.Properties.Resources.Oktext);
                return;
            }
            //var locationInUsePermisionsa = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            var locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (locationInUsePermisions != PermissionStatus.Granted)
            {


                locationInUsePermisions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (locationInUsePermisions == PermissionStatus.Granted)
                {
                    map = new MapEx() { HasScrollEnabled = true, MapType = MapType.Street, HasZoomEnabled = true, IsShowingUser = true };

                    MapContent.Content = map;
                    map.PropertyChanged += map_PropertyChangedAsync;

                    MapIsVisible = true;




                    try
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MapIsVisible)));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrowserIsVisible)));

                        var location = await Geolocation.GetLocationAsync();
#if _DEBUG
                        location = new Location(35.32476085169625, 25.130670035857452);
#endif

                        if (location != null)
                        {
                            map.HasZoomEnabled = true;


                            var zoomLevel = 15; // between 1 and 18
                            var latlongdegrees = 360 / (Math.Pow(2, zoomLevel));
                            if (LatVisibleRegion != null)
                                latlongdegrees = LatVisibleRegion.LatitudeDegrees;

                            MapSpan mapSpan = new MapSpan(new Position(location.Latitude, location.Longitude), latlongdegrees, latlongdegrees);
                            var h = map.Height;
                            var w = map.Width;

                            //    map.MoveToRegion(LatVisibleRegion);
                            //else
                            map.MoveToRegion(mapSpan);
                            map.IsVisible = true;
                        }
                    }
                    catch (Exception error)
                    {

                        throw;
                    }
                }
                //return;                                                                                                                                                   
            }

            var cameraUsePermisions = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (cameraUsePermisions != PermissionStatus.Granted)
            {

                cameraUsePermisions = await Permissions.RequestAsync<Permissions.Camera>();
                if (cameraUsePermisions != PermissionStatus.Granted)
                    return;
            }


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

            PostSubmission post = new PostSubmission();
            try
            {
                bool dontShowAgain = Xamarin.Essentials.Preferences.Get("SubmissionTypeIntroDontShowAgain" + selectedCatagory.code, false);
                if (!dontShowAgain)
                {

                    await SubmissionTypeIntro.DisplayPopUp(selectedCatagory);
                }


                var device = Xamarin.Forms.DependencyService.Get<IDevice>();
                FileResult result = DeviceInfo.Platform.Equals(DevicePlatform.iOS) && device != null
               ? await device.CapturePhotoAsync()
               : await MediaPicker.CapturePhotoAsync();

                //FileResult result = await MediaPicker.PickPhotoAsync();//.CapturePhotoAsync();
                int? angle = null;
                if (result != null)
                {
                    string base64 = "";
                    stream = await result.OpenReadAsync();



                    var rotateAngle = device.GetOrientation(stream);
                    post.Angle = (int)rotateAngle; ;
                    stream = await result.OpenReadAsync();
                    //  DisplayAlert("image", $"Rotate angle :{rotateAngle}", "OK");
                    stream.Position = 0;
                    //return;
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
            }
            catch (Exception error)
            {

                return;
            }

            //{
            //    var names = typeof(JsonHandler).Assembly.GetManifestResourceNames();
            //    stream = typeof(JsonHandler).Assembly.GetManifestResourceStream("DestoPesto.TestPhoto.JPG");
            //}

            if (stream != null)
            {
                if (locationInUsePermisions != PermissionStatus.Granted)
                    return;

                var device = Xamarin.Forms.DependencyService.Get<IDevice>();
                if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Denied)
                {
                    var result = await device.RemoteNotificationsPermissionsRequest();
                    //if (result == PermissionStatus.Disabled)
                    //{
                    //    await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ExitText, DestoPesto.Properties.Resources.TokenExpiredText, DestoPesto.Properties.Resources.Oktext);
                    //    return;
                    //}
                }
                else if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Disabled)
                {
                    // await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ExitText, DestoPesto.Properties.Resources.TokenExpiredText, DestoPesto.Properties.Resources.Oktext);
                    //return;
                }

                //if (await device.RemoteNotificationsPermissionsCheck() != PermissionStatus.Granted)
                //    return;



                await device.PermissionsGranted();



                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                CancellationTokenSource cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);
#if DEBUG
                location = new Xamarin.Essentials.Location(35.32476085169625, 25.130670035857452);
#endif


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

                var userEmail = Authentication.DeviceAuthentication.AuthUser?.Email;
                var userId = Authentication.DeviceAuthentication.AuthUser?.User_ID;
                post.dateTime = dt.Year + "-" + month + "-" + day + "T00:" + hour + ":" + min + ".315Z";
                //post.dateTime = dt.ToString();
                post.UserEmail = userEmail;
                post.userId = userEmail;
                post.lat = location.Latitude;
                post.lng = location.Longitude;
                post.category = selectedCatagory.code;
                post.comments = "Reported";
                //post.photo = base64;
                post.DeviceId = "";
                post.DeviceModel = Xamarin.Essentials.DeviceInfo.Model;
                post.DeviceName = Xamarin.Essentials.DeviceInfo.Name;
                post.DeviceManufacturer = Xamarin.Essentials.DeviceInfo.Manufacturer;
                post.DevicePlatform = Xamarin.Essentials.DeviceInfo.Platform.ToString();
                post.DeviceIdiom = Xamarin.Essentials.DeviceInfo.Idiom.ToString();



                await JsonHandler.BuildTripFile(post, stream);



            }

        }



        private void ContestScrollText_Tapped(object sender, EventArgs e)
        {

        }

        private void My_Clicked(object sender, EventArgs e)
        {
            if (ShowAll)
            {
                ShowAll = false;
                UploadOpacity = 0;
                if (ShowAll)
                    DrawPinsOnMap((App.Current as App).SubmittedDamage);
                else
                    DrawPinsOnMap((App.Current as App).SubmittedDamageUser);
            }

        }

        private void All_Clicked(object sender, EventArgs e)
        {
            if (!ShowAll)
            {
                ShowAll = true;
                //UploadOpacity = 0.6;

                if (ShowAll)
                    DrawPinsOnMap((App.Current as App).SubmittedDamage);
                else
                    DrawPinsOnMap((App.Current as App).SubmittedDamageUser);
            }
        }

        void DrawPinsOnMap(ObservableCollection<DamageData> damages)
        {
            ReportedDamagePins = damages;
            if (map == null)
                return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Code to run on the main thread

                try
                {
                    if (map.CustomPins == null)
                        map.CustomPins = new List<PinEx>();
                    map.MapElements.Clear();
                    foreach (var pin in map.Pins)
                    {
                        if (pin is PinEx)
                            (pin as PinEx).MarkerClicked -= Pin_MarkerClicked;
                    }
                    map.Pins.Clear();
                    foreach (var damage in ReportedDamagePins)
                    {
                        try
                        {
                            var date = damage.firstDateReported.ToShortDateString();
                            PinEx pin = new PinEx
                            {
                                Label = damage.CategoryName,
                                id = damage.id,
                                Url = damage.MarkIconUri,// Services.JsonHandler.GetCatagoryMarkIconUri( category. "https://destopesto.blob.core.windows.net/images/fast-food.png",
                                IconFileName = damage.IconFileName,
                                Address = damage.numberOfUsers + " since " + date,
                                StyleId = damage.id,
                                Type = PinType.Generic,
                                Position = new Position(damage.lat, damage.lng),
                                Name = damage.numberOfUsers + " since " + date


                            };
                            if (pin.Label == null)
                                continue;
                            //pin.InfoWindowClicked += Pin_MarkerClicked;
                            pin.MarkerClicked += Pin_MarkerClicked;
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
        private async void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
        {
            e.HideInfoWindow = true;
            string pinName = ((Pin)sender).Label;
            string pinid = ((Pin)sender).StyleId;
            var damage = this.ReportedDamagePins.Where(x => x.id == pinid).FirstOrDefault();

            if (damage != null)
            {
                var location = await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();
#if DEBUG
                location = new Xamarin.Essentials.Location(35.32476085169625, 25.130670035857452);
#endif
                await PopupNavigation.Instance.PushAsync(new SubmisionDetailsPopupPage(damage, location));
            }
        }
        bool Execute = true;
        internal void UserSignedIn()
        {
            User user = Authentication.DeviceAuthentication.AuthUser.Tag as User;
            if (user.PromoContest != null)
            {

                CurrentContestTitle = user.PromoContest.Description;
                ContestLabel.IsVisible = true;
                ScrollingText.Text = CurrentContestTitle;
                Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                {
                    ScrollingText.TranslationX += 1f;

                    if (Math.Abs(ScrollingText.TranslationX) > Width)
                    {
                        ScrollingText.TranslationX = 0;
                    }

                    return Execute;
                });
            }
            else
            {
                Execute = false;
                ContestLabel.IsVisible = false;
            }

        }



        private async void ShareBtn_Clicked(object sender, EventArgs e)
        {

            if (!String.IsNullOrWhiteSpace(JsonHandler.ShareAppTitle))
            {

                await Share.RequestAsync(new ShareTextRequest
                {
                    Title = JsonHandler.ShareAppTitle,
                    Subject = JsonHandler.ShareAppSubject,

                    Text = JsonHandler.ShareAppText, //"Για πρώτη φορά γίνεται αξιολόγηση των δήμων στην Ελλάδα. Κατέβασε το 'Δες το Πες το' από το link για να ξέρεις και εσύ",
                    Uri = JsonHandler.ShareAppUri
                }); ;
            }

        }

        public MunicipalityStatsVM MunicipalityStats { get; set; }

        private void MoreBtn_Clicked(object sender, EventArgs e)
        {
            if (MunicipalityStats != null)
                PopupNavigation.Instance.PushAsync(new MunicipalityStatsPopUp(MunicipalityStats.MunicipalityStats));
        }
    }



    /// <MetaDataID>{e9ee6fc6-1642-4649-8340-b329b44de7e1}</MetaDataID>
    public class MunicipalityStatsVM
    {
        public readonly MunicipalityStats MunicipalityStats;
        public MunicipalityStatsVM(MunicipalityStats municipalityStats)
        {
            MunicipalityStats = municipalityStats;
        }

        //public string MunicipalityName { get => Properties.Resources.MunicialityText + " " + MunicipalityStats.municipalityName; set { } }
        public string MunicipalityName { get => MunicipalityStats.municipalityName; set { } }
        public string Subs { get => MunicipalityStats.Subs.ToString(); set { } }
        public string _fixed { get => MunicipalityStats.Fixed.ToString(); set { } }

        public string unfixed { get => (MunicipalityStats.Subs - MunicipalityStats.Fixed).ToString(); set { } }

        public string perc { get => MunicipalityStats.perc.ToString() + "%"; set { } }
        public string average_repair_days { get => MunicipalityStats.average_repair_days.ToString(); set { } }
        public string unfixed_since { get => MunicipalityStats.unfixed_since.ToString(); set { } }
        public string Unfixed_days { get => MunicipalityStats.Unfixed_days.ToString(); set { } }
        public string email { get => MunicipalityStats.email; set { } }
        public string date { get => MunicipalityStats.date.ToString(); set { } }
        public string ranking { get => Properties.Resources.MunicipalityScore + " " + MunicipalityStats.ranking.ToString() + " " + Properties.Resources.From + " " + totalMunicipalities.ToString(); set { } }

        public int totalMunicipalities { get => MunicipalityStats.validMunicipalities; set { } }

        public ImageSource ratingStars
        {
            get
            {


                double ratestar = 0;
                double.TryParse(rating?.Replace(",", "."), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.GetCultureInfo(1033), out ratestar);
                ratestar = Math.Round(ratestar, 0);
                switch (ratestar)
                {
                    case 1:

                        return ImageSource.FromResource("DestoPesto.Images.1stars.png");
                    case 2:
                        return ImageSource.FromResource("DestoPesto.Images.2stars.png");
                    case 3:
                        return ImageSource.FromResource("DestoPesto.Images.3stars.png");
                    case 4:
                        return ImageSource.FromResource("DestoPesto.Images.4stars.png");
                    case 5:
                        return ImageSource.FromResource("DestoPesto.Images.5stars.png");
                    default:
                        return null;
                }
            }
            set { }
        }

        public string rating
        {
            get
            {

                return MunicipalityStats.rating;
                //List<double> array = new List<double>();

                //for (int i = 1; i < totalMunicipalities + 1; i++)
                //{
                //    array.Add(i);
                //}


                //double[] arr = array.ToArray();
                //int min = 5;
                //int max = 1;

                //double[] rates = GetScaling(arr, min, max);

                //int rank = MunicipalityStats.ranking;

                //double StarPos = rates[rank - 1];

                //return StarPos.ToString();



                //return MunicipalityStats.ranking.ToString();
            }
            set { }
        }


        public double[] GetScaling(double[] arr, double min, double max)
        {
            double m = (max - min) / (arr.Max() - arr.Min());
            double c = min - arr.Min() * m;
            var newarr = new double[arr.Length];
            for (int i = 0; i < newarr.Length; i++)
            {
                newarr[i] = m * arr[i] + c;
                newarr[i] = Math.Round(newarr[i], 2);
            }
            return newarr;
        }

    }

}