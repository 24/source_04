﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using pb;
using pb.Compiler;
using pb.Data;
using pb.Data.Mongo;
using pb.IO;
using pb.Linq;
using pb.Text;
using pb.Data.Xml;

// Test_Unit_FindPrint directory c:\pib\drive\google\dev_data\exe\runsource\test_unit\Print\FindPrint


//namespace Print.Test
namespace Download.Print.Test
{
    public class TestPrint
    {
        public string title;
        public string category;
        public PrintType postType;
        public bool isPrint;
        public NamedValues<ZValue> infos;
    }

    public class TestFindPrint
    {
        public string post_title;
        public string post_category;
        public bool post_isPrint;
        //public PostType post_postType;
        public string findPrint_file;
        public FindPrintType findPrint_type;
        public string findPrint_name;
        public string findPrint_title;
        public string print_name;
        public string print_title;
        public string titleInfo_formatedTitle;
        public Date? findPrint_date;
        public DateType findPrint_dateType = DateType.Unknow;
        //public string findPrint_dateCapture;
        //public NamedValues<ZValue> findPrint_dateValues;
        public RegexCaptureValues findPrint_dateCapture;
        public RegexCaptureValues[] findPrint_dateOtherCaptureList;
        public int? findPrint_number;
        //public string findPrint_numberCapture;
        //public NamedValues<ZValue> findPrint_numberValues;
        public RegexCaptureValues findPrint_numberCapture;
        public bool findPrint_special = false;
        //public string findPrint_specialCapture;
        //public NamedValues<ZValue> findPrint_specialValues;
        public RegexCaptureValues findPrint_specialCapture;
        public string findPrint_specialText;
        public string findPrint_remainText;
        public string[] warnings;
    }

    public static class Test_Unit_FindPrint
    {
        //public static void Test_FindPrint(DownloadAutomateManager_v1 downloadAutomate, string file)
        public static void Test_FindPrint(DownloadAutomateManager downloadAutomate, string file)
        {
            Trace.WriteLine("Test_FindPrint \"{0}\"", file);
            if (downloadAutomate.FindPrintManager != null)
                Trace.WriteLine("  FindPrintManager2.PrintRegexList  : {0}", downloadAutomate.FindPrintManager.FindPrintList.Count);
            file = zPath.Combine(GetDirectory(), file);
            string bsonFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_bson");
            zmongo.FileReader<TestPrint>(file).zFindPrint(downloadAutomate).zSave(bsonFile);
            WriteAllFindPrint(bsonFile);
        }

        public static void Test_OneFindPrint(DownloadAutomateManager downloadAutomate, string title, string category = null, PrintType postType = PrintType.Unknow)
        {
            TestFindPrint testFindPrint = FindPrint(downloadAutomate, new TestPrint { title = title, category = category, postType = postType });
            Trace.WriteLine(testFindPrint.zToJson());
        }

        public static void Test_FindPrintFromMongo(DownloadAutomateManager downloadAutomate, string printName, string query, int limit = 0, string sort = null)
        {
            Trace.WriteLine("regex {0} : \"{1}\"", printName, downloadAutomate.FindPrintManager.FindPrintList[printName].Pattern);
            string file = zPath.Combine(GetDirectory(), printName + ".txt");
            string bsonFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_bson");
            FindPrintFromMongo(downloadAutomate, query, limit, sort).zFindPrint(downloadAutomate).zSave(bsonFile);
            WriteAllFindPrint(bsonFile);
            zmongo.FileReader<TestFindPrint>(bsonFile).zView();
        }

        public static void Test_Compare(string file1, string file2, string resultFile, IEnumerable<string> elementsToCompare = null, BsonDocumentComparatorOptions options = BsonDocumentComparatorOptions.ReturnNotEqualDocuments)
        {
            //string file1 = @"c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\FindPrint_out_bson.txt";
            //string file2 = @"c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\_archive\FindPrint_v1_SelectPost_02\FindPrint_out_bson.txt";
            //string resultFile = @"c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\test_compare.txt";
            file1 = zPath.Combine(GetDirectory(), file1);
            file2 = zPath.Combine(GetDirectory(), file2);
            if (elementsToCompare == null)
                elementsToCompare = new string[] { "findPrint_file" };
            resultFile = zPath.Combine(GetDirectory(), resultFile);
            BsonDocumentComparator.CompareBsonDocumentFilesWithKey(file1, file2, "post_title", "post_title", joinType: pb.Linq.JoinType.InnerJoin, elementsToCompare: elementsToCompare, comparatorOptions: options)
                .Select(result => result.GetResultDocument()).zSave(resultFile);
            RunSource.CurrentRunSource.SetResult(zmongo.FileReader<BsonDocument>(resultFile)
                .Select(doc => new BsonDocument { { "result", doc["result"] } }).zToDataTable2_old());
        }

        public static void Test_Correction(string file, string correctionFile, string correctedFile)
        {
            file = zPath.Combine(GetDirectory(), file);
            correctionFile = zPath.Combine(GetDirectory(), correctionFile);
            correctedFile = zPath.Combine(GetDirectory(), correctedFile);

            //string newFile1 = zpath.PathSetFile(file, zPath.GetFileNameWithoutExtension(file) + "_new1");
            //zmongo.BsonReader<TestFindPrint>(file).zJoin(zmongo.BsonReader<TestFindPrint>(correctionFile), tfp => tfp.post_title, tfp => tfp.post_title, (tfp1, tfp2) => tfp1,
            //    JoinType.LeftOuterJoinWithoutInner).zSave(newFile1);
            //Trace.WriteLine("\"{0}\" count {1}", file, zmongo.BsonReader<TestFindPrint>(file).Count());
            //Trace.WriteLine("\"{0}\" count {1}", correctionFile, zmongo.BsonReader<TestFindPrint>(correctionFile).Count());
            //Trace.WriteLine("\"{0}\" count {1}", newFile1, zmongo.BsonReader<TestFindPrint>(newFile1).Count());
            //Trace.WriteLine();

            zmongo.FileReader<TestFindPrint>(file).zJoin(zmongo.FileReader<TestFindPrint>(correctionFile), tfp => tfp.post_title, tfp => tfp.post_title, (tfp1, tfp2) => tfp1,
                JoinType.LeftOuterJoinWithoutInner).Union(zmongo.FileReader<TestFindPrint>(correctionFile)).zSave(correctedFile);
            Trace.WriteLine("\"{0}\" count {1}", file, zmongo.FileReader<TestFindPrint>(file).Count());
            Trace.WriteLine("\"{0}\" count {1}", correctionFile, zmongo.FileReader<TestFindPrint>(correctionFile).Count());
            Trace.WriteLine("\"{0}\" count {1}", correctedFile, zmongo.FileReader<TestFindPrint>(correctedFile).Count());
        }

        public static void Test_ViewDateCapture_01(string bsonFile)
        {
            bsonFile = zPath.Combine(GetDirectory(), bsonFile);
            //var query = from tfp in zmongo.BsonReader<TestFindPrint>(bsonFile)
            //            where tfp.findPrint_date != null
            //            //orderby tfp.findPrint_dateCapture != null ? tfp.findPrint_dateCapture.capture : null
            //            orderby tfp.findPrint_dateCapture
            //            select new
            //            {
            //                post_title = tfp.post_title,
            //                post_category = tfp.post_category,
            //                findPrint_date = tfp.findPrint_date,
            //                findPrint_dateType = tfp.findPrint_dateType,
            //                //findPrint_dateCapture = tfp.findPrint_dateCapture != null ? tfp.findPrint_dateCapture.capture : null
            //                findPrint_dateCapture = tfp.findPrint_dateCapture
            //            };
            var query = from tfp2 in
                        from tfp in zmongo.FileReader<TestFindPrint>(bsonFile)
                        where tfp.findPrint_date != null
                        orderby tfp.findPrint_dateCapture != null ? tfp.findPrint_dateCapture.capture : null
                        select new
                        {
                            title = tfp.post_title,
                            category = tfp.post_category,
                            date = tfp.findPrint_date,
                            dateType = tfp.findPrint_dateType,
                            dateCapture = tfp.findPrint_dateCapture != null ? tfp.findPrint_dateCapture.capture : null
                        }
                        //select tfp2
                        select new
                        {
                            title = tfp2.title,
                            category = tfp2.category,
                            date = tfp2.date,
                            dateType = tfp2.dateType,
                            dateCapture = tfp2.dateCapture,
                            //dateCapture2 = ModifyDateCapture(tfp2.dateCapture)
                            dateCaptureContainDash = DateCaptureContainDash(tfp2.dateCapture)
                        }
                            ;
            query.zView();
        }

        public static void Test_ViewDateCapture_02(string bsonFile)
        {
            bsonFile = zPath.Combine(GetDirectory(), bsonFile);
            var query = from tfp in zmongo.FileReader<TestFindPrint>(bsonFile)
                            where tfp.findPrint_date == null
                            select new
                            {
                                title = tfp.post_title,
                                category = tfp.post_category
                            }
                            ;
            query.zView();
        }

        public static bool DateCaptureContainDash(string dateCapture)
        {
            return dateCapture.TrimStart(' ', '-').Contains('-');
        }

        public static string ModifyDateCapture(string dateCapture)
        {
            return null;
        }

        public static IEnumerable<TestPrint> FindPrintFromMongo(DownloadAutomateManager downloadAutomate, string query, int limit = 0, string sort = null)
        {
            if (sort == null)
                sort = "{ 'download.title': 1 }";
            //var query2 = pb.Data.Mongo.MongoCommand.Find("dl", "RapideDdl_Detail2", query, limit: limit, sort: sort, fields: "{ '_id': 1, 'download.title': 1, 'download.creationDate': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");
            var query2 = MongoCommand.Find("dl", "RapideDdl_Detail2", query, limit: limit, sort: sort, fields: "{ '_id': 1, 'download.title': 1, 'download.creationDate': 1, 'download.category': 1, 'download.postType': 1, 'download.sourceUrl': 1  }");
            var query3 = query2.Select(document =>
                new TestPrint
                    {
                        title = document["download"]["title"].AsString,
                        category = document["download"]["category"].AsString,
                        //isPrint = document["download"]["isPrint"].AsBoolean
                        postType = (PrintType)document["download"]["postType"].AsInt32
                    }
            );
            return query3;
        }

        public static IEnumerable<TestFindPrint> zFindPrint(this IEnumerable<TestPrint> prints, DownloadAutomateManager downloadAutomate)
        {
            try
            {
                ActiveTraceCatch();
                foreach (TestPrint print in prints)
                {
                    yield return FindPrint(downloadAutomate, print);
                }
            }
            finally
            {
                DesactiveTraceCatch();
            }
        }

        //public static TestFindPrint FindPrint(DownloadAutomateManager_v1 downloadAutomate, TestPrint print)
        public static TestFindPrint FindPrint(DownloadAutomateManager downloadAutomate, TestPrint print)
        {
            //FindPrint findPrint = null;
            //if (downloadAutomate.FindPrintManager != null)
            //    findPrint = downloadAutomate.FindPrint(print.title, print.category);
            //else if (downloadAutomate.FindPrintManager_new != null)
            //    findPrint = downloadAutomate.FindPrint_new(print.title, print.postType);
            FindPrintInfo findPrint = downloadAutomate.FindPrint(print.title, print.postType);
            TestFindPrint testFindPrint = new TestFindPrint();
            testFindPrint.post_title = print.title;
            testFindPrint.post_category = print.category;
            testFindPrint.post_isPrint = print.postType == PrintType.Print;
            //testFindPrint.post_postType = print.postType;

            testFindPrint.findPrint_file = findPrint.file;
            testFindPrint.findPrint_type = findPrint.findPrintType;
            testFindPrint.findPrint_name = findPrint.name;
            testFindPrint.findPrint_title = findPrint.title;
            testFindPrint.print_name = findPrint.print != null ? findPrint.print.Name : null;
            testFindPrint.print_title = findPrint.print != null ? findPrint.print.Title : null;
            testFindPrint.titleInfo_formatedTitle = findPrint.titleInfo != null ? findPrint.titleInfo.FormatedTitle : null;

            testFindPrint.findPrint_date = findPrint.date;
            testFindPrint.findPrint_dateType = findPrint.dateType;
            if (findPrint.titleInfo != null)
            {
                testFindPrint.findPrint_dateCapture = RegexCaptureValues.CreateRegexCaptureValues(findPrint.titleInfo.DateMatch, allValues: true);
                //testFindPrint.findPrint_dateOtherCaptureList = RegexCaptureValues.CreateRegexCaptureValuesList(findPrint.titleInfo.DateOtherMatchList, allValues: true);
            }
            testFindPrint.findPrint_number = findPrint.number;
            if (findPrint.titleInfo != null)
                testFindPrint.findPrint_numberCapture = RegexCaptureValues.CreateRegexCaptureValues(findPrint.titleInfo.NumberMatch, allValues: true);
            testFindPrint.findPrint_special = findPrint.special;
            if (findPrint.titleInfo != null)
                testFindPrint.findPrint_specialCapture = RegexCaptureValues.CreateRegexCaptureValues(findPrint.titleInfo.SpecialMatch, allValues: true);
            testFindPrint.findPrint_specialText = findPrint.specialText;

            testFindPrint.findPrint_remainText = findPrint.remainText;
            testFindPrint.warnings = __traceMessages.ToArray();
            __traceMessages.Clear();

            return testFindPrint;
        }

        private static Regex __rgSpecial = new Regex(@"hors[-\s]*s.rie", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static void WriteAllFindPrint(string bsonFile)
        {
            string filename = zPath.GetFileNameWithoutExtension(bsonFile);
            if (filename.EndsWith("_bson"))
                filename = filename.Substring(0, filename.Length - 5);
            WriteFindPrint(zpath.PathSetFileNameWithoutExtension(bsonFile, filename), zmongo.FileReader<TestFindPrint>(bsonFile), old: true);
            WriteFindPrint(zpath.PathSetFileNameWithoutExtension(bsonFile, filename + "_new"), zmongo.FileReader<TestFindPrint>(bsonFile));
            WriteFindPrint(zpath.PathSetFileNameWithoutExtension(bsonFile, filename + "_hors_serie"), from tfp in zmongo.FileReader<TestFindPrint>(bsonFile) where __rgSpecial.IsMatch(tfp.post_title) select tfp);
            WriteFindPrint(zpath.PathSetFileNameWithoutExtension(bsonFile, filename + "_remain_text"), from tfp in zmongo.FileReader<TestFindPrint>(bsonFile) where !string.IsNullOrEmpty(tfp.findPrint_remainText) select tfp);
            WriteFindPrint(zpath.PathSetFileNameWithoutExtension(bsonFile, filename + "_warning"), from tfp in zmongo.FileReader<TestFindPrint>(bsonFile) where tfp.warnings.Length > 0 select tfp);
            WriteFindPrint(zpath.PathSetFileNameWithoutExtension(bsonFile, filename + "_no_title"), from tfp in zmongo.FileReader<TestFindPrint>(bsonFile) where tfp.findPrint_file != null && tfp.findPrint_title == null select tfp);
            WriteFindPrint_NotSelected(zpath.PathSetFileNameWithoutExtension(bsonFile, filename + "_not_selected"), from tfp in zmongo.FileReader<TestFindPrint>(bsonFile) where tfp.findPrint_file == null select tfp);
        }

        private static void WriteFindPrint(string file, IEnumerable<TestFindPrint> testFindPrintList, bool old = false)
        {
            using (StreamWriter sw = zFile.CreateText(file))
            {
                foreach (TestFindPrint testFindPrint in testFindPrintList)
                {
                    sw.WriteLine("post               : \"{0}\"", testFindPrint.post_title);
                    sw.WriteLine("    category       : \"{0}\"", testFindPrint.post_category);
                    sw.WriteLine("    file           : \"{0}\"", testFindPrint.findPrint_file);
                    if (!old)
                    {
                        sw.WriteLine("    findPrintType  : \"{0}\"", testFindPrint.findPrint_type);
                        sw.WriteLine("    name           : \"{0}\"", testFindPrint.findPrint_name);
                    }
                    sw.WriteLine("    print          : \"{0}\"", testFindPrint.print_name);
                    sw.WriteLine("    title          : \"{0}\"", testFindPrint.print_title);
                    sw.WriteLine("    remain text    : \"{0}\"", testFindPrint.findPrint_remainText);
                    foreach (string warning in testFindPrint.warnings)
                        sw.WriteLine("    warning     : \"{0}\"", warning);
                    sw.WriteLine();
                }
            }
        }

        private static void WriteFindPrint_NotSelected(string file, IEnumerable<TestFindPrint> testFindPrintList)
        {
            using (StreamWriter sw = zFile.CreateText(file))
            {
                foreach (TestFindPrint testFindPrint in from tfp in testFindPrintList orderby tfp.post_category, tfp.post_title select tfp)
                {
                    sw.WriteLine("post            : {0,-25} \"{1}\"", testFindPrint.post_category, testFindPrint.post_title);
                }
            }
        }

        private static List<string> __traceMessages = new List<string>();
        public static void ActiveTraceCatch()
        {
            Trace.CurrentTrace.DisableViewer = true;
            Trace.CurrentTrace.AddOnWrite("Test_Unit_FindPrint", msg => __traceMessages.Add(msg.TrimEnd('\r', '\n')));
        }

        public static void DesactiveTraceCatch()
        {
            Trace.CurrentTrace.RemoveOnWrite("Test_Unit_FindPrint");
            Trace.CurrentTrace.DisableViewer = false;
        }

        private static string GetDirectory()
        {
            return zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("TestUnitDirectory"), @"Print\FindPrint");
        }
    }
}
