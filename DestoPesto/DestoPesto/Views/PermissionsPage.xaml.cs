using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PermissionsPage : ContentPage
    {
        private bool LocationPermisionsChecked;

        public PermissionsPage()
        {
            InitializeComponent();

            if (Xamarin.Essentials.DeviceInfo.Platform == DevicePlatform.iOS)
            {
                this.TitleBar.Padding = new Thickness(20, 30, 20, 0);
            }

            var htmlSource = new HtmlWebViewSource();

//            htmlSource.Html = @"
//                                <h1>Δες το Πες το</h1>
//<h3>Το app του πολίτη!</h3>
//<p>Πόσες κακοτεχνίες συναντάς καθημερινά στον δρόμο σου; Λακκούβες, χαλασμένα φανάρια, σπασμένα πεζοδρόμια είναι ορισμένα μόνο από αυτά που κάνουν τη ζωή μας δύσκολη και ενίοτε επικίνδυνη, ειδικά στις ευπαθείς ομάδες.
//<br>Και το παράδοξο είναι ότι ποτέ δεν γνωρίζεις σε ποιο φορέα πρέπει να απευθυνθείς για την αποκατάστασή τους. </p>
//<p>Αυτά μέχρι χθες! Γιατί σήμερα υπάρχει το <b>ΔΕΣ ΤΟ - ΠΕΣ ΤΟ</b>.</p>
//Το πρωτοποριακό app που γίνεται ο καθημερινός σύμμαχος του πολίτη δίνοντας δύναμη στη φωνή του!
//<br><b>Μπορούμε  όλοι να συμβάλλουμε στην βελτίωση της καθημερινότητάς μας. </b>
//<br><b>Κάθε κακοτεχνία που διορθώνεται είναι κέρδος για όλους!</b></p>
//<p><h3>Απλή διαδικασία με 2 βήματα! </h3></p>
//<p><b>ΒΗΜΑ 1ο:</b> Μπες στο app και επίλεξε την κατηγορία (λακκούβα, φανάρι κτλ.)
//<br><b>ΒΗΜΑ 2ο:</b> Φωτογράφισε την κακοτεχνία και σε λίγα μόνο λεπτά θα ειδοποιηθείς αν η ανάρτησή σου είναι έγκυρη.
//<br><b>Από εκεί και πέρα οι αρμόδιοι φορείς δεν έχουν καμία δικαιολογία να μην αποκαταστήσουν το πρόβλημα εφόσον υπάρχει αναρτημένο.</b> 
//<br>Και αυτό γιατί οποιοσδήποτε μπαίνει στην εφαρμογή μπορεί να βλέπει στον χάρτη ανά κατηγορία τις κακοτεχνίες και τα προβλήματα. </p>
//<p>Επιπλέον μπορείς να: 
//<br>- παρακολουθείς αν η κακοτεχνία αποκαταστάθηκε 
//<br>- βλέπεις πόσοι άλλοι έχουν εντοπίσει το ίδιο πρόβλημα. 
//<br><b>Η τεχνητή νοημοσύνη στην υπηρεσία του πολίτη!</b>
//To ΔΕΣ ΤΟ – ΠΕΣ ΤΟ, σχεδιασμένο από την ARION SOFTWARE, αξιοποιεί τις δυνατότητες της τεχνητής νοημοσύνης. Κάθε φωτογραφία που ανεβαίνει έχει «ελεγχθεί» από μηχανισμό που βασίζεται στην <b>τεχνητή νοημοσύνη</b>, ώστε να διασφαλιστεί ότι είναι πραγματική. Αυτό εξασφαλίζει την απόλυτη αξιοπιστία της εφαρμογής, ενώ εμποδίζει την ανάρτηση μη σχετικών φωτογραφιών.
//</p>
//<p>Και φυσικά...
//<br><b>το app του Πολίτη είναι διαθέσιμο εντελώς ΔΩΡΕΑΝ για όλες τις συσκευές! </b>
//<br><b>Κατέβασέ τώρα το Δες το Πες το και δώσε δύναμη στη φωνή σου! </b></p>                               ";

            htmlSource.BaseUrl = DependencyService.Get<IBaseUrl>().Get();
            browser.Source = htmlSource;
        }

        private async void continueBtn_Clicked(object sender, EventArgs e)
        {
            //if (await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ApplicationName, DestoPesto.Properties.Resources.LocationPrompt, DestoPesto.Properties.Resources.TurnOn, DestoPesto.Properties.Resources.TurnOff))
            {
                try
                {
                    //AppInfo.ShowSettingsUI()

                    DateTime dateTime = DateTime.Now;

                    var device = Xamarin.Forms.DependencyService.Get<IDevice>();

                    if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Denied)
                    {
                        var result = await device.RemoteNotificationsPermissionsRequest();
                        if (result == PermissionStatus.Disabled)
                        {
                            await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ExitText, DestoPesto.Properties.Resources.TokenExpiredText, DestoPesto.Properties.Resources.Oktext);
                            return;
                        }


                    }
                    else if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Disabled)
                    {
                        await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ExitText, DestoPesto.Properties.Resources.TokenExpiredText, DestoPesto.Properties.Resources.Oktext);
                        return;
                    }




                    var locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                    if (locationInUsePermisions != PermissionStatus.Granted)
                    {
                        ////if (DeviceInfo.Version >= version)
                        //if (locationInUsePermisions != PermissionStatus.Granted)
                        //{
                        //    locationInUsePermisions = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                        //    if (locationInUsePermisions != PermissionStatus.Granted)
                        //        LocationPermisionsChecked = true;
                        //}
                        //else
                        //    LocationPermisionsChecked = true;

                        //if (locationInUsePermisions == PermissionStatus.Granted)
                        locationInUsePermisions = await Permissions.RequestAsync<Permissions.LocationAlways>();
                    }

                    locationInUsePermisions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    if (locationInUsePermisions == PermissionStatus.Granted)
                    {
                        var cameraPermisions = await Permissions.CheckStatusAsync<Permissions.Camera>();
                        if (cameraPermisions == PermissionStatus.Granted)
                        {

                        }
                        else
                        {
                            cameraPermisions = await Permissions.RequestAsync<Permissions.Camera>();

                        }
                    }
                    if (await device.RemoteNotificationsPermissionsCheck() == PermissionStatus.Granted &&
                        await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted &&
                        await Permissions.CheckStatusAsync<Permissions.Camera>() == PermissionStatus.Granted)
                    {
                        await device.PermissionsGranted();
                        App.Current.MainPage = new AppShell();

                    }
                    else
                    {
                        if ((DateTime.Now - dateTime).TotalSeconds < 2)
                        {
                            GoToSettings.IsVisible = true;
                            continueBtn.IsVisible = false;
                        }

                    }

                }
                finally
                {
                }
            }



        }

        private void GoToSettings_Clicked(object sender, EventArgs e)
        {
            AppInfo.ShowSettingsUI();
        }
    }
}