using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using System.Collections.Generic;

namespace Download.Print.Test
{
    public class TestPrint_v2
    {
        public string Title;
        public PrintType PrintType;
        public string Category;
    }

    public class TestFindPrint_v2
    {
        public TestPrint_v2 Print;
        public FindPrintInfo FindPrintInfo;
    }

    public static class Test_Unit_FindPrint_v2
    {
        public static void Test_FindPrint(string file, string parameters = null, int version = 6)
        {
            Trace.WriteLine("Test_FindPrint \"{0}\"", file);
            FindPrintManager findPrintManager = CreateFindPrintManager(parameters, version);
            Trace.WriteLine("  FindPrintList.Count  : {0}", findPrintManager.FindPrintList.Count);
            file = zPath.Combine(GetDirectory(), file);
            string bsonFile = zpath.PathSetFileNameWithoutExtension(file, zPath.GetFileNameWithoutExtension(file) + "_out_bson");
            zmongo.FileReader<TestPrint_v2>(file).zFindPrint(findPrintManager).zSave(bsonFile);
            //WriteAllFindPrint(bsonFile);
        }

        public static IEnumerable<TestFindPrint_v2> zFindPrint(this IEnumerable<TestPrint_v2> prints, FindPrintManager findPrintManager)
        {
            foreach (TestPrint_v2 print in prints)
            {
                yield return new TestFindPrint_v2 { Print = print, FindPrintInfo = findPrintManager.Find(print.Title, print.PrintType) };
            }
        }

        private static FindPrintManager CreateFindPrintManager(string parameters, int version = 6)
        {
            // parameters : bool DailyPrintManager, int GapDayBefore, int GapDayAfter
            LoadConfig();
            //return FindPrintManagerCreator.Create(parameters: WebData.ParseParameters(parameters), version: version);
            return WebData.CreateFindPrintManager(parameters, version);
        }

        public static void LoadConfig()
        {
            XmlConfig.CurrentConfig = new XmlConfig(@"$ProjectRoot$\Source\Print\Project\download.config.xml".zGetRunSourceProjectVariableValue().zRootPath(RunSource.CurrentRunSource.ProjectDirectory));
        }


        private static string GetDirectory()
        {
            return zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("TestUnitDirectory"), @"Print\FindPrint");
        }
    }
}
