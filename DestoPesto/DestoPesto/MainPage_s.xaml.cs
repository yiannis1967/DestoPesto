using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DestoPesto.Views
{
    public partial class MainPage_s : ContentPage
    {
        public MainPage_s()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            
            this.DisplayAlert("Loading time", (DateTime.UtcNow - App_s.StartTime).TotalSeconds.ToString(),"OK");
        }
    }
}
