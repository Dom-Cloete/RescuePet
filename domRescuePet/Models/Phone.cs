using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace domRescuePet.Models
{
    public class Phone
    {
        public int PhoneId { get; set; }
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
    }
}