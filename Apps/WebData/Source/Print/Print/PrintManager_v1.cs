using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using pb.old; // for zSetTextVariables() in pb\_old\TextVariables.cs

namespace Print
{
    public class PrintManager_v1
    {
        private Dictionary<string, Print1> _prints = null;
        private RegexValuesList _printRegexList = null;
        private RegexValuesList _printRegexList2 = null;
        private RegexValuesList _dateRegexList = null;
        private Dictionary<string, RegexValuesModel> _regexModels = null;
        private string _directory = null;
        private static bool _trace = false;

        public PrintManager_v1(XElement xelement)
        {
            init(xelement);
        }

        private void init(XElement xelement)
        {
            _directory = xelement.zXPathValue("Directory");

            Dictionary<string, RegexValuesModel> regexModels = new Dictionary<string, RegexValuesModel>();
            // zElements("FilenamesModel/FilenameModel")
            foreach (XElement xe in xelement.zXPathElements("FilenamesModel/FilenameModel"))
            {
                RegexValuesModel rvm = new RegexValuesModel(xe);
                regexModels.Add(rvm.key, rvm);
            }

            _regexModels = new Dictionary<string, RegexValuesModel>();
            // zElements("FilenameModels/FilenameModel")
            foreach (XElement xe in xelement.zXPathElements("FilenameModels/FilenameModel"))
            {
                RegexValuesModel rvm = new RegexValuesModel(xe);
                _regexModels.Add(rvm.key, rvm);
            }
            // zElements("FilenameDates/FilenameDate")
            _dateRegexList = new RegexValuesList(xelement.zXPathElements("FilenameDates/FilenameDate"));
            //if (_trace)
            //    Trace.CurrentTrace.WriteLine("_dateRegexList {0}", _dateRegexList.Count);

            _printRegexList = new RegexValuesList();
            _printRegexList2 = new RegexValuesList();
            _prints = new Dictionary<string, Print1>();
            // zElements("Prints/Print")
            foreach (XElement xe in xelement.zXPathElements("Prints/Print"))
            {
                Print1 print = null;
                switch (xe.zXPathValue("Class"))
                {
                    case "LeMonde":
                        print = new LeMonde(xe, _directory, _regexModels);
                        break;
                    case "LeParisien":
                        print = new LeParisien(xe, _directory, _regexModels);
                        break;
                    case "LeVifExpress":
                        print = new LeVifExpress(xe, _directory, _regexModels);
                        break;
                    default:
                        print = new Print1(xe, _directory, _regexModels);
                        break;
                }
                string name = print.Name;
                _prints.Add(name, print);
                int n = 1;
                // zElements("Filenames/Filename")
                foreach (XElement xe2 in xe.zXPathElements("Filenames/Filename"))
                {
                    string key = name + n++.ToString();
                    //string name = xe2.zExplicitAttributeValue("name");
                    //string model = xe2.zExplicitAttributeValue("model");
                    string model = xe2.zAttribValue("model");
                    if (model != null)
                    {
                        if (!regexModels.ContainsKey(model))
                            throw new PBException("unknow filename model \"{0}\"", model);
                        RegexValuesModel rvm = regexModels[model];
                        //Dictionary<string, string> attribs = (from xa in xe2.Attributes() where xa.Name.ToString().StartsWith("v_") select xa).zAttribs();
                        Dictionary<string, string> attribs = new Dictionary<string, string>();
                        attribs.zAdd(from xa in xe2.Attributes() where xa.Name.ToString().StartsWith("v_") select xa);
                        string pattern = rvm.pattern.zSetTextVariables(attribs, true);
                        //Trace.CurrentTrace.WriteLine("\"{0}\" - \"{1}\"", rvm.pattern, pattern);
                        //string values = xe2.zAttribValue("values");
                        //if (values != null)
                        //{
                        //    if (rvm.values != null && rvm.values != "")
                        //        values = rvm.values + ", " + values;
                        //}
                        //else
                        //    values = rvm.values;
                        string values = rvm.values;
                        if (values != null)
                            values = values.zSetTextVariables(attribs, true);
                        _printRegexList.Add(key, new RegexValues(key, name, pattern, rvm.options, values));
                    }
                    else
                    {
                        string regex = xe2.zExplicitAttribValue("regex");
                        string values = xe2.zExplicitAttribValue("values");
                        string options = xe2.zAttribValue("options");
                        _printRegexList.Add(key, new RegexValues(key, name, regex, options, values));
                    }
                }

                n = 1;
                // zElements("Filenames2/Filename")
                foreach (XElement xe2 in xe.zXPathElements("Filenames2/Filename"))
                {
                    string key = name + n++.ToString();
                    string regex = xe2.zExplicitAttribValue("regex");
                    string values = xe2.zAttribValue("values");
                    string options = xe2.zAttribValue("options");
                    _printRegexList2.Add(key, new RegexValues(key, name, regex, options, values));
                }
            }
        }

        public RegexValuesList PrintRegexList { get { return _printRegexList; } }

        public Print1 Find0(string filename, out string error)
        {
            error = null;
            if (_trace)
                Trace.CurrentTrace.WriteLine("search \"{0}\"", filename);
            //FindText_old findText = _printRegexList.Find_old(filename);
            FindText findText = _printRegexList.Find(filename);
            if (!findText.found)
            {
                if (_trace)
                    Trace.CurrentTrace.WriteLine("print not found \"{0}\"", filename);
                return null;
            }
            //Print1 print = Get(findText.regexValues.Name);
            Print1 print = Get(findText.matchValues.Name);
            //NamedValues<ZValue> values = findText.regexValues.GetValues_old();
            NamedValues<ZValue> values = findText.matchValues.GetValues();
            if (_trace)
            {
                bool first = true;
                foreach (KeyValuePair<string, ZValue> value in values)
                {
                    if (!first)
                        Trace.CurrentTrace.Write(", ");
                    first = false;
                    Trace.CurrentTrace.Write("{0}={1}", value.Key, value.Value);
                }
                Trace.CurrentTrace.WriteLine();
            }
            if (!print.TrySetValues(values))
            {
                error = string.Format("find \"{0}\" error \"{1}\"", print.Name, values.Error);
                return null;
            }
            return print;
        }

        //public Print Find2(string filename, out string error)
        //{
        //    error = null;
        //    if (_trace)
        //        Trace.CurrentTrace.WriteLine("search \"{0}\"", filename);
        //    foreach (RegexValues rv in _printRegexList2.Values)
        //    {
        //        Match match = rv.Match(filename);
        //        if (match.Success)
        //        {
        //            Print print = Get(rv.Name);
        //            print.NewIssue();
        //            NamedValues values = rv.GetValues();
        //            if (_trace)
        //            {
        //                Trace.CurrentTrace.WriteLine("found {0}", print.Name);
        //                values.zTrace();
        //            }
        //            if (!print.TrySetValues(values))
        //            {
        //                error = string.Format("find \"{0}\" error \"{1}\"", print.Name, values.Error);
        //                continue;
        //            }

        //            if (MatchRegexValues(print, print.NormalizedFilename, filename, out error))
        //                return print;
        //            if (MatchRegexValues(print, print.NormalizedSpecialFilename, filename, out error))
        //                return print;
        //            string filename2 = filename.Substring(0, match.Index) + filename.Substring(match.Index + match.Length);
        //            if (_trace)
        //                Trace.CurrentTrace.WriteLine("search date \"{0}\" (\"{1}\")", filename2, filename);
        //            foreach (RegexValues rv2 in _dateRegexList.Values)
        //            {
        //                match = rv.Match(filename2);
        //                if (match.Success)
        //                {
        //                    values = rv2.GetValues();
        //                    if (date.IsDateValid(values))
        //                    {
        //                        //print.SetValues(values);
        //                        if (!print.TrySetValues(values))
        //                        {
        //                            error = string.Format("find \"{0}\" error \"{1}\"", print.Name, values.Error);
        //                            continue;
        //                        }
        //                        return print;
        //                    }
        //                }
        //            }
        //            //return print;
        //        }
        //    }
        //    if (_trace)
        //        Trace.CurrentTrace.WriteLine("print not found \"{0}\"", filename);
        //    return null;
        //    //foreach (Print print in _prints.Values)
        //    //{
        //    //    Match match = print.MatchFilename(filename);
        //    //    //if (print.IsMatchFilename(filename))
        //    //    if (match != null && match.Success)
        //    //    {
        //    //        if (_trace)
        //    //            Trace.CurrentTrace.WriteLine("found {0}", print.Name);
        //    //        if (MatchRegexValues(print, print.NormalizedFilename, filename, out error))
        //    //            return print;
        //    //        if (MatchRegexValues(print, print.NormalizedSpecialFilename, filename, out error))
        //    //            return print;
        //    //        string filename2 = filename.Substring(0, match.Index) + filename.Substring(match.Index + match.Length);
        //    //        if (_trace)
        //    //            Trace.CurrentTrace.WriteLine("search date \"{0}\" (\"{1}\")", filename2, filename);
        //    //        //RegexValues rv = _dateRegexList.Find(filename2);
        //    //        foreach (RegexValues rv in _dateRegexList.Values)
        //    //        {
        //    //            match = rv.Match(filename2);
        //    //            if (match.Success)
        //    //            {
        //    //                Dictionary<string, object> values = rv.GetValues();
        //    //                if (date.IsValidDate(values))
        //    //                {
        //    //                    print.SetValues(values);
        //    //                    return print;
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    //return null;
        //}

        private bool MatchRegexValues(Print1 print, RegexValues rv, string filename, out string error)
        {
            error = null;
            //Match match = rv.Match_old(filename);
            MatchValues matchValues = rv.Match(filename);
            //if (!match.Success)
            if (!matchValues.Success)
                return false;
            //NamedValues<ZValue> values = rv.GetValues_old();
            NamedValues<ZValue> values = matchValues.GetValues();

            if (_trace)
                values.zTrace();

            if (!print.TrySetValues(values))
            {
                error = string.Format("find \"{0}\" error \"{1}\"", print.Name, values.Error);
                return false;
            }
            return true;
        }

        public Print1 Find(string filename, out string error)
        {
            error = null;
            if (_trace)
                Trace.CurrentTrace.WriteLine("search \"{0}\"", filename);
            foreach (RegexValues rv in _printRegexList.Values)
            {
                //Match match = rv.Match_old(filename);
                MatchValues matchValues = rv.Match(filename);
                //if (match.Success)
                if (matchValues.Success)
                {
                    Print1 print = Get(rv.Name);
                    print.NewIssue();
                    //NamedValues<ZValue> values = rv.GetValues_old();
                    NamedValues<ZValue> values = matchValues.GetValues();
                    if (_trace)
                        values.zTrace();
                    if (!print.TrySetValues(values))
                    {
                        error = string.Format("find \"{0}\" error \"{1}\"", print.Name, values.Error);
                        continue;
                    }
                    return print;
                }
            }

            if (_trace)
                Trace.CurrentTrace.WriteLine("print not found \"{0}\"", filename);
            return null;
        }

        public Print1 Get(string name)
        {
            if (!_prints.ContainsKey(name))
                throw new PBException("error unknow print \"{0}\"", name);
            return _prints[name];
        }
    }

    public partial class pu1
    {
        //private static ITrace _tr = Trace.CurrentTrace;

        public static void RenameFile(PrintManager_v1 pm, string path, bool simulate = false, bool moveFile = false, string printFile = null)
        {
            string fmt1 = "file {0,-70}";
            string fmt2 = " {0,-30}";
            string fmt3 = " {0,-60}";
            bool writeFilenameOk = true;
            bool writeUnknowPrint = true;
            bool logFileInDestinationDirectory = false;
            bool debug = false;
            string file = Path.GetFileName(path);
            if (!File.Exists(path))
            {
                Trace.WriteLine("file dont exists \"{0}\"", file);
                return;
            }
            Print1 print;
            //Trace.WriteLine("search print start");
            string error;
            if (printFile != null)
                print = pm.Find(printFile + ".pdf", out error);
            else
                print = pm.Find(file, out error);
            //Trace.WriteLine("search print end");
            string msgFile = "\"" + file + "\"";
            if (printFile != null)
                msgFile += " (\"" + printFile + "\")";
            if (print == null)
            {
                if (writeUnknowPrint)
                {
                    Trace.Write(fmt1, msgFile);
                    Trace.Write(" unknow print");
                    if (error != null)
                        Trace.Write(" " + error);
                    Trace.WriteLine();
                }
                return;
            }
            if (debug)
            {
                Trace.Write(fmt1, msgFile);
                Trace.WriteLine(fmt2, print.Name);
                //foreach (KeyValuePair<string, object> value in print.PrintValues)
                foreach (KeyValuePair<string, ZValue> value in print.PrintValues)
                    Trace.WriteLine("  {0} = {1}", value.Key, value.Value);
            }
            string file2 = print.GetFilename();
            if (file == file2 && (!moveFile || Path.GetDirectoryName(path) == print.Directory))
            {
                if (writeFilenameOk)
                {
                    Trace.Write(fmt1, msgFile);
                    Trace.Write(fmt2, print.Name);
                    Trace.Write(" filename ok");
                    if (moveFile)
                        Trace.Write(" move to same directory");
                    Trace.WriteLine();
                }
                return;
            }

            if (moveFile && !simulate && !Directory.Exists(print.Directory))
                Directory.CreateDirectory(print.Directory);
            string traceFile = null;
            if (moveFile && !simulate && logFileInDestinationDirectory)
            {
                traceFile = Path.Combine(print.Directory, "log.txt");
                //_tr.AddTraceFile(traceFile, LogOptions.None);
                if (traceFile != null)
                    Trace.CurrentTrace.AddOnWrite("pu1", WriteToFile.Create(traceFile, FileOption.None).Write);
            }
            try
            {
                Trace.Write(fmt1, msgFile);
                Trace.Write(fmt2, print.Name);
                string path2;
                bool fileExists = false;
                bool filesEquals = false;
                if (moveFile)
                {
                    //if (!simulate && !Directory.Exists(print.Directory))
                    //    Directory.CreateDirectory(print.Directory);
                    path2 = Path.Combine(print.Directory, file2);
                }
                else
                    path2 = zpath.PathSetFileName(path, file2);
                int index = 2;
                while (File.Exists(path2))
                {
                    fileExists = true;
                    if (path == path2)
                        break;
                    filesEquals = zfile.FilesEquals(path, path2);
                    if (filesEquals)
                        break;
                    file2 = print.GetFilename(index++);
                    //path2 = path.PathSetFileWithExt(path, file2);
                    path2 = zpath.PathSetFileName(path2, file2);
                }
                //if (simulate)
                //    Trace.Write(" simulate");
                //Trace.Write(" rename");
                //if (moveFile)
                //    Trace.Write(" and move");
                //Trace.WriteLine(" \"{0}\"", file2);
                Trace.Write(fmt3, "\"" + file2 + "\"");
                if (simulate)
                    Trace.Write(" simulate");
                if (fileExists)
                {
                    Trace.Write(" file exists");
                    if (filesEquals)
                        Trace.Write(" and is equals");
                    else
                        Trace.Write(" and is different");
                }
                if (filesEquals)
                    Trace.Write(" delete file");
                else
                {
                    Trace.Write(" rename");
                    if (moveFile)
                        Trace.Write(" and move");
                    Trace.Write(" file");
                }
                Trace.WriteLine();
                if (!simulate)
                {
                    if (filesEquals)
                        File.Delete(path);
                    else if (!File.Exists(path2))
                        File.Move(path, path2);
                }
            }
            finally
            {
                if (traceFile != null)
                    //_tr.RemoveTraceFile(traceFile);
                    Trace.CurrentTrace.RemoveOnWrite("pu1");
            }
        }
    }
}
