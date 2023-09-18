using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DestoPesto.Models
{
    public class Catagories
    {
       public int id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string description_en { get; set; }
        public string comments { get; set; }
        public string descriptionCognity { get; set; }
        public string color { get; set; }
        public string textColor { get; set; }
        public string VectorImageUrl { get; set; }
        public string IconUrl { get; set; }
        public string markIconUrl { get; set; }
         
    }
}