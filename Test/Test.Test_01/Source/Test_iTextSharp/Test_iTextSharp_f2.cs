using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using pb;

namespace Test.Test_iTextSharp
{
    public static class Test_iTextSharp_f2
    {
        public static void Test_ReadPdf_01(string file)
        {
            PdfReader reader = null;
            try
            {
                reader = new PdfReader(file);
                Trace.WriteLine("read pdf                          : \"{0}\"", file);
                Trace.WriteLine("number of pages                   : {0}", reader.NumberOfPages);
                Rectangle mediabox = reader.GetPageSize(1);
                Trace.WriteLine("size of page 1                    : [ {0}, {1}, {2}, {3} ]", mediabox.Left, mediabox.Bottom, mediabox.Right, mediabox.Top);
                Trace.WriteLine("rotation of page 1                : {0}", reader.GetPageRotation(1));
                Trace.WriteLine("page size with rotation of page 1 : {0}", reader.GetPageSizeWithRotation(1));
                Trace.WriteLine("file length                       : {0}", reader.FileLength);
                Trace.WriteLine("is rebuilt ?                      : {0}", reader.IsRebuilt());
                Trace.WriteLine("is encrypted ?                    : {0}", reader.IsEncrypted());
            }
            finally
            {
                if (reader != null)
                reader.Close();
            }
        }

        public static void Test_ControlPdfDirectory_01(string directory)
        {
            foreach (string file in Directory.EnumerateFiles(directory, "*.pdf", SearchOption.AllDirectories))
            {
                Test_ControlPdf_01(file);
            }
        }

        public static void Test_ControlPdf_01(string file)
        {
            Trace.Write("control pdf {0,-130}", "\"" + file + "\"");
            PdfReader reader = null;
            try
            {
                reader = new PdfReader(file);
                Trace.WriteLine("   ok");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("   error       {0}", ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        // String[] args
        //public static void Test_ReadPdf_01(string file) // throws IOException
        //{
        //    PdfReader reader = new PdfReader(file);
        //    //PdfDictionary trailer = reader.getTrailer();
        //    PdfDictionary trailer = reader.Trailer;
        //    showEntries(trailer);
        //    //PdfNumber size = (PdfNumber)trailer.get(PdfName.SIZE);
        //    PdfNumber size = (PdfNumber)trailer.Get(PdfName.SIZE);
        //    showObject(size);
        //    //size = trailer.getAsNumber(PdfName.SIZE);
        //    size = trailer.GetAsNumber(PdfName.SIZE);
        //    showObject(size);
        //     PdfArray ids = trailer.GetAsArray(PdfName.ID);
        //     PdfString id1 = ids.GetAsString(0);
        //     showObject(id1);
        //     PdfString id2 = ids.GetAsString(1);
        //     showObject(id2);
        //     PdfObject obj = trailer.Get(PdfName.INFO);
        //     showObject(obj);
        //     showObject(trailer.GetAsDict(PdfName.INFO));
        //     PdfIndirectReference info = trailer.GetAsIndirectObject(PdfName.INFO);
        //     showObject(info);
        //     //obj = reader.GetPdfObject(info.getNumber());
        //     obj = reader.GetPdfObject(info.Number);
        //     showObject(obj);
        //     obj = PdfReader.GetPdfObject(trailer.Get(PdfName.INFO));
        //     showObject(obj);
        //     reader.Close();
        //}

        //public static void showEntries(PdfDictionary dict)
        //{
        //    for (PdfName key : dict.getKeys()) {
        //        System.out.print(key + ": ");
        //        System.out.println(dict.get(key));
        //    }
        //}

        //public static void showObject(PdfObject obj) {
        //    System.out.println(obj.getClass().getName() + ":");
        //    System.out.println("-> type: " + obj.type());
        //    System.out.println("-> toString: " + obj.toString());
        //}


        //public static void Test_GetPdfText_05(string file)
        //{
        //    string outputFile = zpath.PathSetFileWithExt(file, Path.GetFileNameWithoutExtension(file) + "_blocks.txt");
        //    Trace.WriteLine("export pdf file \"{0}\" to \"{1}\"", file, outputFile);
        //    FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
        //    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
        //    sw.WriteLine("export pdf text blocks of \"{0}\"", file);
        //    sw.WriteLine();
        //    Trace.WriteLine("read pdf file \"{0}\"", file);
        //    PdfReader reader = new PdfReader(file);
        //    LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
        //    for (int page = 1; page <= reader.NumberOfPages; page++)
        //    {
        //        sw.WriteLine("================ page {0} ================", page);
        //        //GetTextFromPage(reader, page, strategy);
        //        ProcessContentPage(reader, page, strategy);
        //        PrintTextBlocks(sw, strategy.textBlocks);
        //        sw.WriteLine();
        //    }
        //    //string s = strategy.GetResultantText();
        //    //Trace.WriteLine("LocationTextExtractionStrategy()");
        //    reader.Close();
        //    //List<TextChunk> locationalResult = strategy.locationalResult;
        //    //string s = GetResultantText(locationalResult);
        //    //Trace.WriteLine(s);
        //    //PrintResultantText(locationalResult);
        //    sw.Close();
        //}

        //public static void ProcessContentPage(PdfReader reader, int page, ITextExtractionStrategy strategy)
        //{
        //    PdfReaderContentParser parser = new PdfReaderContentParser(reader);

        //    PdfDictionary pageDic = reader.GetPageN(page);
        //    PdfDictionary resourcesDic = pageDic.GetAsDict(PdfName.RESOURCES);

        //    PdfContentStreamProcessor processor = new PdfContentStreamProcessor(strategy);
        //    byte[] bytes = ContentByteUtils.GetContentBytesForPage(reader, page);
        //    processor.ProcessContent(bytes, resourcesDic);
        //}

        //public static void PrintTextBlocks(StreamWriter sw, List<TextBlock> textBlocks)
        //{
        //    foreach (TextBlock textBlock in textBlocks)
        //    {
        //        PrintTextBlock(sw, textBlock, 0);
        //    }
        //}
    }
}
