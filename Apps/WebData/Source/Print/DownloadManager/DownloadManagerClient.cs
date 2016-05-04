using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using MyDownloader.Service;
using pb.IO;

namespace Download.Print
{
    public abstract class DownloadManagerClientBase : IDisposable
    {
        public virtual void Dispose()
        {
            Close();
        }

        public abstract void Close();
        public abstract void Start();
        public abstract bool IsStarted();
        public abstract void Stop();
        public abstract int GetMaxRetryCount();
        public abstract int GetDownloadCount();
        public abstract int AddDownload(string url, string file = null, int segmentNb = 1, bool startNow = false);
        public abstract void RemoveDownloadById(int id);
        public abstract void RemoveCompleted();
        public abstract int GetOrderedDownloadId(int i);
        public abstract int GetDownloadOrderedIndexById(int id);
        public abstract string GetDownloadUrlById(int id);
        public abstract string GetDownloadLocalFileById(int id);
        public abstract bool GetDownloadIsWorkingById(int id);
        public abstract DownloaderState GetDownloadStateById(int id);
        public abstract string GetDownloadStatusMessageById(int id);
        public abstract DateTime GetDownloadCreatedDateById(int id);
        public abstract long GetDownloadFileSizeById(int id);
        public abstract long GetDownloadTransferedById(int id);
        public abstract double GetDownloadRateById(int id);
        public abstract TimeSpan GetDownloadLeftTimeById(int id);
        public abstract double GetDownloadProgressById(int id);
        public abstract int GetDownloadRetryCountById(int id);
    }

    public class DownloadManagerClient : DownloadManagerClientBase
    {
        private string _address;  // http://localhost:8019/WCFDownloadManager
        private ChannelFactory<IDownloadManagerService> _channelFactory = null;
        private IDownloadManagerService _service = null;
        private int _maxRetryCount = -1;

        public DownloadManagerClient(string address)
        {
            _address = address;
            InitClient();
        }

        //public void Dispose()
        //{
        //    Close();
        //}

        public override void Close()
        {
            _channelFactory.Close();
        }

        private void InitClient()
        {
            //Trace.WriteLine("create client service WCFDownloadManager");
            ContractDescription contractDescription = ContractDescription.GetContract(typeof(IDownloadManagerService));
            WSHttpBinding binding = new WSHttpBinding();
            EndpointAddress endpointAddress = new EndpointAddress(new Uri(_address));
            ServiceEndpoint endpoint = new ServiceEndpoint(contractDescription, binding, endpointAddress);
            _channelFactory = new ChannelFactory<IDownloadManagerService>(endpoint);
            _service = _channelFactory.CreateChannel();
        }

        public override void Start()
        {
            _service.Start();
        }

        public override bool IsStarted()
        {
            return _service.IsStarted();
        }

        public override void Stop()
        {
            _service.Stop();
        }

        public override int GetMaxRetryCount()
        {
            if (_maxRetryCount == -1)
                _maxRetryCount = _service.GetMaxRetryCount();
            return _maxRetryCount;
        }

        public override int GetDownloadCount()
        {
            return _service.GetDownloadCount();
        }

        public override int AddDownload(string url, string file = null, int segmentNb = 1, bool startNow = false)
        {
            return _service.AddDownload(url, file, segmentNb, startNow);
        }

        public override void RemoveDownloadById(int id)
        {
            _service.RemoveDownloadById(id);
        }

        public override void RemoveCompleted()
        {
            _service.RemoveCompleted();
        }

        public override int GetOrderedDownloadId(int i)
        {
            return _service.GetOrderedDownloadId(i);
        }

        public override int GetDownloadOrderedIndexById(int id)
        {
            return _service.GetDownloadOrderedIndexById(id);
        }

        public override string GetDownloadUrlById(int id)
        {
            return _service.GetDownloadUrlById(id);
        }

        public override string GetDownloadLocalFileById(int id)
        {
            return _service.GetDownloadLocalFileById(id);
        }

        public override bool GetDownloadIsWorkingById(int id)
        {
            return _service.GetDownloadIsWorkingById(id);
        }

        public override DownloaderState GetDownloadStateById(int id)
        {
            return _service.GetDownloadStateById(id);
        }

        public override string GetDownloadStatusMessageById(int id)
        {
            return _service.GetDownloadStatusMessageById(id);
        }

        public override DateTime GetDownloadCreatedDateById(int id)
        {
            return _service.GetDownloadCreatedDateById(id);
        }

        public override long GetDownloadFileSizeById(int id)
        {
            return _service.GetDownloadFileSizeById(id);
        }

        public override long GetDownloadTransferedById(int id)
        {
            return _service.GetDownloadTransferedById(id);
        }

        public override double GetDownloadRateById(int id)
        {
            return _service.GetDownloadRateById(id);
        }

        public override TimeSpan GetDownloadLeftTimeById(int id)
        {
            return _service.GetDownloadLeftTimeById(id);
        }

        public override double GetDownloadProgressById(int id)
        {
            return _service.GetDownloadProgressById(id);
        }

        public override int GetDownloadRetryCountById(int id)
        {
            return _service.GetDownloadRetryCountById(id);
        }

        public override string ToString()
        {
            return base.ToString() + string.Format(" - address = \"{0}\", maxRetryCount = {1}", _address, _maxRetryCount);
        }
    }

    public class DownloadFileTest
    {
        public int Id;
        public string Url;
        public string File;
        public DateTime? DownloadStartTime;
        public TimeSpan DownloadDuration;
        public DateTime? DownloadEndTime;
    }

    public class DownloadManagerClientTest : DownloadManagerClientBase
    {
        //private string _address;  // http://localhost:8019/WCFDownloadManager
        //private ChannelFactory<IWCFDownloadManager> _channelFactory = null;
        //private IWCFDownloadManager _service = null;
        private static TimeSpan __defaultDownloadDuration = TimeSpan.FromSeconds(10);
        private string _downloadDirectory = null;
        private int _id = 1;
        private Dictionary<int, DownloadFileTest> _downloadFiles = new Dictionary<int, DownloadFileTest>();

        public DownloadManagerClientTest(string downloadDirectory)
        {
            //_address = address;
            //InitClient();
            _downloadDirectory = downloadDirectory;
        }

        //public void Dispose()
        //{
        //    Close();
        //}

        public override void Close()
        {
            //_channelFactory.Close();
        }

        private void InitClient()
        {
            ////Trace.WriteLine("create client service WCFDownloadManager");
            //ContractDescription contractDescription = ContractDescription.GetContract(typeof(IWCFDownloadManager));
            //WSHttpBinding binding = new WSHttpBinding();
            //EndpointAddress endpointAddress = new EndpointAddress(new Uri(_address));
            //ServiceEndpoint endpoint = new ServiceEndpoint(contractDescription, binding, endpointAddress);
            //_channelFactory = new ChannelFactory<IWCFDownloadManager>(endpoint);
            //_service = _channelFactory.CreateChannel();
        }

        public override void Start()
        {
            //_service.Start();
        }

        public override bool IsStarted()
        {
            //return _service.IsStarted();
            return true;
        }

        public override void Stop()
        {
            //_service.Stop();
        }

        public override int GetMaxRetryCount()
        {
            return 10;
        }

        public override int GetDownloadCount()
        {
            //return _service.GetDownloadCount();
            return _downloadFiles.Count;
        }

        public override int AddDownload(string url, string file = null, int segmentNb = 1, bool startNow = false)
        {
            //return _service.AddDownload(url, file, segmentNb, startNow);
            DownloadFileTest downloadFileTest = new DownloadFileTest();
            downloadFileTest.Id = _id++;
            downloadFileTest.Url = url;
            downloadFileTest.File = file;
            downloadFileTest.DownloadStartTime = DateTime.Now;
            downloadFileTest.DownloadDuration = __defaultDownloadDuration;
            downloadFileTest.DownloadEndTime = downloadFileTest.DownloadStartTime + downloadFileTest.DownloadDuration;
            _downloadFiles.Add(downloadFileTest.Id, downloadFileTest);
            return downloadFileTest.Id;
        }

        public override void RemoveDownloadById(int id)
        {
            //_service.RemoveDownloadById(id);
            if (_downloadFiles.ContainsKey(id))
                _downloadFiles.Remove(id);
        }

        public override void RemoveCompleted()
        {
            //_service.RemoveCompleted();
            int[] keys = new int[_downloadFiles.Count];
            _downloadFiles.Keys.CopyTo(keys, 0);
            foreach (int key in keys)
            {
                DownloadFileTest downloadFileTest = _downloadFiles[key];
                if (downloadFileTest.DownloadEndTime <= DateTime.Now)
                    _downloadFiles.Remove(key);
            }
        }

        public override int GetOrderedDownloadId(int i)
        {
            //return _service.GetOrderedDownloadId(i);
            if (i >= _downloadFiles.Count)
                return 0;
            int[] keys = new int[_downloadFiles.Count];
            _downloadFiles.Keys.CopyTo(keys, 0);
            return keys[i];
        }

        public override int GetDownloadOrderedIndexById(int id)
        {
            //return _service.GetDownloadOrderedIndexById(id);
            int i = 0;
            foreach (int key in _downloadFiles.Keys)
            {
                if (key == id)
                    return i;
                i++;
            }
            return -1;
        }

        public override string GetDownloadUrlById(int id)
        {
            //return _service.GetDownloadUrlById(id);
            if (_downloadFiles.ContainsKey(id))
                return _downloadFiles[id].Url;
            else
                return null;
        }

        public override string GetDownloadLocalFileById(int id)
        {
            //return _service.GetDownloadLocalFileById(id);
            if (_downloadFiles.ContainsKey(id))
                return zPath.Combine(_downloadDirectory, _downloadFiles[id].File);
            else
                return null;
        }

        public override bool GetDownloadIsWorkingById(int id)
        {
            //return _service.GetDownloadIsWorkingById(id);
            if (_downloadFiles.ContainsKey(id))
                return true;
            else
                return false;
        }

        public override DownloaderState GetDownloadStateById(int id)
        {
            //return _service.GetDownloadStateById(id);
            if (_downloadFiles.ContainsKey(id))
            {
                if (_downloadFiles[id].DownloadEndTime <= DateTime.Now)
                    return DownloaderState.Ended;
                else
                    return DownloaderState.Working;
            }
            else
                return DownloaderState.EndedWithError;
        }

        public override string GetDownloadStatusMessageById(int id)
        {
            //return _service.GetDownloadStatusMessageById(id);
            return null;
        }

        public override DateTime GetDownloadCreatedDateById(int id)
        {
            //return _service.GetDownloadCreatedDateById(id);
            if (_downloadFiles.ContainsKey(id))
                return (DateTime)_downloadFiles[id].DownloadStartTime;
            else
                return DateTime.MinValue;
        }

        public override long GetDownloadFileSizeById(int id)
        {
            //return _service.GetDownloadFileSizeById(id);
            if (_downloadFiles.ContainsKey(id))
            {
                string file = zPath.Combine(_downloadDirectory, _downloadFiles[id].File);
                if (zFile.Exists(file))
                    //return new FileInfo(file).Length;
                    return zFile.CreateFileInfo(file).Length;
            }
            return 0;
        }

        public override long GetDownloadTransferedById(int id)
        {
            //return _service.GetDownloadTransferedById(id);
            return 0;
        }

        public override double GetDownloadRateById(int id)
        {
            //return _service.GetDownloadRateById(id);
            return 0;
        }

        public override TimeSpan GetDownloadLeftTimeById(int id)
        {
            //return _service.GetDownloadLeftTimeById(id);
            return TimeSpan.FromSeconds(0);
        }

        public override double GetDownloadProgressById(int id)
        {
            //return _service.GetDownloadProgressById(id);
            return 0;
        }

        public override int GetDownloadRetryCountById(int id)
        {
            //return _service.GetDownloadRetryCountById(id);
            return 0;
        }
    }
}
