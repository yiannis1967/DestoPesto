using DestoPesto.ViewModels;
using DestoPesto.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DestoPesto
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext= this;
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
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

    }
}

//Severity Code	Description	Project	File	Line	Suppression State
//Error		ADB0020: Android ABI mismatch. You are deploying an app supporting 'armeabi-v7a;arm64-v8a' ABIs to an incompatible device of ABI 'x86'. You should either create an emulator matching one of your app's ABIs or add 'x86' to the list of ABIs your app builds for.		 	0	

