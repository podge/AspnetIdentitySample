using AspnetIdentitySample.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace AspnetIdentitySample.Controllers
{
    public class CertGeneratorController : Controller
    {
        string originalFile = "~/Documents/uk_cert.pdf";
        string copyOfOriginal = "~/Documents/newfile.pdf";

        private MyDbContext db;
        private UserManager<ApplicationUser> manager;
        public CertGeneratorController()
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

            Certificate cert = new Certificate();
            Consignor myConor = new Consignor("Padraic Lavin", "Canonbrook Court", "Lucan", "Co. Dublin", "Ireland", "00000", "353861061676");
            Consignee myConee = new Consignee("Padraic Lavin", "Canonbrook Court", "Lucan", "Co. Dublin", "Ireland", "00000", "353861061676");
            
            cert.Consignor = myConor;
            cert.Consignee = myConee;
            cert.Pets = pets.ToList();

            return View(cert);

            //return View(pets.ToPagedList(pageNumber, pageSize));
        }

        // GET: CertGenerator
        //public ActionResult Index(int? id)
        //{
        //    //CreatePDFByCopy();
        //    return View();
        //}

        // GET: CertGenerator
        public ActionResult Create(FormCollection formCollection)
        {
            //CreatePDFByCopy();
            Certificate cert = new Certificate();
            List<Pet> certPets = new List<Pet>();

            foreach (var key in formCollection.AllKeys)
            {
                if (key.StartsWith("selected_"))
                {
                    string petId = key.Replace("selected_", "");
                    Pet pet = db.Pets.Find(int.Parse(petId));
                    certPets.Add(pet);
                }                
            }
            if (certPets.Count == 0)
            {
                // Must select at least one pet
                ModelState.AddModelError(string.Empty, "You must select at least one pet.");
                return RedirectToAction("Index", "CertGenerator");
            }
            else if (certPets.Count > 5)
            {
                // A maximum of 5 pets may be selected
                ModelState.AddModelError(string.Empty, "You cannot select more than five pets.");
                return RedirectToAction("Index", "CertGenerator");
            }

            Consignor consignor = new Consignor(Request.Form["Consignor.Name"], Request.Form["Consignor.Address1"], Request.Form["Consignor.Address2"], Request.Form["Consignor.Address3"], Request.Form["Consignor.Address4"], Request.Form["Consignor.Postcode"], Request.Form["Consignor.Telephone"]);
            Consignee consignee = new Consignee(Request.Form["Consignee.Name"], Request.Form["Consignee.Address1"], Request.Form["Consignee.Address2"], Request.Form["Consignee.Address3"], Request.Form["Consignee.Address4"], Request.Form["Consignee.Postcode"], Request.Form["Consignee.Telephone"]);
            
            cert.Consignor = consignor;
            cert.Consignee = consignee;
            cert.Pets = certPets;

            db.Certificate.Add(cert);
            db.SaveChangesAsync();

            //return DownloadFile(cert);
            return View(cert);
        }

        // POST: /CertGenerator/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Consignor.Name,SpeciesId,GenderId,DateOfBirth,Breed,MicrochipNumber")] Certificate cert)
        //{
            
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Download(Certificate cert)
        {
            Certificate myCert = new Certificate();
            Consignor myConor = new Consignor("Padraic Lavin", "Canonbrook Court", "Lucan", "Co. Dublin", "Ireland", "00000", "353861061676");
            Consignee myConee = new Consignee("Padraic Lavin", "Canonbrook Court", "Lucan", "Co. Dublin", "Ireland", "00000", "353861061676");
            
            myCert.Consignor = myConor;
            myCert.Consignee = myConee;

            return DownloadFile(myCert);
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
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignor.Name, 125, 670, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignor.Address1 + ", " + cert.Consignor.Address2, 125, 660, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignor.Address3 + ", " + cert.Consignor.Address4, 125, 650, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignor.Telephone, 125, 640, 0);
            // Consignee
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignee.Name, 125, 610, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignee.Address1 + ", " + cert.Consignee.Address2, 125, 600, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignee.Address3 + ", " + cert.Consignee.Address4, 125, 590, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignee.PostCode, 125, 580, 0);
            cb.ShowTextAligned(Element.ALIGN_LEFT, cert.Consignee.Telephone, 125, 570, 0);
            cb.EndText();

            cb.BeginText();
            int yy = 150;
            foreach (Pet item in cert.Pets)
            {
                cb.ShowTextAligned(Element.ALIGN_LEFT, item.Species.ScientificName, 60, yy, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT, item.Gender.GenderName, 110, yy, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT, item.IdentificationSystem.IdentificationSystemName, 150, yy, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT, item.Colour, 190, yy, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT, item.Breed, 240, yy, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT, item.DateOfMicrochipping.ToShortDateString(), 320, yy, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT, item.MicrochipNumber, 400, yy, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT, item.DateOfBirth.ToShortDateString(), 480, yy, 0);
                yy -= 10;
            }
            cb.EndText();

            // create the new page and add it to the pdf
            PdfImportedPage page = writer.GetImportedPage(reader, 1);
            cb.AddTemplate(page, 0, 0);

            // Page 2
            document.NewPage();
            page = writer.GetImportedPage(reader, 2);
            cb.AddTemplate(page, 0, 0);

            // Page 3
            document.NewPage();
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
            page = writer.GetImportedPage(reader, 7);
            cb.AddTemplate(page, 0, 0);

            // Page 8
            document.NewPage();
            page = writer.GetImportedPage(reader, 8);
            cb.AddTemplate(page, 0, 0);

            // Coords page
            document.NewPage();
            bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb.SetColorFill(BaseColor.DARK_GRAY);
            cb.SetFontAndSize(bf, 8);
            cb.BeginText();
            for (int x=0; x <= 600; x += 50)
            {
                for (int y=0; y <= 900; y += 50)
                {
                    
                    string coords = x+"x"+y;
                    // put the alignment and coordinates here
                    cb.ShowTextAligned(Element.ALIGN_LEFT, coords, x, y, 0);
                }
            }
            cb.EndText();

            //// Page 5
            //document.NewPage();
            //page = writer.GetImportedPage(reader, 5);
            //cb.AddTemplate(page, 0, 0);

            //// Page 6
            //document.NewPage();
            //page = writer.GetImportedPage(reader, 6);
            //cb.AddTemplate(page, 0, 0);

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


        private void CreatePDFByCopy()
        {
            using (Document document = new Document())
            {
                using (PdfSmartCopy copy = new PdfSmartCopy(document, new FileStream(Server.MapPath(copyOfOriginal), FileMode.Create)))
                {
                    document.Open();
                    for (int i = 1; i <= 1; ++i)
                    {
                        PdfReader reader = new PdfReader(AddDataSheets("Some Text" + i.ToString()));
                        PdfImportedPage impPage = copy.GetImportedPage(reader, i);
                        copy.AddPage(impPage);
                    
                    }
                    document.Close();
                }
            }
        }

        public byte[] AddDataSheets(string _data)
        {
            string pdfTemplatePath = Server.MapPath(originalFile);
            PdfReader reader = new PdfReader(pdfTemplatePath);
            using (MemoryStream ms = new MemoryStream())
            {
                using (PdfStamper stamper = new PdfStamper(reader, ms))
                {
                    AcroFields form = stamper.AcroFields;
                    var fieldKeys = form.Fields.Keys;
                    foreach (string fieldKey in fieldKeys)
                    {
                        //Change some data
                        if (fieldKey.Contains("name") || fieldKey.Contains("address"))
                        {
                            form.SetField(fieldKey, _data);
                            //form.SetFieldProperty(fieldKey, "name", 0, null);
                        }
                    }
                    //stamper.FormFlattening = true;
                }
                return ms.ToArray();
            }
        }

        public void first(){
            //FileStream fs = new FileStream("c:\\eupp\\Chapter1_Example1.pdf", FileMode.Create, FileAccess.Write, FileShare.None);

            using (FileStream fs = new FileStream(originalFile, FileMode.Create, FileAccess.Write, FileShare.None))
            using (Document doc = new Document(PageSize.LETTER))
            using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
            {
                doc.Open();
                doc.Add(new Paragraph("Hi! I'm Original"));
                doc.Close();
            }
            PdfReader reader = new PdfReader(originalFile);
            using (FileStream fs = new FileStream(copyOfOriginal, FileMode.Create, FileAccess.Write, FileShare.None))
            using (PdfStamper stamper = new PdfStamper(reader, fs)) { }
        }
    }
}