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

        public string Message { get; private set; }
    }
}