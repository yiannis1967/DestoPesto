using DestoPesto.Models;
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

            //(App.Current as App).SubmittedDamageUser[0].category
        }

        private void SubmisionsListPageDetail_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Damages));

        }

        private void SubmisionsListPageDetail_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
           
        }

        public List<DamageData> Damages
        {
            get
            {
                if ((App.Current as App).SubmittedDamageUser == null)
                    return new List<DamageData>();
                return (App.Current as App).SubmittedDamageUser.ToList();
            }
        }
    }
}