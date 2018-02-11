using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfextractjpeg
{
    public delegate string GenFileName();

    internal class GenerateFileName
    {
        string filenamebase;
        string filesuffix;
        int number = 1;

        internal GenerateFileName(string basename, string suffix)
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


    }
}
