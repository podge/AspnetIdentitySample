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
            ViewBag.PetID = new SelectList(db.Pets, "Id", "Name", id); //.Where(r => r.Pet.User.Id == currentUser.Id);
            // Only show vaccinations from the current user
            rabiesVaccinations = rabiesVaccinations.Where(r => r.Pet.User.Id == currentUser.Id);

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
        public ActionResult Create(int? id)
        {
            //ViewBag.PetID = new SelectList(db.Pets.Where(r => r.Id == id), "Id", "Name", id);
            ViewBag.PetID = new SelectList(db.Pets, "Id", "Name", id);
            ViewBag.id = id;         // Defines ViewBag
            return View();
        }

        // POST: RabiesVaccination/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RabiesVaccinationID,DateOfRabiesVaccination,DateOfValidityFrom,DateOfValidityTo,PetID")] RabiesVaccination rabiesVaccination)
        {
            if (ModelState.IsValid)
            {
                db.RabiesVaccinations.Add(rabiesVaccination);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PetID = new SelectList(db.Pets, "Id", "Name", rabiesVaccination.PetID);
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
        public async Task<ActionResult> Edit([Bind(Include = "RabiesVaccinationID,DateOfRabiesVaccination,DateOfValidityFrom,DateOfValidityTo,PetID")] RabiesVaccination rabiesVaccination)
        {
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
    }
}
