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

namespace AspnetIdentitySample.Controllers
{
    public class RabiesVaccinationsController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: RabiesVaccination
        public async Task<ActionResult> Index()
        {
            var rabiesVaccinations = db.RabiesVaccinations.Include(r => r.Pet);
            return View(await rabiesVaccinations.ToListAsync());
        }

        // GET: RabiesVaccination/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RabiesVaccination rabiesVaccination = await db.RabiesVaccinations.FindAsync(id);
            if (rabiesVaccination == null)
            {
                return HttpNotFound();
            }
            return View(rabiesVaccination);
        }

        // GET: RabiesVaccination/Create
        public ActionResult Create()
        {
            ViewBag.PetID = new SelectList(db.Pets, "Id", "Name");
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RabiesVaccination rabiesVaccination = await db.RabiesVaccinations.FindAsync(id);
            if (rabiesVaccination == null)
            {
                return HttpNotFound();
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RabiesVaccination rabiesVaccination = await db.RabiesVaccinations.FindAsync(id);
            if (rabiesVaccination == null)
            {
                return HttpNotFound();
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
