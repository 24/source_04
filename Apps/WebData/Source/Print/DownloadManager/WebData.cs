using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Text;
using System.Xml.Linq;

namespace Download.Print
{
    public static partial class WebData
    {
        // bool initServers = false
        public static DownloadAutomateManagerCreator GetDownloadAutomateManagerCreator(string parameters = null)
        {
            //if (initServers)
            //    InitServers();
            DownloadAutomateManagerCreator createDownloadAutomateManager = new DownloadAutomateManagerCreator();
            createDownloadAutomateManager.Init(GetDownloadAutomateManagerConfig(), XmlConfig.CurrentConfig);
            if (parameters != null)
                createDownloadAutomateManager.SetParameters(NamedValues.ParseValues(parameters));
            return createDownloadAutomateManager;
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

        public static FindPrintManager CreateFindPrintManager(string parameters = null)
        {
            // parameters : version = 6, dailyPrintManager = true, gapDayBefore = 5, gapDayAfter = 2
            FindPrintManagerCreator createFindPrintManager = new FindPrintManagerCreator();
            createFindPrintManager.Init(GetDownloadAutomateManagerConfig());
            if (parameters != null)
                createFindPrintManager.SetParameters(NamedValues.ParseValues(parameters));
            return createFindPrintManager.Create();
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
            createPrintTitleManager.Init(GetDownloadAutomateManagerConfig());
            if (parameters != null)
                createPrintTitleManager.SetParameters(NamedValues.ParseValues(parameters));
            return createPrintTitleManager;
        }

        public static XElement GetDownloadAutomateManagerConfig()
        {
            if (!DownloadPrint.Test)
                return XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager");
            else
            {
                Trace.WriteLine("use DownloadAutomateManager test config");
                return XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager_Test");
            }
        }
    }
}
