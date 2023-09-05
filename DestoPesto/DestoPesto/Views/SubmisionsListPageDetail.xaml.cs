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

            //(App.Current as App).SubmittedDamageUser[0].category
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