using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace DestoPesto.Models
{
    public class User
    {
        public string locale { get; set; }


        /// <summary>
        /// User's first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// User's date of birth 
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }


        public PromoContest PromoContest { get; set; }


        public bool ParticipateToContest { get; set; }

        public int PromoAcceptedPhotos { get; set; }

        public int AcceptedPhotos { get; set; }

        public int RejectedPhotos { get; set; }

        
        
        public int unFixedSubs { get; set; }
        public int fixedSubs{ get; set; }


        public int percentage { get; }
        public int Subs { get; }
        public int average_repair_days { get; }
        public int unfixed_days { get; }



    }
}
