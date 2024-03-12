using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#if DeviceDotNet
using Xamarin.Forms;
#endif

namespace Authentication
{

    public delegate void AuthStateChangeHandler(object sender, AuthStateEventArgs authArgs);

    public delegate void IdTokenChangeHandler(object sender, IdTokenEventArgs idTokenArgs);



    
    public interface IAuth
    {
        event AuthStateChangeHandler AuthStateChange;

        event IdTokenChangeHandler IdTokenChange;

        IAuthUser CurrentUser { get; }

        Task<string> GetIdToken();

        bool SignInWith(SignInProvider provider);

        void SignOut();

    }
    /// <MetaDataID>{179d71b7-2a40-4525-bd1d-cef724192522}</MetaDataID>
    public enum SignInProvider
    {
        Google = 0,
        Facebook = 1,
        Twitter = 2,
        Email = 3
    }
    /// <MetaDataID>{e95f3c30-c154-43d9-9694-027c33114745}</MetaDataID>
    public class OOAdvantechAuth
    {
        static IAuth _Auth;
        public static IAuth Auth
        {
            get
            {
                if (_Auth == null)
                {
                    //IDeviceInstantiator deviceInstantiator = DependencyService.Get<IDeviceInstantiator>();
                    //_Auth = deviceInstantiator.GetDeviceSpecific(typeof(IAuth)) as IAuth;
                }
                return _Auth;
            }
        }
    }

    /// <MetaDataID>{ffbd96e8-353f-45b0-ad70-ae7d4cd978dd}</MetaDataID>
    public class AuthStateEventArgs : EventArgs
    {
        public AuthStateEventArgs(IAuth auth)
        {
            Auth = auth;
        }
        IAuthUser CurrentUser { get; }
        public IAuth Auth { get; private set; }
    }
    /// <MetaDataID>{d04e0552-a165-4cf0-bdc7-681246926ba4}</MetaDataID>
    public class IdTokenEventArgs : EventArgs
    {
        public IdTokenEventArgs(IAuth auth)
        {
            Auth = auth;
        }

        public IAuth Auth { get; private set; }
    }

    /// <MetaDataID>{990247cb-4a4a-4b27-9cc5-0abf5117dd2f}</MetaDataID>
    public interface IAuthUser
    {
        string DisplayName { get; }
        bool IsEmailVerified { get; }
        bool IsAnonymous { get; }

        string Email { get; }
        string PhotoUrl { get; }
        IList<IUserInfo> ProviderData { get; }
        string ProviderId { get; }
        IList<string> Providers { get; }
        string PhoneNumber { get; }
        string Uid { get; }

    }

    /// <MetaDataID>{b9b65383-6104-462a-bcf6-dcf56d465c71}</MetaDataID>
    public class AuthUser : IAuthUser
    {


        public bool IsEmailVerified { get; set; }

        public bool IsAnonymous { get; set; }


        public string PhotoUrl { get; set; }


        public IList<IUserInfo> ProviderData { get; set; }


        public string ProviderId { get; set; }

        public IList<string> Providers { get; set; }

        public string PhoneNumber { get; set; }

        public string Uid { get; set; }
        public string DisplayName { get; set; }

        public AuthSubsystem AuthSubsystem { get; set; } = AuthSubsystem.Device;


        /// <summary>
        ///  The "iss" (issuer) claim identifies the principal that issued the
        ///  JWT.  The processing of this claim is generally application specific.
        ///  The "iss" value is a case-sensitive string containing a StringOrURI
        //   value.  Use of this claim is OPTIONAL.
        /// </summary>
        public string Iss { get; set; }
        /// <MetaDataID>{f58fee1b-7b4a-452a-a841-a971a76f54ac}</MetaDataID>
        public string Name { get; set; }

        /// <MetaDataID>{ae90bc8a-8a94-4712-a6de-f86231fa5a91}</MetaDataID>
        public string Picture { get; set; }

        /// <summary>
        ///  The "aud" (audience) claim identifies the recipients that the JWT is
        ///  intended for.  Each principal intended to process the JWT MUST
        ///  identify itself with a value in the audience claim.If the principal
        ///  processing the claim does not identify itself with a value in the
        ///  "aud" claim when this claim is present, then the JWT MUST be
        ///  rejected.In the general case, the "aud" value is an array of case-
        ///  sensitive strings, each containing a StringOrURI value.In the
        ///  special case when the JWT has one audience, the "aud" value MAY be a
        ///  single case-sensitive string containing a StringOrURI value.The
        ///  interpretation of audience values is generally application specific.
        ///  Use of this claim is OPTIONAL.
        /// </summary>
        /// <MetaDataID>{556af901-16a3-4f47-9fa7-1071fb7bdd76}</MetaDataID>
        public string Audience { get; set; }

        /// <summary>
        /// Time when the End-User authentication occurred
        /// </summary>
        /// <MetaDataID>{5a877566-471c-4b73-8afd-5320e5358c98}</MetaDataID>
        public DateTime Auth_Time { get; set; }

        /// <MetaDataID>{4678d405-aa61-47a7-bd0a-efad761c74c8}</MetaDataID>
        public string User_ID { get; set; }

        /// <summary>
        /// The "sub" (subject) claim identifies the principal that is the
        /// subject of the JWT.The claims in a JWT are normally statements
        /// about the subject.The subject value MUST either be scoped to be
        /// locally unique in the context of the issuer or be globally unique.
        /// The processing of this claim is generally application specific.The
        /// "sub" value is a case-sensitive string containing a StringOrURI
        /// value.Use of this claim is OPTIONAL.
        ///  </summary>
        /// <MetaDataID>{ef411975-2d77-4c88-bbf1-22c1a7ab4afc}</MetaDataID>
        public string Subject { get; set; }

        /// <summary>
        /// The "iat" (issued at) claim identifies the time at which the JWT was
        /// issued.This claim can be used to determine the age of the JWT.Its
        /// value MUST be a number containing a NumericDate value.  Use of this
        /// claim is OPTIONAL.
        ///  </summary>
        /// <MetaDataID>{6b72e4ad-a4eb-4b87-8b23-8c096509c7c5}</MetaDataID>
        public DateTime IssuedAt { get; set; }

        /// <summary>
        /// The "exp" (expiration time) claim identifies the expiration time on
        /// or after which the JWT MUST NOT be accepted for processing.The
        /// processing of the "exp" claim requires that the current date/time
        /// MUST be before the expiration date/time listed in the "exp" claim.
        ///  </summary>
        /// <MetaDataID>{bc1dc873-561c-4019-a2f2-03ed9bb54f46}</MetaDataID>
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// User Email
        /// </summary>
        /// <MetaDataID>{b5a0bc29-a93b-4b95-b491-a1659ceb5f29}</MetaDataID>
        public string Email { get; set; }

        /// <MetaDataID>{e88a2323-91b4-4474-88a9-d5f3d7064976}</MetaDataID>
        public bool Email_Verified { get; set; }

        /// <MetaDataID>{4a1060d8-7edb-42bb-8e17-f6f6e2732136}</MetaDataID>
        public string Firebase_Sign_in_Provider { get; set; }


        /// <MetaDataID>{4fa04e99-9c39-41a0-b19c-422bf2518058}</MetaDataID>
        public string AuthToken { get; set; }
        public object Tag { get; set; }
    }

    /// <MetaDataID>{575d4f5e-2c31-4bff-89d7-cdee483ee371}</MetaDataID>
    public enum AuthSubsystem
    {
        Device = 0,
        Web = 1
    }


    /// <MetaDataID>{1fc57859-3b1b-42d6-a720-01ddc158e94e}</MetaDataID>
    public interface IUserInfo
    {
        string DisplayName { get; }
        string Email { get; }
        bool IsEmailVerified { get; }
        string PhoneNumber { get; }
        string PhotoUrl { get; }
        string ProviderId { get; }
        string Uid { get; }
    }

    /// <MetaDataID>{320bbc08-b971-4834-a90a-d3c0e27cd5e0}</MetaDataID>
    public class UserInfo : IUserInfo
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoUrl { get; set; }
        public string ProviderId { get; set; }
        public string Uid { get; set; }
        public bool IsAnonymous { get; set; }

    }

}
