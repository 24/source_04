using pb;
using pb.Data;
using System.Collections.Generic;

namespace Download.Print
{
    public static partial class WebData
    {
        public static void RunDownloadAutomate(string parameters = null)
        {
            //DownloadAutomateManager downloadAutomateManager = CreateDownloadAutomateManager(parameters);
            DownloadAutomateManager downloadAutomateManager = CreateDownloadAutomateManagerWithServers(parameters);
            try
            {
                //DownloadAutomateManagerCreator downloadAutomateManagerCreator = GetDownloadAutomateManagerCreator(parameters);
                //downloadAutomateManager = downloadAutomateManagerCreator.Create();
                //InitServers();
                //downloadAutomateManager.AddServerManagers(downloadAutomateManagerCreator.CreateServerManagers());

                if (!downloadAutomateManager.ControlDownloadManagerClient())
                    throw new PBException("error download manager client is not working");

                downloadAutomateManager.Start();
                downloadAutomateManager.Run();
            }
            finally
            {
                if (downloadAutomateManager != null)
                    downloadAutomateManager.Dispose();
            }
        }

        public static DownloadAutomateManager CreateDownloadAutomateManagerWithServers(string parameters = null)
        {
            //NamedValues<ZValue> parameters2 = NamedValues.ParseValues(parameters);
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            DownloadAutomateManagerCreator downloadAutomateManagerCreator = GetDownloadAutomateManagerCreator(parameters2, test);
            DownloadAutomateManager downloadAutomateManager = downloadAutomateManagerCreator.Create();
            InitServers(test);
            downloadAutomateManager.AddServerManagers(downloadAutomateManagerCreator.CreateServerManagers());
            return downloadAutomateManager;
        }

        public static void InitServers(bool test)
        {
            //Vosbooks.Vosbooks.FakeInit();
            Vosbooks.Vosbooks.Init(test);
            //Ebookdz.Ebookdz.FakeInit();
            Ebookdz.Ebookdz.Init(test);
            //MagazinesGratuits.MagazinesGratuits.FakeInit();
            MagazinesGratuits.MagazinesGratuits.Init(test);
            //TelechargerMagazine.TelechargerMagazine.FakeInit();
            TelechargerMagazine.TelechargerMagazine.Init(test);
            //ExtremeDown.ExtremeDown.FakeInit();
            ExtremeDown.ExtremeDown.Init(test);
        }
    }
}
