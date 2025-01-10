using System;
using System.Collections.Generic;
using System.Text;

namespace DestoPesto.Services
{
    /// <MetaDataID>{2198b56c-d22f-40d5-b3fe-c33f97b33d2b}</MetaDataID>
    public class MunicipalityStats
    {
        public int Subs { get; set; }
        public int _fixed { get; set; }
        public double perc { get; set; }
        public int average_repair_days { get; set; }
        public DateTime unfixed_since { get; set; }
        public int Unfixed_days { get; set; }
        public string email { get; set; }
        public DateTime date { get; set; }
        public int ranking { get; set; }
        public int validMunicipalities { get;set; }
      
    }

}