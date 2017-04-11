using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;

namespace anki
{
    public static class Run
    {
        public static void InitAlways()
        {
            XmlConfig.CurrentConfig = new XmlConfig(RunSourceCommand.GetFilePath("anki.config.xml"));
            //XmlConfig config = XmlConfig.CurrentConfig;
            //TraceManager.Current.SetWriter(WriteToFile.Create(@"log\log.txt", FileOption.None));
            RunSourceCommand.TraceManager.SetWriter(WriteToFile.Create(RunSourceCommand.GetFilePath(@"log\log.txt"), FileOption.None));
            Trace.WriteLine($"set log file to \"{RunSourceCommand.TraceManager.GetWriter().File}\"");
        }

        public static void EndAlways()
        {
            XmlConfig.CurrentConfig = null;
        }
    }
}
