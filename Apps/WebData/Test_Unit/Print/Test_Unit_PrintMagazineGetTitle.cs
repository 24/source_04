using System;
using System.Collections.Generic;
using System.IO;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using Print;

namespace Test.Test_Unit.Print
{
    public class TestPrintTitle
    {
        public string title;
        public string category;
        public string title1;
        public string title2;
    }

    public static class Test_Unit_PrintMagazineGetTitle
    {
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

        public static void Test_PrintMagazineGetTitle_01(PrintTitleManager printTitleManager, string file)
        {
            Trace.WriteLine("Test_PrintMagazineGetTitle_01 \"{0}\"", file);
            file = Path.Combine(GetDirectory(), file);
            string bsonFile = zpath.PathSetFileNameWithoutExtension(file, Path.GetFileNameWithoutExtension(file) + "_out_bson");
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            StreamWriter sw = null;
            BsonWriter bsonWriter = null;
            Dictionary<string, string> prints = GetPrintList();

            try
            {
                sw = File.CreateText(bsonFile);
                JsonWriterSettings settings = new JsonWriterSettings();
                settings.Indent = true;
                bsonWriter = JsonWriter.Create(sw, settings);
                foreach (BsonDocument document in zmongo.BsonReader<BsonDocument>(file))
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
                    bsonWriter.zWrite("file", titleInfo.file);
                    bsonWriter.zWrite("titleStructure", titleInfo.titleStructure);
                    bsonWriter.zWrite("title", titleInfo.title);
                    bsonWriter.zWrite("formatedTitle", titleInfo.formatedTitle);
                    bsonWriter.zWrite("name", titleInfo.name);
                    if (prints.ContainsKey(titleInfo.name))
                        bsonWriter.zWrite("print", titleInfo.name);
                    else
                        bsonWriter.zWrite("print", (string)null);
                    bsonWriter.zWrite("remainText", titleInfo.remainText);
                    bsonWriter.zWrite("special", titleInfo.special);
                    bsonWriter.zWrite("specialText", titleInfo.specialText);
                    bsonWriter.zWrite("number", titleInfo.number);
                    bsonWriter.zWrite("date", titleInfo.date);
                    bsonWriter.zWrite("dateType", titleInfo.dateType.ToString());
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
            file = Path.Combine(GetDirectory(), file);
            string splitFile = zpath.PathSetFileNameWithoutExtension(file, Path.GetFileNameWithoutExtension(file) + "_split_bson");
            zmongo.BsonReader<TestPrintTitle>(file).zSplitTitle().zSave(splitFile);
            zmongo.BsonReader<TestPrintTitle>(splitFile).zView();
        }

        public static IEnumerable<TestPrintTitle> zSplitTitle(this IEnumerable<TestPrintTitle> prints)
        {
            foreach (TestPrintTitle print in prints)
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

        private static string GetDirectory()
        {
            return Path.Combine(XmlConfig.CurrentConfig.GetExplicit("TestUnitDirectory"), @"Print\MagazineTitle");
        }
    }
}
