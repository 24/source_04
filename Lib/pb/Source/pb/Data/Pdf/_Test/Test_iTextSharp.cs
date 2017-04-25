using iTextSharp.text.pdf;
using pb.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace pb.Data.Pdf.Test
{
    public class PdfObjectInfo
    {
        public PdfObject PdfObject;
        public int Page;
        public int Index;
    }

    public static class Test_iTextSharp
    {
        public static void Test_TraceInfos(string file)
        {
            Trace.WriteLine($"pdf \"{file}\"");
            using (PdfReader pdfReader = new PdfReader(file))
            {
                int imagesCount = GetImagesCount(pdfReader);
                Trace.WriteLine($"{pdfReader.NumberOfPages} pages {imagesCount} images {pdfReader.XrefSize} object");
            }
        }

        public static int GetImagesCount(PdfReader pdfReader)
        {
            int objectCount = pdfReader.XrefSize;
            int count = 0;
            for (int i = 0; i < objectCount; i++)
            {
                PdfObject obj = pdfReader.GetPdfObject(i);
                if (obj is PdfDictionary)
                {
                    PdfDictionary objDic = (PdfDictionary)obj;
                    if (objDic.Contains(PdfName.TYPE) && objDic.Get(PdfName.TYPE).ToString() == "/XObject" && objDic.Contains(PdfName.SUBTYPE) && objDic.Get(PdfName.SUBTYPE).ToString() == "/Image")
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public static void Test_TraceImages(string file)
        {
            Trace.WriteLine($"pdf \"{file}\"");
            using (PdfReader pdfReader = new PdfReader(file))
            {
                int objectCount = pdfReader.XrefSize;
                for (int i = 0; i < objectCount; i++)
                {
                    PdfObject obj = pdfReader.GetPdfObject(i);
                    if (obj is PdfDictionary)
                    {
                        PdfDictionary objDic = (PdfDictionary)obj;
                        if (objDic.Contains(PdfName.TYPE) && objDic.Get(PdfName.TYPE).ToString() == "/XObject" && objDic.Contains(PdfName.SUBTYPE) && objDic.Get(PdfName.SUBTYPE).ToString() == "/Image")
                        {
                            string filter = objDic.Get(PdfName.FILTER).ToString();
                            int width = int.Parse(objDic.Get(PdfName.WIDTH).ToString());
                            int height = int.Parse(objDic.Get(PdfName.HEIGHT).ToString());
                            string bpp = objDic.Get(PdfName.BITSPERCOMPONENT).ToString();
                            Trace.WriteLine($"object {i + 1,3} image width {width} height {height} filter {filter} bits per component {bpp}");
                        }
                    }
                }
            }
        }

        public static void Test_TraceObjects(string file)
        {
            Trace.WriteLine($"pdf \"{file}\"");
            using (PdfReader pdfReader = new PdfReader(file))
            {
                int objectCount = pdfReader.XrefSize;
                for (int i = 0; i < objectCount; i++)
                {
                    PdfObject obj = pdfReader.GetPdfObject(i);
                    //if (obj != null)
                    if (obj is PdfDictionary)
                    {
                        PdfDictionary objDic = (PdfDictionary)obj;
                        string type = null;
                        if (objDic.Contains(PdfName.TYPE))
                            type = objDic.Get(PdfName.TYPE).ToString();
                        string subtype = null;
                        if (objDic.Contains(PdfName.SUBTYPE))
                            subtype = objDic.Get(PdfName.SUBTYPE).ToString();
                        Trace.WriteLine($"object {i + 1,3} object type {obj.GetType()} dictionary type {type} subtype {subtype}");
                    }
                    else if (obj != null)
                    {
                        Trace.WriteLine($"object {i + 1,3} object type {obj.GetType()}");
                    }
                    else
                        Trace.WriteLine($"object {i + 1,3} null");
                }
            }
        }

        public static void Test_ExtractImage(string file, int index, string imageFile)
        {
            Trace.WriteLine($"extract image index {index} from pdf \"{file}\" to \"{imageFile}\"");
            if (!zPath.IsPathRooted(imageFile))
                imageFile = zPath.Combine(zPath.GetDirectoryName(file), imageFile);
            using (PdfReader pdfReader = new PdfReader(file))
            {
                PdfObject obj = pdfReader.GetPdfObject(index);
                if (!(obj is PdfDictionary))
                {
                    Trace.WriteLine("object is not dictionary");
                    return;
                }
                PdfDictionary objDic = (PdfDictionary)obj;
                if (!objDic.Contains(PdfName.TYPE) || objDic.Get(PdfName.TYPE).ToString() != "/XObject" || !objDic.Contains(PdfName.SUBTYPE) || objDic.Get(PdfName.SUBTYPE).ToString() != "/Image")
                {
                    Trace.WriteLine("object is not an image");
                    return;
                }
                //iTextSharp.text.pdf
                //Image
                //iTextSharp.text.pdf.PdfImage
                //PdfImage pdfImage = new PdfImage();
                byte[] bytes = PdfReader.FlateDecode(PdfReader.GetStreamBytesRaw((PRStream)obj), true);
                //byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)obj);
                // error : The byte array is not a recognized imageformat
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(bytes);
                Trace.WriteLine($"width {image.Width} height {image.Height} dpiX {image.DpiX} dpiY {image.DpiY}");

                //image.IsContent();
                //image.IsImgRaw();
                //image.IsJpeg();
                //image.OriginalType;
                //image.RawData;
                //image.Type;
            }

        }

        public static void Test_ExtractImages(string file, string imageDirectory)
        {
            // from http://stackoverflow.com/questions/802269/extract-images-using-itextsharp/804392#804392

            Trace.WriteLine($"extract images from pdf \"{file}\" to \"{imageDirectory}\"");
            if (!zPath.IsPathRooted(imageDirectory))
                imageDirectory = zPath.Combine(zPath.GetDirectoryName(file), imageDirectory);
            using (PdfReader pdfReader = new PdfReader(file))
            {
                int index = 1;
                int objectCount = pdfReader.XrefSize;
                for (int i = 0; i < objectCount; i++)
                {
                    PdfObject obj = pdfReader.GetPdfObject(i);
                    if (obj is PdfDictionary)
                    {
                        PdfDictionary objDic = (PdfDictionary)obj;
                        if (objDic.Contains(PdfName.TYPE) && objDic.Get(PdfName.TYPE).ToString() == "/XObject" && objDic.Contains(PdfName.SUBTYPE) && objDic.Get(PdfName.SUBTYPE).ToString() == "/Image")
                        {
                            string filter = objDic.Get(PdfName.FILTER).ToString();
                            int width = int.Parse(objDic.Get(PdfName.WIDTH).ToString());
                            int height = int.Parse(objDic.Get(PdfName.HEIGHT).ToString());
                            string bpp = objDic.Get(PdfName.BITSPERCOMPONENT).ToString();
                            Trace.WriteLine($"object {i + 1} image width {width} height {height} filter {filter} bits per component {bpp}");
                            if (filter == "/FlateDecode")
                            {
                                byte[] arr = PdfReader.FlateDecode(PdfReader.GetStreamBytesRaw((PRStream)obj), true);
                                Trace.WriteLine($"  bytes count {arr.Length}");
                                // PixelFormat.Format24bppRgb
                                // System.Drawing.Imaging.PixelFormat 8 bits
                                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                                // PixelFormat.Format24bppRgb
                                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                                Marshal.Copy(arr, 0, bmpData.Scan0, arr.Length);
                                bmp.UnlockBits(bmpData);
                                //bmp.Save(zPath.Combine(imageDirectory, $"image-{index++:000}.jpeg"), ImageFormat.Jpeg);
                                bmp.Save(zPath.Combine(imageDirectory, $"image-{index++:000}.png"), ImageFormat.Png);
                            }
                        }

                    }
                    //if (obj != null && obj.IsStream())
                    //{
                    //    PdfDictionary objDic = (PdfDictionary)obj;
                    //    if (objDic.Contains(PdfName.SUBTYPE) && objDic.Get(PdfName.SUBTYPE).ToString() == "/Image")
                    //    {
                    //        string filter = objDic.Get(PdfName.FILTER).ToString();
                    //        int width = int.Parse(objDic.Get(PdfName.WIDTH).ToString());
                    //        int height = int.Parse(objDic.Get(PdfName.HEIGHT).ToString());
                    //        string bpp = objDic.Get(PdfName.BITSPERCOMPONENT).ToString();
                    //        Trace.WriteLine($"object {i + 1} image width {width} height {height} filter {filter} bits per component {bpp}");
                    //        //string extent = ".";
                    //        //byte[] img = null;
                    //        switch (filter)
                    //        {
                    //            case "/FlateDecode":
                    //                byte[] arr = PdfReader.FlateDecode(PdfReader.GetStreamBytesRaw((PRStream)obj), true);
                    //                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                    //                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                    //                Marshal.Copy(arr, 0, bmpData.Scan0, arr.Length);
                    //                bmp.UnlockBits(bmpData);
                    //                //bmp.Save("c:\\temp\\bmp1.png", ImageFormat.Png);
                    //                bmp.Save(zPath.Combine(imageDirectory, $"image-{index++:000}.jpeg"), ImageFormat.Jpeg);
                    //                break;
                    //            default:
                    //                break;
                    //        }
                    //    }
                    //}
                }
            }
        }

        public static void Test_ExtractImages_v2(string file, string imageDirectory)
        {
            Trace.WriteLine($"extract images from pdf \"{file}\" to \"{imageDirectory}\"");
            if (!zPath.IsPathRooted(imageDirectory))
                imageDirectory = zPath.Combine(zPath.GetDirectoryName(file), imageDirectory);

            using (PdfReader pdfReader = new PdfReader(file))
            {
                for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                {
                    int imageIndex = 1;
                    foreach (int xrefIndex in EnumImagesIndex(pdfReader.GetPageN(pageNumber)))
                    {

                        string imageFile = zPath.Combine(imageDirectory, $"page-{pageNumber:000}{(imageIndex != 1 ? $"-{imageIndex:00}" : "") }.jpeg");
                        Trace.WriteLine($"save image to \"{imageFile}\"");
                        SavePdfImage(pdfReader.GetPdfObject(xrefIndex), imageFile, ImageFormat.Jpeg);
                    }
                }
            }

            //foreach (PdfObjectInfo image in EnumImages(file))
            //{
            //    string imageFile = zPath.Combine(imageDirectory, $"page-{image.Page:000}{(image.Index != 1 ? $"-{image.Index:00}" : "") }.jpeg");
            //    Trace.WriteLine($"save image to \"{imageFile}\"");
            //    SavePdfImage(image.PdfObject, imageFile, ImageFormat.Jpeg);
            //}
        }

        //public static void Test_ExtractImages_v3(string file, string imageDirectory)
        //{
        //    Trace.WriteLine($"extract images from pdf \"{file}\" to \"{imageDirectory}\"");
        //    if (!zPath.IsPathRooted(imageDirectory))
        //        imageDirectory = zPath.Combine(zPath.GetDirectoryName(file), imageDirectory);

        //    using (PdfReader pdfReader = new PdfReader(file))
        //    {
        //        for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
        //        {
        //            int imageIndex = 1;
        //            foreach (int xrefIndex in EnumImagesIndex(pdfReader.GetPageN(pageNumber)))
        //            {

        //                string imageFile = zPath.Combine(imageDirectory, $"page-{pageNumber:000}{(imageIndex != 1 ? $"-{imageIndex:00}" : "") }.jpeg");
        //                Trace.WriteLine($"save image to \"{imageFile}\"");
        //                //SavePdfImage(pdfReader.GetPdfObject(xrefIndex), imageFile, ImageFormat.Jpeg);
        //                Image image = GetImage(pdfReader.GetPdfObject(xrefIndex));
        //                if (image == null)
        //                    throw new PBException($"can't read pdf image {xrefIndex} from \"{file}\"");
        //                image.Save(imageFile, ImageFormat.Jpeg);
        //            }
        //        }
        //    }
        //}

        //public static Image GetImage(PdfObject pdfObj)
        //{
        //    PdfStream pdfStrem = (PdfStream)pdfObj;
        //    byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
        //    if ((bytes != null))
        //    {
        //        using (MemoryStream memStream = new MemoryStream(bytes))
        //        {
        //            memStream.Position = 0;
        //            //return Image.FromStream(memStream);
        //            return CloneImage(Image.FromStream(memStream));
        //        }
        //    }
        //    return null;
        //}

        //public static Bitmap CloneImage(Image image)
        //{
        //    Bitmap bitmap = new Bitmap(image.Width, image.Height);
        //    using (Graphics graphics = Graphics.FromImage(bitmap))
        //    {
        //        graphics.DrawImage(image, 0, 0);
        //    }
        //    return bitmap;
        //}

        public static IEnumerable<int> EnumImagesIndex(PdfDictionary page)
        {
            PdfDictionary pageResources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
            PdfDictionary pageXObject = (PdfDictionary)PdfReader.GetPdfObject(pageResources.Get(PdfName.XOBJECT));
            if (pageXObject != null)
            {
                //int imageIndex = 1;
                foreach (PdfName name in pageXObject.Keys)
                {
                    PdfObject obj = pageXObject.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                        PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                        if (PdfName.IMAGE.Equals(type))
                        {
                            //int xrefIndex = ((PRIndirectReference)obj).Number;
                            //yield return new PdfObjectInfo { PdfObject = pdfReader.GetPdfObject(xrefIndex), Page = pageNumber, Index = imageIndex };
                            yield return ((PRIndirectReference)obj).Number;
                        }

                    }
                }
            }
        }

        //public static IEnumerable<PdfObjectInfo> EnumImages(string file)
        //{
        //    using (PdfReader pdfReader = new PdfReader(file))
        //    {
        //        for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
        //        {
        //            PdfDictionary page = pdfReader.GetPageN(pageNumber);
        //            PdfDictionary pageResources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
        //            PdfDictionary pageXObject = (PdfDictionary)PdfReader.GetPdfObject(pageResources.Get(PdfName.XOBJECT));
        //            if (pageXObject != null)
        //            {
        //                int imageIndex = 1;
        //                foreach (PdfName name in pageXObject.Keys)
        //                {
        //                    PdfObject obj = pageXObject.Get(name);
        //                    if (obj.IsIndirect())
        //                    {
        //                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
        //                        PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
        //                        if (PdfName.IMAGE.Equals(type))
        //                        {
        //                            int xrefIndex = ((PRIndirectReference)obj).Number;
        //                            yield return new PdfObjectInfo { PdfObject = pdfReader.GetPdfObject(xrefIndex), Page = pageNumber, Index = imageIndex };
        //                        }

        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public static void SavePdfImage(PdfObject pdfObj, string file, ImageFormat imageFormat)
        {
            string filter = null;
            int width = 0;
            int height = 0;
            string bpp = null;
            if (pdfObj is PdfDictionary)
            {
                PdfDictionary objDic = (PdfDictionary)pdfObj;
                filter = objDic.Get(PdfName.FILTER).ToString();

                width = int.Parse(objDic.Get(PdfName.WIDTH).ToString());
                height = int.Parse(objDic.Get(PdfName.HEIGHT).ToString());
                bpp = objDic.Get(PdfName.BITSPERCOMPONENT).ToString();
                Trace.WriteLine($"image width {width} height {height} filter {filter} bits per component {bpp}");
            }
            PdfStream pdfStrem = (PdfStream)pdfObj;
            //string filter = objDic.Get(PdfName.FILTER).ToString();
            byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
            if (bytes != null)
            {
                if (filter == "/FlateDecode")
                {
                    Trace.WriteLine("FlateDecode()");
                    bytes = PdfReader.FlateDecode(bytes, true);

                    zfile.CreateFileDirectory(file);
                    zFile.WriteAllBytes(file + ".bin", bytes);

                    // PixelFormat.Format24bppRgb
                    // System.Drawing.Imaging.PixelFormat 8 bits  PixelFormat.Format8bppIndexed
                    Bitmap bmp = new Bitmap(width - 1, height, PixelFormat.Format8bppIndexed);
                    // PixelFormat.Format24bppRgb  PixelFormat.Format8bppIndexed
                    BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width - 1, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                    //Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length);
                    Marshal.Copy(bytes, 0, bmpData.Scan0, (width - 1) * height);
                    bmp.UnlockBits(bmpData);
                    //bmp.Save(zPath.Combine(imageDirectory, $"image-{index++:000}.jpeg"), ImageFormat.Jpeg);
                    //bmp.Save(file, imageFormat);
                    bmp.Save(file);
                }
                else
                {
                    zfile.CreateFileDirectory(file);
                    zFile.WriteAllBytes(file + ".bin", bytes);
                    try
                    {
                        using (MemoryStream memStream = new MemoryStream(bytes))
                        {
                            memStream.Position = 0;
                            //using (Image image = Image.FromStream(memStream, true, true))
                            //using (Image image = Image.FromStream(memStream, false, true))
                            //using (Image image = Image.FromStream(memStream, true, false))
                            using (Image image = Image.FromStream(memStream, false, false))
                            {
                                //EncoderParameters parms = new EncoderParameters(1);
                                //parms.Param[0] = new EncoderParameter(Encoder.Compression, 0);
                                //// GetImageEncoder is found below this method
                                //ImageCodecInfo jpegEncoder = GetImageEncoder("JPEG");
                                //image.Save(file, jpegEncoder, parms);
                                //image.Save(file, ImageFormat.Jpeg);
                                zfile.CreateFileDirectory(file);
                                image.Save(file, imageFormat);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                }
            }
        }

        public static void ExtractImagesFromPDF(string file, string imageDirectory)
        {
            // from http://www.vbforums.com/showthread.php?530736-2005-Extract-Images-from-a-PDF-file-using-iTextSharp&s=483b5eccd159f359c23e5a28b9126d9c&p=3455355#post3455355

            // NOTE:  This will only get the first image it finds per page.
            PdfReader pdfReader = new PdfReader(file);
            //RandomAccessFileOrArray raf = new RandomAccessFileOrArray(file);
            try
            {
                bool foundImage = false;

                for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                {
                    PdfDictionary page = pdfReader.GetPageN(pageNumber);
                    PdfDictionary pageResources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
                    PdfDictionary pageXObject = (PdfDictionary)PdfReader.GetPdfObject(pageResources.Get(PdfName.XOBJECT));
                    if (pageXObject != null)
                    {
                        foreach (PdfName name in pageXObject.Keys)
                        {
                            PdfObject obj = pageXObject.Get(name);
                            if (obj.IsIndirect())
                            {
                                PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                                PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                                if (PdfName.IMAGE.Equals(type))
                                {

                                    //int xrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                    int xrefIndex = ((PRIndirectReference)obj).Number;

                                    foundImage = true;
                                    SavePdfImage(pdfReader.GetPdfObject(xrefIndex), zPath.Combine(imageDirectory, $"page-{pageNumber:000}.jpeg"), ImageFormat.Jpeg);
                                    //SavePdfObjectImage(pdfReader.GetPdfObject(XrefIndex), zPath.Combine(imageDirectory, $"page-{pageNumber:000}.png"), ImageFormat.Png);
                                    break;

                                    //PdfObject pdfObj = pdfReader.GetPdfObject(XrefIndex);
                                    //PdfStream pdfStrem = (PdfStream)pdfObj;
                                    //byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);

                                    //if ((bytes != null))
                                    //{
                                    //    using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(bytes))
                                    //    {
                                    //        memStream.Position = 0;
                                    //        System.Drawing.Image img = System.Drawing.Image.FromStream(memStream);
                                    //        // must save the file while stream is open.
                                    //        if (!zDirectory.Exists(imageDirectory))
                                    //            zDirectory.CreateDirectory(imageDirectory);

                                    //        string path = zPath.Combine(imageDirectory, String.Format(@"{0}.jpg", pageNumber));
                                    //        System.Drawing.Imaging.EncoderParameters parms = new System.Drawing.Imaging.EncoderParameters(1);
                                    //        parms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 0);
                                    //        // GetImageEncoder is found below this method
                                    //        System.Drawing.Imaging.ImageCodecInfo jpegEncoder = GetImageEncoder("JPEG");
                                    //        img.Save(path, jpegEncoder, parms);
                                    //        break;

                                    //    }
                                    //}
                                }
                            }
                        }
                        if (foundImage)
                            break;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                pdfReader.Close();
            }
        }

        public static ImageCodecInfo GetImageEncoder(string imageType)
        {
            imageType = imageType.ToUpperInvariant();

            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
            {
                if (info.FormatDescription == imageType)
                {
                    return info;
                }
            }

            return null;
        }
    }
}
