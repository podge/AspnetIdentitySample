﻿using System;
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
using PagedList;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

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
        
        // GET: /Pet/
        // GET Pets for the logged in user
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var pets = from s in db.Pets where (s.User.Id == currentUser.Id)
                       select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                pets = pets.Where(s => s.Name.Contains(searchString)
                                       || s.MicrochipNumber.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    pets = pets.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    pets = pets.OrderBy(s => s.DateOfBirth);
                    break;
                case "date_desc":
                    pets = pets.OrderByDescending(s => s.DateOfBirth);
                    break;
                default:  // Name ascending 
                    pets = pets.OrderBy(s => s.Name);
                    break;
            }

            return View(pets.ToPagedList(pageNumber, pageSize));
        }

        // GET: /Pet/All
        [Authorize(Roles="Admin")]
        public async Task<ActionResult> All()
        {
            return View(await db.Pets.ToListAsync());
        }

        // GET: /Pet/Details/5
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

        // GET: /Pet/Create
        public ActionResult Create()
        {
            ViewBag.SpeciesId = new SelectList(db.Species, "SpeciesId", "SpeciesName");
            ViewBag.GenderId = new SelectList(db.Gender, "GenderId", "GenderName");
            return View();
        }

        // POST: /Pet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,SpeciesId,GenderId,DateOfBirth,Breed,MicrochipNumber")] Pet pet, HttpPostedFileBase upload)
        {
            pet.Species = db.Species.Find(pet.SpeciesId);
            pet.Gender = db.Gender.Find(pet.GenderId);
            ViewBag.SpeciesId = new SelectList(db.Species, "SpeciesId", "SpeciesName");
            ViewBag.GenderId = new SelectList(db.Gender, "GenderId", "GenderName");
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    var avatar = new PetFile
                    {
                        FileName = System.IO.Path.GetFileName(upload.FileName),
                        FileType = FileType.Avatar,
                        ContentType = upload.ContentType
                    };
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        avatar.Content = reader.ReadBytes(upload.ContentLength);
                        reader.Close();
                    }
                    pet.PetFiles = new List<PetFile> { avatar };

                    // Resize Image

                    //attach the uploaded image to the object before saving to Database
                    pet.ImageMimeType = upload.ContentLength;
                    pet.Image = new byte[upload.ContentLength];
                    upload.InputStream.Read(pet.Image, 0, upload.ContentLength);

                    //Save image to file
                    var filename = upload.FileName;
                    var filePathOriginal = Server.MapPath("/Content/Uploads/Originals");
                    var filePathThumbnail = Server.MapPath("/Content/Uploads/Thumbnails");
                    string savedFileName = Path.Combine(filePathOriginal, filename);
                    upload.SaveAs(savedFileName);

                    ////Read image back from file and create thumbnail from it
                    //var imageFile = Path.Combine(Server.MapPath("~/Content/Uploads/Originals"), filename);
                    //using (var srcImage = Image.FromFile(imageFile))
                    //using (var newImage = new Bitmap(100, 100))
                    //using (var graphics = Graphics.FromImage(newImage))
                    //using (var stream = new MemoryStream())
                    //{
                    //    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    //    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //    graphics.DrawImage(srcImage, new Rectangle(0, 0, 100, 100));
                    //    newImage.Save(stream, ImageFormat.Png);
                    //    var thumbNew = File(stream.ToArray(), "image/png");
                    //    pet.thumbnail = thumbNew.FileContents;
                    //}
                    
                }
                pet.User = currentUser;
                db.Pets.Add(pet);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(pet);
        }

        public FileContentResult GetThumbnailImage(int? petID)
        {
            Pet Pet = db.Pets.FirstOrDefault(p => p.Id == petID);
            if (Pet != null)
            {
                return File(Pet.thumbnail, Pet.ImageMimeType.ToString());
            }
            else
            {
                return null;
            }
        }

        // GET: /Pet/Edit/5
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
            ViewBag.SpeciesId = new SelectList(db.Species, "SpeciesId", "SpeciesName", pet.SpeciesId);
            ViewBag.GenderId = new SelectList(db.Gender, "GenderId", "GenderName", pet.GenderId);
            return View(pet);
        }

        // POST: /Pet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,SpeciesId,GenderId,DateOfBirth,Breed,MicrochipNumber")] Pet pet)
        {
            ViewBag.SpeciesId = new SelectList(db.Species, "SpeciesId", "SpeciesName");
            ViewBag.GenderId = new SelectList(db.Gender, "GenderId", "GenderName");
            if (ModelState.IsValid)
            {
                db.Entry(pet).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(pet);
        }

        // GET: /Pet/Delete/5
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

        // POST: /Pet/Delete/5
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
