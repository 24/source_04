﻿using pb;
using pb.Data;
using pb.Data.Xml;
using Print;
using System.Xml.Linq;

namespace Download.Print
{
    public static class WebData
    {
        public static void RunDownloadAutomate(string parameters = null)
        {
            DownloadAutomateManager downloadAutomateManager = CreateDownloadAutomateManager(parameters);
            try
            {
                downloadAutomateManager.Start();
                // loadNewPost: loadNewPost, searchPostToDownload: searchPostToDownload, uncompressFile: uncompressFile, sendMail: sendMail
                downloadAutomateManager.Run();
            }
            finally
            {
                downloadAutomateManager.Dispose();
            }
        }

        public static DownloadAutomateManagerCreator GetDownloadAutomateManagerCreator(string parameters = null, bool initServers = true)
        {
            if (initServers)
                InitServers();
            DownloadAutomateManagerCreator createDownloadAutomateManager = new DownloadAutomateManagerCreator();
            createDownloadAutomateManager.Init(GetDownloadAutomateManagerConfig());
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

            return GetDownloadAutomateManagerCreator(parameters, initServers: true).Create();
        }

        public static FindPrintManager CreateFindPrintManager(string parameters = null)
        {
            CreateFindPrintManager createFindPrintManager = new CreateFindPrintManager();
            createFindPrintManager.Init(GetDownloadAutomateManagerConfig());
            if (parameters != null)
                createFindPrintManager.SetParameters(NamedValues.ParseValues(parameters));
            return createFindPrintManager.Create();
        }

        public static PrintTitleManager CreatePrintTitleManager(string parameters = null)
        {
            CreatePrintTitleManager createPrintTitleManager = new CreatePrintTitleManager();
            createPrintTitleManager.Init(GetDownloadAutomateManagerConfig());
            if (parameters != null)
                createPrintTitleManager.SetParameters(NamedValues.ParseValues(parameters));
            return createPrintTitleManager.Create();
        }

        public static void InitServers()
        {
            //Vosbooks.Vosbooks_v1.FakeInit();
            //Vosbooks.Vosbooks.FakeInit();
            Vosbooks.Vosbooks.FakeInit();

            //Ebookdz.old.Ebookdz_v1.FakeInit();
            //Ebookdz.old.Ebookdz_v2.FakeInit();
            //Ebookdz.Ebookdz_v3.FakeInit();
            Ebookdz.Ebookdz.FakeInit();

            MagazinesGratuits.MagazinesGratuits.FakeInit();
            //TelechargerMagazine.old.TelechargerMagazine_v1.FakeInit();
            TelechargerMagazine.TelechargerMagazine.FakeInit();
            //ExtremeDown.old.ExtremeDown_v2.FakeInit();
            ExtremeDown.ExtremeDown.FakeInit();
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
