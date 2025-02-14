using System;
using System.Collections.Generic;
using System.Text;

namespace DestoPesto.Models
{
    public class XAppConstants
    {
		public static string AppName = "toposmou";

		// OAuth
		// For Google login, configure at https://console.developers.google.com/
		
		public static string AndroidClientId = "959003601559-itvbbidvfe7bqqi75slglk6nrjktdb04.apps.googleusercontent.com";

		// These values do not need changing
		public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
		public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
		public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
		public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

		// Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
		
		public static string AndroidRedirectUrl = "com.googleusercontent.apps.959003601559-itvbbidvfe7bqqi75slglk6nrjktdb04:/oauth2redirect";


		// Facebook OAuth
		// For Facebook login, configure at https://developers.facebook.com
		public static string FacebookiOSClientId = "<insert IOS client ID here>";
		public static string FacebookAndroidClientId = "479499616808537";

		// These values do not need changing
		public static string FacebookScope = "email";
		public static string FacebookAuthorizeUrl = "https://www.facebook.com/dialog/oauth/";
		public static string FacebookAccessTokenUrl = "https://www.facebook.com/connect/login_success.html";
		public static string FacebookUserInfoUrl = "https://graph.facebook.com/me?fields=email&access_token={accessToken}";

		// Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
		public static string FacebookiOSRedirectUrl = "<insert IOS redirect URL here>:/oauth2redirect";
		public static string FacebookAndroidRedirectUrl = "https://www.facebook.com/connect/login_success.html";

		




	}
}
