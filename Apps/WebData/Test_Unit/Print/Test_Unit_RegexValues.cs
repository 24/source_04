using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using pb;
using pb.Compiler;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using pb.Text;

namespace Test.Test_Unit.Print
{
    public class TestText
    {
        public string text;
    }

    public class TestFindDate
    {
        public string text;
        public bool foundDate;
        public string dateFound;
        public Date? date;
        public DateType dateType = DateType.Unknow;
        public string remainText;
        public NamedValues<ZValue> namedValues;
    }

    public static class Test_Unit_RegexValues
    {
        public static void Test(string dir)
        {
            //Trace.WriteLine("Test_Unit_RegexValues");
            //Test_FindDate(zPath.Combine(dir, @"RegexValues\RegexValues_FindDate_01.txt"));
        }

        public static void Test_FindDate_04(FindDateManager findDateManager, string file)
        {
            file = zPath.Combine(GetDirectoryDate(), file);

            string traceFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_new_out");
            string bsonFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_new_bson");

            Trace.WriteLine("Test_FindDate \"{0}\" (nb date regex {1})", file, findDateManager.DateRegexList.Count);
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            //Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Test_FindDate_04", WriteToFile.Create(traceFile, FileOption.RazFile).Write);

            try
            {
                DateTime dt = DateTime.Now;
                TraceRegexValuesList(findDateManager.DateRegexList);
                Trace.WriteLine();

                zmongo.FileBsonReader<TestText>(file).zFindDateNew(findDateManager).zSave(bsonFile);
                zmongo.FileBsonReader<TestFindDate>(bsonFile).zTraceFindDate();
                Trace.WriteLine("test duration {0}", DateTime.Now - dt);
            }
            finally
            {
                //Trace.CurrentTrace.EnableBaseLog();
                //Trace.CurrentTrace.RemoveTraceFile(traceFile);
                Trace.CurrentTrace.RemoveOnWrite("Test_FindDate_04");
                Trace.CurrentTrace.DisableViewer = false;
            }
        }

        public static void Test_FindDate_03(FindDateManager_v1 findDateManager, string file)
        {
            file = zPath.Combine(GetDirectoryDate(), file);
            int year = Date.Today.Year;
            findDateManager.DateRegexList.Add("year", new RegexValues("year", "year", string.Format(@"(?:^|[_\s])({0}|{1}|{2})(?:$|[_\s])", year - 1, year, year + 1), "IgnoreCase", "year", compileRegex: true));

            string traceFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out");
            Trace.WriteLine("Test_FindDate \"{0}\" (nb date regex {1})", file, findDateManager.DateRegexList.Count);
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            //Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Test_FindDate_03", WriteToFile.Create(traceFile, FileOption.RazFile).Write);

            try
            {
                DateTime dt = DateTime.Now;
                TraceRegexValuesList(findDateManager.DateRegexList);
                Trace.WriteLine();

                string bsonFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_bson");
                zmongo.FileBsonReader<TestText>(file).zFindDate(findDateManager).zSave(bsonFile);
                zmongo.FileBsonReader<TestFindDate>(bsonFile).zTraceFindDate();
                Trace.WriteLine("test duration {0}", DateTime.Now - dt);
            }
            finally
            {
                //Trace.CurrentTrace.EnableBaseLog();
                //Trace.CurrentTrace.RemoveTraceFile(traceFile);
                Trace.CurrentTrace.RemoveOnWrite("Test_FindDate_03");
                Trace.CurrentTrace.DisableViewer = false;
            }
        }

        public static IEnumerable<TestFindDate> zFindDateNew(this IEnumerable<TestText> textList, FindDateManager findDateManager)
        {
            foreach (TestText text in textList)
            {
                //FindDate findDate = findDateManager.Find(text.text);
                //TestFindDate testFindDate = new TestFindDate();
                //testFindDate.text = text.text;
                //testFindDate.foundDate = findDate.found;
                //if (findDate.found)
                //{
                //    testFindDate.dateFound = findDate.regexValues.MatchValue.Value;
                //    testFindDate.date = findDate.date;
                //    testFindDate.dateType = findDate.dateType;
                //    testFindDate.remainText = findDate.regexValues.MatchReplace("_");
                //    testFindDate.namedValues = findDate.regexValues.GetValues();
                //}
                //yield return testFindDate;
                yield return FindDateNew(findDateManager, text.text);
            }
        }

        public static TestFindDate FindDateNew(FindDateManager findDateManager, string text)
        {
            //FindDate_old findDate = findDateManager.Find_old(text);
            FindDate findDate = findDateManager.Find(text);
            TestFindDate testFindDate = new TestFindDate();
            testFindDate.text = text;
            testFindDate.foundDate = findDate.found;
            if (findDate.found)
            {
                //testFindDate.dateFound = findDate.regexValues.MatchValue_old.Value;
                testFindDate.dateFound = findDate.matchValues.Match.Value;
                testFindDate.date = findDate.date;
                testFindDate.dateType = findDate.dateType;
                //testFindDate.remainText = findDate.regexValues.MatchReplace_old("_");
                testFindDate.remainText = findDate.matchValues.Replace("_");
                //testFindDate.namedValues = findDate.regexValues.GetValues_old();
                testFindDate.namedValues = findDate.matchValues.GetValues();
            }
            return testFindDate;
        }

        public static IEnumerable<TestFindDate> zFindDate(this IEnumerable<TestText> textList, FindDateManager_v1 findDateManager)
        {
            foreach (TestText text in textList)
            {
                //FindDate_old findDate = findDateManager.Find_old(text.text);
                FindDate findDate = findDateManager.Find(text.text);
                TestFindDate testFindDate = new TestFindDate();
                testFindDate.text = text.text;
                testFindDate.foundDate = findDate.found;
                if (findDate.found)
                {
                    //testFindDate.dateFound = findDate.regexValues.MatchValue_old.Value;
                    testFindDate.dateFound = findDate.matchValues.Match.Value;
                    testFindDate.date = findDate.date;
                    testFindDate.dateType = findDate.dateType;
                    //testFindDate.remainText = findDate.regexValues.MatchReplace_old("_");
                    testFindDate.remainText = findDate.matchValues.Replace("_");
                    //testFindDate.namedValues = findDate.regexValues.GetValues_old();
                    testFindDate.namedValues = findDate.matchValues.GetValues();
                }
                yield return testFindDate;
            }
        }

        public static void zTraceFindDate(this IEnumerable<TestFindDate> testFindDateList)
        {
            int nb = 0;
            int nbDateFound = 0;
            foreach (TestFindDate testFindDate in testFindDateList)
            {
                Trace.WriteLine("search date in  : \"{0}\"", testFindDate.text);
                if (testFindDate.foundDate)
                {
                    Trace.WriteLine("    found date  : \"{0}\"", testFindDate.dateFound);
                    Trace.WriteLine("    remain text : \"{0}\"", testFindDate.remainText);

                    Trace.WriteLine("    date        : {0:dd-MM-yyyy} type {1}", testFindDate.date, testFindDate.dateType);

                    Trace.Write("    values      : ");
                    testFindDate.namedValues.zTrace();
                    Trace.WriteLine();
                    nbDateFound++;
                }
                else
                    Trace.WriteLine("    date not found ");
                Trace.WriteLine();
                nb++;
            }
            Trace.WriteLine();
            Trace.WriteLine();
            Trace.WriteLine("search date in {0} text", nb);
            Trace.WriteLine("found date in {0} text", nbDateFound);
        }

        public static void Test_FindDate_02(FindDateManager_v1 findDateManager, string file)
        {
            file = zPath.Combine(GetDirectoryDate(), file);
            int year = Date.Today.Year;
            findDateManager.DateRegexList.Add("year", new RegexValues("year", "year", string.Format(@"(?:^|[_\s])({0}|{1}|{2})(?:$|[_\s])", year - 1, year, year + 1), "IgnoreCase", "year", compileRegex: true));
            string traceFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out");
            Trace.WriteLine("Test_FindDate \"{0}\" (nb date regex {1})", file, findDateManager.DateRegexList.Count);
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            //Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Test_FindDate_02", WriteToFile.Create(traceFile, FileOption.RazFile).Write);
            try
            {
                DateTime dt = DateTime.Now;
                TraceRegexValuesList(findDateManager.DateRegexList);
                Trace.WriteLine();
                int nb = 0;
                int nbDateFound = 0;
                foreach (BsonDocument document in zmongo.FileBsonReader<BsonDocument>(file))
                {
                    string text = document["text"].AsString;

                    //FindDate_old findDate = FindDate(findDateManager, text);
                    FindDate findDate = FindDate(findDateManager, text);

                    if (findDate.found)
                        nbDateFound++;

                    nb++;
                }
                Trace.WriteLine();
                Trace.WriteLine();
                Trace.WriteLine("search date in {0} text", nb);
                Trace.WriteLine("found date in {0} text", nbDateFound);
                Trace.WriteLine("test duration {0}", DateTime.Now - dt);
            }
            finally
            {
                //Trace.CurrentTrace.EnableBaseLog();
                //Trace.CurrentTrace.RemoveTraceFile(traceFile);
                Trace.CurrentTrace.RemoveOnWrite("Test_FindDate_02");
                Trace.CurrentTrace.DisableViewer = false;
            }
        }

        //public static FindDate_old FindDate(FindDateManager findDateManager, string text)
        public static FindDate FindDate(FindDateManager_v1 findDateManager, string text)
        {
            //FindDate_old findDate = findDateManager.Find_old(text);
            FindDate findDate = findDateManager.Find(text);

            Trace.WriteLine("search date in  : \"{0}\"", text);
            if (findDate.found)
            {
                //Trace.WriteLine("    found date  : \"{0}\"", findDate.regexValues.MatchValue_old);
                Trace.WriteLine("    found date  : \"{0}\"", findDate.matchValues.Match.Value);
                //Trace.WriteLine("    remain text : \"{0}\"", findDate.regexValues.MatchReplace_old("_"));
                Trace.WriteLine("    remain text : \"{0}\"", findDate.matchValues.Replace("_"));

                Trace.WriteLine("    date        : {0:dd-MM-yyyy} type {1}", findDate.date, findDate.dateType);
                //Trace.Write("not found ");

                Trace.Write("    values      : ");
                //findDate.regexValues.GetValues_old().zTrace();
                findDate.matchValues.GetValues().zTrace();
                Trace.WriteLine();
                //nbDateFound++;
            }
            else
                Trace.WriteLine("    date not found ");
            Trace.WriteLine();
            return findDate;
        }

        public static void Test_FindNumber_02(FindNumberManager findNumberManager, string file)
        {
            file = zPath.Combine(GetDirectoryNumber(), file);
            string traceFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out");
            Trace.WriteLine("Test_FindNumber \"{0}\" (nb number regex {1})", file, findNumberManager.NumberRegexList.Count);
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            //Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Test_FindNumber_02", WriteToFile.Create(traceFile, FileOption.RazFile).Write);
            try
            {
                TraceRegexValuesList(findNumberManager.NumberRegexList);
                Trace.WriteLine();
                foreach (BsonDocument document in zmongo.FileBsonReader<BsonDocument>(file))
                {
                    string title = document["title"].AsString;
                    //FindNumber_old findNumber = findNumberManager.Find_old(title);
                    FindNumber findNumber = findNumberManager.Find(title);

                    Trace.WriteLine("search number in   : \"{0}\"", title);
                    if (findNumber.found)
                    {
                        //Trace.WriteLine("    found number : \"{0}\"", findNumber.regexValues.MatchValue_old);
                        Trace.WriteLine("    found number : \"{0}\"", findNumber.matchValues.Match.Value);
                        //Trace.WriteLine("    remain text  : \"{0}\"", findNumber.regexValues.MatchReplace_old("_"));
                        Trace.WriteLine("    remain text  : \"{0}\"", findNumber.matchValues.Replace("_"));

                        Trace.WriteLine("    number       : {0}", findNumber.number);
                        //Trace.Write("not found ");

                        Trace.Write("    values      : ");
                        //findNumber.regexValues.GetValues_old().zTrace();
                        findNumber.matchValues.GetValues().zTrace();
                        Trace.WriteLine();
                    }
                    else
                        Trace.WriteLine("    number not found ");
                    Trace.WriteLine();
                }
            }
            finally
            {
                //Trace.CurrentTrace.EnableBaseLog();
                //Trace.CurrentTrace.RemoveTraceFile(traceFile);
                Trace.CurrentTrace.RemoveOnWrite("Test_FindNumber_02");
                Trace.CurrentTrace.DisableViewer = false;
            }
        }

        public static void Test_FindDate_01(RegexValuesList dateRegexList, string file)
        {
            file = zPath.Combine(GetDirectoryDate(), file);
            string traceFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out");
            Trace.WriteLine("Test_FindDate \"{0}\" (nb date regex {1})", file, dateRegexList.Count);
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            //Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Test_FindDate_01", WriteToFile.Create(traceFile, FileOption.RazFile).Write);
            try
            {
                TraceRegexValuesList(dateRegexList);
                Trace.WriteLine();
                foreach (BsonDocument document in zmongo.FileBsonReader<BsonDocument>(file))
                    FindDate(dateRegexList, document["text"].AsString);
            }
            finally
            {
                //Trace.CurrentTrace.EnableBaseLog();
                //Trace.CurrentTrace.RemoveTraceFile(traceFile);
                Trace.CurrentTrace.RemoveOnWrite("Test_FindDate_01");
                Trace.CurrentTrace.DisableViewer = false;
            }
        }

        private static void FindDate(RegexValuesList dateRegexList, string text)
        {
            bool found = false;
            Trace.WriteLine("search date in  : \"{0}\"", text);
            foreach (RegexValues rv in dateRegexList.Values)
            {
                //rv.Match_old(text);
                MatchValues matchValues = rv.Match(text);
                //if (rv.Success_old)
                if (matchValues.Success)
                {
                    //NamedValues<ZValue> values = rv.GetValues_old();
                    NamedValues<ZValue> values = matchValues.GetValues();
                    Date date;
                    DateType dateType;

                    //Trace.WriteLine("    found date  : \"{0}\"", rv.MatchValue_old);
                    Trace.WriteLine("    found date  : \"{0}\"", matchValues.Match.Value);
                    //Trace.WriteLine("    remain text : \"{0}\"", rv.MatchReplace_old("_"));
                    Trace.WriteLine("    remain text : \"{0}\"", matchValues.Replace("_"));

                    Trace.Write("    date        : ");
                    found = zdate.TryCreateDate(values, out date, out dateType);
                    if (found)
                        Trace.Write("{0:dd-MM-yyyy} type {1}", date, dateType);
                    else
                        Trace.Write("not found ");
                    Trace.WriteLine();

                    Trace.Write("    values      : ");
                    values.zTrace();
                    Trace.WriteLine();

                    if (found)
                        break;
                }
            }
            if (!found)
                Trace.WriteLine("    date not found ");
            Trace.WriteLine();
        }

        private static void TraceRegexValuesList(RegexValuesList regexList)
        {
            int i = 1;
            foreach (RegexValues regex in regexList.Values)
            {
                Trace.WriteLine("pattern {0} \"{1}\"", i++, regex.Pattern);
            }
        }

        private static string GetDirectoryDate()
        {
            return zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("TestUnitDirectory"), @"Print\RegexValues\Date");
        }

        private static string GetDirectoryNumber()
        {
            return zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("TestUnitDirectory"), @"Print\RegexValues\Number");
        }
    }
}
