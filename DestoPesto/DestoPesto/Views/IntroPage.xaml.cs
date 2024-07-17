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
    public partial class IntroPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        private static TaskCompletionSource<bool> task;

        public IntroPage()
        {
            InitializeComponent();

            try
            {
                _DontShowAgain = Xamarin.Essentials.Preferences.Get("IntroDontShowAgain", false);
            }
            catch (Exception)
            {

                throw;
            }
            BindingContext = this;

        }

        bool _DontShowAgain;
        public bool DontShowAgain
        {
            get => _DontShowAgain; set
            {
                _DontShowAgain = value;
                Xamarin.Essentials.Preferences.Set("IntroDontShowAgain", value);
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

        private void RightBtn_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                PopupNavigation.Instance.PopAsync();

        }
    }
}