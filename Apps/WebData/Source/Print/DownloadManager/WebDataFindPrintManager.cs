using pb.Data;

namespace Download.Print
{
    public static partial class WebData
    {
        public static FindPrintManager CreateFindPrintManager(string parameters = null, int version = 0)
        {
            // parameters : version = 6, dailyPrintManager = true, gapDayBefore = 5, gapDayAfter = 2
            FindPrintManagerCreator createFindPrintManager = new FindPrintManagerCreator();
            if (version != 0)
                createFindPrintManager.Version = version;
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            //NamedValues.TraceValues(parameters2);
            createFindPrintManager.Init(GetDownloadAutomateManagerConfig(GetTestValue(parameters2)));
            createFindPrintManager.SetParameters(parameters2);
            //Trace.WriteLine("DailyPrintManager {0}", createFindPrintManager.DailyPrintManager);
            return createFindPrintManager.Create();
        }
    }
}
