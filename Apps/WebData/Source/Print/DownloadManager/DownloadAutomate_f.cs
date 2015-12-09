using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using pb;
using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using pb.Web;
using pb.Web.Data;
using Print;

// useTestManager = true
//   downloadManagerClient = new DownloadManagerClientTest(@"c:\pib\_dl\_pib\dl");
//   debrider = new DebriderAlldebridTest();

namespace Download.Print
{
    public static class DownloadAutomate_f
    {
        public static void BackupSite(ServerManager serverManager)
        {
            string mongoBackupDirectory = XmlConfig.CurrentConfig.Get("MongoBackupDirectory");
            string mongoBackupTmpDirectory = XmlConfig.CurrentConfig.Get("MongoBackupTmpDirectory");
        }

        public static void Test_Download_01(string file, IEnumerable<string> links)
        {
            PostDownloadLinks postDownloadLinks = PostDownloadLinks.Create(links);
        }

        // collectionName : Ebookdz_Detail Vosbooks_Detail TelechargerMagazine_Detail
        // serverName : ebookdz.com vosbooks.net telecharger-magazine.com
        public static IEnumerable<BsonDocument> GetDownloadedInfo(string collectionName, string serverName, bool onlyNotDownloaded = false, bool file = false, string databaseName = "dl", string query = null, string sort = null, int limit = 0, string server = null)
        {
            string downloadedCollectionName = "DownloadedFile";
            if (sort == null)
                sort = "{ _id: -1 }";
            foreach (BsonDocument document in TraceMongoCommand.Find(databaseName, collectionName, query: query, sort: sort, limit: limit, server: server))
            {
                string queryDownloaded = string.Format("{{ $and: [ {{ 'downloadedFile.Key.server': '{0}' }}, {{ 'downloadedFile.Key._id': {1} }} ]}}", serverName, document.zGet("_id").zAsInt());
                BsonDocument documentDownloaded = TraceMongoCommand.Find(databaseName, downloadedCollectionName, queryDownloaded, server).FirstOrDefault().zGet("downloadedFile").zAsBsonDocument();

                string state = documentDownloaded.zGet("State").zAsString();
                if (onlyNotDownloaded && state == "DownloadCompleted")
                    continue;

                BsonDocument documentDownload = document.zGet("download").zAsBsonDocument();

                // download : SourceUrl, LoadFromWebDate, Title, PrintType, OriginalTitle, PostAuthor, PostCreationDate, Category, Description, Infos, Images, DownloadLinks
                // documentDownloaded : State, DownloadItemLinks, DownloadedFiles, UncompressFiles, RequestTime, StartDownloadTime, EndDownloadTime, DownloadDuration

                BsonDocument resultDocument = new BsonDocument();
                resultDocument.zSet("_id", document.zGet("_id"));
                resultDocument.zSet("server", serverName);
                resultDocument.zSet("LoadFromWebDate", documentDownload.zGet("LoadFromWebDate"));
                resultDocument.zSet("Title", documentDownload.zGet("Title"));
                resultDocument.zSet("State", documentDownloaded.zGet("State"));
                resultDocument.zSet("PrintType", documentDownload.zGet("PrintType"));
                resultDocument.zSet("Category", documentDownload.zGet("Category"));
                resultDocument.zSet("PostAuthor", documentDownload.zGet("PostAuthor"));
                resultDocument.zSet("PostCreationDate", documentDownload.zGet("PostCreationDate"));
                resultDocument.zSet("SourceUrl", documentDownload.zGet("SourceUrl"));
                if (file)
                {
                    resultDocument.zSet("DownloadedFiles", documentDownloaded.zGet("DownloadedFiles"));
                    resultDocument.zSet("UncompressFiles", documentDownloaded.zGet("UncompressFiles"));
                }

                yield return resultDocument;
            }
        }

        // old
        public static void Test_ManageDirectoryGroup_01(string[] directories, string[] subDirectories, string bonusDirectory = null, bool usePrintDirectories = true, bool simulate = true, bool moveFiles = false)
        {
            PrintFileManager printFileManager = CreatePrintFileManager(simulate: simulate, moveFiles: moveFiles);
            //Dictionary<string, List<PrintDirectoryInfo>> printDirectoriesInfos = DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(directories, usePrintDirectories);
            Dictionary<string, List<EnumDirectoryInfo>> printDirectoriesInfos = DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(directories, usePrintDirectories);
            //List<PrintDirectoryInfo> printDirectoryInfoList = DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(directories)[subDirectory];
            foreach (string subDirectory in subDirectories)
            {
                //DownloadAutomate_f.CreatePrintFileManager(simulate: simulate, moveFiles: moveFiles).ManageDirectoryGroup(printDirectoriesInfos[subDirectory], directories[0]);
                printFileManager.ManageDirectoryGroup(printDirectoriesInfos[subDirectory], directories[0], bonusDirectory);
            }
        }

        // old
        public static void Test_ManageDirectoryGroup_02(string[] directories, string[] subDirectories, string bonusDirectory = null, bool usePrintDirectories = true, bool simulate = true, bool moveFiles = false)
        {
            PrintFileManager printFileManager = CreatePrintFileManager(simulate: simulate, moveFiles: moveFiles);
            //Dictionary<string, List<PrintDirectoryInfo>> printDirectoriesInfos = CreatePrintDirectoryManager().GetDirectoryGroups(directories, usePrintDirectories);
            Dictionary<string, List<EnumDirectoryInfo>> printDirectoriesInfos = CreatePrintDirectoryManager().GetDirectoryGroups(directories, usePrintDirectories);
            foreach (string subDirectory2 in printDirectoriesInfos.Keys)
            {
                foreach (string subDirectory in subDirectories)
                {
                    if (subDirectory2.StartsWith(subDirectory))
                    {
                        Trace.WriteLine("manage directory group \"{0}\"", subDirectory2);
                        printFileManager.ManageDirectoryGroup(printDirectoriesInfos[subDirectory2], directories[0], bonusDirectory);
                    }
                }
            }
        }

        // old
        public static void Test_ManageDirectoryGroup_03(string[] directories, string bonusDirectory = null, bool usePrintDirectories = true, bool simulate = true, bool moveFiles = false)
        {
            PrintFileManager printFileManager = CreatePrintFileManager(simulate: simulate, moveFiles: moveFiles);
            //Dictionary<string, List<PrintDirectoryInfo>> printDirectoriesInfos = CreatePrintDirectoryManager().GetDirectoryGroups(directories, usePrintDirectories);
            Dictionary<string, List<EnumDirectoryInfo>> printDirectoriesInfos = CreatePrintDirectoryManager().GetDirectoryGroups(directories, usePrintDirectories);
            foreach (string subDirectory2 in printDirectoriesInfos.Keys)
            {
                Trace.WriteLine("manage directory group \"{0}\"", subDirectory2);
                printFileManager.ManageDirectoryGroup(printDirectoriesInfos[subDirectory2], directories[0], bonusDirectory);
            }
        }

        public static void Test_ManageDirectories_01(IEnumerable<string> sourceDirectories, string destinationDirectory, string bonusDirectory = null, bool usePrintDirectories = true,
            bool simulate = true, bool moveFiles = false, Func<string, bool> directoryFilter = null)
        {
            PrintFileManager_v2 printFileManager = CreatePrintFileManager_v2(simulate: simulate, moveFiles: moveFiles);
            PrintDirectoryManager  printDirectoryManager = CreatePrintDirectoryManager();
            foreach (string sourceDirectory in sourceDirectories)
            {
                foreach (EnumDirectoryInfo directory in CreatePrintDirectoryManager().GetDirectories(sourceDirectory, usePrintDirectories: usePrintDirectories))
                {
                    if (directoryFilter != null)
                    {
                        if (!directoryFilter(directory.SubDirectory))
                            continue;
                    }
                    printFileManager.ManageDirectory(directory.Directory, zPath.Combine(destinationDirectory, directory.SubDirectory), bonusDirectory);
                    //directory.Directory.zTrace();
                    //zPath.Combine(destinationDirectory, directory.SubDirectory).zTrace();
                    //"".zTrace();
                }
            }
        }

        public static void Test_ManageDirectory_01(string sourceDirectory, string destinationDirectory, string bonusDirectory = null, bool simulate = true, bool moveFiles = false)
        {
            PrintFileManager_v2 printFileManager = CreatePrintFileManager_v2(simulate: simulate, moveFiles: moveFiles);
            printFileManager.ManageDirectory(sourceDirectory, destinationDirectory, bonusDirectory);
        }

        //public static void Test_RenamePrintFiles_01(string sourceDirectory, string destinationDirectory, bool simulate = true, int version = 3, string printSubDirectory = @".01_quotidien\Journaux")
        //{
        //    // sourceDirectory      : g:\pib\media\ebook\_dl\_dl_pib\print\02\print
        //    // destinationDirectory : g:\pib\media\ebook
        //    FindPrintManager findPrintManager = CreateFindPrintManager(version);
        //    foreach (EnumDirectoryInfo dir in zdir.EnumerateDirectoriesInfo(zPath.Combine(sourceDirectory, printSubDirectory), 1, 1))
        //    {
        //        PrintFileManager_v2.RenamePrintFiles(findPrintManager, dir.Directory, destinationDirectory, simulate);
        //    }
        //}

        public static void Test_GetDirectoryInfo_01(string directory, bool excludeBonusDirectory = true)
        {
            //zdir.EnumerateDirectoriesInfo(directory, minLevel: 2).Select(dir => zPath.GetFileName(dir.SubDirectory)).zToJson().zTrace();
            //.zToJson().zTrace()
            //zdir.EnumerateDirectoriesInfo(directory, minLevel: 2).Select(dir => zPath.GetFileName(dir.SubDirectory)).GroupBy(dir => dir).Select(group => new { dir = group.Key, count = group.Count() })
            //    .Where(group => group.count > 1).OrderByDescending(group => group.count).zView();

            Func<EnumDirectoryInfo, EnumDirectoryFilter> directoryFilter = null;
            if (excludeBonusDirectory)
                directoryFilter = CreatePrintFileManager_v2().NotBonusDirectoryFilter;
            var directories = zdir.EnumerateDirectoriesInfo(directory, minLevel: 2, directoryFilter: directoryFilter).Select(dir => new { dirInfo = dir, subDirectory = zPath.GetFileName(dir.SubDirectory) }).ToArray();
            directories.GroupBy(dir => dir.subDirectory).Select(group => new { dir = group.Key, count = group.Count() }).Where(group => group.count > 1).OrderByDescending(group => group.count)
                .Join(directories, dir => dir.dir, dir => dir.subDirectory, (group, dir) => new { dir = group.dir, count = group.count, subDirectory = dir.dirInfo.SubDirectory }).zView();
        }

        public static void Test_DownloadAutomate_01(bool loadNewPost = false, bool searchPostToDownload = true, bool uncompressFile = true, bool sendMail = false, int version = 3, bool useNewDownloadManager = false,
            bool useTestManager = false, int? traceLevel = null)
        {
            //bool searchPostToDownload = true;
            //_lastRunDateTime = new DateTime(2014, 8, 14, 10, 0, 0);
            DownloadAutomateManager downloadAutomate = CreateDownloadAutomate(version: version, useNewDownloadManager: useNewDownloadManager, useTestManager: useTestManager, traceLevel: traceLevel);
            try
            {
                downloadAutomate.Run(loadNewPost: loadNewPost, searchPostToDownload: searchPostToDownload, uncompressFile: uncompressFile, sendMail: sendMail);
            }
            finally
            {
                downloadAutomate.Dispose();
            }
        }

        public static void Test_TryDownload_02<TKey, TData>(WebDataManager<TKey, TData> webDataManager, string query, string downloadDirectory = null, bool uncompressFile = true, bool forceDownloadAgain = false,
            bool forceSelect = false, bool simulateDownload = false, bool useNewDownloadManager = false, int version = 3, bool useTestManager = false)
        {
            bool loadNewPost = false;
            //string downloadDirectory = "extreme-down.net";
            bool searchPostToDownload = false;
            bool sendMail = false;
            string sort = "{ 'download.creationDate': -1 }";
            //bool forceDownloadAgain = true;
            //bool useNewDownloadManager = false;
            DownloadAutomateManager downloadAutomate = CreateDownloadAutomate(version: version, useNewDownloadManager: useNewDownloadManager, useTestManager: useTestManager);
            downloadAutomate.DesactivateFilterTracePost = true;
            try
            {
                //foreach (IPost post in webDocumentStore.FindDocuments(query, sort: sort, loadImage: false))
                foreach (IPost post in webDataManager.FindDocuments(query, sort: sort, loadImage: false))
                {
                    downloadAutomate.TryDownloadPost(post, downloadDirectory: downloadDirectory, forceDownloadAgain: forceDownloadAgain, forceSelect: forceSelect, simulateDownload: simulateDownload);
                }
                downloadAutomate.Run(loadNewPost: loadNewPost, searchPostToDownload: searchPostToDownload, uncompressFile: uncompressFile, sendMail: sendMail);
            }
            finally
            {
                downloadAutomate.Dispose();
            }
        }

        public static void Test_ManagePrintDirectory_01(string[] directories, int level = 1, string journauxDirectory = null, bool simulate = false, bool moveFiles = false)
        {
            CreatePrintFileManager(simulate: simulate, moveFiles: moveFiles).ManageDirectory_v1(GetPrintDirectoryGroups(directories, level, journauxDirectory), directories[0]);
        }

        public static void Test_GetPrintDirectories_01(string[] directories, int level = 1, string journauxDirectory = null)
        {
            Trace.WriteLine("get directories from");
            foreach (string directory in directories)
                Trace.WriteLine("  \"{0}\"", directory);
            //int level = 2;
            //int l = directory.Length;
            //var query = from dir in zdir.EnumerateDirectories(directory, minLevel: level, maxLevel: level)
            //            select
            //                new DirectoryGroup
            //                {
            //                    BaseDirectory = directory,
            //                    Directory = dir,
            //                    SubDirectory = dir.Substring(l + 1)
            //                };
            //var query2 = from dir in zdir.EnumerateDirectoriesInfo(directory,
            //            filter: dirInfo =>
            //                new EnumDirectoryFilter
            //                {
            //                    Select = dirInfo.Level == level || (dirInfo.SubDirectory.StartsWith(".01_quotidien\\Journaux\\") && dirInfo.Level == level + 1),
            //                    RecurseSubDirectory = dirInfo.Level < level || (dirInfo.SubDirectory == ".01_quotidien\\Journaux" && dirInfo.Level < level + 1)
            //                }
            //            )
            //            select
            //                new DirectoryGroup
            //                {
            //                    BaseDirectory = directory,
            //                    Directory = dir.Directory,
            //                    SubDirectory = dir.SubDirectory
            //                };

            //Dictionary<string, List<DirectoryGroup>> directoryGroups = new Dictionary<string, List<DirectoryGroup>>();
            ////directoryGroups.zAddKeyList(query, dir => dir.SubDirectory);
            //directoryGroups.zAddKeyList(query2, dir => dir.SubDirectory);
            //Trace.WriteLine(directoryGroups.zToJson());
            Trace.WriteLine(GetPrintDirectoryGroups(directories, level, journauxDirectory).zToJson());
        }

        public static Dictionary<string, List<DirectoryGroup>> GetPrintDirectoryGroups(string[] directories, int level = 1, string journauxDirectory = null)
        {
            //int level = 2;
            string journauxDirectory2 = null;
            if (journauxDirectory != null)
                journauxDirectory2 = journauxDirectory + "\\";
            Dictionary<string, List<DirectoryGroup>> directoryGroups = new Dictionary<string, List<DirectoryGroup>>();
            foreach (string directory in directories)
            {
                var query = zdir.EnumerateDirectoriesInfo(directory,
                            directoryFilter: dirInfo =>
                                {
                                    //bool journauxDir = dirInfo.SubDirectory == ".01_quotidien\\Journaux";
                                    //bool journauxSubdir = dirInfo.SubDirectory.StartsWith(".01_quotidien\\Journaux\\");
                                    bool journauxDir = false;
                                    if (journauxDirectory != null)
                                        journauxDir = dirInfo.SubDirectory == journauxDirectory;
                                    bool journauxSubdir = false;
                                    if (journauxDirectory2 != null)
                                        journauxSubdir = dirInfo.SubDirectory.StartsWith(journauxDirectory2);
                                    return new EnumDirectoryFilter
                                    {
                                        Select = (!journauxDir && dirInfo.Level == level) || (journauxSubdir && dirInfo.Level == level + 1),
                                        RecurseSubDirectory = dirInfo.Level < level || (journauxDir && dirInfo.Level < level + 1)
                                    };
                                }
                            ).Select(
                                dir =>
                                        new DirectoryGroup
                                        {
                                            BaseDirectory = directory,
                                            Directory = dir.Directory,
                                            //SubDirectory = dir.SubDirectory
                                            //SubDirectory = zPath.Combine(zPath.GetDirectoryName(dir.SubDirectory), zpath.PathGetFilenameNumberInfo(dir.SubDirectory).BaseFilename)
                                            SubDirectory = zPath.Combine(zPath.GetDirectoryName(dir.SubDirectory), FilenameNumberInfo.GetFilenameNumberInfo(dir.SubDirectory).BaseFilename)
                                        }
                                    );

                directoryGroups.zKeyListAdd(query, dir => dir.SubDirectory);
            }
            return directoryGroups;
        }

        public static void Test_PostDownloadLinks_01()
        {
            PostDownloadLinks links = new PostDownloadLinks();
            links.AddItem("test1");
            links.AddServer("Uploaded");
            links.AddLink("http://extreme-protect.net/EVGrmAwYOl");
            Trace.WriteLine(links.zToJson());


            //BsonClassMap.RegisterClassMap<PostDownloadLinks>(cm =>
            //{
            //    cm.AutoMap();
            //    cm.SetIgnoreExtraElements(true);
            //});
            //MongoDB.Bson.Serialization.Conventions.

        }

        public static PrintDirectoryManager CreatePrintDirectoryManager()
        {
            return new PrintDirectoryManager(XmlConfig.CurrentConfig.GetConfig("PrintList2Config").GetValues("PrintDirectories/Directory").ToArray());
        }

        // bool dailyPrintManager = false
        public static PrintTitleManager CreatePrintTitleManager(int version = 3, int gapDayBefore = 0, int gapDayAfter = 0)
        {
            // version 2 : utilise le nouveau PrintTitleManager, l'ancien pattern de date FindPrints/Dates/Date avec l'ancien FindDateManager
            // version 3 : version 2 + le nouveau pattern de date FindPrints/Dates/DateNew avec le nouveau FindDateManager_new
            // version 4 (not used) : version 3 + découpe le titre avec "du" ou "-" (PrintTitleManager)
            // version 5 : version 3 +  new find date
            // version 6 : version 5 +  printTitleManager version 2

            if (version < 3 || version > 6)
                throw new PBException("bad version {0}", version);

            XmlConfig config = XmlConfig.CurrentConfig;
            PrintTitleManager printTitleManager = new PrintTitleManager();
            //printTitleManager.FindDateManager = new FindDateManager(config.GetConfig("PrintList1Config").GetElements("FindPrints/Dates/DateNew"), compileRegex: true);
            printTitleManager.FindDateManager = CreateFindDateManager(version, gapDayBefore, gapDayAfter);
            //if (dailyPrintManager)
            //{
            //    printTitleManager.FindDayManager = CreateFindDayManager();
            //    printTitleManager.UseFindDay = true;
            //    printTitleManager.GapDayBefore = gapDayBefore;
            //    printTitleManager.GapDayAfter = gapDayAfter;
            //}
            printTitleManager.FindNumberManager = new FindNumberManager(config.GetConfig("PrintList1Config").GetElements("FindPrints/Numbers/Number"), compileRegex: true);
            printTitleManager.FindSpecial = new RegexValuesList(config.GetConfig("PrintList1Config").GetElements("FindPrints/Specials/Special"), compileRegex: true);
            printTitleManager.PrintDirectory = config.GetConfig("PrintList1Config").GetExplicit("FindPrints/UnknowPrintDirectory");
            if (version == 4)
                printTitleManager.SplitTitle = true;
            else
                printTitleManager.SplitTitle = false;
            if (version == 6)
                printTitleManager.Version = 2;
            return printTitleManager;
        }

        public static FindDateManager CreateFindDateManager(int version = 3, int gapDayBefore = 0, int gapDayAfter = 0)
        {
            // version 5 : version 3 +  new find date
            //if (version < 3 || version > 6)
            //    throw new PBException("bad version {0}", version);
            FindDateManager findDateManager;
            if (version < 5)
                findDateManager = new FindDateManager(XmlConfig.CurrentConfig.GetConfig("PrintList1Config").GetElements("FindPrints/Dates/Date"), compileRegex: true);
            else
            {
                //findDateManager = new FindDateManager(XmlConfig.CurrentConfig.GetConfig("PrintList1Config").GetElements("FindPrints/Dates/Date2"), compileRegex: true);
                findDateManager = new FindDateManager(XmlConfig.CurrentConfig.GetConfig("PrintList1Config").GetElements("FindPrints/Dates/Date2"),
                    XmlConfig.CurrentConfig.GetConfig("PrintList1Config").GetElements("FindPrints/Dates/DigitDate"),
                    compileRegex: true);
                findDateManager.MultipleSearch = true;
                findDateManager.GapDayBefore = gapDayBefore;
                findDateManager.GapDayAfter = gapDayAfter;
            }
            return findDateManager;
        }

        public static FindDayManager CreateFindDayManager()
        {
            return new FindDayManager(XmlConfig.CurrentConfig.GetConfig("PrintList1Config").GetElements("FindPrints/Dates/ShortDay"), compileRegex: true);
        }

        private static Dictionary<PrintType, string> GetPostTypeDirectories()
        {
            XmlConfig config = XmlConfig.CurrentConfig;
            Dictionary<PrintType, string> postTypeDirectories = new Dictionary<PrintType, string>();
            postTypeDirectories.Add(PrintType.Print, config.GetConfig("PrintList1Config").GetExplicit("FindPrints/PostTypeDirectories/Print"));
            postTypeDirectories.Add(PrintType.Book, config.GetConfig("PrintList1Config").GetExplicit("FindPrints/PostTypeDirectories/Book"));
            postTypeDirectories.Add(PrintType.Comics, config.GetConfig("PrintList1Config").GetExplicit("FindPrints/PostTypeDirectories/Comics"));
            postTypeDirectories.Add(PrintType.UnknowEBook, config.GetConfig("PrintList1Config").GetExplicit("FindPrints/PostTypeDirectories/UnknowEBook"));
            postTypeDirectories.Add(PrintType.Unknow, config.GetConfig("PrintList1Config").GetExplicit("FindPrints/PostTypeDirectories/UnknowPostType"));
            return postTypeDirectories;
        }

        public static UncompressManager CreateUncompressManager()
        {
            UncompressManager uncompressManager = new UncompressManager();
            uncompressManager.ArchiveCompressDirectory = XmlConfig.CurrentConfig.Get("DownloadAutomateManager/UncompressManager/ArchiveCompressDirectory");
            uncompressManager.ArchiveErrorCompressDirectory = XmlConfig.CurrentConfig.Get("DownloadAutomateManager/UncompressManager/ArchiveErrorCompressDirectory");
            return uncompressManager;
        }

        public static PrintFileManager CreatePrintFileManager(UncompressManager uncompressManager = null, bool simulate = false, bool moveFiles = false)
        {
            if (uncompressManager == null)
                uncompressManager = CreateUncompressManager();
            PrintFileManager printFileManager = new PrintFileManager(uncompressManager);
            printFileManager.Simulate = simulate;
            printFileManager.MoveFiles = moveFiles;
            return printFileManager;
        }

        public static PrintFileManager_v2 CreatePrintFileManager_v2(UncompressManager uncompressManager = null, bool simulate = false, bool moveFiles = false)
        {
            if (uncompressManager == null)
                uncompressManager = CreateUncompressManager();
            RegexValuesList bonusDirectories = new RegexValuesList(XmlConfig.CurrentConfig.GetConfig("PrintList2Config").GetElements("BonusDirectories/Directory"), compileRegex: true);
            PrintFileManager_v2 printFileManager = new PrintFileManager_v2();
            printFileManager.Simulate = simulate;
            printFileManager.MoveFiles = moveFiles;
            printFileManager.UncompressManager = uncompressManager;
            printFileManager.BonusDirectories = bonusDirectories;
            return printFileManager;
        }

        public static Debrider CreateDebrider()
        {
            //return CreateDebriderAlldebrid();
            return CreateDebriderDebridLinkFr();
        }

        public static Debrider CreateDebriderAlldebrid()
        {
            //if (localConfig == null)
            //    localConfig = new XmlConfig(XmlConfig.CurrentConfig.GetExplicit("LocalConfig"));
            //return new DebriderAlldebrid(localConfig.GetExplicit("DownloadAutomateManager/Alldebrid/Login"), localConfig.GetExplicit("DownloadAutomateManager/Alldebrid/Password"));
            return new DebriderAlldebrid(XmlConfig.CurrentConfig.GetConfig("LocalConfig").GetExplicit("DownloadAutomateManager/Alldebrid/Login"),
                XmlConfig.CurrentConfig.GetConfig("LocalConfig").GetExplicit("DownloadAutomateManager/Alldebrid/Password"));
        }

        public static Debrider CreateDebriderDebridLinkFr()
        {
            DebriderDebridLinkFr debrider = new DebriderDebridLinkFr();
            debrider.DebridLinkFr = CreateDebridLinkFr();
            return debrider;
        }

        public static DebridLinkFr CreateDebridLinkFr()
        {
            DebridLinkFr debrider = new DebridLinkFr();
            XmlConfig localConfig = XmlConfig.CurrentConfig.GetConfig("LocalConfig");
            debrider.Login = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Login");
            debrider.Password = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/Password");
            debrider.PublicKey = localConfig.GetExplicit("DownloadAutomateManager/DebridLink/PublicKey");
            debrider.ConnexionLifetime = DebridLinkFr.GetConnexionLifetime(localConfig.GetExplicit("DownloadAutomateManager/DebridLink/ConnexionLifetime"));
            debrider.ConnexionFile = XmlConfig.CurrentConfig.GetExplicit("DebridLink/ConnexionFile");
            //debrider.ServerTimeFile = XmlConfig.CurrentConfig.GetExplicit("DebridLink/ServerTimeFile");
            return debrider;
        }

        public static DownloadManagerClientBase CreateDownloadManagerClient(bool useTestManager = false)
        {
            DownloadManagerClientBase downloadManagerClient = null;
            if (!useTestManager)
                downloadManagerClient = new DownloadManagerClient(XmlConfig.CurrentConfig.GetExplicit("DownloadAutomateManager/DownloadManagerClient/Address"));
            else
                downloadManagerClient = new DownloadManagerClientTest(@"c:\pib\_dl\_pib\dl");
            return downloadManagerClient;
        }

        public static DownloadManager_v1<DownloadPostKey> CreateDownloadManager_v1(bool useTestManager = false)
        {
            XmlConfig config = XmlConfig.CurrentConfig;

            //DownloadManagerClientBase downloadManagerClient = null;
            //if (!useTestManager)
            //    downloadManagerClient = new DownloadManagerClient(config.Get("DownloadAutomateManager/DownloadManagerClient/Address"));
            //else
            //    downloadManagerClient = new DownloadManagerClientTest(@"c:\pib\_dl\_pib\dl");
            DownloadManagerClientBase downloadManagerClient = CreateDownloadManagerClient(useTestManager);

            //MongoCollectionManager_v1<DownloadPostKey, DownloadedFile_v1<DownloadPostKey>> mongoDownloadManager = new MongoCollectionManager_v1<DownloadPostKey, DownloadedFile_v1<DownloadPostKey>>(
            //    config.Get("DownloadAutomateManager/MongoDownloadFile/MongoServer"),
            //    config.Get("DownloadAutomateManager/MongoDownloadFile/MongoDatabase"),
            //    config.Get("DownloadAutomateManager/MongoDownloadFile/MongoCollection"),
            //    key => new QueryDocument { { "downloadFile.key.server", key.server }, { "downloadFile.key._id", BsonValue.Create(key.id) } },
            //    config.Get("DownloadAutomateManager/MongoDownloadFile/MongoDocumentItemName"));
            MongoCollectionManager_v1<DownloadPostKey, DownloadedFile_v1<DownloadPostKey>> mongoDownloadManager =
                new MongoCollectionManager_v1<DownloadPostKey, DownloadedFile_v1<DownloadPostKey>>(config.GetElement("DownloadAutomateManager/MongoDownloadFile"));
            mongoDownloadManager.QueryKey = key => new QueryDocument { { "downloadFile.key.server", key.server }, { "downloadFile.key._id", BsonValue.Create(key.id) } };

            //MongoCollectionManager_v1<DownloadPostKey, DownloadFile_v1<DownloadPostKey>> mongoQueueDownloadManager = new MongoCollectionManager_v1<DownloadPostKey, DownloadFile_v1<DownloadPostKey>>(
            //    config.Get("DownloadAutomateManager/MongoQueueDownloadFile/MongoServer"),
            //    config.Get("DownloadAutomateManager/MongoQueueDownloadFile/MongoDatabase"),
            //    config.Get("DownloadAutomateManager/MongoQueueDownloadFile/MongoCollection"),
            //    key => new QueryDocument { { "queueDownloadFile.key.server", key.server }, { "queueDownloadFile.key._id", BsonValue.Create(key.id) } },
            //    config.Get("DownloadAutomateManager/MongoQueueDownloadFile/MongoDocumentItemName"));
            MongoCollectionManager_v1<DownloadPostKey, DownloadFile_v1<DownloadPostKey>> mongoQueueDownloadManager =
                new MongoCollectionManager_v1<DownloadPostKey, DownloadFile_v1<DownloadPostKey>>(config.GetElement("DownloadAutomateManager/MongoQueueDownloadFile"));
            mongoQueueDownloadManager.QueryKey = key => new QueryDocument { { "queueDownloadFile.key.server", key.server }, { "queueDownloadFile.key._id", BsonValue.Create(key.id) } };

            Debrider debrider = CreateDebrider();

            return new DownloadManager_v1<DownloadPostKey>(downloadManagerClient, mongoDownloadManager, mongoQueueDownloadManager, debrider, CreateUncompressManager());
        }

        public static DownloadManager<DownloadPostKey> CreateDownloadManager(bool useTestManager = false)
        {
            XmlConfig config = XmlConfig.CurrentConfig;

            //DownloadManagerClientBase downloadManagerClient = null;
            //if (!useTestManager)
            //    downloadManagerClient = new DownloadManagerClient(config.Get("DownloadAutomateManager/DownloadManagerClient/Address"));
            //else
            //    downloadManagerClient = new DownloadManagerClientTest(@"c:\pib\_dl\_pib\dl");

            //MongoCollectionManager_v1<DownloadPostKey, DownloadedFile<DownloadPostKey>> mongoDownloadedFileManager =
            //    new MongoCollectionManager_v1<DownloadPostKey, DownloadedFile<DownloadPostKey>>(
            //        config.Get("DownloadAutomateManager/MongoDownloadedFile/MongoServer"),
            //        config.Get("DownloadAutomateManager/MongoDownloadedFile/MongoDatabase"),
            //        config.Get("DownloadAutomateManager/MongoDownloadedFile/MongoCollection"),
            //        key => new QueryDocument { { "downloadedFile.Key.server", key.server }, { "downloadedFile.Key._id", BsonValue.Create(key.id) } },
            //        config.Get("DownloadAutomateManager/MongoDownloadedFile/MongoDocumentItemName"));
            MongoCollectionManager_v1<DownloadPostKey, DownloadedFile<DownloadPostKey>> mongoDownloadedFileManager =
                new MongoCollectionManager_v1<DownloadPostKey, DownloadedFile<DownloadPostKey>>(config.GetElement("DownloadAutomateManager/MongoDownloadedFile"));
            mongoDownloadedFileManager.QueryKey = key => new QueryDocument { { "downloadedFile.Key.server", key.server }, { "downloadedFile.Key._id", BsonValue.Create(key.id) } };

            //MongoCollectionManager_v1<DownloadPostKey, QueueDownloadFile<DownloadPostKey>> mongoQueueDownloadFileManager =
            //    new MongoCollectionManager_v1<DownloadPostKey, QueueDownloadFile<DownloadPostKey>>(
            //        config.Get("DownloadAutomateManager/MongoQueueDownloadFile_new/MongoServer"),
            //        config.Get("DownloadAutomateManager/MongoQueueDownloadFile_new/MongoDatabase"),
            //        config.Get("DownloadAutomateManager/MongoQueueDownloadFile_new/MongoCollection"),
            //        key => new QueryDocument { { "queueDownloadFile.Key.server", key.server }, { "queueDownloadFile.Key._id", BsonValue.Create(key.id) } },
            //        config.Get("DownloadAutomateManager/MongoQueueDownloadFile_new/MongoDocumentItemName"));
            MongoCollectionManager_v1<DownloadPostKey, QueueDownloadFile<DownloadPostKey>> mongoQueueDownloadFileManager =
                new MongoCollectionManager_v1<DownloadPostKey, QueueDownloadFile<DownloadPostKey>>(config.GetElement("DownloadAutomateManager/MongoQueueDownloadFile_new"));
            mongoQueueDownloadFileManager.QueryKey = key => new QueryDocument { { "queueDownloadFile.Key.server", key.server }, { "queueDownloadFile.Key._id", BsonValue.Create(key.id) } };

            //MongoCollectionManager_v1<DownloadPostKey, DownloadLinkRef> mongoCurrentDownloadFileManager =
            //    new MongoCollectionManager_v1<DownloadPostKey,DownloadLinkRef>(
            //        config.Get("DownloadAutomateManager/MongoCurrentDownloadFile/MongoServer"),
            //        config.Get("DownloadAutomateManager/MongoCurrentDownloadFile/MongoDatabase"),
            //        config.Get("DownloadAutomateManager/MongoCurrentDownloadFile/MongoCollection"),
            //        null,
            //        config.Get("DownloadAutomateManager/MongoCurrentDownloadFile/MongoDocumentItemName"));
            MongoCollectionManager_v1<DownloadPostKey, DownloadLinkRef> mongoCurrentDownloadFileManager =
                new MongoCollectionManager_v1<DownloadPostKey, DownloadLinkRef>(config.GetElement("DownloadAutomateManager/MongoCurrentDownloadFile"));

            ProtectLink protectLink = null;
            //if (!useTestManager)
            //    protectLink = new Download.Print.ExtremeDown.ExtremeProtect();
            //else
            //    protectLink = new Download.Print.ExtremeDown.ExtremeProtectTest();

            Debrider debrider = null;
            if (!useTestManager)
                debrider = CreateDebrider();
            else
                debrider = new DebriderAlldebridTest();

            DownloadManager<DownloadPostKey> downloadManager = new DownloadManager<DownloadPostKey>();
            downloadManager.DownloadManagerClient = CreateDownloadManagerClient(useTestManager);
            downloadManager.MongoDownloadedFileManager = mongoDownloadedFileManager;
            downloadManager.MongoQueueDownloadFileManager = mongoQueueDownloadFileManager;
            downloadManager.MongoCurrentDownloadFileManager = mongoCurrentDownloadFileManager;
            downloadManager.ProtectLink = protectLink;
            downloadManager.Debrider = debrider;
            downloadManager.UncompressManager = CreateUncompressManager();
            return downloadManager;
        }

        public static FindPrintManager CreateFindPrintManager(int version = 3, bool dailyPrintManager = false, int gapDayBefore = 0, int gapDayAfter = 0)
        {
            XmlConfig config = XmlConfig.CurrentConfig;
            FindPrintManager findPrintManager = new FindPrintManager();
            findPrintManager.PrintTitleManager = CreatePrintTitleManager(version, gapDayBefore, gapDayAfter);
            findPrintManager.FindPrintList = new RegexValuesList(config.GetConfig("PrintList2Config").GetElements("FindPrints/Prints/Print"), compileRegex: true);
            if (dailyPrintManager)
            {
                findPrintManager.FindPrintList.Add(config.GetConfig("PrintList2Config").GetElements("FindPrints/Prints/ShortPrint"), compileRegex: true);
                findPrintManager.FindDayManager = CreateFindDayManager();
                findPrintManager.UseFindDay = true;
                findPrintManager.GapDayBefore = gapDayBefore;
                findPrintManager.GapDayAfter = gapDayAfter;
            }
            findPrintManager.PrintManager = CreatePrintManager();
            findPrintManager.PostTypeDirectories = GetPostTypeDirectories();
            findPrintManager.DefaultPrintDirectory = config.GetConfig("PrintList1Config").Get("FindPrints/DefaultPrintDirectory");
            findPrintManager.UnknowPrintDirectory = config.GetConfig("PrintList1Config").Get("FindPrints/UnknowPrintDirectory");

            if (version >= 6)
                findPrintManager.Version = 2;

            return findPrintManager;
        }

        public static DownloadAutomateManager CreateDownloadAutomate(int version = 3, bool useNewDownloadManager = false, bool useTestManager = false, int? traceLevel = null)
        {
            // le 01/11/2014 désactive version 1 et version 2
            // version 1 : utilise l'ancien FindPrintManager, l'ancienne liste de print dans print_list1.xml, l'ancien pattern de date FindPrints/Dates/Date avec l'ancien FindDateManager
            // version 2 : version 1 + le nouveau FindPrintManager_new avec le nouveau PrintTitleManager, la nouvelle liste de print dans print_list2.xml,
            // version 3 : version 2 + le nouveau pattern de date FindPrints/Dates/DateNew avec le nouveau FindDateManager_new
            // version 4 (not used) : version 3 + découpe le titre avec "du" ou "-" (PrintTitleManager)
            // version 5 : version 3 +  new find date
            // version 6 : version 5 +  printTitleManager version 2 + findPrintManager version 2

            if (version < 3 || version > 6)
                throw new PBException("bad version {0}", version);

            Trace.WriteLine("create download automate : version {0} useNewDownloadManager {1} useTestManager {2} traceLevel {3}", version, useNewDownloadManager, useTestManager, traceLevel.zToStringOrNull());

            XmlConfig config = XmlConfig.CurrentConfig;
            MongoDownloadAutomateManager mongoDownloadAutomateManager = GetMongoDownloadAutomateManager();

            DownloadManager_v1<DownloadPostKey> downloadManager_v1 = null;
            if (!useNewDownloadManager)
                downloadManager_v1 = CreateDownloadManager_v1(useTestManager);

            DownloadManager<DownloadPostKey> downloadManager = null;
            if (useNewDownloadManager)
                downloadManager = CreateDownloadManager(useTestManager);

            MailSender mailSender = new MailSender(config.GetConfig("LocalConfig").Get("DownloadAutomateManager/Gmail/Mail"), config.GetConfig("LocalConfig").Get("DownloadAutomateManager/Gmail/Password"),
                config.GetConfig("LocalConfig").Get("DownloadAutomateManager/Gmail/SmtpHost"), config.GetConfig("LocalConfig").Get("DownloadAutomateManager/Gmail/SmtpPort").zTryParseAs<int>(587));
            MailMessage mailMessage = new MailMessage(config.GetConfig("LocalConfig").Get("DownloadAutomateManager/Gmail/Mail"), config.Get("DownloadAutomateManager/Mail/To"),
                config.Get("DownloadAutomateManager/Mail/Subject"), config.Get("DownloadAutomateManager/Mail/Body"));

            DownloadAutomateManager downloadAutomate = new DownloadAutomateManager();

            downloadAutomate.MongoDownloadAutomateManager = mongoDownloadAutomateManager;
            downloadAutomate.DownloadAllPrintType = printType => printType == PrintType.Print || printType == PrintType.Book || printType == PrintType.UnknowEBook;
            downloadAutomate.FindPrintManager = CreateFindPrintManager(version);
            downloadAutomate.PrintManager = downloadAutomate.FindPrintManager.PrintManager;
            downloadAutomate.DownloadManager_v1 = downloadManager_v1;
            downloadAutomate.DownloadManager = downloadManager;
            downloadAutomate.MailSender = mailSender;
            downloadAutomate.MailMessage = mailMessage;

            Ebookdz.Ebookdz.FakeInit();
            Vosbooks.Vosbooks.FakeInit();
            TelechargerMagazine.TelechargerMagazine.FakeInit();
            ExtremeDown.ExtremeDown.FakeInit();
            foreach (XElement xe in config.GetElements("DownloadAutomateManager/ServerManagers/ServerManager"))
            {
                ServerManager serverManager = ServerManagers.Get(xe.zExplicitAttribValue("name"));
                serverManager.EnableLoadNewPost = xe.zAttribValue("enableLoadNewPost").zTryParseAs<bool>(true);
                serverManager.EnableSearchPostToDownload = xe.zAttribValue("enableSearchPostToDownload").zTryParseAs<bool>(true);
                serverManager.DownloadDirectory = xe.zAttribValue("downloadDirectory").zNullIfEmpty();
                downloadAutomate.AddServerManager(serverManager);
                Trace.WriteLine("add server manager \"{0}\" enable load new post {1} enable search post to download {2} download directory \"{3}\"", serverManager.Name, serverManager.EnableLoadNewPost, serverManager.EnableSearchPostToDownload, serverManager.DownloadDirectory);
            }

            Trace.WriteLine("mongo download automate manager collection {0}", downloadAutomate.MongoDownloadAutomateManager.GetCollection().zGetFullName());
            if (downloadAutomate.DownloadManager_v1 != null)
            {
                Trace.WriteLine("mongo download manager collection                 {0}", downloadAutomate.DownloadManager_v1.MongoDownloadedFileManager.GetCollection().zGetFullName());
                Trace.WriteLine("mongo queue download manager collection           {0}", downloadAutomate.DownloadManager_v1.MongoQueueDownloadFileManager.GetCollection().zGetFullName());
            }
            else if (downloadAutomate.DownloadManager != null)
            {
                Trace.WriteLine("mongo download manager collection                 {0}", downloadAutomate.DownloadManager.MongoDownloadedFileManager.GetCollection().zGetFullName());
                Trace.WriteLine("mongo queue download manager collection           {0}", downloadAutomate.DownloadManager.MongoQueueDownloadFileManager.GetCollection().zGetFullName());
                Trace.WriteLine("mongo current download file manager collection    {0}", downloadAutomate.DownloadManager.MongoCurrentDownloadFileManager.GetCollection().zGetFullName());
                Trace.WriteLine("download manager client                           {0}", downloadAutomate.DownloadManager.DownloadManagerClient);
            }

            Trace.WriteLine("last run time {0:dd-MM-yyyy HH:mm:ss}", downloadAutomate.MongoDownloadAutomateManager.GetLastRunDateTime());

            if (!ControlDownloadManagerClient(downloadAutomate))
                throw new PBException("error DownloadManagerClient is not working");

            if (traceLevel != null)
                Trace.CurrentTrace.TraceLevel = (int)traceLevel;

            downloadAutomate.Start();

            return downloadAutomate;
        }

        public static bool ControlDownloadManagerClient(DownloadAutomateManager downloadAutomate)
        {
            Trace.Write("control download manager client");
            try
            {
                if (downloadAutomate.DownloadManager_v1 != null)
                    downloadAutomate.DownloadManager_v1.DownloadManagerClient.GetDownloadCount();
                else if (downloadAutomate.DownloadManager != null)
                    downloadAutomate.DownloadManager.DownloadManagerClient.GetDownloadCount();
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

        public static PrintManager CreatePrintManager()
        {
            //XmlConfig config = XmlConfig.CurrentConfig;
            //XmlConfig printConfig = new XmlConfig(zPath.Combine(zPath.GetDirectoryName(config.ConfigPath), config.GetExplicit("PrintConfig")));
            return new PrintManager(XmlConfig.CurrentConfig.GetConfig("PrintConfig").GetElements("Print/Prints/Print"));
        }

        // 15 / 16 AOUT 2014  ------- 13 au 19 Août 2014
        private static Regex __printDate = new Regex(@"([0-9]+)\s*(?:(?:/|au)\s*[0-9]+\s*)?(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|juill|a[o0][uû]t?|septembre|octobre|novembre|d[eé]cembre)\s*([0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static DateTime? GetDate(string title)
        {
            Match match = __printDate.Match(title);
            if (match.Success)
                return new DateTime(int.Parse(match.Groups[3].Value), zdate.GetMonthNumber(match.Groups[2].Value), int.Parse(match.Groups[1].Value));
            else
                return null;
        }

        private static string SelectPost(Regex print, string title, string name)
        {
            Match match = print.Match(title);
            if (match.Success)
            {
                DateTime? dt = GetDate(title);
                if (dt != null)
                    return string.Format(@"{0}\{0}_{1:yyyy-MM-dd}", name, dt);
                else
                {
                    Trace.WriteLine("warning can't find date in \"{0}\"", title);
                    title = match.zReplace(title, "");
                    title = zfile.ReplaceBadFilenameChars(title, "");
                    //return string.Format(@"{0}\{0}_autre", name);
                    return string.Format(@"{0}\{0}_{1}", name, title);
                }
            }
            return null;
        }

        public static MongoDownloadAutomateManager GetMongoDownloadAutomateManager()
        {
            XmlConfig config = XmlConfig.CurrentConfig;
            return new MongoDownloadAutomateManager(config.Get("DownloadAutomateManager/MongoDownloadAutomate/MongoServer"), config.Get("DownloadAutomateManager/MongoDownloadAutomate/MongoDatabase"),
                config.Get("DownloadAutomateManager/MongoDownloadAutomate/MongoCollection"));
        }
    }
}
