using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Xobject;
using pb.IO;
using System.Text;

// http://developers.itextpdf.com/fr/itext-7
// https://github.com/itext/itext7-dotnet
// http://developers.itextpdf.com/examples/content-extraction-and-redaction/clone-extracting-objects-pdf

namespace pb.Data.Pdf.Test
{
    public static class Test_iText7
    {
        public static void Test_ExtractText(string file, int page)
        {
            // from Why is the text I extract from an English PDF page garbled? http://developers.itextpdf.com/fr/node/3049
            Trace.WriteLine($"extract text from \"{file}\"");
            using (PdfReader pdfReader = new PdfReader(file))
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                //for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                Trace.WriteLine($"page {page} :");
                string text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), new LocationTextExtractionStrategy());
                Trace.WriteLine(text);
            }
        }

        public static void Test_TraceObjects(string file)
        {
            Trace.WriteLine($"objects from pdf \"{file}\"");

            using (PdfReader pdfReader = new PdfReader(file))
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                int objectsNb = pdfDocument.GetNumberOfPdfObjects();
                Trace.WriteLine($"{objectsNb} objects");
                for (int i = 0; i < objectsNb; i++)
                {
                    PdfObject pdfObject = pdfDocument.GetPdfObject(i);
                    //Trace.WriteLine($"{i + 1,3} object : {GetObjectInfo(pdfObject)}");
                    Trace.WriteLine($"{i + 1,3} object : {GetObjectType(pdfObject)}");
                }
            }
        }

        public static void Test_ExtractImages(string file, string directory)
        {
            Trace.WriteLine($"extract images from pdf \"{file}\" to \"{directory}\"");
            if (!zPath.IsPathRooted(directory))
                directory = zPath.Combine(zPath.GetDirectoryName(file), directory);
            using (PdfReader pdfReader = new PdfReader(file))
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                for (int pageNumber = 1; pageNumber <= pdfDocument.GetNumberOfPages(); pageNumber++)
                {
                    PdfPage page = pdfDocument.GetPage(pageNumber);
                    PdfResources pageResources = page.GetResources();
                    PdfDictionary pageObjects = pageResources.GetResource(PdfName.XObject);
                    //Trace.WriteLine($"pageObjects {(pageObjects != null ? "-not-null-" : "-null-")}");
                    if (pageObjects != null)
                    {
                        int imageIndex = 1;
                        foreach (PdfObject pageObject in pageObjects.Values())
                        {
                            if (pageObject.IsIndirect())
                            {
                                PdfObject directObject = pdfDocument.GetPdfObject(pageObject.GetIndirectReference().GetObjNumber());
                                //Trace.WriteLine($"directObject {(directObject != null ? "-not-null-" : "-null-")}");
                                if (directObject is PdfDictionary)
                                {
                                    PdfDictionary dictionary = (PdfDictionary)directObject;
                                    //Trace.WriteLine($"is dictionary : Subtype {dictionary.Get(PdfName.Subtype)} is PdfStream {directObject is PdfStream}");
                                    if (dictionary.Get(PdfName.Subtype).ToString() == "/Image")
                                    {
                                        Trace.WriteLine($"SaveImage page {pageNumber} image {imageIndex}");
                                        SaveImage(directObject, zPath.Combine(directory, $"page-{pageNumber:000}-{imageIndex++:00}"));
                                    }
                                }
                                //else
                                //    Trace.WriteLine($"is not dictionary");
                            }
                        }
                    }
                    //break;

                    //int imageIndex = 1;
                    //foreach (PdfName pdfName in pageResources.GetResourceNames())
                    //{
                    //    // resources.GetResource(pdfName)                        => null
                    //    // resources.GetResourceObject(PdfName.XObject, pdfName) => PdfStream
                    //    // resources.GetProperties(pdfName)                      => null
                    //    // resources.GetImage(pdfName)                           => Error : Object reference not set to an instance of an object

                    //    PdfObject pdfObject = pageResources.GetResourceObject(PdfName.XObject, pdfName);
                    //    if (pdfObject is PdfStream)
                    //    {
                    //        Trace.WriteLine($"page {pageNumber}");
                    //        SaveImage(pdfObject, zPath.Combine(directory, $"page-{pageNumber:000}-{imageIndex++:00}"));
                    //    }
                    //}
                }
            }
        }

        public static void Test_ExtractImage(string file, int index, string outputFile)
        {
            if (!zPath.IsPathRooted(outputFile))
                outputFile = zPath.Combine(zPath.GetDirectoryName(file), outputFile);
            using (PdfReader pdfReader = new PdfReader(file))
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                PdfObject pdfObject = pdfDocument.GetPdfObject(index);
                if (!(pdfObject is PdfDictionary))
                {
                    Trace.WriteLine("pdfObject is not PdfDictionary");
                    return;
                }
                PdfDictionary pdfDictionary = (PdfDictionary)pdfObject;
                if (pdfDictionary.Get(PdfName.Type)?.ToString() != "/XObject" || pdfDictionary.Get(PdfName.Subtype)?.ToString() != "/Image")
                {
                    Trace.WriteLine("pdfObject is not an image");
                    return;
                }
                SaveImage(pdfObject, outputFile);
            }
        }

        public static void SaveImage(PdfObject pdfObject, string file)
        {
            zfile.CreateFileDirectory(file);
            if (!(pdfObject is PdfStream))
                throw new PBException("object is not a PdfStream");
            PdfImageXObject imageObject = new PdfImageXObject((PdfStream)pdfObject);
            //Trace.WriteLine($"width {imageObject.GetWidth()} height {imageObject.GetHeight()} imageType {imageObject.IdentifyImageType()} extension {imageObject.IdentifyImageFileExtension()}");
            zFile.WriteAllBytes(file + "." + imageObject.IdentifyImageFileExtension(), imageObject.GetImageBytes());
        }

        public static string GetObjectType(PdfObject pdfObject)
        {
            if (pdfObject != null)
            {
                if (pdfObject is PdfDictionary)
                {
                    PdfDictionary pdfDictionary = (PdfDictionary)pdfObject;
                    return pdfDictionary.Get(PdfName.Type)?.ToString() + "-" + pdfDictionary.Get(PdfName.Subtype)?.ToString();
                }
                else
                    return "-unknow-";
            }
            else
                return "-null-";
        }

        public static string GetObjectInfo(PdfObject pdfObject)
        {
            StringBuilder sb = new StringBuilder();
            if (pdfObject != null)
            {
                if (pdfObject.IsBoolean())
                    sb.Append(", bool");
                if (pdfObject.IsNumber())
                    sb.Append(", number");
                if (pdfObject.IsString())
                    sb.Append(", string");
                if (pdfObject.IsLiteral())
                    sb.Append(", literal");
                if (pdfObject.IsArray())
                    sb.Append(", array");
                if (pdfObject.IsDictionary())
                    sb.Append(", dictionary");
                if (pdfObject.IsName())
                    sb.Append(", name");
                if (pdfObject.IsStream())
                    sb.Append(", stream");
                if (pdfObject.IsIndirect())
                    sb.Append(", indirect");
                if (pdfObject.IsIndirectReference())
                    sb.Append(", indirect reference");
                if (pdfObject.IsModified())
                    sb.Append(", modified");
                if (pdfObject.IsNull())
                    sb.Append(", null");
                if (sb.Length > 0)
                    sb.Remove(0, 2);
            }
            else
                sb.Append("null");
            return sb.ToString();
        }

        public static void Test_ExtractObjects(string file, string directory)
        {
            Trace.WriteLine($"extract objects from pdf \"{file}\" to \"{directory}\"");
            if (!zPath.IsPathRooted(directory))
                directory = zPath.Combine(zPath.GetDirectoryName(file), directory);
            zdir.CreateDirectory(directory);
            using (PdfReader pdfReader = new PdfReader(file))
            using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
            {
                int objectsNb = pdfDocument.GetNumberOfPdfObjects();
                for (int i = 0; i < objectsNb; i++)
                {
                    PdfObject pdfObject = pdfDocument.GetPdfObject(i);
                    if (pdfObject != null && pdfObject.IsStream())
                    {
                        byte[] bytes = ((PdfStream)pdfObject).GetBytes();
                        zFile.WriteAllBytes(zPath.Combine(directory, $"object-{i + 1:000}.bin"), bytes);
                    }
                }
            }
        }

    }

    public static class iText7Extension
    {
        public static PdfObject zGet(this PdfDictionary pdfDictionary, PdfName name)
        {
            if (pdfDictionary.ContainsKey(name))
                return pdfDictionary.Get(name);
            else
                return null;
        }
    }
}
