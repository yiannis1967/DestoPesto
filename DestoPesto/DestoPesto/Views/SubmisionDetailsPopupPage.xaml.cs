using DestoPesto.Models;
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
    public partial class SubmisionDetailsPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        public SubmisionDetailsPopupPage(DamageData submission, Xamarin.Essentials.Location locaion)
        {

            Submission = submission;
            //Description= description;
            //SubmisionThumb=submisionThumb;
            //Comments= comments;
            InitializeComponent();
            //IsRepairedHyperlink.Clicked+=IsRepairedHyperlink_Clicked;

           


            var distance = Xamarin.Essentials.LocationExtensions.CalculateDistance(locaion, submission.lat,submission.lng, Xamarin.Essentials.DistanceUnits.Kilometers)*1000;
            if (distance> Services.JsonHandler.MaxDistanceForFixed_meters)
            {
                enableFixButton=false;
            }
            else
            {
                FixedCommand=submission?.FixedCommand;
                enableFixButton =true;
            }

            BindingContext=this;
            



        }
        public Command FixedCommand { get; set; }
        public Color ButtonColor
        {
            get
            {
                if (enableFixButton)
                    return Color.FromHex("#2196F3");
                else
                    return Color.Gray;
        }
        }
        public DamageData Submission { get; } 
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
        public bool enableFixButton { get; private set; }

        private async void Fixed_Clicked(object sender, EventArgs e)
        {
            bool FixedResult = await Submission?.Fixed();
            if (FixedResult!=false)
                if (PopupNavigation.Instance.PopupStack.Count > 0)
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();

        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }
    }
}