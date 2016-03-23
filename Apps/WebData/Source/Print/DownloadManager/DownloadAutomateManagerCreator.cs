using System;
using System.Net.Mail;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Text;
using pb.Web;
using Print;
using pb.Data;
using System.Collections.Generic;

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

        public void Init(XElement xe)
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

            _config = XmlConfig.CurrentConfig;
            _localConfig = _config.GetConfig("LocalConfig");
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
            switch (parameter.Key.ToLower())
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

            // moved to Init()
            //_config = XmlConfig.CurrentConfig;
            //_localConfig = _config.GetConfig("LocalConfig");
            //_printList1Config = _config.GetConfig("PrintList1Config");
            //_printList2Config = _config.GetConfig("PrintList2Config");

            //if (!_test)
            //    _xeConfig = XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager");
            //else
            //    _xeConfig = XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager_Test");

            _mongoDownloadAutomateManager = CreateMongoDownloadAutomateManager();

            //_findPrintManager = CreateFindPrintManager.Create(_version, _dailyPrintManager, _gapDayBefore, _gapDayAfter);
            CreateFindPrintManager createFindPrintManager = new CreateFindPrintManager();
            createFindPrintManager.Init(_xeConfig);
            createFindPrintManager.SetParameters(_parameters);
            createFindPrintManager.Version = _version;
            //createFindPrintManager.DailyPrintManager = _dailyPrintManager;
            //createFindPrintManager.GapDayBefore = _gapDayBefore;
            //createFindPrintManager.GapDayAfter = _gapDayAfter;
            _findPrintManager = createFindPrintManager.Create();

            _downloadManager = CreateDownloadManager();
            _mailSender = CreateMailSender();
            _mailMessage = CreateMailMessage();

            _downloadAutomateManager = _CreateDownloadAutomateManager();

            //InitServers();
            CreateServerManagers();

            _downloadAutomateManager.Init(_xeConfig);
            _downloadAutomateManager.SetParameters(_parameters);

            TraceResult();

            if (!ControlDownloadManagerClient())
                throw new PBException("error DownloadManagerClient is not working");

            if (_traceLevel != null)
                Trace.CurrentTrace.TraceLevel = (int)_traceLevel;

            //_downloadAutomateManager.Start();

            return _downloadAutomateManager;
        }

        private MongoDownloadAutomateManager CreateMongoDownloadAutomateManager()
        {
            //XmlConfig config = XmlConfig.CurrentConfig;
            // 
            //return new MongoDownloadAutomateManager(config.Get("DownloadAutomateManager/MongoDownloadAutomate/MongoServer"), config.Get("DownloadAutomateManager/MongoDownloadAutomate/MongoDatabase"),
            //    config.Get("DownloadAutomateManager/MongoDownloadAutomate/MongoCollection"));
            return new MongoDownloadAutomateManager(_xeConfig.zXPathValue("MongoDownloadAutomate/MongoServer"), _xeConfig.zXPathValue("MongoDownloadAutomate/MongoDatabase"),
                _xeConfig.zXPathValue("MongoDownloadAutomate/MongoCollection"));
        }


        //CreateDebrider();
        public Debrider CreateDebrider()
        {
            //return CreateDebriderAlldebrid();
            Debrider debrider;
            if (!_useTestManager)
                debrider = CreateDebriderDebridLinkFr();
            else
                debrider = new DebriderAlldebridTest();
            return debrider;
        }

        public Debrider CreateDebriderDebridLinkFr()
        {
            DebriderDebridLinkFr debrider = new DebriderDebridLinkFr();
            debrider.DebridLinkFr = CreateDebridLinkFr();
            return debrider;
        }

        public DebridLinkFr CreateDebridLinkFr()
        {
            DebridLinkFr debrider = new DebridLinkFr();
            //XmlConfig localConfig = XmlConfig.CurrentConfig.GetConfig("LocalConfig");
            debrider.Login = _localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Login");
            debrider.Password = _localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Password");
            debrider.PublicKey = _localConfig.GetExplicit("DownloadAutomateManager/DebridLink/PublicKey");
            debrider.ConnexionLifetime = DebridLinkFr.GetConnexionLifetime(_localConfig.GetExplicit("DownloadAutomateManager/DebridLink/ConnexionLifetime"));
            debrider.ConnexionFile = _config.GetExplicit("DebridLink/ConnexionFile");
            //debrider.ServerTimeFile = XmlConfig.CurrentConfig.GetExplicit("DebridLink/ServerTimeFile");
            return debrider;
        }

        public DownloadManagerClientBase CreateDownloadManagerClient()
        {
            DownloadManagerClientBase downloadManagerClient = null;
            if (!_useTestManager)
                //downloadManagerClient = new DownloadManagerClient(XmlConfig.CurrentConfig.GetExplicit("DownloadAutomateManager/DownloadManagerClient/Address"));
                downloadManagerClient = new DownloadManagerClient(_xeConfig.zXPathExplicitValue("DownloadManagerClient/Address"));
            else
                downloadManagerClient = new DownloadManagerClientTest(@"c:\pib\_dl\_pib\dl");
            return downloadManagerClient;
        }

        public UncompressManager CreateUncompressManager()
        {
            UncompressManager uncompressManager = new UncompressManager();
            //uncompressManager.ArchiveCompressDirectory = XmlConfig.CurrentConfig.Get("DownloadAutomateManager/UncompressManager/ArchiveCompressDirectory");
            uncompressManager.ArchiveCompressDirectory = _xeConfig.zXPathValue("UncompressManager/ArchiveCompressDirectory");
            //uncompressManager.ArchiveErrorCompressDirectory = XmlConfig.CurrentConfig.Get("DownloadAutomateManager/UncompressManager/ArchiveErrorCompressDirectory");
            uncompressManager.ArchiveErrorCompressDirectory = _xeConfig.zXPathValue("UncompressManager/ArchiveErrorCompressDirectory");
            return uncompressManager;
        }

        //public DownloadManager_v2 CreateDownloadManager_v2(bool useTestManager = false)
        public DownloadManager CreateDownloadManager()
        {
            //MongoCollectionManager_v2<DownloadedFile_v2> mongoDownloadedFileManager = MongoCollectionManager_v2<DownloadedFile_v2>.Create(config.GetElement("DownloadAutomateManager/MongoDownloadedFile"));
            MongoCollectionManager<DownloadedFile> mongoDownloadedFileManager = MongoCollectionManager<DownloadedFile>.Create(_xeConfig.zXPathElement("MongoDownloadedFile"));
            mongoDownloadedFileManager.IdGenerator = new MongoIdGeneratorInt(mongoDownloadedFileManager.GetCollection());
            mongoDownloadedFileManager.KeyName = "Key";     // Key is the name of key field in DownloadedFile_v2
            //mongoDownloadedFileManager.QueryKey = key => new QueryDocument { { "downloadedFile.Key.server", key.server }, { "downloadedFile.Key._id", BsonValue.Create(key.id) } };

            //MongoCollectionManager_v2<QueueDownloadFile_v2> mongoQueueDownloadFileManager = MongoCollectionManager_v2<QueueDownloadFile_v2>.Create(config.GetElement("DownloadAutomateManager/MongoQueueDownloadFile_new"));
            MongoCollectionManager<QueueDownloadFile> mongoQueueDownloadFileManager = MongoCollectionManager<QueueDownloadFile>.Create(_xeConfig.zXPathElement("MongoQueueDownloadFile_new"));
            // MongoIdGenerator_v2 IdGenerator
            mongoQueueDownloadFileManager.IdGenerator = new MongoIdGeneratorInt(mongoQueueDownloadFileManager.GetCollection());
            mongoDownloadedFileManager.KeyName = "Key";     // Key is the name of key field in QueueDownloadFile_v2
            //mongoQueueDownloadFileManager.QueryKey = key => new QueryDocument { { "queueDownloadFile.Key.server", key.server }, { "queueDownloadFile.Key._id", BsonValue.Create(key.id) } };

            //MongoCollectionManager_v2<DownloadLinkRef_v2> mongoCurrentDownloadFileManager = MongoCollectionManager_v2<DownloadLinkRef_v2>.Create(config.GetElement("DownloadAutomateManager/MongoCurrentDownloadFile"));
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
            UncompressManager uncompressManager = CreateUncompressManager();

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

        private void CreateServerManagers()
        {
            //Ebookdz.Ebookdz.FakeInit();
            //Vosbooks.Vosbooks.FakeInit();
            //TelechargerMagazine.TelechargerMagazine.FakeInit();
            //ExtremeDown.ExtremeDown.FakeInit();

            //foreach (XElement xe in config.GetElements("DownloadAutomateManager/ServerManagers/ServerManager"))
            foreach (XElement xe in _xeConfig.zXPathElements("ServerManagers/ServerManager"))
            {
                ServerManager serverManager = ServerManagers.Get(xe.zExplicitAttribValue("name"));
                serverManager.EnableLoadNewPost = xe.zAttribValue("enableLoadNewPost").zTryParseAs<bool>(true);
                serverManager.EnableSearchPostToDownload = xe.zAttribValue("enableSearchPostToDownload").zTryParseAs<bool>(true);
                serverManager.DownloadDirectory = xe.zAttribValue("downloadDirectory").zNullIfEmpty();
                _downloadAutomateManager.AddServerManager(serverManager);
                Trace.WriteLine("add server manager \"{0}\" enable load new post {1} enable search post to download {2} download directory \"{3}\"", serverManager.Name, serverManager.EnableLoadNewPost, serverManager.EnableSearchPostToDownload, serverManager.DownloadDirectory);
            }

        }

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

        private bool ControlDownloadManagerClient()
        {
            Trace.Write("control download manager client");
            try
            {
                if (_downloadAutomateManager.DownloadManager != null)
                    _downloadAutomateManager.DownloadManager.DownloadManagerClient.GetDownloadCount();
                Trace.WriteLine(" ok");
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(" error");
                Trace.WriteLine("error {0}", ex.Message);
                return false;
            }
        }
    }
}
