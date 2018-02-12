using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfextractjpeg
{
    public delegate void BinaryFileSaver(byte[] content);

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


            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(content);
            bw.Close();
        }

    }
}
