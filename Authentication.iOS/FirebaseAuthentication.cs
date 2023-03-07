using Firebase.Auth;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Authentication.iOS
{
    public class FirebaseAuthentication: global::Authentication.FirebaseAuthentication
    {
        static Firebase.Auth.Auth _FirebaseAuth;
        // private static UIViewController _viewController;

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

                        if (_FirebaseAuth.CurrentUser != null)
                            AuthStateDidChangeListener(_FirebaseAuth, _FirebaseAuth.CurrentUser);
                    }
                }
                return _FirebaseAuth;
            }
        }

        private static async void AuthStateDidChangeListener(Auth auth, User user)
        {
            if (user != null)
            {
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

                DeviceAuthentication.Current.AuthIDTokenChanged(idToken, DateTime.Now + TimeSpan.FromHours(2), authUser);
            }
           else
                DeviceAuthentication.Current.AuthIDTokenChanged(null, DateTime.Now + TimeSpan.FromHours(2), null);

        }
    }
}