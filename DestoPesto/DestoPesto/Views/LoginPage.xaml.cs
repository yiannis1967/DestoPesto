
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DestoPesto.Models;
using DestoPesto.ViewModels;
using Xamarin.Auth;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Authentication;
using System.Windows.Input;
using Xamarin.Forms.Internals;
using DestoPesto.Services;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public TapGestureRecognizer Signup_tap = new TapGestureRecognizer();
        public TapGestureRecognizer Signin_tap = new TapGestureRecognizer();
        public TapGestureRecognizer google_signin = new TapGestureRecognizer();
        public TapGestureRecognizer fb_signin = new TapGestureRecognizer();
        public TapGestureRecognizer apple_signin = new TapGestureRecognizer();
        public string WebAPIkey = "AIzaSyCH9F_m6KO7_1BB3NN0eiSjN9_d99bRjsk";

        public ICommand BackCommand => new Command<string>(async (url) =>
        {
            //await Shell.Current.Navigation.PopAsync();
        });

        public LoginPage()
        {

            Authentication.DeviceAuthentication.AuthStateChanged += DeviceAuthentication_AuthStateChanged;
            if (!string.IsNullOrEmpty(Preferences.Get("MyFirebaseRefreshToken", "")))
                Shell.Current.GoToAsync("//AboutPage");
            InitializeComponent();
            store = AccountStore.Create();



            // Your label tap event

            SignupCommand = new Command(OnSignUp);
            SigninCommand = new Command(OnSignIn);
            GoogleSigninCommand = new Command(OnGoogleSignIn);
            FacebookSigninCommand = new Command(OnFacebookSignIn);
            AppleSigninCommand = new Command(OnAppleSignIn);

            

            Signup_tap.Tapped += Signup_tap_Tapped;
            Signin_tap.Tapped += Signin_tap_Tapped;
            google_signin.Tapped += Google_signin_Tapped;
            fb_signin.Tapped += Fb_signin_Tapped;
            apple_signin.Tapped += apple_signin_Tapped;

            gogleSignin.GestureRecognizers.Add(google_signin);
            fbSignin.GestureRecognizers.Add(fb_signin);
            appleSignin.GestureRecognizers.Add(apple_signin);

            //lblSignup.GestureRecognizers.Add(Signup_tap);

            emailSignin.GestureRecognizers.Add(Signin_tap);
            //DeviceAuthentication.SignedOut();

            JsonHandler.getUri();

            GoogleSignInVisible=JsonHandler.GoogleSignInMethod;
            AppleSignInVisible=JsonHandler.AppleSignInMethod;
            FacebookSignInVisible= JsonHandler.FacebookSignInMethod;
            EmailSignInVisible= JsonHandler.EmailSignInMethod;

            if (!GoogleSignInVisible&&!AppleSignInVisible&&!FacebookSignInVisible&&!EmailSignInVisible)
                EmailSignInVisible=true;

            this.BindingContext = this;// new LoginViewModel(Navigation);

        }

        private async void OnAppleSignIn()
        {
            apple_signin_Tapped(this, EventArgs.Empty);
            //await SignInMessagePopUp.DisplayPopUp();
            // await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ApplicationName, DestoPesto.Properties.Resources.LocationPrompt, DestoPesto.Properties.Resources.TurnOn, DestoPesto.Properties.Resources.TurnOff);
            //await MessageDialogPopup.DisplayPopUp("Hello", "hello", "ok");



        }


        public bool GoogleSignInVisible { get; set; }
        //{
        //    get
        //    {
        //        if (Xamarin.Essentials.DeviceInfo.Platform.ToString() == "iOS")
        //            return false;
        //        else
        //            return true;
        //    }
        //}

        public bool AppleSignInVisible { get; set; }
        //{
        //    get
        //    {
        //        if (Xamarin.Essentials.DeviceInfo.Platform.ToString() == "iOS")
        //            return true;
        //        else
        //            return false;
        //    }
        //}

        public bool FacebookSignInVisible { get; set; }
        public bool EmailSignInVisible { get; }

        private void OnFacebookSignIn(object obj)
        {
            Fb_signin_Tapped(this, EventArgs.Empty);

        }

        private void OnSignIn(object obj)
        {
            Signin_tap_Tapped(this, EventArgs.Empty);

        }

        private void OnGoogleSignIn(object obj)
        {
            Google_signin_Tapped(this, EventArgs.Empty);

        }



        private void OnSignUp()
        {
            Signup_tap_Tapped(this, EventArgs.Empty);
        }

        private async void DeviceAuthentication_AuthStateChanged(object sender, AuthUser e)
        {
            if (e != null)
            {
                Device.BeginInvokeOnMainThread(
            async () =>
            {
                int backStep = Navigation.NavigationStack.Count- Navigation.NavigationStack.IndexOf(this);

                if (Navigation.NavigationStack.Count == 1)
                    await Shell.Current.GoToAsync("//AboutPage");
                else
                    while (backStep>0)
                    {
                        backStep--;
                        await Navigation.PopAsync();
                    }
            });
            }
        }
        private void apple_signin_Tapped(object sender, EventArgs e)
        {
            DeviceAuthentication.AppleSignIn();
        }
        private void Fb_signin_Tapped(object sender, EventArgs e)
        {

            DeviceAuthentication.FacebookSignIn();
            //String clientId = null;
            //string redirectUri = null;



            //clientId = XAppConstants.FacebookAndroidClientId;
            //redirectUri = XAppConstants.FacebookAndroidRedirectUrl;



            //account = store.FindAccountsForService(XAppConstants.AppName).FirstOrDefault();

            //var authenticator = new OAuth2Authenticator(
            //    clientId,
            //    XAppConstants.FacebookScope,
            //    new Uri(XAppConstants.FacebookAuthorizeUrl),
            //    new Uri(XAppConstants.FacebookAccessTokenUrl),
            //    null);

            //authenticator.Completed += OnAuthCompletedfb;
            //authenticator.Error += OnAuthErrorfb;

            //AuthenticationState.Authenticator = authenticator;

            //var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            //presenter.Login(authenticator);

        }

        Account account;
        AccountStore store;

        public Command SignupCommand { get; }
        public Command SigninCommand { get; }
        public Command GoogleSigninCommand { get; }
        public Command FacebookSigninCommand { get; }
        public Command AppleSigninCommand { get; }

        private async void Google_signin_Tapped(object sender, EventArgs e)
        {

            DeviceAuthentication.GoogleSignIn();

            //string clientId = null;
            //string redirectUri = null;

            //switch (Device.RuntimePlatform)
            //{

            //    case Device.Android:
            //        clientId = XAppConstants.AndroidClientId;
            //        redirectUri = XAppConstants.AndroidRedirectUrl;
            //        break;
            //}

            //account = store.FindAccountsForService(XAppConstants.AppName).FirstOrDefault();

            //var authenticator = new OAuth2Authenticator(
            //    clientId,
            //    null,

            //    XAppConstants.Scope,
            //    new Uri(XAppConstants.AuthorizeUrl),
            //    new Uri(redirectUri),
            //    new Uri(XAppConstants.AccessTokenUrl),
            //    null,
            //    true);

            //authenticator.Completed += OnAuthCompleted;
            //authenticator.Error += OnAuthError;

            //AuthenticationState.Authenticator = authenticator;

            //var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            //presenter.Login(authenticator);




        }
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            //var authenticator = sender as OAuth2Authenticator;
            //if (authenticator != null)
            //{
            //    authenticator.Completed -= OnAuthCompleted;
            //    authenticator.Error -= OnAuthError;
            //}

            //User user = null;
            //if (e.IsAuthenticated)
            //{
            //    // If the user is authenticated, request their basic user data from Google
            //    // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
            //    var request = new OAuth2Request("GET", new Uri(XAppConstants.UserInfoUrl), null, e.Account);
            //    var response = await request.GetResponseAsync();
            //    if (response != null)
            //    {
            //        // Deserialize the data and store it in the account store
            //        // The users email address will be used to identify data in SimpleDB
            //        string userJson = await response.GetResponseTextAsync();
            //        user = JsonConvert.DeserializeObject<User>(userJson);
            //    }

            //    if (user != null)
            //    {
            //        Dictionary<string,string> authtokenDic = e.Account.Properties;
            //        var keyValuePair = new KeyValuePair<string, string>("access_token", authtokenDic["access_token"]);


            //        var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));

            //        String accessToken =keyValuePair.Value.ToString();
            //        var idToken = e.Account.Properties["id_token"];


            //        try

            //        {


            //            var auth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Google, accessToken);

            //          String token=  auth.FirebaseToken.ToString();
            //            String token2 = auth.User.LocalId;

            //            var content = await auth.GetFreshAuthAsync();
            //            var serializedcontnet = JsonConvert.SerializeObject(content);
            //            Preferences.Set("MyFirebaseRefreshToken", serializedcontnet);



            //            await Shell.Current.GoToAsync("//AboutPage");
            //        }
            //        catch (Exception ex)
            //        {
            //            await App.Current.MainPage.DisplayAlert("Alert", "Invalid useremail or password", "OK");
            //        }

            //        // TODO: your path within your DB structure.

            //    }

            //    //await store.SaveAsync(account = e.Account, AppConstant.Constants.AppName);
            //    //await DisplayAlert("Email address", user.Email, "OK");
            //}
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

        //    App.Current.MainPage.DisplayAlert("Alert", e.Message, "OK");
        }

        async void OnAuthCompletedfb(object sender, AuthenticatorCompletedEventArgs e)
        {
            //var authenticator = sender as OAuth2Authenticator;
            //if (authenticator != null)
            //{
            //    authenticator.Completed -= OnAuthCompletedfb;
            //    authenticator.Error -= OnAuthErrorfb;
            //}

            //User user = null;
            //if (e.IsAuthenticated)
            //{




            //    var request = new OAuth2Request("GET", new Uri(XAppConstants.FacebookUserInfoUrl), null, e.Account);
            //    var response = await request.GetResponseAsync();
            //    if (response != null)
            //    {
            //        // Deserialize the data and store it in the account store
            //        // The users email address will be used to identify data in SimpleDB
            //        string userJson = await response.GetResponseTextAsync();
            //        user = JsonConvert.DeserializeObject<User>(userJson);
            //    }

            //    if (user != null)
            //    {
            //        Dictionary<string, string> authtokenDic = e.Account.Properties;
            //        var keyValuePair = new KeyValuePair<string, string>("access_token", authtokenDic["access_token"]);


            //        var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));

            //        String accessToken = keyValuePair.Value.ToString();
            //        var idToken = e.Account.Properties["id_token"];


            //        try

            //        {


            //            var auth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook, accessToken);

            //            String token = auth.FirebaseToken.ToString();
            //            String token2 = auth.User.LocalId;

            //            var content = await auth.GetFreshAuthAsync();
            //            var serializedcontnet = JsonConvert.SerializeObject(content);
            //            Preferences.Set("MyFirebaseRefreshToken", serializedcontnet);



            //            await Shell.Current.GoToAsync("//AboutPage");
            //        }
            //        catch (Exception ex)
            //        {
            //            await App.Current.MainPage.DisplayAlert("Alert", "Invalid useremail or password", "OK");
            //        }

            //        // TODO: your path within your DB structure.

            //    }

            //    //await store.SaveAsync(account = e.Account, AppConstant.Constants.AppName);
            //    //await DisplayAlert("Email address", user.Email, "OK");
            //}
        }

        void OnAuthErrorfb(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompletedfb;
                authenticator.Error -= OnAuthErrorfb;
            }

            MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.AlertText, e.Message, DestoPesto.Properties.Resources.Oktext);
        }

        private async void Signup_tap_Tapped(object sender, EventArgs e)
        {


            await Shell.Current.Navigation.PushAsync(new SignupPage());


        }
        private async void Signin_tap_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new SignInPage());
        }

        //private async void btnSignIn_Clicked(object sender, EventArgs e)
        //{


        //    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        //    {
        //        string error = await DeviceAuthentication.EmailSignIn(txtEmail.Text, txtPassword.Text);
        //        if (!string.IsNullOrWhiteSpace(error))
        //            await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.AlertText, error, DestoPesto.Properties.Resources.Oktext);

        //        //    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
        //        //    try

        //        //    {


        //        //        var auth = await authProvider.SignInWithEmailAndPasswordAsync(txtEmail.Text, txtPassword.Text);

        //        //        String token = auth.FirebaseToken.ToString();


        //        //        var content = await auth.GetFreshAuthAsync();

        //        //        var serializedcontnet = JsonConvert.SerializeObject(content);
        //        //        Preferences.Set("MyFirebaseRefreshToken", serializedcontnet);
        //        //        await Shell.Current.GoToAsync("//AboutPage");
        //        //    }
        //        //    catch (Exception ex)
        //        //    {
        //        //        await App.Current.MainPage.MessageDialogPopup.DisplayPopUp("Alert", "Invalid useremail or password", "OK");
        //        //    }
        //    }
        //    else
        //    {
        //        await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.AlertText, Properties.Resources.NoInternetText, DestoPesto.Properties.Resources.Oktext);
        //    }


        //}
    }
}