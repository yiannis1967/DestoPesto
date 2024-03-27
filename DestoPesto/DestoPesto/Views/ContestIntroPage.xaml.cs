using DestoPesto.Services;
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
    public partial class ContestIntroPage :  Rg.Plugins.Popup.Pages.PopupPage
    {
        public ContestIntroPage(Models.PromoContest promoContest)
        {
            InitializeComponent();
            PromoContest=promoContest;
        }

      

        Models.PromoContest PromoContest;

        bool DialogResult = false;
        static TaskCompletionSource<bool> task;
        public static Task<bool> DisplayPopUp(Models.PromoContest promoContest)
        {
            task = new TaskCompletionSource<bool>();
            PopupNavigation.Instance.PopAsync();
            PopupNavigation.Instance.PushAsync(new ContestIntroPage(promoContest));

            return task.Task;
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
            DialogResult =false;
            PopupNavigation.Instance.PopAsync();
            DialogResult=true;
            
        }

        private void CancelBtn_Clicked(object sender, EventArgs e)
        {
            DialogResult=false;
            PopupNavigation.Instance.PopAsync();
            
        }
    }
}