using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
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

            TextFileSaver textfilesaver =
                (new TextFileSave(
                    Path.Combine(
                        Path.GetDirectoryName(pdffilenname),
                        Path.GetFileNameWithoutExtension(pdffilenname) + ".txt")))
                        .SaveToBuffer;

            ExtractStuffFromPdf(pdffilenname, filenamegenerator, textfilesaver);
        }


        static void ExtractStuffFromPdf(string pdffile, BinaryFileSaver filenamegenerator, TextFileSaver textsave)
        {
            foreach(var page in PdfReader.Open(pdffile).Pages) 
            {
                textsave(ExtractText(page));

                ICollection< PdfItem > items =
                    page.Elements.GetDictionary("/Resources")?
                    .Elements.GetDictionary("/XObject")
                    .Elements.Values;

                foreach (PdfItem item in items)
                {
                    PdfDictionary xObject = (item as PdfReference)?.Value as PdfDictionary;
                    if (xObject != null && xObject.Elements.GetString("/Subtype") == "/Image")
                    {
                        ExportImage(xObject, filenamegenerator);
                    }
                }
            }

            textsave(null);
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

        public static IEnumerable<string> ExtractText(PdfPage page)
        {
            var content = ContentReader.ReadContent(page);
            var text = ExtractText(content);
            return text;
        }

        public static IEnumerable<string> ExtractText(CObject cObject)
        {
            if (cObject is COperator)
            {
                var cOperator = cObject as COperator;
                if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                    cOperator.OpCode.Name == OpCodeName.TJ.ToString())
                {
                    foreach (var cOperand in cOperator.Operands)
                        foreach (var txt in ExtractText(cOperand))
                            yield return txt;
                }
            }
            else if (cObject is CSequence)
            {
                var cSequence = cObject as CSequence;
                foreach (var element in cSequence)
                    foreach (var txt in ExtractText(element))
                        yield return txt;
            }
            else if (cObject is CString)
            {
                var cString = cObject as CString;
                yield return cString.Value.Replace("\0", string.Empty);
            }
        }
    }
}
