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
            BindingContext=this;
            
        }

        public bool DontShowAgain { get;  set; }

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