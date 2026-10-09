using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace domRescuePet.Models
{
    public class Donation
    {
        public int DonationId { get; set; }
        public int DonorUserId { get; set; }
        public int DonorPhoneId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DonationDate { get; set; }
    }
}