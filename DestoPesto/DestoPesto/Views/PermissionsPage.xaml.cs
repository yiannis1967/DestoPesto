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
    public partial class PermissionsPage : ContentPage
    {
        private bool LocationPermisionsChecked;

        public PermissionsPage()
        {
            InitializeComponent();
        }

        private async void continueBtn_Clicked(object sender, EventArgs e)
        {
            //if (await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ApplicationName, DestoPesto.Properties.Resources.LocationPrompt, DestoPesto.Properties.Resources.TurnOn, DestoPesto.Properties.Resources.TurnOff))
            {
                try
                {
                    var locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                    if (locationInUsePermisions != PermissionStatus.Granted)
                    {
                        ////if (DeviceInfo.Version >= version)
                        //if (locationInUsePermisions != PermissionStatus.Granted)
                        //{
                        //    locationInUsePermisions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                        //    if (locationInUsePermisions != PermissionStatus.Granted)
                        //        LocationPermisionsChecked = true;
                        //}
                        //else
                        //    LocationPermisionsChecked = true;

                        //if (locationInUsePermisions == PermissionStatus.Granted)
                            locationInUsePermisions = await Permissions.RequestAsync<Permissions.LocationAlways>();
                    }

                    locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    if (locationInUsePermisions == PermissionStatus.Granted)
                    {
                        var cameraPermisions = await Permissions.CheckStatusAsync<Permissions.Camera>();
                        if (cameraPermisions == PermissionStatus.Granted)
                            App.Current.MainPage=new AppShell();
                        else
                        {
                            cameraPermisions = await Permissions.RequestAsync<Permissions.Camera>();
                            if (cameraPermisions == PermissionStatus.Granted)
                                App.Current.MainPage = new AppShell();

                        }
                    }
                        

                }
                finally
                {
                }
            }



        }
    }
}