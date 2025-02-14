using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestoPesto.Models;
using DestoPesto.ViewModels;
using DestoPesto.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DestoPesto.Views
{
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            var myValue = Preferences.Get("onlyWifi", false);
            toggle.IsToggled = myValue;
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Preferences.Set("onlyWifi", e.Value);
        }
    }
}