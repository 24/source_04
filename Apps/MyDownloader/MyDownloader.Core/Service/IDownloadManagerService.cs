using System;
using System.ServiceModel;

namespace MyDownloader.Service
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IDownloadManagerService
    {
        [OperationContract()]
        void Start();

        [OperationContract()]
        bool IsStarted();

        [OperationContract()]
        void Stop();

        //[OperationContract()]
        //string GetDownloadFolder();

        [OperationContract()]
        string GetDownloadDirectory();

        [OperationContract()]
        int GetMaxRetryCount();

        [OperationContract()]
        int GetDownloadCount();

        [OperationContract()]
        int AddDownload(string url, string file = null, int segmentNb = 1, bool startNow = false);

        [OperationContract()]
        void RemoveDownloadById(int id);

        [OperationContract()]
        void RemoveCompleted();

        [OperationContract()]
        int GetOrderedDownloadId(int i);

        [OperationContract()]
        int GetDownloadOrderedIndexById(int id);

        [OperationContract()]
        string GetDownloadUrlById(int id);

        // path :
        //   state Preparing : c:\pib\_dl\_pib\dm\..\dl\l_express\l_express_2014-08-13.pdf
        //   state Working   : c:\pib\_dl\_pib\dl\l_express\l_express_2014-08-13(3).pdf
        [OperationContract()]
        string GetDownloadLocalFileById(int id);

        [OperationContract()]
        bool GetDownloadIsWorkingById(int id);

        [OperationContract()]
        DownloaderState GetDownloadStateById(int id);

        [OperationContract()]
        string GetDownloadStatusMessageById(int id);

        [OperationContract()]
        DateTime GetDownloadCreatedDateById(int id);

        [OperationContract()]
        long GetDownloadFileSizeById(int id);

        [OperationContract()]
        long GetDownloadTransferedById(int id);

        [OperationContract()]
        double GetDownloadRateById(int id);

        [OperationContract()]
        TimeSpan GetDownloadLeftTimeById(int id);

        [OperationContract()]
        double GetDownloadProgressById(int id);

        [OperationContract()]
        int GetDownloadRetryCountById(int id);
    }
}
