using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using MyDownloader.Service;
using pb;
using pb.Data.Mongo;
using pb.IO;
using pb.Web;

namespace Download.Print
{
    //public class DownloadFile<TKey>
    public class DownloadFile_v1<TKey>
    {
        // mongo id
        public int id;                                    // mongo id in queue collection (QueueDownloadFile4)
        // request download file variables
        public TKey key;                                  // file key DownloadPostKey, ex : { server: "vosbooks.net", id: 75274 }
        public string[] downloadLinks;                    // list of download links, ex : http://ul.to/....
        public string downloadLink;                       // selected and debrided download link, ex : http://s18.alldebrid.com/dl/....
        public string file;                               // relative file path, ex : "print\\.01_quotidien\\Libération\\Libération - 2015-02-12 - no 10495.pdf"
        public DateTime? requestTime;
        public DateTime? startDownloadTime;
        // manage download file variables
        public int downloadId;                            // id from download manager
        public DownloadState state;                       // download state : WaitToDownload or DownloadStarted
    }

    [BsonIgnoreExtraElements]
    public class DownloadedFile_v1<TKey>
    {
        // mongo id
        public int id;
        public TKey key;
        public string[] downloadLinks;
        public string downloadLink;
        public string file;
        public DownloadState state;
        public string downloadedFile;
        public string[] uncompressFiles;
        public DateTime? requestTime;
        public DateTime? startDownloadTime;
        public DateTime? endDownloadTime;
        public TimeSpan? downloadDuration;

        public DownloadedFile<TKey> ToDownloadedFile()
        {
            return new DownloadedFile<TKey>
            {
                Id = id,
                Key = key,
                State = state,
                //DownloadItemLinks = DownloadItemLink.CreateDownloadItemLinkArray(PostDownloadLinks.Create(downloadLinks)),
                DownloadItemLinks = CreateDownloadItemLinkArray(),
                UncompressFiles = uncompressFiles,
                RequestTime = requestTime,
                StartDownloadTime = startDownloadTime,
                EndDownloadTime = endDownloadTime,
                DownloadDuration = downloadDuration
            };
        }

        private DownloadItemLink[] CreateDownloadItemLinkArray()
        {
            //IRequestDownloadItemLink[] requestItemLinks = requestDownloadLinks.GetLinks();

            //DownloadItemLink[] downloadItemLinks = new DownloadItemLink[downloadLinks.Length + 1];
            //List<DownloadItemLink> downloadItemLinks = new List<DownloadItemLink>();
            //for (int i1 = 0; i1 < downloadLinks.Length; i1++)
            //foreach (string link in downloadLinks)
            //{
                //IRequestDownloadItemLink requestItemLink = requestItemLinks[i1];
                DownloadItemLink downloadItemLink = new DownloadItemLink();
                //downloadItemLink.Name = requestItemLink.GetName();
                downloadItemLink.Downloaded = false;
                //IRequestDownloadServerLink[] requestFileLinks = requestItemLink.GetServerLinks();
                downloadItemLink.ServerLinks = new DownloadServerLink[downloadLinks.Length + 1];

                int i2 = 0;

                DownloadServerLink fileLink = new DownloadServerLink();
                fileLink.Name = "DownloadedLink";
                fileLink.FilePartLinks = new DownloadFilePartLink[1];
                DownloadFilePartLink filePartLink = new DownloadFilePartLink();
                filePartLink.Downloaded = false;
                filePartLink.DownloadLink = downloadLink;
                filePartLink.State = state;
                filePartLink.DownloadedFile = file;
                filePartLink.File = file;
                filePartLink.StartDownloadTime = startDownloadTime;
                filePartLink.EndDownloadTime = endDownloadTime;

                filePartLink.DownloadDuration = downloadDuration;
                fileLink.FilePartLinks[0] = filePartLink;

                downloadItemLink.ServerLinks[i2++] = fileLink;

                //for (int i2 = 0; i2 < requestFileLinks.Length; i2++)
                foreach (string link in downloadLinks)
                {
                    //IRequestDownloadServerLink requestFileLink = requestFileLinks[i2];
                    fileLink = new DownloadServerLink();
                    //fileLink.Name = requestFileLink.GetName();
                    fileLink.Name = DownloadFileServerInfo.GetServerNameFromLink(link);
                    //string[] filePartLinks = requestFileLink.GetFilePartLinks();
                    fileLink.FilePartLinks = new DownloadFilePartLink[1];
                    //for (int i3 = 0; i3 < filePartLinks.Length; i3++)
                    //{
                        filePartLink = new DownloadFilePartLink();
                        filePartLink.Downloaded = false;
                        filePartLink.DownloadLink = link;
                        filePartLink.State = DownloadState.NotDownloaded;
                        fileLink.FilePartLinks[0] = filePartLink;
                    //}
                    downloadItemLink.ServerLinks[i2++] = fileLink;
                }
                //downloadItemLinks.Add(downloadItemLink);
            //}
            return new DownloadItemLink[] { downloadItemLink };
        }

        public static bool Save(MongoCollectionManager_v1<DownloadPostKey, DownloadedFile<DownloadPostKey>> mongoDownloadedFileManager, DownloadedFile<DownloadPostKey> downloadedFile)
        {
            bool saved = false;
            if (mongoDownloadedFileManager.Load(downloadedFile.Key) == null)
            {
                downloadedFile.Id = mongoDownloadedFileManager.GetNewId();
                mongoDownloadedFileManager.Save(downloadedFile.Id, downloadedFile);
                saved = true;
            }
            //Trace.WriteLine("{0} : key {1}, file \"{2}\"", saved ? "saved    " : "not saved", downloadedFile.Key, downloadedFile.UncompressFiles != null ? downloadedFile.UncompressFiles.FirstOrDefault() : "null");
            Trace.WriteLine("{0} : key {1}, file \"{2}\"", saved ? "saved    " : "not saved", downloadedFile.Key,
                downloadedFile.DownloadItemLinks != null ? downloadedFile.DownloadItemLinks.FirstOrDefault().ServerLinks.FirstOrDefault().FilePartLinks.FirstOrDefault().DownloadedFile : "null");
            return true;
        }
    }

    public class DownloadManager_v1<TKey> : AsyncManager, IDisposable
    {
        private static int __defaultMaxSimultaneousDownload = 3;  // 3 pour Alldebrid, 3 pour DebridLinkFr
        private bool _trace = false;
        //private DownloadManagerClient _downloadClient = null;
        private DownloadManagerClientBase _downloadClient = null;
        private int _maxSimultaneousDownload = __defaultMaxSimultaneousDownload;
        private MongoCollectionManager_v1<TKey, DownloadedFile_v1<TKey>> _mongoDownloadedFileManager = null;
        private MongoCollectionManager_v1<TKey, DownloadFile_v1<TKey>> _mongoQueueDownloadFileManager = null;
        private ConcurrentDictionary<int, DownloadFile_v1<TKey>> _downloadFiles = new ConcurrentDictionary<int, DownloadFile_v1<TKey>>();
        private Debrider _debrider = null;
        private UncompressManager _uncompressManager = null;
        private Action<DownloadedFile_v1<TKey>> _onDownloaded = null;

        // DownloadManagerClient downloadClient
        public DownloadManager_v1(DownloadManagerClientBase downloadClient, MongoCollectionManager_v1<TKey, DownloadedFile_v1<TKey>> mongoDownloadedFileManager,
            MongoCollectionManager_v1<TKey, DownloadFile_v1<TKey>> mongoQueueDownloadFileManager, Debrider debrider, UncompressManager uncompressManager)
        {
            _downloadClient = downloadClient;
            _mongoDownloadedFileManager = mongoDownloadedFileManager;
            _mongoQueueDownloadFileManager = mongoQueueDownloadFileManager;
            _debrider = debrider;
            _uncompressManager = uncompressManager;
        }

        public new void Dispose()
        {
            Stop();
            if (_uncompressManager != null)
                _uncompressManager.Dispose();
        }

        public bool Trace { get { return _trace; } set { _trace = value; } }

        public void StartThread()
        {
            TaskManager.StartTaskManager();
            base.Start();
        }

        public override void Stop()
        {
            TaskManager.StopTaskManager();
            base.Stop();
            if (_uncompressManager != null)
                _uncompressManager.Stop();
        }

        //public DownloadManagerClient DownloadManagerClient { get { return _downloadClient; } }
        public DownloadManagerClientBase DownloadManagerClient { get { return _downloadClient; } }
        public MongoCollectionManager_v1<TKey, DownloadedFile_v1<TKey>> MongoDownloadedFileManager { get { return _mongoDownloadedFileManager; } }
        public MongoCollectionManager_v1<TKey, DownloadFile_v1<TKey>> MongoQueueDownloadFileManager { get { return _mongoQueueDownloadFileManager; } }
        public Action<DownloadedFile_v1<TKey>> OnDownloaded { get { return _onDownloaded; } set { _onDownloaded = value; } }
        public int DownloadFilesCount { get { return _downloadFiles.Count; } }
        public long QueueDownloadFilesCount { get { return _mongoQueueDownloadFileManager.Count(); } }

        protected override void ThreadExecute()
        {
            ManageEndDownloadFiles();
            ManageNewDownloadFiles();
            ControlDownloadFiles();
        }

        // execute in thread
        private void ManageEndDownloadFiles()
        {
            foreach (KeyValuePair<int, DownloadFile_v1<TKey>> value in _downloadFiles)
            {
                DownloadFile_v1<TKey> downloadFile = value.Value;
                if (downloadFile.state == DownloadState.DownloadStarted)
                {
                    DownloaderState state = _downloadClient.GetDownloadStateById(downloadFile.downloadId);
                    if (state == DownloaderState.Ended || state == DownloaderState.EndedWithError)
                    {
                        if (state == DownloaderState.Ended)
                            downloadFile.state = DownloadState.DownloadCompleted;
                        else
                            downloadFile.state = DownloadState.DownloadFailed;
                        EndDownloadFile(downloadFile);
                        //DownloadedFile_v1<TKey> downloadedFile = new DownloadedFile_v1<TKey>
                        //{
                        //    key = downloadFile.key,
                        //    downloadLinks = downloadFile.downloadLinks,
                        //    downloadLink = downloadFile.downloadLink,
                        //    file = downloadFile.file,
                        //    state = downloadFile.state,
                        //    requestTime = downloadFile.requestTime,
                        //    startDownloadTime = downloadFile.startDownloadTime,
                        //    endDownloadTime = DateTime.Now,
                        //    downloadDuration = DateTime.Now - downloadFile.startDownloadTime
                        //};
                        //string downloadedPath = _downloadClient.GetDownloadLocalFileById(downloadFile.downloadId);
                        //string downloadDirectory = Path.GetDirectoryName(downloadFile.file);
                        ////if (_trace)
                        ////{
                        ////    pb.Trace.WriteLine("ManageEndDownloadFiles()                      : downloadFile.file : \"{0}\"", downloadFile.file);
                        ////    pb.Trace.WriteLine("ManageEndDownloadFiles()                      : _downloadClient.GetDownloadLocalFileById() : \"{0}\"", downloadedPath);
                        ////}
                        //downloadedFile.downloadedFile = zpath.PathSetDirectory(downloadedPath, downloadDirectory);
                        //downloadedFile.id = _mongoDownloadedFileManager.GetNewId();

                        //_mongoDownloadedFileManager.Save(downloadedFile.id, downloadedFile);
                        //_mongoQueueDownloadFileManager.Remove(downloadFile.id);
                        //_downloadClient.RemoveDownloadById(downloadFile.downloadId);

                        //// _uncompressFile
                        //if (_uncompressManager != null && CompressManager.IsCompressFile(downloadedPath))
                        //{
                        //    TaskManager.AddTask(new Task
                        //    {
                        //        name = "Uncompress download file",
                        //        task = () =>
                        //            {
                        //                UncompressFile(downloadedPath, downloadedFile, downloadDirectory);
                        //                _mongoDownloadedFileManager.Save(downloadedFile.id, downloadedFile);
                        //            }
                        //    });
                        //    if (_onDownloaded != null)
                        //        TaskManager.AddTask(new Task { name = "onDownloaded", task = () => _onDownloaded(downloadedFile) });
                        //}
                        //else if (_onDownloaded != null)
                        //    _onDownloaded(downloadedFile);

                        //DownloadFile<TKey> downloadFile2;
                        //if (!_downloadFiles.TryRemove(downloadFile.id, out downloadFile2))
                        //    pb.Trace.WriteLine("error unable to remove downloadFile with id {0} from ConcurrentDictionary _downloadFiles (DownloadManager<TKey>.ManageEndDownloadFiles())", downloadFile.id);
                        //continue;
                    }
                }
            }
        }

        private void EndDownloadFile(DownloadFile_v1<TKey> downloadFile)
        {
            try
            {
                DownloadedFile_v1<TKey> downloadedFile = new DownloadedFile_v1<TKey>
                {
                    key = downloadFile.key,
                    downloadLinks = downloadFile.downloadLinks,
                    downloadLink = downloadFile.downloadLink,
                    file = downloadFile.file,
                    state = downloadFile.state,
                    requestTime = downloadFile.requestTime,
                    startDownloadTime = downloadFile.startDownloadTime,
                    endDownloadTime = DateTime.Now,
                    downloadDuration = DateTime.Now - downloadFile.startDownloadTime
                };
                string downloadedPath = _downloadClient.GetDownloadLocalFileById(downloadFile.downloadId);
                string downloadDirectory = Path.GetDirectoryName(downloadFile.file);
                //if (_trace)
                //{
                //    pb.Trace.WriteLine("ManageEndDownloadFiles()                      : downloadFile.file : \"{0}\"", downloadFile.file);
                //    pb.Trace.WriteLine("ManageEndDownloadFiles()                      : _downloadClient.GetDownloadLocalFileById() : \"{0}\"", downloadedPath);
                //}
                downloadedFile.downloadedFile = zpath.PathSetDirectory(downloadedPath, downloadDirectory);
                downloadedFile.id = _mongoDownloadedFileManager.GetNewId();

                _mongoDownloadedFileManager.Save(downloadedFile.id, downloadedFile);
                _mongoQueueDownloadFileManager.Remove(downloadFile.id);
                _downloadClient.RemoveDownloadById(downloadFile.downloadId);

                // _uncompressFile
                if (downloadFile.state == DownloadState.DownloadCompleted && _uncompressManager != null && CompressManager.IsCompressFile(downloadedPath))
                {
                    TaskManager.AddTask(new Task
                    {
                        name = "Uncompress download file",
                        task = () =>
                        {
                            UncompressFile(downloadedPath, downloadedFile, downloadDirectory);
                            _mongoDownloadedFileManager.Save(downloadedFile.id, downloadedFile);
                        }
                    });
                    if (_onDownloaded != null)
                        TaskManager.AddTask(new Task { name = "onDownloaded", task = () => _onDownloaded(downloadedFile) });
                }
                else if (_onDownloaded != null)
                    _onDownloaded(downloadedFile);

                DownloadFile_v1<TKey> downloadFile2;
                if (!_downloadFiles.TryRemove(downloadFile.id, out downloadFile2))
                    pb.Trace.WriteLine("error unable to remove downloadFile with id {0} from ConcurrentDictionary _downloadFiles (DownloadManager<TKey>.ManageEndDownloadFiles())", downloadFile.id);
            }
            catch (Exception exception)
            {
                pb.Trace.WriteLine("error in DownloadManager_v1.EndDownloadFile() : {0}", exception.Message);
                pb.Trace.WriteLine(exception.StackTrace);
            }
        }

        private void ManageNewDownloadFiles()
        {
            int nb = _downloadClient.GetDownloadCount();
            if (nb >= _maxSimultaneousDownload)
                return;
            foreach (DownloadFile_v1<TKey> downloadFile in _mongoQueueDownloadFileManager.Find("{ 'queueDownloadFile.state': 'WaitToDownload' }", sort: "{ _id: 1 }"))
            {
                if (downloadFile.state == DownloadState.WaitToDownload)
                {
                    if (!GetDownloadLink(downloadFile))
                    {
                        // no link found remove post from mongo queue
                        _mongoQueueDownloadFileManager.Remove(downloadFile.id);
                        pb.Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - {1,-50} - {2,-25} - \"{3}\" - key {4}", DateTime.Now, "can't find download link", null, downloadFile.file, downloadFile.key);
                    }
                    else
                    {
                        // add download to download manager and save post to mongo queue
                        downloadFile.downloadId = _downloadClient.AddDownload(downloadFile.downloadLink, downloadFile.file, startNow: true);
                        downloadFile.state = DownloadState.DownloadStarted;
                        downloadFile.startDownloadTime = DateTime.Now;
                        _mongoQueueDownloadFileManager.Save(downloadFile.id, downloadFile);

                        if (!_downloadFiles.TryAdd(downloadFile.id, downloadFile))
                            pb.Trace.WriteLine("error adding downloadFile with id {0} to ConcurrentDictionary _downloadFiles (DownloadManager<TKey>.DownloadFile())", downloadFile.id);

                        if (++nb >= _maxSimultaneousDownload)
                            break;
                    }
                }
            }
        }

        private void ControlDownloadFiles()
        {
            foreach (KeyValuePair<int, DownloadFile_v1<TKey>> value in _downloadFiles)
            {
                DownloadFile_v1<TKey> downloadFile = value.Value;
                // downloadFile.state == DownloadState.DownloadStarted
                //if (downloadFile.state == DownloadState.WaitToDownload)
                //{
                    DownloaderState state = _downloadClient.GetDownloadStateById(downloadFile.downloadId);
                    if (state == DownloaderState.NeedToPrepare)
                    {
                        if (_downloadClient.GetDownloadRetryCountById(downloadFile.downloadId) == _downloadClient.GetMaxRetryCount())
                        {
                            downloadFile.state = DownloadState.DownloadFailed;
                            EndDownloadFile(downloadFile);
                        }
                    }
                //}
            }
        }

        private bool GetDownloadLink(DownloadFile_v1<TKey> downloadFile)
        {
            downloadFile.downloadLink = _debrider.DebridLink(downloadFile.downloadLinks);
            if (downloadFile.downloadLink != null)
            {
                downloadFile.file += zurl.GetExtension(downloadFile.downloadLink);
                return true;
            }
            else
                return false;
        }

        // execute in thread
        private void UncompressFile(string downloadedFile, DownloadedFile_v1<TKey> downloadFile, string downloadDirectory)
        {
            string[] uncompressFiles = _uncompressManager.Uncompress(downloadedFile);
            downloadFile.uncompressFiles = SetDirectoryFiles(uncompressFiles, downloadDirectory);
        }

        // not used
        private void AsyncUncompressFile(string downloadedFile, DownloadedFile_v1<TKey> downloadFile, string downloadDirectory)
        {
            _uncompressManager.AsyncUncompress(downloadedFile, uncompressFiles =>
            {
                downloadFile.uncompressFiles = SetDirectoryFiles(uncompressFiles, downloadDirectory);
                _mongoDownloadedFileManager.Save(downloadFile.id, downloadFile);
            });
        }

        public DownloadedFile_v1<TKey> GetDownloadedFile(TKey key)
        {
            return _mongoDownloadedFileManager.Load(key);
        }

        public DownloadFile_v1<TKey> GetQueueDownloadFile(TKey key)
        {
            return _mongoQueueDownloadFileManager.Load(key);
        }

        public DownloadState GetDownloadFileState(TKey key)
        {
            DownloadedFile_v1<TKey> downloadFile = GetDownloadedFile(key);
            if (downloadFile != null)
                return downloadFile.state;
            DownloadFile_v1<TKey> queueDownloadFile = GetQueueDownloadFile(key);
            if (queueDownloadFile != null)
                return queueDownloadFile.state;
            return DownloadState.NotDownloaded;
        }

        public void DownloadFile(TKey key, string[] downloadLinks, string file)
        {
            DownloadFile_v1<TKey> downloadFile = new DownloadFile_v1<TKey> { key = key, downloadLinks = downloadLinks, requestTime = DateTime.Now, file = file, downloadId = 0, state = DownloadState.WaitToDownload };
            downloadFile.id = _mongoQueueDownloadFileManager.GetNewId();
            _mongoQueueDownloadFileManager.Save(downloadFile.id, downloadFile);
        }

        private static string[] SetDirectoryFiles(string[] files, string directory)
        {
            if (files == null || files.Length == 0)
                return null;
            string[] files2 = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                int j = file.IndexOf(directory);
                if (j != -1)
                    file = file.Substring(j);
                files2[i] = file;
            }
            return files2;
        }
    }
}
