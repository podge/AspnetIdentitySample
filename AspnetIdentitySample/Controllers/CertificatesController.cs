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
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using Stripe;
using System.Web.Routing;

namespace AspnetIdentitySample.Controllers
{
    public class CertificatesController : Controller
    {
        string originalFile = "~/Documents/uk_cert.pdf";
        string copyOfOriginal = "~/Documents/newfile.pdf";
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
            certificate.Consignor = db.Consignors.Find(certificate.ConsignorId);
            certificate.Consignee = db.Consignees.Find(certificate.ConsigneeId);
            return View(certificate);
        }

        // GET: Certificates/Create
        public ActionResult Create()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());

            IQueryable<Consignor> consignorList = db.Consignors.Where(s => s.User.Id == currentUser.Id);
            IQueryable<Consignee> consigneeList = db.Consignees.Where(s => s.User.Id == currentUser.Id);

            int selectedConsignor = 0, selectedConsignee = 0;

            if (consignorList.AsQueryable().Count() == 1)
            {
                selectedConsignor = consignorList.First().ConsignorId;
            }

            if (consigneeList.AsQueryable().Count() == 1)
            {
                selectedConsignee = consigneeList.First().ConsigneeId;
            }

            ViewBag.ConsignorId = new SelectList(consignorList, "ConsignorId", "DropdownName", selectedConsignor);
            ViewBag.ConsigneeId = new SelectList(consigneeList, "ConsigneeId", "DropdownName", selectedConsignee);
            var pets = from s in db.Pets
                       where (s.User.Id == currentUser.Id)
                       select s;

            Certificate cert = new Certificate();
            cert.Paid = false;
            cert.Pets = pets.ToList();

            return View(cert);
        }

        // POST: Certificates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PetIDs,CertificateId,ConsignorId,ConsigneeId,CountryOfOrigin,ISOCode,CommodityDescription")] Certificate certificate)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            certificate.User = currentUser;

            certificate.Consignor = db.Consignors.Find(certificate.ConsignorId);
            certificate.Consignee = db.Consignees.Find(certificate.ConsigneeId);

            if (certificate.PetIDs != null && certificate.PetIDs.Count > 0 && certificate.PetIDs.Count <= 5)
            {
                List<Pet> certPets = new List<Pet>();
                int petCount = 1;
                foreach (int item in certificate.PetIDs)
                {
                    certPets.Add(db.Pets.Find(item));
                    switch (petCount)
                    {
                        case 1:
                            certificate.Pet1 = item;
                            break;
                        case 2:
                            certificate.Pet2 = item;
                            break;
                        case 3:
                            certificate.Pet3 = item;
                            break;
                        case 4:
                            certificate.Pet4 = item;
                            break;
                        case 5:
                            certificate.Pet5 = item;
                            break;
                        default:
                            break;
                    }
                    petCount++;
                }
                certificate.Pets = certPets;
                // Calculate Irregularities for all pets in the certificate
                certificate = doIrregularities(certificate);
            }
            else if (certificate.PetIDs == null || certificate.PetIDs.Count == 0)
            {
                ModelState.AddModelError("", "Please select at least one pet.");
            }
            else if (certificate.PetIDs.Count > 5)
            {
                ModelState.AddModelError("", "You may not select more than five pets.");
            }

            if (ModelState.IsValid)
            {
                db.Certificate.Add(certificate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                IQueryable<Consignor> consignorList = db.Consignors.Where(s => s.User.Id == currentUser.Id);
                IQueryable<Consignee> consigneeList = db.Consignees.Where(s => s.User.Id == currentUser.Id);

                int selectedConsignor = 0, selectedConsignee = 0;

                if (consignorList.AsQueryable().Count() == 1)
                {
                    selectedConsignor = consignorList.First().ConsignorId;
                }

                if (consigneeList.AsQueryable().Count() == 1)
                {
                    selectedConsignee = consigneeList.First().ConsigneeId;
                }

                ViewBag.ConsignorId = new SelectList(consignorList, "ConsignorId", "DropdownName", selectedConsignor);
                ViewBag.ConsigneeId = new SelectList(consigneeList, "ConsigneeId", "DropdownName", selectedConsignee);
                var pets = from s in db.Pets
                           where (s.User.Id == currentUser.Id)
                           select s;

                certificate.Pets = pets.ToList();
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

        //public FileResult DownloadFile(Certificate cert)
        //{
        //    doPdf(cert);
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath(copyOfOriginal));
        //    string fileName = "newFile.pdf";
        //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        //}

        public ActionResult Download(int id)
        {
            Certificate cert = db.Certificate.Find(id);
            Consignor Consignor = db.Consignors.Find(cert.ConsignorId);
            Consignee Consignee = db.Consignees.Find(cert.ConsigneeId);

            return DownloadFile(cert);
        }

        public FileResult DownloadFile(Certificate cert)
        {
            doPdf(cert);
            byte[] fileBytes = System.IO.File.ReadAllBytes(HttpContext.Server.MapPath(copyOfOriginal));
            string fileName = "newFile.pdf";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        private void doPdf(Certificate cert)
        {
            cert.Pets = getListPets(cert);

            string oldFile = HttpContext.Server.MapPath(originalFile);
            string newFile = HttpContext.Server.MapPath(copyOfOriginal);

            // open the reader
            PdfReader reader = new PdfReader(oldFile);
            Rectangle size = reader.GetPageSizeWithRotation(1);
            Document document = new Document(size);

            // open the writer
            FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();

            // the pdf content
            PdfContentByte cb = writer.DirectContent;

            // select the font properties
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb.SetColorFill(BaseColor.DARK_GRAY);
            cb.SetFontAndSize(bf, 8);

            // write the text in the pdf content
            cb.BeginText();
            // Consignor
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignor.ConsignorName, 125, 670, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignor.Address1 + ", " + cert.Consignor.Address2, 125, 660, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignor.Address3 + ", " + cert.Consignor.Address4, 125, 650, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignor.Telephone, 125, 640, 0);
            // Consignee
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignee.ConsigneeName, 125, 610, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignee.Address1 + ", " + cert.Consignee.Address2, 125, 600, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignee.Address3 + ", " + cert.Consignee.Address4, 125, 590, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignee.Postcode, 125, 580, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignee.Telephone, 125, 570, 0);
            cb.EndText();

            cb.BeginText();
            int yy = 150;
            foreach (Pet item in cert.Pets)
            {
                if (item != null)
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, item.Species.ScientificName, 60, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, item.Gender.GenderName, 110, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, item.IdentificationSystem.IdentificationSystemName, 145, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, item.Colour, 190, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, item.Breed, 240, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, item.DateOfMicrochipping.ToShortDateString(), 320, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, item.MicrochipNumber, 400, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, item.DateOfBirth.ToShortDateString(), 480, yy, 0);
                    yy -= 10;
                }
            }
            cb.EndText();

            // create the new page and add it to the pdf
            PdfImportedPage page = writer.GetImportedPage(reader, 1);
            cb.AddTemplate(page, 0, 0);

            // Page 2
            document.NewPage();
            page = writer.GetImportedPage(reader, 2);
            cb.AddTemplate(page, 0, 0);

            // Page 3 Rabies Vaccinations and Bloodtests
            document.NewPage();
            cb.SetFontAndSize(bf, 8);
            cb.BeginText();
            yy = 515;
            foreach (Pet item in cert.Pets)
            {
                if (item != null)
                {
                    var orderedRVList = item.RabiesVaccinations.OrderByDescending(x => DateTime.Parse(x.DateOfValidityFrom.ToShortDateString())).ToList();
                    var orderedBTList = item.FAVNBloodTests.OrderByDescending(x => DateTime.Parse(x.DateOfBloodtest.ToShortDateString())).ToList();
                    cb.ShowTextAligned(Element.ALIGN_LEFT, item.MicrochipNumber, 106, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, orderedRVList.First().DateOfRabiesVaccination.ToShortDateString(), 186, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, orderedRVList.First().Manufacturer, 242, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, orderedRVList.First().BatchNo, 294, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, orderedRVList.First().DateOfValidityFrom.ToShortDateString(), 350, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, orderedRVList.First().DateOfValidityTo.ToShortDateString(), 407, yy, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, orderedBTList.First().DateOfBloodtest.ToShortDateString(), 490, yy, 0);
                    yy -= 15;
                }
            }

            // Anti-Echinococcus Treatment for dogs only
            yy = 280;

            List<Pet> dogPets = new List<Pet>(); ;

            try
            {
                dogPets = cert.Pets.Where(p => p.Species.ScientificName == "Canine").ToList();
            }
            catch
            {
                // No dogs
            }              

            if (dogPets != null)
            {
                foreach (Pet item in dogPets)
                {
                    if (item != null)
                    {
                        cb.ShowTextAligned(Element.ALIGN_LEFT, item.MicrochipNumber, 106, yy, 0);
                        yy -= 15;
                    }
                }
            }

            cb.EndText();
            page = writer.GetImportedPage(reader, 3);
            cb.AddTemplate(page, 0, 0);

            // Page 4
            document.NewPage();
            page = writer.GetImportedPage(reader, 4);
            cb.AddTemplate(page, 0, 0);

            // Page 5
            document.NewPage();
            page = writer.GetImportedPage(reader, 5);
            cb.AddTemplate(page, 0, 0);

            // Page 6
            document.NewPage();
            page = writer.GetImportedPage(reader, 6);
            cb.AddTemplate(page, 0, 0);

            // Page 7
            document.NewPage();
            cb.SetFontAndSize(bf, 10);
            cb.BeginText();

            // Written Declaration
            yy = 505;
            foreach (Pet item in cert.Pets)
            {
                if (item != null)
                {
                    cb.ShowTextAligned(Element.ALIGN_LEFT, item.MicrochipNumber, 120, yy, 0);
                    yy -= 18;
                }                
            }

            cb.EndText();
            page = writer.GetImportedPage(reader, 7);
            cb.AddTemplate(page, 0, 0);

            // Page 8
            //document.NewPage();
            //page = writer.GetImportedPage(reader, 8);
            //cb.AddTemplate(page, 0, 0);

            //// Coords page
            //document.NewPage();
            //bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            //cb.SetColorFill(BaseColor.DARK_GRAY);
            //cb.SetFontAndSize(bf, 8);
            //cb.BeginText();
            //for (int x = 0; x <= 600; x += 50)
            //{
            //    for (int y = 0; y <= 900; y += 50)
            //    {

            //        string coords = x + "x" + y;
            //        // put the alignment and coordinates here
            //        cb.ShowTextAligned(Element.ALIGN_LEFT, coords, x, y, 0);
            //    }
            //}
            //cb.EndText();

            // close the streams and voilá the file should be changed :)
            document.Close();
            fs.Close();
            writer.Close();
            reader.Close();

            // close the streams and voilá the file should be changed :)
            document.Close();
            fs.Close();
            writer.Close();
            reader.Close();
        }

        public ActionResult Charge()
        {
            ViewBag.Message = "Learn how to process payments with Stripe";

            return View(new StripeChargeModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Charge(FormCollection form)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            var token = form.Get("stripeToken");
            var tokenType = form.Get("stripeTokenType");
            var email = form.Get("stripeEmail");

            StripeChargeModel scm = new StripeChargeModel();
            scm.Token = token;
            scm.Amount = 10; // €10
            scm.Email = email;

            string chargeId = "1";

            try
            {
                chargeId = await ProcessPayment(scm);
            }
            catch
            {
                // exception
            }

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            if (chargeId != "")
            {
                int certId = Convert.ToInt32(RouteData.Values["id"]);
                Certificate Cert = db.Certificate.Find(certId);
                Cert.PetIDs = getListPetIds(Cert);
                Cert.User = currentUser;
                Cert.Paid = true;
                Cert.CertificateId = certId;
                //db.Certificate.Add(Cert);
                db.Entry(Cert).State = EntityState.Modified;
                db.SaveChanges();
            }

            // You should do something with the chargeId --> Persist it maybe?
            // Get pets, consignors and consignees for current user
            // Warn if user needs to create any of these objects

            ViewBag.pets = db.Pets.Count(s => s.User.Id == currentUser.Id);
            ViewBag.consignors = db.Consignors.Count(s => s.User.Id == currentUser.Id);
            ViewBag.consignees = db.Consignees.Count(s => s.User.Id == currentUser.Id);

            //return View(db.Certificate.ToList().Where(s => s.User.Id == currentUser.Id));

            return View("Index", db.Certificate.ToList().Where(s => s.User.Id == currentUser.Id));
        }

        private async Task<string> ProcessPayment(StripeChargeModel model)
        {
            return await Task.Run(() =>
            {
                var myCharge = new StripeChargeCreateOptions
                {
                    // convert the amount of £12.50 to pennies i.e. 1250
                    Amount = (int)(model.Amount * 100),
                    Currency = "EUR",
                    Description = "Annex IV Certificate for Importing Pets into the EU",
                    CardId = model.Token,
                    ReceiptEmail = model.Email
                };

                var chargeService = new StripeChargeService("sk_test_OnzP859ZOl0l8x4XOUZqc8aK");

                StripeCharge stripeCharge = new StripeCharge();

                try
                {
                    stripeCharge = chargeService.Create(myCharge);
                }
                catch
                {
                    // Problem
                }

                return stripeCharge.Id;
            });
        }

        public List<int> getListPetIds(Certificate cert)
        {
            List<int> Pets = new List<int>();
            Pets.Add(cert.Pet1);
            Pets.Add(cert.Pet2);
            Pets.Add(cert.Pet3);
            Pets.Add(cert.Pet4);
            Pets.Add(cert.Pet5);
            return Pets;
        }

        public List<Pet> getListPets(Certificate cert)
        {
            List<Pet> Pets = new List<Pet>();
            Pets.Add(getPet(cert.Pet1));
            Pets.Add(getPet(cert.Pet2));
            Pets.Add(getPet(cert.Pet3));
            Pets.Add(getPet(cert.Pet4));
            Pets.Add(getPet(cert.Pet5));
            return Pets;
        }

        public Pet getPet(int id)
        {
            return db.Pets.Find(id);
        }

        public Certificate doIrregularities(Certificate certificate)
        {
            foreach (Pet item in certificate.Pets)
            {
                item.Irregularities = new List<Irregularity>();
                // Check if Rabies Vaccination has been given
                if (item.RabiesVaccinations.Count < 1)
                {
                    Irregularity irreg = new Irregularity();
                    irreg.IrregularityCode = 1;
                    irreg.IrregularityText = "Pet does not have a rabies vaccination";
                    irreg.DateRaised = new DateTime(2015, 05, 29);
                    irreg.DateResolved = new DateTime(2015, 05, 29);
                    item.Irregularities.Add(irreg);
                    //db.Irregularities.Add(irreg);
                    //db.SaveChanges();
                }
            }
            return certificate;
        }

    }
}
