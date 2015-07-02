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
    [Authorize]
    public class BloodtestsController : Controller
    {
        private MyDbContext db;
        private UserManager<ApplicationUser> manager;

        public BloodtestsController()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: Bloodtests
        public async Task<ActionResult> Index()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var bloodtests = db.Bloodtests.Include(b => b.Pet);
            if (!User.IsInRole("Admin"))
            {
                bloodtests = bloodtests.Where(b => b.Pet.User.Id == currentUser.Id);
            }
            
            return View(await bloodtests.ToListAsync());
        }

        // GET: Bloodtests/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bloodtest bloodtest = await db.Bloodtests.FindAsync(id);
            if (bloodtest == null)
            {
                return HttpNotFound();
            }
            if (bloodtest.Pet.User.Id != currentUser.Id && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
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
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bloodtest bloodtest = await db.Bloodtests.FindAsync(id);
            if (bloodtest == null)
            {
                return HttpNotFound();
            }
            if (bloodtest.Pet.User.Id != currentUser.Id && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
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
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bloodtest bloodtest = await db.Bloodtests.FindAsync(id);
            if (bloodtest == null)
            {
                return HttpNotFound();
            }
            if (bloodtest.Pet.User.Id != currentUser.Id && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
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
