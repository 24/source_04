using hts.WebData.Unea;
using pb;
using pb.Data;
using pb.Data.Xml;
using System.Xml.Linq;

namespace hts.WebData
{
    static partial class WebData
    {
        public static Unea_v2 CreateUnea(string parameters = null)
        {
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            return Unea_v2.Create(GetUneaConfigElement(test));
        }

        public static UneaExport CreateUneaExport(string parameters = null)
        {
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            return UneaExport.Create(GetUneaConfigElement(test));
        }

        private static XElement GetUneaConfigElement(bool test = false)
        {
            string configName = Unea_v2.ConfigName;
            if (test)
            {
                configName += "_Test";
                Trace.WriteLine($"{Unea_v2.ConfigName} init for test");
            }
            return XmlConfig.CurrentConfig.GetElement(configName);
        }
    }
}
