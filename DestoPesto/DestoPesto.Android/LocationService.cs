using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DestoPesto.Droid
{

    /// <MetaDataID>{7826d101-0eb3-406a-99ce-9ac4e7aea326}</MetaDataID>
    [Service(Name = "com.companyname.DestoPesto.LocationService", Exported = true)]
    public class LocationService: ForegroundService
    {
    }
}