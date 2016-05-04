using pb;

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
            DownloadAutomateManagerCreator downloadAutomateManagerCreator = GetDownloadAutomateManagerCreator(parameters);
            DownloadAutomateManager downloadAutomateManager = downloadAutomateManagerCreator.Create();
            InitServers();
            downloadAutomateManager.AddServerManagers(downloadAutomateManagerCreator.CreateServerManagers());
            return downloadAutomateManager;
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

    }
}
