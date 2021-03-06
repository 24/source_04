﻿using System.Net.Mail;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Text;
using pb.Web;
using pb.Data;
using System.Collections.Generic;
using pb.IO;
using pb.Web.Data;

namespace Download.Print
{
    public class DownloadAutomateManagerCreator
    {
        // parameters
        //private bool _test = false;
        private int _version = 6;
        private bool _useTestManager = false;
        private int? _traceLevel = null;

        // CreateFindPrintManager parameters
        //private bool _dailyPrintManager = false;
        //private int _gapDayBefore = 0;
        //private int _gapDayAfter = 0;

        private XElement _xeConfig = null;
        private NamedValues<ZValue> _parameters = null;

        private XmlConfig _config = null;
        private XmlConfig _localConfig = null;
        private XmlConfig _printList1Config = null;
        private XmlConfig _printList2Config = null;

        private DownloadAutomateManager _downloadAutomateManager = null;
        private MongoDownloadAutomateManager _mongoDownloadAutomateManager = null;
        private FindPrintManager _findPrintManager = null;
        private DownloadManager _downloadManager = null;
        private MailSender _mailSender = null;
        private MailMessage _mailMessage = null;

        //public bool Test { get { return _test; } } // set { _test = value; }
        public int Version { get { return _version; } set { _version = value; } }
        public bool UseTestManager { get { return _useTestManager; } set { _useTestManager = value; } }
        public int? TraceLevel { get { return _traceLevel; } set { _traceLevel = value; } }
        //public bool DailyPrintManager { get { return _dailyPrintManager; } set { _dailyPrintManager = value; } }
        //public int GapDayBefore { get { return _gapDayBefore; } set { _gapDayBefore = value; } }
        //public int GapDayAfter { get { return _gapDayAfter; } set { _gapDayAfter = value; } }

        public DownloadAutomateManagerCreator()
        {
        }

        public void Init(XElement xe, XmlConfig config)
        {
            //XElement xe;
            //if (!test)
            //    _xeConfig = XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager");
            //else
            //{
            //    Trace.WriteLine("CreateDownloadAutomateManager init for test");
            //    _xeConfig = XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager_Test");
            //}
            //_test = test;
            _xeConfig = xe;
            _version = xe.zXPathValue("Version").zTryParseAs(_version);
            _useTestManager = xe.zXPathValue("UseTestManager").zTryParseAs(false);
            _traceLevel = xe.zXPathValue("TraceLevel").zTryParseAs<int?>();
            //_dailyPrintManager = xe.zXPathValue("DailyPrintManager").zTryParseAs(false);
            //_gapDayBefore = xe.zXPathValue("GapDayBefore").zTryParseAs(0);
            //_gapDayAfter = xe.zXPathValue("GapDayAfter").zTryParseAs(0);

            //_config = XmlConfig.CurrentConfig;
            _config = config;
            _localConfig = _config.GetConfig("LocalConfig");
            //_localConfig = _config.GetConfig("LocalConfig", optional: true);
            _printList1Config = _config.GetConfig("PrintList1Config");
            _printList2Config = _config.GetConfig("PrintList2Config");
        }

        public void SetParameters(NamedValues<ZValue> parameters)
        {
            _parameters = parameters;
            if (parameters == null)
                return;
            foreach (KeyValuePair<string, ZValue> parameter in parameters)
                SetParameter(parameter);
        }

        public void SetParameter(KeyValuePair<string, ZValue> parameter)
        {
            //switch (parameter.Key.ToLower())
            switch (parameter.Key)
            {
                case "tracelevel":
                    _traceLevel = (int)parameter.Value;
                    break;
                case "version":
                    _version = (int)parameter.Value;
                    break;
                case "usetestmanager":
                    _useTestManager = (bool)parameter.Value;
                    break;
                //case "dailyprintmanager":
                //    _dailyPrintManager = (bool)parameter.Value;
                //    break;
                //case "gapdaybefore":
                //    _gapDayBefore = (int)parameter.Value;
                //    break;
                //case "gapdayafter":
                //    _gapDayAfter = (int)parameter.Value;
                //    break;
            }
        }

        // int gapDayBefore = 0, int gapDayAfter = 0, int? traceLevel = null
        //public static DownloadAutomateManager_v2 Create(int version = 3, bool dailyPrintManager = false, bool useTestManager = false, bool test = false)
        //{
        //    CreateDownloadAutomateManager create = new CreateDownloadAutomateManager();
        //    create._version = version;
        //    create._dailyPrintManager = dailyPrintManager;
        //    //create._gapDayBefore = gapDayBefore;
        //    //create._gapDayAfter = gapDayAfter;
        //    create._useTestManager = useTestManager;
        //    //create._traceLevel = traceLevel;
        //    create._test = test;
        //    return create._Create();
        //}

        public DownloadAutomateManager Create()
        {
            // from DownloadAutomate_f.CreateDownloadAutomate_v2()

            // le 01/11/2014 désactive version 1 et version 2
            // version 1 : utilise l'ancien FindPrintManager, l'ancienne liste de print dans print_list1.xml, l'ancien pattern de date FindPrints/Dates/Date avec l'ancien FindDateManager
            // version 2 : version 1 + le nouveau FindPrintManager_new avec le nouveau PrintTitleManager, la nouvelle liste de print dans print_list2.xml,
            // version 3 : version 2 + le nouveau pattern de date FindPrints/Dates/DateNew avec le nouveau FindDateManager_new
            // version 4 (not used) : version 3 + découpe le titre avec "du" ou "-" (PrintTitleManager)
            // version 5 : version 3 +  new find date
            // version 6 : version 5 +  printTitleManager version 2 + findPrintManager version 2

            if (_version < 3 || _version > 6)
                throw new PBException("bad version {0}", _version);

            Trace.WriteLine("create download automate : version {0} useTestManager {1} traceLevel {2}", _version, _useTestManager, _traceLevel.zToStringOrNull());

            //if (!_test)
            //    _xeConfig = XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager");
            //else
            //    _xeConfig = XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager_Test");

            _mongoDownloadAutomateManager = CreateMongoDownloadAutomateManager();

            //_findPrintManager = CreateFindPrintManager();
            _findPrintManager = FindPrintManagerCreator.Create(_xeConfig, _parameters, _version);

            _downloadManager = CreateDownloadManager();
            _mailSender = CreateMailSender();
            _mailMessage = CreateMailMessage();

            _downloadAutomateManager = _CreateDownloadAutomateManager();

            //InitServers();
            //CreateServerManagers();

            _downloadAutomateManager.Init(_xeConfig);
            _downloadAutomateManager.SetParameters(_parameters);

            TraceResult();

            //if (!ControlDownloadManagerClient())
            //    throw new PBException("error DownloadManagerClient is not working");

            if (_traceLevel != null)
                //Trace.CurrentTrace.TraceLevel = (int)_traceLevel;
                pb.TraceLevel.Level = (int)_traceLevel;

            //_downloadAutomateManager.Start();

            return _downloadAutomateManager;
        }

        //public FindPrintManager CreateFindPrintManager()
        //{
        //    CreateFindPrintManager createFindPrintManager = new CreateFindPrintManager();
        //    createFindPrintManager.Init(_xeConfig);
        //    createFindPrintManager.SetParameters(_parameters);
        //    createFindPrintManager.Version = _version;
        //    return createFindPrintManager.Create();
        //}

        private MongoDownloadAutomateManager CreateMongoDownloadAutomateManager()
        {
            //XmlConfig config = XmlConfig.CurrentConfig;
            // 
            //return new MongoDownloadAutomateManager(config.Get("DownloadAutomateManager/MongoDownloadAutomate/MongoServer"), config.Get("DownloadAutomateManager/MongoDownloadAutomate/MongoDatabase"),
            //    config.Get("DownloadAutomateManager/MongoDownloadAutomate/MongoCollection"));
            return new MongoDownloadAutomateManager(_xeConfig.zXPathValue("MongoDownloadAutomate/MongoServer"), _xeConfig.zXPathValue("MongoDownloadAutomate/MongoDatabase"),
                _xeConfig.zXPathValue("MongoDownloadAutomate/MongoCollection"));
        }

        private Debrider CreateDebrider()
        {
            return CreateDebrider(_config, _useTestManager);
        }

        public static Debrider CreateDebrider(XmlConfig config, bool useTestManager = false)
        {
            //return CreateDebriderAlldebrid();
            Debrider debrider;
            if (!useTestManager)
                debrider = CreateDebriderDebridLinkFr(config);
            else
                debrider = new DebriderAlldebridTest();
            return debrider;
        }

        private static Debrider CreateDebriderDebridLinkFr(XmlConfig config)
        {
            DebriderDebridLinkFr debrider = new DebriderDebridLinkFr();
            debrider.DebridLinkFr = CreateDebridLinkFr(config);
            return debrider;
        }

        private static DebridLinkFr_v3 CreateDebridLinkFr(XmlConfig config)
        {
            DebridLinkFr_v3 debrider = new DebridLinkFr_v3();
            XmlConfig localConfig = config.GetConfig("LocalConfig");
            debrider.Login = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Login");
            debrider.Password = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Password");
            debrider.PublicKey = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/PublicKey");
            debrider.ConnexionLifetime = DebridLinkFr_v3.GetConnexionLifetime(localConfig.GetExplicit("DownloadAutomateManager/DebridLink/ConnexionLifetime"));
            debrider.ConnexionFile = config.GetExplicit("DebridLink/ConnexionFile");
            //debrider.ServerTimeFile = XmlConfig.CurrentConfig.GetExplicit("DebridLink/ServerTimeFile");
            return debrider;
        }

        public DownloadManagerClientBase CreateDownloadManagerClient()
        {
            return CreateDownloadManagerClient(_xeConfig, _useTestManager);
        }

        public static DownloadManagerClientBase CreateDownloadManagerClient(XElement xe, bool useTestManager = false)
        {
            DownloadManagerClientBase downloadManagerClient = null;
            if (!useTestManager)
                //downloadManagerClient = new DownloadManagerClient(XmlConfig.CurrentConfig.GetExplicit("DownloadAutomateManager/DownloadManagerClient/Address"));
                downloadManagerClient = new DownloadManagerClient(xe.zXPathExplicitValue("DownloadManagerClient/Address"));
            else
                downloadManagerClient = new DownloadManagerClientTest(@"c:\pib\_dl\_pib\dl");
            return downloadManagerClient;
        }

        public UncompressQueueManager CreateUncompressManager()
        {
            UncompressQueueManager uncompressManager = new UncompressQueueManager(new SharpCompressManager());
            //uncompressManager.ArchiveCompressDirectory = XmlConfig.CurrentConfig.Get("DownloadAutomateManager/UncompressManager/ArchiveCompressDirectory");
            uncompressManager.ArchiveCompressDirectory = _xeConfig.zXPathValue("UncompressManager/ArchiveCompressDirectory");
            //uncompressManager.ArchiveErrorCompressDirectory = XmlConfig.CurrentConfig.Get("DownloadAutomateManager/UncompressManager/ArchiveErrorCompressDirectory");
            uncompressManager.ArchiveErrorCompressDirectory = _xeConfig.zXPathValue("UncompressManager/ArchiveErrorCompressDirectory");
            return uncompressManager;
        }

        //public DownloadManager_v2 CreateDownloadManager_v2(bool useTestManager = false)
        public DownloadManager CreateDownloadManager()
        {
            MongoCollectionManager<DownloadedFile> mongoDownloadedFileManager = MongoCollectionManager<DownloadedFile>.Create(_xeConfig.zXPathElement("MongoDownloadedFile"));
            mongoDownloadedFileManager.IdGenerator = new MongoIdGeneratorInt(mongoDownloadedFileManager.GetCollection());
            mongoDownloadedFileManager.KeyName = "Key";     // Key is the name of key field in DownloadedFile_v2

            //MongoCollectionManager<QueueDownloadFile> mongoQueueDownloadFileManager = MongoCollectionManager<QueueDownloadFile>.Create(_xeConfig.zXPathElement("MongoQueueDownloadFile_new"));
            //mongoQueueDownloadFileManager.IdGenerator = new MongoIdGeneratorInt(mongoQueueDownloadFileManager.GetCollection());
            //mongoDownloadedFileManager.KeyName = "Key";     // Key is the name of key field in QueueDownloadFile_v2
            MongoCollectionManager<QueueDownloadFile> mongoQueueDownloadFileManager = CreateMongoQueueDownloadFileManager(_xeConfig);

            MongoCollectionManager<DownloadLinkRef> mongoCurrentDownloadFileManager = MongoCollectionManager<DownloadLinkRef>.Create(_xeConfig.zXPathElement("MongoCurrentDownloadFile"));
            mongoCurrentDownloadFileManager.IdGenerator = new MongoIdGeneratorInt(mongoCurrentDownloadFileManager.GetCollection());

            ProtectLink protectLink = null;

            //Debrider debrider = null;
            //if (!_useTestManager)
            //    debrider = CreateDebrider();
            //else
            //    debrider = new DebriderAlldebridTest();
            Debrider debrider = CreateDebrider();
            DownloadManagerClientBase downloadManagerClient = CreateDownloadManagerClient();
            UncompressQueueManager uncompressManager = CreateUncompressManager();

            DownloadManager downloadManager = new DownloadManager();
            downloadManager.DownloadManagerClient = downloadManagerClient;
            downloadManager.MongoDownloadedFileManager = mongoDownloadedFileManager;
            downloadManager.MongoQueueDownloadFileManager = mongoQueueDownloadFileManager;
            downloadManager.MongoCurrentDownloadFileManager = mongoCurrentDownloadFileManager;
            downloadManager.ProtectLink = protectLink;
            downloadManager.Debrider = debrider;
            downloadManager.UncompressManager = uncompressManager;
            return downloadManager;
        }

        public static MongoCollectionManager<QueueDownloadFile> CreateMongoQueueDownloadFileManager(XElement xe)
        {
            MongoCollectionManager<QueueDownloadFile> mongoQueueDownloadFileManager = MongoCollectionManager<QueueDownloadFile>.Create(xe.zXPathElement("MongoQueueDownloadFile_new"));
            mongoQueueDownloadFileManager.IdGenerator = new MongoIdGeneratorInt(mongoQueueDownloadFileManager.GetCollection());
            return mongoQueueDownloadFileManager;
        }

        private MailSender CreateMailSender()
        {
            return new MailSender(_localConfig.Get("DownloadAutomateManager/Gmail/Mail"), _localConfig.Get("DownloadAutomateManager/Gmail/Password"),
                _localConfig.Get("DownloadAutomateManager/Gmail/SmtpHost"), _localConfig.Get("DownloadAutomateManager/Gmail/SmtpPort").zTryParseAs<int>(587));
        }

        private MailMessage CreateMailMessage()
        {
            //MailMessage mailMessage = new MailMessage(config.GetConfig("LocalConfig").Get("DownloadAutomateManager/Gmail/Mail"), config.Get("DownloadAutomateManager/Mail/To"),
            //    config.Get("DownloadAutomateManager/Mail/Subject"), config.Get("DownloadAutomateManager/Mail/Body"));
            return new MailMessage(_localConfig.Get("DownloadAutomateManager/Gmail/Mail"), _xeConfig.zXPathValue("Mail/To"),
                _xeConfig.zXPathValue("Mail/Subject"), _xeConfig.zXPathValue("Mail/Body"));
        }

        private DownloadAutomateManager _CreateDownloadAutomateManager()
        {
            DownloadAutomateManager downloadAutomateManager = new DownloadAutomateManager();
            downloadAutomateManager.MongoDownloadAutomateManager = _mongoDownloadAutomateManager;
            downloadAutomateManager.DownloadAllPrintType = printType => printType == PrintType.Print;
            downloadAutomateManager.FindPrintManager = _findPrintManager;
            downloadAutomateManager.DownloadManager = _downloadManager;
            downloadAutomateManager.MailSender = _mailSender;
            downloadAutomateManager.MailMessage = _mailMessage;
            //downloadAutomateManager.Init(_xeConfig);
            //downloadAutomateManager.SetParameters(_parameters);
            return downloadAutomateManager;
        }

        //public IEnumerable<ServerManager> CreateServerManagers()
        //{
        //    foreach (XElement xe in _xeConfig.zXPathElements("ServerManagers/ServerManager"))
        //    {
        //        ServerManager serverManager = ServerManagers.Get(xe.zExplicitAttribValue("name"));
        //        serverManager.EnableLoadNewPost = xe.zAttribValue("enableLoadNewPost").zTryParseAs(true);
        //        serverManager.EnableSearchPostToDownload = xe.zAttribValue("enableSearchPostToDownload").zTryParseAs(true);
        //        serverManager.DownloadDirectory = xe.zAttribValue("downloadDirectory").zNullIfEmpty();
        //        //Trace.WriteLine("  create server manager \"{0}\" enable load new post {1} enable search post to download {2} download directory \"{3}\"", serverManager.Name, serverManager.EnableLoadNewPost, serverManager.EnableSearchPostToDownload, serverManager.DownloadDirectory);
        //        yield return serverManager;
        //    }
        //}

        public IEnumerable<IServerManager> CreateServerManagers()
        {
            foreach (XElement xe in _xeConfig.zXPathElements("ServerManagers/ServerManager"))
            {
                IServerManager serverManager = ServerManagers_v2.Get(xe.zExplicitAttribValue("name"));
                serverManager.EnableLoadNewDocument = xe.zAttribValue("enableLoadNewPost").zTryParseAs(true);
                serverManager.EnableSearchDocumentToDownload = xe.zAttribValue("enableSearchPostToDownload").zTryParseAs(true);
                serverManager.DownloadDirectory = xe.zAttribValue("downloadDirectory").zNullIfEmpty();
                //Trace.WriteLine("  create server manager \"{0}\" enable load new post {1} enable search post to download {2} download directory \"{3}\"", serverManager.Name, serverManager.EnableLoadNewPost, serverManager.EnableSearchPostToDownload, serverManager.DownloadDirectory);
                yield return serverManager;
            }
        }

        //private void CreateServerManagers()
        //{
        //    foreach (XElement xe in _xeConfig.zXPathElements("ServerManagers/ServerManager"))
        //    {
        //        ServerManager serverManager = ServerManagers.Get(xe.zExplicitAttribValue("name"));
        //        serverManager.EnableLoadNewPost = xe.zAttribValue("enableLoadNewPost").zTryParseAs(true);
        //        serverManager.EnableSearchPostToDownload = xe.zAttribValue("enableSearchPostToDownload").zTryParseAs(true);
        //        serverManager.DownloadDirectory = xe.zAttribValue("downloadDirectory").zNullIfEmpty();
        //        _downloadAutomateManager.AddServerManager(serverManager);
        //        Trace.WriteLine("add server manager \"{0}\" enable load new post {1} enable search post to download {2} download directory \"{3}\"", serverManager.Name, serverManager.EnableLoadNewPost, serverManager.EnableSearchPostToDownload, serverManager.DownloadDirectory);
        //    }
        //}

        private void TraceResult()
        {
            Trace.WriteLine("mongo download automate manager collection {0}", _downloadAutomateManager.MongoDownloadAutomateManager.GetCollection().zGetFullName());
            //if (downloadAutomate.DownloadManager_v1 != null)
            //{
            //    Trace.WriteLine("mongo download manager collection                 {0}", downloadAutomate.DownloadManager_v1.MongoDownloadedFileManager.GetCollection().zGetFullName());
            //    Trace.WriteLine("mongo queue download manager collection           {0}", downloadAutomate.DownloadManager_v1.MongoQueueDownloadFileManager.GetCollection().zGetFullName());
            //}
            if (_downloadAutomateManager.DownloadManager != null)
            {
                Trace.WriteLine("mongo download manager collection                 {0}", _downloadAutomateManager.DownloadManager.MongoDownloadedFileManager.GetCollection().zGetFullName());
                Trace.WriteLine("mongo queue download manager collection           {0}", _downloadAutomateManager.DownloadManager.MongoQueueDownloadFileManager.GetCollection().zGetFullName());
                Trace.WriteLine("mongo current download file manager collection    {0}", _downloadAutomateManager.DownloadManager.MongoCurrentDownloadFileManager.GetCollection().zGetFullName());
                Trace.WriteLine("download manager client                           {0}", _downloadAutomateManager.DownloadManager.DownloadManagerClient);
            }

            Trace.WriteLine("last run time {0:dd-MM-yyyy HH:mm:ss}", _downloadAutomateManager.MongoDownloadAutomateManager.GetLastRunDateTime());
        }

        //private bool ControlDownloadManagerClient()
        //{
        //    Trace.Write("control download manager client");
        //    try
        //    {
        //        if (_downloadAutomateManager.DownloadManager != null)
        //            _downloadAutomateManager.DownloadManager.DownloadManagerClient.GetDownloadCount();
        //        Trace.WriteLine(" ok");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.WriteLine(" error");
        //        Trace.WriteLine("error {0}", ex.Message);
        //        return false;
        //    }
        //}
    }
}
