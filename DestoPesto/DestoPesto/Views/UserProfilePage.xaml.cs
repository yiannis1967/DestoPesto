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
using Xamarin.Forms.Xaml;

namespace DestoPesto.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfilePage : ContentPage,INotifyPropertyChanged
    {
        public UserProfilePage()
        {

            InitializeComponent();
            

            User User = Authentication.DeviceAuthentication.AuthUser.Tag as User;

            Name=User?.FirstName;
            Email=User?.Email;
            PhoneNumber=User?.PhoneNumber;
            BirthDate=User?.DateOfBirth;
            if (BirthDate!=null)
                BirthDateText= BirthDate.Value.ToShortDateString();

            this.BindingContext = this;



        }

        protected override void OnDisappearing()
        {
            User User = Authentication.DeviceAuthentication.AuthUser.Tag as User;
            if(User != null)
            {
                User.FirstName = Name;
                User.Email = Email;
                User.PhoneNumber = PhoneNumber;
                User.DateOfBirth = BirthDate;
                JsonHandler.UpdateUser(User);
            }
            base.OnDisappearing();
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string BirthDateText { get; set; }

        public DateTime? BirthDate { get; set; }

        private void Button_Clicked(object sender, EventArgs e)
        {
           
        }

        private void BirthDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            birthDatePicker.DateSelected-=BirthDatePicker_DateSelected;
            BirthDate=birthDatePicker.Date;
            if (BirthDate!=null)
                BirthDateText= BirthDate.Value.ToShortDateString();

            
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void Entry_Focused(object sender, FocusEventArgs e)
        {

        }

        private void birthDateEntry_Focused(object sender, FocusEventArgs e)
        {

            if (BirthDate.HasValue)
                birthDatePicker.Date=BirthDate.Value;
            //birthDatePicker.DateSelected+=BirthDatePicker_DateSelected;
            birthDatePicker.Unfocused+=BirthDatePicker_Unfocused;
            birthDatePicker.IsEnabled = true;
            birthDateEntry.IsEnabled=false;
            birthDatePicker.Focus();
            birthDateEntry.IsEnabled=true;
        }

        private void BirthDatePicker_Unfocused(object sender, FocusEventArgs e)
        {
            birthDatePicker.Unfocused-=BirthDatePicker_Unfocused;
            BirthDate=birthDatePicker.Date;
            if (BirthDate!=null)
                BirthDateText= BirthDate.Value.ToShortDateString();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BirthDateText)));
        }
    }
}