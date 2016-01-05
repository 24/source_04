using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using pb;
using pb.Compiler;
using pb.Data;
using pb.IO;
using pb.Web;
using pb.Web.Data;
using Print;
using pb.Data.old;

namespace Download.Print.old
{
    // used by mongo
    public interface IPostToDownload_v1 : IKeyData_v4<int>, IHttpRequestData
    {
        string GetServer();
        string GetTitle();
        PrintType GetPrintType();
        string[] GetDownloadLinks();
        PostDownloadLinks GetDownloadLinks_new();
    }

    public class DownloadPost_v1
    {
        public DownloadPostKey_v1 key;
        public string downloadLink;
        public string file;
    }

    public class DownloadPostKey_v1
    {
        public string server;
        public int id;
        public override string ToString()
        {
            return string.Format("{0} id {1}", server, id);
        }
    }

    public static class ServerManagers_v1
    {
        private static Dictionary<string, ServerManager_v1> __servers = new Dictionary<string, ServerManager_v1>();

        public static void Add(string name, ServerManager_v1 serverManager)
        {
            if (__servers.ContainsKey(name))
                throw new PBException("ServerManager \"{0}\" already in ServerManagers list", name);
            __servers.Add(name, serverManager);
        }

        public static ServerManager_v1 Get(string name)
        {
            if (!__servers.ContainsKey(name))
                throw new PBException("unknow ServerManager \"{0}\" in ServerManagers list", name);
            return __servers[name];
        }
    }

    //public delegate IEnumerable<BsonDocument> GetPostInfoListDelegate(string query = null, string sort = null, int limit = 0);

    public class ServerManager_v1
    {
        public string Name;
        public bool EnableLoadNewPost;
        public bool EnableSearchPostToDownload;
        public string DownloadDirectory;
        public Action LoadNewPost = null;
        public Func<DateTime, IEnumerable<IPostToDownload_v1>> GetPostList = null;
        //public GetPostInfoListDelegate GetPostInfoList = null;
        public Func<int, IPostToDownload_v1> LoadPost = null;
        public Action Backup = null;
    }

    public class DownloadAutomateManager_v1
    {
        private Dictionary<string, ServerManager_v1> _servers_v1 = new Dictionary<string, ServerManager_v1>();
        private MongoDownloadAutomateManager _mongoDownloadAutomateManager = null;
        private Func<PrintType, bool> _downloadAllPrintType = null;
        private FindPrintManager _findPrintManager = null;
        //private PrintManager _printManager = null;
        private DownloadManager_v1<DownloadPostKey_v1> _downloadManager_v1 = null;
        private DownloadManager_v2<DownloadPostKey_v1> _downloadManager = null;
        private MailSender _mailSender = null;
        private MailMessage _mailMessage = null;
        private TimeSpan _waitTimeBetweenOperation = TimeSpan.FromSeconds(5);     // 5 sec
        private TimeSpan _mailWaitDownloadFinish = TimeSpan.FromMinutes(10);      // 10 minutes

        private StringBuilder _mailBody = new StringBuilder();
        private int _mailBodyLineNumber = 0;
        private DateTime? _mailBodyDateTime = null;

        private bool _desactivateFilterTracePost = false;

        public DownloadAutomateManager_v1()
        {
        }

        public void Dispose()
        {
            //Stop();
            if (_downloadManager_v1 != null)
                _downloadManager_v1.Dispose();
            if (_downloadManager != null)
                _downloadManager.Dispose();
        }

        public MongoDownloadAutomateManager MongoDownloadAutomateManager { get { return _mongoDownloadAutomateManager; } set { _mongoDownloadAutomateManager = value; } }
        public Func<PrintType, bool> DownloadAllPrintType { get { return _downloadAllPrintType; } set { _downloadAllPrintType = value; } }
        public FindPrintManager FindPrintManager { get { return _findPrintManager; } set { _findPrintManager = value; } }
        //public PrintManager PrintManager { get { return _printManager; } set { _printManager = value; } }
        public DownloadManager_v1<DownloadPostKey_v1> DownloadManager_v1 { get { return _downloadManager_v1; } set { _downloadManager_v1 = value; } }
        public DownloadManager_v2<DownloadPostKey_v1> DownloadManager { get { return _downloadManager; } set { _downloadManager = value; } }
        public MailSender MailSender { get { return _mailSender; } set { _mailSender = value; } }
        public MailMessage MailMessage { get { return _mailMessage; } set { _mailMessage = value; } }
        public TimeSpan WaitTimeBetweenOperation { get { return _waitTimeBetweenOperation; } set { _waitTimeBetweenOperation = value; } }
        public TimeSpan MailWaitDownloadFinish { get { return _mailWaitDownloadFinish; } set { _mailWaitDownloadFinish = value; } }
        public bool DesactivateFilterTracePost { get { return _desactivateFilterTracePost; } set { _desactivateFilterTracePost = value; } }

        public void AddServerManager(ServerManager_v1 server)
        {
            _servers_v1.Add(server.Name, server);
        }

        public virtual void Start()
        {
            if (_downloadManager_v1 != null)
            {
                _downloadManager_v1.OnDownloaded = Downloaded;
                _downloadManager_v1.StartThread();
            }
            if (_downloadManager != null)
            {
                _downloadManager.OnDownloaded = Downloaded;
                _downloadManager.StartThread();
            }
        }

        public virtual void Stop()
        {
            if (_downloadManager_v1 != null)
                _downloadManager_v1.Stop();
            if (_downloadManager != null)
                _downloadManager.Stop();
        }

        public void Run(bool loadNewPost = true, bool searchPostToDownload = true, bool uncompressFile = true, bool sendMail = false)
        {
            DateTime nextRunDateTime = _mongoDownloadAutomateManager.GetNextRunDateTime();
            bool messageNextRun = true;
            while (true)
            {
                if (RunSource.CurrentRunSource.IsExecutionAborted())
                    break;
                if (DateTime.Now >= nextRunDateTime)
                {
                    if (loadNewPost)
                        //Try(() => LoadNewPost());
                        LoadNewPost();

                    if (searchPostToDownload)
                    {
                        DateTime? lastRunDateTime = _mongoDownloadAutomateManager.GetLastRunDateTime();
                        if (lastRunDateTime == null)
                            lastRunDateTime = DateTime.Now.AddDays(-3);

                        SearchPostToDownload((DateTime)lastRunDateTime);

                        _mongoDownloadAutomateManager.SetLastRunDateTime(DateTime.Now);
                        nextRunDateTime = _mongoDownloadAutomateManager.GetNextRunDateTime();
                    }

                    messageNextRun = true;
                }
                else
                {
                    Try(() => SendMail(sendMail));
                }
                //if (!searchPostToDownload && _downloadManager.DownloadFilesCount == 0 && _downloadManager.QueueDownloadFilesCount == 0 && TaskManager.CurrentTaskManager.Count == 0)
                //Trace.WriteLine("TaskManager.CurrentTaskManager.Count {0}", TaskManager.CurrentTaskManager.Count);
                if (!searchPostToDownload && !ActiveDownload() && TaskManager.CurrentTaskManager.Count == 0)
                    break;

                //if (_downloadManager.DownloadFilesCount == 0 && _downloadManager.QueueDownloadFilesCount == 0 && TaskManager.CurrentTaskManager.Count == 0 && messageNextRun)
                if (!ActiveDownload() && TaskManager.CurrentTaskManager.Count == 0 && messageNextRun)
                {
                    Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - next run {1:dd-MM-yyyy HH:mm:ss}", DateTime.Now, nextRunDateTime);
                    messageNextRun = false;
                }

                Thread.Sleep(_waitTimeBetweenOperation);
            }
        }

        private bool ActiveDownload()
        {
            if (_downloadManager_v1 != null)
            {
                //Trace.WriteLine("_downloadManager.DownloadFilesCount      {0}", _downloadManager.DownloadFilesCount);
                //Trace.WriteLine("_downloadManager.QueueDownloadFilesCount {0}", _downloadManager.QueueDownloadFilesCount);
                if (_downloadManager_v1.DownloadFilesCount != 0 || _downloadManager_v1.QueueDownloadFilesCount != 0)
                    return true;
            }
            if (_downloadManager != null)
            {
                //Trace.WriteLine("_downloadManager_new.DownloadFilesCount      {0}", _downloadManager_new.DownloadFilesCount);
                //Trace.WriteLine("_downloadManager_new.QueueDownloadFilesCount {0}", _downloadManager_new.QueueDownloadFilesCount);
                if (_downloadManager.DownloadFilesCount != 0 || _downloadManager.QueueDownloadFilesCount != 0)
                    return true;
            }
            return false;
        }

        private void LoadNewPost()
        {
            foreach (ServerManager_v1 server in _servers_v1.Values)
            {
                try
                {
                    Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - Download new post from {1}", DateTime.Now, server.Name);
                    if (server.EnableLoadNewPost)
                        server.LoadNewPost();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("error : {0}", ex.Message);
                    Trace.WriteLine(ex.StackTrace);
                }

            }
        }

        private void SearchPostToDownload(DateTime lastRunDateTime)
        {
            //Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - Search download from {1:dd-MM-yyyy HH:mm:ss}", DateTime.Now, lastRunDateTime);
            bool download = false;
            lastRunDateTime = lastRunDateTime.AddDays(-1);
            foreach (ServerManager_v1 server in _servers_v1.Values)
            {
                if (server.EnableSearchPostToDownload)
                {
                    Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - Search download on {1} from {2:dd-MM-yyyy HH:mm:ss}", DateTime.Now, server.Name, lastRunDateTime);
                    foreach (IPostToDownload_v1 post in server.GetPostList(lastRunDateTime))
                    {
                        if (RunSource.CurrentRunSource.IsExecutionAborted())
                            break;

                        if (TryDownloadPost(post, server.DownloadDirectory))
                            download = true;
                    }
                }
            }
            if (!download)
                Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - Nothing to download", DateTime.Now);
        }

        // used to download multiple posts
        public bool TryDownloadPosts(IEnumerable<IPostToDownload_v1> posts, string downloadDirectory = null, bool forceDownloadAgain = false, bool forceSelect = false, bool simulateDownload = false)
        {
            bool ret = false;
            foreach (IPostToDownload_v1 post in posts)
            {
                if (TryDownloadPost(post, downloadDirectory, forceDownloadAgain, forceSelect, simulateDownload))
                ret = true;
            }
            return ret;
        }

        // used to download one post
        public bool TryDownloadPost(IPostToDownload_v1 post, string downloadDirectory = null, bool forceDownloadAgain = false, bool forceSelect = false, bool simulateDownload = false)
        {
            if (_downloadAllPrintType != null)
                forceSelect = forceSelect || _downloadAllPrintType(post.GetPrintType());
            FindPrintInfo findPrint = FindPrint(post.GetTitle(), post.GetPrintType(), forceSelect);
            if (!findPrint.found)
            {
                TracePost(post, "post not selected");
                return false;
            }

            // corrigé le 20/11/2014
            //DownloadPostKey key = new DownloadPostKey { server = post.server, id = post.id };
            DownloadPostKey_v1 key = new DownloadPostKey_v1 { server = post.GetServer(), id = post.GetKey() };
            // state : NotDownloaded, WaitToDownload, DownloadStarted, DownloadCompleted, DownloadFailed
            DownloadState state = GetDownloadFileState(key);
            //if (state != DownloadState.NotDownloaded && !forceDownloadAgain)
            if ((state == DownloadState.WaitToDownload || state == DownloadState.DownloadStarted || state == DownloadState.DownloadCompleted) && !forceDownloadAgain)
            {
                if (FilterTracePost(state))
                    TracePost(post, "post " + GetDownloadStateText1(state), findPrint.file);
                return false;
            }

            string file = findPrint.file;
            if (downloadDirectory != null)
                file = downloadDirectory + "\\" + file;

            if (simulateDownload)
            {
                TracePost(post, "simulate start download", file);
                return false;
            }
            else
                TracePost(post, "start download", file);

            if (_downloadManager_v1 != null)
                Try(() => _downloadManager_v1.DownloadFile(key, post.GetDownloadLinks(), file));
            if (_downloadManager != null)
                Try(() => _downloadManager.AddFileToDownload(key, post.GetDownloadLinks_new(), file));

            return true;
        }

        private DownloadState GetDownloadFileState(DownloadPostKey_v1 key)
        {
            if (_downloadManager_v1 != null)
                return _downloadManager_v1.GetDownloadFileState(key);
            else if (_downloadManager != null)
                return _downloadManager.GetDownloadFileState(key);
            else
                return DownloadState.NotDownloaded;
        }

        public FindPrintInfo FindPrint(string title, PrintType postType = PrintType.Unknow, bool forceSelect = false)
        {
            FindPrintInfo findPrint = null;
            if (Try(() => findPrint = _findPrintManager.Find(title, postType, forceSelect)))
                return findPrint;
            else
                return new FindPrintInfo { found = false };
        }

        private IPostToDownload_v1 LoadPost(DownloadPostKey_v1 key)
        {
            if (!_servers_v1.ContainsKey(key.server))
            {
                Trace.WriteLine("error unknow server \"{0}\"", key.server);
                return null;
            }
            return _servers_v1[key.server].LoadPost(key.id);
        }

        private void Downloaded(DownloadedFile_v1<DownloadPostKey_v1> downloadFile)
        {
            string message = GetDownloadStateText2(downloadFile.state);
            IPostToDownload_v1 post = LoadPost(downloadFile.key);
            if (post != null)
                TracePost(post, message, downloadFile.downloadedFile, downloadFile.downloadLink);
            MailAddLine(string.Format("{0} {1} {2:dd-MM-yyyy HH:mm:ss}", zPath.GetFileName(downloadFile.downloadedFile), message, DateTime.Now));
            if (downloadFile.state == DownloadState.DownloadCompleted)
            {
                if (downloadFile.uncompressFiles != null)
                {
                    foreach (string uncompressFile in downloadFile.uncompressFiles)
                    {
                        //Trace.WriteLine("  \"{0}\"", uncompressFile);
                        MailAddLine(string.Format("  {0}", zPath.GetFileName(uncompressFile)));
                    }
                }
            }
        }

        private void Downloaded(DownloadedFile_v2<DownloadPostKey_v1> downloadedFile)
        {
            string message = GetDownloadStateText2(downloadedFile.State);
            IPostToDownload_v1 post = LoadPost(downloadedFile.Key);
            //if (post != null)
            //    TracePost(post, message, null, null);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetPostMessage(post, message));
            if (downloadedFile.DownloadedFiles != null)
            {
                foreach (string file in downloadedFile.DownloadedFiles)
                    sb.AppendLine(string.Format("  file : \"{0}\"", file));
            }
            if (downloadedFile.UncompressFiles != null)
            {
                foreach (string file in downloadedFile.UncompressFiles)
                    sb.AppendLine(string.Format("  uncompress file : \"{0}\"", file));
            }
            Trace.Write(sb.ToString());

            //MailAddLine(string.Format("{0} {1:dd-MM-yyyy HH:mm:ss}", message, DateTime.Now));
            MailAddLine(GetPostMessage(post, message, formated: false));
            if (downloadedFile.DownloadedFiles != null)
            {
                foreach (string file in downloadedFile.DownloadedFiles)
                    MailAddLine(string.Format("  file : \"{0}\"", file));
            }
            if (downloadedFile.UncompressFiles != null)
            {
                foreach (string file in downloadedFile.UncompressFiles)
                    MailAddLine(string.Format("  uncompress file : \"{0}\"", file));
            }
            //if (downloadedFile.State == DownloadState.DownloadCompleted)
            //{
            //    if (downloadedFile.UncompressFiles != null)
            //    {
            //        foreach (string uncompressFile in downloadedFile.UncompressFiles)
            //        {
            //            MailAddLine(string.Format("  {0}\r\n", zPath.GetFileName(uncompressFile)));
            //        }
            //    }
            //}
        }

        private void MailAddLine(string message)
        {
            _mailBody.Append(message);
            _mailBody.Append(Environment.NewLine);
            _mailBodyLineNumber++;
            if (_mailBodyDateTime == null)
                _mailBodyDateTime = DateTime.Now;
        }

        private void SendMail(bool sendMail)
        {
            if (_mailBodyLineNumber == 0)
                return;
            //if ((_downloadManager.DownloadFilesCount == 0 && _downloadManager.QueueDownloadFilesCount == 0)
            //  || DateTime.Now.Subtract((DateTime)_mailBodyDateTime) >= _mailWaitDownloadFinish)
            if (!ActiveDownload() || DateTime.Now.Subtract((DateTime)_mailBodyDateTime) >= _mailWaitDownloadFinish)
            {
                try
                {
                    //Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - {1}", DateTime.Now, sendMail ? "Send mail" : "Mail (not send)");
                    //Trace.WriteLine(_mailMessage.Body + _mailBody.ToString());
                    string mail = string.Format("{0:dd-MM-yyyy HH:mm:ss} - {1}{2}", DateTime.Now, sendMail ? "Send mail" : "Mail (not send)", Environment.NewLine);
                    mail += _mailMessage.Body + _mailBody.ToString() + "end of mail";
                    Trace.WriteLine(mail);

                    if (sendMail)
                    {
                        MailMessage message = new MailMessage();
                        message.From = _mailMessage.From;
                        message.Subject = _mailMessage.Subject;
                        foreach (MailAddress to in _mailMessage.To)
                            message.To.Add(to);
                        message.Body = _mailMessage.Body + _mailBody.ToString();
                        _mailSender.Send(message);
                    }
                }
                finally
                {
                    _mailBody.Clear();
                    _mailBodyLineNumber = 0;
                    _mailBodyDateTime = null;
                }
            }
        }

        private static string GetPostMessage(IPostToDownload_v1 post, string message, string file = null, string downloadLink = null, bool formated = true)
        {
            int messageLength = 50;
            int printTypeLength = 25;
            if (!formated)
            {
                messageLength = 0;
                printTypeLength = 0;
            }

            //string formatMessage;
            //if (formated)
            //    formatMessage = "{0,-50}";
            //else
            //    formatMessage = "{0}";
            //string formatPrintType;
            //if (formated)
            //    formatPrintType = "{0,-25}";
            //else
            //    formatPrintType = "{0}";

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:dd-MM-yyyy HH:mm:ss}", DateTime.Now);
            sb.Append(" - ");
            //sb.AppendFormat("{0,-50}", message);
            sb.Append(message.PadRight(messageLength));
            sb.Append(" - ");
            if (post != null)
            {
                //sb.AppendFormat("{0,-25} - ", post.GetPrintType());
                sb.Append(post.GetPrintType().ToString().PadRight(printTypeLength));
                sb.Append(" - ");
                sb.AppendFormat("\"{0}\"", post.GetTitle());
            }
            else
                sb.Append("(post is null)");
            if (file != null)
                sb.AppendFormat(" - \"{0}\"", file);
            if (post != null)
                sb.AppendFormat(" - \"{0}\"", post.GetDataHttpRequest().Url);
            if (downloadLink != null)
                sb.AppendFormat(" - link \"{0}\"", downloadLink);
            return sb.ToString();
        }

        private static void TracePost(IPostToDownload_v1 post, string message, string file = null, string downloadLink = null)
        {
            Trace.WriteLine(GetPostMessage(post, message, file, downloadLink));
        }

        private bool FilterTracePost(DownloadState state)
        {
            if (_desactivateFilterTracePost)
                return true;
            switch (state)
            {
                case DownloadState.DownloadCompleted:
                    return false;
                case DownloadState.DownloadFailed:
                case DownloadState.DownloadStarted:
                case DownloadState.WaitToDownload:
                case DownloadState.UnknowEndDownload:
                case DownloadState.NotDownloaded:
                    return true;
                default:
                    return true;
            }
        }

        private static string GetDownloadStateText1(DownloadState state)
        {
            switch (state)
            {
                case DownloadState.DownloadCompleted:
                    return "already downloaded";
                case DownloadState.DownloadFailed:
                    return "download failed";
                case DownloadState.DownloadStarted:
                    return "download already started";
                case DownloadState.WaitToDownload:
                    return "download is already waiting for download";
                case DownloadState.UnknowEndDownload:
                    return "download end unknow";
                case DownloadState.NotDownloaded:
                    return "not downloaded";
                default:
                    return string.Format("download unknow state ???????? {0}", state);
            }
        }

        private static string GetDownloadStateText2(DownloadState state)
        {
            switch (state)
            {
                case DownloadState.DownloadCompleted:
                    return "download complete";
                case DownloadState.DownloadFailed:
                    return "download failed";
                default:
                    return string.Format("error download finish with unknow state {0}", state);
            }
        }

        private static bool Try(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("error : {0}", ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
            return false;
        }
    }
}
