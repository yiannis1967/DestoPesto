
using Newtonsoft.Json;
using PCLStorage;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DestoPesto.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Xml.Linq;
using LocalNotifications;
//using Plugin.LocalNotification;
//using Plugin.LocalNotification.AndroidOption;
using System.Linq;
using System.IO;
using Xamarin.Auth;


namespace DestoPesto.Services
{
    public static class JsonHandler
    {
        static HttpClient httpClient;
        public static ObservableCollection<Catagories> catagories;
        public static ObservableCollection<DamageData> damageData;
        static String URLString = "https://asfameazure.blob.core.windows.net/applications/arionapps/destopesto.xml";
        static XmlTextReader reader = new XmlTextReader(URLString);
        static String FileData = "";
        public static async Task CreateFolder()
        {
            try
            {
                IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;

                ExistenceCheckResult folderExist = await rootFolder.CheckExistsAsync("SubFolder");
                if (folderExist == ExistenceCheckResult.FolderExists)
                {
                    return;

                }
                else if (folderExist == ExistenceCheckResult.NotFound)
                {
                    IFolder folder = await rootFolder.CreateFolderAsync("SubFolder", CreationCollisionOption.OpenIfExists);

                }
            }
            catch (Exception e)
            {
            }

        }
        public static async Task<string> BuildTripFile(PostSubmission post, Stream image)
        {
            string fileName = "";
            try
            {
                IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("SubFolder", CreationCollisionOption.OpenIfExists);
                fileName = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                IFile file = await folder.CreateFileAsync(fileName + ".txt", CreationCollisionOption.ReplaceExisting);
                IFile imgFile = await folder.CreateFileAsync(fileName + ".jpg", CreationCollisionOption.ReplaceExisting);

                var imgFileStream = await imgFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);
                image.Position = 0;
                image.CopyTo(imgFileStream);
                fileName = imgFile.Path;
                imgFileStream.Close();
                string data = JsonConvert.SerializeObject(post);
                await file.WriteAllTextAsync(data);
                SubmitTripFilesTask();

            }
            catch (Exception e)
            {
                String a = e.Message;
            }
            return fileName;
        }

        static bool Exit = false;

        static public bool SuspendBKService;

        public static async Task SubmitTripFilesTask()
        {
            var device = Xamarin.Forms.DependencyService.Get<IDevice>();
            if (!device.IsBackgroundServiceStarted)
            {
                BackgroundServiceState serviceState = new BackgroundServiceState();
                IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;
                IFolder folder = await rootFolder.CreateFolderAsync("SubFolder", CreationCollisionOption.OpenIfExists);
                IList<IFile> files = await folder.GetFilesAsync();

                var dataFiles = files.Where(x => x.Name.Split('.')[1] == "txt").ToList();

                if (dataFiles.Count > 0 && !SuspendBKService)
                {
                    device.RunInBackground(new Action(() =>
                    {
                        bool _continue = true;
                        Task SendTask = Task.Run(async () =>
                        {
                            do
                            {
                                try
                                {

                                    files = await folder.GetFilesAsync();
                                    dataFiles = files.Where(x => x.Name.Split('.')[1] == "txt").ToList();
                                }
                                catch (Exception error)
                                {

                                    throw;
                                }
                                try
                                {
                                    //Starbucks Ευελπίδων

                                    var onlyWifi = Preferences.Get("onlyWifi", false);
                                    var profiles = Connectivity.ConnectionProfiles;
                                    if (dataFiles.Count == 0)
                                        _continue = false;

                                    var m_files = dataFiles.Count;

                                    if (Authentication.DeviceAuthentication.IDToken != null && !SuspendBKService)
                                    {
                                        if (onlyWifi == false || profiles.Contains(ConnectionProfile.WiFi))
                                        {
                                            foreach (IFile dataFile in dataFiles)
                                            {
                                                try
                                                {
                                                    _continue = await UploadDamage(files, dataFile);

                                                    break;
                                                    //await JsonHandler.PostSubmission(Damages);

                                                }
                                                catch (Exception error)
                                                {


                                                }
                                            }
                                        }
                                        _continue = dataFiles.Count > 0;
                                    }
                                    //try
                                    //{
                                    //    var parts = files[0].Name.Split('.');
                                    //    //files[0].Name.Split(".")
                                    //}
                                    //catch
                                    //{
                                    //}
                                    //for (int i = 0; i < files.Count; i++)
                                    //{
                                    //    String text = await files[i].ReadAllTextAsync();
                                    //    PostSubmission Damages = JsonConvert.DeserializeObject<PostSubmission>(text);
                                    //    //await JsonHandler.PostSubmission(Damages);
                                    //}
                                    //for (int i = 0; i < files.Count; i++)
                                    //{
                                    //    await DeleteTripFile(files[i].Name);
                                    //}
                                }
                                catch (Exception e)
                                {
                                    string a = e.Message;
                                }
                                finally
                                {
                                }

                                System.Threading.Thread.Sleep(1000);
                            } while (_continue);
                            device.StopBackgroundService();
                        });

                        SendTask.Wait();


                        //int count = 20;
                        //while (EventDispactherStarted)
                        //{
                        //    System.Threading.Thread.Sleep(2000);
                        //    count--;
                        //    if (count<0)
                        //    {
                        //        device.StopBackgroundService();
                        //        break;
                        //    }
                        //}
                    }), serviceState);
                }
            }


        }

        public static bool HasTripDamages()
        {
            return true;
            //Directory.Exists(PCLStorage.FileSystem.Current.LocalStorage.Path+@"\SubFolder");

            //IFolder folder = await rootFolder.CreateFolderAsync("SubFolder", CreationCollisionOption.OpenIfExists);
            //IList<IFile> files = await folder.GetFilesAsync();
            //var dataFiles = files.Where(x => x.Name.Split('.')[1] == "txt").ToList();
            //return dataFiles.Count>0;
        }
        public static async Task<bool> SubmitNextTripDamage()
        {
            BackgroundServiceState serviceState = new BackgroundServiceState();
            IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("SubFolder", CreationCollisionOption.OpenIfExists);
            IList<IFile> files = await folder.GetFilesAsync();

            var dataFiles = files.Where(x => x.Name.Split('.')[1] == "txt").ToList();
            if (dataFiles.Count > 0)
            {
                await UploadDamage(files, dataFiles[0]);
                return true;
            }
            return false;

        }
        private static async Task<bool> UploadDamage(IList<IFile> files, IFile dataFile)
        {
            bool _continue = true;
            string fileName = dataFile.Name.Split('.')[0];
            var imageFile = files.Where(x => x.Name.Split('.')[1] == "jpg" && x.Name.Split('.')[0] == fileName).FirstOrDefault();
            if (imageFile != null)
            {
                PostSubmission damage = null;
                Stream imageStream = null;
                try
                {
                    String text = await dataFile.ReadAllTextAsync();
                    damage = JsonConvert.DeserializeObject<PostSubmission>(text);
                    imageStream = await imageFile.OpenAsync(PCLStorage.FileAccess.Read);
                    imageStream.Position = 0;

                    if (JsonHandler.PostSubmissionWithImageSync(damage, imageStream))
                    {
                        _continue = false;
                        await DeleteTripFile(dataFile.Name);
                        await DeleteTripFile(imageFile.Name);
                    }
                    else
                    {

                    }

                }
                catch (Exception error)
                {

                }
                if (damage == null || imageStream == null)
                {
                    try
                    {
                        await DeleteTripFile(dataFile.Name);
                    }
                    catch (Exception ierror)
                    {


                    }
                    try
                    {
                        await DeleteTripFile(imageFile.Name);
                    }
                    catch (Exception ierror)
                    {


                    }

                }


            }

            return _continue;
        }

        public static async Task DeleteTripFile(String FileName)
        {
            try
            {
                IFolder rootFolder = PCLStorage.FileSystem.Current.LocalStorage;

                IFolder folder = await rootFolder.CreateFolderAsync("SubFolder", CreationCollisionOption.OpenIfExists);
                IFile file = await folder.GetFileAsync(FileName);
                ExistenceCheckResult fileExist = await folder.CheckExistsAsync(file.Name);
                if (fileExist == ExistenceCheckResult.FileExists)
                {
                    await file.DeleteAsync();



                }
                else if (fileExist == ExistenceCheckResult.NotFound)
                {

                    DependencyService.Get<Toast>().Show("File Not Found");

                }

            }
            catch (Exception e)

            {
                string a = e.Message;
            }
        }
        public static List<Catagories> JasonToCatagories(String jsonData)
        {

            return null;


        }


        static string _Uri;

        internal static string getUri()
        {
            if (_Uri == null)
            {
                XDocument doc = XDocument.Load(URLString);
                XElement signInElement = null;
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                    signInElement = doc.Root.Element("SignIn").Element("ios");

                if (DeviceInfo.Platform == DevicePlatform.Android)
                    signInElement = doc.Root.Element("SignIn").Element("Android");

                if (signInElement != null)
                {
                    FacebookSignInMethod = signInElement.Attribute("Facebook")?.Value?.ToLower() == "true";
                    GoogleSignInMethod = signInElement.Attribute("Google")?.Value?.ToLower() == "true";
                    AppleSignInMethod = signInElement.Attribute("Apple")?.Value?.ToLower() == "true";
                    EmailSignInMethod = signInElement.Attribute("Email")?.Value?.ToLower() == "true";
                }
                var device = Xamarin.Forms.DependencyService.Get<IDevice>();

                


                string uri = doc.Root.Attribute("ServiceUrl")?.Value;

                _Uri = uri;
#if DEBUG
                var profiles = Connectivity.ConnectionProfiles;
                //if(profiles.Contains(ConnectionProfile.WiFi))
                _Uri = "http://10.0.0.13:5005/";
                _Uri = "http://10.0.0.10:5005/";
#endif

                var deviceID = device.DeviceID;
                DebugLog.AppEventLog.Start(_Uri, deviceID, doc.Root.Element("DebugLogs"));
            }
            return _Uri;



        }

        public static async Task<string> UploadImage(Stream stream)
        {
            try
            {

                var POS = stream.Position;
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(stream), "Photo", "MyPhoto.jpg");
                content.Add(new StringContent("Mitsos"), "Name");

                var httpClient = new HttpClient();
                Uri uri = new Uri(getUri() + "api/Submissions");

                var uploadServiceBaseAddress = getUri() + "api/Submissions/UploadImage";// getUri() + "api/Submissions/UploadImage";

                var httpResponseMessage = await httpClient.PostAsync(uploadServiceBaseAddress, content);

                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    return "OK";
                return httpResponseMessage.StatusCode.ToString();
                //var result = await httpResponseMessage.Content.ReadAsStringAsync();


            }
            catch (Exception error)
            {
                return error.Message;
                throw;
            }
        }

        public static async Task<bool> PostSubmissionWithImage(PostSubmission submission, Stream image)
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    return false;
                submission.photo = null;
                //if (submission.category=="9")
                //{
                //    var names = typeof(JsonHandler).Assembly.GetManifestResourceNames();
                //    image = typeof(JsonHandler).Assembly.GetManifestResourceStream("DestoPesto.TestPhoto.JPG");
                //}
                var client = new HttpClient();
                //Uri uri = new Uri(getUri() + "api/Submissions");

                string token = Authentication.DeviceAuthentication.IDToken;
                client.DefaultRequestHeaders.Add("Authorization", Authentication.DeviceAuthentication.IDToken);
                image.Position = 0;
                var multiPartContent = new MultipartFormDataContent();
                var submissionJson = Newtonsoft.Json.JsonConvert.SerializeObject(submission);
                multiPartContent.Add(new StringContent(submissionJson), "SubmissionJson");

                multiPartContent.Add(new StreamContent(image), "Photo", "MyPhoto.jpg");




                var url = getUri() + "api/Submissions/UploadImage";// getUri() + "api/Submissions/UploadImage";
                var response = await client.PostAsync(url, multiPartContent);

                var result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ShowNotification(Properties.Resources.AlertText, Properties.Resources.ReportSuccess);
                    MessagingCenter.Send<string>("1", "GetData");
                    return true;
                }
                else
                {
                    ShowNotification(Properties.Resources.AlertText, Properties.Resources.ReportFailed);
                    return false;

                }

            }
            catch (Exception error)
            {

                return false;
            }

        }


        public static bool PostSubmissionWithImageSync(PostSubmission submission, Stream image)
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                    return false;
                submission.photo = null;
                //if (submission.category=="9")
                //{
                //    var names = typeof(JsonHandler).Assembly.GetManifestResourceNames();
                //    image = typeof(JsonHandler).Assembly.GetManifestResourceStream("DestoPesto.TestPhoto.JPG");
                //}
                var client = new HttpClient();
                //Uri uri = new Uri(getUri() + "api/Submissions");

                string token = Authentication.DeviceAuthentication.IDToken;
                client.DefaultRequestHeaders.Add("Authorization", Authentication.DeviceAuthentication.IDToken);

                var multiPartContent = new MultipartFormDataContent();
                var submissionJson = Newtonsoft.Json.JsonConvert.SerializeObject(submission);
                multiPartContent.Add(new StringContent(submissionJson), "SubmissionJson");

                multiPartContent.Add(new StreamContent(image), "Photo", "MyPhoto.jpg");




                var url = getUri() + "api/Submissions/UploadImage";// getUri() + "api/Submissions/UploadImage";
                var responseTask = client.PostAsync(url, multiPartContent);
                responseTask.Wait();
                var response = responseTask.Result;
                var resultTask = response.Content.ReadAsStringAsync();
                resultTask.Wait();
                var result = resultTask;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ShowNotification(Properties.Resources.AlertText, Properties.Resources.ReportSuccess);
                    MessagingCenter.Send<string>("1", "GetData");
                    return true;
                }
                else
                {
                    ShowNotification(Properties.Resources.AlertText, Properties.Resources.ReportFailed);
                    return false;

                }

            }
            catch (Exception error)
            {

                return false;
            }

        }



        //   public static async Task PostSubmission(String DateTime,String userEmail,String userId,String lat,String lng,String category,String comments,String photo,String deviceId)
        public static async Task PostSubmission(PostSubmission post)
        {


            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {

                return;

            }
    
            

                var client = new HttpClient();
            Uri uri = new Uri(getUri() + "api/Submissions");


            //  string jsonData = @"{""dateTime"" : ""DateHere"", ""userEmail"" : ""mypassword"", ""userId"" : ""mypassword"", ""lat"" : ""mypassword"", ""lng"" : ""mypassword"", ""category"" : ""mypassword"", ""comments"" : ""mypassword"", ""photo"" : ""mypassword"", ""deviceId"" : ""mypassword""}";
            string serializedObject = JsonConvert.SerializeObject(post);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            await getUserData();
            //var savedfirebaseauth = Authentication.DeviceAuthentication.IDToken JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
            //client.DefaultRequestHeaders.Add("Authorization", savedfirebaseauth.FirebaseToken);
            string token = Authentication.DeviceAuthentication.IDToken;
            client.DefaultRequestHeaders.Add("Authorization", Authentication.DeviceAuthentication.IDToken);

            HttpResponseMessage response = await client.PostAsync(uri, content);

            // this result string should be something like: "{"token":"rgh2ghgdsfds"}"

            var result = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)

            {
                //  await App.Current.MainPage.DisplayAlert(Properties.Resources.AlertText, Properties.Resources.ReportSuccess, Properties.Resources.Oktext) ;
                // DependencyService.Get<INotification>().ShowNotification(Properties.Resources.AlertText, Properties.Resources.ReportSuccess);
                ShowNotification(Properties.Resources.AlertText, Properties.Resources.ReportSuccess);
                MessagingCenter.Send<string>("1", "GetData");
            }
            else

            {
                //DependencyService.Get<INotification>().ShowNotification(Properties.Resources.AlertText, Properties.Resources.ReportFailed);
                // await App.Current.MainPage.DisplayAlert(Properties.Resources.AlertText, Properties.Resources.ReportFailed, Properties.Resources.Oktext);

                ShowNotification(Properties.Resources.AlertText, Properties.Resources.ReportFailed);
            }


        }


        public static async Task SignIn(string firebaseToken)
        {


            var device = Xamarin.Forms.DependencyService.Get<IDevice>();
            string deviceID = device.DeviceID;

            var client = new HttpClient();
            String Parameters = "?deviceFirebaseToken=" + firebaseToken;// + "&lng=" + lng + "&rad=" + rad;

            Uri uri = new Uri(getUri() + $"api/Account/SignIn?deviceFirebaseToken={firebaseToken}&deviceID={deviceID}");

            //var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));

            //httpClient.DefaultRequestHeaders.Add("Authorization", savedfirebaseauth.FirebaseToken);

            client.DefaultRequestHeaders.Add("Authorization", Authentication.DeviceAuthentication.IDToken);



            var response = await client.GetAsync(uri);

            //   var content = await response.Content.ReadAsStringAsync();

        }

        public static async Task<bool> PutSubmission(FixdDamage post)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {

                return false;

            }
            var client = new HttpClient();
            Uri uri = new Uri(getUri() + "api/Submissions");



            string serializedObject = JsonConvert.SerializeObject(post);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            await getUserData();
            //var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));

            //client.DefaultRequestHeaders.Add("Authorization", savedfirebaseauth.FirebaseToken);
            client.DefaultRequestHeaders.Add("Authorization", Authentication.DeviceAuthentication.IDToken);

            HttpResponseMessage response = await client.PutAsync(uri, content);

            // this result string should be something like: "{"token":"rgh2ghgdsfds"}"

            var result = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)

            {
                // await App.Current.MainPage.DisplayAlert(Properties.Resources.AlertText,  Properties.Resources.DamageSatusSuccess,Properties.Resources.Oktext);
                // DependencyService.Get<INotification>().ShowNotification(Properties.Resources.AlertText, Properties.Resources.DamageSatusSuccess);
                ShowNotification(Properties.Resources.AlertText, Properties.Resources.DamageSatusSuccess);

                MessagingCenter.Send<string>("1", "GetData");
                return true;
            }
            else

            {
                //  await App.Current.MainPage.DisplayAlert(Properties.Resources.AlertText,  Properties.Resources.DamageSatusFail, Properties.Resources.Oktext);
                //  DependencyService.Get<INotification>().ShowNotification(Properties.Resources.AlertText, Properties.Resources.DamageSatusFail);
                ShowNotification(Properties.Resources.AlertText, Properties.Resources.DamageSatusFail);
                return false;
            }


        }

        static INotificationManager _NotificationManager;
        public static INotificationManager NotificationManager
        {
            get
            {
                if (_NotificationManager == null)
                    _NotificationManager = DependencyService.Get<INotificationManager>();
                return _NotificationManager;
            }
        }

        public static bool FacebookSignInMethod { get; private set; }
        public static bool GoogleSignInMethod { get; private set; }
        public static bool AppleSignInMethod { get; private set; }
        public static bool EmailSignInMethod { get; private set; }
        public static double MaxDistanceForFixed = 100;

        public static void ShowNotification(string title, string message)
        {
            //var notification = new NotificationRequest
            //{
            //    BadgeNumber = 1,
            //    Title = title,
            //    Description = message,

            //    Android = new AndroidOptions
            //    {
            //        Priority = NotificationPriority.High,
            //        ChannelId = "my_notification_channel"
            //    }



            //};
            // NotificationCenter.Current.Show(notification);
            //V notificationManager = DependencyService.Get<INotificationManager>();

            // string title = $"Local Notification #{notificationNumber}";
            // string message = $"You have now received {notificationNumber} notifications!";




            //NotificationManager.SendNotification(title, message);

        }

        public static string GetCatagory(int code)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName.ToLower() != "ell")
                return catagories.Where(x => x.code == code.ToString()).FirstOrDefault()?.description_en;
            else
                return catagories.Where(x => x.code == code.ToString()).FirstOrDefault()?.description;

        }

        public static string GetCatagoryMarkIconUri(int code)
        {
            return catagories.Where(x => x.code == code.ToString()).FirstOrDefault()?.markIconUrl;
        }
        public static async Task GetCatagories()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {

                return;

            }

            using (httpClient = new HttpClient())
            {
                String link = getUri();
                Uri uri = new Uri(link + "api/Categories");
                //await getUserData();
                System.Diagnostics.Debug.WriteLine("Uri = " + uri.AbsoluteUri);
                //var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));
                ////     var authHeader = new AuthenticationHeaderValue("Authorization", savedfirebaseauth.FirebaseToken);
                //httpClient.DefaultRequestHeaders.Add("Authorization", savedfirebaseauth.FirebaseToken);

                // httpClient.DefaultRequestHeaders.Add("Authorization", Authentication.DeviceAuthentication.IDToken);

                var response = await httpClient.GetStringAsync(uri);

                //   var content = await response.Content.ReadAsStringAsync();


                var Catagories = JsonConvert.DeserializeObject<List<Catagories>>(response);
                catagories = new ObservableCollection<Catagories>(Catagories);

                //             
            }
        }
        public static string WebAPIkey = "AIzaSyCH9F_m6KO7_1BB3NN0eiSjN9_d99bRjsk";
        public static async Task<System.Collections.ObjectModel.ObservableCollection<DamageData>> GetDamages(bool isUser, double lat, double lng, double rad)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet && !string.IsNullOrWhiteSpace(Authentication.DeviceAuthentication.IDToken))
            {

                return new ObservableCollection<DamageData>();

            }


            using (httpClient = new HttpClient())
            {
                String Parameters = "?lat=" + lat.ToString(System.Globalization.CultureInfo.GetCultureInfo(1033)) + "&lng=" + lng.ToString(System.Globalization.CultureInfo.GetCultureInfo(1033)) + "&rad=" + rad;

                Uri uri = new Uri(getUri() + "api/Submissions" + Parameters);
                await getUserData();
                //var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));

                //httpClient.DefaultRequestHeaders.Add("Authorization", savedfirebaseauth.FirebaseToken);

                httpClient.DefaultRequestHeaders.Add("Authorization", Authentication.DeviceAuthentication.IDToken);


                if (isUser)
                {

                    uri = new Uri(getUri() + "api/Submissions/UserAll" );
                }

                var response = await httpClient.GetStringAsync(uri);

                //   var content = await response.Content.ReadAsStringAsync();


                var Damages = JsonConvert.DeserializeObject<List<DamageData>>(response);
                damageData = new ObservableCollection<DamageData>(Damages);
                return damageData;

                //             
            }
        }

        public static List<Location> getLocationsFromReports(System.Collections.ObjectModel.ObservableCollection<DamageData> data)
        {
            List<Location> _locations = new List<Location>();

            for (int i = 0; i < data.Count; i++)
            {

                _locations.Add(new Location(data[i].lat, data[i].lng));

            }
            return _locations;

        }

        public static async Task getUserData()
        {

            //var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));


            //try
            //{

            //       //This is the saved firebaseauthentication that was saved during the time of login
            //       var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));

            //    try
            //    {
            //        var user = await authProvider.GetUserAsync(savedfirebaseauth.FirebaseToken);
            //    }
            //    catch 
            //    {
            //        var RefreshedContent = await authProvider.RefreshAuthAsync(savedfirebaseauth);
            //        Preferences.Set("MyFirebaseRefreshToken", JsonConvert.SerializeObject(RefreshedContent));
            //        savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));

            //    }

            //    //Now lets grab user information


            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    await App.Current.MainPage.DisplayAlert("Alert", "Oh no !  Token expired", "OK");
            //}
        }

        internal async static Task<bool> RemoveUser()
        {

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                return false;
            var client = new HttpClient();
            Uri uri = new Uri(getUri() + "api/Account/Delete");



            string serializedObject = JsonConvert.SerializeObject("");
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            await getUserData();
            //var savedfirebaseauth = JsonConvert.DeserializeObject<Firebase.Auth.FirebaseAuth>(Preferences.Get("MyFirebaseRefreshToken", ""));

            //client.DefaultRequestHeaders.Add("Authorization", savedfirebaseauth.FirebaseToken);
            client.DefaultRequestHeaders.Add("Authorization", Authentication.DeviceAuthentication.IDToken);

            HttpResponseMessage response = await client.PostAsync(uri, content);

            // this result string should be something like: "{"token":"rgh2ghgdsfds"}"

            var result = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result?.ToLower() == true.ToString().ToLower();

            }
            else

            {
                return false;
            }


        }
    }

}
