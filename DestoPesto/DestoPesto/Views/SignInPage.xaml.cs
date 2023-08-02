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
            Forgot_Password_tap.Tapped+=Forgot_Password_tap_Tapped;
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
        public TapGestureRecognizer Forgot_Password_tap= new TapGestureRecognizer();

        

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
                if (!string.IsNullOrWhiteSpace(error))
                {
                    await PopupNavigation.Instance.PushAsync(new SignInMessagePopUp(error));

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