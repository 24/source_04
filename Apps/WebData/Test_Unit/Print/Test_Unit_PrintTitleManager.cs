using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using Print;
using MongoDB.Bson.Serialization;

namespace Test.Test_Unit.Print
{
    public class TestPrintTitle
    {
        public string Title;
        public string PrintType;
    }

    public class TestPrintTitleSplit
    {
        public string title;
        public string category;
        public string title1;
        public string title2;
    }

    public static class Test_Unit_PrintTitleManager
    {
        private static string __subDirectory = @"Print\PrintTitle";

        public static Dictionary<string, string> GetPrintList()
        {
            Dictionary<string, string> prints = new Dictionary<string, string>();
            foreach (string name in XmlConfig.CurrentConfig.GetValues("FindPrints/Prints/Print/@name"))
            {
                if (!prints.ContainsKey(name))
                    prints.Add(name, null);
            }
            return prints;
        }

        public static void Test_ExportTitle_TelechargerMagazine_01()
        {
            string file = "PrintTitle_TelechargerMagazine.txt";
            TraceMongoCommand.Export("dl", "TelechargerMagazine_Detail", GetFile(file),
              //query: "{ $or: [ { 'download.category': 'Ebooks/Journaux' }, { 'download.category': 'Ebooks/Magazine' } ] }",
              query: "{ 'download.PrintType': 'Print' }",
              fields: "{ '_id': 0 'download.Title': 1, 'download.PrintType': 1 }",
              limit: 1000,
              sort: "{ _id: -1 }",
              transformDocument: doc => new MongoDB.Bson.BsonDocument { { "Title", doc.zGet("download.Title") }, { "PrintType", doc.zGet("download.PrintType") } });
        }

        public static void Test_ExportTitle_Vosbooks_01()
        {
            string file = "PrintTitle_Vosbooks.txt";
            TraceMongoCommand.Export("dl", "Vosbooks_Detail", GetFile(file),
              //query: "{ 'download.PrintType': { $ne: 'Comics' } }",
              query: "{ 'download.PrintType': 'Print' }",
              fields: "{ '_id': 0 'download.Title': 1, 'download.PrintType': 1 }",
              limit: 1000,
              sort: "{ 'download.PostCreationDate': -1 }",
              transformDocument: doc => new MongoDB.Bson.BsonDocument { { "Title", doc.zGet("download.Title") }, { "PrintType", doc.zGet("download.PrintType") } });
        }

        public static void Test_ExportTitle_Ebookdz_01()
        {
            string file = "PrintTitle_Ebookdz.txt";
            TraceMongoCommand.Export("dl", "Ebookdz_Detail", GetFile(file),
              //query: "{ 'download.PrintType': { $ne: 'Comics' } }",
              query: "{ 'download.PrintType': 'Print' }",
              fields: "{ '_id': 0 'download.Title': 1, 'download.PrintType': 1 }",
              limit: 1000,
              sort: "{ 'download.PostCreationDate': -1 }",
              transformDocument: doc => new MongoDB.Bson.BsonDocument { { "Title", doc.zGet("download.Title") }, { "PrintType", doc.zGet("download.PrintType") } });
        }

        public static void Test_GetPrintTitleInfo_01(PrintTitleManager printTitleManager, string file, int? version = null)
        {
            Trace.WriteLine("Test_PrintMagazineGetTitle_01 \"{0}\"", file);
            file = GetFile(file);
            string suffix = "_out_bson";
            if (version != null)
                suffix = "_v" + version.ToString() + suffix;
            string bsonFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + suffix);
            Trace.WriteLine("  output to \"{0}\"", bsonFile);
            //zmongo.FileReader<TestPrintTitle>(file).Select(printTitle => new { Title = printTitle.Title, PrintType = printTitle.PrintType, PrintTitleInfo = printTitleManager.GetPrintTitleInfo(printTitle.Title) }).zSave(bsonFile);
            zmongo.FileReader<TestPrintTitle>(file)
                .Select(printTitle => new { Title = printTitle.Title, PrintType = printTitle.PrintType, PrintTitleInfo = printTitleManager.GetPrintTitleInfo(printTitle.Title) })
                .Select(printTitle => { BsonDocument document = printTitle.ToBsonDocument(); document.zSet("PrintTitleInfo.File", ".06_unknow_print\\" + PrintTitleManager.GetFile(printTitle.PrintTitleInfo)); return document; })
                .zSave(bsonFile);
        }

        public static void Test_PrintMagazineGetTitle_01_old(PrintTitleManager printTitleManager, string file)
        {
            Trace.WriteLine("Test_PrintMagazineGetTitle_01 \"{0}\"", file);
            file = GetFile(file);
            string bsonFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_bson");
            Trace.WriteLine("  output to \"{0}\"", bsonFile);
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            StreamWriter sw = null;
            BsonWriter bsonWriter = null;
            Dictionary<string, string> prints = GetPrintList();

            try
            {
                sw = zFile.CreateText(bsonFile);
                JsonWriterSettings settings = new JsonWriterSettings();
                settings.Indent = true;
                bsonWriter = JsonWriter.Create(sw, settings);
                foreach (BsonDocument document in zmongo.FileReader<BsonDocument>(file))
                {
                    string category = document["category"].AsString;
                    string title = document["title"].AsString;
                    Trace.WriteLine("{0,-25} - \"{1}\"", category, title);
                    PrintTitleInfo titleInfo = printTitleManager.GetPrintTitleInfo(title);
                    //FindPrint findPrint = downloadAutomate.SelectPost(title);
                    //Trace.WriteLine("post            : \"{0}\"", title);
                    //Trace.WriteLine("    file        : \"{0}\"", findPrint.file);
                    //Trace.WriteLine("    remain text : \"{0}\"", findPrint.found ? findPrint.remainText : null);
                    //Trace.WriteLine();
                    bsonWriter.WriteStartDocument();
                    bsonWriter.zWrite("category", category);
                    bsonWriter.zWrite("originalTitle", title);
                    //bsonWriter.zWrite("file", titleInfo.File);
                    bsonWriter.zWrite("titleStructure", titleInfo.TitleStructure);
                    bsonWriter.zWrite("title", titleInfo.Title);
                    bsonWriter.zWrite("formatedTitle", titleInfo.FormatedTitle);
                    bsonWriter.zWrite("name", titleInfo.Name);
                    if (prints.ContainsKey(titleInfo.Name))
                        bsonWriter.zWrite("print", titleInfo.Name);
                    else
                        bsonWriter.zWrite("print", (string)null);
                    bsonWriter.zWrite("remainText", titleInfo.RemainText);
                    bsonWriter.zWrite("special", titleInfo.Special);
                    bsonWriter.zWrite("specialText", titleInfo.SpecialText);
                    bsonWriter.zWrite("number", titleInfo.Number);
                    bsonWriter.zWrite("date", titleInfo.Date);
                    bsonWriter.zWrite("dateType", titleInfo.DateType.ToString());
                    bsonWriter.WriteEndDocument();
                    sw.WriteLine();
                    bsonWriter.WriteName("fake");    // ??? pour éviter l'erreur : WriteString can only be called when State is Value or Initial, not when State is Name (System.InvalidOperationException)
                }
            }
            finally
            {
                //Trace.CurrentTrace.EnableBaseLog();
                Trace.CurrentTrace.DisableViewer = false;
                if (bsonWriter != null)
                    bsonWriter.Close();
                if (sw != null)
                    sw.Close();
            }
        }

        public static void Test_SplitTitle_01(string file)
        {
            file = GetFile(file);
            string splitFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_split_bson");
            zmongo.FileReader<TestPrintTitleSplit>(file).zSplitTitle().zSave(splitFile);
            zmongo.FileReader<TestPrintTitleSplit>(splitFile).zView();
        }

        public static IEnumerable<TestPrintTitleSplit> zSplitTitle(this IEnumerable<TestPrintTitleSplit> prints)
        {
            foreach (TestPrintTitleSplit print in prints)
            {
                string title = print.title;
                int i = title.IndexOf("- ");
                if (i != -1)
                {
                    print.title1 = title.Substring(0, i);
                    print.title2 = title.Substring(i + 2);
                }
                else
                {
                    i = title.IndexOf(" du ", StringComparison.InvariantCultureIgnoreCase);
                    if (i != -1)
                    {
                        print.title1 = title.Substring(0, i);
                        print.title2 = title.Substring(i + 4);
                    }
                }
                yield return print;
            }

        }

        public static string GetFile(string file)
        {
            return zPath.Combine(GetDirectory(), file); ;
        }

        private static string GetDirectory()
        {
            return zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("TestUnitDirectory"), __subDirectory);
        }
    }
}
