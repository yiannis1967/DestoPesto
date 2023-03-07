using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FCMTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window ,System.ComponentModel.INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            //dXrguneiDxg:APA91bHEqKEUfTUMAr6wJowMC4nDgPeg6Qn6JnCtr4mb3cfKbLBBh_zvLOofgpN-2uMaNIs987spk7p0DZNMneXgG7-47gEppVom_Lt2L04DxSgCJq-5FJgLVr-JJSLQQV4KgawugzPL
            CloudNotificationManager.SendMessage(DeviceID,Guid.NewGuid().ToString("N"), $"Hello from Desto {Environment.NewLine} Hello from Desto {Environment.NewLine} Hello from Desto", "Desto", "https://asfameazure.blob.core.windows.net/destopesto/images/019b894f-c22f-4be0-ae1f-0c700431dd7d.png");
        }

        public string DeviceID { get; set; } = "d5MzkRILQhGdMpWK9n10fW:APA91bFUfIZXRQm2f6ZdRA6nQEaeRm-dphn9bcD4UVlBXF85Bcla0X4kGUOVko-cBgOMvNXg3QrMXxvy1LUX_foORl-DACVjhrA9lLx_jN6eOft5u8r6WZZCZGsgp1xfj8IeArXxmGcU";
    }
}
