using pb.Data;
using pb.Data.Xml;
using pb.Text;

namespace Download.Print
{
    public static partial class WebData
    {
        public static DownloadAutomateManagerCreator GetDownloadAutomateManagerCreator(string parameters = null)
        {
            //NamedValues<ZValue> parameters2 = NamedValues.ParseValues(parameters);
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            return GetDownloadAutomateManagerCreator(parameters2, GetTestValue(parameters2));
        }

        // bool initServers = false
        public static DownloadAutomateManagerCreator GetDownloadAutomateManagerCreator(NamedValues<ZValue> parameters, bool test)
        {
            //if (initServers)
            //    InitServers();
            DownloadAutomateManagerCreator downloadAutomateManagerCreator = new DownloadAutomateManagerCreator();
            downloadAutomateManagerCreator.Init(GetDownloadAutomateManagerConfig(test), XmlConfig.CurrentConfig);
            if (parameters != null)
                downloadAutomateManagerCreator.SetParameters(parameters);
            return downloadAutomateManagerCreator;
        }

        public static DownloadAutomateManager CreateDownloadAutomateManager(string parameters = null)
        {
            // parameters : version = 6, traceLevel = 0, useTestManager = true
            //   waitTimeBetweenOperation = 00:05, mailWaitDownloadFinish = 10:00, postDownloadServerLimit = 3
            //   dailyPrintManager = true, gapDayBefore = 5, gapDayAfter = 2
            //   runNow = true, loadNewPost = true, searchPostToDownload = true, sendMail = true

            //InitServers();
            //CreateDownloadAutomateManager createDownloadAutomateManager = new CreateDownloadAutomateManager();
            //createDownloadAutomateManager.Init(GetDownloadAutomateManagerConfig());
            //if (parameters != null)
            //    createDownloadAutomateManager.SetParameters(NamedValues.ParseValues(parameters));
            //return createDownloadAutomateManager.Create();

            //return GetDownloadAutomateManagerCreator(parameters, initServers: true).Create();
            return GetDownloadAutomateManagerCreator(parameters).Create();
        }

        public static PrintTitleManager CreatePrintTitleManager(string parameters = null)
        {
            // parameters : version = 6, gapDayBefore = 5, gapDayAfter = 2
            return GetCreatePrintTitleManager(parameters).Create();
        }

        public static FindDateManager CreateFindDateManager(string parameters = null)
        {
            // parameters : version = 6, gapDayBefore = 5, gapDayAfter = 2
            return GetCreatePrintTitleManager(parameters).CreateFindDateManager();
        }

        public static PrintTitleManagerCreator GetCreatePrintTitleManager(string parameters = null)
        {
            PrintTitleManagerCreator createPrintTitleManager = new PrintTitleManagerCreator();
            //NamedValues<ZValue> parameters2 = NamedValues.ParseValues(parameters);
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            createPrintTitleManager.Init(GetDownloadAutomateManagerConfig(GetTestValue(parameters2)));
            //if (parameters != null)
            //    createPrintTitleManager.SetParameters(NamedValues.ParseValues(parameters));
            createPrintTitleManager.SetParameters(parameters2);
            return createPrintTitleManager;
        }

        public static DownloadManagerClientBase CreateDownloadManagerClient(string parameters = null, bool useTestManager = false)
        {
            return DownloadAutomateManagerCreator.CreateDownloadManagerClient(GetDownloadAutomateManagerConfig(GetTestValue(ParseParameters(parameters))), useTestManager);
        }
    }
}
