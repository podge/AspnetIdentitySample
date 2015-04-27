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
        string originalFile = "c:\\eupp\\pet_cert.pdf";
        string copyOfOriginal = "c:\\eupp\\Copy.pdf";

        // GET: CertGenerator
        public ActionResult Index(int? id)
        {
            CreatePDFByCopy();
            return View();
        }

        private void CreatePDFByCopy()
        {
            using (Document document = new Document())
            {
                using (PdfSmartCopy copy = new PdfSmartCopy(document, new FileStream(copyOfOriginal, FileMode.Create)))
                {
                    document.Open();
                    for (int i = 1; i <= 9; ++i)
                    {
                        PdfReader reader = new PdfReader(AddDataSheets("Some Text" + i.ToString()));
                        copy.AddPage(copy.GetImportedPage(reader, i));
                    
                    }
                    document.Close();
                }
            }
        }

        public byte[] AddDataSheets(string _data)
        {
            string pdfTemplatePath = originalFile;
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
                        if (fieldKey.Contains("Address"))
                        {
                            form.SetField(fieldKey, _data);
                        }
                    }
                    stamper.FormFlattening = true;
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