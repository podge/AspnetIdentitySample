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
    public class BloodtestsController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Bloodtests
        public async Task<ActionResult> Index()
        {
            var bloodtests = db.Bloodtests.Include(b => b.Pet);
            return View(await bloodtests.ToListAsync());
        }

        // GET: Bloodtests/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bloodtest bloodtest = await db.Bloodtests.FindAsync(id);
            if (bloodtest == null)
            {
                return HttpNotFound();
            }
            return View(bloodtest);
        }

        // GET: Bloodtests/Create
        public ActionResult Create()
        {
            ViewBag.PetID = new SelectList(db.Pets, "Id", "Name");
            return View();
        }

        // POST: Bloodtests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,DateOfBloodtest,Result,PetID")] Bloodtest bloodtest)
        {
            if (ModelState.IsValid)
            {
                db.Bloodtests.Add(bloodtest);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PetID = new SelectList(db.Pets, "Id", "Name", bloodtest.PetID);
            return View(bloodtest);
        }

        // GET: Bloodtests/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bloodtest bloodtest = await db.Bloodtests.FindAsync(id);
            if (bloodtest == null)
            {
                return HttpNotFound();
            }
            ViewBag.PetID = new SelectList(db.Pets, "Id", "Name", bloodtest.PetID);
            return View(bloodtest);
        }

        // POST: Bloodtests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DateOfBloodtest,Result,PetID")] Bloodtest bloodtest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bloodtest).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PetID = new SelectList(db.Pets, "Id", "Name", bloodtest.PetID);
            return View(bloodtest);
        }

        // GET: Bloodtests/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bloodtest bloodtest = await db.Bloodtests.FindAsync(id);
            if (bloodtest == null)
            {
                return HttpNotFound();
            }
            return View(bloodtest);
        }

        // POST: Bloodtests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Bloodtest bloodtest = await db.Bloodtests.FindAsync(id);
            db.Bloodtests.Remove(bloodtest);
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
