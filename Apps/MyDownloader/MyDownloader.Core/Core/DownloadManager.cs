using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using System.Threading;
using MyDownloader.Core.Concurrency;
using MyDownloader.Service;

namespace MyDownloader.Core
{
    public delegate void OnChangeDelegate();

    public class DownloadManager
    {
        #region Singleton

        private static DownloadManager instance = new DownloadManager();

        public static DownloadManager Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion

        #region Fields

        public OnChangeDelegate OnChange;
        private string defaultDownloadDirectorySource = null; // @"..\dl"
        private string defaultDownloadDirectory = null;
        private int lastDownloadId = 0;
        //private List<Downloader> downloads = new List<Downloader>();
        private Dictionary<int, Downloader> downloads = new Dictionary<int, Downloader>();
        private List<int> orderedDownloads = new List<int>();
        private int addBatchCount;
        private ReaderWriterObjectLocker downloadListSync = new ReaderWriterObjectLocker();

        #endregion

        #region Properties

        public event EventHandler BeginAddBatchDownloads;

        public event EventHandler EndAddBatchDownloads;

        public event EventHandler<DownloaderEventArgs> DownloadEnded;

        public event EventHandler<DownloaderEventArgs> DownloadAdded;

        public event EventHandler<DownloaderEventArgs> DownloadRemoved;

        public string DefaultDownloadDirectorySource { get { return defaultDownloadDirectorySource; } set { defaultDownloadDirectorySource = value; } }
        public string DefaultDownloadDirectory { get { return defaultDownloadDirectory; } set { defaultDownloadDirectory = value; } }
        public int LastDownloadId { get { return lastDownloadId; } set { lastDownloadId = value; } }

        //public ReadOnlyCollection<Downloader> Downloads
        //{
        //    get
        //    {
        //        return downloads.AsReadOnly();
        //    }
        //}

        public IEnumerable<Downloader> Downloads
        {
            get
            {
                return downloads.Values;
            }
        }

        public IEnumerable<Downloader> OrderedDownloads
        {
            get
            {
                foreach (int id in orderedDownloads)
                {
                    yield return downloads[id];
                }
            }
        }

        public double TotalDownloadRate
        {
            get 
            {
                double total = 0;

                using (LockDownloadList(false))
                {
                    //for (int i = 0; i < this.Downloads.Count; i++)
                    foreach (Downloader download in downloads.Values)
                    {
                        //if (this.Downloads[i].State == DownloaderState.Working)
                        if (download.State == DownloaderState.Working)
                        {
                            //total += this.Downloads[i].Rate;
                            total += download.Rate;
                        }
                    }
                }

                return total; 
            }
        }

        public int AddBatchCount
        {
            get
            {
                return addBatchCount;
            }
        }

        #endregion 

        #region Methods

        public bool HasDownloadStarted()
        {
            foreach (Downloader downloader in OrderedDownloads)
            {
                if (downloader.State == DownloaderState.Working)
                    return true;
            }
            return false;
        }

        public int GetDownloadCount()
        {
            return downloads.Count;
        }

        public Downloader GetOrderedDownloader(int index)
        {
            if (index >= orderedDownloads.Count)
                return null;
            int id = orderedDownloads[index];
            if (!downloads.ContainsKey(id))
                return null;
            return downloads[id];
        }

        public Downloader GetDownloaderById(int id)
        {
            // $$pb modif le 17/02/2015
            if (downloads.ContainsKey(id))
                return downloads[id];
            else
                return null;
        }

        void downloader_StateChanged(object sender, EventArgs e)
        {
            Downloader downloader = (Downloader)sender;

            if (downloader.State == DownloaderState.Ended ||
                downloader.State == DownloaderState.EndedWithError)
            {
                OnDownloadEnded((Downloader)sender);
            }
        }

        public IDisposable LockDownloadList(bool lockForWrite)
        {
            if (lockForWrite)
            {
                return downloadListSync.LockForWrite();
            }
            else
            {
                return downloadListSync.LockForRead();
            }
        }

        public void RemoveDownloadByOrderedIndex(int index)
        {
            //RemoveDownload(downloads[index]);
            RemoveDownload(downloads[orderedDownloads[index]]);
        }

        public void RemoveDownloadById(int id)
        {
            RemoveDownload(downloads[id]);
        }

        public void RemoveDownload(Downloader downloader)
        {
            if (downloader.State != DownloaderState.NeedToPrepare ||
                downloader.State != DownloaderState.Ended ||
                downloader.State != DownloaderState.Paused)
            {
                downloader.Pause();
            }

            using (LockDownloadList(true))
            {
                //downloads.Remove(downloader);
                int id = orderedDownloads[downloader.OrderedIndex];
                if (downloader.Id != id)
                    throw new Exception(string.Format("error cant remove downloader id {0} ordered index {1}, downloader id in ordered list is {2}", downloader.Id, downloader.OrderedIndex, id));
                orderedDownloads.RemoveAt(downloader.OrderedIndex);
                downloads.Remove(downloader.Id);
                // refresh ordered index
                int i = 0;
                foreach (int id2 in orderedDownloads)
                    downloads[id2].OrderedIndex = i++;
            }

            OnDownloadRemoved(downloader);
        }

        public void ClearEnded()
        {
            using (LockDownloadList(true))
            {
                for (int i = downloads.Count - 1; i >= 0; i--)
                {
                    Downloader downloader = GetOrderedDownloader(i);
                    if (downloader.State == DownloaderState.Ended)
                    {
                        //Downloader d = downloads[i];
                        //downloads.RemoveAt(i);
                        RemoveDownload(downloader);
                        //OnDownloadRemoved(downloader);
                    }
                }
                //foreach (Downloader downloader in OrderedDownloads)
                //{
                //    if (downloader.State == DownloaderState.Ended)
                //        RemoveDownload(downloader);
                //}
            }
        }

        public void StartAll()
        {
            using (LockDownloadList(false))
            {
                //for (int i = 0; i < this.Downloads.Count; i++)
                //{
                //    this.Downloads[i].Start();
                //}
                foreach (Downloader download in OrderedDownloads)
                {
                    download.Start();
                }
                OnChange();
            }
        }

        public void PauseAll()
        {
            using (LockDownloadList(false))
            {
                //for (int i = 0; i < this.Downloads.Count; i++)
                //{
                //    this.Downloads[i].Pause();
                //}
                foreach (Downloader download in OrderedDownloads)
                {
                    download.Pause();
                }
                OnChange();
            }
        }

        public Downloader Add(ResourceLocation rl, ResourceLocation[] mirrors, string localFile, int segments, bool autoStart)
        {
            //Downloader d = new Downloader(rl, mirrors, localFile, segments);
            Downloader d = new Downloader(++lastDownloadId, rl, mirrors, localFile, segments);
            Add(d, autoStart);

            return d;
        }

        public Downloader Add(ResourceLocation rl, ResourceLocation[] mirrors, string localFile, List<Segment> segments, RemoteFileInfo remoteInfo, int requestedSegmentCount, bool autoStart, DateTime createdDateTime)
        {
            //Downloader d = new Downloader(rl, mirrors, localFile, segments, remoteInfo, requestedSegmentCount, createdDateTime);
            Downloader d = new Downloader(++lastDownloadId, rl, mirrors, localFile, segments, remoteInfo, requestedSegmentCount, createdDateTime);
            Add(d, autoStart);

            return d;
        }

        public void Add(Downloader downloader, bool autoStart)
        {
            downloader.StateChanged += new EventHandler(downloader_StateChanged);

            using (LockDownloadList(true))
            {
                //downloads.Add(downloader);
                downloader.OrderedIndex = orderedDownloads.Count;
                downloads.Add(downloader.Id, downloader);
                orderedDownloads.Add(downloader.Id);
            }

            OnDownloadAdded(downloader, autoStart);

            if (autoStart)
            {
                downloader.Start();
            }
        }

        public virtual void OnBeginAddBatchDownloads()
        {
            addBatchCount++;

            if (BeginAddBatchDownloads != null)
            {
                BeginAddBatchDownloads(this, EventArgs.Empty);
            }
        }

        public virtual void OnEndAddBatchDownloads()
        {
            addBatchCount--;

            if (EndAddBatchDownloads != null)
            {
                EndAddBatchDownloads(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDownloadEnded(Downloader d)
        {
            if (DownloadEnded != null)
            {
                DownloadEnded(this, new DownloaderEventArgs(d));
            }
        }

        protected virtual void OnDownloadAdded(Downloader d, bool willStart)
        {
            if (DownloadAdded != null)
            {
                DownloadAdded(this, new DownloaderEventArgs(d, willStart));
            }
        }

        protected virtual void OnDownloadRemoved(Downloader d)
        {
            if (DownloadRemoved != null)
            {
                DownloadRemoved(this, new DownloaderEventArgs(d));
            }
        }

        public void SwapDownloads(int idx, bool isThreadSafe)
        {
            if (isThreadSafe)
            {
                InternalSwap(idx);
            }
            else
            {
                using (LockDownloadList(true))
                {
                    InternalSwap(idx);
                }
            }
        }

        private void InternalSwap(int idx)
        {
            //if (this.downloads.Count <= idx)
            //{
            //    //return;
            //}

            //Downloader it1 = this.downloads[idx];
            //Downloader it2 = this.downloads[idx - 1];

            //this.downloads.RemoveAt(idx);
            //this.downloads.RemoveAt(idx - 1);

            //this.downloads.Insert(idx - 1, it1);
            //this.downloads.Insert(idx, it2);

            Downloader downloader1 = downloads[orderedDownloads[idx]];
            if (downloader1.OrderedIndex != idx)
                throw new Exception(string.Format("error cant swap downloader id {0} ordered index {1}, id index in ordered list is {2}", downloader1.Id, downloader1.OrderedIndex, idx));
            Downloader downloader2 = downloads[orderedDownloads[idx - 1]];
            if (downloader2.OrderedIndex != idx - 1)
                throw new Exception(string.Format("error cant swap downloader id {0} ordered index {1}, id index in ordered list is {2}", downloader2.Id, downloader2.OrderedIndex, idx - 1));
            orderedDownloads[idx] = downloader2.Id;
            orderedDownloads[idx - 1] = downloader1.Id;
            downloader1.OrderedIndex = idx - 1;
            downloader2.OrderedIndex = idx;
        }

        #endregion


    }
}
