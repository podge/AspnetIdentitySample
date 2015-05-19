﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspnetIdentitySample.Models;

namespace AspnetIdentitySample
{
    public class ConsignorsController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Consignors
        public ActionResult Index()
        {
            return View(db.Consignors.ToList());
        }

        // GET: Consignors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consignor consignor = db.Consignors.Find(id);
            if (consignor == null)
            {
                return HttpNotFound();
            }
            return View(consignor);
        }

        // GET: Consignors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Consignors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ConsignorId,Name,Address1,Address2,Address3,Address4,Postcode,Telephone")] Consignor consignor)
        {
            if (ModelState.IsValid)
            {
                db.Consignors.Add(consignor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(consignor);
        }

        // GET: Consignors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consignor consignor = db.Consignors.Find(id);
            if (consignor == null)
            {
                return HttpNotFound();
            }
            return View(consignor);
        }

        // POST: Consignors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ConsignorId,Name,Address1,Address2,Address3,Address4,Postcode,Telephone")] Consignor consignor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consignor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(consignor);
        }

        // GET: Consignors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consignor consignor = db.Consignors.Find(id);
            if (consignor == null)
            {
                return HttpNotFound();
            }
            return View(consignor);
        }

        // POST: Consignors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Consignor consignor = db.Consignors.Find(id);
            db.Consignors.Remove(consignor);
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
