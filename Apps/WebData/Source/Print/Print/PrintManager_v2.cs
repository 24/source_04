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

namespace Print
{
    public class PrintManager_v2
    {
        private static bool _trace = false;
        //private static ITrace _tr = pb.Trace.CurrentTrace;
        private Dictionary<string, Print> _prints = null;
        private RegexValuesList _printRegexList = null;
        private RegexValuesList _dateRegexList = null;
        //private RegexValues _dayRegex = null;
        private RegexValuesList _dayRegexList = null;
        private RegexValuesList _numberRegexList = null;
        //private RegexValues _specialRegex = null;
        private RegexValuesList _specialRegexList = null;
        private Dictionary<string, RegexValuesModel> _regexModels = null;
        private string _directory = null;

        public PrintManager_v2(XElement xelement)
        {
            init(xelement);
        }

        private void init(XElement xelement)
        {
            _directory = xelement.zXPathValue("Directory");

            _regexModels = new Dictionary<string, RegexValuesModel>();
            // zElements("FilenameInfos/FilenameModel")
            foreach (XElement xe in xelement.zXPathElements("FilenameInfos/FilenameModel"))
            {
                RegexValuesModel rvm = new RegexValuesModel(xe);
                _regexModels.Add(rvm.key, rvm);
            }
            // zElements("FilenameInfos/FilenameDate")
            _dateRegexList = new RegexValuesList(xelement.zXPathElements("FilenameInfos/FilenameDate"));

            //XElement xe3 = xelement.zXPathElement("FilenameInfos/FilenameDay");
            //if (xe3 != null)
            //    _dayRegex = new RegexValues(xe3);
            // zElements("FilenameInfos/FilenameDay")
            _dayRegexList = new RegexValuesList(xelement.zXPathElements("FilenameInfos/FilenameDay"));

            // zElements("FilenameInfos/FilenameNumber")
            _numberRegexList = new RegexValuesList(xelement.zXPathElements("FilenameInfos/FilenameNumber"));

            //xe3 = xelement.zXPathElement("FilenameInfos/FilenameSpecial");
            //if (xe3 != null)
            //    _specialRegex = new RegexValues(xe3);
            // zElements("FilenameInfos/FilenameSpecial")
            _specialRegexList = new RegexValuesList(xelement.zXPathElements("FilenameInfos/FilenameSpecial"));

            _printRegexList = new RegexValuesList();
            _prints = new Dictionary<string, Print>();
            // zElements("Prints/Print")
            foreach (XElement xe in xelement.zXPathElements("Prints/Print"))
            {
                Print print = null;
                switch (xe.zXPathValue("Class"))
                {
                    case "LeMonde":
                        print = new PrintLeMonde(xe, _directory, _regexModels);
                        break;
                    case "LeParisien":
                        print = new PrintLeParisien(xe, _directory, _regexModels);
                        break;
                    case "LExpress":
                        print = new PrintLExpress(xe, _directory, _regexModels);
                        break;
                    case "LeVifExpress":
                        print = new PrintLeVifExpress(xe, _directory, _regexModels);
                        break;
                    default:
                        print = new Print(xe, _directory, _regexModels);
                        break;
                }
                string name = print.Name;
                _prints.Add(name, print);
                int n = 1;
                // zElements("Filename")
                foreach (XElement xe2 in xe.zXPathElements("Filename"))
                {
                    string key = name + n++.ToString();
                    string regex = xe2.zExplicitAttribValue("regex");
                    string values = xe2.zAttribValue("values");
                    string options = xe2.zAttribValue("options");
                    _printRegexList.Add(key, new RegexValues(key, name, regex, options, values));
                }
            }
        }

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public RegexValuesList PrintRegexList { get { return _printRegexList; } }
        public RegexValuesList DateRegexList { get { return _dateRegexList; } }
        public RegexValuesList DayRegexList { get { return _dayRegexList; } }
        public RegexValuesList NumberRegex { get { return _numberRegexList; } }
        public RegexValuesList SpecialRegexList { get { return _specialRegexList; } }

        public Print GetPrint(string name)
        {
            if (!_prints.ContainsKey(name))
                throw new PBException("error unknow print \"{0}\"", name);
            return _prints[name];
        }

        public PrintIssue Find(string filename)
        {
            WriteLine("search \"{0}\"", filename);
            PrintIssue issue = null;
            PrintIssue lastIssueError = null;
            //List<GClass2<PrintIssue, RegexValues>> issues = new List<GClass2<PrintIssue,RegexValues>>();
            List<GClass2<PrintIssue, MatchValues>> issues = new List<GClass2<PrintIssue,MatchValues>>();
            bool foundIssue = false;
            filename = zstr.ReplaceUTF8Code(filename);
            filename = filename.Replace('\u0430', 'a');
            filename = filename.Replace('\u0435', 'e');
            filename = filename.Replace('\u043E', 'o');
            WriteLine("search \"{0}\"", filename);
            foreach (RegexValues rv in _printRegexList.Values)
            {
                //issue = Match(rv, filename);
                GClass2<PrintIssue, MatchValues> gc = Match(rv, filename);
                //if (issue != null && issue.Error == null)
                if (gc.Value1 != null && gc.Value1.Error == null)
                {
                    Print print = issue.Print;

                    //if (issue.Match(print.NormalizedFilename, filename))
                    MatchValues matchValues = issue.Match(print.NormalizedFilename, filename);
                    if (matchValues != null)
                    {
                        foundIssue = true;
                        break;
                    }
                    //if (issue.Match(print.NormalizedSpecialFilename, filename))
                    matchValues = issue.Match(print.NormalizedSpecialFilename, filename);
                    if (matchValues != null)
                    {
                        foundIssue = true;
                        break;
                    }

                    //issues.Add(new GClass2<PrintIssue, RegexValues>(issue, rv));
                    issues.Add(new GClass2<PrintIssue, MatchValues>(issue, gc.Value2));
                }

                //if (issue != null && issue.Error != null)
                if (gc.Value1 != null && gc.Value1.Error != null)
                    lastIssueError = issue;
            }

            if (!foundIssue)
            {
                //foreach (GClass2<PrintIssue, RegexValues> issue2 in issues)
                foreach (GClass2<PrintIssue, MatchValues> issue2 in issues)
                {
                    issue = issue2.Value1;
                    //RegexValues rv = issue2.Value2;
                    MatchValues matchValues = issue2.Value2;
                    Print print = issue.Print;
                    //string filename2 = filename = rv.MatchReplace_old("_");
                    string filename2 = filename = matchValues.Replace("_");

                    FindSpecial(issue, ref filename2);

                    if (print.Frequency != PrintFrequency.Daily && print.Frequency != PrintFrequency.Weekly)
                    {
                        if (FindNumber(issue, filename2))
                            foundIssue = true;
                        //if (issue.Error != null)
                        //    lastIssueError = issue;
                    }


                    bool foundDate = false;
                    if (!foundIssue)
                    {
                        if (FindDate(issue, ref filename2))
                            foundDate = true;
                        //if (issue.Error != null)
                        //    lastIssueError = issue;
                    }

                    if (!foundIssue && print.Frequency != PrintFrequency.Daily)
                    {
                        if (FindNumber(issue, filename2))
                            foundIssue = true;
                        //if (issue.Error != null)
                        //    lastIssueError = issue;
                    }

                    if (foundDate)
                        foundIssue = true;

                    if (!foundIssue && (print.Frequency == PrintFrequency.Daily || print.Frequency == PrintFrequency.Weekly))
                    {
                        if (FindDay(issue, filename2))
                            foundIssue = true;
                        //if (issue.Error != null)
                        //    lastIssueError = issue;
                    }

                    if (issue.Error != null)
                        lastIssueError = issue;

                    if (foundIssue)
                        break;
                }
            }

            if (!foundIssue)
            {
                if (_trace)
                    WriteLine("print not found \"{0}\"", filename);
                return lastIssueError;
            }

            issue.Control();
            return issue;
        }

        private bool FindSpecial(PrintIssue issue, ref string filename)
        {
            foreach (RegexValues rv in _specialRegexList.Values)
            {
                //if (issue.Match(rv, filename))
                MatchValues matchValues = issue.Match(rv, filename);
                if (matchValues != null)
                {
                    //filename = rv.MatchReplace_old("_");
                    filename = matchValues.Replace("_");
                    return true;
                }
            }
            return false;
        }

        private bool FindDate(PrintIssue issue, ref string filename)
        {
            foreach (RegexValues rv in _dateRegexList.Values)
            {
                //if (issue.Match(rv, filename, v => zdate.IsDateValid(v)))
                MatchValues matchValues = issue.Match(rv, filename, v => zdate.IsDateValid(v));
                if (matchValues != null)
                {
                    //filename = rv.MatchReplace_old("_");
                    filename = matchValues.Replace("_");
                    if (!issue.Special)
                        return true;
                    break;
                }
            }
            return false;
        }

        private bool FindDay(PrintIssue issue, string filename)
        {
            foreach (RegexValues rv in _dayRegexList.Values)
            {
                //Match match = rv.Match_old(filename);
                MatchValues matchValues = rv.Match(filename);
                //while (match.Success)
                while (matchValues.Success)
                {
                    //if (issue.MatchSetValues(rv, v => zdate.IsDayValid(v)))
                    if (issue.MatchSetValues(matchValues, v => zdate.IsDayValid(v)))
                    {
                        return true;
                    }
                    //match = rv.Next_old();
                    matchValues = matchValues.Next();
                }
            }
            return false;
        }

        private bool FindNumber(PrintIssue issue, string filename)
        {
            foreach (RegexValues rv in _numberRegexList.Values)
            {
                //Match match = rv.Match_old(filename);
                MatchValues matchValues = rv.Match(filename);
                //while (match.Success)
                while (matchValues.Success)
                {
                    //if (issue.MatchSetValues(rv, v => PrintIssue.IsNumberValid(v)))
                    if (issue.MatchSetValues(matchValues, v => PrintIssue.IsNumberValid(v)))
                    {
                        if (issue.Control())
                        {
                            return true;
                        }
                    }
                    //match = rv.Next_old();
                    matchValues = matchValues.Next();
                }
            }
            return false;
        }

        //public PrintIssue Match(RegexValues rv, string filename)
        public GClass2<PrintIssue, MatchValues> Match(RegexValues rv, string filename)
        {
            //PrintIssue issue = null;
            //Match match = rv.Match_old(filename);
            MatchValues matchValues = rv.Match(filename);
            //if (match.Success)
            if (matchValues.Success)
            {
                Print print = GetPrint(rv.Name);
                PrintIssue issue = print.NewPrintIssue();
                //NamedValues<ZValue> values = rv.GetValues_old();
                NamedValues<ZValue> values = matchValues.GetValues();
                if (_trace)
                {
                    WriteLine("found {0}", print.Name);
                    WriteLine("pattern \"{0}\"", rv.Pattern);
                    values.zTrace();
                }
                issue.TrySetValues(values);
                return new GClass2<PrintIssue, MatchValues> { Value1 = issue, Value2 = matchValues };
            }
            else
                //return issue;
                return null;
        }

        private static void WriteLine(string msg, params object[] prm)
        {
            if (_trace)
                pb.Trace.WriteLine(msg, prm);
        }
    }

    public partial class zprint
    {
        public static void RenameFile(PrintManager_v2 pm, string path, bool simulate = false, bool moveFile = false, string printFile = null)
        {
            string fmt1 = "file {0,-70}";
            string fmt2 = " {0,-30}";
            string fmt3 = " {0,-60}";
            bool writeFilenameOk = true;
            bool writeUnknowPrint = true;
            bool logFileInDestinationDirectory = false;
            bool debug = false;
            //Trace.WriteLine("path \"{0}\"", path);
            if (!Path.IsPathRooted(path))
                path = Path.GetFullPath(path);
            //Trace.WriteLine("path \"{0}\"", path);
            string file = Path.GetFileName(path);
            if (!File.Exists(path))
            {
                Trace.WriteLine("file dont exists \"{0}\"", file);
                return;
            }
            PrintIssue issue;
            if (printFile != null)
                issue = pm.Find(printFile + ".pdf");
            else
                issue = pm.Find(file);
            string msgFile = "\"" + file + "\"";
            if (printFile != null)
                msgFile += " (\"" + printFile + "\")";
            if (issue == null || issue.Error != null)
            {
                if (writeUnknowPrint)
                {
                    Trace.Write(fmt1, msgFile);
                    if (issue == null)
                        Trace.Write(" unknow print");
                    else
                        Trace.Write(" {0}", issue.Print.Name);
                    if (issue != null && issue.Error != null)
                        Trace.Write(" " + issue.Error);
                    Trace.WriteLine();
                }
                return;
            }
            if (debug)
            {
                Trace.Write(fmt1, msgFile);
                Trace.WriteLine(fmt2, issue.Print.Name);
                issue.PrintValues.zTrace();
            }
            string file2 = issue.GetFilename();
            //Trace.WriteLine("Path.GetDirectoryName(path) \"{0}\"", Path.GetDirectoryName(path));
            //Trace.WriteLine("issue.Print.Directory       \"{0}\"", issue.Print.Directory);
            if (file == file2 && (!moveFile || Path.GetDirectoryName(path).Equals(issue.Print.Directory, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (writeFilenameOk)
                {
                    Trace.Write(fmt1, msgFile);
                    Trace.Write(fmt2, issue.Print.Name);
                    Trace.Write(" filename ok");
                    if (moveFile)
                        Trace.Write(" move to same directory");
                    Trace.WriteLine();
                }
                return;
            }

            if (moveFile && !simulate && !Directory.Exists(issue.Print.Directory))
                Directory.CreateDirectory(issue.Print.Directory);
            string traceFile = null;
            if (moveFile && !simulate && logFileInDestinationDirectory)
            {
                traceFile = Path.Combine(issue.Print.Directory, "log.txt");
                //_tr.AddTraceFile(traceFile);
                //_tr.AddTraceFile(traceFile, LogOptions.None);
                if (traceFile != null)
                    Trace.CurrentTrace.AddOnWrite("zprint", WriteToFile.Create(traceFile, FileOption.None).Write);
            }
            try
            {
                Trace.Write(fmt1, msgFile);
                Trace.Write(fmt2, issue.Print.Name);
                string path2;
                bool fileExists = false;
                bool filesEquals = false;
                if (moveFile)
                    path2 = Path.Combine(issue.Print.Directory, file2);
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
                    file2 = issue.GetFilename(index++);
                    path2 = zpath.PathSetFileName(path2, file2);
                }
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
                    Trace.CurrentTrace.RemoveOnWrite("zprint");
            }
        }
    }
}
