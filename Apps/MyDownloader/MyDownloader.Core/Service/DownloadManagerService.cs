using System;
using System.IO;
using System.ServiceModel;
using MyDownloader.Core;
//using MyDownloader.Core.Core;

//namespace MyDownloader.App.WCF
namespace MyDownloader.Service
{
    //public static class wcf
    // DownloaderService
    public static class DownloaderService
    {
        private static ServiceHost _hostDownloadManager = null;
        //private static DownloadList _downloadList = null;

        //public static DownloadList DownloadList { get { return _downloadList; } }

        public static void StartService()
        {
            //_downloadList.Log(null, "start service", MyDownloader.App.UI.DownloadList.LogMode.Information);
            Log.Write("start service", LogMode.Information);
            _hostDownloadManager = new ServiceHost(typeof(DownloadManagerService));
            _hostDownloadManager.Open();
            //_downloadList.Log(null, "service started", MyDownloader.App.UI.DownloadList.LogMode.Information);
            Log.Write("service started", LogMode.Information);
        }

        public static void StopService()
        {
            if (_hostDownloadManager != null && (_hostDownloadManager.State == CommunicationState.Opened | _hostDownloadManager.State == CommunicationState.Faulted | _hostDownloadManager.State == CommunicationState.Opening))
            {
                //_downloadList.Log(null, "stop service", MyDownloader.App.UI.DownloadList.LogMode.Information);
                Log.Write("stop service", LogMode.Information);
                _hostDownloadManager.Close();
                //_downloadList.Log(null, "service stopped", MyDownloader.App.UI.DownloadList.LogMode.Information);
                Log.Write("service stopped", LogMode.Information);
            }
        }

        public static bool IsServiceStarted()
        {
            if (_hostDownloadManager != null && (_hostDownloadManager.State == CommunicationState.Opened | _hostDownloadManager.State == CommunicationState.Faulted | _hostDownloadManager.State == CommunicationState.Opening))
                return true;
            else
                return false;
        }

        //public static void SetDownloadList(DownloadList downloadList)
        //{
        //    _downloadList = downloadList;
        //}
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    //public class WCFDownloadManager : IWCFDownloadManager
    // DownloadManagerService
    public class DownloadManagerService : IDownloadManagerService
    {
        private delegate void ExecuteFunction();

        private void Execute(ExecuteFunction function)
        {
            try
            {
                function();
            }
            catch (Exception ex)
            {
                //DownloaderService.DownloadList.Log(null, ex.Message + "\r\n" + ex.StackTrace, MyDownloader.App.UI.DownloadList.LogMode.Information);
                Log.Write(ex.Message + "\r\n" + ex.StackTrace, LogMode.Information);
            }
        }

        public void Start()
        {
            //DownloaderService.DownloadList.Log(null, "service : Start() begin", MyDownloader.App.UI.DownloadList.LogMode.Information);
            Log.Write("service : Start() begin", LogMode.Information);
            //Execute(() => DownloaderService.DownloadList.StartAll());
            Execute(() => DownloadManager.Instance.StartAll());
            //DownloaderService.DownloadList.Log(null, "service : Start() end", MyDownloader.App.UI.DownloadList.LogMode.Information);
            Log.Write("service : Start() end", LogMode.Information);
        }

        public bool IsStarted()
        {
            //wcf.DownloadList.Log(null, string.Format("service : IsStarted() = {0}", ret), MyDownloader.App.UI.DownloadList.LogMode.Information);
            bool ret = false;
            //Execute(() => ret = DownloaderService.DownloadList.HasDownloadStarted());
            Execute(() => ret = DownloadManager.Instance.HasDownloadStarted());
            return ret;
        }

        public void Stop()
        {
            //DownloaderService.DownloadList.Log(null, "service : Stop() begin", MyDownloader.App.UI.DownloadList.LogMode.Information);
            Log.Write("service : Stop() begin", LogMode.Information);
            //Execute(() => DownloaderService.DownloadList.PauseAll());
            Execute(() => DownloadManager.Instance.PauseAll());
            //DownloaderService.DownloadList.Log(null, "service : Stop() end", MyDownloader.App.UI.DownloadList.LogMode.Information);
            Log.Write("service : Stop() end", LogMode.Information);
        }

        //public bool IsStopped()
        //{
        //    bool ret = !wcf.DownloadList.HasDownloadStarted();
        //    //wcf.DownloadList.Log(null, string.Format("service : IsStopped() = {0}", ret), MyDownloader.App.UI.DownloadList.LogMode.Information);
        //    return ret;
        //}

        public int GetMaxRetryCount()
        {
            return Settings.Default.MaxRetries;
        }

        public int GetDownloadCount()
        {
            int nb = 0;
            //Execute(() => nb = DownloaderService.DownloadList.GetDownloadNb());
            Execute(() => nb = DownloadManager.Instance.GetDownloadCount());
            return nb;
        }

        //public int AddDownload(string url, string file = null, string directory = null, int segmentNb = 1, bool startNow = false)
        public int AddDownload(string url, string file = null, int segmentNb = 1, bool startNow = false)
        {
            int id = 0;
            Execute(() =>
                {
                    if (file == null)
                    {
                        Uri uri = new Uri(url);
                        file = uri.Segments[uri.Segments.Length - 1];
                    }
                    //if (directory != null)
                    //    file = Path.Combine(directory, file);
                    if (!Path.IsPathRooted(file))
                        file = Path.Combine(DownloadManager.Instance.DefaultDownloadDirectory, file);
                    ResourceLocation rl = new ResourceLocation();
                    rl.URL = url;
                    ResourceLocation[] mirrors = new ResourceLocation[0];
                    Downloader download = DownloadManager.Instance.Add(rl, mirrors, file, segmentNb, startNow);
                    //DownloaderService.DownloadList.Log(null, string.Format("service : add download = url \"{0}\" file \"{1}\"", download.ResourceLocation.URL, download.LocalFile), MyDownloader.App.UI.DownloadList.LogMode.Information);
                    Log.Write(string.Format("service : add download = url \"{0}\" file \"{1}\"", download.ResourceLocation.URL, download.LocalFile), LogMode.Information);
                    id = download.Id;
                }
            );
            return id;
        }

        public void RemoveDownloadById(int id)
        {
            //DownloaderService.DownloadList.Log(null, string.Format("service : remove download = id {0}", id), MyDownloader.App.UI.DownloadList.LogMode.Information);
            Log.Write(string.Format("service : remove download = id {0}", id), LogMode.Information);
            //Execute(() => DownloaderService.DownloadList.RemoveDownloadById(id));
            Execute(() => DownloadManager.Instance.RemoveDownload(DownloadManager.Instance.GetDownloaderById(id)));
        }

        public void RemoveCompleted()
        {
            //DownloaderService.DownloadList.Log(null, string.Format("service : remove completed download"), MyDownloader.App.UI.DownloadList.LogMode.Information);
            Log.Write(string.Format("service : remove completed download"), LogMode.Information);
            //Execute(() => DownloaderService.DownloadList.RemoveCompleted());
            Execute(() => DownloadManager.Instance.ClearEnded());
        }

        public int GetOrderedDownloadId(int index)
        {
            int id = 0;
            //Execute(() => id = GetOrderedDownloader(index).Id);
            Execute(() =>
            {
                Downloader downloader = GetOrderedDownloader(index);
                if (downloader != null)
                    id = downloader.Id;
            });
            return id;
        }

        public int GetDownloadOrderedIndexById(int id)
        {
            int index = 0;
            //Execute(() => index = GetDownloaderById(id).OrderedIndex);
            Execute(() =>
                {
                    Downloader downloader = GetDownloaderById(id);
                    if (downloader != null)
                        index = downloader.OrderedIndex;
                });
            return index;
        }

        public string GetDownloadUrlById(int id)
        {
            string url = null;
            //Execute(() => url = GetDownloaderById(id).ResourceLocation.URL);
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    url = downloader.ResourceLocation.URL;
            });
            return url;
        }

        public string GetDownloadLocalFileById(int id)
        {
            string file = null;
            //Execute(() => file = GetDownloaderById(id).LocalFile);
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    file = downloader.LocalFile;
            });
            return file;
        }

        public bool GetDownloadIsWorkingById(int id)
        {
            bool isWorking = false;
            //Execute(() => isWorking = GetDownloaderById(id).IsWorking());
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    isWorking = downloader.IsWorking();
            });
            return isWorking;
        }

        public DownloaderState GetDownloadStateById(int id)
        {
            //DownloaderState state = DownloaderState.Paused;
            DownloaderState state = DownloaderState.UnknowDownload;
            //Execute(() => state = GetDownloaderById(id).State);
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    state = downloader.State;
            });
            return state;
        }

        public string GetDownloadStatusMessageById(int id)
        {
            string status = null;
            //Execute(() => status = GetDownloaderById(id).StatusMessage);
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    status = downloader.StatusMessage;
            });
            return status;
        }

        public DateTime GetDownloadCreatedDateById(int id)
        {
            DateTime date = DateTime.MinValue;
            //Execute(() => date = GetDownloaderById(id).CreatedDateTime);
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    date = downloader.CreatedDateTime;
            });
            return date;
        }

        public long GetDownloadFileSizeById(int id)
        {
            long size = 0;
            //Execute(() => size = GetDownloaderById(id).FileSize);
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    size = downloader.FileSize;
            });
            return size;
        }

        public long GetDownloadTransferedById(int id)
        {
            long transfered = 0;
            //Execute(() => transfered = GetDownloaderById(id).Transfered);
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    transfered = downloader.Transfered;
            });
            return transfered;
        }

        public double GetDownloadRateById(int id)
        {
            double rate = 0;
            //Execute(() => rate = GetDownloaderById(id).Rate);
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    rate = downloader.Rate;
            });
            return rate;
        }

        public TimeSpan GetDownloadLeftTimeById(int id)
        {
            TimeSpan left = TimeSpan.FromTicks(0);
            //Execute(() => left = GetDownloaderById(id).Left);
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    left = downloader.Left;
            });
            return left;
        }

        public double GetDownloadProgressById(int id)
        {
            double progress = 0;
            //Execute(() => progress = GetDownloaderById(id).Progress);
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    progress = downloader.Progress;
            });
            return progress;
        }

        public int GetDownloadRetryCountById(int id)
        {
            int retry = 0;
            Execute(() =>
            {
                Downloader downloader = GetDownloaderById(id);
                if (downloader != null)
                    retry = downloader.RetryCount;
            });
            return retry;
        }

        private static Downloader GetOrderedDownloader(int i)
        {
            return DownloadManager.Instance.GetOrderedDownloader(i);
        }

        private static Downloader GetDownloaderById(int id)
        {
            return DownloadManager.Instance.GetDownloaderById(id);
        }
    }
}
