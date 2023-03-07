using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication
{
   



    ///// <MetaDataID>{0d247309-92fb-4d62-953b-15ddfd6b7d78}</MetaDataID>
    //public class AuthUserData
    //{
    //    public string PhoneNumber { get; set; }
    //    public string PhotoURL;
    //    public string DisplayName;
    //    public string Email;
    //    public bool EmailVerified;
    //    public string Uid;
    //    public string ProviderId;
    //}

    /// <MetaDataID>{a3c80482-edbe-4a1e-a514-3210ec880729}</MetaDataID>
    public class FirebaseAuthentication
    {

        static string _FirebaseProjectId;
        static internal string FirebaseProjectId
        {
            get
            {
                return _FirebaseProjectId;
            }
        }
        public static void InitializeFirebase(string FirebaseProjectId)
        {
            _FirebaseProjectId = FirebaseProjectId;
        }
    }

}
