
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DestoPesto.Services;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPage, System.ComponentModel.INotifyPropertyChanged
    {

        public string WebAPIkey = "AIzaSyCH9F_m6KO7_1BB3NN0eiSjN9_d99bRjsk";
        public TapGestureRecognizer Signin_tap = new TapGestureRecognizer();
        public SignupPage()
        {
            InitializeComponent();
            Signin_tap.Tapped += Signin_tap_Tapped;

            BindingContext = this;

            lblSignin.GestureRecognizers.Add(Signin_tap);
        }

        private async void Signin_tap_Tapped(object sender, EventArgs e)
        {

            //await Shell.Current.Navigation.PushAsync(new LoginPage());
            //await Shell.Current.GoToAsync("//LoginPage");
            await Navigation.PopAsync();
        }

        public bool IsValidInfo
        {
            get
            {

                return EmailMultiValidator.IsValid && PasswordValidator.IsValid && UserValidator.IsValid && RePasswordValidator.IsValid;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string error = await Authentication.DeviceAuthentication.EmailSignUp(txtEmail.Text, txtPassword.Text);


                string errorMessage = error;

                switch (error)
                {

                    case "ERROR_OPERATION_NOT_ALLOWED": //Indicates that Anonymous accounts are not enabled.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_OPERATION_NOT_ALLOWED;
                        break;

                    case "ERROR_WEAK_PASSWORD": //If the password is not strong enough.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_WEAK_PASSWORD;
                        break;
                    case "ERROR_INVALID_EMAIL": //If the email address is malformed.  
                        errorMessage = DestoPesto.Properties.Resources.ERROR_INVALID_EMAIL;
                        break;
                    case "ERROR_EMAIL_ALREADY_IN_USE": // If the email is already in use by a different account. 
                        errorMessage = DestoPesto.Properties.Resources.ERROR_EMAIL_ALREADY_IN_USE;
                        break;
                    case "ERROR_INVALID_CREDENTIAL": //If the [email] address is malformed.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_INVALID_CREDENTIAL;
                        break;
                    case "ERROR_NOT_ALLOWED": // Indicates that email and email sign-in link accounts are not enabled. Enable them in the Auth section of the Firebase console.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_NOT_ALLOWED;
                        break;
                    case "ERROR_DISABLED ": // Indicates the users account is disabled.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_DISABLED;
                        break;
                    case "ERROR_INVALID": // Indicates the email address is invalid.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_INVALID;
                        break;
                    case "ERROR_WRONG_PASSWORD": // If the [password] is wrong.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_WRONG_PASSWORD;
                        break;
                    case "ERROR_USER_NOT_FOUND": // If there is no user corresponding to the given [email] address, or if the user has been deleted.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_USER_NOT_FOUND;
                        break;
                    case "ERROR_USER_DISABLED": // If the user has been disabled (for example, in the Firebase console)
                        errorMessage = DestoPesto.Properties.Resources.ERROR_USER_DISABLED;
                        break;
                    case "ERROR_TOO_MANY_REQUESTS": // If there was too many attempts to sign in as this user.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_TOO_MANY_REQUESTS;
                        break;
                    case "ERROR_ACCOUNT_EXISTS_WITH_DIFFERENT_CREDENTIAL": // If there already exists an account with the email address asserted by Google.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_ACCOUNT_EXISTS_WITH_DIFFERENT_CREDENTIAL;
                        break;
                    case "ERROR_INVALID_ACTION_CODE": // If the action code in the link is malformed, expired, or has already been used.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_INVALID_ACTION_CODE;
                        break;
                    case "ERROR_INVALID_CUSTOM_TOKEN": // The custom token format is incorrect. Please check the documentation.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_INVALID_CUSTOM_TOKEN;
                        break;
                    case "ERROR_CUSTOM_TOKEN_MISMATCH": // Invalid configuration. Ensure your apps SHA1 is correct in the Firebase console.
                        errorMessage = DestoPesto.Properties.Resources.ERROR_CUSTOM_TOKEN_MISMATCH;
                        break;
                    case "EXPIRED_ACTION_CODE": // if the password reset code has expired.
                        errorMessage = DestoPesto.Properties.Resources.EXPIRED_ACTION_CODE;
                        break;
                    case "INVALID_ACTION_CODE": // if the password reset code is invalid. This can happen if the code is malformed or has already been used.
                        errorMessage = DestoPesto.Properties.Resources.INVALID_ACTION_CODE;
                        break;
                    case "USER_DISABLED": // if the user corresponding to the given password reset code has been disabled.
                        errorMessage = DestoPesto.Properties.Resources.USER_DISABLED;
                        break;
                    case "USER_NOT_FOUND": // if there is no user corresponding to the password reset code. This may have happened if the user was deleted between when the code was issued and when this method was called.
                        errorMessage = DestoPesto.Properties.Resources.USER_NOT_FOUND;
                        break;
                    case "WEAK_PASSWORD": // if the new password is not strong enough.
                        errorMessage = DestoPesto.Properties.Resources.WEAK_PASSWORD;
                        break;
                    case "ERROR_INVALID_VERIFICATION_CODE": // if the code is invalid
                        errorMessage = DestoPesto.Properties.Resources.ERROR_INVALID_VERIFICATION_CODE;
                        break;

                }

                if (!string.IsNullOrWhiteSpace(error))
                    await PopupNavigation.Instance.PushAsync(new SignInMessagePopUp(errorMessage));

                    
                    //if (!string.IsNullOrWhiteSpace(error))
                    //await App.Current.MainPage.DisplayAlert("Alert", error, "OK");


                //    try
                //    {
                //        var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
                //        var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(txtEmail.Text, txtPassword.Text);

                //        string gettoken = auth.FirebaseToken;

                //        await App.Current.MainPage.DisplayAlert("Alert", Properties.Resources.SignupSuccess, "Ok");
                //    }
                //    catch (Exception ex)
                //    {
                //        await App.Current.MainPage.DisplayAlert("Alert", Properties.Resources.SignupFail, "OK");
                //    }

            }
            //else {
            //    await App.Current.MainPage.DisplayAlert("Alert", Properties.Resources.NoInternetText, "OK");
            //}


            //await JsonHandler.GetCatagories();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValidInfo)));
        }
    }
}