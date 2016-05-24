using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Text;
using System.Collections.Generic;
using System.Xml.Linq;

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
            DownloadAutomateManagerCreator createDownloadAutomateManager = new DownloadAutomateManagerCreator();
            createDownloadAutomateManager.Init(GetDownloadAutomateManagerConfig(test), XmlConfig.CurrentConfig);
            if (parameters != null)
                createDownloadAutomateManager.SetParameters(parameters);
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
            //NamedValues<ZValue> parameters2 = NamedValues.ParseValues(parameters);
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            createFindPrintManager.Init(GetDownloadAutomateManagerConfig(GetTestValue(parameters2)));
            //if (parameters != null)
            //    createFindPrintManager.SetParameters(NamedValues.ParseValues(parameters));
            createFindPrintManager.SetParameters(parameters2);
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
            //NamedValues<ZValue> parameters2 = NamedValues.ParseValues(parameters);
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            createPrintTitleManager.Init(GetDownloadAutomateManagerConfig(GetTestValue(parameters2)));
            //if (parameters != null)
            //    createPrintTitleManager.SetParameters(NamedValues.ParseValues(parameters));
            createPrintTitleManager.SetParameters(parameters2);
            return createPrintTitleManager;
        }

        public static NamedValues<ZValue> ParseParameters(string parameters)
        {
            return NamedValues.ParseValues(parameters, useLowercaseKey: true);
        }

        public static bool GetTestValue(NamedValues<ZValue> parameters)
        {
            //foreach (KeyValuePair<string, ZValue> parameter in parameters)
            //{
            //    if (parameter.Key.ToLower() == "test")
            //        return (bool)parameter.Value;
            //}
            if (parameters.ContainsKey("test"))
                return (bool)parameters["test"];
            return false;
        }

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
    }
}
