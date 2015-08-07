using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using pb.IO;

namespace ExportPdfContent
{
    class Program
    {
        private static bool _outputDictionary = false;
        private static bool _outputXObject = false;
        private static bool _outputContentStream = false;
        private static bool _outputText = false;
        private static bool _outputTextBlocks1 = false;
        private static bool _outputTextBlocks2 = false;
        private static bool _outputTextBlocks3 = true;
        private static int _outputMaxCol = 140;
        private static string[] _pdfFiles;

        public static void Main(string[] args)
        {
            //Console.WriteLine("Pdf content reader tool");
            Console.WriteLine("extract pdf content");
            //iTextSharp.text.pdf.parser.PdfContentReaderTool.Main(args);
            xpdf(args);
        }

        public static bool GetParams(string[] args)
        {
            if (args.Length == 0)
                _pdfFiles = new string[] { "*.pdf" };
            else
                _pdfFiles = args;
            return true;
        }

        public static void xpdf(string[] args)
        {
            try
            {
                if (!GetParams(args))
                {
                    //Console.WriteLine("Usage:  xpdf <pdf files> [<output file>|stdout] [<page num>]");
                    Console.WriteLine("Usage:  xpdf <pdf files>");
                    return;
                }

                foreach (string pdfFile in _pdfFiles)
                {
                    string dir = zPath.GetDirectoryName(pdfFile);
                    if (dir == "") dir = ".";
                    foreach (string file in zDirectory.GetFiles(dir, zPath.GetFileName(pdfFile)))
                    {
                        xpdfFile(file);
                    }
                }


                //string pdfFile = args[0];
                //Console.WriteLine("read pdf file \"{0}\"", pdfFile);
                ////TextWriter writer = Console.Out;
                //string outputFile = null;
                ////if (args.Length >= 2)
                ////{
                ////    if (!string.Equals(args[1], "stdout", StringComparison.CurrentCultureIgnoreCase))
                ////    {
                ////        Console.WriteLine("writing pdf content to " + args[1]);
                ////        writer = new StreamWriter(args[1]);
                ////    }
                ////}
                //if (args.Length >= 2)
                //    outputFile = args[1];
                //else
                //    outputFile = zPath.Combine(zPath.GetDirectoryName(pdfFile), zPath.GetFileNameWithoutExtension(pdfFile) + ".txt");
                //Console.WriteLine("writing pdf content to \"{0}\"", outputFile);
                //TextWriter writer = null;
                //bool outputToConsole = false;
                //if (string.Equals(outputFile, "stdout", StringComparison.CurrentCultureIgnoreCase))
                //{
                //    writer = Console.Out;
                //    outputToConsole = true;
                //}
                //else
                //    writer = new StreamWriter(outputFile);
                ////int page = 1;
                ////if (args.Length >= 3)
                ////{
                ////    if (!int.TryParse(args[2], out page))
                ////        page = 1;
                ////}
                ////Console.WriteLine("read page {0}", page);
                //PdfReader reader = new PdfReader(pdfFile);
                //for (int page = 1; page <= reader.NumberOfPages; page++)
                //{
                //    Console.Write(".");
                //    ListContentStreamForPage(reader, page, writer);
                //}
                //Console.WriteLine();
                //writer.Flush();
                ////if (args.Length >= 2)
                ////{
                ////    writer.Close();
                ////    Console.WriteLine("Finished writing content to " + args[1]);
                ////}
                //if (!outputToConsole)
                //    writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void xpdfFile(string file)
        {
            try
            {
                string outputFile = zPath.Combine(zPath.GetDirectoryName(file), zPath.GetFileNameWithoutExtension(file) + ".txt");
                Console.WriteLine("extract pdf content from \"{0}\" to \"{1}\"", file, outputFile);
                TextWriter writer = new StreamWriter(outputFile);
                PdfReader reader = new PdfReader(file);
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    Console.Write(".");
                    xpdfPage(reader, page, writer);
                }
                Console.WriteLine();
                writer.Flush();
                writer.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void xpdfPage(PdfReader reader, int pageNum, TextWriter outp)
        {
            outp.WriteLine("==============Page " + pageNum + "====================");
            PdfDictionary pageDictionary = reader.GetPageN(pageNum);
            if (_outputDictionary)
            {
                outp.WriteLine("- - - - - Dictionary - - - - - -");
                //outp.WriteLine(PdfContentReaderTool.GetDictionaryDetail(pageDictionary));
                //string s = PdfContentReaderTool.GetDictionaryDetail(pageDictionary);
                string s = GetDictionaryDetail(pageDictionary);
                outp.WriteLine(s);
            }

            if (_outputXObject)
            {
                outp.WriteLine("- - - - - XObject summary - - - - - -");
                outp.WriteLine(PdfContentReaderTool.GetXObjectDetail(pageDictionary.GetAsDict(PdfName.RESOURCES)));
            }

            if (_outputContentStream)
            {
                outp.WriteLine("- - - - - Content stream - - - - - -");
                RandomAccessFileOrArray f = reader.SafeFile;
                byte[] contentBytes = reader.GetPageContent(pageNum, f);
                f.Close();

                outp.Flush();

                foreach (byte b in contentBytes)
                {
                    outp.Write((char)b);
                }

                outp.Flush();
            }

            Test_iTextSharp.LocationTextExtractionStrategy strategy = new Test_iTextSharp.LocationTextExtractionStrategy();
            //GetTextFromPage(reader, pageNum, strategy);
            Test_iTextSharp.PdfTools.ProcessContentPage(reader, pageNum, strategy);

            if (_outputText)
            {
                outp.WriteLine("- - - - - Text extraction - - - - - -");
                //LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                //String extractedText = PdfTextExtractor.GetTextFromPage(reader, pageNum, new LocationTextExtractionStrategy());
                string extractedText = strategy.GetResultantText();
                if (extractedText.Length != 0)
                {
                    outp.WriteLine(extractedText);
                    outp.WriteLine();
                }
                else
                    outp.WriteLine("No text found on page " + pageNum);
            }

            if (_outputTextBlocks1)
            {
                outp.WriteLine("- - - - - Text blocks extraction 1 - - - - - -");
                //GetTextFromPage(reader, pageNum, strategy);
                //PrintTextBlocks(outp, strategy.textBlocks);
                foreach (Test_iTextSharp.TextBlock textBlock in strategy.textBlocks)
                    PrintTextBlock(outp, textBlock, 0);
                outp.WriteLine();
            }

            if (_outputTextBlocks2)
            {
                outp.WriteLine("- - - - - Text blocks extraction 2 - - - - - -");
                foreach (Test_iTextSharp.TextBlock textBlock in strategy.textBlocks)
                {
                    outp.Write("block  ");
                    //outp.WriteLine(GetTextBlock(textBlock));
                    outp.WriteLine(textBlock.GetText());
                    if (textBlock.childs.Count > 0)
                        outp.WriteLine("   **** warning childs blocks not printed ****");
                }
                outp.WriteLine();
            }

            if (_outputTextBlocks3)
            {
                outp.WriteLine("- - - - - Text blocks extraction 3 - - - - - -");
                foreach (Test_iTextSharp.TextBlock textBlock in strategy.textBlocks)
                {
                    bool first = true;
                    //foreach (string s in GetTextBlockByLines(textBlock, _outputMaxCol))
                    foreach (string s in textBlock.GetTextByLines(_outputMaxCol))
                    {
                        if (first)
                        {
                            outp.Write("block  ");
                            first = false;
                        }
                        else
                            outp.Write("       ");
                        outp.WriteLine(s);
                    }
                    if (textBlock.childs.Count > 0)
                        outp.WriteLine("   **** warning childs blocks not printed ****");
                }
                outp.WriteLine();
            }

            outp.WriteLine();
        }

        //public static void GetTextFromPage(PdfReader reader, int pageNumber, Test_iTextSharp.ITextExtractionStrategy strategy)
        //{
        //    PdfReaderContentParser parser = new PdfReaderContentParser(reader);

        //    PdfDictionary pageDic = reader.GetPageN(pageNumber);
        //    PdfDictionary resourcesDic = pageDic.GetAsDict(PdfName.RESOURCES);

        //    Test_iTextSharp.PdfContentStreamProcessor processor = new Test_iTextSharp.PdfContentStreamProcessor(strategy);
        //    byte[] bytes = ContentByteUtils.GetContentBytesForPage(reader, pageNumber);
        //    processor.ProcessContent(bytes, resourcesDic);
        //}

        //public static string GetTextBlock(Test_iTextSharp.TextBlock textBlock)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    bool first = true;
        //    foreach (Test_iTextSharp.TextChunk chunk in textBlock.text)
        //    {
        //        if (first)
        //            first = false;
        //        else
        //            sb.Append(" ");
        //        sb.Append(chunk.text);
        //    }
        //    // attention les blocks enfants ne sont pas traités
        //    //foreach (Test_iTextSharp.TextBlock child in textBlock.childs)
        //    return sb.ToString();
        //}

        //public static string[] gettextblockbylines(test_itextsharp.textblock textblock, int maxcol)
        //{
        //    list<string> lines = new list<string>();
        //    stringbuilder sb = new stringbuilder();
        //    bool first = true;
        //    foreach (test_itextsharp.textchunk chunk in textblock.text)
        //    {
        //        if (sb.length + 1 + chunk.text.length > maxcol)
        //        {
        //            lines.add(sb.tostring());
        //            sb.clear();
        //            first = true;
        //        }
        //        if (first)
        //            first = false;
        //        else
        //            sb.append(" ");
        //        sb.append(chunk.text);
        //    }
        //    lines.add(sb.tostring());
        //    // attention les blocks enfants ne sont pas traités
        //    //foreach (test_itextsharp.textblock child in textblock.childs)
        //    return lines.toarray();
        //}

        public static void PrintTextBlock(TextWriter tw, Test_iTextSharp.TextBlock textBlock, int depth)
        {
            string indent = "";
            for (int i = 0; i < depth; i++) indent += "  ";
            tw.Write("{0}block {{", indent);
            //tw.WriteLine(indent + "{");
            int col = _outputMaxCol;

            foreach (Test_iTextSharp.TextChunk chunk in textBlock.text)
            {
                //tw.WriteLine("{0}  \"{1}\"", indent, chunk.text);
                if (col + chunk.text.Length + 4 > _outputMaxCol)
                {
                    tw.WriteLine();
                    tw.Write("{0}  ", indent);
                    col = indent.Length + 2;
                }
                else
                {
                    tw.Write(", ");
                    col += 2;
                }
                tw.Write("\"{0}\"", chunk.text);
                col += chunk.text.Length + 2;
            }
            tw.WriteLine();
            foreach (Test_iTextSharp.TextBlock child in textBlock.childs)
            {
                PrintTextBlock(tw, child, depth + 1);
            }
            tw.WriteLine(indent + "}");
        }

        public static String GetDictionaryDetail(PdfDictionary dic)
        {
            return GetDictionaryDetail(dic, 0);
        }

        public static String GetDictionaryDetail(PdfDictionary dic, int depth)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('(');
            IList<PdfName> subDictionaries = new List<PdfName>();
            bool first = true;
            foreach (PdfName key in dic.Keys)
            {
                if (!first)
                {
                    builder.Append(", ");
                    first = false;
                }
                PdfObject val = dic.GetDirectObject(key);
                if (val.IsDictionary())
                    subDictionaries.Add(key);
                builder.Append(key);
                builder.Append('=');
                builder.Append(val);
                //builder.Append(", ");
            }
            //builder.Length = builder.Length - 2;
            builder.Append(')');
            foreach (PdfName pdfSubDictionaryName in subDictionaries)
            {
                builder.Append('\n');
                for (int i = 0; i < depth + 1; i++)
                {
                    builder.Append('\t');
                }
                builder.Append("Subdictionary ");
                builder.Append(pdfSubDictionaryName);
                builder.Append(" = ");
                builder.Append(GetDictionaryDetail(dic.GetAsDict(pdfSubDictionaryName), depth + 1));
            }
            return builder.ToString();
        }
    }
}
