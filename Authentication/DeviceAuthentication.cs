using System;

using System.Collections.Generic;

using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;
using Xamarin.Forms.PlatformConfiguration;
using System.Threading.Tasks;

namespace Authentication
{


    /// <MetaDataID>{44ed95c5-3794-4f1a-bec3-3664e0b7c529}</MetaDataID>
    public class DeviceAuthentication
    {

        public static void FacebookSignIn()
        {

            try
            {
                IAuthentication authentication = DependencyService.Get<IAuthentication>();
                authentication.FacebookSignIn();
            }
            catch (Exception error)
            {
                
            }

        }

        public static System.Threading.Tasks.Task<string> EmailSignUp(string email, string password)
        {
            IAuthentication authentication = DependencyService.Get<IAuthentication>();
            return authentication.EmailSignUp(email, password);
        }

        public static System.Threading.Tasks.Task<string> EmailSignIn(string email, string password)
        {
            IAuthentication authentication = DependencyService.Get<IAuthentication>();

            return authentication.EmailSignIn(email, password);
        }

        public static void SendPasswordResetEmail(string email)
        {
            IAuthentication authentication = DependencyService.Get<IAuthentication>();
            authentication.SendPasswordResetEmail(email);
        }


        public static void GoogleSignIn()
        {
            try
            {
                IAuthentication authentication = DependencyService.Get<IAuthentication>();
                authentication.GoogleSignIn();
            }
            catch (Exception error)
            {
            }
        }


        public DeviceAuthentication()
        {

        }

        static DeviceAuthentication _Current;
        public static DeviceAuthentication Current
        {
            get
            {
                if (_Current == null)
                    _Current = new DeviceAuthentication();

                return _Current;
                //return DeviceAuthentication.GetInstance(typeof(DeviceAuthentication), true) as DeviceAuthentication;
            }
        }


        static System.Threading.Timer Timer;

        static DeviceAuthentication()
        {

            Timer = new System.Threading.Timer(new System.Threading.TimerCallback(OnTimer), null, 500, 500);
        }
        static void OnTimer(object state)
        {
            if (_AuthUser!=null)
            {
                if (_AuthUser.ExpirationTime<DateTime.Now)
                {

                }
                if (((_AuthUser.ExpirationTime- DateTime.Now).TotalMinutes<2))
                {

                }
            }

        }





        public static event EventHandler<AuthUser> AuthStateChanged;



        public async Task<bool> AuthIDTokenChanged(string idToken, DateTime expirationTime, AuthUser authUserData)
        {
            if (IDToken != idToken)
            {
                _UnInitialized = true;
                if (string.IsNullOrWhiteSpace(idToken))
                {
                    _AuthUser = null;
                    IDToken = idToken;
                    AuthStateChanged?.Invoke(this, _AuthUser);
                    return true;
                }
                else
                {
                    _AuthUser = GetAuthData(idToken);
                    if (_AuthUser == null && authUserData != null)
                    {
                        _AuthUser = new AuthUser()
                        {
                            AuthToken = idToken,
                            ExpirationTime = expirationTime,
                            Email = authUserData.Email,
                            Email_Verified = authUserData.IsEmailVerified,
                            Firebase_Sign_in_Provider = authUserData.ProviderId,
                            Name = authUserData.DisplayName,
                            Picture = authUserData.PhotoUrl,
                            User_ID = authUserData.Uid
                        };
                    }

                    IDToken = idToken;
                    _UnInitialized = false;

                    if(_AuthUser != null)
                    {

                        await DebugLog.AppEventLog.Log("bool AuthIDTokenChanged   User signed in "+_AuthUser.User_ID);
                    }

                    AuthStateChanged?.Invoke(this, _AuthUser);
                    return true;

                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(IDToken) && _AuthUser == null)
                {
                    _AuthUser = GetAuthData(idToken);
                    if (_AuthUser == null && authUserData != null)
                    {
                        _AuthUser = new AuthUser()
                        {
                            AuthToken = idToken,
                            ExpirationTime = expirationTime,
                            Email = authUserData.Email,
                            Email_Verified = authUserData.IsEmailVerified,
                            Firebase_Sign_in_Provider = authUserData.ProviderId,
                            Name = authUserData.DisplayName,
                            Picture = authUserData.PhotoUrl,
                            User_ID = authUserData.Uid
                        };
                        AuthStateChanged?.Invoke(this, _AuthUser);
                    }
                }

            }
            return true;
            
        }

        /// <MetaDataID>{34df0860-c61e-41c6-80cc-8c5a3b1d9d65}</MetaDataID>
        public static string IDToken;

        /// <MetaDataID>{4d9077fe-5041-4ef2-9bcc-fd1d5008970e}</MetaDataID>

        public bool AuthIDTokenChanged(string idToken, AuthUser authUserData)
        {
            //System.Reflection.di


            if (IDToken != idToken)
            {
                _UnInitialized = true;

                if (string.IsNullOrWhiteSpace(idToken))
                {
                    _AuthUser = null;
                    IDToken = idToken;
                    AuthStateChanged?.Invoke(this, _AuthUser);
                    return true;
                }
                else
                {
                    _AuthUser = GetAuthData(idToken);
                    if (_AuthUser == null && authUserData != null)
                    {
                        _AuthUser = new AuthUser()
                        {
                            AuthToken = idToken,
                            Email = authUserData.Email,
                            Email_Verified = authUserData.IsEmailVerified,
                            Firebase_Sign_in_Provider = authUserData.ProviderId,
                            Name = authUserData.DisplayName,
                            Picture = authUserData.PhotoUrl,
                            User_ID = authUserData.Uid
                        };
                    }

                    IDToken = idToken;
                    _UnInitialized = false;
                    AuthStateChanged?.Invoke(this, _AuthUser);

                    return true;

                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(IDToken) && _AuthUser == null)
                {
                    _AuthUser = GetAuthData(idToken);
                    if (_AuthUser == null && authUserData != null)
                    {
                        _AuthUser = new AuthUser()
                        {
                            AuthToken = idToken,
                            Email = authUserData.Email,
                            Email_Verified = authUserData.IsEmailVerified,
                            Firebase_Sign_in_Provider = authUserData.ProviderId,
                            Name = authUserData.DisplayName,
                            Picture = authUserData.PhotoUrl,
                            User_ID = authUserData.Uid
                        };
                        AuthStateChanged?.Invoke(this, _AuthUser);
                    }
                }

            }
            return true;
        }
        internal void InternalAuthIDTokenChanged(string idToken, AuthUser authUser)
        {

            IDToken = idToken;
            if (!string.IsNullOrWhiteSpace(idToken) && authUser != null)
                _AuthUser = authUser;
            else
                _AuthUser = authUser;

        }

#if DeviceDotNet
        [OOAdvantech.MetaDataRepository.HttpVisible]
        public OOAdvantech.Authentication.IAuth Auth
        {
            get
            {
                return OOAdvantech.Authentication.OOAdvantechAuth.Auth;
            }
        }
#endif


        static bool _UnInitialized;
        public static bool UnInitialized
        {
            get
            {
                return _UnInitialized;
            }
        }

        static AuthUser _AuthUser;
        static public AuthUser AuthUser
        {
            get
            {

                return _AuthUser;
            }
        }



        public static void SignedIn(AuthUser authUser)
        {
            _AuthUser = authUser;
            AuthStateChanged?.Invoke(null, _AuthUser);
        }
        public static void SignedOut()
        {

            IAuthentication authentication = DependencyService.Get<IAuthentication>();
            authentication.SignOut();
            _AuthUser = null;
            AuthStateChanged?.Invoke(null, _AuthUser);

        }


        internal AuthUser GetAuthData(string authToken)
        {


            return null;

        }


        /// <MetaDataID>{dc8b5103-ea81-40bb-97d0-3a59ff766059}</MetaDataID>
        public static DateTime FromUnixTime(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }

        public static void AppleSignIn()
        {
            IAuthentication authentication = DependencyService.Get<IAuthentication>();
            authentication.AppleSignIn();

        }

        /// <MetaDataID>{6ea4ebf2-c232-465c-92cd-76413220df7c}</MetaDataID>
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);




    }



    /// <MetaDataID>{c08a26ac-3779-43b1-8d5e-5ae5b11361d9}</MetaDataID>
    public interface IAuthentication
    {
        System.Threading.Tasks.Task<string> EmailSignUp(string email, string password);
        System.Threading.Tasks.Task<string> EmailSignIn(string email, string password);
        void SendPasswordResetEmail(string email);
        void FacebookSignIn();
        void GoogleSignIn();
        Task AppleSignIn();
        void SignOut();
    }
}
