using pb.Compiler;
using pb.Data.Xml;

namespace tv
{
    public static class Run
    {
        public static void InitAlways()
        {
            XmlConfig.CurrentConfig = new XmlConfig(RunSourceCommand.GetFilePath("tv.config.xml"));
            XmlConfig config = XmlConfig.CurrentConfig;
        }

        public static void EndAlways()
        {
            XmlConfig.CurrentConfig = null;
        }
    }
}
