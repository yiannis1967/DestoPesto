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
        public ContestIntroPage()
        {
            InitializeComponent();
        }

        bool DialogResult = false;
        static TaskCompletionSource<bool> task;
        public static Task<bool> DisplayPopUp()
        {
            task = new TaskCompletionSource<bool>();
            PopupNavigation.Instance.PushAsync(new ContestIntroPage());

            return task.Task;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            task.SetResult(DialogResult);
        }

    }
}