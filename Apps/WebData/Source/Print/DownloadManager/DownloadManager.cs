using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MyDownloader.Service;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Web;
using pb.IO;

namespace Download.Print
{
    public enum DownloadState
    {
        NotDownloaded = 1,     // used by GetDownloadFileState() when file is not in DownloadFile3 collection
        WaitToDownload,        // when file is added to the queue (_downloadFiles, mongo collection : QueueDownloadFile3)
        DownloadStarted,       // when file is send to _downloadClient for download
        DownloadCompleted,     // state from _downloadClient when download end
        DownloadFailed,        // state from _downloadClient when download end
        UnknowEndDownload      // not used
    }

    public interface IRequestDownloadLinks
    {
        IRequestDownloadItemLink[] GetLinks();
    }

    public interface IRequestDownloadItemLink
    {
        string GetName();                               // "Pack1", "Pack2", ""
        IRequestDownloadServerLink[] GetServerLinks();
    }

    public interface IRequestDownloadServerLink
    {
        string GetName();                  // Uploaded, Turbobit, Uptobox, 1fichier, Letitbit
        string[] GetFilePartLinks();       // .part01.rar, .part02.rar, ...
    }

    public class QueueDownloadFile
    {
        // mongo id
        public BsonValue Id = 0;
        [BsonIgnore]
        public bool Modified = false;
        // request download file variables
        //public IQuery Key;
        public ServerKey Key;
        public bool UncompleteDownload = false;
        public bool AllDownloadLinkTreated = false;
        public DownloadItemLink[] DownloadItemLinks;
        public string File = null;
        public DateTime? RequestTime = null;
        public DateTime? StartDownloadTime = null;
        public DateTime? EndDownloadTime;
        public TimeSpan? DownloadDuration;
        public int DownloadNbInProgress = 0;
    }

    public class DownloadedFile
    {
        // mongo id
        public BsonValue Id;
        //public IQuery Key;
        public ServerKey Key;
        public DownloadState State;
        public DownloadItemLink[] DownloadItemLinks;
        public string[] DownloadedFiles;
        public string[] UncompressFiles;
        public DateTime? RequestTime;
        public DateTime? StartDownloadTime;
        public DateTime? EndDownloadTime;
        public TimeSpan? DownloadDuration;
    }

    public class DownloadItemLink
    {
        public string Name = null;                               // "Pack1", "Pack2", ""
        // not used
        public bool Downloaded = false;
        public bool UncompleteDownload = false;
        public bool NoDownloadLinkFound = false;
        public int SelectedServerIndex = -1;
        public DownloadServerLink[] ServerLinks = null;
        public string[] UncompressFiles = null;

        //public static DownloadItemLink[] CreateDownloadItemLinkArray(IRequestDownloadItemLink[] requestLinks)
        public static DownloadItemLink[] CreateDownloadItemLinkArray(IRequestDownloadLinks requestDownloadLinks)
        {
            IRequestDownloadItemLink[] requestItemLinks = requestDownloadLinks.GetLinks();

            DownloadItemLink[] downloadLinks = new DownloadItemLink[requestItemLinks.Length];
            for (int i1 = 0; i1 < requestItemLinks.Length; i1++)
            {
                IRequestDownloadItemLink requestItemLink = requestItemLinks[i1];
                DownloadItemLink downloadLink = new DownloadItemLink();
                downloadLink.Name = requestItemLink.GetName();
                downloadLink.Downloaded = false;
                IRequestDownloadServerLink[] requestFileLinks = requestItemLink.GetServerLinks();
                downloadLink.ServerLinks = new DownloadServerLink[requestFileLinks.Length];
                for (int i2 = 0; i2 < requestFileLinks.Length; i2++)
                {
                    IRequestDownloadServerLink requestFileLink = requestFileLinks[i2];
                    DownloadServerLink fileLink = new DownloadServerLink();
                    fileLink.Name = requestFileLink.GetName();
                    string[] filePartLinks = requestFileLink.GetFilePartLinks();
                    fileLink.FilePartLinks = new DownloadFilePartLink[filePartLinks.Length];
                    for (int i3 = 0; i3 < filePartLinks.Length; i3++)
                    {
                        DownloadFilePartLink filePartLink = new DownloadFilePartLink();
                        filePartLink.Downloaded = false;
                        filePartLink.DownloadLink = filePartLinks[i3];
                        filePartLink.State = DownloadState.NotDownloaded;
                        fileLink.FilePartLinks[i3] = filePartLink;
                    }
                    downloadLink.ServerLinks[i2] = fileLink;
                }
                downloadLinks[i1] = downloadLink;
            }
            return downloadLinks;
        }
    }

    public class DownloadServerLink
    {
        public string Name = null;                                       // Uploaded, Turbobit, Uptobox, 1fichier, Letitbit
        public DownloadFilePartLink[] FilePartLinks = null;              // .part01.rar, .part02.rar, ...
    }

    public class DownloadFilePartLink
    {
        // not used
        public bool Downloaded = false;
        public string DownloadLink = null;
        public string ProtectedDownloadLink = null;
        public bool Debrided = false;
        public string DebridedDownloadLink = null;
        public string File = null;
        [BsonIgnore]
        public string DownloadedPath = null;
        public string DownloadedFile = null;
        public DownloadState State = DownloadState.NotDownloaded;
        public DateTime? StartDownloadTime = null;
        public DateTime? EndDownloadTime = null;
        public TimeSpan? DownloadDuration = null;
    }

    public class DownloadLinkRef
    {
        public BsonValue QueueDownloadFileId = 0;
        //public BsonValue DownloadId = 0;
        public int DownloadId = 0;
        public int ItemIndex = -1;
        public int ServerIndex = -1;
        public int FilePartIndex = -1;
        public string DebridedDownloadLink = null;
        public string File = null;
    }

    public static class DownloadManagerExtension
    {
        public static IEnumerable<DownloadFilePartLink> GetDownloadFilePartLinks(this DownloadItemLink[] downloadItemLinks)
        {
            foreach (DownloadItemLink itemLink in downloadItemLinks)
            {
                if (itemLink.SelectedServerIndex != -1)
                {
                    foreach (DownloadFilePartLink filePartLink in itemLink.ServerLinks[itemLink.SelectedServerIndex].FilePartLinks)
                    {
                        yield return filePartLink;
                    }
                }
            }
        }
    }

    public class DownloadManager : AsyncManager, IDisposable
    {
        private static int __defaultMaxSimultaneousDownload = 3;  // 3 pour Alldebrid, 3 pour DebridLinkFr
        private static bool _trace = false;
        private DownloadManagerClientBase _downloadClient = null;
        private int _maxSimultaneousDownload = __defaultMaxSimultaneousDownload;
        private MongoCollectionManager<DownloadedFile> _mongoDownloadedFileManager = null;
        private MongoCollectionManager<QueueDownloadFile> _mongoQueueDownloadFileManager = null;
        private MongoCollectionManager<DownloadLinkRef> _mongoCurrentDownloadFileManager = null;
        private ConcurrentDictionary<BsonValue, QueueDownloadFile> _queueDownloadFiles = new ConcurrentDictionary<BsonValue, QueueDownloadFile>();
        //private Dictionary<BsonValue, DownloadLinkRef_v2> _currentDownloadFiles = new Dictionary<BsonValue, DownloadLinkRef_v2>();
        private Dictionary<int, DownloadLinkRef> _currentDownloadFiles = new Dictionary<int, DownloadLinkRef>();
        private ProtectLink _protectLink = null;
        private Debrider _debrider = null;
        private UncompressManager _uncompressManager = null;
        private Action<DownloadedFile> _onDownloaded = null;

        public DownloadManager()
        {
        }

        public new void Dispose()
        {
            Stop();
            if (_uncompressManager != null)
                _uncompressManager.Dispose();
        }

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public DownloadManagerClientBase DownloadManagerClient { get { return _downloadClient; } set { _downloadClient = value; } }
        public MongoCollectionManager<DownloadedFile> MongoDownloadedFileManager { get { return _mongoDownloadedFileManager; } set { _mongoDownloadedFileManager = value; } }
        public MongoCollectionManager<QueueDownloadFile> MongoQueueDownloadFileManager { get { return _mongoQueueDownloadFileManager; } set { _mongoQueueDownloadFileManager = value; } }
        public MongoCollectionManager<DownloadLinkRef> MongoCurrentDownloadFileManager { get { return _mongoCurrentDownloadFileManager; } set { _mongoCurrentDownloadFileManager = value; } }
        public ProtectLink ProtectLink { get { return _protectLink; } set { _protectLink = value; } }
        public Debrider Debrider { get { return _debrider; } set { _debrider = value; } }
        public UncompressManager UncompressManager { get { return _uncompressManager; } set { _uncompressManager = value; } }
        public Action<DownloadedFile> OnDownloaded { get { return _onDownloaded; } set { _onDownloaded = value; } }
        public int DownloadFilesCount { get { return _queueDownloadFiles.Count; } }
        public long QueueDownloadFilesCount { get { return _mongoQueueDownloadFileManager.Count(); } }

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

        //public void AddFileToDownload(IQuery key, IRequestDownloadLinks downloadLinks, string file)
        public void AddFileToDownload(ServerKey key, IRequestDownloadLinks downloadLinks, string file)
        {
            DownloadItemLink[] downloadItemLinks = DownloadItemLink.CreateDownloadItemLinkArray(downloadLinks);

            QueueDownloadFile downloadFile = new QueueDownloadFile { Key = key, DownloadItemLinks = downloadItemLinks, RequestTime = DateTime.Now, File = file };
            downloadFile.Id = _mongoQueueDownloadFileManager.GetNewId();
            downloadFile.Modified = true;
            SaveQueueDownloadFile(downloadFile);
        }

        protected override void ThreadExecute()
        {
            if (_trace)
            {
                pb.Trace.WriteLine("DownloadManager.ThreadExecute() 01 begin");
                foreach (QueueDownloadFile downloadFile in _queueDownloadFiles.Values)
                {
                    pb.Trace.WriteLine("DownloadManager.ThreadExecute() 02                            : downloadFile.Id {0} downloadFile.DownloadNbInProgress {1} downloadFile.AllDownloadLinkTreated {2} file \"{3}\"", downloadFile.Id, downloadFile.DownloadNbInProgress, downloadFile.AllDownloadLinkTreated, downloadFile.File);
                }
            }
            ManageEndDownloadFiles();
            ManageNewDownloadFiles();
            ControlDownloadFiles();
            if (_trace)
                pb.Trace.WriteLine("DownloadManager.ThreadExecute() 03 end");
        }

        private void ManageEndDownloadFiles()
        {
            if (_trace)
                pb.Trace.WriteLine("DownloadManager.ManageEndDownloadFiles() 01 begin");
            foreach (int downloadId in _currentDownloadFiles.Keys.ToArray())
            {
                DownloadLinkRef downloadLinkRef = _currentDownloadFiles[downloadId];
                DownloaderState state = _downloadClient.GetDownloadStateById(downloadLinkRef.DownloadId);
                if (state == DownloaderState.Ended || state == DownloaderState.EndedWithError || state == DownloaderState.UnknowDownload)
                {
                    QueueDownloadFile downloadFile = GetQueueDownloadFile(downloadLinkRef.QueueDownloadFileId);
                    downloadFile.Modified = true;
                    DownloadFilePartLink filePartLink = GetDownloadFilePartLink(downloadLinkRef);
                    if (state == DownloaderState.Ended)
                        filePartLink.State = DownloadState.DownloadCompleted;
                    else
                    {
                        downloadFile.UncompleteDownload = true;
                        DownloadItemLink itemLink = GetDownloadItemLink(downloadLinkRef);
                        itemLink.UncompleteDownload = true;
                        filePartLink.State = DownloadState.DownloadFailed;
                    }

                    EndDownloadFilePart(downloadId);
                }
            }
            if (_trace)
                pb.Trace.WriteLine("DownloadManager.ManageEndDownloadFiles() 06 end");
        }

        private void EndDownloadFilePart(int downloadId)
        {
            try
            {
                DownloadLinkRef downloadLinkRef = _currentDownloadFiles[downloadId];
                QueueDownloadFile downloadFile = GetQueueDownloadFile(downloadLinkRef.QueueDownloadFileId);
                DownloadFilePartLink filePartLink = GetDownloadFilePartLink(downloadLinkRef);

                if (_trace)
                    pb.Trace.WriteLine("DownloadManager.ManageEndDownloadFiles() 02                   : end download file : state {0} file \"{1}\"", filePartLink.State, filePartLink.File);

                filePartLink.DownloadedPath = _downloadClient.GetDownloadLocalFileById(downloadLinkRef.DownloadId);
                filePartLink.DownloadedFile = zpath.PathSetDirectory(filePartLink.DownloadedPath, zPath.GetDirectoryName(downloadFile.File));
                filePartLink.EndDownloadTime = DateTime.Now;
                filePartLink.DownloadDuration = filePartLink.EndDownloadTime - filePartLink.StartDownloadTime;
                downloadFile.EndDownloadTime = filePartLink.EndDownloadTime;
                downloadFile.DownloadDuration = downloadFile.EndDownloadTime - downloadFile.StartDownloadTime;

                if (_trace)
                    pb.Trace.WriteLine("DownloadManager.ManageEndDownloadFiles() 03                   : _currentDownloadFiles.Remove({0})", downloadId);
                _currentDownloadFiles.Remove(downloadId);

                if (_trace)
                    pb.Trace.WriteLine("DownloadManager.ManageEndDownloadFiles() 04                   : _mongoCurrentDownloadFileManager.Remove({0})", downloadLinkRef.DownloadId);
                _mongoCurrentDownloadFileManager.Remove(downloadLinkRef.DownloadId);

                downloadFile.DownloadNbInProgress--;

                _downloadClient.RemoveDownloadById(downloadLinkRef.DownloadId);

                if (_trace)
                    pb.Trace.WriteLine("DownloadManager.ManageEndDownloadFiles() 05                   : downloadFile.Id {0} downloadFile.DownloadNbInProgress {1} downloadFile.AllDownloadLinkTreated {2} file \"{3}\"", downloadFile.Id, downloadFile.DownloadNbInProgress, downloadFile.AllDownloadLinkTreated, downloadFile.File);
                if (downloadFile.DownloadNbInProgress == 0 && downloadFile.AllDownloadLinkTreated)
                {
                    EndDownload(downloadFile);
                }
                else
                    SaveQueueDownloadFile(downloadFile);
            }
            catch (Exception exception)
            {
                pb.Trace.WriteLine("error in DownloadManager.EndDownloadFilePart() : {0}", exception.Message);
                pb.Trace.WriteLine(exception.StackTrace);
            }
        }

        private void EndDownload(QueueDownloadFile queueDownloadFile)
        {
            try
            {
                if (_trace)
                    pb.Trace.WriteLine("DownloadManager.EndDownload() 01                              : file \"{0}\"", queueDownloadFile.File);

                DownloadedFile downloadedFile = new DownloadedFile();
                downloadedFile.Key = queueDownloadFile.Key;
                if (queueDownloadFile.UncompleteDownload)
                    downloadedFile.State = DownloadState.DownloadFailed;
                else
                    downloadedFile.State = DownloadState.DownloadCompleted;
                downloadedFile.DownloadItemLinks = queueDownloadFile.DownloadItemLinks;
                downloadedFile.DownloadedFiles = queueDownloadFile.DownloadItemLinks.GetDownloadFilePartLinks().Select(filePartLink => filePartLink.DownloadedFile).ToArray();
                downloadedFile.RequestTime = queueDownloadFile.RequestTime;
                downloadedFile.StartDownloadTime = queueDownloadFile.StartDownloadTime;
                downloadedFile.EndDownloadTime = queueDownloadFile.EndDownloadTime;
                downloadedFile.DownloadDuration = queueDownloadFile.DownloadDuration;

                _mongoDownloadedFileManager.RemoveFromKey(downloadedFile.Key);
                downloadedFile.Id = _mongoDownloadedFileManager.GetNewId();
                _mongoDownloadedFileManager.Save(downloadedFile.Id, downloadedFile);
                _mongoQueueDownloadFileManager.Remove(queueDownloadFile.Id);

                // _uncompressFile
                if (_uncompressManager != null && ContainsCompressFiles(queueDownloadFile))
                {
                    TaskManager.AddTask(new Task
                    {
                        name = "Uncompress download file",
                        task = () =>
                        {
                            string[] uncompressFiles = UncompressFiles(queueDownloadFile.DownloadItemLinks.GetDownloadFilePartLinks().Select(filePartLink => filePartLink.DownloadedPath).ToArray());
                            downloadedFile.UncompressFiles = SetDirectoryFiles(uncompressFiles, zPath.GetDirectoryName(queueDownloadFile.File));
                            _mongoDownloadedFileManager.Save(downloadedFile.Id, downloadedFile);
                        }
                    });
                    if (_onDownloaded != null)
                        TaskManager.AddTask(new Task { name = "onDownloaded", task = () => _onDownloaded(downloadedFile) });
                }
                else if (_onDownloaded != null)
                    _onDownloaded(downloadedFile);

                QueueDownloadFile downloadFile2;
                if (!_queueDownloadFiles.TryRemove(queueDownloadFile.Id, out downloadFile2))
                {
                    pb.Trace.WriteLine("error unable to remove downloadFile with id {0} from ConcurrentDictionary _downloadFiles (DownloadManager<TKey>.ManageEndDownloadFiles())", queueDownloadFile.Id);
                }
                _mongoQueueDownloadFileManager.Remove(queueDownloadFile.Id);
            }
            catch (Exception exception)
            {
                pb.Trace.WriteLine("error in DownloadManager.EndDownload() : {0}", exception.Message);
                pb.Trace.WriteLine(exception.StackTrace);
            }
        }

        private void ManageNewDownloadFiles()
        {
            if (_trace)
                pb.Trace.WriteLine("DownloadManager.ManageNewDownloadFiles() 01 begin");

            int nb = _downloadClient.GetDownloadCount();

            if (_trace)
                pb.Trace.WriteLine("DownloadManager.ManageNewDownloadFiles() 02                   : search new download link : current download count {0}", nb);

            if (nb >= _maxSimultaneousDownload)
            {
                if (_trace)
                    pb.Trace.WriteLine("DownloadManager.ManageNewDownloadFiles() 09 end");
                return;
            }

            foreach (QueueDownloadFile downloadFile in _mongoQueueDownloadFileManager.Find("{}", sort: "{ _id: 1 }"))
            {
                QueueDownloadFile downloadFile2 = downloadFile;
                if (_queueDownloadFiles.ContainsKey(downloadFile2.Id))
                {
                    downloadFile2 = GetQueueDownloadFile(downloadFile2.Id);
                }

                while (true)
                {
                    if (!_queueDownloadFiles.ContainsKey(downloadFile2.Id))
                    {
                        if (!_queueDownloadFiles.TryAdd(downloadFile2.Id, downloadFile2))
                            pb.Trace.WriteLine("error adding QueueDownloadFile id {0} to ConcurrentDictionary _queueDownloadFiles", downloadFile2.Id);
                        if (_trace)
                            pb.Trace.WriteLine("DownloadManager.ManageNewDownloadFiles() 05                   : add to _queueDownloadFiles, downloadFile.Id {0} downloadFile.Key {1} downloadFile.File \"{2}\"", downloadFile2.Id, downloadFile2.Key, downloadFile2.File);
                    }

                    DownloadLinkRef downloadLinkRef = GetNextDownloadLink(downloadFile2);
                    if (downloadLinkRef == null)
                    {
                        if (_trace)
                        {
                            pb.Trace.WriteLine("DownloadManager.ManageNewDownloadFiles() 03                   : no more link to download for \"{0}\"", downloadFile2.File);
                            pb.Trace.WriteLine("DownloadManager.ManageNewDownloadFiles() 04                   : downloadFile.Id {0} downloadFile.DownloadNbInProgress {1} downloadFile.AllDownloadLinkTreated {2} file \"{3}\"", downloadFile2.Id, downloadFile2.DownloadNbInProgress, downloadFile2.AllDownloadLinkTreated, downloadFile2.File);
                        }

                        // sauver dans mongo Downloaded _mongoDownloadedFileManager
                        if (downloadFile2.DownloadNbInProgress == 0)
                            EndDownload(downloadFile2);
                        else
                            SaveQueueDownloadFile(downloadFile2);
                        break;
                    }
                    else
                    {
                        downloadLinkRef.DownloadId = _downloadClient.AddDownload(downloadLinkRef.DebridedDownloadLink, downloadLinkRef.File, startNow: true);
                        downloadFile2.Modified = true;

                        if (_trace)
                            pb.Trace.WriteLine("DownloadManager.ManageNewDownloadFiles() 06                   : start download DownloadId {0} file \"{1}\"", downloadLinkRef.DownloadId, downloadLinkRef.File);

                        DownloadFilePartLink filePartLink = GetDownloadFilePartLink(downloadLinkRef);
                        if (filePartLink != null)
                        {
                            filePartLink.State = DownloadState.DownloadStarted;
                            filePartLink.StartDownloadTime = DateTime.Now;
                        }
                        if (downloadFile2.StartDownloadTime == null)
                            downloadFile2.StartDownloadTime = DateTime.Now;
                        downloadFile2.DownloadNbInProgress++;
                        SaveQueueDownloadFile(downloadFile2);

                        if (_trace)
                        {
                            pb.Trace.WriteLine("DownloadManager.ManageNewDownloadFiles() 07                   : downloadFile.Id {0} downloadFile.DownloadNbInProgress {1} downloadFile.AllDownloadLinkTreated {2} file \"{3}\"", downloadFile2.Id, downloadFile2.DownloadNbInProgress, downloadFile2.AllDownloadLinkTreated, downloadFile2.File);
                            pb.Trace.WriteLine("DownloadManager.ManageNewDownloadFiles() 08                   : _currentDownloadFiles.Add({0})", downloadLinkRef.DownloadId);
                        }

                        _currentDownloadFiles.Add(downloadLinkRef.DownloadId, downloadLinkRef);
                        _mongoCurrentDownloadFileManager.Save(BsonValue.Create(downloadLinkRef.DownloadId), downloadLinkRef);

                        if (++nb >= _maxSimultaneousDownload)
                            break;
                    }
                }
                if (nb >= _maxSimultaneousDownload)
                    break;
            }
            if (_trace)
                pb.Trace.WriteLine("DownloadManager.ManageNewDownloadFiles() 09 end");
        }

        private void ControlDownloadFiles()
        {
            foreach (int downloadId in _currentDownloadFiles.Keys.ToArray())
            {
                DownloadLinkRef downloadLinkRef = _currentDownloadFiles[downloadId];
                DownloaderState state = _downloadClient.GetDownloadStateById(downloadLinkRef.DownloadId);
                if (state == DownloaderState.NeedToPrepare)
                {
                    if (_downloadClient.GetDownloadRetryCountById(downloadLinkRef.DownloadId) == _downloadClient.GetMaxRetryCount())
                    {
                        QueueDownloadFile downloadFile = GetQueueDownloadFile(downloadLinkRef.QueueDownloadFileId);
                        downloadFile.Modified = true;
                        DownloadFilePartLink filePartLink = GetDownloadFilePartLink(downloadLinkRef);

                        downloadFile.UncompleteDownload = true;
                        DownloadItemLink itemLink = GetDownloadItemLink(downloadLinkRef);
                        itemLink.UncompleteDownload = true;
                        filePartLink.State = DownloadState.DownloadFailed;

                        EndDownloadFilePart(downloadId);
                    }
                }
            }
        }

        private DownloadLinkRef GetNextDownloadLink(QueueDownloadFile downloadFile)
        {
            if (downloadFile.AllDownloadLinkTreated)
                return null;
            int itemIndex = 0;
            foreach (DownloadItemLink itemLink in downloadFile.DownloadItemLinks)
            {
                if (!itemLink.Downloaded)
                {
                    if (!itemLink.NoDownloadLinkFound)
                    {
                        if (itemLink.SelectedServerIndex == -1)
                        {
                            var q = (from server in itemLink.ServerLinks select new { server = server, rate = DownloadFileServerInfo.GetLinkRate(server.Name) }).OrderBy(server => server.rate).Select(server => server.server);
                            int serverIndex = 0;
                            foreach (DownloadServerLink serverLink in q)
                            {
                                DownloadFilePartLink filePartLink = serverLink.FilePartLinks[0];
                                if (!UnprotectLink(downloadFile, serverLink, filePartLink, 0))
                                    continue;
                                string debridedLink = null;
                                string file = null;
                                if (DebridLink(downloadFile, itemLink, serverLink, filePartLink, out debridedLink, out file))
                                {
                                    itemLink.SelectedServerIndex = serverIndex;
                                    downloadFile.Modified = true;
                                    return new DownloadLinkRef
                                        { QueueDownloadFileId = downloadFile.Id, ItemIndex = itemIndex, ServerIndex = serverIndex, FilePartIndex = 0, DebridedDownloadLink = debridedLink, File = file };
                                }
                                serverIndex++;
                            }
                            itemLink.NoDownloadLinkFound = true;
                            downloadFile.UncompleteDownload = true;
                            downloadFile.Modified = true;
                            string message = string.Format("can't find download link for item no {0}", itemIndex + 1);
                            pb.Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - {1,-50} - {2,-25} - file \"{3}\" key {4}", DateTime.Now, message, null, downloadFile.File, downloadFile.Key);
                        }
                        else
                        {
                            DownloadServerLink serverLink = itemLink.ServerLinks[itemLink.SelectedServerIndex];
                            // dont use foreach because UnprotectLink() may change serverLink.FilePartLinks
                            for (int filePartIndex = 0; filePartIndex < serverLink.FilePartLinks.Length; filePartIndex++)
                            {
                                DownloadFilePartLink filePartLink = serverLink.FilePartLinks[filePartIndex];
                                if (filePartLink.State != DownloadState.NotDownloaded)
                                    continue;
                                string errorMessage = null;
                                if (UnprotectLink(downloadFile, serverLink, filePartLink, filePartIndex))
                                {

                                    string debridedLink = null;
                                    string file = null;
                                    if (DebridLink(downloadFile, itemLink, serverLink, filePartLink, out debridedLink, out file))
                                    {
                                        return new DownloadLinkRef { QueueDownloadFileId = downloadFile.Id, ItemIndex = itemIndex, ServerIndex = itemLink.SelectedServerIndex, FilePartIndex = filePartIndex, DebridedDownloadLink = debridedLink, File = file };
                                    }
                                    else
                                    {
                                        errorMessage = "can't find download link";
                                    }
                                }
                                else
                                {
                                    errorMessage = "error unable to unprotect link";
                                }
                                if (errorMessage != null)
                                {
                                    downloadFile.UncompleteDownload = true;
                                    downloadFile.Modified = true;
                                    errorMessage = errorMessage + string.Format(" for item no {0} server no {1} part no {2}", itemIndex + 1, itemLink.SelectedServerIndex + 1, filePartIndex + 1);
                                    pb.Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - {1,-50} - {2,-25} - link \"{3}\" file \"{4}\" key {5}", DateTime.Now, errorMessage, null, filePartLink.DownloadLink, downloadFile.File, downloadFile.Key);
                                }
                            }
                        }
                    }
                }
                itemIndex++;
            }
            downloadFile.AllDownloadLinkTreated = true;
            downloadFile.Modified = true;
            return null;
        }

        private bool UnprotectLink(QueueDownloadFile downloadFile, DownloadServerLink serverLink, DownloadFilePartLink filePartLink, int filePartLinkIndex)
        {
            if (_protectLink != null && _protectLink.IsLinkProtected(filePartLink.DownloadLink))
            {
                string[] links = _protectLink.UnprotectLink(filePartLink.DownloadLink);

                if (_trace)
                    pb.Trace.WriteLine("DownloadManager.UnprotectLink() 01                            : _protectLink.UnprotectLink(\"{0}\") : {1}", filePartLink.DownloadLink, links.zToStringValues());

                if (links == null || links.Length == 0)
                {
                    pb.Trace.WriteLine("error unable to unprotect link \"{0}\"", filePartLink.DownloadLink);
                    return false;
                }
                downloadFile.Modified = true;
                filePartLink.ProtectedDownloadLink = filePartLink.DownloadLink;
                filePartLink.DownloadLink = links[0];
                if (links.Length > 1)
                {
                    List<DownloadFilePartLink> filePartLinks = new List<DownloadFilePartLink>();
                    for (int i = 0; i <= filePartLinkIndex; i++)
                        filePartLinks.Add(serverLink.FilePartLinks[i]);
                    for (int i = 1; i < links.Length; i++)
                    {
                        DownloadFilePartLink filePartLink2 = new DownloadFilePartLink();
                        filePartLink2.Downloaded = false;
                        filePartLink2.DownloadLink = links[i];
                        filePartLink2.State = DownloadState.NotDownloaded;
                        filePartLinks.Add(filePartLink2);
                    }
                    for (int i = filePartLinkIndex + 1; i < serverLink.FilePartLinks.Length; i++)
                        filePartLinks.Add(serverLink.FilePartLinks[i]);
                    serverLink.FilePartLinks = filePartLinks.ToArray();
                }
            }
            return true;
        }

        private bool DebridLink(QueueDownloadFile downloadFile, DownloadItemLink itemLink, DownloadServerLink serverLink, DownloadFilePartLink filePartLink, out string debridedLink, out string file)
        {
            if (!filePartLink.Debrided)
            {
                // http://s19.alldebrid.com/dl/f3nmdg2f05/Herc-FULLBluRay.part01.rar
                debridedLink = _debrider.DebridLink(filePartLink.DownloadLink);
                filePartLink.Debrided = true;
                downloadFile.Modified = true;
                if (debridedLink != null)
                {
                    filePartLink.DebridedDownloadLink = debridedLink;
                    file = downloadFile.File;
                    if (downloadFile.DownloadItemLinks.Length > 1)
                        file += "_" + itemLink.Name;
                    if (serverLink.FilePartLinks.Length > 1)
                        file += ZipManager.GetZipFilePartName(zurl.GetFileName(debridedLink));
                    file += zurl.GetExtension(debridedLink);
                    filePartLink.File = file;

                    if (_trace)
                        pb.Trace.WriteLine("DownloadManager.DebridLink() 01                               : _debrider.DebridLink(\"{0}\") : \"{1}\"", filePartLink.DownloadLink, debridedLink);

                    return true;
                }
            }
            debridedLink = null;
            file = null;
            return false;
        }

        //public DownloadState GetDownloadFileState(IQuery key)
        public DownloadState GetDownloadFileState(ServerKey key)
        {
            DownloadedFile downloadedFile = GetDownloadedFile(key);
            if (downloadedFile != null)
                return downloadedFile.State;
            QueueDownloadFile queueDownloadFile = GetQueueDownloadFile(key);
            if (queueDownloadFile != null)
                return DownloadState.WaitToDownload;
            return DownloadState.NotDownloaded;
        }

        public QueueDownloadFile GetQueueDownloadFile(IQuery key)
        {
            return _mongoQueueDownloadFileManager.LoadFromKey(key);
        }

        private QueueDownloadFile GetQueueDownloadFile(BsonValue id)
        {
            QueueDownloadFile downloadFile = null;
            if (!_queueDownloadFiles.TryGetValue(id, out downloadFile))
            {
                pb.Trace.WriteLine("error get QueueDownloadFile id {0} from ConcurrentDictionary _queueDownloadFiles", id);
                return null;
            }
            return downloadFile;
        }

        public DownloadedFile GetDownloadedFile(IQuery key)
        {
            return _mongoDownloadedFileManager.LoadFromKey(key);
        }

        private DownloadItemLink GetDownloadItemLink(DownloadLinkRef downloadLinkRef)
        {
            QueueDownloadFile downloadFile = GetQueueDownloadFile(downloadLinkRef.QueueDownloadFileId);
            if (downloadFile != null)
                return downloadFile.DownloadItemLinks[downloadLinkRef.ItemIndex];
            else
                return null;
        }

        private DownloadFilePartLink GetDownloadFilePartLink(DownloadLinkRef downloadLinkRef)
        {
            QueueDownloadFile downloadFile = GetQueueDownloadFile(downloadLinkRef.QueueDownloadFileId);
            if (downloadFile != null)
                return downloadFile.DownloadItemLinks[downloadLinkRef.ItemIndex].ServerLinks[downloadLinkRef.ServerIndex].FilePartLinks[downloadLinkRef.FilePartIndex];
            else
                return null;
        }

        private bool ContainsCompressFiles(QueueDownloadFile downloadFile)
        {
            foreach (DownloadItemLink itemLink in downloadFile.DownloadItemLinks)
            {
                if (itemLink.SelectedServerIndex != -1)
                {
                    foreach (DownloadFilePartLink filePartLink in itemLink.ServerLinks[itemLink.SelectedServerIndex].FilePartLinks)
                    {
                        if (CompressManager.IsCompressFile(filePartLink.DownloadedPath))
                            return true;
                    }
                }
            }
            return false;
        }

        // execute in thread
        private string[] UncompressFiles(string[] downloadedPaths)
        {
            List<string> uncompressFileList = new List<string>();
            foreach (string downloadedPath in downloadedPaths)
            {
                if (CompressManager.IsCompressFile(downloadedPath))
                {
                    string[] uncompressFiles = _uncompressManager.Uncompress(downloadedPath);
                    uncompressFileList.AddRange(uncompressFiles);
                }
            }
            return uncompressFileList.ToArray();
        }

        private void SaveQueueDownloadFile(QueueDownloadFile downloadFile)
        {
            if (downloadFile.Modified)
            {
                _mongoQueueDownloadFileManager.Save(downloadFile.Id, downloadFile);
                downloadFile.Modified = false;
            }
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
