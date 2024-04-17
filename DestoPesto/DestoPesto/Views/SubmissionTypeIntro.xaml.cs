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
        public SubmissionTypeIntro(Catagories submissionType )
        {
            InitializeComponent();
            SubmissionType = submissionType;
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


        public static Task<bool> DisplayPopUp()
        {
            task = new TaskCompletionSource<bool>();
            PopupNavigation.Instance.PushAsync(new IntroPage());

            return task.Task;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            task.SetResult(DontShowAgain);
        }
    }
}