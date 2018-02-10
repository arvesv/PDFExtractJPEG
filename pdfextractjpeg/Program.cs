using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfextractjpeg
{
    class Program
    {
        static void Main(string[] args)
        {
            int imageCount = 0;
            var pdffile = @"C:\Users\Arve\OneDrive\ScannerPro\Scan 20 Nov 2017 at 15.33.pdf";

            foreach(var page in PdfReader.Open(pdffile).Pages) 
            {            
                ICollection < PdfItem > items =
                    page.Elements.GetDictionary("/Resources")?
                    .Elements.GetDictionary("/XObject")
                    .Elements.Values;

                foreach (PdfItem item in items)
                {
                    PdfDictionary xObject = (item as PdfReference)?.Value as PdfDictionary;
                    if (xObject != null && xObject.Elements.GetString("/Subtype") == "/Image")
                    {
                        ExportImage(xObject, ref imageCount);
                    }
                }
            }
        }

        private static void ExportImage(PdfDictionary xObject, ref int imageCount)
        {
            int count = 0;
            foreach (var elem in xObject.Elements)
            {
                if(elem.Key == "/Filter")
                {
                    var z = elem;
                    PdfArray  x = elem.Value as PdfArray;
                    var qah = x.First().ToString();

                    if(qah == "/DCTDecode")
                    {
                        byte[] stream = xObject.Stream.Value;

                        FileStream fs = new FileStream(String.Format("e:\\Image{0}.jpeg", count++), FileMode.Create, FileAccess.Write);
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(stream);
                        bw.Close();
                    }
                }
            }
        }
    }
}
