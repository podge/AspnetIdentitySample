﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AspnetIdentitySample.Models;

namespace AspnetIdentitySample.Views.CertGenerator
{
    public class ConsigneesController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Consignees
        public ActionResult Index()
        {
            return View(db.Consignees.ToList());
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
            return View();
        }

        // POST: Consignees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ConsigneeId,Name,Address1,Address2,Address3,Address4,PostCode,Telephone")] Consignee consignee)
        {
            if (ModelState.IsValid)
            {
                db.Consignees.Add(consignee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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
            return View(consignee);
        }

        // POST: Consignees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ConsigneeId,Name,Address1,Address2,Address3,Address4,PostCode,Telephone")] Consignee consignee)
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
