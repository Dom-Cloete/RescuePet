using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace domRescuePet.Models
{
    public class DonateViewModel
    {
        public int SelectedUserId { get; set; }
        public int SelectedPhoneId { get; set; }
        public decimal Amount { get; set; }
        public List<SelectListItem> Users { get; set; }
        public List<SelectListItem> Phones { get; set; }
        public decimal TotalRaised { get; set; }
        public decimal Goal { get; set; } = 75000m;
    }
}