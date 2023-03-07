using System;
using System.Collections.Generic;
using System.Text;

namespace DestoPesto.Models
{
    public class DamageData
    {
        public string id { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string category { get; set; }
        public int numberOfUsers { get; set; }
        public string firstDateReported { get; set; }
        public string photoUrl { get; set; }

        public string fullAddress { get; set; }

        public string CategoryName
        {
            get
            {
                return DestoPesto.Services.JsonHandler.GetCatagory(int.Parse(category));
            }
            set
            {

            }
        }

        public string MarkIconUri
        {
            get
            {
                return DestoPesto.Services.JsonHandler.GetCatagoryMarkIconUri(int.Parse(category));
            }
            set
            {

            }
        }
    }
}
