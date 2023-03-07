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
    public partial class SubmisionPopupPage:Rg.Plugins.Popup.Pages.PopupPage 
    {
        public SubmisionPopupPage()
        {
            InitializeComponent();

            IsRepairedHyperlink.Clicked+=IsRepairedHyperlink_Clicked;

        }

        private async void IsRepairedHyperlink_Clicked(object sender, EventArgs e)
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }
    }
}