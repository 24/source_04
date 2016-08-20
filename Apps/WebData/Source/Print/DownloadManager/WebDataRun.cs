using pb;
using pb.Data;
using System.Collections.Generic;

namespace Download.Print
{
    static partial class WebData
    {
        private static bool __serverInitialized = false;

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

        //public static bool TryDownloadPosts(string serverName, string parameters = null, string query = null, string sort = null, int limit = 0,
        //    string downloadDirectory = null, bool forceDownloadAgain = false, bool forceSelect = false, bool simulateDownload = false)
        //{
        //    return CreateDownloadAutomateManagerWithServers(parameters).TryDownloadPosts(ServerManagers_v2.Get(serverName).Find(query, sort, limit), downloadDirectory, forceDownloadAgain, forceSelect, simulateDownload);
        //}

        public static bool TryDownloadPosts(IEnumerable<IPostToDownload> posts, string parameters = null, string downloadDirectory = null, bool forceDownloadAgain = false, bool forceSelect = false, bool simulateDownload = false)
        {
            return CreateDownloadAutomateManagerWithServers(parameters).TryDownloadPosts(posts, downloadDirectory, forceDownloadAgain, forceSelect, simulateDownload);
        }

        public static IServerManager GetServer(string serverName, string parameters = null)
        {
            NamedValues<ZValue> parameters2 = ParseParameters(parameters);
            bool test = WebData.GetTestValue(parameters2);
            InitServers(test);
            return ServerManagers_v2.Get(serverName);
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
            if (!__serverInitialized)
            {
                __serverInitialized = true;
                //Vosbooks.Vosbooks.Init(test);
                //Ebookdz.Ebookdz.Init(test);
                //MagazinesGratuits.MagazinesGratuits.Init(test);
                //TelechargerMagazine.TelechargerMagazine.Init(test);
                //ExtremeDown.ExtremeDown.Init(test);
                ServerManagers_v2.Add(Vosbooks.Vosbooks.CreateServerManager(test));
                ServerManagers_v2.Add(Ebookdz.Ebookdz.CreateServerManager(test));
                ServerManagers_v2.Add(MagazinesGratuits.MagazinesGratuits.CreateServerManager(test));
                ServerManagers_v2.Add(TelechargerMagazine.TelechargerMagazine.CreateServerManager(test));
                ServerManagers_v2.Add(ExtremeDown.ExtremeDown.CreateServerManager(test));
            }
        }
    }
}
