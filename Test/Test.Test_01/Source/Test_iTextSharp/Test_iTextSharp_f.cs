using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Test_iTextSharp;

// CMapToUnicode.AddChar()
// CMapParserEx.ParseCid()
// CMapAwareDocumentFont.InitFont()
// PdfReader.GetStreamBytes()
// PdfReader.GetStreamBytesRaw()

namespace Test.Test_iTextSharp
{
    //class toto
    //{
    //    public string a;
    //    public int i;
    //}

    static partial class w
    {
        public static ITrace _tr = Trace.CurrentTrace;
        public static RunSource _wr = RunSource.CurrentRunSource;

        public static bool log = false;
        public static bool logStream = false;
        public static bool logSource = false;

        public static void Init()
        {
            _wr.InitConfig("Test");
        }

        public static void Test_01()
        {
            string s = null;
            _tr.WriteLine("null : \"{0}\"", s);
            s = "";
            _tr.WriteLine("empty : \"{0}\"", s);
        }

        public static void Test_02(string s = "toto", int a = 10)
        {
            _tr.WriteLine("s : \"{0}\"", s);
            _tr.WriteLine("a : {0}", a);
        }

        //public static void Test_03()
        //{
        //    Graphic g = new Graphic();
        //    Bitmap b = new Bitmap(10, 10);
        //    b.SetPixel(5, 5, Color.Blue);
        //}

        public static void Test_04()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("k-eeee", "v-eeee");
            dic.Add("k-dddd", "v-dddd");
            dic.Add("k-cccc", "v-cccc");
            dic.Add("k-bbbb", "v-bbbb");
            dic.Add("k-aaaa", "v-aaaa");

            _tr.WriteLine();
            _tr.WriteLine("value by index");
            string s;
            s = "k-aaaa"; _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", s, dic[s]);
            s = "k-bbbb"; _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", s, dic[s]);
            s = "k-cccc"; _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", s, dic[s]);
            s = "k-dddd"; _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", s, dic[s]);
            s = "k-eeee"; _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", s, dic[s]);

            _tr.WriteLine();
            _tr.WriteLine("value with foreach");
            foreach (KeyValuePair<string, string> v in dic)
            {
                _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", v.Key, v.Value);
            }
            foreach(var i in dic.Keys)
            {
                _tr.WriteLine(i);
            }
            foreach (var i in dic.Values)
            {
                _tr.WriteLine(i);
            }
        }

        public static void Test_05()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(4, "v-dddd");
            dic.Add(5, "v-eeee");
            dic.Add(2, "v-bbbb");
            dic.Add(3, "v-cccc");
            dic.Add(1, "v-aaaa");

            _tr.WriteLine();
            _tr.WriteLine("value by index");
            int i;
            i = 1; _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", i, dic[i]);
            i = 2; _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", i, dic[i]);
            i = 3; _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", i, dic[i]);
            i = 4; _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", i, dic[i]);
            i = 5; _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", i, dic[i]);

            _tr.WriteLine();
            _tr.WriteLine("value with foreach");
            foreach (KeyValuePair<int, string> v in dic)
            {
                _tr.WriteLine("dic[\"{0}\"] = \"{1}\"", v.Key, v.Value);
            }
            foreach (var k in dic.Keys)
            {
                _tr.WriteLine(k.ToString());
            }
            foreach (var v in dic.Values)
            {
                _tr.WriteLine(v.ToString());
            }
        }

        public static void Test_06()
        {
            string file = @"c:\pib\dev_data\exe\pdf\test.txt";
            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            //BinaryWriter bw = new BinaryWriter(fs, Encoding.Default);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                string s = "totototo\n";
                bw.Write(s.ToCharArray());
                s = "tata\n";
                bw.Write(s.ToCharArray());
            }
            finally
            {
                fs.Close();
            }
        }

        public static void Test_07()
        {
            string file = @"c:\pib\dev_data\exe\pdf\test.txt";
            Writer fw = new Writer(file, FileMode.Create);
            try
            {
                fw.Write("toto\n");
                fw.Write("tata\n");
            }
            finally
            {
                fw.Close();
            }
        }

        public static void Test_Regex_01()
        {
            //Test_Regex("^/([a-z]+) ([0-9]+) ?([0-9])? ?([a-z])?$", RegexOptions.Compiled | RegexOptions.IgnoreCase, "/Size 5274");
            //Test_Regex("^/([a-z]+) ([0-9]+) ?([0-9])? ?([a-z])?$", RegexOptions.Compiled | RegexOptions.IgnoreCase, "/Root 5273 0 R");
            //000000B0 C4 00 17 00 00 03 01 00 00 00 00 00 00 00 00 00
            //private Regex grgBytes = new Regex("^[0-9a-f]+ +$", RegexOptions.Compiled | RegexOptions.RightToLeft);
            Test_Regex("^[0-9a-f]+(?: +([0-9a-f]+))+$", RegexOptions.Compiled | RegexOptions.IgnoreCase, "000000B0 C4 00 17 00 00 03 01 00 00 00 00 00 00 00 00 00");
        }

        public static void Test_Regex(string pattern, RegexOptions options, string input)
        {
            Regex rg = new Regex(pattern, options);
            Match match = rg.Match(input);
            //wr.View(match);
            _tr.WriteLine("regex : \"{0}\"", pattern);
            _tr.WriteLine("input : \"{0}\"", input);
            _tr.WriteLine("match : {0}", match.Success);
            int i = 0;
            foreach (Group group in match.Groups)
            {
                string s = group.Value;
                if (s == null)
                    s = "--null--";
                else if (s == "")
                    s = "--empty--";
                _tr.WriteLine("group {0} : {1}", i, s);
                int j = 0;
                foreach (Capture capture in group.Captures)
                {
                    string s2 = capture.Value;
                    if (s2 == null)
                        s2 = "--null--";
                    else if (s2 == "")
                        s2 = "--empty--";
                    _tr.WriteLine("group {0} capture {1} : {2}", i, j, s2);
                    j++;
                }
                i++;
            }
        }

        public static void Test_GetPdfText_01(String file)
        {
            _tr.WriteLine("read pdf file    \"{0}\"", file);
            PB_Pdf.PdfReader pr = new PB_Pdf.PdfReader(file);
            try
            {
                IPdfObject obj = ReadPdfObject(pr, 1138, "Object");
            }
            finally
            {
                pr.Close();
            }
        }

        public static void Test_PdfExportDeflatedStream_02(String pdfFile, int objectId, string streamFile)
        {
            Writer w = null;
            _tr.WriteLine("export stream object {0} pdf file \"{1}\" to \"{2}\"", objectId, pdfFile, streamFile);
            PB_Pdf.PdfReader pr = new PB_Pdf.PdfReader(pdfFile);
            try
            {
                IPdfObject obj = ReadPdfObject(pr, objectId, "Object");
                if (obj.deflatedStream == null)
                {
                    _tr.WriteLine("no stream");
                    return;
                }
                _tr.WriteLine("read data");
                //IPdfInstruction[] instructions = PdfDataReader.PdfReadAll(obj.deflatedStream);
                PdfDataReader pdr = new PdfDataReader(obj.deflatedStream);
                //pdr.Trace += new TraceDelegate(_tr.WriteLine);
                IPdfInstruction[] instructions = pdr.ReadAll();
                _tr.WriteLine("{0} instructions", instructions.Length);
                //var q = from instruction in instructions select instruction.opeString;
                //foreach (string opeString in q.Distinct().OrderBy(opeString => opeString))
                //{
                //    _tr.WriteLine(opeString);
                //}
                w = new Writer(streamFile, FileMode.Create);
                foreach (IPdfInstruction instruction in instructions)
                {
                    //_tr.WriteLine(instruction);
                    //w.WriteLine(instruction.ToString());
                    instruction.Export(w);
                }
            }
            finally
            {
                if (w != null) w.Close();
                pr.Close();
            }
        }

        public static void Test_PdfExportDeflatedStream_01(string pdfFile, int idObject, string streamFile)
        {
            _tr.WriteLine("export stream object {0} of pdf file \"{1}\" to \"{2}\"", idObject, pdfFile, streamFile);
            PB_Pdf.PdfReader pr = null;
            Writer w = null;
            try
            {
                pr = new PB_Pdf.PdfReader(pdfFile);
                IPdfObject obj = ReadPdfObject(pr, idObject, "Object");
                w = new Writer(streamFile, FileMode.Create);
                if (obj.deflatedStream != null)
                    w.Write(obj.deflatedStream);
            }
            finally
            {
                if (pr != null) pr.Close();
                if (w != null) w.Close();
            }
        }

        public static void Test_PdfUpdateStream_01(string pdfFile, int objectId, string streamFile)
        {
            _tr.WriteLine("update stream for object {0} with \"{1}\" of pdf file \"{2}\"", objectId, streamFile, pdfFile);
            PB_Pdf.PdfWriter pw = null;
            Reader r = null;
            try
            {
                pw = new PB_Pdf.PdfWriter(pdfFile, updatePdf: true);
                IPdfObject obj = pw.reader.ReadObject(objectId);
                r = new Reader(streamFile);
                obj.stream = r.ReadBytes((int)r.Length);
                PdfNValue length = obj.value["Length"];
                if (length == null || !length.value.isInt()) throw new PBException("error wrong /Length of object {0}", obj.id);
                length.value.valueInt = obj.stream.Length;
                pw.UpdateObject(obj);
            }
            finally
            {
                if (r != null) r.Close();
                if (pw != null) pw.Close();
            }
        }

        public static void Test_PdfImportStream_01(string inputPdfFile, string outputPdfFile, string streamFile)
        {
            _tr.WriteLine("import stream for object TPL1 of pdf file \"{0}\" from \"{1}\" and save pdf to \"{2}\"", inputPdfFile, streamFile, outputPdfFile);
            PB_Pdf.PdfReader pr = null;
            Reader r = null;
            PB_Pdf.PdfWriter pw = null;
            try
            {
                pr = new PB_Pdf.PdfReader(inputPdfFile);
                //IPdfObject obj = ReadPdfObject(pr, idObject, "Object");
                r = new Reader(streamFile);
                //obj.stream = r.ReadBytes((int)r.Length);
                //PdfNValue length = obj.value["Length"];
                //if (length == null || !length.value.isInt()) throw new PBException("error wrong /Length of object {0}", idObject);
                //length.value.valueInt = obj.stream.Length;

                pw = new PB_Pdf.PdfWriter(outputPdfFile, FileMode.Create);
                pw.reader = pr;
                IPdfObject info = ReadPdfObject(pr, pr.Trailer["Info"].value.valueObjectId, "Info");
                pw.WriteObject(info, "Info");
                IPdfObject root = ReadPdfObject(pr, pr.Trailer["Root"].value.valueObjectId, "Root");
                pw.WriteObject(root, "Root");

                IPdfObject pages = ReadPdfObject(pr, root.value["Pages"].value.valueObjectId, "Pages");
                pw.WriteObject(pages);

                IPdfObject page1 = ReadPdfObject(pr, pages.value["Kids"].value[0].valueObjectId, "Page 1");
                pw.WriteObject(page1);

                IPdfObject page1Content = ReadPdfObject(pr, page1.value["Contents"].value.valueObjectId, "Contents page 1");
                pw.WriteObject(page1Content);

                IPdfObject page1Ressource = ReadPdfObject(pr, page1.value["Resources"].value.valueObjectId, "Resources page 1");
                pw.WriteObject(page1Ressource);

                IPdfObject page1Ressource_01 = ReadPdfObject(pr, page1Ressource.value["XObject"].value["TPL1"].value.valueObjectId, "Resources page 1 /TPL1");

                // import stream from file
                page1Ressource_01.stream = r.ReadBytes((int)r.Length);
                PdfNValue length = page1Ressource_01.value["Length"];
                if (length == null || !length.value.isInt()) throw new PBException("error wrong /Length of object {0}", page1Ressource_01.id);
                length.value.valueInt = page1Ressource_01.stream.Length;

                pw.WriteObjectWithChilds(page1Ressource_01);
            }
            finally
            {
                if (pr != null) pr.Close();
                if (r != null) r.Close();
                if (pw != null) pw.Close();
            }
        }

        //public static void Test_GetPdfText_01()
        //{
        //    string file = @"c:\pib\media\print\Le monde\_quotidien\Le monde - 2012-12\Le monde - 2012-12-06 - no 21113.pdf";
        //    _tr.WriteLine("read pdf file \"{0}\"", file);
        //    string s = Test_GetPdfText_01(file);
        //    _tr.WriteLine(s);
        //    //PdfTextExtractor.GetTextFromPage();
        //    //ITextExtractionStrategy
        //    // iTextSharp.text.pdf.parser.LocationTextExtractionStrategy
        //}

        public static void Test_GetPdfText_07(String file)
        {
            _tr.WriteLine("read pdf file \"{0}\"", file);
            PB_Pdf.PdfReader pr = new PB_Pdf.PdfReader(file);
            pr.KeepObjectSource = true;
            try
            {
                //_tr.WriteLine();
                //_tr.WriteLine("read xref position");
                //pr.ReadXrefPosition();
                //_tr.WriteLine("xref table position : {0}", pr.XrefPosition.zToHex());

                //_tr.WriteLine();
                //_tr.WriteLine("read xref headers");
                //pr.ReadXrefHeaders();
                //_tr.WriteLine("trailer position : {0}", pr.TrailerPosition.zToHex());
                //_tr.WriteLine("objects number   : {0}", pr.ObjectsNumber);
                //foreach (PdfXrefHeader header in pr.XrefHeaders)
                //{
                //    _tr.WriteLine("xref header      : pos {0} id {1} nb {2}", header.filePosition.zToHex(), header.objectId, header.objectNb);
                //}

                //_tr.WriteLine();
                //_tr.WriteLine("read trailer");
                //pr.ReadTrailer();
                _tr.WriteLine("trailer");
                //PrintPdfObject(pr.Trailer);
                PrintPdfTrailer(pr.Trailer);
                //if (pr.Trailer.source != null)
                //{
                //    _tr.WriteLine("Source trailer");
                //    _tr.WriteLine(pr.Trailer.source);
                //}

                //_tr.WriteLine();
                //_tr.WriteLine("read xref");
                //pr.ReadXref();
                //int i = 0;
                //foreach (PdfXref xref in pr.Xref.Values)
                //{
                //    _tr.WriteLine("xref             : id {0,5} pos {1} generation {2}", xref.objectId, xref.filePosition.zToHex(), xref.generationNumber);
                //    if (++i == 10) break;
                //}

                //IPdfObject info = ReadPdfObject(pr, pr.Trailer["Info"].valueObjectId, "Info");
                IPdfObject info = ReadPdfObject(pr, pr.Trailer["Info"].value.valueObjectId, "Info");
                IPdfObject root = ReadPdfObject(pr, pr.Trailer["Root"].value.valueObjectId, "Root");
                IPdfObject pages = ReadPdfObject(pr, root.value["Pages"].value.valueObjectId, "Pages");
                IPdfObject page1 = ReadPdfObject(pr, pages.value["Kids"].value[0].valueObjectId, "Page 1");
                IPdfObject page1Content = ReadPdfObject(pr, page1.value["Contents"].value.valueObjectId, "Contents page 1");
                IPdfObject page1Ressource = ReadPdfObject(pr, page1.value["Resources"].value.valueObjectId, "Resources page 1");
                //IPdfObject page1Ressource_01 = ReadPdfObject(pr, 59, "Contents page 1 (2)");
                IPdfObject page1Ressource_01 = ReadPdfObject(pr, page1Ressource.value["XObject"].value["TPL1"].value.valueObjectId, "Resources page 1 /TPL1");
                IPdfObject page1Ressource_ProcSet = ReadPdfObject(pr, page1Ressource_01.value["Resources"].value["ProcSet"].value.valueObjectId, "Resources page 1 /TPL1 ProcSet");

                IPdfObject page1Ressource_Font_01 = ReadPdfObject(pr, page1Ressource_01.value["Resources"].value["Font"].value["F23"].value.valueObjectId, "Resources page 1 /TPL1 Font F23");
                IPdfObject page1Ressource_Font_01_Widths = ReadPdfObject(pr, page1Ressource_Font_01.value["Widths"].value.valueObjectId, "Resources page 1 /TPL1 Font F23 Widths");
                IPdfObject page1Ressource_Font_01_Encoding = ReadPdfObject(pr, page1Ressource_Font_01.value["Encoding"].value.valueObjectId, "Resources page 1 /TPL1 Font F23 Encoding");
                IPdfObject page1Ressource_Font_01_FontDescriptor = ReadPdfObject(pr, page1Ressource_Font_01.value["FontDescriptor"].value.valueObjectId, "Resources page 1 /TPL1 Font F23 FontDescriptor");
                IPdfObject page1Ressource_Font_01_FontDescriptor_FontFile3 = ReadPdfObject(pr, page1Ressource_Font_01_FontDescriptor.value["FontFile3"].value.valueObjectId, "Resources page 1 /TPL1 Font F23 FontDescriptor FontFile3");
                IPdfObject page1Ressource_Font_01_ToUnicode = ReadPdfObject(pr, page1Ressource_Font_01.value["Widths"].value.valueObjectId, "Resources page 1 /TPL1 Font F23 ToUnicode");

                IPdfObject page1Ressource_Img_01 = ReadPdfObject(pr, page1Ressource_01.value["Resources"].value["XObject"].value["img8"].value.valueObjectId, "Resources page 1 /TPL1 XObject img8");
                IPdfObject page1Ressource_Img_01_Length = ReadPdfObject(pr, page1Ressource_Img_01.value["Length"].value.valueObjectId, "Resources page 1 /TPL1 XObject img8 Length");

                IPdfObject page1Ressource_OPMON = ReadPdfObject(pr, page1Ressource_01.value["Resources"].value["ExtGState"].value["OPMON"].value.valueObjectId, "Resources page 1 /TPL1 ExtGState OPMON");
                //_tr.WriteLine("stream as string :");
                //_tr.WriteLine(page1Ressource_01.deflatedStream.zzToString());
                //_tr.WriteLine();
                //IPdfObject obj = ReadPdfObject(pr, 1043, "Object 1043");
            }
            finally
            {
                pr.Close();
            }
        }

        public static void Test_GetPdfText_08(String file)
        {
            PB_Pdf.PdfWriter pw = null;
            string file2 = zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + "_new");
            _tr.WriteLine("read pdf file    \"{0}\"", file);
            _tr.WriteLine("save pdf to file \"{0}\"", file2);
            PB_Pdf.PdfReader pr = new PB_Pdf.PdfReader(file);
            pr.Trace += new TraceDelegate(_tr.WriteLine);
            try
            {
                pw = new PB_Pdf.PdfWriter(file2, FileMode.Create);
                pw.reader = pr;

                IPdfObject info = ReadPdfObject(pr, pr.Trailer["Info"].value.valueObjectId, "Info");
                pw.WriteObject(info, "Info");

                IPdfObject root = ReadPdfObject(pr, pr.Trailer["Root"].value.valueObjectId, "Root");
                pw.WriteObject(root, "Root");

                IPdfObject pages = ReadPdfObject(pr, root.value["Pages"].value.valueObjectId, "Pages");
                pages.value["Kids"].value.arrayValues = new IPdfValue[] { pages.value["Kids"].value.arrayValues[0] };
                pages.value["Count"].value.valueInt = 1;
                pw.WriteObject(pages);

                IPdfObject page1 = ReadPdfObject(pr, pages.value["Kids"].value[0].valueObjectId, "Page 1");
                pw.WriteObject(page1);

                IPdfObject page1Content = ReadPdfObject(pr, page1.value["Contents"].value.valueObjectId, "Contents page 1");
                page1Content.value.objectValues.Remove("Filter");
                page1Content.stream = page1Content.deflatedStream;
                page1Content.value.objectValues["Length"].value.valueInt = page1Content.stream.Length;
                pw.WriteObject(page1Content);

                IPdfObject page1Ressource = ReadPdfObject(pr, page1.value["Resources"].value.valueObjectId, "Resources page 1");
                Dictionary<string, PdfNValue>  objectValues = new Dictionary<string, PdfNValue>();
                objectValues["TPL1"] = page1Ressource.value["XObject"].value["TPL1"];
                page1Ressource.value["XObject"].value.objectValues = objectValues;
                pw.WriteObject(page1Ressource);

                IPdfObject page1Ressource_01 = ReadPdfObject(pr, page1Ressource.value["XObject"].value["TPL1"].value.valueObjectId, "Resources page 1 /TPL1");
                page1Ressource_01.value.objectValues.Remove("Filter");
                page1Ressource_01.stream = page1Ressource_01.deflatedStream;
                page1Ressource_01.value.objectValues["Length"].value.valueInt = page1Ressource_01.stream.Length;
                pw.WriteObjectWithChilds(page1Ressource_01);

                //IPdfObject page1Ressource_ProcSet = ReadPdfObject(pr, page1Ressource_01.value["Resources"].value["ProcSet"].value.valueObjectId, "Resources page 1 /TPL1 ProcSet");

                //IPdfObject page1Ressource_Font_01 = ReadPdfObject(pr, page1Ressource_01.value["Resources"].value["Font"].value["F23"].value.valueObjectId, "Resources page 1 /TPL1 Font F23");
                //IPdfObject page1Ressource_Font_01_Widths = ReadPdfObject(pr, page1Ressource_Font_01.value["Widths"].value.valueObjectId, "Resources page 1 /TPL1 Font F23 Widths");
                //IPdfObject page1Ressource_Font_01_Encoding = ReadPdfObject(pr, page1Ressource_Font_01.value["Encoding"].value.valueObjectId, "Resources page 1 /TPL1 Font F23 Encoding");
                //IPdfObject page1Ressource_Font_01_FontDescriptor = ReadPdfObject(pr, page1Ressource_Font_01.value["FontDescriptor"].value.valueObjectId, "Resources page 1 /TPL1 Font F23 FontDescriptor");
                //IPdfObject page1Ressource_Font_01_FontDescriptor_FontFile3 = ReadPdfObject(pr, page1Ressource_Font_01_FontDescriptor.value["FontFile3"].value.valueObjectId, "Resources page 1 /TPL1 Font F23 FontDescriptor FontFile3");
                //IPdfObject page1Ressource_Font_01_ToUnicode = ReadPdfObject(pr, page1Ressource_Font_01.value["Widths"].value.valueObjectId, "Resources page 1 /TPL1 Font F23 ToUnicode");

                //IPdfObject page1Ressource_Img_01 = ReadPdfObject(pr, page1Ressource_01.value["Resources"].value["XObject"].value["img8"].value.valueObjectId, "Resources page 1 /TPL1 XObject img8");
                //IPdfObject page1Ressource_Img_01_Length = ReadPdfObject(pr, page1Ressource_Img_01.value["Length"].value.valueObjectId, "Resources page 1 /TPL1 XObject img8 Length");

                //IPdfObject page1Ressource_OPMON = ReadPdfObject(pr, page1Ressource_01.value["Resources"].value["ExtGState"].value["OPMON"].value.valueObjectId, "Resources page 1 /TPL1 ExtGState OPMON");

            }
            finally
            {
                pr.Close();
                if (pw != null) pw.Close();
            }
        }

        public static IPdfObject ReadPdfObject(PB_Pdf.PdfReader pr, int id, string name)
        {
            _tr.WriteLine();
            //int id = ((PdfValueObject)(pr.Trailer.values["Root"].value)).id;
            _tr.WriteLine("read {0} object {1}", name, id);
            IPdfObject o = pr.ReadObject(id);
            _tr.WriteLine("{0} object {1}", name, id);
            PrintPdfObject(o);
            if (o.source != null && logSource)
            {
                _tr.WriteLine("Source {0} object {1}", name, id);
                _tr.WriteLine(o.source);
            }
            return o;
        }

        public static void PrintPdfTrailer(PdfValueObject trailer)
        {
            _tr.WriteLine("trailer");
            foreach (PdfNValue value in trailer.objectValues.Values)
            {
                _tr.WriteLine("  {0}", value);
            }
        }

        public static void PrintPdfObject(IPdfObject o)
        {
            _tr.WriteLine("object id {0} generation {1}", o.id, o.generationNumber);
            if (o.value.isObject())
            {
                foreach (PdfNValue value in o.value.objectValues.Values)
                {
                    _tr.WriteLine("  {0}", value);
                }
            }
            else if (o.value.isArray())
            {
                int i = 0;
                foreach (IPdfValue value in o.value.arrayValues)
                {
                    _tr.WriteLine("  [{0}] = {1}", i++, value);
                }
            }
            else
                _tr.WriteLine("  {0}", o.value);
            if (o.stream != null)
            {
                _tr.WriteLine("  stream {0} bytes, deflated stream {1}", o.stream.Length, o.deflatedStream.Length);
            }
            if (o.stream != null && logStream)
            {
                ////PrintStream(o.stream);
                //byte[] stream = o.deflatedStream;
                ////_tr.WriteLine("deflated stream {0} bytes", stream.Length);
                ////PrintStream(stream);
                //StringBuilder sb = new StringBuilder();
                //bool cr = false;
                //foreach (byte b in stream)
                //{
                //    if (b == 10 && !cr)
                //        sb.Append('\r');
                //    if (b == 13)
                //        cr = true;
                //    else
                //        cr = false;
                //    sb.Append((char)b);
                //}
                _tr.WriteLine("stream as string :");
                //_tr.WriteLine(sb.ToString());
                _tr.WriteLine(o.deflatedStream.zzToString());
                _tr.WriteLine();
            }
        }

        public static void PrintStream(byte[] stream)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (byte b in stream)
            {
                if (i % 16 == 0)
                {
                    if (sb.Length > 0)
                        _tr.WriteLine(sb.ToString());
                    sb = new StringBuilder();
                    sb.AppendFormat(" {0}", i.zToHex());
                }
                sb.AppendFormat(" {0}", b.zToHex());
                i++;
            }
            if (sb.Length > 0)
                _tr.WriteLine(sb.ToString());
        }

        public static void Test_GetPdfText_06(String file)
        {
            _tr.WriteLine("read pdf file \"{0}\"", file);
            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            //fs.Length
            fs.Position = fs.Length - 25;
            string s = sr.ReadToEnd();
            _tr.WriteLine(s);
            int pos = int.Parse(s.Substring(10, 8));
            int n = (int)fs.Length - pos - 25;
            _tr.WriteLine("xref table position : {0}", pos);
            _tr.WriteLine("xref table + trailer length : {0}", n);
            char[] buffer = new char[n];
            fs.Position = pos;
            s = sr.ReadLine(); // xref
            s = sr.ReadLine(); // object_id number_of _objects
            while (s != "trailer")
            {
            }

            //sr.ReadBlock(buffer, 0, n);
            //_tr.WriteLine("xref table and trailer :");
            //_tr.WriteLine(new string(buffer));
            for (int i = 0; i < 10; i++)
            {
                s = sr.ReadLine();
                _tr.WriteLine("line {0} : \"{1}\"", i + 1, s);
            }

            fs.Close();
        }

        public static void Test_GetPdfText_05(String file)
        {
            string outputFile = zpath.PathSetFileNameWithExtension(file, Path.GetFileNameWithoutExtension(file) + "_blocks.txt");
            Trace.WriteLine("export pdf file \"{0}\" to \"{1}\"", file, outputFile);
            FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.WriteLine("export pdf text blocks of \"{0}\"", file);
            sw.WriteLine();
            //_tr.WriteLine("read pdf file \"{0}\"", file);
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(file);
            // Error	8	'LocationTextExtractionStrategy' is an ambiguous reference between 'iTextSharp.text.pdf.parser.LocationTextExtractionStrategy' and 'Test_iTextSharp.LocationTextExtractionStrategy'	C:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Test\Test_iTextSharp\Test_iTextSharp_f.cs	649	13	Source_01
            iTextSharp.text.pdf.parser.LocationTextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                sw.WriteLine("================ page {0} ================", page);
                //GetTextFromPage(reader, page, strategy);
                Test_iTextSharp.PdfTools.ProcessContentPage(reader, page, strategy);
                PrintTextBlocks(sw, strategy.textBlocks);
                sw.WriteLine();
            }
            //string s = strategy.GetResultantText();
            //_tr.WriteLine("LocationTextExtractionStrategy()");
            reader.Close();
            //List<TextChunk> locationalResult = strategy.locationalResult;
            //string s = GetResultantText(locationalResult);
            //_tr.WriteLine(s);
            //PrintResultantText(locationalResult);
            sw.Close();
        }

        public static void PrintResultantText(List<TextChunk> locationalResult)
        {
            locationalResult.Sort();

            //        False    0.00  9.58  46.92  118.71 -1133   46.92 1133.53 1  118.71 1133.53 1 0 1 0 0 "DENTISTE"
            _tr.WriteLine("s.line  dist  space  d.start d.end  d.perp start             end              orient  text");

            TextChunk lastChunk = null;
            foreach (TextChunk chunk in locationalResult)
            {
                bool sameLine = false;
                float dist = 0;

                if (lastChunk != null)
                {
                    sameLine = chunk.SameLine(lastChunk);
                    dist = chunk.DistanceFromEndOf(lastChunk);
                }
                //chunk.charSpaceWidth float
                //chunk.distParallelStart float
                //chunk.distParallelEnd float
                //chunk.distPerpendicular int
                //chunk.startLocation Vector
                //chunk.endLocation Vector
                //chunk.orientationMagnitude int
                //chunk.orientationVector Vector
                //chunk.text

                _tr.WriteLine("{0,-5} {1,7:0.00} {2,5:0.00} {3,7:0.00} {4,7:0.00} {5,5} {6,7:0.00} {7,7:0.00} {8} {9,7:0.00} {10,7:0.00} {11} {12} {13} {14} {15} \"{16}\"",
                    sameLine, dist,
                    chunk.charSpaceWidth, chunk.distParallelStart, chunk.distParallelEnd, chunk.distPerpendicular,
                    chunk.startLocation[0], chunk.startLocation[1], chunk.startLocation[2],
                    chunk.endLocation[0], chunk.endLocation[1], chunk.endLocation[2],
                    chunk.orientationMagnitude,
                    chunk.orientationVector[0], chunk.orientationVector[1], chunk.orientationVector[2],
                    chunk.text);

                lastChunk = chunk;
            }
        }

        public static void PrintTextBlock(StreamWriter sw, TextBlock textBlock, int depth)
        {
            string indent = "";
            for (int i = 0; i < depth; i++) indent += "  ";
            sw.WriteLine("{0}block", indent);
            sw.WriteLine(indent + "{");
            
            foreach (TextChunk chunk in textBlock.text)
            {
                sw.WriteLine("{0}  \"{1}\"", indent, chunk.text);
            }
            foreach (TextBlock child in textBlock.childs)
            {
                PrintTextBlock(sw, child, depth + 1);
            }
            sw.WriteLine(indent + "}");
        }

        public static void PrintTextBlocks(StreamWriter sw, List<TextBlock> textBlocks)
        {
            foreach (TextBlock textBlock in textBlocks)
            {
                PrintTextBlock(sw, textBlock, 0);
            }
        }

        public static string GetResultantText(List<TextChunk> locationalResult)
        {

            //if (DUMP_STATE) DumpState();

            //locationalResult.Sort();

            StringBuilder sb = new StringBuilder();
            TextChunk lastChunk = null;
            foreach (TextChunk chunk in locationalResult)
            {

                if (lastChunk == null)
                {
                    sb.Append(chunk.text);
                }
                else
                {
                    if (chunk.SameLine(lastChunk))
                    {
                        float dist = chunk.DistanceFromEndOf(lastChunk);

                        if (dist < -chunk.charSpaceWidth)
                            sb.Append(' ');

                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        else if (dist > chunk.charSpaceWidth / 2.0f && !StartsWithSpace(chunk.text) && !EndsWithSpace(lastChunk.text))
                            sb.Append(' ');

                        sb.Append(chunk.text);
                    }
                    else
                    {
                        sb.Append('\n');
                        sb.Append(chunk.text);
                    }
                }
                lastChunk = chunk;
            }

            return sb.ToString();

        }

        public static bool StartsWithSpace(string str)
        {
            if (str.Length == 0) return false;
            return str[0] == ' ';
        }

        public static bool EndsWithSpace(string str)
        {
            if (str.Length == 0) return false;
            return str[str.Length - 1] == ' ';
        }

        public static void Test_GetPdfText_04(string file)
        {
            string outputFile = zpath.PathSetFileNameWithExtension(file, Path.GetFileNameWithoutExtension(file) + "_text.txt");
            _tr.WriteLine("export pdf file \"{0}\" to \"{1}\"", file, outputFile);
            FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.WriteLine("export pdf text of \"{0}\"", file);
            sw.WriteLine();
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(file);
            LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                sw.WriteLine("================ page {0} ================", page);
                //string s = GetTextFromPage(reader, 1, strategy);
                //GetTextFromPage(reader, page, strategy);
                Test_iTextSharp.PdfTools.ProcessContentPage(reader, page, strategy);
                string s = strategy.GetResultantText();
                sw.Write(s);
                sw.WriteLine();
            }
            //_tr.WriteLine("LocationTextExtractionStrategy()");
            //_tr.WriteLine(s);
            reader.Close();
            sw.Close();
        }

        //public static void GetTextFromPage(iTextSharp.text.pdf.PdfReader reader, int pageNumber, ITextExtractionStrategy strategy)
        //{
        //    PdfReaderContentParser parser = new PdfReaderContentParser(reader);
        //    //return parser.ProcessContent(pageNumber, strategy).GetResultantText();

        //    // PdfReaderContentParser.ProcessContent()
        //    PdfDictionary pageDic = reader.GetPageN(pageNumber);
        //    PdfDictionary resourcesDic = pageDic.GetAsDict(PdfName.RESOURCES);

        //    PdfContentStreamProcessor processor = new PdfContentStreamProcessor(strategy);
        //    byte[] bytes = ContentByteUtils.GetContentBytesForPage(reader, pageNumber);
        //    processor.ProcessContent(bytes, resourcesDic);
        //    //return strategy.GetResultantText();
        //}

        public static string GetOperatorInfo(PdfLiteral ope)
        {
            string s = "";
            if (ope.IsArray())
                s = "array";
            if (ope.IsBoolean())
            {
                if (s != "") s += "+";
                s += "bool";
            }
            if (ope.IsDictionary())
            {
                if (s != "") s += "+";
                s += "dictionary";
            }
            if (ope.IsIndirect())
            {
                if (s != "") s += "+";
                s += "indirect";
            }
            if (ope.IsName())
            {
                if (s != "") s += "+";
                s += "name";
            }
            if (ope.IsNull())
            {
                if (s != "") s += "+";
                s += "null";
            }
            if (ope.IsNumber())
            {
                if (s != "") s += "+";
                s += "number";
            }
            if (ope.IsStream())
            {
                if (s != "") s += "+";
                s += "stream";
            }
            if (ope.IsString())
            {
                if (s != "") s += "+";
                s += "string";
            }
            if (s == "") s = "???";
            return s;



            //RegisterContentOperator(DEFAULTOPERATOR, new IgnoreOperatorContentOperator());
            //RegisterContentOperator("q", new PushGraphicsState());
            //RegisterContentOperator("Q", new PopGraphicsState());
            //RegisterContentOperator("cm", new ModifyCurrentTransformationMatrix());
            //RegisterContentOperator("gs", new ProcessGraphicsStateResource());

            //SetTextCharacterSpacing tcOperator = new SetTextCharacterSpacing();
            //RegisterContentOperator("Tc", tcOperator);
            //SetTextWordSpacing twOperator = new SetTextWordSpacing();
            //RegisterContentOperator("Tw", twOperator);
            //RegisterContentOperator("Tz", new SetTextHorizontalScaling());
            //SetTextLeading tlOperator = new SetTextLeading();
            //RegisterContentOperator("TL", tlOperator);
            //RegisterContentOperator("Tf", new SetTextFont());
            //RegisterContentOperator("Tr", new SetTextRenderMode());
            //RegisterContentOperator("Ts", new SetTextRise());

            //RegisterContentOperator("BT", new BeginTextC());
            //RegisterContentOperator("ET", new EndTextC());
            //RegisterContentOperator("BMC", new BeginMarkedContentC());
            //RegisterContentOperator("BDC", new BeginMarkedContentDictionary());
            //RegisterContentOperator("EMC", new EndMarkedContentC());

            //TextMoveStartNextLine tdOperator = new TextMoveStartNextLine();
            //RegisterContentOperator("Td", tdOperator);
            //RegisterContentOperator("TD", new TextMoveStartNextLineWithLeading(tdOperator, tlOperator));
            //RegisterContentOperator("Tm", new TextSetTextMatrix());
            //TextMoveNextLine tstarOperator = new TextMoveNextLine(tdOperator);
            //RegisterContentOperator("T*", tstarOperator);

            //ShowText tjOperator = new ShowText();
            //RegisterContentOperator("Tj", new ShowText());
            //MoveNextLineAndShowText tickOperator = new MoveNextLineAndShowText(tstarOperator, tjOperator);
            //RegisterContentOperator("'", tickOperator);
            //RegisterContentOperator("\"", new MoveNextLineAndShowTextWithSpacing(twOperator, tcOperator, tickOperator));
            //RegisterContentOperator("TJ", new ShowTextArray());

            //RegisterContentOperator("Do", new Do());

        }

        public static void Test_GetPdfText_03(String file)
        {
            _tr.WriteLine("read pdf file \"{0}\"", file);
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(file);
            //byte[] bytes = reader.GetPageContent(1);
            StringWriter output = new StringWriter();
            iTextSharp.text.pdf.parser.PdfContentReaderTool.ListContentStreamForPage(reader, 1, output);
            string s = output.ToString();
            _tr.WriteLine("ListContentStreamForPage()");
            _tr.WriteLine(s);
            reader.Close();
        }

        public static void Test_GetPdfText_02(String file)
        {
            //PdfReader reader = new PdfReader(file);
            //_tr.WriteLine("read pdf file \"{0}\"", file);
            //string s = PdfTextExtractor.GetTextFromPage(reader, 1, new SimpleTextExtractionStrategy());
            //_tr.WriteLine("SimpleTextExtractionStrategy()");
            //_tr.WriteLine(s);
            //_tr.WriteLine();
            //_tr.WriteLine();
            //s = PdfTextExtractor.GetTextFromPage(reader, 1, new LocationTextExtractionStrategy());
            //_tr.WriteLine("LocationTextExtractionStrategy()");
            //_tr.WriteLine(s);
            //reader.Close();
        }

        //public static string Test_GetPdfText_01(String file)
        //{
        //    //// from : How to use PDFTextExtractor on iTextSharp http://stackoverflow.com/questions/4412790/how-to-use-pdftextextractor-on-itextsharp
        //    //PdfReader reader = new PdfReader(file);

        //    //StringWriter output = new StringWriter();

        //    //for (int i = 1; i <= reader.NumberOfPages; i++)
        //    //    //output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, i, new SimpleTextExtractionStrategy()));
        //    //    output.WriteLine(PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy()));

        //    //reader.Close();
        //    //return output.ToString();
        //}
    }

}
