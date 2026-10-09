using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace domRescuePet.Models
{
    public class Adoption
    {
        public int AdoptionId { get; set; }
        public int PetId { get; set; }
        public int AdopterUserId { get; set; }
        public int AdopterPhoneId { get; set; }
        public DateTime AdoptionDate { get; set; }
    }
}