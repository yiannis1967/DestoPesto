using Facebook.LoginKit;
using Firebase.Auth;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Authentication.iOS
{
    /// <summary>
    /// https://firebase.google.com/docs/auth/android/facebook-login
    /// https://firebase.google.com/docs/android/setup/
    /// https://evgenyzborovsky.com/2018/03/09/using-native-facebook-login-button-in-xamarin-forms/
    /// </summary>
    class FacebookLoginService : Facebook.Services.FacebookLoginService
    {

        public override string AccessToken => "";

        public static void Init()
        {
            if (CurrentFacebookLoginService == null)
                CurrentFacebookLoginService = new FacebookLoginService();
            else
                throw new Exception("FacebookLoginService already initilized");


        }

        public override Action<string, string> AccessTokenChanged { get; set; }

        public FacebookLoginService()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(
             new NSString(global::Facebook.CoreKit.AccessToken.DidChangeNotification),
             async (n) =>
             {
                 string token = null;
                 if (global::Facebook.CoreKit.AccessToken.CurrentAccessToken != null)
                     token = global::Facebook.CoreKit.AccessToken.CurrentAccessToken.TokenString;

                 var oldToken = (n.UserInfo[global::Facebook.CoreKit.AccessToken.OldTokenKey] as global::Facebook.CoreKit.AccessToken)?.TokenString;
                 var newToken = (n.UserInfo[global::Facebook.CoreKit.AccessToken.NewTokenKey] as global::Facebook.CoreKit.AccessToken)?.TokenString;

                 if (token != null)
                 {
                     //Firebase sign in
                     AuthCredential credentials = Firebase.Auth.FacebookAuthProvider.GetCredential(token);
                     if (credentials != null)
                     {
                         FirebaseAuthentication.FirebaseAuth.SignInWithCredential(credentials, null);
                         //FirebaseAuthentication.FacebookSignInCompletted(true);

                     }
                     //else
                     //    FirebaseAuthentication.FacebookSignInCompletted(false);

                 }
                 //else
                 //    FirebaseAuthentication.FacebookSignInCompletted(false);


                 AccessTokenChanged?.Invoke(oldToken, newToken);
             });

            //myAccessTokenTracker = new MyAccessTokenTracker(this, firebaseAuthEvents);
            //// TODO: Stop tracking
            //myAccessTokenTracker.StartTracking();

            ////LoginManager.Instance.SetLoginBehavior(LoginBehavior.DeviceAuth)
            //var ss = this.AccessToken;
        }


        public override void SignOut()
        {
            using (var loginManager = new LoginManager())
            {
                try
                {
                    string token = null;
                    var tokenResult = global::Facebook.CoreKit.AccessToken.CurrentAccessToken;
                    if (tokenResult != null)
                    {
                        token = tokenResult.TokenString;
                    }
                    loginManager.LogOut();

                    tokenResult = global::Facebook.CoreKit.AccessToken.CurrentAccessToken;

                    if (tokenResult != null)
                    {
                        token = tokenResult.TokenString;
                    }
                }
                catch (Exception ex)
                {

                }

            }

        }

        public override void SignIn()
        {
            using (var loginManager = new LoginManager())
            {
                try
                {
                    loginManager.LogIn(new string[] { }, Authentication.GetTopViewController(), new global::Facebook.LoginKit.LoginManagerLoginResultBlockHandler(LoginManagerLoginResultBlock));
                }
                catch (Exception ex)
                {

                }

            }
        }




        void LoginManagerLoginResultBlock(global::Facebook.LoginKit.LoginManagerLoginResult result, NSError error)
        {

            string oldToken = global::Facebook.CoreKit.AccessToken.OldTokenKey;
            string newToken = global::Facebook.CoreKit.AccessToken.NewTokenKey;

        }

    }

}