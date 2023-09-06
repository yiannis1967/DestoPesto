using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[assembly: Xamarin.Forms.Dependency(typeof(Authentication.Android.Authenticatio))]
namespace Authentication.Android
{
    /// <MetaDataID>{830da1d2-5feb-44e0-93b8-af500ea75922}</MetaDataID>
    public class Authenticatio : IAuthentication
    {
        public void FacebookSignIn()
        {
            Authentication.Android.FirebaseAuthentication.FacebookeSignOut();
            Authentication.Android.FirebaseAuthentication.SignOut();
            //Authentication.Android.FirebaseAuthentication.GoogleSignIn();
            Authentication.Android.FirebaseAuthentication.FacebookSignIn();
        }

        public void GoogleSignIn()
        {
            Authentication.Android.FirebaseAuthentication.FacebookeSignOut();
            Authentication.Android.FirebaseAuthentication.SignOut();
            Authentication.Android.FirebaseAuthentication.GoogleSignIn();
            //Authentication.Android.FirebaseAuthentication.FacebookSignIn();
        }

        public System.Threading.Tasks.Task<string> EmailSignUp(string email, string password)
        {
            return Authentication.Android.FirebaseAuthentication.EmailSignUp(email, password);
            
        }

        public System.Threading.Tasks.Task<string> EmailSignIn(string email, string password)
        {
            try
            {
                return Authentication.Android.FirebaseAuthentication.EmailSignIn(email, password);
            }
            catch (Exception error)
            {

                throw;
            }
            
        }


        public void SendPasswordResetEmail(string email)
        {
            Authentication.Android.FirebaseAuthentication.SendPasswordResetEmail(email);
        }

        public void AppleSignIn()
        {
            
        }

        public void SignOut()
        {
            Authentication.Android.FirebaseAuthentication.SignOut();
        }
    }
}