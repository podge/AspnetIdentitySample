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
    public class PetsController : Controller
    {
        private MyDbContext db;
        private UserManager<ApplicationUser> manager;
        public PetsController()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        
        // GET: /ToDo/
        // GET ToDo for the logged in user
        public ActionResult Index()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            return View(db.Pets.ToList().Where(todo => todo.User.Id == currentUser.Id));
        }

        // GET: /ToDo/All
        [Authorize(Roles="Admin")]
        public async Task<ActionResult> All()
        {
            return View(await db.Pets.ToListAsync());
        }

        // GET: /ToDo/Details/5
        public async Task<ActionResult> Details(int? id)
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
            return View(pet);
        }

        // GET: /ToDo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ToDo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include="Id,Description,IsDone")] Pet pet)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (ModelState.IsValid)
            {
                pet.User = currentUser;
                db.Pets.Add(pet);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(pet);
        }

        // GET: /ToDo/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
            return View(pet);
        }

        // POST: /ToDo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include="Id,Description,IsDone")] Pet pet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pet).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(pet);
        }

        // GET: /ToDo/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
            return View(pet);
        }

        // POST: /ToDo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Pet pet = await db.Pets.FindAsync(id);
            db.Pets.Remove(pet);
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
