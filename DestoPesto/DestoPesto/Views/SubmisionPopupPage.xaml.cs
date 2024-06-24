using Rg.Plugins.Popup.Services;
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
    public partial class SubmisionPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public SubmisionPopupPage(string description, string submisionThumb, string comments)
        {
            Description= description;
            SubmisionThumb=submisionThumb;
            Comments= comments;
            InitializeComponent();
            //IsRepairedHyperlink.Clicked+=IsRepairedHyperlink_Clicked;

            BindingContext=this;
            


        }
    
        async void OnImageNameTapped(object sender, EventArgs args)
        {
            try
            {
                if (PopupNavigation.Instance.PopupStack.Count > 0)
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
                //Code to execute on tapped event
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void IsRepairedHyperlink_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }


        public string SubmisionThumb { get; set; }// = "https://destopesto.blob.core.windows.net/destopesto/images/eb25c486-14cf-42f6-b300-0f8107c1df21.jpg";

        public string Description { get; set; }

        public string Comments { get; set; }


    }
}