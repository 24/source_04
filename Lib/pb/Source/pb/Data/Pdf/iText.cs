using iTextSharp.text.pdf;
using pb.IO;
using pb.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace pb.Data.Pdf
{
    public static class iText
    {
        public static void ExtractImages(string file, Action<Image, int, int> action, string range = null)
        {
            //Trace.WriteLine($"extract images from pdf \"{file}\" to \"{imageDirectory}\"");
            //if (!zPath.IsPathRooted(imageDirectory))
            //    imageDirectory = zPath.Combine(zPath.GetDirectoryName(file), imageDirectory);

            using (PdfReader pdfReader = new PdfReader(file))
            {
                //for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                foreach (int page in range != null ? zstr.EnumRange(range) : EnumPages(pdfReader.NumberOfPages))
                {
                    int imageIndex = 1;
                    foreach (int xrefIndex in EnumPageImagesIndex(pdfReader.GetPageN(page)))
                    {
                        //string imageFile = zPath.Combine(imageDirectory, $"page-{page:000}{(imageIndex != 1 ? $"-{imageIndex:00}" : "") }.jpeg");
                        //Trace.WriteLine($"save image to \"{imageFile}\"");
                        //SavePdfImage(pdfReader.GetPdfObject(xrefIndex), imageFile, ImageFormat.Jpeg);
                        //yield return GetImage(pdfReader.GetPdfObject(xrefIndex));
                        ExtractImage(pdfReader.GetPdfObject(xrefIndex), page, imageIndex++, action);
                    }
                }
            }
        }

        public static IEnumerable<int> EnumPages(int nb)
        {
            for (int page = 1; page <= nb; page++)
                yield return page;
        }

        // ExtractImages(string file, string range, )
        public static void ExtractImage(PdfObject pdfObj, int page, int imageIndex, Action<Image, int, int> action)
        {
            PdfStream pdfStrem = (PdfStream)pdfObj;
            byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
            if ((bytes != null))
            {
                using (MemoryStream memStream = new MemoryStream(bytes))
                {
                    memStream.Position = 0;
                    action(Image.FromStream(memStream), page, imageIndex);
                }
            }
        }

        public static IEnumerable<int> EnumPageImagesIndex(PdfDictionary page)
        {
            PdfDictionary pageResources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
            PdfDictionary pageObjects = (PdfDictionary)PdfReader.GetPdfObject(pageResources.Get(PdfName.XOBJECT));
            if (pageObjects != null)
            {
                foreach (PdfName name in pageObjects.Keys)
                {
                    PdfObject obj = pageObjects.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                        PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                        if (PdfName.IMAGE.Equals(type))
                        {
                            yield return ((PRIndirectReference)obj).Number;
                        }

                    }
                }
            }
        }

        // bad bad bad bad bad bad bad bad
        //public static Image GetImage(PdfObject pdfObj)
        //{
        //    PdfStream pdfStrem = (PdfStream)pdfObj;
        //    byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
        //    if ((bytes != null))
        //    {
        //        using (MemoryStream memStream = new MemoryStream(bytes))
        //        {
        //            memStream.Position = 0;
        //            return zimg.Clone(Image.FromStream(memStream));
        //        }
        //    }
        //    return null;
        //}

        public static void SavePdfImage(PdfObject pdfObj, string file, ImageFormat imageFormat)
        {
            PdfStream pdfStrem = (PdfStream)pdfObj;
            byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
            if ((bytes != null))
            {
                using (MemoryStream memStream = new MemoryStream(bytes))
                {
                    memStream.Position = 0;
                    using (Image image = Image.FromStream(memStream))
                    {
                        zfile.CreateFileDirectory(file);
                        image.Save(file, imageFormat);
                    }
                }
            }
        }
    }
}
