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
    /// <MetaDataID>{5a13c166-c2d0-4b59-af6f-5c15b4707142}</MetaDataID>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MunicipalityStatsPopUp : Rg.Plugins.Popup.Pages.PopupPage
            
    {
        public MunicipalityStatsPopUp(MunicipalityStats municipalityStats )
        {
            InitializeComponent();


            MunicipalityStats = new MunicipalityStatsVM(municipalityStats);
            BindingContext = this;

        }

        public MunicipalityStatsVM MunicipalityStats { get; }

        private void RightBtn_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                PopupNavigation.Instance.PopAsync();
        }
    }
}