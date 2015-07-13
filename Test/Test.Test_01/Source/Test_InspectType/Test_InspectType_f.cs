using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;
using pb.old;

namespace Test_InspectType
{
    public class test_01
    {
        public string name = null;
        public int number = 0;
    }

    static partial class w
    {
        private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _wr = RunSource.CurrentRunSource;
        private static HtmlXmlReader _hxr = HtmlXmlReader.CurrentHtmlXmlReader;

        public static void Init()
        {
            XmlConfig.CurrentConfig = new XmlConfig("test");
            //string trace = XmlConfig.CurrentConfig.Get("Trace").zRootPath(zapp.GetAppDirectory());
            //if (trace != null)
            //    Trace.CurrentTrace.SetTraceDirectory(trace);
            _hxr.SetResult += new SetResultEvent(_wr.SetResult);
        }

        public static void End()
        {
            _hxr.SetResult -= new SetResultEvent(_wr.SetResult);
        }

        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }

        public static void Test_InspectType_01()
        {
            _tr.WriteLine("Test_InspectType_01");
            //InspectType it = new InspectType("toto tata");
            //InspectType it = new InspectType(new test_01 { name = "zozo", number = 1234 });
            //InspectType it = new InspectType(new string[] { "tata", "toto", "zaza", "zozo" });
            InspectType it = new InspectType(new test_01[] {
                new test_01 { name = "tata", number = 1001 },
                new test_01 { name = "toto", number = 1002 },
                new test_01 { name = "zaza", number = 1003 },
                new test_01 { name = "zozo", number = 1004 } });
            foreach (InspectValue v in it)
            {
                _tr.WriteLine("name \"{0}\", valueType {1}, type {2}, value \"{3}\"", v.name, v.valueType, v.type, v.value);
            }
        }
    }
}
