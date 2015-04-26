using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspnetIdentitySample.Controllers
{
    public class CertGeneratorController : Controller
    {
        // GET: CertGenerator
        public ActionResult Index(int? id)
        {
            //FileStream fs = new FileStream("c:\\eupp\\Chapter1_Example1.pdf", FileMode.Create, FileAccess.Write, FileShare.None);

            string originalFile = "c:\\eupp\\pet_cert.pdf";
            string copyOfOriginal = "c:\\eupp\\Copy.pdf";
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

            return View();
        }
    }
}