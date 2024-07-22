using DestoPesto.Models;
using DestoPesto.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfilePage : ContentPage, INotifyPropertyChanged
    {
        public UserProfilePage()
        {

            InitializeComponent();

            IsAndroid = Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.Android;

            iOS = Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS;

            User User = Authentication.DeviceAuthentication.AuthUser.Tag as User;

            Name = User?.FirstName;
            Email = User?.Email;
            PhoneNumber = User?.PhoneNumber;
            BirthDate = User?.DateOfBirth;


            ContestAcceptedPhotosText = User?.PromoAcceptedPhotos.ToString();
            AcceptedPhotosText = User?.AcceptedPhotos.ToString();

            RejectedPhotosText = User?.RejectedPhotos.ToString();


            if (BirthDate != null)
                BirthDateText = BirthDate.Value.ToShortDateString();

            this.BindingContext = this;

            iOsBirthDatePicker.Unfocused += BirthDatePicker_Unfocused;

            if (BirthDate.HasValue)
            {
                birthDatePicker.Date = BirthDate.Value;
                iOsBirthDatePicker.Date = BirthDate.Value;
            }


        }

        public bool ContestParticipation
        {
            get
            {
                User User = Authentication.DeviceAuthentication.AuthUser.Tag as User;
                if (User?.PromoContest != null)
                    return true;
                else
                    return false;
            }
        }

        protected override void OnDisappearing()
        {
            try
            {
                User User = Authentication.DeviceAuthentication.AuthUser.Tag as User;
                if (User != null)
                {
                    User.FirstName = Name;
                    User.Email = Email;
                    User.PhoneNumber = PhoneNumber;
                    User.DateOfBirth = BirthDate;
                    JsonHandler.UpdateUser(User);
                }

            }
            catch (Exception error)
            {
            }
            base.OnDisappearing();
        }

        public bool IsAndroid { get; set; }
        public bool iOS { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string BirthDateText { get; set; }

        public DateTime? BirthDate { get; set; }
        public string ContestAcceptedPhotosText { get; }
        public string AcceptedPhotosText { get; }
        public string RejectedPhotosText { get; private set; }

        private void Button_Clicked(object sender, EventArgs e)
        {

        }

        private void BirthDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {

            BirthDate = birthDatePicker.Date;
            if (BirthDate != null)
                BirthDateText = BirthDate.Value.ToShortDateString();


        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void Entry_Focused(object sender, FocusEventArgs e)
        {

        }

        private void birthDateEntry_Focused(object sender, FocusEventArgs e)
        {

            if (IsAndroid)
                birthDatePicker.Unfocused += BirthDatePicker_Unfocused;
            //birthDatePicker.DateSelected+=BirthDatePicker_DateSelected;

            birthDatePicker.IsEnabled = true;
            birthDateEntry.IsEnabled = false;
            birthDatePicker.Focus();
            birthDateEntry.IsEnabled = true;
        }

        private void BirthDatePicker_Unfocused(object sender, FocusEventArgs e)
        {
            birthDatePicker.Unfocused -= BirthDatePicker_Unfocused;

            if (IsAndroid)
                BirthDate = birthDatePicker.Date;
            if (iOS)
                BirthDate = iOsBirthDatePicker.Date;
            iOS = Xamarin.Essentials.DeviceInfo.Platform == Xamarin.Essentials.DevicePlatform.iOS;

            if (BirthDate != null)
                BirthDateText = BirthDate.Value.ToShortDateString();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BirthDateText)));
        }
    }
}