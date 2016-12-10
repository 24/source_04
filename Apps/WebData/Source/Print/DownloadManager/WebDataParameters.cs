using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Xml;

namespace Download.Print
{
    static partial class WebData
    {
        public static XElement GetDownloadAutomateManagerConfig(bool test)
        {
            //if (!DownloadPrint.Test)
            if (!test)
                return XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager");
            else
            {
                Trace.WriteLine("use DownloadAutomateManager test config");
                return XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager_Test");
            }
        }

        public static NamedValues<ZValue> ParseParameters(string parameters)
        {
            return ParseNamedValues.ParseValues(parameters, useLowercaseKey: true);
        }

        public static bool GetTestValue(NamedValues<ZValue> parameters)
        {
            if (parameters.ContainsKey("test"))
                return (bool)parameters["test"];
            return false;
        }
    }
}
