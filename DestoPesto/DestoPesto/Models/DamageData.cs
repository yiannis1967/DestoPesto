using DestoPesto.Services;
using DestoPesto.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DestoPesto.Models
{
    public class DamageData : INotifyPropertyChanged
    {

        public DamageData()
        {
            FixedCommand = new Command(new Action(async () =>
            {
                await Fixed();

            }));

            CloseCommand = new Command(new Action(async () =>
            {
                IsExpanded = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));

            }));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task<bool> Fixed()
        {
            if (!await MessageDialogPopup.DisplayPopUp(DestoPesto.Properties.Resources.ApplicationName, DestoPesto.Properties.Resources.WarningForUserBanMessage, DestoPesto.Properties.Resources.YesText, DestoPesto.Properties.Resources.NoText))
            {
                return false;
            }



            FixdDamage fix = new FixdDamage();
            fix.id = id;
            DateTime dt = DateTime.Now;
            string month = dt.Month.ToString();
            if (dt.Month < 10)
            {
                month = "0" + month;


            }
            string day = dt.Day.ToString();
            if (dt.Day < 10)
            {
                day = "0" + day;


            }
            string hour = dt.Hour.ToString();
            if (dt.Hour < 10)
            {
                hour = "0" + hour;


            }
            string min = dt.Minute.ToString();
            if (dt.Minute < 10)
            {
                min = "0" + min;


            }
            fix.fixedDate = dt.Year + "-" + month + "-" + day + "T00:" + hour + ":" + min + ".315Z";
            //await getUserData();
            if (Authentication.DeviceAuthentication.AuthUser == null)
            {
                //await Shell.Current.Navigation.PushAsync(new LoginPage());
                await Shell.Current.GoToAsync("//LoginPage");
                return false;
            }


            string sss = Authentication.DeviceAuthentication.IDToken;
            //fix.userId = userEmail;


            (App_s.Current as App_s).RemoveUserSubmittedDamage(this);
            if (await JsonHandler.PutSubmission(fix))
            {
                return true;
            }
            return false;


        }

        public string id { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string category { get; set; }
        public int numberOfUsers { get; set; }
        public DateTime firstDateReported { get; set; }
        public DateTime lastDateReported { get; set; }
        public string photoUrl { get; set; }

        public bool IsActivate { get; set; }

        public bool Review { get => !IsActivate; set { } }

        public string fullAddress { get; set; }

        public string CategoryName
        {
            get
            {
                return DestoPesto.Services.JsonHandler.GetCatagory(int.Parse(category));
            }
            set
            {

            }
        }

        public string MarkIconUri
        {
            get
            {
                return DestoPesto.Services.JsonHandler.GetCatagoryMarkIconUri(int.Parse(category));
            }
            set
            {

            }
        }

        public string IconFileName
        {
            get
            {
                return DestoPesto.Services.JsonHandler.GetCategoryIconFileName(int.Parse(category));
            }
        }

        [JsonIgnore]
        public string SubmissionsNumberPrompt
        {
            get
            {

                return Properties.Resources.NumUserText + " " + numberOfUsers.ToString();

            }
            set
            {

            }
        }

        public Command FixedCommand { get; set; }


        public Command CloseCommand { get; set; }

        public bool IsExpanded { get; set; }
    }
}
