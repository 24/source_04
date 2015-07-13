using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using pb;
using pb.Compiler;
using pb.Data.Xml;

namespace Download.Print
{
    public static class DownloadManager_f
    {
        //private static ChannelFactory<IWCFDownloadManager> _channelFactory = null;
        //private static IWCFDownloadManager _service = null;
        private static DownloadManagerClient __downloadManager = null;

        public static void Test_DownloadService_01()
        {
            Trace.WriteLine("Test_DownloadService_01");
            //GetIWCFDownloadManagerService();
            CreateDownloadManagerClient();
            //Trace.WriteLine("service : IsStarted() = {0}", _service.IsStarted());
            Trace.WriteLine("service : IsStarted() = {0}", __downloadManager.IsStarted());
            //int nb = _service.GetDownloadCount();
            int nb = __downloadManager.GetDownloadCount();
            Trace.WriteLine("service : GetDownloadCount() = {0}", nb);
            //int i = 0;
            for (int i = 0; i < nb; i++)
            {
                //Trace.WriteLine("service : GetDownloadLocalFile({0})     = {1}", i, _service.GetDownloadLocalFile(i));
                //Trace.WriteLine("service : GetDownloadIsWorking({0})     = {1}", i, _service.GetDownloadIsWorking(i));
                //Trace.WriteLine("service : GetDownloadState({0})         = {1}", i, _service.GetDownloadState(i));
                //Trace.WriteLine("service : GetDownloadStatusMessage({0}) = {1}", i, _service.GetDownloadStatusMessage(i));
                //Trace.WriteLine("service : GetDownloadCreatedDate({0})   = {1}", i, _service.GetDownloadCreatedDate(i));
                //Trace.WriteLine("service : GetDownloadFileSize({0})      = {1}", i, _service.GetDownloadFileSize(i));
                //Trace.WriteLine("service : GetDownloadTransfered({0})    = {1}", i, _service.GetDownloadTransfered(i));
                //Trace.WriteLine("service : GetDownloadRate({0})          = {1}", i, _service.GetDownloadRate(i));
                //Trace.WriteLine("service : GetDownloadLeftTime({0})      = {1}", i, _service.GetDownloadLeftTime(i));
                //Trace.WriteLine("service : GetDownloadProgress({0})      = {1}", i, _service.GetDownloadProgress(i));

                //int id = _service.GetOrderedDownloadId(i);
                int id = __downloadManager.GetOrderedDownloadId(i);

                //Trace.WriteLine("  file {0} : index {1} id {2} url \"{3}\" file \"{4}\" state {5} create date {6} size {7} is working {8} status message \"{9}\" transfered {10} rate {11} left time {12} progress {13}",
                //    i + 1, _service.GetDownloadOrderedIndexById(id) + 1, id,
                //    _service.GetDownloadUrlById(id), _service.GetDownloadLocalFileById(id), _service.GetDownloadStateById(id), _service.GetDownloadCreatedDateById(id),
                //    _service.GetDownloadFileSizeById(id), _service.GetDownloadIsWorkingById(id), _service.GetDownloadStatusMessageById(id), _service.GetDownloadTransferedById(id),
                //    _service.GetDownloadRateById(id), _service.GetDownloadLeftTimeById(id), _service.GetDownloadProgressById(id));
                Trace.WriteLine("  file {0} : index {1} id {2} url \"{3}\" file \"{4}\" state {5} create date {6} size {7} is working {8} status message \"{9}\" transfered {10} rate {11} left time {12} progress {13}",
                    i + 1, __downloadManager.GetDownloadOrderedIndexById(id) + 1, id,
                    __downloadManager.GetDownloadUrlById(id), __downloadManager.GetDownloadLocalFileById(id), __downloadManager.GetDownloadStateById(id), __downloadManager.GetDownloadCreatedDateById(id),
                    __downloadManager.GetDownloadFileSizeById(id), __downloadManager.GetDownloadIsWorkingById(id), __downloadManager.GetDownloadStatusMessageById(id), __downloadManager.GetDownloadTransferedById(id),
                    __downloadManager.GetDownloadRateById(id), __downloadManager.GetDownloadLeftTimeById(id), __downloadManager.GetDownloadProgressById(id));
            }
        }

        //public static void Test_DownloadService_02()
        //{
        //    Trace.WriteLine("Test_DownloadService_02");
        //    CreateDownloadManagerClient();
        //    Trace.WriteLine("service : IsStarted() = {0}", __downloadManager.IsStarted());
        //    int nb = __downloadManager.GetDownloadCount();
        //    Trace.WriteLine("service : GetDownloadCount() = {0}", nb);
        //    //int i = 0;
        //    for (int i = 0; i < nb; i++)
        //    {
        //        //Trace.WriteLine("service : GetDownloadLocalFile({0})     = {1}", i, _service.GetDownloadLocalFile(i));
        //        //Trace.WriteLine("service : GetDownloadIsWorking({0})     = {1}", i, _service.GetDownloadIsWorking(i));
        //        //Trace.WriteLine("service : GetDownloadState({0})         = {1}", i, _service.GetDownloadState(i));
        //        //Trace.WriteLine("service : GetDownloadStatusMessage({0}) = {1}", i, _service.GetDownloadStatusMessage(i));
        //        //Trace.WriteLine("service : GetDownloadCreatedDate({0})   = {1}", i, _service.GetDownloadCreatedDate(i));
        //        //Trace.WriteLine("service : GetDownloadFileSize({0})      = {1}", i, _service.GetDownloadFileSize(i));
        //        //Trace.WriteLine("service : GetDownloadTransfered({0})    = {1}", i, _service.GetDownloadTransfered(i));
        //        //Trace.WriteLine("service : GetDownloadRate({0})          = {1}", i, _service.GetDownloadRate(i));
        //        //Trace.WriteLine("service : GetDownloadLeftTime({0})      = {1}", i, _service.GetDownloadLeftTime(i));
        //        //Trace.WriteLine("service : GetDownloadProgress({0})      = {1}", i, _service.GetDownloadProgress(i));

        //        int id = __downloadManager.GetOrderedDownloadId(i);

        //        Trace.WriteLine("  file {0} : index {1} id {2} url \"{3}\" file \"{4}\" state {5} create date {6} size {7} is working {8} status message \"{9}\" transfered {10} rate {11} left time {12} progress {13}",
        //            i + 1, __downloadManager.GetDownloadOrderedIndexById(id) + 1, id,
        //            __downloadManager.GetDownloadUrlById(id), __downloadManager.GetDownloadLocalFileById(id), __downloadManager.GetDownloadStateById(id), __downloadManager.GetDownloadCreatedDateById(id),
        //            __downloadManager.GetDownloadFileSizeById(id), __downloadManager.GetDownloadIsWorkingById(id), __downloadManager.GetDownloadStatusMessageById(id), __downloadManager.GetDownloadTransferedById(id),
        //            __downloadManager.GetDownloadRateById(id), __downloadManager.GetDownloadLeftTimeById(id), __downloadManager.GetDownloadProgressById(id));
        //    }
        //}

        // http://s11.alldebrid.com/dl/5sbtv85e03/Windows_Internet_Pratique_12_2013.pdf

        public static void Test_DownloadService_Start_01()
        {
            //GetIWCFDownloadManagerService();
            CreateDownloadManagerClient();
            //bool isStarted = _service.IsStarted();
            bool isStarted = __downloadManager.IsStarted();
            Trace.WriteLine("service : IsStarted() = {0}", isStarted);
            if (!isStarted)
            {
                Trace.WriteLine("service : Start()");
                //_service.Start();
                __downloadManager.Start();
            }
        }

        public static void Test_DownloadService_Stop_01()
        {
            //GetIWCFDownloadManagerService();
            CreateDownloadManagerClient();
            //bool isStarted = _service.IsStarted();
            bool isStarted = __downloadManager.IsStarted();
            Trace.WriteLine("service : IsStarted() = {0}", isStarted);
            if (isStarted)
            {
                Trace.WriteLine("service : Stop()");
                //_service.Stop();
                __downloadManager.Stop();
            }
        }

        public static void Test_DownloadService_IsStarted_01()
        {
            //GetIWCFDownloadManagerService();
            CreateDownloadManagerClient();
            //bool isStarted = _service.IsStarted();
            bool isStarted = __downloadManager.IsStarted();
            Trace.WriteLine("service : IsStarted() = {0}", isStarted);
        }

        //public static void Test_DownloadService_AddDownload_01(string url, string file, string directory)
        public static void Test_DownloadService_AddDownload_01(string url, string file)
        {
            //GetIWCFDownloadManagerService();
            CreateDownloadManagerClient();
            // Géo N°418 - Décembre 2013 [Liens Direct] Gratuit
            //string url = "http://s15.alldebrid.com/dl/5tjohw8b64/@Ge0@N@418.pdf";   // http://clz.to/9d837e3u http://www.telechargement-plus.com/e-book-magazines/94928-gyo-n418-dycembre-2013-liens-direct.html
            //string file = "Géo N°418 - Décembre 2013.pdf";
            //string directory = "Géo N°418 - Décembre 2013";
            Trace.WriteLine("service : AddDownload() = url \"{0}\" file \"{1}\"", url, file);
            //int id = _service.AddDownload(url, file, directory);
            //int id = __downloadManager.AddDownload(url, file, directory);
            int id = __downloadManager.AddDownload(url, file);
            Trace.WriteLine("service : id {0}", id);
        }

        public static void Test_DownloadService_RemoveDownloadById_01(int id)
        {
            Trace.WriteLine("Test_DownloadService_RemoveDownloadById_01");
            //GetIWCFDownloadManagerService();
            CreateDownloadManagerClient();
            //int nb = _service.GetDownloadCount();
            int nb = __downloadManager.GetDownloadCount();
            Trace.WriteLine("service : GetDownloadNb() = {0}", nb);
            Trace.WriteLine("service : RemoveDownloadById({0})", id);
            //_service.RemoveDownloadById(id);
            __downloadManager.RemoveDownloadById(id);
            Trace.WriteLine("service : GetDownloadNb() = {0}", nb);
        }

        public static void Test_DownloadService_GetDownloadLocalFileById_01(int id)
        {
            //Trace.WriteLine("Test_DownloadService_GetDownloadLocalFileById_01");
            CreateDownloadManagerClient();
            Trace.WriteLine("service : GetDownloadLocalFileById({0}) = \"{1}\"", id, __downloadManager.GetDownloadLocalFileById(id));
        }

        public static void Test_DownloadService_GetDownloadIsWorkingById_01(int id)
        {
            //Trace.WriteLine("Test_DownloadService_GetDownloadIsWorkingById_01");
            CreateDownloadManagerClient();
            Trace.WriteLine("service : GetDownloadIsWorkingById({0}) = {1}", id, __downloadManager.GetDownloadIsWorkingById(id));
        }

        public static void Test_DownloadService_GetDownloadStateById_01(int id)
        {
            //Trace.WriteLine("Test_DownloadService_GetDownloadStateById_01");
            CreateDownloadManagerClient();
            Trace.WriteLine("service : GetDownloadStateById({0}) = {1}", id, __downloadManager.GetDownloadStateById(id));
        }

        public static void Test_DownloadService_GetDownloadStatusMessageById_01(int id)
        {
            CreateDownloadManagerClient();
            Trace.WriteLine("service : GetDownloadStatusMessageById({0}) = {1}", id, __downloadManager.GetDownloadStatusMessageById(id));
        }

        public static void Test_DownloadService_GetDownloadFileSizeById_01(int id)
        {
            CreateDownloadManagerClient();
            Trace.WriteLine("service : GetDownloadFileSizeById({0}) = {1}", id, __downloadManager.GetDownloadFileSizeById(id));
        }

        public static void Test_DownloadService_GetDownloadTransferedById_01(int id)
        {
            CreateDownloadManagerClient();
            Trace.WriteLine("service : GetDownloadTransferedById({0}) = {1}", id, __downloadManager.GetDownloadTransferedById(id));
        }

        public static void Test_DownloadService_GetDownloadRateById_01(int id)
        {
            CreateDownloadManagerClient();
            Trace.WriteLine("service : GetDownloadRateById({0}) = {1}", id, __downloadManager.GetDownloadRateById(id));
        }

        public static void Test_DownloadService_GetDownloadLeftTimeById_01(int id)
        {
            CreateDownloadManagerClient();
            Trace.WriteLine("service : GetDownloadLeftTimeById({0}) = {1:dd\\.hh\\:mm\\:ss}", id, __downloadManager.GetDownloadLeftTimeById(id));
        }

        public static void Test_DownloadService_GetDownloadProgressById_01(int id)
        {
            CreateDownloadManagerClient();
            Trace.WriteLine("service : GetDownloadProgressById({0}) = {1,6:0.00}", id, __downloadManager.GetDownloadProgressById(id));
        }

        public static void Test_DownloadService_RemoveCompleted_01()
        {
            //GetIWCFDownloadManagerService();
            CreateDownloadManagerClient();
            Trace.WriteLine("service : RemoveCompleted()");
            //_service.RemoveCompleted();
            __downloadManager.RemoveCompleted();
        }


        public static void Test_DownloadService_StartAndTrace_01()
        {
            CreateDownloadManagerClient();

            TraceCurrentDownload();
            bool isStarted = __downloadManager.IsStarted();
            if (isStarted)
                Trace.WriteLine("service : IsStarted() = {0}", isStarted);
            else
            {
                Trace.WriteLine("service : Start()");
                __downloadManager.Start();
            }
            TraceCurrentDownload();
            TraceCurrentDownload();
            TraceCurrentDownload();
            TraceCurrentDownload();
            TraceCurrentDownload();
            TraceCurrentDownload();
            while (IsDownloadManagerWorking())
            {
                TraceCurrentDownload();
                Thread.Sleep(1000);
            }
            TraceCurrentDownload();
        }

        public static void TraceCurrentDownload()
        {
            CreateDownloadManagerClient();
            int nb = __downloadManager.GetDownloadCount();
            Trace.WriteLine("service : {0} files", nb);
            for (int i = 0; i < nb; i++)
            {
                int id = __downloadManager.GetOrderedDownloadId(i);
                Trace.WriteLine("  is working {0,-5} state {1,-20} file size {2,12} transfered {3,12} rate {4,15:0.00} time {5,17:dd\\.hh\\:mm\\:ss} progress {6,6:0.00} status \"{7}\" file \"{8}\"",
                    __downloadManager.GetDownloadIsWorkingById(id), __downloadManager.GetDownloadStateById(id), __downloadManager.GetDownloadFileSizeById(id), __downloadManager.GetDownloadTransferedById(id),
                    __downloadManager.GetDownloadRateById(id), __downloadManager.GetDownloadLeftTimeById(id), __downloadManager.GetDownloadProgressById(id), __downloadManager.GetDownloadStatusMessageById(id),
                    __downloadManager.GetDownloadLocalFileById(id));
            }
        }

        public static bool IsDownloadManagerWorking()
        {
            CreateDownloadManagerClient();
            int nb = __downloadManager.GetDownloadCount();
            for (int i = 0; i < nb; i++)
            {
                int id = __downloadManager.GetOrderedDownloadId(i);
                if (__downloadManager.GetDownloadIsWorkingById(id))
                    return true;
            }
            return false;
        }

        //public static void GetIWCFDownloadManagerService()
        //{
        //    //if (_rs.Data.ContainsKey("Service_ChannelFactory") && _rs.Data.ContainsKey("Service_IWCFDownloadManager"))
        //    //{
        //    //    _channelFactory = (ChannelFactory<IWCFDownloadManager>)_rs.Data["Service_ChannelFactory"];
        //    //    _service = (IWCFDownloadManager)_rs.Data["Service_IWCFDownloadManager"];
        //    //}
        //    Trace.WriteLine("create client service WCFDownloadManager");
        //    string address = "http://localhost:8019/WCFDownloadManager";
        //    ContractDescription contractDescription = ContractDescription.GetContract(typeof(IWCFDownloadManager));
        //    WSHttpBinding binding = new WSHttpBinding();
        //    EndpointAddress endpointAddress = new EndpointAddress(new Uri(address));
        //    ServiceEndpoint endpoint = new ServiceEndpoint(contractDescription, binding, endpointAddress);
        //    _channelFactory = new ChannelFactory<IWCFDownloadManager>(endpoint);
        //    _service = _channelFactory.CreateChannel();
        //}

        public static void CreateDownloadManagerClient()
        {
            if (__downloadManager == null)
                __downloadManager = new DownloadManagerClient(XmlConfig.CurrentConfig.Get("DownloadAutomateManager/DownloadManager/Address"));
        }
    }
}
