using System;
using System.Collections.Generic;
using System.Text;

namespace DestoPesto.Models
{
    public class PostSubmission
    {

        public string uri { get; set; }=Guid.NewGuid().ToString("N");
        public string dateTime { get; set; }
        public string UserEmail { get; set; }
        public string userId { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string category { get; set; }
        public string comments { get; set; }
        public string photo { get; set; }
        public string Code { get; set; }
        public string DeviceId { get; set; }

        public string DeviceModel { get; set; }
        public string DeviceManufacturer { get; set; }
        public string DeviceName { get; set; }
        public string DevicePlatform { get; set; }
        public string DeviceIdiom { get; set; }
        public int Angle { get;  set; }
    }
}
