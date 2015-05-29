using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AspnetIdentitySample.Controllers
{
    public class RabiesVaccinationsController : Controller
    {
        private MyDbContext db = new MyDbContext();
        private UserManager<ApplicationUser> manager;

        public RabiesVaccinationsController()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: RabiesVaccination
        //public async Task<ActionResult> Index()
        //{
        //    var rabiesVaccinations = db.RabiesVaccinations.Include(r => r.Pet);
        //    return View(await rabiesVaccinations.ToListAsync());
        //}

        // GET: RabiesVaccination/1
        public async Task<ActionResult> Index(int? id)
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var rabiesVaccinations = db.RabiesVaccinations.Include(r => r.Pet);
            SelectList petList = new SelectList(db.Pets.Where(r => r.User.Id == currentUser.Id), "Id", "Name", id);
            ViewBag.PetID = petList;
            ViewBag.PetCount = petList.ToList().Count;

            // Only show vaccinations from the current user
            if (!User.IsInRole("Admin"))
            {
                rabiesVaccinations = rabiesVaccinations.Where(r => r.Pet.User.Id == currentUser.Id);
            }            

            if (id != null)
            {
                rabiesVaccinations = rabiesVaccinations.Where(r => r.Pet.Id == id);
            }

            rabiesVaccinations = rabiesVaccinations.OrderBy(r => r.DateOfValidityFrom);

            return View(await rabiesVaccinations.ToListAsync());

            //return View(db.Pets.ToList().Where(pet => pet.User.Id == currentUser.Id));
            //Need to tighten up rabiesVaccinations for Users and pets so that only rabies vaccinations for a user and pet are shown
        }

        // GET: RabiesVaccination/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RabiesVaccination rabiesVaccination = await db.RabiesVaccinations.FindAsync(id);
            if (rabiesVaccination == null)
            {
                return HttpNotFound();
            }
            if (rabiesVaccination.Pet.User.Id != currentUser.Id && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(rabiesVaccination);
        }

        // GET: RabiesVaccination/Create
        public async Task<ActionResult> Create(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            ViewBag.PetID = new SelectList(db.Pets.Where(r => r.User.Id == currentUser.Id), "Id", "Name", id);
            ViewBag.id = id;
            return View();
        }

        // POST: RabiesVaccination/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RabiesVaccinationID,Manufacturer,BatchNo,DateOfRabiesVaccination,DateOfValidityFrom,DateOfValidityTo,PetID")] RabiesVaccination rabiesVaccination)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (rabiesVaccination.PetID == 0)
            {
                ModelState.AddModelError("PetID", "You must select a pet.");
            }
            rabiesVaccinationValidation(rabiesVaccination);
            if (ModelState.IsValid)
            {
                db.RabiesVaccinations.Add(rabiesVaccination);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PetID = new SelectList(db.Pets.Where(r => r.User.Id == currentUser.Id), "Id", "Name", rabiesVaccination.PetID);
            return View(rabiesVaccination);
        }

        // GET: RabiesVaccination/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RabiesVaccination rabiesVaccination = await db.RabiesVaccinations.FindAsync(id);
            if (rabiesVaccination == null)
            {
                return HttpNotFound();
            }
            if (rabiesVaccination.Pet.User.Id != currentUser.Id && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            ViewBag.PetID = new SelectList(db.Pets, "Id", "Name", rabiesVaccination.PetID);
            return View(rabiesVaccination);
        }

        // POST: RabiesVaccination/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RabiesVaccinationID,Manufacturer,BatchNo,DateOfRabiesVaccination,DateOfValidityFrom,DateOfValidityTo,PetID")] RabiesVaccination rabiesVaccination)
        {
            rabiesVaccinationValidation(rabiesVaccination);
            if (ModelState.IsValid)
            {
                db.Entry(rabiesVaccination).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PetID = new SelectList(db.Pets, "Id", "Name", rabiesVaccination.PetID);
            return View(rabiesVaccination);
        }

        // GET: RabiesVaccination/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RabiesVaccination rabiesVaccination = await db.RabiesVaccinations.FindAsync(id);
            if (rabiesVaccination == null)
            {
                return HttpNotFound();
            }
            if (rabiesVaccination.Pet.User.Id != currentUser.Id && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(rabiesVaccination);
        }

        // POST: RabiesVaccination/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RabiesVaccination rabiesVaccination = await db.RabiesVaccinations.FindAsync(id);
            db.RabiesVaccinations.Remove(rabiesVaccination);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void rabiesVaccinationValidation(RabiesVaccination rVax)
        {
            // Get Pet Details
            Pet pet = db.Pets.Find(rVax.PetID);
            if (pet != null)
            {
                // Date Valid From must be after Date Given
                if (rVax.DateOfValidityFrom.CompareTo(rVax.DateOfRabiesVaccination) < 0)
                {
                    ModelState.AddModelError("", "Date Valid From must be on or after Date Given.");
                } else if (rVax.DateOfValidityFrom.CompareTo(rVax.DateOfRabiesVaccination.AddMonths(1)) > 0)
                {
                    // Date Valid From must be no more than a month after Date Given
                    ModelState.AddModelError("", "Date Valid From must be within a month of Date Given.");
                }
                // Date Valid To must be after Date Given
                if (rVax.DateOfValidityTo.CompareTo(rVax.DateOfRabiesVaccination) < 0)
                {
                    ModelState.AddModelError("", "Date Valid To must be after Date Given.");
                }
                // Pet must be at least 12 weeks old when vaccinated
                DateTime dobPlus12 = pet.DateOfBirth.AddDays(84);
                if (rVax.DateOfRabiesVaccination.CompareTo(dobPlus12) < 0)
                {
                    string dob = pet.DateOfBirth.ToString("dd-MMM-yyyy");
                    ModelState.AddModelError("", "Pet must be at least 12 weeks old when vaccinated. " + pet.Name + " was born on " + dob + ".");
                }
                // Validity period must be at least one year
                if (rVax.DateOfValidityTo.CompareTo(rVax.DateOfValidityFrom.AddYears(1)) < 0)
                {
                    ModelState.AddModelError("", "Validity period must be at least one year.");
                }

                if (rVax.DateOfValidityTo.CompareTo(rVax.DateOfValidityFrom.AddYears(3)) > 0)
                {
                    ModelState.AddModelError("", "Validity period cannot be greater than three years.");
                }
            }           
        }
    }
}
