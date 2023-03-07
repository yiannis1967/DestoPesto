using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Facebook;

namespace DestoPesto.Droid
{
    class MyAccessTokenTracker : AccessTokenTracker
    {
        
        readonly FirebaseAuthEvents FirebaseAuthEvents;
        public MyAccessTokenTracker( FirebaseAuthEvents firebaseAuthEvents)
        {
            
            FirebaseAuthEvents = firebaseAuthEvents;
        }

        string AccessToken => Xamarin.Facebook.AccessToken.CurrentAccessToken?.Token;

        protected override void OnCurrentAccessTokenChanged(AccessToken oldAccessToken, AccessToken currentAccessToken)
        {

            if (!string.IsNullOrWhiteSpace(AccessToken))
            {
                System.Diagnostics.Debug.WriteLine(AccessToken);
                var credentials = Firebase.Auth.FacebookAuthProvider.GetCredential(AccessToken);
                FirebaseAuth.SignInWithCredential(credentials)
                    .AddOnCompleteListener(FirebaseAuthEvents)
                    .AddOnSuccessListener(FirebaseAuthEvents)
                    .AddOnFailureListener(FirebaseAuthEvents)
                    .AddOnCanceledListener(FirebaseAuthEvents);
            }

            //if (oldAccessToken != null && currentAccessToken == null)
            //    FirebaseAuthentication.FirebaseAuth.SignOut();


            // facebookLoginService.AccessTokenChanged?.Invoke(oldAccessToken?.Token, currentAccessToken?.Token);
        }

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
        private static async void AuthStateChanged(object sender, FirebaseAuth.AuthStateEventArgs e)
        {

        }
        private static async void IdTokenChanged(object sender, FirebaseAuth.IdTokenEventArgs e)
        {
        }
    }
    class FirebaseAuthEvents : Java.Lang.Object, IOnSuccessListener, IOnCompleteListener, IOnFailureListener, IOnCanceledListener
    {
        public void OnCanceled()
        {
            
        }

        public void OnComplete(Task task)
        {
            
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            
        }

        public async void OnSuccess(Java.Lang.Object result)
        {
            var token = MyAccessTokenTracker.FirebaseAuth.CurrentUser.DisplayName;
        }
            
    }
}