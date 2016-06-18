using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;

namespace Test.Test_Debug
{
    static partial class w
    {
        public static RunSource _rs = RunSource.CurrentRunSource;
        public static string _dataDir = null;

        public static void Init()
        {
            //_rs.InitConfig("Test_Debug");
            XmlConfig.CurrentConfig = new XmlConfig("Test_Debug");
            //string log = XmlConfig.CurrentConfig.Get("Log").zRootPath(zapp.GetAppDirectory());
            //if (log != null)
            //    Trace.CurrentTrace.SetLogFile(log, LogOptions.IndexedFile);
            Trace.CurrentTrace.SetWriter(XmlConfig.CurrentConfig.Get("Log"), XmlConfig.CurrentConfig.Get("Log/@option").zTextDeserialize(FileOption.None));

            _dataDir = XmlConfig.CurrentConfig.GetExplicit("DataDir");
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            //System.Diagnostics.Debugger.Break();
            //System.Diagnostics.MonitoringDescriptionAttribute
            Trace.WriteLine("toto");
        }

        public static void Test_Exception_01()
        {
            object o = null;
            o.ToString();
        }

        public static void Test_StackTrace_01()
        {
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
            //Trace.WriteLine(stackTrace.GetFrame(1).GetMethod().Name);
            int i = 1;
            foreach (System.Diagnostics.StackFrame stackFrame in stackTrace.GetFrames())
            {
                Trace.WriteLine("stack frame {0}", i++);
                Trace.WriteLine("  Method               : {0}", stackFrame.GetMethod().Name);
                Trace.WriteLine("  Method assembly      : {0}", stackFrame.GetMethod().Module.Assembly);
                Trace.WriteLine("  Method module        : {0}", stackFrame.GetMethod().Module.Name);
                Trace.WriteLine("  FileName             : {0}", stackFrame.GetFileName());
                Trace.WriteLine("  LineNumber           : {0}", stackFrame.GetFileLineNumber());
                Trace.WriteLine("  ColumnNumber         : {0}", stackFrame.GetFileColumnNumber());
                Trace.WriteLine();
            }
        }
    }
}
