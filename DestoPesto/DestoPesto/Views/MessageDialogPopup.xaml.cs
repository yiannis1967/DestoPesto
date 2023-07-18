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
    /// <MetaDataID>{f5403d97-b228-43db-b882-31b5401320f8}</MetaDataID>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageDialogPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        public MessageDialogPopup(string title, string message, string accept, string cancel)
        {
            InitializeComponent();

            
            
            MessageDialogTitle=title;
            MessageDialogMessage=message;
            RightBtnText=accept;
            LeftBtnText=cancel;
            BindingContext=this;
        }

        public MessageDialogPopup(string title, string message, string cancel)
        {
            InitializeComponent();

            
            MessageDialogTitle=title;
            MessageDialogMessage=message;
            RightBtnText=cancel;
            LeftBtn.IsVisible=false;
            BindingContext=this;
        }


        static TaskCompletionSource<bool> task;

        public static Task<bool> DisplayPopUp(string title, string message, string accept, string cancel)
        {
            task = new TaskCompletionSource<bool>();
            PopupNavigation.Instance.PushAsync(new MessageDialogPopup(title,message,accept,cancel));

            return task.Task;

        }
        public static Task<bool> DisplayPopUp(string title, string message,  string cancel)
        {
            task = new TaskCompletionSource<bool>();
            PopupNavigation.Instance.PushAsync(new MessageDialogPopup(title, message,  cancel));

            return task.Task;
        }

        bool DialogResult = false;

        public string MessageDialogTitle { get; private set; }
        public string MessageDialogMessage { get; private set; }
        public string RightBtnText { get; private set; }
        public string LeftBtnText { get; private set; }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            task.SetResult(DialogResult);
        }



        private void RightBtn_Clicked(object sender, EventArgs e)
        {
            DialogResult=true;
            PopupNavigation.Instance.PopAsync();
        }

        private void LeftBtn_Clicked(object sender, EventArgs e)
        {
            
            DialogResult=false;
            PopupNavigation.Instance.PopAsync();
        }
        
    }
}