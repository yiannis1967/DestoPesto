using System.ComponentModel;
using DestoPesto.ViewModels;
using Xamarin.Forms;

namespace DestoPesto.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}