using DestoPesto.Models;
using DestoPesto.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubmisionsListPageDetail : ContentPage
    {
        public SubmisionsListPageDetail()
        {
            InitializeComponent();

            BindingContext = this;


            (App_s.Current as App_s).PropertyChanged += SubmisionsListPageDetail_PropertyChanged;
            
            UserSubmissions = (App_s.Current as App_s).UserSubmissions;

            //(App.Current as App).SubmittedDamageUser[0].category
        }

        public bool Loading { get; set; }
        protected override async void OnAppearing()
        {

            Loading = true;
            OnPropertyChanged(nameof(Loading));

            Task.Run( async () =>
            {

                await UserSubmissions.GetAllUserDamages();
                MainThread.BeginInvokeOnMainThread(() =>
                {


                    

                    var userSubmissions = UserSubmissions;
                    UserSubmissions = null;
                    OnPropertyChanged(nameof(UserSubmissions));
                    UserSubmissions = userSubmissions;
                    OnPropertyChanged(nameof(UserSubmissions));
                    Loading = false;
                    OnPropertyChanged(nameof(Loading));
                });
            });
            base.OnAppearing();
        }



        private void SubmisionsListPageDetail_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {



        }

        private void SubmisionsListPageDetail_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {

        }



        //List<DamageData> _Damages= new List<DamageData>();
        //public List<DamageData> Damages
        //{
        //    get
        //    {
        //        return _Damages;
        //    }
        //}

        public UserSubmissions UserSubmissions { get; private set; }
    }
}