using hts.WebData.Gesat;
using pb.Data;

namespace hts.WebData
{
    static partial class WebData
    {
        public static Gesat_v2 CreateGesat(string parameters = null)
        {
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            return Gesat_v2.Create(test);
            //return Gesat_v2.Create(GetGesatConfigElement(test));
        }

        public static GesatExport CreateGesatExport(string parameters = null)
        {
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            return GesatExport.Create(Gesat_v2.GetConfigElement(test));
        }

        //private static XElement GetGesatConfigElement(bool test = false)
        //{
        //    string configName = Gesat_v2.ConfigName;
        //    if (test)
        //    {
        //        configName += "_Test";
        //        Trace.WriteLine($"{Gesat_v2.ConfigName} init for test");
        //    }
        //    return XmlConfig.CurrentConfig.GetElement(configName);
        //}
    }
}
