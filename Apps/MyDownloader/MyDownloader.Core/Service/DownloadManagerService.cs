using System;
using System.IO;
using System.ServiceModel;
using MyDownloader.Core;

namespace MyDownloader.Service
{
    public static class DownloaderService
    {
        private static ServiceHost _hostDownloadManager = null;

        public static void StartService()
        {
            Log.Write("start service", LogMode.Information);
            _hostDownloadManager = new ServiceHost(typeof(DownloadManagerService));
            _hostDownloadManager.Open();
            Log.Write("service started", LogMode.Information);
        }

        public static void StopService()
        {
            if (_hostDownloadManager != null && (_hostDownloadManager.State == CommunicationState.Opened | _hostDownloadManager.State == CommunicationState.Faulted | _hostDownloadManager.State == CommunicationState.Opening))
            {
                Log.Write("stop service", LogMode.Information);
                _hostDownloadManager.Close();
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
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
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
                Log.Write(ex.Message + "\r\n" + ex.StackTrace, LogMode.Information);
            }
        }

        public void Start()
        {
            Log.Write("service : Start() begin", LogMode.Information);
            Execute(() => DownloadManager.Instance.StartAll());
            Log.Write("service : Start() end", LogMode.Information);
        }

        public bool IsStarted()
        {
            bool ret = false;
            Execute(() => ret = DownloadManager.Instance.HasDownloadStarted());
            return ret;
        }

        public void Stop()
        {
            Log.Write("service : Stop() begin", LogMode.Information);
            Execute(() => DownloadManager.Instance.PauseAll());
            Log.Write("service : Stop() end", LogMode.Information);
        }

        // return "c:\pib\_dl\_dl\_pib\dm"
        //public string GetDownloadFolder()
        //{
        //    return Settings.Default.DownloadFolder;
        //}

        // return "c:\pib\_dl\_dl\_pib\dm\..\dl"
        public string GetDownloadDirectory()
        {
            string directory = null;
            Execute(() => directory = DownloadManager.Instance.DefaultDownloadDirectory);
            return directory;
        }

        public int GetMaxRetryCount()
        {
            return Settings.Default.MaxRetries;
        }

        public int GetDownloadCount()
        {
            int nb = 0;
            Execute(() => nb = DownloadManager.Instance.GetDownloadCount());
            return nb;
        }

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
                    if (!Path.IsPathRooted(file))
                        file = Path.Combine(DownloadManager.Instance.DefaultDownloadDirectory, file);
                    ResourceLocation rl = new ResourceLocation();
                    rl.URL = url;
                    ResourceLocation[] mirrors = new ResourceLocation[0];
                    Downloader download = DownloadManager.Instance.Add(rl, mirrors, file, segmentNb, startNow);
                    Log.Write(string.Format("service : add download = url \"{0}\" file \"{1}\"", download.ResourceLocation.URL, download.LocalFile), LogMode.Information);
                    id = download.Id;
                }
            );
            return id;
        }

        public void RemoveDownloadById(int id)
        {
            Log.Write(string.Format("service : remove download = id {0}", id), LogMode.Information);
            Execute(() => DownloadManager.Instance.RemoveDownload(DownloadManager.Instance.GetDownloaderById(id)));
        }

        public void RemoveCompleted()
        {
            Log.Write(string.Format("service : remove completed download"), LogMode.Information);
            Execute(() => DownloadManager.Instance.ClearEnded());
        }

        public int GetOrderedDownloadId(int index)
        {
            int id = 0;
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
            DownloaderState state = DownloaderState.UnknowDownload;
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
