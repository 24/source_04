using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using pb.IO;
using pb.Text;
using System.Collections.Generic;

namespace pb.Data.Pdf
{
    public class PdfImage
    {
        public PdfImageXObject Image;
        public int PageNumber;
        public int ImageNumber;
    }

    public static class iText7
    {
        public static IEnumerable<PdfImage> EnumImages(string file, string range = null)
        {
            using (PdfReader pdfReader = new PdfReader(file))
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                //for (int pageNumber = 1; pageNumber <= pdfDocument.GetNumberOfPages(); pageNumber++)
                foreach (int pageNumber in range != null ? zstr.EnumRange(range) : EnumPages(pdfDocument.GetNumberOfPages()))
                {
                    int imageIndex = 1;
                    foreach (PdfObject imageObject in EnumPageImages(pdfDocument.GetPage(pageNumber)))
                    {
                        if (imageObject is PdfStream)
                            yield return new PdfImage { Image = new PdfImageXObject((PdfStream)imageObject), PageNumber = pageNumber, ImageNumber = imageIndex++ };
                    }
                }
            }
        }

        public static IEnumerable<int> EnumPages(int nb)
        {
            for (int page = 1; page <= nb; page++)
                yield return page;
        }

        public static IEnumerable<PdfObject> EnumPageImages(PdfPage page)
        {
            PdfDocument pdfDocument = page.GetDocument();
            foreach (PdfObject pageObject in page.GetResources().GetResource(PdfName.XObject).Values())
            {

                if (pageObject.IsIndirect())
                {
                    PdfObject directObject = pdfDocument.GetPdfObject(pageObject.GetIndirectReference().GetObjNumber());
                    if (directObject is PdfDictionary)
                    {
                        if (((PdfDictionary)directObject).Get(PdfName.Subtype).ToString() == "/Image")
                        {
                            yield return directObject;
                        }
                    }
                }
            }
        }

        public static void SaveImage(PdfImageXObject image, string file)
        {
            //zfile.CreateFileDirectory(file);
            zFile.WriteAllBytes(file + "." + image.IdentifyImageFileExtension(), image.GetImageBytes());
        }
    }
}
