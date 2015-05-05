﻿using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AspnetIdentitySample.Controllers
{
    public class PetCheckController : Controller
    {
        private MyDbContext db;
        private UserManager<ApplicationUser> manager;
        public PetCheckController()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: PetCheck
        public async Task<ActionResult> Index(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pet pet = await db.Pets.FindAsync(id);
            if (pet == null)
            {
                return HttpNotFound();
            }
            if (pet.User.Id != currentUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            ViewBag.errorList = errors(pet);
            return View(pet);
            //return View();
        }

        public List<String> errors(Pet pet)
        {
            List<String> errorList = new List<string>();

            if (null == pet.RabiesVaccinations || pet.RabiesVaccinations.Count == 0)
            {
                errorList.Add("No rabies Vaccinations found");
            }

            if (null == pet.FAVNBloodTests || pet.FAVNBloodTests.Count == 0)
            {
                errorList.Add("No bloodtests found");
            }
            else
            {
                errorList.Add("Pet is eligible for entry on or after " + pet.FAVNBloodTests.First().DateOfBloodtest.AddMonths(3));
            }

            
            return errorList;
        }
    }
}