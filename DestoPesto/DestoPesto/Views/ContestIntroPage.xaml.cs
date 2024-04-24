using DestoPesto.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static System.Net.WebRequestMethods;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContestIntroPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public ContestIntroPage(Models.PromoContest promoContest)
        {
            InitializeComponent();
            PromoContest = promoContest;
            BindingContext=promoContest;
            //browser.Source=promoContest.InfoUrl;// "https://dotnet.microsoft.com/apps/xamarin";
        }



        Models.PromoContest PromoContest;

        bool DialogResult = false;
        static TaskCompletionSource<bool> task;
        public static Task<bool> DisplayPopUp(Models.PromoContest promoContest)
        {
            task = new TaskCompletionSource<bool>();

            if (PopupNavigation.Instance.PopupStack.Count > 0)
                PopupNavigation.Instance.PopAsync();

            PopupNavigation.Instance.PushAsync(new ContestIntroPage(promoContest));

            return task.Task;
        }

        async void howto_Clicked(System.Object sender, System.EventArgs e)
        {
            string url = Properties.Resources.HowItWorksLink;
            await Launcher.OpenAsync(url);
        }
        private string _MobileHomePage;
        public string MobileHomePage
        {
            get
            {
                if (_MobileHomePage == null)
                {
                    WebClient client = new WebClient();
                    _MobileHomePage = client.DownloadString(Properties.Resources.HomeScreenMobileLink);
                }
                return _MobileHomePage;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            task.SetResult(DialogResult);
        }
        //


        private async void OKBtn_Clicked(object sender, EventArgs e)
        {

            JsonHandler.ParticipateToContest(PromoContest);
            DialogResult = false;
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                PopupNavigation.Instance.PopAsync();
            DialogResult = true;

        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            JsonHandler.IgnoreContest(PromoContest);
            DialogResult = false;
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                PopupNavigation.Instance.PopAsync();

        }
    }
}