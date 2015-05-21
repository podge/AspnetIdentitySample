using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspnetIdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace AspnetIdentitySample.Controllers
{
    public class CertificatesController : Controller
    {
        private MyDbContext db;
        private UserManager<ApplicationUser> manager;

        public CertificatesController()
        {
            db = new MyDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: Certificates
        public ActionResult Index()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());

            // Get pets, consignors and consignees for current user
            // Warn if user needs to create any of these objects

            ViewBag.pets = db.Pets.Count(s => s.User.Id == currentUser.Id);
            ViewBag.consignors = db.Consignors.Count(s => s.User.Id == currentUser.Id);
            ViewBag.consignees = db.Consignees.Count(s => s.User.Id == currentUser.Id);
            
            return View(db.Certificate.ToList().Where(s => s.User.Id == currentUser.Id));
        }

        // GET: Certificates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Certificate certificate = db.Certificate.Find(id);
            if (certificate == null)
            {
                return HttpNotFound();
            }
            return View(certificate);
        }

        // GET: Certificates/Create
        public ActionResult Create()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            ViewBag.ConsignorId = new SelectList(db.Consignors.Where(s => s.User.Id == currentUser.Id), "ConsignorId", "DropdownName");
            ViewBag.ConsigneeId = new SelectList(db.Consignees.Where(s => s.User.Id == currentUser.Id), "ConsigneeId", "DropdownName");
            var pets = from s in db.Pets
                       where (s.User.Id == currentUser.Id)
                       select s;

            Certificate cert = new Certificate();
            cert.Pets = pets.ToList();

            return View(cert);
        }

        // POST: Certificates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CertificateId,ConsignorId,ConsigneeId,CountryOfOrigin,ISOCode,CommodityDescription")] Certificate certificate)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            certificate.User = currentUser;
            
            certificate.Consignor = db.Consignors.Find(certificate.ConsignorId);
            certificate.Consignee = db.Consignees.Find(certificate.ConsigneeId);
            if (ModelState.IsValid)
            {
                db.Certificate.Add(certificate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(certificate);
        }

        // GET: Certificates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Certificate certificate = db.Certificate.Find(id);
            if (certificate == null)
            {
                return HttpNotFound();
            }
            return View(certificate);
        }

        // POST: Certificates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CertificateId,CountryOfOrigin,ISOCode,CommodityDescription")] Certificate certificate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(certificate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(certificate);
        }

        // GET: Certificates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Certificate certificate = db.Certificate.Find(id);
            if (certificate == null)
            {
                return HttpNotFound();
            }
            return View(certificate);
        }

        // POST: Certificates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Certificate certificate = db.Certificate.Find(id);
            db.Certificate.Remove(certificate);
            db.SaveChanges();
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
