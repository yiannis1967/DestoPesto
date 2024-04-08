using System;
using System.Collections.Generic;
using System.Text;

namespace DestoPesto.Models
{
    public class PromoContest
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime StartingDate { get; set; }

        public String InfoUrl { get; set; }

        public String InfoText { get; set; }
    }
}
