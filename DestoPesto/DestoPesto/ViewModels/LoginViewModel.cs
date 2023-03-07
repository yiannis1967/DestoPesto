using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DestoPesto.Views;
using Xamarin.Forms;

namespace DestoPesto.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
   

        public LoginViewModel(INavigation _navigation)
        {
            Navigation = _navigation;
           
        }

        private readonly INavigation Navigation;

       

        #region Properties

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
             //   if (SetPropertyValue(ref _email, value))
                {
                    ((Command)LoginCommand).ChangeCanExecute();
                }
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
              //  if (SetPropertyValue(ref _password, value))
                {
                    ((Command)LoginCommand).ChangeCanExecute();
                }
            }
        }

        private bool _isShowCancel;
        public bool IsShowCancel
        {
            get { return _isShowCancel; }
            set {

                // SetPropertyValue(ref _isShowCancel, value);

                _isShowCancel = value;
            }
        }

        #endregion


        #region Commands

        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get { return _loginCommand = _loginCommand ?? new Command(LoginAction, CanLoginAction); }
        }

        private ICommand _cancelLoginCommand;
        public ICommand CancelLoginCommand
        {
            get { return _cancelLoginCommand = _cancelLoginCommand ?? new Command(CancelLoginAction); }
        }

        private ICommand _forgotPasswordCommand;
        public ICommand ForgotPasswordCommand
        {
            get { return _forgotPasswordCommand = _forgotPasswordCommand ?? new Command(ForgotPasswordAction); }
        }

        private ICommand _newAccountCommand;
        public ICommand NewAccountCommand
        {
            get { return _newAccountCommand = _newAccountCommand ?? new Command(NewAccountAction); }
        }

        #endregion


        #region Methods

        bool CanLoginAction()
        {
            //Perform your "Can Login?" logic here...

            if (string.IsNullOrWhiteSpace(this.Email) || string.IsNullOrWhiteSpace(this.Password))
                return false;

            return true;
        }

        async void LoginAction()
        {
            IsBusy = true;

            //TODO - perform your login action + navigate to the next page

            //Simulate an API call to show busy/progress indicator            
            Task.Delay(20000).ContinueWith((t) => IsBusy = false);

            //Show the Cancel button after X seconds
            Task.Delay(5000).ContinueWith((t) => IsShowCancel = true);
        }

        void CancelLoginAction()
        {
            //TODO - perform cancellation logic

            IsBusy = false;
            IsShowCancel = false;
        }

        void ForgotPasswordAction()
        {
            //TODO - navigate to your forgotten password page
            //Navigation.PushAsync(XXX);
        }

        void NewAccountAction()
        {
            //TODO - navigate to your registration page
            //Navigation.PushAsync(XXX);
        }

        #endregion
    }

}

