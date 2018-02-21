# PDFExtractJPEG
This is a tool created for extracting JPEG images and text from PDFs created
by [Scanner Pro](https://readdle.com/products/scannerpro)

I made this program is to scratch my own very specific itch, so it
may not be usable for anyone else. The other command line tools I found for the same purpose
(XPFD/pdfimages) crashed when running on PDFs created by Scanner Pro. Therefore 
I created my own tool using the [PDFsharp](http://www.pdfsharp.net) library.
The code is inspired by [this example](http://www.pdfsharp.net/wiki/ExportImages-sample.ashx)
but changed a bit, again to avoid crash when processing files created by Scanner Pro.

And yes I know that Scanner Pro can make both PDF and JPG. I usually like PDF
(you can get one document with many pages), but I want the option to extract
the JPG if i need to.
