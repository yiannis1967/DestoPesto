using DestoPesto.Models;
using DestoPesto.Services;
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
    public partial class SubmisionsListPageDetail : ContentPage
    {
        public SubmisionsListPageDetail()
        {
            InitializeComponent();

            BindingContext= this;


            (App.Current as App).PropertyChanged += SubmisionsListPageDetail_PropertyChanged;
            UserSubmissions = (App.Current as App).UserSubmissions;

            //(App.Current as App).SubmittedDamageUser[0].category
        }
        protected override async void OnAppearing()
        {

            await UserSubmissions.GetAllUserDamages();

            



            OnPropertyChanged(nameof(UserSubmissions));
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

        public UserSubmissions UserSubmissions { get; }
    }
}