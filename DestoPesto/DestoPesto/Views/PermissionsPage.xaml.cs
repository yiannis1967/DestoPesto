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

            if(Xamarin.Essentials.DeviceInfo.Platform== DevicePlatform.iOS)
            {
                this.TitleBar.Padding = new Thickness(20, 30, 20, 0);
            }
        }

        private async void continueBtn_Clicked(object sender, EventArgs e)
        {
            //if (await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ApplicationName, DestoPesto.Properties.Resources.LocationPrompt, DestoPesto.Properties.Resources.TurnOn, DestoPesto.Properties.Resources.TurnOff))
            {
                try
                {
                    //AppInfo.ShowSettingsUI()

                    DateTime dateTime = DateTime.Now;

                    var device = Xamarin.Forms.DependencyService.Get<IDevice>();

                    if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Denied)
                    {
                        var result = await device.RemoteNotificationsPermissionsRequest();
                        if (result == PermissionStatus.Disabled)
                        {
                            await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ExitText, DestoPesto.Properties.Resources.TokenExpiredText, DestoPesto.Properties.Resources.Oktext);
                            return;
                        }
                            

                    }
                    else if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Disabled)
                    {
                        await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ExitText, DestoPesto.Properties.Resources.TokenExpiredText, DestoPesto.Properties.Resources.Oktext);
                        return;
                    }




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
                        {
                            
                        }
                        else
                        {
                            cameraPermisions = await Permissions.RequestAsync<Permissions.Camera>();

                        }
                    }
                    if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Granted && 
                        await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>()==PermissionStatus.Granted &&
                        await Permissions.CheckStatusAsync<Permissions.Camera>()==PermissionStatus.Granted)
                    {
                        device.PermissionsGranted();
                        App.Current.MainPage = new AppShell();

                    }
                    else
                    {
                        if ((DateTime.Now - dateTime).TotalSeconds < 2)
                        {
                            GoToSettings.IsVisible = true;
                            continueBtn.IsVisible = false;
                        }

                    }

                }
                finally
                {
                }
            }



        }

        private void GoToSettings_Clicked(object sender, EventArgs e)
        {
            AppInfo.ShowSettingsUI();
        }
    }
}