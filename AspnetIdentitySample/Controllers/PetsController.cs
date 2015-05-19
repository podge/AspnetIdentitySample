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
using PagedList;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Helpers;

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

            var pets = from s in db.Pets
                       where (s.User.Id == currentUser.Id)
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
        [Authorize(Roles = "Admin")]
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
            if (pet.User.Id != currentUser.Id && !User.IsInRole("Admin"))
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
            ViewBag.IdentificationSystemId = new SelectList(db.IdentificationSystem, "IdentificationSystemId", "IdentificationSystemName");
            return View();
        }

        // POST: /Pet/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,SpeciesId,GenderId,DateOfBirth,Breed,IdentificationSystemId,MicrochipNumber")] Pet pet, HttpPostedFileBase upload)
        {
            pet.Species = db.Species.Find(pet.SpeciesId);
            pet.Gender = db.Gender.Find(pet.GenderId);
            ViewBag.SpeciesId = new SelectList(db.Species, "SpeciesId", "SpeciesName");
            ViewBag.GenderId = new SelectList(db.Gender, "GenderId", "GenderName");
            ViewBag.IdentificationSystemId = new SelectList(db.IdentificationSystem, "IdentificationSystemId", "IdentificationSystemName");
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
                    }
                    pet.PetFiles = new List<PetFile> { avatar };

                    // Resize avatar to make thumbnail
                    PetFile thumbFile = new PetFile();
                    thumbFile.Content = ResizeImage(avatar.Content, 100, 100, false);
                    thumbFile.FileName = "thumb" + avatar.FileName;
                    thumbFile.FileType = FileType.Thumbnail;
                    thumbFile.ContentType = avatar.ContentType;
                    pet.PetFiles.Add(thumbFile);

                }
                pet.User = currentUser;
                db.Pets.Add(pet);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(pet);
        }

        /// <summary>
        /// Allows for image resizing. if AllowLargerImageCreation = true 
        /// you want to increase the size of the image
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="NewWidth"></param>
        /// <param name="MaxHeight"></param>
        /// <param name="AllowLargerImageCreation"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] ResizeImage(byte[] bytes, int NewWidth, int MaxHeight, bool AllowLargerImageCreation)
        {

            Image FullsizeImage = null;
            //Cast bytes to an image
            FullsizeImage = byteArrayToImage(bytes);

            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            // If we are re sizing upwards to a bigger size
            if (AllowLargerImageCreation)
            {
                if (FullsizeImage.Width <= NewWidth)
                {
                    NewWidth = FullsizeImage.Width;
                }
            }

            //Keep aspect ratio
            int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
            if (NewHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                NewHeight = MaxHeight;
            }

            Bitmap result = new Bitmap(NewWidth, NewHeight);
            //set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(FullsizeImage.HorizontalResolution, FullsizeImage.VerticalResolution);

            //use a graphics object to draw the resized image into the bitmap
            using (Graphics graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(FullsizeImage, 0, 0, result.Width, result.Height);
            }

            // Clear handle to original file so that we can overwrite it if necessary
            FullsizeImage.Dispose();

            //return imageToByteArray(ResizedImage);
            return imageToByteArray(result);
        }


        /// <summary>
        /// convert image to byte array
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        private static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }


        /// <summary>
        /// Convert a byte array to an image
        /// </summary>
        /// <remarks></remarks>
        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
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
            if (pet.User.Id != currentUser.Id && !User.IsInRole("Admin"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            ViewBag.SpeciesId = new SelectList(db.Species, "SpeciesId", "SpeciesName", pet.SpeciesId);
            ViewBag.GenderId = new SelectList(db.Gender, "GenderId", "GenderName", pet.GenderId);
            ViewBag.IdentificationSystemId = new SelectList(db.IdentificationSystem, "IdentificationSystemId", "IdentificationSystemName", pet.IdentificationSystemId);
            return View(pet);
        }

        // POST: /Pet/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,SpeciesId,GenderId,DateOfBirth,Breed,IdentificationSystemId,MicrochipNumber")] Pet pet, HttpPostedFileBase upload)
        {
            ViewBag.SpeciesId = new SelectList(db.Species, "SpeciesId", "SpeciesName");
            ViewBag.GenderId = new SelectList(db.Gender, "GenderId", "GenderName");
            ViewBag.IdentificationSystemId = new SelectList(db.IdentificationSystem, "IdentificationSystemId", "IdentificationSystemName");
            
            Pet petToUpdate = db.Pets.Find(pet.Id);

            if (upload != null && upload.ContentLength > 0)
            {

                if (petToUpdate.PetFiles.Any(f => f.FileType == FileType.Avatar))
                {
                    db.PetFiles.Remove(petToUpdate.PetFiles.First(f => f.FileType == FileType.Avatar));
                }
                if (petToUpdate.PetFiles.Any(f => f.FileType == FileType.Thumbnail))
                {
                    db.PetFiles.Remove(petToUpdate.PetFiles.First(f => f.FileType == FileType.Thumbnail));
                }

                var avatar = new PetFile
                {
                    FileName = System.IO.Path.GetFileName(upload.FileName),
                    FileType = FileType.Avatar,
                    ContentType = upload.ContentType
                };
                using (var reader = new System.IO.BinaryReader(upload.InputStream))
                {
                    avatar.Content = reader.ReadBytes(upload.ContentLength);
                }

                // Resize avatar to make thumbnail
                PetFile thumbFile = new PetFile();
                thumbFile.Content = ResizeImage(avatar.Content, 100, 100, false);
                thumbFile.FileName = "thumb" + avatar.FileName;
                thumbFile.FileType = FileType.Thumbnail;
                thumbFile.ContentType = avatar.ContentType;
                
                petToUpdate.PetFiles = new List<PetFile> { avatar };
                petToUpdate.PetFiles.Add(thumbFile);
            }

            petToUpdate.Breed = pet.Breed;
            petToUpdate.DateOfBirth = pet.DateOfBirth;
            petToUpdate.Gender = pet.Gender;
            petToUpdate.GenderId = pet.GenderId;
            petToUpdate.IdentificationSystem = pet.IdentificationSystem;
            petToUpdate.IdentificationSystemId = pet.IdentificationSystemId;
            petToUpdate.MicrochipNumber = pet.MicrochipNumber;
            petToUpdate.Name = pet.Name;
            petToUpdate.Species = pet.Species;
            petToUpdate.SpeciesId = pet.SpeciesId;

            if (ModelState.IsValid)
            {
                db.Entry(petToUpdate).State = EntityState.Modified;
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
            if (pet.User.Id != currentUser.Id && !User.IsInRole("Admin"))
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
