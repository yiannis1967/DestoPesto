using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Essentials;

namespace DebugLog
{
    public static class AppEventLog
    {
        static bool Active = false;

        static HttpClient client = new HttpClient();

        static string ServerUrl;

        static string DeviceID;
        public static void Start(string server, string deviceID, XElement settings)
        {
            ServerUrl=server;
            DeviceID=deviceID;
            if (settings.Attribute("Enable")?.Value?.ToLower()=="true")
            {
                if (string.IsNullOrWhiteSpace(settings.Attribute("Platform")?.Value))
                {
                    if (string.IsNullOrWhiteSpace(settings.Attribute("DeviceID")?.Value))
                        Active=true;
                    else
                    {
                        if (settings.Attribute("DeviceID")?.Value==deviceID)
                            Active=true;
                    }

                }
                else if (settings.Attribute("Platform")?.Value==Xamarin.Essentials.DeviceInfo.Platform.ToString())
                {
                    if (string.IsNullOrWhiteSpace(settings.Attribute("DeviceID")?.Value))
                        Active=true;
                    else
                    {
                        if (settings.Attribute("DeviceID")?.Value==deviceID)
                            Active=true;
                    }
                }
            }

#if DEBUG
            Active = true;
#endif
        }


        public static async Task<bool> Log(string message, [CallerLineNumber] int lineNumber = 0,    [CallerMemberName] string caller = null, [CallerFilePath] string callerFile=null)
        {



            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                return false;
            if (!Active)
                return false;


            message= string.Format($"Code line : {lineNumber}  caller  : {caller} file path : {callerFile} {Environment.NewLine}{message}");


            Uri uri = new Uri(ServerUrl+ "api/DeviceLog?deviceID="+DeviceID);
            
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(  message), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync(uri, content);

            // this result string should be something like: "{"token":"rgh2ghgdsfds"}"

            var result = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;


        }

    }
}
