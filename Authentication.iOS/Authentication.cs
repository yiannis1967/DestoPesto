using Authentication.Facebook.Services;
using Firebase.Auth;
using Firebase.CloudMessaging;
using Foundation;
using Google.SignIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(Authentication.iOS.Authentication))]
namespace Authentication.iOS
{
    public class Authentication : IAuthentication
    {
        public Authentication()
        {

        }

        public void SignOut()
        {
            NSError nsError = null;
            FirebaseAuthentication.FirebaseAuth.SignOut(out nsError);
        }
        public static void Init(string googleAuthWebClientID)
        {

            #region Google provider initialze

            if (SignIn.SharedInstance != null&& SignIn.SharedInstance.ClientId != googleAuthWebClientID)
            {
                SignIn.SharedInstance.ClientId = googleAuthWebClientID;

                var currentUser = SignIn.SharedInstance.CurrentUser;
                SignIn.SharedInstance.PresentingViewController = Authentication.GetTopViewController();
                Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
                {
                    // called every 1 second
                    if (currentUser != SignIn.SharedInstance.CurrentUser)
                    {
                        currentUser = SignIn.SharedInstance.CurrentUser;
                        if (currentUser != null)
                        {
                            if (currentUser.Authentication?.IdToken != null)
                            {
                                
                                var credentials = Firebase.Auth.GoogleAuthProvider.GetCredential(currentUser.Authentication?.IdToken, currentUser.Authentication?.IdToken);
                                FirebaseAuth.SignInWithCredential(credentials, new Firebase.Auth.AuthDataResultHandler(OnAuthDataResult));

                                //if (GoogleCompletionSource != null)
                                //    GoogleCompletionSource.SetResult(true);
                            }
                        }
                    }
                    // do stuff here

                    return true; // return true to repeat counting, false to stop timer
                });

                var firebaseAuth = FirebaseAuthentication.FirebaseAuth;
                FacebookLoginService.Init();
            }
            #endregion





            #region  Facebook provider initialization

            FacebookLoginService.Init();
            if (!string.IsNullOrWhiteSpace(FacebookLoginService.CurrentFacebookLoginService.AccessToken))
            {
                DebugLog.AppEventLog.Log("already signed in with Facebook");
                var credentials = Firebase.Auth.FacebookAuthProvider.GetCredential(FacebookLoginService.CurrentFacebookLoginService.AccessToken);
                FirebaseAuth.SignInWithCredential(credentials, new Firebase.Auth.AuthDataResultHandler(OnAuthDataResult));
            }

            #endregion



        }
        static void OnAuthDataResult(Firebase.Auth.AuthDataResult authResult, NSError error)
        {

        }

        public static UIViewController GetTopViewController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
                vc = vc.PresentedViewController;

            if (vc is UINavigationController navController)
                vc = navController.ViewControllers.Last();

            return vc;
        }



        static Firebase.Auth.Auth _FirebaseAuth;

        public static Firebase.Auth.Auth FirebaseAuth
        {
            get
            {

                if (_FirebaseAuth == null)
                {

                    _FirebaseAuth = Firebase.Auth.Auth.DefaultInstance;
                    if (_FirebaseAuth != null)
                    {
                        _FirebaseAuth.AddAuthStateDidChangeListener(new Firebase.Auth.AuthStateDidChangeListenerHandler(AuthStateDidChangeListener));

                        _FirebaseAuth.AddIdTokenDidChangeListener(new Firebase.Auth.IdTokenDidChangeListenerHandler(AuthStateDidChangeListener));
                    }
                }
                return _FirebaseAuth;
            }
        }

        static async void AuthStateDidChangeListener(Firebase.Auth.Auth auth, Firebase.Auth.User user)
        {
            try
            {
                if (user != null)
                {
                    string authToken = await user.GetIdTokenAsync(false);

                    string providerId = "email";
                    if (user.ProviderData.Where(x => x.ProviderId == "facebook.com").Count() > 0)
                        providerId = "facebook.com";
                    else if (user.ProviderData.Where(x => x.ProviderId == "google.com").Count() > 0)
                        providerId = "google.com";
                    else if (user.ProviderData.Where(x => x.ProviderId == "apple.com").Count() > 0)
                        providerId = "apple.com";

                    await DebugLog.AppEventLog.Log("User provider = " + providerId);

                        string idToken = await user.GetIdTokenAsync(false);

                        var authUser = new AuthUser()
                        {
                            DisplayName = user.DisplayName,
                            Email = user.Email,
                            IsAnonymous = user.IsAnonymous,
                            IsEmailVerified = user.IsEmailVerified,
                            PhoneNumber = user.PhoneNumber,
                            PhotoUrl = user.PhotoUrl?.ToString(),
                            ProviderId = user.ProviderId,
                            Uid = user.Uid,
                            //Providers = firebaseUser.ProviderData.
                        };

                        await DeviceAuthentication.Current.AuthIDTokenChanged(idToken, DateTime.Now + TimeSpan.FromHours(2), authUser);

                    await DebugLog.AppEventLog.Log("User display name = " + user.DisplayName);

                }
                else
                {
                    await DebugLog.AppEventLog.Log("user is null here");
                }
            }
            catch (Exception firebaseException)
            {

                await DebugLog.AppEventLog.Log("AuthStateDidChangeListener " + firebaseException.Message + Environment.NewLine+firebaseException.StackTrace);
            }

        }


        public async Task AppleSignIn()
        {
            try
            {
                await DebugLog.AppEventLog.Log("Apple signin ");

                AppleSignInService appleSignInService = new AppleSignInService();
                await appleSignInService.SignInAsync();

                //FacebookLoginService.CurrentFacebookLoginService.SignIn();
            }
            catch (Exception ex)
            {
                await DebugLog.AppEventLog.Log("Apple signin catch = " + ex.Message + "stack trace " + ex.StackTrace);
            }
        }

        public async Task<string> EmailSignIn(string email, string password)
        {
            await DebugLog.AppEventLog.Log("EmailSignIn user " + email + "Password "+password);

            try
            {
                AuthDataResult authDataResult = await FirebaseAuth.SignInWithPasswordAsync(email, password);
                await DebugLog.AppEventLog.Log("EmailSignIn ok " );
                return null;
            }
            catch (Foundation.NSErrorException error)
            {

               var errorCode= error.UserInfo["FIRAuthErrorUserInfoNameKey"]?.ToString();
                await DebugLog.AppEventLog.Log("EmailSignIn error " + error.Message);

                //FIRAuthErrorUserInfoNameKey
                return errorCode;
            }

         
            
        }

        public async Task<string> EmailSignUp(string email, string password)
        {
            try
            {
                AuthDataResult res = await FirebaseAuth.CreateUserAsync(email, password);
                return null;

            }
            catch (Foundation.NSErrorException error)
            {
                var errorCode = error.UserInfo["FIRAuthErrorUserInfoNameKey"]?.ToString();
                //FIRAuthErrorUserInfoNameKey
                return errorCode;
            }
            catch (Exception error)
            {

                return error.Message;
            }
        }

        public void FacebookSignIn()
        {
            SignOut();
            DebugLog.AppEventLog.Log("Facebook signin");
            FacebookLoginService.CurrentFacebookLoginService.SignIn();
        }

        public void GoogleSignIn()
        {
            try
            {
                SignOut();
                var user = SignIn.SharedInstance.CurrentUser;
                if (UIApplication.SharedApplication != null)
                {
                    SignIn.SharedInstance.SignInUser();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void SendPasswordResetEmail(string email)
        {
            try
            {
                FirebaseAuth.SendPasswordReset(email, null);
                
            }
            catch (Foundation.NSErrorException error)
            {

                //var errorCode = error.UserInfo["FIRAuthErrorUserInfoNameKey"]?.ToString();


                //FIRAuthErrorUserInfoNameKey
                //return errorCode;
            }
        }
    }
}