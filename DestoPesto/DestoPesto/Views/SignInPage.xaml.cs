using Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
        public TapGestureRecognizer Signup_tap = new TapGestureRecognizer();
        public TapGestureRecognizer Signin_tap = new TapGestureRecognizer();


        private async void Signup_tap_Tapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new SignupPage());

        }

        private async void btnSignIn_Clicked(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                string error = await DeviceAuthentication.EmailSignIn(txtEmail.Text, txtPassword.Text);
                if (!string.IsNullOrWhiteSpace(error))
                    await App.Current.MainPage.DisplayAlert("Alert", error, "OK");

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
                await App.Current.MainPage.DisplayAlert("Alert", Properties.Resources.NoInternetText, "OK");
            }
        }
    }
}