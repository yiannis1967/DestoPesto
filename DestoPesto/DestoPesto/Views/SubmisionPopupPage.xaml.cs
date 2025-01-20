using Rg.Plugins.Popup.Services;
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
    public partial class SubmisionPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public SubmisionPopupPage(string description, string submisionThumb, string comments, Xamarin.Essentials.ShareTextRequest shareTextRequest)
        {
            Description = description;
            SubmisionThumb = submisionThumb;
            Comments = comments;
            InitializeComponent();
            //IsRepairedHyperlink.Clicked+=IsRepairedHyperlink_Clicked;

            ShareTextRequest = shareTextRequest;
            ShareVisible = shareTextRequest != null;
            BindingContext = this;
        }

       public bool ShareVisible { get; set; }

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
        public ShareTextRequest ShareTextRequest { get; private set; }

        private async void ShareBtn_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Subject = ShareTextRequest?.Subject,
                Text = ShareTextRequest?.Uri,
                Title = ShareTextRequest?.Title,
                Uri = ShareTextRequest?.Uri,
            });
        }
    }
}