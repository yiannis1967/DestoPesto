using DestoPesto.Services;
using DestoPesto.ViewModels;
using DestoPesto.Views;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Auth.OAuth2;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DestoPesto
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext= this;


            //if (Authentication.DeviceAuthentication.AuthUser==null)

            // this.CurrentItem = this.PermissionsPage;

            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;
            Authentication.DeviceAuthentication.SignedOut();
            

        }

        private async void OnSubmisionsClicked(object sender, EventArgs e)
        {

            await Shell.Current.GoToAsync("//Submisions");
        }


        public string _AppVersion;
        public string AppVersion
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_AppVersion))
                    //_AppVersion = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionCode;

                    //Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;

                    _AppVersion = (App.Current as App).Version;
                return _AppVersion;
            }
        }

        private async void MenuItem_Clicked(object sender, EventArgs e)
        {


            if (await JsonHandler.RemoveUser())
            {
                Shell.Current.FlyoutIsPresented = false;
                Authentication.DeviceAuthentication.SignedOut();

            }

        }

        void HyperlinkLabel_Clicked(System.Object sender, System.EventArgs e)
        {
        }

        async void howto_Clicked(System.Object sender, System.EventArgs e)
        {
            string url = Properties.Resources.HowItWorksLink;
            await Launcher.OpenAsync(url);
        }

        async void vision_Clicked(object sender, EventArgs e)
        {
            string url = Properties.Resources.VisionLink;
            await Launcher.OpenAsync(url);
        }

        async void disclaimer_Clicked(object sender, EventArgs e)
        {
            string url = Properties.Resources.Accuracy_disclaimerLink;
            await Launcher.OpenAsync(url);
        }

      

        async void PrivacyPolicy_Clicked(object sender, EventArgs e)
        {
            string url = Properties.Resources.PrivacyPolicyLInk;
            await Launcher.OpenAsync(url);
        }

        async void CookiesPolicy_Clicked(object sender, EventArgs e)
        {
            string url = Properties.Resources.CookiesPolicyLInk;
            await Launcher.OpenAsync(url);
        }

        private async void UserProfile_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new UserProfilePage());
            Shell.Current.FlyoutIsPresented = false;
        }

        private void UserProfile_Clicked_1(object sender, EventArgs e)
        {

        }
    }
}

//Severity Code	Description	Project	File	Line	Suppression State
//Error		ADB0020: Android ABI mismatch. You are deploying an app supporting 'armeabi-v7a;arm64-v8a' ABIs to an incompatible device of ABI 'x86'. You should either create an emulator matching one of your app's ABIs or add 'x86' to the list of ABIs your app builds for.		 	0	

