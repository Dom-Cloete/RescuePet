using domRescuePet.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace domRescuePet.Controllers
{
    public class HomeController : Controller
    {
        private DefaultDataService dataService = new DefaultDataService();

        // home page = call methods to get total pets adopted and adoptions summary
        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                AdoptedPetsCount = dataService.GetAdoptedPetsCount(),
                Adoptions = dataService.GetAdoptions()
            };
            return View(model);
        }

        // pets page = call methods to get pet types + breeds + locations
        public ActionResult Pets(int typeId = 0, int breedId = 0, int locationId = 0)
        {
            var types = dataService.GetPetTypes();
            var breeds = dataService.GetBreeds(typeId);
            var locations = dataService.GetLocations();

            List<Pet> pets;

            // if any of filters activated
            if (typeId != 0 || breedId != 0 || locationId != 0)
                pets = dataService.GetPets(typeId, breedId, locationId);
            else
                pets = dataService.GetPets();

            ViewBag.Types = dataService.GetPetTypes();
            ViewBag.Breeds = dataService.GetBreeds(typeId);
            ViewBag.Locations = dataService.GetLocations();

            // declare selected items as vars
            ViewBag.SelectedType = typeId;
            ViewBag.SelectedBreed = breedId;
            ViewBag.SelectedLocation = locationId;

            return View(pets);
        }

        // JSON method = show only breeds per type of pet selected
        public JsonResult GetBreedsByType(int typeId) => 
            Json(dataService.GetBreeds(typeId), JsonRequestBehavior.AllowGet);

        // adopt page = pass id from pet selected on pets page
        public ActionResult Adopt(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Pets");

            var pet = dataService.GetPets().FirstOrDefault(p => p.PetId == id.Value);
            if (pet == null) return HttpNotFound();

            // call users method
            ViewBag.Users = new SelectList(
                dataService.GetUsers().Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName }),
                "UserId", "FullName"
            );

            return View(pet);
        }

        // adopt page = save pet adoption
        [HttpPost]
        public ActionResult Adopt(int petId, int adopterUserId, int adopterPhoneId)
        {
            if (dataService.SaveAdoption(petId, adopterUserId, adopterPhoneId))
                return RedirectToAction("Pets");

            TempData["Error"] = "Could not save adoption. Try again.";
            return RedirectToAction("Adopt", new { id = petId });
        }

        // JSON method = show only phone number of selected user
        public JsonResult GetPhonesByUser(int userId) =>
            Json(dataService.GetPhones(userId), JsonRequestBehavior.AllowGet);

        // post page = call methods to get users + pet types + locations
        public ActionResult Post()
        {
            var users = dataService.GetUsers();
            var types = dataService.GetPetTypes();
            var locations = dataService.GetLocations();

            ViewBag.Users = new SelectList(users.Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName }), "UserId", "FullName");
            ViewBag.Types = new SelectList(types, "TypeId", "TypeName");
            ViewBag.Breeds = new SelectList(Enumerable.Empty<SelectListItem>(), "BreedId", "BreedName");
            ViewBag.Locations = new SelectList(locations, "LocationId", "LocationName");
            ViewBag.Genders = new SelectList(new[] { "Male", "Female", "Unknown" });

            // JSON methods = populate dropdowns based on selected pet type/user
            ViewBag.BreedsJson = Newtonsoft.Json.JsonConvert.SerializeObject(dataService.GetBreeds());
            ViewBag.PhonesJson = Newtonsoft.Json.JsonConvert.SerializeObject(dataService.GetPhones(0));


            return View(new Pet());
        }

        // post page = save pet posting
        [HttpPost]
        public ActionResult Post(Pet pet, HttpPostedFileBase uploadImage, int posterPhoneId)
        {
            if (ModelState.IsValid)
            {
                // convert image to base64
                if (uploadImage != null && uploadImage.ContentLength > 0)
                {
                    using (var reader = new System.IO.BinaryReader(uploadImage.InputStream))
                    {
                        var bytes = reader.ReadBytes(uploadImage.ContentLength);
                        pet.Image = Convert.ToBase64String(bytes);
                    }
                }

                pet.Status = "Available";

                bool success = dataService.SavePet(pet, posterPhoneId);

                if (success)
                    return RedirectToAction("Pets");
            }

            // populate dropdowns
            ViewBag.Users = new SelectList(dataService.GetUsers().Select(u => new { u.UserId, FullName = u.FirstName + " " + u.LastName }), "UserId", "FullName");
            ViewBag.Types = new SelectList(dataService.GetPetTypes(), "TypeId", "TypeName");
            ViewBag.Breeds = new SelectList(dataService.GetBreeds(), "BreedId", "BreedName");
            ViewBag.Locations = new SelectList(dataService.GetLocations(), "LocationId", "LocationName");
            ViewBag.Genders = new SelectList(new[] { "Male", "Female", "Unknown" });


            return View(pet);
        }

        // donate page = call methods to get users + total donations
        public ActionResult Donate()
        {
            var users = dataService.GetUsers();
            var model = new DonateViewModel
            {
                Users = users.Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.FirstName + " " + u.LastName
                }).ToList(),
                Phones = new List<SelectListItem>(),
                TotalRaised = dataService.GetTotalDonations()
            };
            return View(model);
        }

        // donation page = save money donated
        [HttpPost]
        public ActionResult Donate(DonateViewModel model)
        {
            if (model.Amount > 0)
                dataService.SaveDonation(model.SelectedUserId, model.SelectedPhoneId, model.Amount);

            model.Users = dataService.GetUsers().Select(u => new SelectListItem { Value = u.UserId.ToString(), Text = u.FirstName + " " + u.LastName }).ToList();
            model.Phones = dataService.GetPhones(model.SelectedUserId).Select(p => new SelectListItem { Value = p.PhoneId.ToString(), Text = p.PhoneNumber }).ToList();
            model.TotalRaised = dataService.GetTotalDonations();

            return View(model);
        }
    }
}