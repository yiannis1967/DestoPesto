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

            var timeStamp = DateTime.UtcNow;
            

            //dXrguneiDxg:APA91bHEqKEUfTUMAr6wJowMC4nDgPeg6Qn6JnCtr4mb3cfKbLBBh_zvLOofgpN-2uMaNIs987spk7p0DZNMneXgG7-47gEppVom_Lt2L04DxSgCJq-5FJgLVr-JJSLQQV4KgawugzPL
            CloudNotificationManager.SendMessage(DeviceID,Guid.NewGuid().ToString("N"), $"Hello from Desto {Environment.NewLine} Hello from Desto {Environment.NewLine} Hello from Desto", "Desto", "https://destopesto.blob.core.windows.net/destopesto/images/019b894f-c22f-4be0-ae1f-0c700431dd7d.png");
        }

        public string DeviceID { get; set; } = "d7hT7X2t406cnpYof5n2Yv:APA91bEyIpgWnXlKA1golNNj2MAc9Dd04tR7TuuODkHQWXlYqXs72wMb-afDxY-fRv77Eb7jHdbn-Rt-jaTlpsDEXiXjGEmOlzT-w8HKglVEsSc4ypGYcAwyM_b9xhPnZ3vpCI8M8oeO";
    }
}
