using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.IO;
using pb.Text;

namespace Print.old
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
}
