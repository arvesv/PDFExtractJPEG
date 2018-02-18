using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfextractjpeg
{
    public delegate void BinaryFileSaver(byte[] content);
    public delegate void TextFileSaver(IEnumerable<string> content);


    internal class TextFileSave
    {
        string myfilename;
        MemoryStream ms;
        StreamWriter sw;
        int pos = 0;

        internal TextFileSave(string filename)
        {
            myfilename = filename;
            ms = new MemoryStream();
            sw = new StreamWriter(ms);



        }

        internal void SaveToBuffer(IEnumerable<string> content)
        {
            if (content == null)
            {
                Flush();
                return;
            }
            foreach(var s in content) {

                sw.Write(s);
                sw.Write(" ");
                pos += s.Length +1;
                if(pos > 60)
                {
                    pos = 0;
                    sw.WriteLine();
                }
            }
        }

        internal void Flush()
        {
            sw.Flush();
            var w = ms.ToString();
            var fs = new FileStream(myfilename, FileMode.Create);

            ms.WriteTo(fs);
            fs.Flush();
            ms.Close();

        }

    }

    internal class FileSaver
    {
        string filenamebase;
        string filesuffix;
        int number = 1;

        internal FileSaver(string basename, string suffix)
        {
            filenamebase = basename;
            filesuffix = suffix;
        }

        internal string GetNextFileName()
        {
            var filename = $"{filenamebase}-{number}{filesuffix}";
            number++;
            return filename;
        }

        internal void Save(byte[] content)
        {
            var filename = GetNextFileName();

            if(File.Exists(filename))
            {
                var existingContent = File.ReadAllBytes(filename);
                if(StructuralComparisons.StructuralEqualityComparer.Equals(existingContent, content))
                {
                    return;
                }
            }

            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(content);
            bw.Close();
        }
    }
}
