
using System;
using System.Linq;
using Android.Content;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.AsyncExtensions;
using Android.Gms.Common.Apis;
using Android.Gms.Drive;
using Android.Gms.Tasks;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Java.Interop;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Android.Graphics;
using Android.Runtime;

namespace Authentication.Android
{

    /// <MetaDataID>{cd2447fc-0c70-4343-b67d-fead1b151690}</MetaDataID>
    public class FirebaseAuthentication
    {
        static FirebaseAuthentication()
        {
            //if (Xamarin.Forms.Application.Current is IAppLifeTime)
            //    (Xamarin.Forms.Application.Current as IAppLifeTime).ApplicationResuming += FirebaseAuthentication_ApplicationResuming;
        }



        internal static FirebaseAuthEvents FirebaseAuthEventsConsumer = new FirebaseAuthEvents();
        public static void OnCanceled()
        {
        }

        public static void OnComplete(System.Threading.Tasks.Task task)
        {
        }

        public static void OnFailure(Java.Lang.Exception e)
        {
        }

        public static async void OnSuccess(Java.Lang.Object result)
        {

            try
            {
                await FirebaseUserSignedIn();

                //var token = await FirebaseAuth.CurrentUser.GetIdTokenAsync(false);
                //string authToken= token.Token;

                ////DeviceAuthentication.AuthUser
                //var authUser = new Remoting.RestApi.AuthUser()
                //{
                //    AuthToken = authToken,
                //    Email = FirebaseAuth.CurrentUser.Email,
                //    Name = FirebaseAuth.CurrentUser.DisplayName,
                //    Firebase_Sign_in_Provider = FirebaseAuth.CurrentUser.Providers[FirebaseAuth.CurrentUser.Providers.Count - 1],
                //    User_ID = FirebaseAuth.CurrentUser.Uid,
                //    Picture = FirebaseAuth.CurrentUser.PhotoUrl.?ToString()
                //};
                //DeviceAuthentication.SignedIn(authUser);
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                // do stuff
            }



            //DeviceAuthentication.AuthUser=new Remoting.RestApi.AuthUser() {AuthToken=  FirebaseAuth.CurrentUser.Email}


        }

        internal static System.Threading.Tasks.Task<string> EmailSignUp(string email, string password)
        {


            return System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    await FirebaseAuth.CreateUserWithEmailAndPassword(email, password).AsAsync();
                    return null;
                }
                catch (Exception err)
                {
                    return err.Message;
                }
            });

        }

        public static System.Threading.Tasks.Task<string> EmailSignIn(string email, string password)
        {
            return System.Threading.Tasks.Task<string>.Run(async () =>
            {
                try
                {
                    await FirebaseAuth.SignInWithEmailAndPassword(email, password).AsAsync();
                    return null;
                }
                catch (Firebase.Auth.FirebaseAuthInvalidCredentialsException err)
                {

                    return err.Message;
                }
                catch (Java.Lang.IllegalArgumentException errr)
                {
                    return errr.Message;
                }
                catch (Java.Lang.Exception errrr)
                {
                    return errrr.Message;
                }

            });


        }
        internal static void SendPasswordResetEmail(string email)
        {
            FirebaseAuth.SendPasswordResetEmail(email);
        }


        private static System.Threading.Timer Timer = new System.Threading.Timer(new System.Threading.TimerCallback(OnTimer), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));


        private static int RC_SIGN_IN = 9001;

        private GoogleSignInClient mSignInClient;

        static FirebaseUser CurrentUser;

        static GetTokenResult CurrenTokenResult;



        static FirebaseAuth _FirebaseAuth;
        public static FirebaseAuth FirebaseAuth
        {
            get
            {
                if (_FirebaseAuth == null)
                {
                    _FirebaseAuth = FirebaseAuth.Instance;
                    _FirebaseAuth.AuthState += AuthStateChanged;
                    _FirebaseAuth.IdToken += IdTokenChanged;


                }
                return FirebaseAuth.Instance;
            }
        }

        public static bool OnActivityResult(int requestCode, global::Android.App.Result resultCode, Intent data)
        {

            if (requestCode == 1)
            {
                GoogleSignInResult result = global::Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                if (result.IsSuccess)
                {
                    GoogleSignInAccount account = result.SignInAccount;
                    LoginWithFirebase(account);
                }
            }
            else
            {
                try
                {
                    FacebookCallbackManager.OnActivityResult(requestCode, Convert.ToInt32(resultCode), data);
                }
                catch (Exception ee)
                {


                }
            }
            return true;
        }

        public static string User_ID { get; private set; }

        public static void FacebookSignIn()
        {

            Xamarin.Facebook.Login.LoginManager.Instance.LogIn(Xamarin.Essentials.Platform.CurrentActivity, new string[] { });
        }


        static DateTime authTimestamp;
        static DateTime issuedAtTimestamp;
        static DateTime expirationTimestamp;
        static string idToken;

        private static async void IdTokenChanged(object sender, FirebaseAuth.IdTokenEventArgs e)
        {

            try
            {

                if (FirebaseAuth.CurrentUser != null)
                {
                    var email = FirebaseAuth.CurrentUser.Email;
                    var data = FirebaseAuth.CurrentUser.ProviderData.ToList().Select(x => new { x.ProviderId, x.Email }).ToArray();

                    GetTokenResult token = await FirebaseAuth.CurrentUser.GetIdToken(false).AsAsync<GetTokenResult>();

                    authTimestamp = DeviceAuthentication.FromUnixTime(token.AuthTimestamp);
                    issuedAtTimestamp = DeviceAuthentication.FromUnixTime(token.IssuedAtTimestamp);
                    expirationTimestamp = DeviceAuthentication.FromUnixTime(token.ExpirationTimestamp);
                    idToken = token.Token;

                    var firebaseUser = FirebaseAuth.CurrentUser;
                    string providerId = firebaseUser.ProviderData?.Where(x => x.ProviderId!="firebase").Select(x => x.ProviderId).FirstOrDefault();
                    if (providerId == null)
                        providerId=firebaseUser.ProviderId;


                    lock (AuthenticationTokenLock)
                    {
                        CurrenTokenResult = token;
                    }

                    var authUser = new Authentication.AuthUser()
                    {
                        DisplayName = firebaseUser.DisplayName,
                        Email = firebaseUser.Email,
                        IsAnonymous = firebaseUser.IsAnonymous,
                        IsEmailVerified = firebaseUser.IsEmailVerified,
                        PhoneNumber = firebaseUser.PhoneNumber,
                        PhotoUrl = firebaseUser.PhotoUrl?.ToString(),
                        ProviderId = providerId,
                        Uid = firebaseUser.Uid,
                        //Providers = firebaseUser.ProviderData.
                    };

                    DeviceAuthentication.Current.AuthIDTokenChanged(idToken, expirationTimestamp, authUser);
                }
                else
                {
                    DeviceAuthentication.Current.AuthIDTokenChanged(null, expirationTimestamp, null);
                }
            }
            catch (Exception error)
            {


            }




        }
        private static async void FirebaseAuthentication_ApplicationResuming(object sender, EventArgs e)
        {
            ValidateAuthenticationToken();
        }
        static async void OnTimer(object state)
        {
            ValidateAuthenticationToken();
        }

        static object AuthenticationTokenLock = new object();
        private static void ValidateAuthenticationToken()
        {
            lock (AuthenticationTokenLock)
            {
                if (CurrentUser != null && CurrenTokenResult != null)
                {
                    bool newToken = false;
                    if (expirationTimestamp - DateTime.UtcNow < TimeSpan.FromMinutes(5))
                    {

                        //var tokenTask = FirebaseAuth.CurrentUser.GetIdTokenAsync(true);
                        //if (tokenTask.Wait(TimeSpan.FromSeconds(30)))
                        //{
                        //    var token = tokenTask.Result;
                        //    newToken = CurrenTokenResult.Token != token.Token;
                        //    CurrenTokenResult = token;
                        //    authTimestamp = DeviceAuthentication.FromUnixTime(token.AuthTimestamp);
                        //    issuedAtTimestamp = DeviceAuthentication.FromUnixTime(token.IssuedAtTimestamp);
                        //    expirationTimestamp = DeviceAuthentication.FromUnixTime(token.ExpirationTimestamp);
                        //}


                    }
                }
            }

        }



        private static async void AuthStateChanged(object sender, FirebaseAuth.AuthStateEventArgs e)
        {
            try
            {
                if (e.Auth.CurrentUser != null)
                {
                    await FirebaseUserSignedIn();
                }
                else
                {
                    CurrentUser = null;
                    CurrenTokenResult = null;
                    DeviceAuthentication.SignedOut();
                }
            }
            catch (FirebaseAuthInvalidUserException firebaseException)
            {
                DeviceAuthentication.SignedOut();
                // do stuff
            }

        }

        private static async System.Threading.Tasks.Task FirebaseUserSignedIn()
        {
            //var token = await FirebaseAuth.CurrentUser.GetIdTokenAsync(false);
            //lock (AuthenticationTokenLock)
            //{
            //    CurrentUser = FirebaseAuth.CurrentUser;
            //    CurrenTokenResult = token;
            //    authTimestamp = DeviceAuthentication.FromUnixTime(token.AuthTimestamp);
            //    issuedAtTimestamp = DeviceAuthentication.FromUnixTime(token.IssuedAtTimestamp);
            //    expirationTimestamp = DeviceAuthentication.FromUnixTime(token.ExpirationTimestamp);
            //}
            //string authToken = token.Token;

            //var authUser = new AuthUser()
            //{
            //    AuthToken = authToken,
            //    ExpirationTime = expirationTimestamp,
            //    Email = FirebaseAuth.CurrentUser.Email,
            //    Name = FirebaseAuth.CurrentUser.DisplayName,
            //    Firebase_Sign_in_Provider = FirebaseAuth.CurrentUser.ProviderId,
            //    User_ID = FirebaseAuth.CurrentUser.Uid,
            //    Picture = FirebaseAuth.CurrentUser.PhotoUrl.?ToString()
            //};
            //DeviceAuthentication.SignedIn(authUser);
        }

        public static void GoogleSignIn()
        {

            if (FirebaseAuth.CurrentUser != null)
            {

            }
            var intent = googleApiClient.SignInIntent;
            Xamarin.Essentials.Platform.CurrentActivity.StartActivityForResult(intent, 1);
        }

        public static void GoogleSignOut()
        {
            googleApiClient.SignOut();


        }



        public static void FacebookeSignOut()
        {

            FacebookLoginService.CurrentFacebookLoginService.SignOut();
            FirebaseAuth.SignOut();
        }

        public static void SignOut()
        {


            if (FirebaseAuth != null && FirebaseAuth.CurrentUser != null)
            {
                if (DeviceAuthentication.AuthUser?.Firebase_Sign_in_Provider.ToLower() == "google.com")
                    GoogleSignOut();

                if (DeviceAuthentication.AuthUser?.Firebase_Sign_in_Provider.ToLower() == "facebook.com")
                {
                    FacebookLoginService.CurrentFacebookLoginService.SignOut();

                }


                FirebaseAuth.SignOut();
            }
        }
        static GoogleSignInClient googleApiClient;
        public static void Init(Context context, string googleAuthWebClientID)
        {
            if (FirebaseAuth.CurrentUser != null)
            {
            }
            FirebaseApp.InitializeApp(context);
            FacebookLoginService.Init(FirebaseAuthEventsConsumer);
            if (!string.IsNullOrWhiteSpace(FacebookLoginService.CurrentFacebookLoginService.AccessToken))
            {
                var credentials = Firebase.Auth.FacebookAuthProvider.GetCredential(FacebookLoginService.CurrentFacebookLoginService.AccessToken);

                FirebaseAuth.SignInWithCredential(credentials)
                    .AddOnCompleteListener(FirebaseAuthEventsConsumer)
                    .AddOnSuccessListener(FirebaseAuthEventsConsumer)
                    .AddOnFailureListener(FirebaseAuthEventsConsumer)
                    .AddOnCanceledListener(FirebaseAuthEventsConsumer);

            }


            GoogleSignInOptions options =
             new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                       .RequestIdToken(googleAuthWebClientID)
                      .RequestScopes(DriveClass.ScopeFile)
                      .Build();

            googleApiClient = global::Android.Gms.Auth.Api.SignIn.GoogleSignIn.GetClient(Xamarin.Essentials.Platform.CurrentActivity, options);



            //gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn).RequestIdToken(googleAuthWebClientID).RequestEmail().Build();
            //googleApiClient = new GoogleApiClient.Builder(context).AddApi(Android.Gms.Auth.Api.Auth.GOOGLE_SIGN_IN_API, gso).Build();

            //googleApiClient.Connect();
        }
        //public static bool OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        //{
        //    if (requestCode == 1)
        //    {

        //        GoogleSignInResult result = Android.Gms.Auth.Api.Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
        //        if (result.IsSuccess)
        //        {
        //            GoogleSignInAccount account = result.SignInAccount;
        //            LoginWithFirebase(account);
        //        }
        //    }
        //    else
        //        FacebookCallbackManager.OnActivityResult(requestCode, Convert.ToInt32(resultCode), data);
        //    return true;
        //}

        private static void LoginWithFirebase(GoogleSignInAccount account)
        {
            var credentials = GoogleAuthProvider.GetCredential(account.IdToken, null);
            FirebaseAuth.SignInWithCredential(credentials).AddOnSuccessListener(FirebaseAuthEventsConsumer)
                .AddOnFailureListener(FirebaseAuthEventsConsumer);

        }

        public static ICallbackManager FacebookCallbackManager
        {
            get
            {
                return FacebookLoginService.CallbackManager;
            }
        }

        internal static void OnComplete(Task task)
        {

        }
    }

    /// <summary>
    /// https://firebase.google.com/docs/auth/android/facebook-login
    /// https://firebase.google.com/docs/android/setup/
    /// https://evgenyzborovsky.com/2018/03/09/using-native-facebook-login-button-in-xamarin-forms/
    /// </summary>
    /// <MetaDataID>{b8aad75d-ac7a-43e4-a669-99b653d31f90}</MetaDataID>
    class FacebookLoginService : Facebook.Services.FacebookLoginService
    {

        public static void Init(FirebaseAuthEvents firebaseAuthEvents)
        {

            FacebookLoginService.CallbackManager = CallbackManagerFactory.Create();

            CurrentFacebookLoginService = new FacebookLoginService(firebaseAuthEvents);



        }
        public static ICallbackManager CallbackManager;
        readonly MyAccessTokenTracker myAccessTokenTracker;
        public override Action<string, string> AccessTokenChanged { get; set; }

        public FacebookLoginService(FirebaseAuthEvents firebaseAuthEvents)
        {
            myAccessTokenTracker = new MyAccessTokenTracker(this, firebaseAuthEvents);
            // TODO: Stop tracking
            myAccessTokenTracker.StartTracking();

            //LoginManager.Instance.SetLoginBehavior(LoginBehavior.DeviceAuth)
            var ss = this.AccessToken;
        }



        public override string AccessToken => Xamarin.Facebook.AccessToken.CurrentAccessToken?.Token;

        public override void SignOut()
        {

            LoginManager.Instance.LogOut();
        }

        public override void SignIn()
        {
            throw new NotImplementedException();
        }

    }

    /// <MetaDataID>{d2897910-5426-41ee-ae58-139aa29551e4}</MetaDataID>
    class MyAccessTokenTracker : AccessTokenTracker
    {
        readonly Facebook.Services.FacebookLoginService facebookLoginService;
        readonly FirebaseAuthEvents FirebaseAuthEvents;
        public MyAccessTokenTracker(FacebookLoginService facebookLoginService, FirebaseAuthEvents firebaseAuthEvents)
        {
            this.facebookLoginService = facebookLoginService;
            FirebaseAuthEvents = firebaseAuthEvents;
        }

        protected override void OnCurrentAccessTokenChanged(AccessToken oldAccessToken, AccessToken currentAccessToken)
        {

            if (!string.IsNullOrWhiteSpace(FacebookLoginService.CurrentFacebookLoginService.AccessToken))
            {
                var credentials = Firebase.Auth.FacebookAuthProvider.GetCredential(FacebookLoginService.CurrentFacebookLoginService.AccessToken);
                FirebaseAuthentication.FirebaseAuth.SignInWithCredential(credentials)
                    .AddOnCompleteListener(FirebaseAuthEvents)
                    .AddOnSuccessListener(FirebaseAuthEvents)
                    .AddOnFailureListener(FirebaseAuthEvents)
                    .AddOnCanceledListener(FirebaseAuthEvents);
            }

            if (oldAccessToken != null && currentAccessToken == null)
                FirebaseAuthentication.FirebaseAuth.SignOut();


            facebookLoginService.AccessTokenChanged?.Invoke(oldAccessToken?.Token, currentAccessToken?.Token);
        }
    }

    /// <MetaDataID>{e851eb91-0720-4e16-9b53-b905bad258ba}</MetaDataID>
    class FirebaseAuthEvents : Java.Lang.Object, IOnSuccessListener, IOnCompleteListener, IOnFailureListener, IOnCanceledListener
    {
        public void OnCanceled()
        {
            FirebaseAuthentication.OnCanceled();
        }

        public void OnComplete(System.Threading.Tasks.Task task)
        {
            FirebaseAuthentication.OnComplete(task);
        }

        public void OnComplete(Task task)
        {
            FirebaseAuthentication.OnComplete(task);

        }

        public void OnFailure(Java.Lang.Exception e)
        {
            FirebaseAuthentication.OnFailure(e);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            FirebaseAuthentication.OnSuccess(result);
        }
    }


}
