
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DestoPesto.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignupPage : ContentPage
    {

        public string WebAPIkey = "AIzaSyCH9F_m6KO7_1BB3NN0eiSjN9_d99bRjsk";
        public TapGestureRecognizer Signin_tap = new TapGestureRecognizer();
        public SignupPage()
        {
            InitializeComponent();
            Signin_tap.Tapped += Signin_tap_Tapped;

            lblSignin.GestureRecognizers.Add(Signin_tap);
        }

        private async void Signin_tap_Tapped(object sender, EventArgs e)
        {

            //await Shell.Current.Navigation.PushAsync(new LoginPage());
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string error = await Authentication.DeviceAuthentication.EmailSignUp(txtEmail.Text, txtPassword.Text);
                if (!string.IsNullOrWhiteSpace(error))
                    await App.Current.MainPage.DisplayAlert("Alert", error, "OK");

                
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
    }
}