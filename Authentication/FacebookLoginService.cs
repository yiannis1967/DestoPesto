using System;
//using Foundation;

namespace Authentication.Facebook.Services
{
    /// <MetaDataID>{61a79494-a595-4760-ab38-ad471927f938}</MetaDataID>
    public abstract class FacebookLoginService
    {
        public abstract string AccessToken { get; }
        public abstract Action<string, string> AccessTokenChanged { get; set; }
        public abstract void SignOut();

        public static FacebookLoginService CurrentFacebookLoginService { get; set; }

        public abstract void SignIn();
    }
}
