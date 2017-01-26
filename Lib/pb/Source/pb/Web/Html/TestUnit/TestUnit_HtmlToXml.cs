using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using pb.Data.Mongo;
using pb.IO;
using System;

namespace pb.Web.Html.TestUnit
{
    public static class TestUnit_HtmlToXml
    {
        private static string __testUnitDirectory = @"c:\pib\drive\google\dev_data\exe\runsource\test_unit";
        private static string __testUnitSubdirectory = @"Web\HtmlToXml\sites";
        private static string __archiveDirectory = "archive";

        //public static void TraceHtmlReader_v4(string file, bool generateCloseTag = false, bool disableLineColumn = false, bool disableScriptTreatment = false,
        //    bool useReadAttributeValue_v2 = true, bool useTranslateChar = true)
        public static void TraceHtmlReader_v4(string file, HtmlReaderOptions options = HtmlReaderOptions.Default)
        {
            string traceFile = GetFile(file, ".trace.v4.txt");
            Trace.WriteLine("HtmlReader v4");
            Trace.WriteLine("  file  \"{0}\"", file);
            Trace.WriteLine("  trace \"{0}\"", traceFile);
            //HtmlReader_v4.ReadFile(file, generateCloseTag: generateCloseTag, disableLineColumn: disableLineColumn, disableScriptTreatment: disableScriptTreatment,
            //    useReadAttributeValue_v2: useReadAttributeValue_v2, useTranslateChar: useTranslateChar)
            //    .zSave(traceFile);
            HtmlReader_v4.ReadFile(file, options: options).zSave(traceFile);
        }

        public static void TraceHtmlReader_v3(string file, bool generateCloseTag = false, bool disableLineColumn = false)
        {
            string traceFile = GetFile(file, ".trace.v3.txt");
            Trace.WriteLine("HtmlReader v3");
            Trace.WriteLine("  file  \"{0}\"", file);
            Trace.WriteLine("  trace \"{0}\"", traceFile);
            //HtmlReader_v3.GenerateCloseTag = false;
            //HtmlReader_v3.DisableLineColumn = true;
            HtmlReader_v3.ReadFile(file, generateCloseTag: generateCloseTag, disableLineColumn: disableLineColumn).zSave(traceFile);
        }

        public static void TraceHtmlReader_v3_2(string file, bool generateCloseTag = false, bool disableLineColumn = false)
        {
            string traceFile = GetFile(file, ".trace.v3.2.txt");
            Trace.WriteLine("HtmlReader v3.2");
            Trace.WriteLine("  file  \"{0}\"", file);
            Trace.WriteLine("  trace \"{0}\"", traceFile);
            //HtmlReader_v3.GenerateCloseTag = false;
            //HtmlReader_v3.DisableLineColumn = true;
            HtmlReader_v3.ReadFile_v2(file, generateCloseTag: generateCloseTag, disableLineColumn: disableLineColumn).zSave(traceFile);
        }

        public static void TraceHtmlReader_v2(string file, bool disableScriptTreatment = false, bool useReadAttributeValue_v2 = false, bool textReplaceControl = false)
        {
            string traceFile = GetFile(file, ".trace.v2.txt");
            Trace.WriteLine("HtmlReader v2");
            Trace.WriteLine("  file  \"{0}\"", file);
            Trace.WriteLine("  trace \"{0}\"", traceFile);
            TraceHtmlReader_v2(file, traceFile, disableScriptTreatment: disableScriptTreatment, useReadAttributeValue_v2: useReadAttributeValue_v2, textReplaceControl: textReplaceControl);
        }

        public static void TraceHtmlReader_v2(string file, string traceFile, bool disableScriptTreatment = false, bool useReadAttributeValue_v2 = false, bool textReplaceControl = false)
        {
            try
            {
                __srTraceHtmlReader = zFile.CreateText(traceFile);
                __traceJsonSettings = new JsonWriterSettings();
                __traceJsonSettings.Indent = true;

                using (StreamReader sr = zfile.OpenText(file))
                {
                    HtmlReader_v2 htmlReader = new HtmlReader_v2(sr);
                    htmlReader.Trace = TraceHtmlReader;
                    htmlReader.DisableScriptTreatment = disableScriptTreatment;
                    htmlReader.UseReadAttributeValue_v2 = useReadAttributeValue_v2;
                    htmlReader.TextReplaceControl = textReplaceControl;
                    htmlReader.ReadAll();
                }
            }
            finally
            {
                if (__srTraceHtmlReader != null)
                {
                    __srTraceHtmlReader.Close();
                    __srTraceHtmlReader = null;
                }
                __traceJsonSettings = null;
            }

        }

        public static void Test_HtmlReader_v2_v4(string directory = null, string traceDirectory = null, string pattern = "*.html")
        {
            // compare HtmlReader_v2 and HtmlReader_v4
            // for each html file :
            //   - generate HtmlReader_v2 trace file .html.trace.v2.txt
            //   - generate HtmlReader_v4 trace file .html.trace.v4.txt
            //   - compare .html.trace.v2.txt with .html.trace.v4.txt

            bool deleteOkTrace = true;

            int nb = 0;
            int okTraceNb = 0;
            int notOkTraceNb = 0;
            foreach (string file in GetHtmlFiles(directory, pattern))
            {
                nb++;
                Trace.WriteLine("file \"{0}\"", file);

                string traceFile_v2 = GetFile(file, ".trace.v2.txt", traceDirectory);
                TraceHtmlReader_v2(file, traceFile_v2, disableScriptTreatment: false, textReplaceControl: false);

                string traceFile_v4 = GetFile(file, ".trace.v4.txt", traceDirectory);
                //HtmlReader_v4.ReadFile(file, generateCloseTag: false, disableLineColumn: true, disableScriptTreatment: false, useReadAttributeValue_v2: false,
                //    useTranslateChar: true)
                //    .zSave(traceFile_v4);
                //useReadAttributeValue_v2: false
                HtmlReader_v4.ReadFile(file, options: HtmlReaderOptions.Default | HtmlReaderOptions.DisableLineColumn).zSave(traceFile_v4, jsonIndent: true);

                if (zfile.AreFileEqual(traceFile_v2, traceFile_v4))
                {
                    okTraceNb++;
                    Trace.WriteLine("  trace files ok");
                    if (deleteOkTrace)
                    {
                        zFile.Delete(traceFile_v2);
                        zFile.Delete(traceFile_v4);
                    }
                }
                else
                {
                    notOkTraceNb++;
                    Trace.WriteLine("  trace files not identical");
                }
            }
            Trace.WriteLine();
            Trace.WriteLine("{0} files", nb);
            Trace.WriteLine("trace files : ok {0}, not ok {1}", okTraceNb, notOkTraceNb);
        }

        public static void Test_HtmlToXml_v1_v2(string directory = null, string traceDirectory = null, string pattern = "*.html", bool useXDocumentCreator = false, bool correctionMarkBeginEnd = false)
        {
            bool deleteOkTrace = true;

            int nb = 0;
            int okNb = 0;
            int notOkNb = 0;
            foreach (string file in GetHtmlFiles(directory, pattern))
            {
                nb++;
                Trace.WriteLine("file \"{0}\"", file);

                string xmlFile_v1 = GetFile(file, ".v1.xml", traceDirectory);
                FileHtmlToXml_HtmlReader_v2(file, xmlFile_v1, null, null, useXDocumentCreator, correctionMarkBeginEnd);

                string xmlFile_v2 = GetFile(file, ".v2.xml", traceDirectory);
                FileHtmlToXml_v2(file, xmlFile_v2, false, false);

                if (zfile.AreFileEqual(xmlFile_v1, xmlFile_v2))
                {
                    okNb++;
                    Trace.WriteLine("  xml files ok");
                    if (deleteOkTrace)
                    {
                        zFile.Delete(xmlFile_v1);
                        zFile.Delete(xmlFile_v2);
                    }
                }
                else
                {
                    notOkNb++;
                    Trace.WriteLine("  xml files not identical");
                }
            }
            Trace.WriteLine();
            Trace.WriteLine("{0} files", nb);
            Trace.WriteLine("xml files : ok {0}, not ok {1}", okNb, notOkNb);
        }

        public static void Test_HtmlReader_v2_v3(string directory = null)
        {
            // compare HtmlReader_v2 and HtmlReader_v3
            // for each html file :
            //   - generate HtmlReader_v2 trace file .html.trace.v2.txt
            //   - generate HtmlReader_v3 trace file .html.trace.v3.txt
            //   - compare .html.trace.v2.txt with .html.trace.v3.txt

            int nb = 0;
            int okTraceNb = 0;
            int notOkTraceNb = 0;
            foreach (string file in GetHtmlFiles(directory))
            {
                nb++;
                Trace.WriteLine("file \"{0}\"", file);

                //string xmlFile = GetXmlFile(file);
                string traceFile_v2 = GetFile(file, ".trace.v2.txt");
                //FileHtmlToXml_v2(file, xmlFile, traceFile_v2);
                TraceHtmlReader_v2(file, traceFile_v2);

                string traceFile_v3 = GetFile(file, ".trace.v3.txt");
                HtmlReader_v3.ReadFile(file, generateCloseTag: false, disableLineColumn: true).zSave(traceFile_v3);
                if (zfile.AreFileEqual(traceFile_v2, traceFile_v3))
                {
                    okTraceNb++;
                    Trace.WriteLine("  trace files ok");
                }
                else
                {
                    notOkTraceNb++;
                    Trace.WriteLine("  trace files not identical");
                }
            }
            Trace.WriteLine();
            Trace.WriteLine("{0} files", nb);
            Trace.WriteLine("trace files : ok {0}, not ok {1}", okTraceNb, notOkTraceNb);
        }

        public static void TestUnit(string directory = null, int htmlReaderVersion = 2)
        {
            // test unit HtmlToXml with HtmlReader or HtmlReader_v2
            // for each html file :
            //   - generate HtmlReader or HtmlReader_v2 trace file .html.trace.txt
            //   - compare trace file with .html.trace.ok.txt
            //   - generate xml file
            //   - compare xml file with .ok.xml

            //Trace.WriteLine("Test_Unit_HtmlToXml");
            //string dir = GetDirectory();
            bool traceHtmlReader = true;
            int nb = 0;
            int okXmlNb = 0;
            int notOkXmlNb = 0;
            int noOkXmlNb = 0;
            int okTraceNb = 0;
            int notOkTraceNb = 0;
            int noOkTraceNb = 0;
            //foreach (string file in zDirectory.EnumerateFiles(dir, "*.html", SearchOption.AllDirectories))
            foreach (string file in GetHtmlFiles(directory))
            {
                Trace.WriteLine("convert file \"{0}\"", file);
                string xmlFile = GetXmlFile(file);
                string traceHtmlReaderFile = null;
                if (traceHtmlReader)
                    traceHtmlReaderFile = GetFile(file, ".trace.txt");
                FileHtmlToXml(file, xmlFile, traceHtmlReaderFile, htmlReaderVersion);
                string okXmlFile = GetOkFile(xmlFile);
                string okTraceHtmlReaderFile = GetOkFile(traceHtmlReaderFile);
                nb++;
                if (zFile.Exists(okXmlFile))
                {
                    if (zfile.AreFileEqual(xmlFile, okXmlFile))
                    {
                        okXmlNb++;
                        Trace.WriteLine("  xml file ok \"{0}\"", xmlFile);
                    }
                    else
                    {
                        notOkXmlNb++;
                        Trace.WriteLine("  xml file not identical \"{0}\"", xmlFile);
                    }
                }
                else
                {
                    noOkXmlNb++;
                    //Trace.WriteLine("no ok file for \"{0}\"", xmlFile);
                    Trace.WriteLine("  ok xml file not found \"{0}\"", okXmlFile);
                }
                if (zFile.Exists(okTraceHtmlReaderFile))
                {
                    if (zfile.AreFileEqual(traceHtmlReaderFile, okTraceHtmlReaderFile))
                    {
                        okTraceNb++;
                        Trace.WriteLine("  trace file ok \"{0}\"", traceHtmlReaderFile);
                    }
                    else
                    {
                        notOkTraceNb++;
                        Trace.WriteLine("  trace file not identical \"{0}\"", traceHtmlReaderFile);
                    }
                }
                else
                {
                    noOkTraceNb++;
                    //Trace.WriteLine("no ok trace file for \"{0}\"", traceHtmlReaderFile);
                    Trace.WriteLine("  ok trace file not found \"{0}\"", okTraceHtmlReaderFile);
                }
            }
            Trace.WriteLine();
            Trace.WriteLine("{0} files converted", nb);
            Trace.WriteLine("xml files   : ok {0}, not ok {1}, no ok {2}", okXmlNb, notOkXmlNb, noOkXmlNb);
            Trace.WriteLine("trace files : ok {0}, not ok {1}, no ok {2}", okTraceNb, notOkTraceNb, noOkTraceNb);
        }

        public static void FileHtmlToXml(string file, bool traceHtmlReader = false, int htmlReaderVersion = 2, Encoding encoding = null)
        {
            string xmlFile = GetXmlFile(file);
            string traceHtmlReaderFile = null;
            if (traceHtmlReader)
                traceHtmlReaderFile = GetFile(file, ".trace.txt");
            Trace.WriteLine("convert html file \"{0}\"", file);
            Trace.WriteLine("  to xml file     \"{0}\"", xmlFile);
            Trace.WriteLine("  trace file      \"{0}\"", traceHtmlReaderFile);
            FileHtmlToXml(file, xmlFile, traceHtmlReaderFile, htmlReaderVersion, encoding);
        }

        public static void FileHtmlToXml(string file, string xmlFile, string traceHtmlReaderFile, int htmlReaderVersion = 2, Encoding encoding = null)
        {
            //using (StreamReader sr = zFile.OpenText(file))
            using (StreamReader sr = zfile.OpenText(file, encoding))
            {
                HtmlReader.TraceHtmlReaderFile = traceHtmlReaderFile;
                HtmlReader_v2.TraceHtmlReaderFile = traceHtmlReaderFile;
                //HtmlReader htmlReader = new HtmlReader(sr);
                //htmlReader.TraceHtmlReaderFile = traceHtmlReaderFile;
                //HtmlToXml hx = new HtmlToXml(htmlReader);
                HtmlToXml_v2.HtmlReaderVersion = htmlReaderVersion;
                HtmlToXml_v2 hx = new HtmlToXml_v2(sr);
                //hx.ReadCommentInText = _readCommentInText;
                //file = zpath.PathSetExtension(file, ".xml");
                hx.GenerateXDocument().Save(xmlFile);
            }
        }

        public static void FileHtmlToXml_HtmlReader_v2(string file, bool traceHtmlReader = false, bool traceHtmlToXml = false, bool useXDocumentCreator = false, bool correctionMarkBeginEnd = false, Encoding encoding = null)
        {
            //string xmlFile = GetXmlFile(file);
            string xmlFile = GetFile(file, ".v1.xml");
            string traceHtmlReaderFile = null;
            if (traceHtmlReader)
                traceHtmlReaderFile = GetFile(file, ".HtmlReader.v2.txt");
            string traceHtmlToXmlFile = null;
            if (traceHtmlToXml)
                traceHtmlToXmlFile = GetFile(file, ".HtmlToXml.v1.json");
            Trace.WriteLine("convert html file \"{0}\"", file);
            Trace.WriteLine("  to xml file     \"{0}\"", xmlFile);
            Trace.WriteLine("  trace file      \"{0}\"", traceHtmlReaderFile);

            FileHtmlToXml_HtmlReader_v2(file, xmlFile, traceHtmlReaderFile, traceHtmlToXmlFile, useXDocumentCreator, correctionMarkBeginEnd, encoding);
        }

        public static void FileHtmlToXml_HtmlReader_v2(string file, string xmlFile, string traceHtmlReaderFile, string traceHtmlToXmlFile, bool useXDocumentCreator = false, bool correctionMarkBeginEnd = false, Encoding encoding = null)
        {
            try
            {
                if (traceHtmlReaderFile != null)
                {
                    __srTraceHtmlReader = zFile.CreateText(traceHtmlReaderFile);
                    __traceJsonSettings = new JsonWriterSettings();
                    __traceJsonSettings.Indent = true;
                }

                using (StreamReader sr = zfile.OpenText(file, encoding))
                {
                    //HtmlReader.TraceHtmlReaderFile = traceHtmlReaderFile;
                    //HtmlReader_v2.TraceHtmlReaderFile = traceHtmlReaderFile;
                    //HtmlToXml.HtmlReaderVersion = htmlReaderVersion;
                    HtmlReader_v2 htmlReader = new HtmlReader_v2(sr);
                    //htmlReader.Trace += TraceHtmlReader;
                    htmlReader.Trace = TraceHtmlReader;
                    //HtmlToXml hx = new HtmlToXml(sr);
                    HtmlToXml_v2 hx = new HtmlToXml_v2(htmlReader);
                    hx.UseXDocumentCreator = useXDocumentCreator;
                    hx.CorrectionMarkBeginEnd = correctionMarkBeginEnd;
                    hx.GenerateXDocument().Save(xmlFile);
                    if (useXDocumentCreator && traceHtmlToXmlFile != null)
                        hx.Log.zSave(traceHtmlToXmlFile);
                }
            }
            finally
            {
                if (__srTraceHtmlReader != null)
                {
                    __srTraceHtmlReader.Close();
                    __srTraceHtmlReader = null;
                }
                __traceJsonSettings = null;
            }
        }

        public static void FileHtmlToXml_v2(string file, bool traceHtmlReader = false, bool traceHtmlToXml = false)
        {
            //string xmlFile = GetXmlFile(file);
            string xmlFile = GetFile(file, ".v2.xml");
            Trace.WriteLine("convert html file \"{0}\"", file);
            Trace.WriteLine("  to xml file     \"{0}\"", xmlFile);
            FileHtmlToXml_v2(file, xmlFile, traceHtmlReader, traceHtmlToXml);
        }

        public static void FileHtmlToXml_v2(string file, string xmlFile, bool traceHtmlReader = false, bool traceHtmlToXml = false)
        {
            //HtmlReaderOptions options = HtmlReaderOptions.Default | HtmlReaderOptions.GenerateCloseTag;
            HtmlReaderOptions options = HtmlReaderOptions.Default;
            if (traceHtmlReader)
                HtmlReader_v4.ReadFile(file, options: options | HtmlReaderOptions.DisableLineColumn).zSave(GetFile(file, ".HtmlReader.v4.txt"), jsonIndent: true);

            using (StreamReader sr = zfile.OpenText(file))
            {
                HtmlReader_v4 htmlReader = new HtmlReader_v4(sr, options);
                HtmlToXml_v3 htmlToXml = new HtmlToXml_v3(htmlReader);
                htmlToXml.CreateXml().Save(xmlFile);
                if (traceHtmlToXml)
                    htmlToXml.Log.zSave(GetFile(file, ".HtmlToXml.v2.json"));
            }
        }

        private static StreamWriter __srTraceHtmlReader = null;
        private static JsonWriterSettings __traceJsonSettings = null;
        public static void TraceHtmlReader(HtmlNode htmlNode)
        {
            if (__srTraceHtmlReader != null)
                __srTraceHtmlReader.WriteLine(htmlNode.ToJson(__traceJsonSettings));
        }

        public static void SetFilesAsOk(string directory = null, bool overwrite = false)
        {
            foreach (string file in GetHtmlFiles(directory))
            {
                string xmlFile = GetXmlFile(file);
                string okXmlFile = GetOkFile(xmlFile);
                string traceHtmlReaderFile = GetFile(file, ".trace.txt");
                string okTraceHtmlReaderFile = GetOkFile(traceHtmlReaderFile);
                zfile.RenameFile(xmlFile, okXmlFile, overwrite);
                zfile.RenameFile(traceHtmlReaderFile, okTraceHtmlReaderFile, overwrite);
            }
        }

        public static void ArchiveOkFiles(string directory = null)
        {
            string currentDir = null;
            string currentArchiveDir = null;
            foreach (string file in GetHtmlFiles(directory))
            {
                string dir = zPath.GetDirectoryName(file);
                if (dir != currentDir)
                {
                    currentDir = dir;
                    currentArchiveDir = zdir.GetNewIndexedDirectory(zPath.Combine(dir, __archiveDirectory), indexLength: 2);
                }
                string okXmlFile = GetOkFile(GetXmlFile(file));
                string okTraceHtmlReaderFile = GetOkFile(GetFile(file, ".trace.txt"));
                //zfile.MoveFile(okXmlFile, currentArchiveDir);
                //zfile.MoveFile(okTraceHtmlReaderFile, currentArchiveDir);
                zfile.CopyFileToDirectory(okXmlFile, currentArchiveDir);
                zfile.CopyFileToDirectory(okTraceHtmlReaderFile, currentArchiveDir);
            }
        }

        public static string GetXmlFile(string file)
        {
            return zpath.PathSetExtension(file, ".xml");
        }

        public static string GetFile(string file, string suffix, string directory = null)
        {
            //return HtmlReader.GetTraceFile(file);
            // ".trace.txt"
            //return zpath.PathSetFileName(file, zPath.GetFileName(file) + suffix);
            string directory2;
            if (directory == null)
                directory2 = zPath.GetDirectoryName(file);
            else
                directory2 = directory;
            return zPath.Combine(directory2, zPath.GetFileNameWithoutExtension(file) + suffix);
        }

        //public static string GetTraceFile_v2(string file)
        //{
        //    return zpath.PathSetFileName(file, zPath.GetFileName(file) + ".trace.v2.txt");
        //}

        //public static string GetTraceFile_v3(string file)
        //{
        //    return zpath.PathSetFileName(file, zPath.GetFileName(file) + ".trace.v3.txt");
        //}

        public static string GetOkFile(string file)
        {
            //return zpath.PathSetFileName(file, zPath.GetFileNameWithoutExtension(file) + ".ok");
            return zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + ".ok");
        }

        public static IEnumerable<string> GetHtmlFiles(string directory = null, string pattern = "*.html")
        {
            //return zdir.EnumerateFilesInfo(GetDirectory(), "*.html", filter:
            //    dirInfo =>
            //    {
            //        bool select = zPath.GetFileName(dirInfo.SubDirectory) != _archiveDirectory;
            //        return new EnumDirectoryFilter { Select = select, RecurseSubDirectory = select };
            //    }).Select(fileInfo => fileInfo.File);
            //return zdir.EnumerateFilesInfo(GetDirectory(), "*.html").Select(fileInfo => fileInfo.File);
            if (directory == null)
                directory = GetDirectory();
            directory = RootDirectory(directory);
            //Trace.WriteLine("read directory \"{0}\"", directory);
            Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter =
                dirInfo =>
                {
                    bool select = zPath.GetFileName(dirInfo.SubDirectory) != __archiveDirectory;
                    return new EnumDirectoryFilter { Select = select, RecurseSubDirectory = select };
                };
            return zdir.EnumerateFilesInfo(directory, pattern, directoryFilters: new Func<EnumDirectoryInfo, EnumDirectoryFilter>[] { directoryFilter }).Select(fileInfo => fileInfo.File);
        }

        public static string GetDirectory()
        {
            //return zPath.Combine(RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory"), @"Web\HtmlToXml\sites");
            //return zPath.Combine(@"c:\pib\dev_data\exe\runsource\test_unit", __testUnitSubdirectory);
            return __testUnitSubdirectory;
        }

        public static string RootDirectory(string directory)
        {
            return directory.zRootPath(__testUnitDirectory);
        }

        //public static void Trace_HtmlReader(string file)
        //{
        //    StreamReader sr = null;
        //    StreamWriter sw = null;
        //    string outFile = zpath.PathSetFileNameWithExtension(file, zPath.GetFileName(file) + ".trace.txt");
        //    try
        //    {
        //        sw = File.CreateText(outFile);
        //        sr = File.OpenText(file);

        //        HtmlReader htmlReader = new HtmlReader(sr);
        //        int i = 1;
        //        while (htmlReader.Read())
        //        {
        //            sw.Write("{0,5}", i);
        //            if (htmlReader.IsDocType)
        //            {
        //                sw.Write(" doc type : \"{0}\"", htmlReader.Value);
        //            }
        //            if (htmlReader.IsComment)
        //            {
        //                sw.Write(" comment : \"{0}\"", htmlReader.Value.zReplaceControl());
        //            }
        //            if (htmlReader.IsText)
        //            {
        //                sw.Write(" text : \"{0}\"", htmlReader.Value.zReplaceControl());
        //            }
        //            if (htmlReader.IsMarkBegin)
        //            {
        //                sw.Write(" mark begin : \"{0}\"", htmlReader.MarkName);
        //            }
        //            if (htmlReader.IsMarkEnd)
        //            {
        //                sw.Write(" mark end : \"{0}\"", htmlReader.MarkName);
        //            }
        //            //if (htmlReader.IsMarkInProgress)
        //            //{
        //            //    sw.WriteLine("{0,5} mark in progress : \"{1}\"", i, htmlReader.MarkName);
        //            //}
        //            if (htmlReader.IsProperty)
        //            {
        //                sw.Write(" property : \"{0}\" = \"{1}\" quote {2}", htmlReader.PropertyName, htmlReader.PropertyValue, htmlReader.PropertyQuote);
        //            }
        //            if (htmlReader.IsTextSeparator)
        //            {
        //                sw.WriteLine(" text separator");
        //            }
        //            if (htmlReader.IsScript)
        //            {
        //                sw.Write(" script : \"{0}\"", i, htmlReader.Text);
        //            }
        //            if (htmlReader.IsMarkBeginEnd)
        //            {
        //                sw.Write(" mark begin end : \"{0}\"", htmlReader.MarkName);
        //            }
        //            sw.WriteLine();
        //            i++;
        //        }

        //    }
        //    finally
        //    {
        //        if (sr != null)
        //            sr.Close();
        //        if (sw != null)
        //            sw.Close();
        //    }
        //}

        //public static string ReplaceControl(string text)
        //{
        //    text = text.Replace("\r", @"\r");
        //    text = text.Replace("\n", @"\n");
        //    text = text.Replace("\t", @"\t");
        //    return text;
        //}
    }
}
