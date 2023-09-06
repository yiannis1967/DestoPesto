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
    public partial class SignInMessagePopUp : Rg.Plugins.Popup.Pages.PopupPage
    {
        public SignInMessagePopUp(string message)
        {
            InitializeComponent();
            Message=message;
            this.BindingContext=this;
            
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            task?.SetResult(true);
        }

        public string Message { get; private set; }

        static TaskCompletionSource<bool> task;

        public static Task<bool> DisplayPopUp()
        {
            task = new TaskCompletionSource<bool>();
            PopupNavigation.Instance.PushAsync(new SignInMessagePopUp("error"));

            return task.Task;
        }
    }
}