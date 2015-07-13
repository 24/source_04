using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using pb.Web;

namespace Test.Test_Unit.Web
{
    public static class Test_Unit_HtmlToXml
    {
        private static string _archiveDirectory = "archive";

        public static void Test()
        {
            Trace.WriteLine("Test_Unit_HtmlToXml");
            string dir = GetDirectory();
            bool traceHtmlReader = true;
            int nb = 0;
            int okXmlNb = 0;
            int notOkXmlNb = 0;
            int noOkXmlNb = 0;
            int okTraceNb = 0;
            int notOkTraceNb = 0;
            int noOkTraceNb = 0;
            foreach (string file in Directory.EnumerateFiles(dir, "*.html", SearchOption.AllDirectories))
            {
                Trace.WriteLine("convert file \"{0}\"", file);
                string xmlFile = GetXmlFile(file);
                string traceHtmlReaderFile = null;
                if (traceHtmlReader)
                    traceHtmlReaderFile = GetTraceFile(file);
                FileHtmlToXml(file, xmlFile, traceHtmlReaderFile);
                string okXmlFile = GetOkFile(xmlFile);
                string okTraceHtmlReaderFile = GetOkFile(traceHtmlReaderFile);
                nb++;
                if (File.Exists(okXmlFile))
                {
                    if (zfile.AreFileEqual(xmlFile, okXmlFile))
                    {
                        okXmlNb++;
                        Trace.WriteLine("file ok \"{0}\"", xmlFile);
                    }
                    else
                    {
                        notOkXmlNb++;
                        Trace.WriteLine("file not identical \"{0}\"", xmlFile);
                    }
                }
                else
                {
                    noOkXmlNb++;
                    Trace.WriteLine("no ok file for \"{0}\"", xmlFile);
                }
                if (File.Exists(okTraceHtmlReaderFile))
                {
                    if (zfile.AreFileEqual(traceHtmlReaderFile, okTraceHtmlReaderFile))
                    {
                        okTraceNb++;
                        Trace.WriteLine("trace file ok \"{0}\"", traceHtmlReaderFile);
                    }
                    else
                    {
                        notOkTraceNb++;
                        Trace.WriteLine("trace file not identical \"{0}\"", traceHtmlReaderFile);
                    }
                }
                else
                {
                    noOkTraceNb++;
                    Trace.WriteLine("no ok trace file for \"{0}\"", traceHtmlReaderFile);
                }
            }
            Trace.WriteLine("{0} files converted", nb);
            Trace.WriteLine("xml files   : ok {0}, not ok {1}, no ok {2}", okXmlNb, notOkXmlNb, noOkXmlNb);
            Trace.WriteLine("trace files : ok {0}, not ok {1}, no ok {2}", okTraceNb, notOkTraceNb, noOkTraceNb);
        }

        public static void FileHtmlToXml(string file, bool traceHtmlReader = false)
        {
            string xmlFile = GetXmlFile(file);
            string traceHtmlReaderFile = null;
            if (traceHtmlReader)
                traceHtmlReaderFile = GetTraceFile(file);
            FileHtmlToXml(file, xmlFile, traceHtmlReaderFile);
        }

        public static void FileHtmlToXml(string file, string xmlFile, string traceHtmlReaderFile)
        {
            using (StreamReader sr = File.OpenText(file))
            {
                HtmlReader htmlReader = new HtmlReader(sr);
                htmlReader.TraceHtmlReaderFile = traceHtmlReaderFile;
                HtmlToXml hx = new HtmlToXml(htmlReader);
                //hx.ReadCommentInText = _readCommentInText;
                //file = zpath.PathSetExtension(file, ".xml");
                hx.GenerateXDocument().Save(xmlFile);
            }
        }

        public static void SetFilesAsOk(bool overwrite = false)
        {
            foreach (string file in GetHtmlFiles())
            {
                string xmlFile = GetXmlFile(file);
                string okXmlFile = GetOkFile(xmlFile);
                string traceHtmlReaderFile = GetTraceFile(file);
                string okTraceHtmlReaderFile = GetOkFile(traceHtmlReaderFile);
                zfile.RenameFile(xmlFile, okXmlFile, overwrite);
                zfile.RenameFile(traceHtmlReaderFile, okTraceHtmlReaderFile, overwrite);
            }
        }

        public static void ArchiveOkFiles()
        {
            string currentDir = null;
            string currentArchiveDir = null;
            foreach (string file in GetHtmlFiles())
            {
                string dir = Path.GetDirectoryName(file);
                if (dir != currentDir)
                {
                    currentDir = dir;
                    currentArchiveDir = zdir.GetNewIndexedDirectory(Path.Combine(dir, _archiveDirectory));
                }
                string okXmlFile = GetOkFile(GetXmlFile(file));
                string okTraceHtmlReaderFile = GetOkFile(GetTraceFile(file));
                zfile.MoveFile(okXmlFile, currentArchiveDir);
                zfile.MoveFile(okTraceHtmlReaderFile, currentArchiveDir);
            }
        }

        public static string GetXmlFile(string file)
        {
            return zpath.PathSetExtension(file, ".xml");
        }

        public static string GetTraceFile(string file)
        {
            return HtmlReader.GetTraceFile(file);
        }

        public static string GetOkFile(string file)
        {
            return zpath.PathSetFileName(file, Path.GetFileNameWithoutExtension(file) + ".ok");
        }

        public static IEnumerable<string> GetHtmlFiles()
        {
            return zdir.EnumerateFilesInfo(GetDirectory(), "*.html", filter:
                dirInfo =>
                {
                    bool select = Path.GetFileName(dirInfo.SubDirectory) != _archiveDirectory;
                    return new EnumDirectoryFilter { Select = select, RecurseSubDirectory = select };
                }).Select(fileInfo => fileInfo.File);
        }

        public static string GetDirectory()
        {
            return Path.Combine(RunSource.CurrentRunSource.Config.GetExplicit("TestUnitDirectory"), @"Web\HtmlToXml\sites");
        }

        //public static void Trace_HtmlReader(string file)
        //{
        //    StreamReader sr = null;
        //    StreamWriter sw = null;
        //    string outFile = zpath.PathSetFileNameWithExtension(file, Path.GetFileName(file) + ".trace.txt");
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
