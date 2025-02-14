using Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgotPasswordPage : ContentPage
    {
        public ForgotPasswordPage()
        {

            InitializeComponent();


            reset_password.Tapped += Reset_password_Tapped;
            ForgotPasswordCommand = new Command(Reset_password);
        }
        async void Reset_password()
        {
            try
            {
                DeviceAuthentication.SendPasswordResetEmail(Email);
            }
            catch (Exception error)
            {


            }
            await Shell.Current.Navigation.PushAsync(new PasswordResetPage());
        }
        public ForgotPasswordPage(string email) : this()
        {

            txtEmail.Text = Email = email;
        }
        public string Email;
        private async void Reset_password_Tapped(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Email = txtEmail.Text;
            }
            if (string.IsNullOrWhiteSpace(Email))
            {
                return;
            }
            try
            {
                DeviceAuthentication.SendPasswordResetEmail(Email);
            }
            catch (Exception error)
            {


            }
            await Shell.Current.Navigation.PushAsync(new LoginPage());

        }

        public TapGestureRecognizer reset_password = new TapGestureRecognizer();

        public Command ForgotPasswordCommand { get; }
    }
}