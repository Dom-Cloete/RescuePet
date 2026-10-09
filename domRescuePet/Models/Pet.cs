using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace domRescuePet.Models
{
    public class Pet
    {
        public int PetId { get; set; }
        public string PetName { get; set; }
        public int TypeId { get; set; }
        public int BreedId { get; set; }
        public int LocationId { get; set; }
        public int Age { get; set; }
        public decimal Weight { get; set; }
        public string Gender { get; set; }
        public string PetStory_Short { get; set; }
        public string PetStory_Long { get; set; }
        public string Status { get; set; }
        public int PosterUserId { get; set; }
        public string Image { get; set; }
        public string PosterName { get; set; }
        public string TypeName { get; set; }
        public string BreedName { get; set; }
        public string LocationName { get; set; }
        public string AdopterName { get; set; }
        public DateTime? AdoptionDate { get; set; }

        public string GetImage
        {
            get
            {
                return !string.IsNullOrEmpty(Image) ? Image : null;
            }
        }
    }
}