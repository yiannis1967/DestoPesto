using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCMTest
{
    public class CloudNotificationManager
    {

        static FirebaseApp FirebaseApp;
        static CloudNotificationManager()
        {
            Credential credential = new Credential()
            {
                type = "service_account",
                project_id = "toposmougr",
                private_key_id = "9d48270495575d64783bd3e245aa814065b161b4",
                private_key = "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDMeuAzajl89l7+\nyM6Im31GAOXKDI8QJYgVKgQ5GEfqZY2jCP/EKQ5e4TbA/nXSlntUdPj4+SbPJhJc\nFLkRg79pPp3xaVBsMmGTCg4EkDVVwzOCzSOrxlepRh13uTgqJwWDIWn1DyPQt+tg\ncaHwkjj/WJH5HPebxHrHr61pcurM7gnWufV1ImuQhfWPFeA2k1ojR1VBOC8kBCQ5\nbU7y+vW6jD+P/HYGVFi/yiXogayxjLZSGHrDowOCt46H7S34fMIY/OEpES3GOIGs\nZmKByXcfr0NwDEFi2esB02eIecMMbJZ9RF7lwp1MP0rmnTS7v+81137A7deSmtpB\n63tsXmfTAgMBAAECggEAIfQhYx2MuuL6IS/c7l12BCLFWrM9Kj7hMDUliwmCH4+K\nC6bEjvZg9tZKI5UyoVZFgl+uBpbwn83AQgla4Gf9EmUduubJk5MbarFtDyBBtaJP\n3QoNQJ+9vKjyNF14JbUMZZrYdPh08lp9O4AtlhHhubIjrbbmqAyMdWNcKzoJd8pJ\nl/TUAvdSOl/1QNJzC5Hd14hdTMR/9zl81YhJO3F9ngJHp77mTHyreBdUwlMqg16C\nyKrPzwjO2PIPs8biJt8M6oI+aOgOO/7Q3o2A3ktevsSXS91ZZei72O1K1moqfyzQ\n2DaWaR1HIob1SIjb8KLYwEy41nqnzyfrLCXZ06sVOQKBgQDxAVXJWSIXYnu8lc+q\nzFHu162chWvIK8vr5vNmi+cA30iuA2QbAL0ikE4rNkROapDw9XTXAFK+cstoFTvH\nE/LNR8JRJb4CDAGXnBWsEHsfhs5Klzb/cY//MCbG6ViEqLYfSj2HHeQ1vR1H8rW4\nIs3vGRI+flYPfopCSjq9ctn6lwKBgQDZM8bdmQSC+BgcVYUxwK2lIeELKOzDKQWi\nJmWEikzqVlKDsVfDsjiXAHC08FyKt8hBvycxRAA4GSVqsi3nAx+nmuI7AA9KtzPD\n5gZ/wtP5r0YgFTBwj877EB7R+4yWZNusQQF29CAcLQq6FA8HIO3T2AtxyjUE/MY1\nrmFNB6JQJQKBgB5GpalCukanx5WcWFdDjbNsgtHb7PtbjSWYgNFMF/wDENVTdbry\nr8/swvUovxH+zzCGFWSBFOP59dWgfT74IZNqRV3+WM1XBsguAob4Fw+R1s6GCpGy\niX1sZiKs48LekuqBYBNeTPH3TPth4TQ9oxM7WhBhvJv7sJC9VH3CNOIHAoGBAJN2\nMZeVIZkBcu0ZqroihEHOhIkdGdFN8oMp6lZXxux5+r9qEZnNT9pE9EADx/Bt4cmx\nS5yI/FUZzWto2P00A7O41csUuU0SoFRpwRPQZDXqJ4P0ntRs7itJwILzc3lPtEx4\nPdRS3dIDSnsWzzUZyB3BWdPBYsmmC7O6VlkDy9D5AoGBAM/xPJksfGTi7z+mMfro\ncGPTiIrzHNXGT5VHN47Rq88ycutOmHcl8qAbwpbPGUO9N90O4Hbe/HbqlmA5zHqH\nQjV4UCjsvpRJ2tQ+fTMh2qPdtMTWbT0U445XiXoI0ZLZA+sSRXCi2wWOPZ7BpLAP\n+fUS4B5aw5LkUsE2iSPRUxvG\n-----END PRIVATE KEY-----\n",
                client_email = "firebase-adminsdk-s8pj1@toposmougr.iam.gserviceaccount.com",
                client_id = "103849283409805280635",
                auth_uri = "https://accounts.google.com/o/oauth2/auth",
                token_uri = "https://oauth2.googleapis.com/token",
                auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs",
                client_x509_cert_url = "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-s8pj1%40toposmougr.iam.gserviceaccount.com"
            };



            FirebaseApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromJson(Newtonsoft.Json.JsonConvert.SerializeObject(credential))
            });


        }


        public static async void SendMessage(string deviceFirebaseToken, string messageID, string body, string title, string imageUrl)
        {
            //Documantation
            //https://firebase.google.com/docs/cloud-messaging/concept-options#setting-the-priority-of-a-message
            var fireBaseMessage = new Message()
            {
                Data = new Dictionary<string, string>()
                {

                    { "MessageID", messageID }
                    ,{"ImageUrl", imageUrl}
                },
                Notification = new Notification() { Title = title , Body = body},
                Token = deviceFirebaseToken,
                Android = new AndroidConfig() {
                    Priority = Priority.High,
                    Notification=new AndroidNotification { ImageUrl=imageUrl }
                },
                Apns=new ApnsConfig() {


                    FcmOptions=new ApnsFcmOptions() { ImageUrl=imageUrl },
                    Headers=new Dictionary<string, string>() { { "image", imageUrl } }
                }
                
            };
            //B_5D44abEZbnfQXU8MoOo:APA91bEtP_iQlkOaW0lajFfF2hTNStvyxqrM6MC4q5vYfAAxxoHctVhZB95TeTPcETe8WZpud4vdOgzrA_54XJMLHgHBpxykDq91MyJNTp-cAb05vkMh1-uAJreSXd-h2yT3z6CpNLF9
            //if (message.Notification != null)
            //{
            //    AndroidConfig androidConfig = new AndroidConfig() { Notification = new AndroidNotification { Title = message.Notification.Title, Body = message.Notification.Body,ClickAction= ".MainActivity" } };
            //    fireBaseMessage.Android= androidConfig;//.Notification = new Notification() { Body = message.Notification.Body, Title = message.Notification.Title };
            //}

            // Send a message to the device corresponding to the provided
            // registration token.
            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(fireBaseMessage);
            }
            catch (Exception error)
            {


            }


        }


    }

    /// <MetaDataID>{63f01ed1-bf17-46d4-b609-77aa77b80063}</MetaDataID>
    public class Credential
    {
        /// <MetaDataID>{6eae306e-ef3f-4920-9653-712131468bdd}</MetaDataID>
        public string type { get; set; }
        /// <MetaDataID>{39929589-4f73-4159-81d1-bacadf769070}</MetaDataID>
        public string project_id { get; set; }
        /// <MetaDataID>{621c5962-f0bc-4e28-9d4f-96824848b3d3}</MetaDataID>
        public string private_key_id { get; set; }
        /// <MetaDataID>{35bfc236-dce6-487e-adf0-6989be80fae9}</MetaDataID>
        public string private_key { get; set; }
        /// <MetaDataID>{2925cc24-c3a3-416f-ae34-b6a20554df94}</MetaDataID>
        public string client_email { get; set; }
        /// <MetaDataID>{19de72f2-b0ec-439c-bbdd-8d47e3a71546}</MetaDataID>
        public string client_id { get; set; }
        /// <MetaDataID>{75c19603-6e9d-4fea-9abe-9b32924ac2a1}</MetaDataID>
        public string auth_uri { get; set; }
        /// <MetaDataID>{465c8d11-3873-4acc-b079-9646b6b8e5f7}</MetaDataID>
        public string token_uri { get; set; }
        /// <MetaDataID>{70f62c71-6995-40c9-8ce3-68087782742a}</MetaDataID>
        public string auth_provider_x509_cert_url { get; set; }
        /// <MetaDataID>{005a2bed-71ad-461f-b71f-ff837a230ae6}</MetaDataID>
        public string client_x509_cert_url { get; set; }
    }
}
