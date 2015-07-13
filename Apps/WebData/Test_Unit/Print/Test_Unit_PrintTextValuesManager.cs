using System;
using System.IO;
using MongoDB.Bson;
using pb;
using pb.Data.Mongo;
using Print;
using Download.Print;
using pb.Compiler;
using pb.Data.Xml;

namespace Test.Test_Unit.Print
{
    public static class Test_Unit_PrintTextValuesManager
    {
        public static void Test_01(string file)
        {
            file = Path.Combine(GetDirectory(), file);
            foreach (BsonDocument doc in zmongo.BsonReader<BsonDocument>(file))
            {
                PrintTextValues_v1 textValues = DownloadPrint.PrintTextValuesManager.GetTextValues_v1(doc.zGet("Texts").zAsBsonArray().zAsStrings(), doc.zGet("Title").zAsString());
                Trace.WriteLine("Texts :");
                Trace.WriteLine(doc.zGet("Texts").zToJson());
                Trace.WriteLine("PrintTextValues :");
                Trace.WriteLine(textValues.zToJson());
                Trace.WriteLine();
            }
        }

        public static void Test_02(string file)
        {
            file = Path.Combine(GetDirectory(), file);
            foreach (BsonDocument doc in zmongo.BsonReader<BsonDocument>(file))
            {
                PrintTextValues textValues = DownloadPrint.PrintTextValuesManager.GetTextValues(doc.zGet("Texts").zAsBsonArray().zAsStrings(), doc.zGet("Title").zAsString());
                Trace.WriteLine("Texts :");
                Trace.WriteLine(doc.zGet("Texts").zToJson());
                Trace.WriteLine("PrintTextValues :");
                Trace.WriteLine(textValues.zToJson());
                Trace.WriteLine();
            }
        }

        private static string GetDirectory()
        {
            return Path.Combine(XmlConfig.CurrentConfig.GetExplicit("TestUnitDirectory"), @"Print\PrintTextValues");
        }
    }
}
