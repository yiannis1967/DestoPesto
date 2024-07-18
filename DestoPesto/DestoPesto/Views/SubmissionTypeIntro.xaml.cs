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
    public partial class SubmissionTypeIntro : Rg.Plugins.Popup.Pages.PopupPage
    {
        private static TaskCompletionSource<bool> task;

        Catagories SubmissionType;

        public string InfoText { get; private set; }
        public string Caption { get; private set; }

        public SubmissionTypeIntro(Catagories submissionType)
        {
            InitializeComponent();
            SubmissionType = submissionType;
            InfoText=submissionType.InfoText;
            Caption =submissionType.description_en;
            if (System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName.ToLower() != "ell")
            {
                InfoText=submissionType.InfoText_en;
                Caption  = submissionType.description_en;
            }


            _DontShowAgain= Xamarin.Essentials.Preferences.Get("SubmissionTypeIntroDontShowAgain" + SubmissionType.code, false);
            BindingContext =this;

        }


        bool _DontShowAgain;
        public bool DontShowAgain
        {
            get => _DontShowAgain; set
            {
                _DontShowAgain = value;
                Xamarin.Essentials.Preferences.Set("SubmissionTypeIntroDontShowAgain"+SubmissionType.code, value);
            }
        }


        public static  Task<bool> DisplayPopUp(Catagories submissionType)
        {
            
            if (Xamarin.Essentials.DeviceInfo.Platform!=Xamarin.Essentials.DevicePlatform.iOS&&Xamarin.Essentials.DeviceInfo.Platform!=Xamarin.Essentials.DevicePlatform.macOS)
            {
                var task = new TaskCompletionSource<bool>();

                PopupNavigation.Instance.PushAsync(new SubmissionTypeIntro(submissionType));

                return task.Task;
                


            }
            return Task.FromResult(true);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            task.SetResult(DontShowAgain);
        }

        private void RightBtn_Clicked(object sender, EventArgs e)
        {
            //DialogResult=true;
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                PopupNavigation.Instance.PopAsync();
        }
    }
}