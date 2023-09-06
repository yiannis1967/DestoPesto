using Authentication;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInPage : ContentPage
    {
        public SignInPage()
        {
            InitializeComponent();

            lblSignup.GestureRecognizers.Add(Signup_tap);

            Signup_tap.Tapped += Signup_tap_Tapped;
            Forgot_Password_tap.Tapped += Forgot_Password_tap_Tapped;
            forgoPass.GestureRecognizers.Add(Forgot_Password_tap);
            this.BindingContext = this;

            //ForgotPassword = new Command(OnForgotPassword);
        }
        public ICommand BackCommand => new Command<string>(async (url) => await Shell.Current.Navigation.PopAsync());

        private async void Forgot_Password_tap_Tapped(object sender, EventArgs e)
        {

            await Shell.Current.Navigation.PushAsync(new ForgotPasswordPage(txtEmail.Text));
        }


        public TapGestureRecognizer Signup_tap = new TapGestureRecognizer();
        public TapGestureRecognizer Signin_tap = new TapGestureRecognizer();
        public TapGestureRecognizer Forgot_Password_tap = new TapGestureRecognizer();



        public Command ForgotPassword { get; }

        private async void Signup_tap_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new SignupPage());

        }

        

        private async void btnSignIn_Clicked(object sender, EventArgs e)
        {
            //Xamarin.CommunityToolkit.Effects.TouchEffect
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    txtWrongEmail.IsVisible = true;
                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        txtWrongPass.IsVisible = true;
                        return;

                    }
                    return;
                }


                string error = await DeviceAuthentication.EmailSignIn(txtEmail.Text, txtPassword.Text);

                string errorMessage="";

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
                {
                    await PopupNavigation.Instance.PushAsync(new SignInMessagePopUp(errorMessage));

                    //await App.Current.MainPage.DisplayAlert("Alert", error, "OK");
                }

                //    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(WebAPIkey));
                //    try

                //    {


                //        var auth = await authProvider.SignInWithEmailAndPasswordAsync(txtEmail.Text, txtPassword.Text);

                //        String token = auth.FirebaseToken.ToString();


                //        var content = await auth.GetFreshAuthAsync();

                //        var serializedcontnet = JsonConvert.SerializeObject(content);
                //        Preferences.Set("MyFirebaseRefreshToken", serializedcontnet);
                //        await Shell.Current.GoToAsync("//AboutPage");
                //    }
                //    catch (Exception ex)
                //    {
                //        await App.Current.MainPage.DisplayAlert("Alert", "Invalid useremail or password", "OK");
                //    }
            }
            else
            {
                await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.AlertText, Properties.Resources.NoInternetText, DestoPesto.Properties.Resources.Oktext);
            }
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtWrongEmail.IsVisible = false;
        }

        private void txtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtWrongPass.IsVisible = false;
        }

    }
}