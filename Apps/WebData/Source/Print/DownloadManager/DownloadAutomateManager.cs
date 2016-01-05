﻿using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using MongoDB.Bson;
using pb;
using pb.Compiler;
using pb.Data;
using pb.Web.Data;
using pb.Web;
using Print;
using System.Xml.Linq;
using pb.Data.Xml;
using MongoDB.Bson.Serialization.Attributes;

namespace Download.Print
{
    // IKeyData.GetKey() used in DownloadAutomateManager_v2.TryDownloadPost()
    public interface IPostToDownload : IHttpRequestData, IKeyData
    {
        string GetServer();
        string GetTitle();
        PrintType GetPrintType();
        //string[] GetDownloadLinks();
        PostDownloadLinks GetDownloadLinks();
    }

    // from old DownloadPostKey
    public class ServerKey : IQuery
    {
        public string Server;
        public BsonValue Id;

        public IEnumerable<KeyValuePair<string, object>> GetQueryValues()
        {
            yield return new KeyValuePair<string, object>("server", Server);
            yield return new KeyValuePair<string, object>("id", Id);
        }

        public override string ToString()
        {
            return string.Format("{0} id {1}", Server, Id);
        }
    }

    public class PostDownloadLinks : IRequestDownloadLinks
    {
        [BsonElement("ItemLinks")]
        private List<PostDownloadItemLink> _itemLinks = new List<PostDownloadItemLink>();
        private PostDownloadItemLink _currentItem = null;


        public List<PostDownloadItemLink> ItemLinks { get { return _itemLinks; } }
        public int LinksCount
        {
            get
            {
                int nb = 0;
                foreach (PostDownloadItemLink itemLink in _itemLinks)
                {
                    foreach (PostDownloadServerLink serverLink in itemLink.ServerLinks)
                    {
                        nb += serverLink.FilePartLinks.Count;
                    }
                }
                return nb;
            }
        }

        public IRequestDownloadItemLink[] GetLinks()
        {
            return _itemLinks.ToArray();
        }

        /// <summary>add item and set item as current item</summary>
        /// <param name="name">item name</param>
        /// <returns></returns>
        public PostDownloadItemLink AddItem(string name = null)
        {
            PostDownloadItemLink item = new PostDownloadItemLink();
            item.Name = name;
            _itemLinks.Add(item);
            _currentItem = item;
            return item;
        }

        /// <summary>add server to current item and set server as current server</summary>
        /// <param name="name">server name : Uploaded, Turbobit, Uptobox, 1fichier, Letitbit</param>
        /// <param name="link">optional link : http://extreme-protect.net/bihYv </param>
        /// <returns></returns>
        public PostDownloadServerLink AddServer(string name, string link = null)
        {
            if (_currentItem == null)
                throw new PBException("no current item link");
            return _currentItem.AddServer(name, link);
        }

        /// <summary>add link to current server, .part01.rar, .part02.rar, ...</summary>
        /// <param name="link"></param>
        public void AddLink(string link)
        {
            if (_currentItem == null)
                throw new PBException("no current item link");
            _currentItem.AddLink(link);
        }

        /// <summary>add links to current server, .part01.rar, .part02.rar, ...</summary>
        /// <param name="links"></param>
        public void AddLinks(IEnumerable<string> links)
        {
            if (_currentItem == null)
                throw new PBException("no current item link");
            _currentItem.AddLinks(links);
        }

        public static PostDownloadLinks Create(IEnumerable<string> links)
        {
            PostDownloadLinks postDownloadLinks = new PostDownloadLinks();
            postDownloadLinks.AddItem();
            foreach (string link in links)
            {
                postDownloadLinks.AddServer(DownloadFileServerInfo.GetServerNameFromLink(link));
                postDownloadLinks.AddLink(link);
            }
            return postDownloadLinks;
        }
    }

    public class PostDownloadItemLink : IRequestDownloadItemLink
    {
        public string Name;                               // "Pack1", "Pack2", ""
        [BsonElement("ServerLinks")]
        private List<PostDownloadServerLink> _serverLinks = new List<PostDownloadServerLink>();
        private PostDownloadServerLink _currentServer = null;

        public List<PostDownloadServerLink> ServerLinks { get { return _serverLinks; } }

        public string GetName()
        {
            return Name;
        }

        public IRequestDownloadServerLink[] GetServerLinks()
        {
            return _serverLinks.ToArray();
        }

        public PostDownloadServerLink AddServer(string name, string link = null)
        {
            PostDownloadServerLink server = new PostDownloadServerLink();
            server.Name = name;
            server.Link = link;
            _serverLinks.Add(server);
            _currentServer = server;
            return server;
        }

        public void AddLink(string link)
        {
            if (_currentServer == null)
                throw new PBException("no current server link");
            _currentServer.AddLink(link);
        }

        public void AddLinks(IEnumerable<string> links)
        {
            if (_currentServer == null)
                throw new PBException("no current server link");
            _currentServer.AddLinks(links);
        }
    }

    public class PostDownloadServerLink : IRequestDownloadServerLink
    {
        public string Name;                         // Uploaded, Turbobit, Uptobox, 1fichier, Letitbit
        public string Link;                         // http://extreme-protect.net/bihYv
        [BsonElement("FilePartLinks")]
        private List<string> _filePartLinks = new List<string>();      // .part01.rar, .part02.rar, ...

        public List<string> FilePartLinks { get { return _filePartLinks; } }

        public string GetName()
        {
            return Name;
        }

        public string[] GetFilePartLinks()
        {
            return _filePartLinks.ToArray();
        }

        public void AddLink(string link)
        {
            _filePartLinks.Add(link);
        }

        public void AddLinks(IEnumerable<string> links)
        {
            foreach (string link in links)
                _filePartLinks.Add(link);
        }
    }

    public class DownloadAutomateManager
    {
        private Dictionary<string, ServerManager> _servers = new Dictionary<string, ServerManager>();
        private MongoDownloadAutomateManager _mongoDownloadAutomateManager = null;
        private Func<PrintType, bool> _downloadAllPrintType = null;
        private FindPrintManager _findPrintManager = null;
        private DownloadManager _downloadManager = null;
        private MailSender _mailSender = null;
        private MailMessage _mailMessage = null;
        private TimeSpan _waitTimeBetweenOperation = TimeSpan.FromSeconds(5);     // 5 sec
        private TimeSpan _mailWaitDownloadFinish = TimeSpan.FromMinutes(10);      // 10 minutes
        private int _postDownloadServerLimit = 0;

        private StringBuilder _mailBody = new StringBuilder();
        private int _mailBodyLineNumber = 0;
        private DateTime? _mailBodyDateTime = null;

        private bool _desactivateFilterTracePost = false;

        private bool _runNow = false;
        private bool _loadNewPost = true;
        private bool _searchPostToDownload = true;
        private bool _sendMail = false;

        public DownloadAutomateManager()
        {
            //Init(test: DownloadPrint.Test);
        }

        public void Dispose()
        {
            //if (_downloadManager_v1 != null)
            //    _downloadManager_v1.Dispose();
            if (_downloadManager != null)
                _downloadManager.Dispose();
        }

        public MongoDownloadAutomateManager MongoDownloadAutomateManager { get { return _mongoDownloadAutomateManager; } set { _mongoDownloadAutomateManager = value; } }
        public Func<PrintType, bool> DownloadAllPrintType { get { return _downloadAllPrintType; } set { _downloadAllPrintType = value; } }
        public FindPrintManager FindPrintManager { get { return _findPrintManager; } set { _findPrintManager = value; } }
        public DownloadManager DownloadManager { get { return _downloadManager; } set { _downloadManager = value; } }
        public MailSender MailSender { get { return _mailSender; } set { _mailSender = value; } }
        public MailMessage MailMessage { get { return _mailMessage; } set { _mailMessage = value; } }
        public TimeSpan WaitTimeBetweenOperation { get { return _waitTimeBetweenOperation; } set { _waitTimeBetweenOperation = value; } }
        public TimeSpan MailWaitDownloadFinish { get { return _mailWaitDownloadFinish; } set { _mailWaitDownloadFinish = value; } }
        public int PostDownloadServerLimit { get { return _postDownloadServerLimit; } set { _postDownloadServerLimit = value; } }
        public bool DesactivateFilterTracePost { get { return _desactivateFilterTracePost; } set { _desactivateFilterTracePost = value; } }
        public bool RunNow { get { return _runNow; } set { _runNow = value; } }
        public bool LoadNewPost { get { return _loadNewPost; } set { _loadNewPost = value; } }
        public bool SearchPostToDownload { get { return _searchPostToDownload; } set { _searchPostToDownload = value; } }
        public bool SendMail { get { return _sendMail; } set { _sendMail = value; } }

        public void Init(XElement xe)
        {
            //if (!test)
            //    xe = XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager");
            //else
            //{
            //    Trace.WriteLine("DownloadAutomateManager (v2) init for test");
            //    xe = XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager_Test");
            //}
            _waitTimeBetweenOperation = xe.zXPathValue("WaitTimeBetweenOperation").zTryParseAs(TimeSpan.FromSeconds(5));
            _mailWaitDownloadFinish = xe.zXPathValue("MailWaitDownloadFinish").zTryParseAs(TimeSpan.FromMinutes(10));
            _postDownloadServerLimit = xe.zXPathValue("PostDownloadServerLimit").zTryParseAs(0);
        }

        public void SetParameters(NamedValues<ZValue> parameters)
        {
            if (parameters == null)
                return;
            foreach (KeyValuePair<string, ZValue> parameter in parameters)
                SetParameter(parameter);
        }

        public void SetParameter(KeyValuePair<string, ZValue> parameter)
        {
            switch (parameter.Key.ToLower())
            {
                case "waittimebetweenoperation":
                    _waitTimeBetweenOperation = (TimeSpan)parameter.Value;
                    break;
                case "mailwaitdownloadfinish":
                    _mailWaitDownloadFinish = (TimeSpan)parameter.Value;
                    break;
                case "postdownloadserverlimit":
                    _postDownloadServerLimit = (int)parameter.Value;
                    break;
                case "runnow":
                    _runNow = (bool)parameter.Value;
                    break;
                case "loadnewpost":
                    _loadNewPost = (bool)parameter.Value;
                    break;
                case "searchposttodownload":
                    _searchPostToDownload = (bool)parameter.Value;
                    break;
                case "sendmail":
                    _sendMail = (bool)parameter.Value;
                    break;
            }
        }

        public void AddServerManager(ServerManager server)
        {
            _servers.Add(server.Name, server);
        }

        public virtual void Start()
        {
            //if (_downloadManager_v1 != null)
            //{
            //    _downloadManager_v1.OnDownloaded = Downloaded;
            //    _downloadManager_v1.StartThread();
            //}
            if (_downloadManager != null)
            {
                _downloadManager.OnDownloaded = Downloaded;
                _downloadManager.StartThread();
            }
        }

        public virtual void Stop()
        {
            //if (_downloadManager_v1 != null)
            //    _downloadManager_v1.Stop();
            if (_downloadManager != null)
                _downloadManager.Stop();
        }

        // bool loadNewPost = true, bool searchPostToDownload = true, bool uncompressFile = true, bool sendMail = false
        public void Run()
        {
            DateTime nextRunDateTime = _mongoDownloadAutomateManager.GetNextRunDateTime();
            bool messageNextRun = true;
            while (true)
            {
                if (RunSource.CurrentRunSource.IsExecutionAborted())
                    break;
                if (_runNow || DateTime.Now >= nextRunDateTime)
                {
                    _runNow = false;

                    if (_loadNewPost)
                        _LoadNewPost();

                    if (_searchPostToDownload)
                    {
                        DateTime? lastRunDateTime = _mongoDownloadAutomateManager.GetLastRunDateTime();
                        if (lastRunDateTime == null)
                            lastRunDateTime = DateTime.Now.AddDays(-3);

                        _SearchPostToDownload((DateTime)lastRunDateTime);

                        _mongoDownloadAutomateManager.SetLastRunDateTime(DateTime.Now);
                        nextRunDateTime = _mongoDownloadAutomateManager.GetNextRunDateTime();
                    }

                    messageNextRun = true;
                }
                else
                {
                    Try(() => _SendMail(_sendMail));
                }
                if (!_searchPostToDownload && !ActiveDownload() && TaskManager.CurrentTaskManager.Count == 0)
                    break;

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
            //if (_downloadManager_v1 != null)
            //{
            //    if (_downloadManager_v1.DownloadFilesCount != 0 || _downloadManager_v1.QueueDownloadFilesCount != 0)
            //        return true;
            //}
            if (_downloadManager != null)
            {
                if (_downloadManager.DownloadFilesCount != 0 || _downloadManager.QueueDownloadFilesCount != 0)
                    return true;
            }
            return false;
        }

        private void _LoadNewPost()
        {
            foreach (ServerManager server in _servers.Values)
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

        private void _SearchPostToDownload(DateTime lastRunDateTime)
        {
            //Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - Search download from {1:dd-MM-yyyy HH:mm:ss}", DateTime.Now, lastRunDateTime);
            bool download = false;
            lastRunDateTime = lastRunDateTime.AddDays(-1);
            foreach (ServerManager server in _servers.Values)
            {
                if (server.EnableSearchPostToDownload)
                {
                    Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - Search download on {1} from {2:dd-MM-yyyy HH:mm:ss}", DateTime.Now, server.Name, lastRunDateTime);
                    int nb = 0;
                    foreach (IPostToDownload post in server.GetPostList(lastRunDateTime))
                    {
                        if (RunSource.CurrentRunSource.IsExecutionAborted())
                            break;

                        if (TryDownloadPost(post, server.DownloadDirectory))
                        {
                            download = true;
                            if (_postDownloadServerLimit != 0 && ++nb == _postDownloadServerLimit)
                                break;
                        }
                    }
                }
            }
            if (!download)
                Trace.WriteLine("{0:dd-MM-yyyy HH:mm:ss} - Nothing to download", DateTime.Now);
        }

        public bool TryDownloadPosts(IEnumerable<IPostToDownload> posts, string downloadDirectory = null, bool forceDownloadAgain = false, bool forceSelect = false, bool simulateDownload = false)
        {
            bool ret = false;
            foreach (IPostToDownload post in posts)
            {
                if (TryDownloadPost(post, downloadDirectory, forceDownloadAgain, forceSelect, simulateDownload))
                    ret = true;
            }
            return ret;
        }

        public bool TryDownloadPost(IPostToDownload post, string downloadDirectory = null, bool forceDownloadAgain = false, bool forceSelect = false, bool simulateDownload = false)
        {
            if (_downloadAllPrintType != null)
                forceSelect = forceSelect || _downloadAllPrintType(post.GetPrintType());
            FindPrintInfo findPrint = FindPrint(post.GetTitle(), post.GetPrintType(), forceSelect);
            if (!findPrint.found)
            {
                TracePost(post, "post not selected");
                return false;
            }

            //DownloadPostKey key = new DownloadPostKey { server = post.GetServer(), id = post.GetKey() };
            ServerKey key = new ServerKey { Server = post.GetServer(), Id = post.GetKey() };
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

            //if (_downloadManager_v1 != null)
            //    Try(() => _downloadManager_v1.DownloadFile(key, post.GetDownloadLinks(), file));
            if (_downloadManager != null)
                Try(() => _downloadManager.AddFileToDownload(key, post.GetDownloadLinks(), file));

            return true;
        }

        private DownloadState GetDownloadFileState(ServerKey key)
        {
            //if (_downloadManager_v1 != null)
            //    return _downloadManager_v1.GetDownloadFileState(key);
            if (_downloadManager != null)
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

        private IPostToDownload LoadPost(ServerKey key)
        {
            if (!_servers.ContainsKey(key.Server))
            {
                Trace.WriteLine("error unknow server \"{0}\"", key.Server);
                return null;
            }
            return _servers[key.Server].LoadPost(key.Id);
        }

        private void Downloaded(DownloadedFile downloadedFile)
        {
            string message = GetDownloadStateText2(downloadedFile.State);
            IPostToDownload post = LoadPost(downloadedFile.Key);
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
        }

        private void MailAddLine(string message)
        {
            _mailBody.Append(message);
            _mailBody.Append(Environment.NewLine);
            _mailBodyLineNumber++;
            if (_mailBodyDateTime == null)
                _mailBodyDateTime = DateTime.Now;
        }

        private void _SendMail(bool sendMail)
        {
            if (_mailBodyLineNumber == 0)
                return;
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

        private static string GetPostMessage(IPostToDownload post, string message, string file = null, string downloadLink = null, bool formated = true)
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

        private static void TracePost(IPostToDownload post, string message, string file = null, string downloadLink = null)
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
