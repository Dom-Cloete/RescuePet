using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace domRescuePet.Models
{
    public class HomeViewModel
    {
        public int AdoptedPetsCount { get; set; }
        public List<AdoptionDetail> Adoptions { get; set; }
    }

    public class AdoptionDetail
    {
        public string AdopterName { get; set; }
        public string PetName { get; set; }
        public System.DateTime AdoptionDate { get; set; }
    }
}