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
            string pdffilenname = args[0];

            BinaryFileSaver filenamegenerator =
                (new FileSaver(
                    Path.Combine(
                        Path.GetDirectoryName(pdffilenname),
                        Path.GetFileNameWithoutExtension(pdffilenname)),
                    ".jpg")
                 ).Save;

            ExtractJpegFromPdf(pdffilenname, filenamegenerator);
        }


        static int ExtractJpegFromPdf(string pdffile, BinaryFileSaver filenamegenerator)
        {
            int count = 0;
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
                        ExportImage(xObject, filenamegenerator);
                        count++;
                    }
                }
            }
            return count;
        }

        private static void ExportImage(PdfDictionary xObject, BinaryFileSaver filesaver)
        {
            foreach (var elem in xObject.Elements)
            {
                if(elem.Key == "/Filter")
                {
                    var z = elem;
                    PdfArray  x = elem.Value as PdfArray;
                    var qah = x.First().ToString();

                    if(qah == "/DCTDecode")
                    {
                        filesaver(xObject.Stream.Value);
                    }
                }
            }
        }
    }
}
