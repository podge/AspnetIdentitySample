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
using AspnetIdentitySample.Controllers;

namespace AspnetIdentitySample.Views.CertGenerator
{
    [Authorize]
    public class ConsigneesController : BaseConController
    {
        public ConsigneesController()
        {

        }

        // GET: Consignees
        public ActionResult Index()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            return View(db.Consignees.ToList().Where(s => s.User.Id == currentUser.Id));
        }

        // GET: Consignees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consignee consignee = db.Consignees.Find(id);
            if (consignee == null)
            {
                return HttpNotFound();
            }
            return View(consignee);
        }

        // GET: Consignees/Create
        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(getEUCountries(), "CountryId", "CountryName");
            return View();
        }

        // POST: Consignees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ConsigneeId,ConsigneeName,Address1,Address2,Address3,Address4,Postcode,CountryId,Telephone")] Consignee consignee)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            consignee.User = currentUser;
            if (ModelState.IsValid)
            {
                db.Consignees.Add(consignee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(getEUCountries(), "CountryId", "CountryName");
            return View(consignee);
        }

        // GET: Consignees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consignee consignee = db.Consignees.Find(id);
            if (consignee == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryId = new SelectList(getEUCountries(), "CountryId", "CountryName", consignee.CountryId);
            return View(consignee);
        }

        // POST: Consignees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ConsigneeId,ConsigneeName,Address1,Address2,Address3,Address4,Postcode,CountryId,Telephone")] Consignee consignee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consignee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(consignee);
        }

        // GET: Consignees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consignee consignee = db.Consignees.Find(id);
            if (consignee == null)
            {
                return HttpNotFound();
            }
            return View(consignee);
        }

        // POST: Consignees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Consignee consignee = db.Consignees.Find(id);
            db.Consignees.Remove(consignee);
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
