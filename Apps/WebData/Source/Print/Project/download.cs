// $$info.manage.print.directory
// $$info.test.regex
// $$info.debrid-link.fr
// $$info.test_unit.print
// $$info.GetPrintTitleInfo


//*************************************************************************************************************************
//****                                   Trace
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSource.CurrentRunSource.SetProjectFromSource();

RunSource.CurrentRunSource.CompileProject(@"$Root$\Apps\WebData\Source\Print\Project\download.project.xml");

Trace.CurrentTrace.TraceLevel = 0;
Trace.CurrentTrace.TraceLevel = 1;
Trace.CurrentTrace.TraceLevel = 2;
Trace.WriteLine("TraceLevel {0}", Trace.CurrentTrace.TraceLevel);
//Trace.CurrentTrace.TraceDir = @"c:\pib\dev_data\exe\runsource\download\http";
//Trace.WriteLine("TraceLevel {0} TraceDir \"{1}\"", Trace.CurrentTrace.TraceLevel, Trace.CurrentTrace.TraceDir);

Http.Trace = true;

Compiler.TraceLevel = 1;
Compiler.TraceLevel = 2;

XNodeDescendants.Trace = false;
XNodeDescendants.Trace = true;


Trace.WriteLine(RunSource.CurrentRunSource.ProjectFile);
Trace.WriteLine(RunSource.CurrentRunSource.ProjectDirectory);

//*************************************************************************************************************************
//****                                   Compile_Project
//*************************************************************************************************************************

RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\PibApp\Pib.project.xml");
RunSource.CurrentRunSource.Compile_Project("magazine3k_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\pt\pt1_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\pt\pt_project.xml");

//Metadata file 'C:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Download\Print\download\..\..\..\..\..\DownloadManager\Source\Extension\..\..\..\dll\ICSharpCode.SharpZipLib.dll' could not be found

//RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\MyDownloader\MyDownloader.Service\MyDownloader.Service_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\MyDownloader\MyDownloader.Core\MyDownloader.Core.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\MyDownloader\MyDownloader.Extension\MyDownloader.Extension.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\MyDownloader\MyDownloader.IEPlugin\MyDownloader.IEPlugin.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\MyDownloader\MyDownloader.Spider\MyDownloader.Spider.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\MyDownloader\MyDownloader.App\MyDownloader.App.project.xml");

RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\RunSource\v2\runsource.dll\runsource.dll.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\RunSource\v2\runsource.runsource\runsource.runsource.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\RunSource\v2\runsource.launch\runsource.launch.project.xml");


RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\..\RunSource\Source\runsource_dll\irunsource_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\..\RunSource\Source\runsource_dll\runsource_common_dll_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\..\RunSource\Source\runsource_dll\runsource_dll_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\..\RunSource\Source\runsource_domain\runsourced32_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\..\RunSource\Source\runsource_domain\runsource_launch_project.xml");

RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\PibApp\Pib.project.xml");


RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\Test\Test.Test_01\Source\Test_wcf\Test_wcf_service_01\Test_wcf_service_01.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\Test\Test.Test_01\Source\Test_wcf\Test_wcf_service_02\Test_wcf_service_02.project.xml");


//*************************************************************************************************************************
//****                                   Automate
//*************************************************************************************************************************

DownloadAutomate_f.Test_DownloadAutomate_01(loadNewPost: true, searchPostToDownload: true, uncompressFile: true, sendMail: false,
  version: 3, useNewDownloadManager: true, traceLevel: 0);

DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(DateTime.Parse("2015-08-21 05:00:00"));

//*************************************************************************************************************************
//****                                   get downloaded info
//*************************************************************************************************************************
DownloadAutomate_f.GetDownloadedInfo("Ebookdz_Detail", "ebookdz.com", limit: 100, onlyNotDownloaded: false, file: false).zView();
DownloadAutomate_f.GetDownloadedInfo("Ebookdz_Detail", "ebookdz.com", limit: 300, onlyNotDownloaded: true, file: false).zView();
DownloadAutomate_f.GetDownloadedInfo("Vosbooks_Detail", "vosbooks.net", limit: 100, onlyNotDownloaded: false, file: false).zView();
DownloadAutomate_f.GetDownloadedInfo("Vosbooks_Detail", "vosbooks.net", limit: 300, onlyNotDownloaded: true, file: false).zView();
DownloadAutomate_f.GetDownloadedInfo("TelechargerMagazine_Detail", "telecharger-magazine.com", limit: 100, onlyNotDownloaded: false, file: false).zView();
DownloadAutomate_f.GetDownloadedInfo("TelechargerMagazine_Detail", "telecharger-magazine.com", limit: 300, onlyNotDownloaded: true, file: false).zView();

//*************************************************************************************************************************
//****                                   load new post
//*************************************************************************************************************************
Download.Print.Vosbooks.Vosbooks_DetailManager.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 10, startPage: 1, maxPage: 100);
Download.Print.Ebookdz.Ebookdz_DetailManager.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 7, startPage: 1, maxPage: 1);
Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 25, startPage: 1, maxPage: 100);

//*************************************************************************************************************************
//****                                   mongo pierre
//*************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Eval("{ listDatabases: 1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl");
// DownloadAutomate
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadAutomate3", Path.Combine(AppData.DataDirectory, @"mongo\export\DownloadAutomate\export_DownloadAutomate3.txt"));
// DownloadedFile :
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadedFile", Path.Combine(AppData.DataDirectory, @"mongo\export\Download\export_DownloadedFile.txt"));
pb.Data.Mongo.TraceMongoCommand.Count("dl", "DownloadedFile");
// queue
pb.Data.Mongo.TraceMongoCommand.Export("dl", "QueueDownloadFile_new", Path.Combine(AppData.DataDirectory, @"mongo\export\Download\export_QueueDownloadFile_new.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "CurrentDownloadFile", Path.Combine(AppData.DataDirectory, @"mongo\export\Download\export_CurrentDownloadFile.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "QueueDownloadFile_new");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "CurrentDownloadFile");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.QueueDownloadFile_new.drop()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.CurrentDownloadFile.drop()", "dl");

// old DownloadedFile : DownloadFile3
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadFile3", Path.Combine(AppData.DataDirectory, @"mongo\export\Download\export_DownloadFile3.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadFile3", "{}", limit: 100, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadFile.key': 1, 'downloadFile.downloadedFile': 1, 'downloadFile.state': 1, 'downloadFile.startDownloadTime': 1 }");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "DownloadFile3");
// old queue
pb.Data.Mongo.TraceMongoCommand.Export("dl", "QueueDownloadFile4", Path.Combine(AppData.DataDirectory, @"mongo\export\Download\export_QueueDownloadFile4.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "QueueDownloadFile4");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "QueueDownloadFile4", Path.Combine(AppData.DataDirectory, @"mongo\export\Download\export_QueueDownloadFile4_12.txt"));
//pb.Data.Mongo.TraceMongoCommand.Eval("db.QueueDownloadFile4.drop()", "dl");

//pb.Data.Mongo.TraceMongoCommand.Export("dl", "Download2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_Download2.txt"));
//pb.Data.Mongo.TraceMongoCommand.Export("dl", "QueueDownloadFile3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_QueueDownloadFile3.txt"));

pb.Data.Mongo.TraceMongoCommand.Export("dl", "TelechargerMagazine_Detail", Path.Combine(AppData.DataDirectory, @"mongo\export\telecharger-magazine.com\export_TelechargerMagazine_Detail.txt"), sort: "{ '_id': -1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "Vosbooks_Detail", Path.Combine(AppData.DataDirectory, @"mongo\export\vosbooks.net\export_Vosbooks_Detail.txt"), sort: "{ 'download.PostCreationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "Ebookdz_Detail", Path.Combine(AppData.DataDirectory, @"mongo\export\ebookdz.com\export_Ebookdz_Detail.txt"), sort: "{ 'download.PostCreationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", Path.Combine(AppData.DataDirectory, @"mongo\export\rapide-ddl.com\export_RapideDdl_Detail2.txt"), sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "GoldenDdl_Detail", Path.Combine(AppData.DataDirectory, @"mongo\export\golden-ddl.net\export_GoldenDdl_Detail.txt"), sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "GoldenDdl_Detail", Path.Combine(AppData.DataDirectory, @"mongo\export\golden-ddl.net\export_GoldenDdl_Detail.txt"), sort: "{ '_id': -1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "GoldenDdl_Detail2", Path.Combine(AppData.DataDirectory, @"mongo\export\golden-ddl.net\export_GoldenDdl_Detail2.txt"), sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "Telechargementz_Detail", Path.Combine(AppData.DataDirectory, @"mongo\export\telechargementz.tv\export_Telechargementz_Detail.txt"), sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "ExtremeDown_Detail", Path.Combine(AppData.DataDirectory, @"mongo\export\extreme-down.net\export_ExtremeDown_Detail.txt"), sort: "{ 'download.creationDate': -1 }");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.DownloadFile3.drop()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.Telechargementz_Detail.drop()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.DownloadedFile3.drop()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Import("dl", "RapideDdl_Detail3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail2.txt"));

// Automate : MongoDownloadAutomate = DownloadAutomate3, MongoDownloadFile = DownloadFile3, MongoDownloadedFile = DownloadedFile,
//   MongoQueueDownloadFile = QueueDownloadFile4, MongoQueueDownloadFile_new = QueueDownloadFile_new, MongoCurrentDownloadFile = CurrentDownloadFile

//*************************************************************************************************************************
//****                                   Vosbooks
//*************************************************************************************************************************

// load new post
Download.Print.Vosbooks.Vosbooks_DetailManager.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 100, startPage: 1, maxPage: 1000);
// refresh documents
Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);
Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager.RefreshDocumentsStore(limit: 10, reloadFromWeb: false, loadImage: false);
Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false, query: "{ _id: 74299 }");
// view last post from mongo
//Download.Print.Download_Exe.ViewDocuments(Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager, limit: 0, loadImage: false, sort: "{ 'download.PostCreationDate': -1 }");
Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager.FindDocuments(sort: "{ 'download.PostCreationDate': -1 }", limit: 100, loadImage: false).Select(data => CompactPost.Create(data as IPost)).zView();
// load headers
RunSource.CurrentRunSource.View(Download.Print.Vosbooks.Vosbooks_HeaderManager.HeaderWebDataPageManager.LoadPages(startPage: 1, maxPage: 2, reload: false, loadImage: false, refreshDocumentStore: false));
// export Vosbooks_Detail
TraceMongoCommand.Export("dl", "Vosbooks_Detail", Path.Combine(AppData.DataDirectory, @"mongo\export\vosbooks.net\export_Vosbooks_Detail.txt"), sort: "{ 'download.PostCreationDate': -1 }");
// try download
DownloadManager<DownloadPostKey>.Trace = true;

DownloadAutomate_f.Test_TryDownload_02(Vosbooks.Vosbooks_DetailManager.DetailWebDataManager, "{ _id: 94512 }", uncompressFile: true,
  forceDownloadAgain: false, forceSelect: false, simulateDownload: false, useNewDownloadManager: true, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager, "{ _id: { $gt: 78178 } }", downloadDirectory: "vosbooks.net", uncompressFile: true,
  forceDownloadAgain: true, forceSelect: false, simulateDownload: false, useNewDownloadManager: true, useTestManager: true);
Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager, "{ _id: { $gt: 78178 } }", downloadDirectory: "vosbooks.net", uncompressFile: true,
  forceDownloadAgain: true, forceSelect: false, simulateDownload: true, useNewDownloadManager: true);
DebriderAlldebrid.Trace = true;
Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager, "{ _id: 77302 }", downloadDirectory: null, uncompressFile: true, forceDownloadAgain: true, forceSelect: false, simulateDownload: false, useNewDownloadManager: false);
DebriderAlldebrid.Trace = true;
Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager, "{ _id: 76541 }", downloadDirectory: null, uncompressFile: true, forceDownloadAgain: false, forceSelect: false, simulateDownload: false, useNewDownloadManager: false);
//downloadDirectory: "vosbooks.net"
76541

//*************************************************************************************************************************
//****                                   Ebookdz
//*************************************************************************************************************************

// load new post
Download.Print.Ebookdz.Ebookdz_LoadPostDetail.CurrentLoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 10, startPage: 1, maxPage: 1);
// refresh documents
Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);
Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager.RefreshDocumentsStore(limit: 0, reloadFromWeb: true, loadImage: false);
Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager.RefreshDocumentsStore(limit: 20, reloadFromWeb: false, loadImage: false);
Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false, query: "{ _id: 113291 }");
Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager.RefreshDocumentsStore(limit: 0, reloadFromWeb: true, loadImage: false, query: "{ _id: 111510 }");
// view last post from mongo
Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager.FindDocuments(sort: "{ 'download.PostCreationDate': -1 }", limit: 100, loadImage: false).Select(data => CompactPost.Create(data as IPost)).zView();
//2015-03-30 10:29:42.865992 FindAs : localhost:27017.dl.Ebookdz_Detail query { "download.PostCreationDate" : { "$gt" : ISODate("2015-03-28T21:00:00Z") } } sort { "download.PostCreationDate" : -1 }
//2015-03-30 10:46:39.644149 FindAs : localhost:27017.dl.Ebookdz_Detail query { } sort { "download.PostCreationDate" : -1 } limit 100
pb.Data.Mongo.TraceMongoCommand.Find("dl", "Ebookdz_Detail", "{}", sort: "{ 'download.PostCreationDate' : -1 }", limit: 100).zView();
pb.Data.Mongo.TraceMongoCommand.Find("dl", "Ebookdz_Detail", "{ 'download.PostCreationDate' : { '$gt' : ISODate('2015-03-28T21:00:00Z') } }", sort: "{ 'download.PostCreationDate' : -1 }", limit: 100).zView();


// export data
pb.Data.Mongo.TraceMongoCommand.Export("dl", "Ebookdz_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\ebookdz.com\export_Ebookdz_Detail.txt"), sort: "{ 'download.PostCreationDate': -1 }");
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz_LoadHeaderPagesManager.CurrentLoadHeaderPagesManager.LoadPages(startPage: 1, maxPage: 1, reload: false, loadImage: false, refreshDocumentStore: false));
// try download
Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager, "{ _id: 130981 }", uncompressFile: true,
  forceDownloadAgain: false, forceSelect: false, simulateDownload: false, useNewDownloadManager: true, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager, "{ _id: 117468 }", downloadDirectory: "ebookdz.com", uncompressFile: true,
  forceDownloadAgain: false, forceSelect: false, simulateDownload: false, useNewDownloadManager: true, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager, "ebookdz.com", "{ _id: 113744 }", uncompressFile: true, forceDownloadAgain: true, forceSelect: false, simulateDownload: false, useNewDownloadManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager, "ebookdz.com", "{ _id: 113291 }", uncompressFile: true, forceDownloadAgain: false, forceSelect: false, simulateDownload: false, useNewDownloadManager: false);
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz_LoadForumFromWebManager.CurrentLoadForumFromWebManager.LoadForum());
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz.HeaderWebDataPageManager.LoadPages(startPage: 1, maxPage: 1, reload: true, loadImage: false, refreshDocumentStore: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz.WebHeaderDetailManager.LoadDetails(startPage: 1, maxPage: 1, reloadHeaderPage: false, reloadDetail: false, loadImage: false, refreshDocumentStore: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.old.Ebookdz_LoadPostDetail.CurrentLoadPostDetail.LoadDetailItemList());

//*************************************************************************************************************************
//****                                   Telechargementz
//*************************************************************************************************************************

// load new post
Download.Print.Telechargementz.Telechargementz_LoadPostDetail.CurrentLoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 10, startPage: 1, maxPage: 2);
// refresh documents
Download.Print.Telechargementz.Telechargementz_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);
// view last post from mongo
Download.Print.Download_Exe.Test_LoadDetailItemList_01(Download.Print.Telechargementz.Telechargementz_LoadPostDetail.CurrentLoadPostDetail, limit: 20, loadImage: false);

//*************************************************************************************************************************
//****                                   ExtremeDown
//*************************************************************************************************************************

// load new post
Download.Print.ExtremeDown.ExtremeDown_LoadPostDetail.CurrentLoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 10, startPage: 1, maxPage: 2);
Download.Print.ExtremeDown.ExtremeDown_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);
// view last post from mongo
Download.Print.Download_Exe.Test_LoadDetailItemList_01(Download.Print.ExtremeDown.ExtremeDown_LoadPostDetail.CurrentLoadPostDetail, limit: 20, loadImage: false);
//Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeDown_LoadDetailItemList_01(limit: 20, loadImage: false);

//*************************************************************************************************************************
//****                                   GoldenDdl
//*************************************************************************************************************************

// load new post
//Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 7, startPage: 1, maxPage: 20);
//Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 7, startPage: 1, maxPage: 2);
//Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 0, startPage: 1, maxPage: 100);
Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.CurrentLoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 7, startPage: 1, maxPage: 20);

// refresh documents
//Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);
//Download.Print.RapideDdl.RapideDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);
Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);
Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false, query: "{ 'download.loadFromWebDate': { $lt: ISODate('2014-12-27T00:00:00.000Z') } }");
Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 20, reloadFromWeb: false, loadImage: false, query: "{ 'download.loadFromWebDate': { $lt: ISODate('2014-12-27T00:00:00.000Z') } }");
Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 100, reloadFromWeb: false, loadImage: false);
Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 20, reloadFromWeb: false, loadImage: false, query: "{ '_id': 6561 }");
Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 20, reloadFromWeb: false, loadImage: false, query: "{ '_id': 54406 }");

// view last post from mongo
Download.Print.Download_Exe.Test_LoadDetailItemList_01(Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.CurrentLoadPostDetail, limit: 20, loadImage: false);
//Download.Print.GoldenDdl.GoldenDdl_Exe.Test_GoldenDdl_LoadDetailItemList_01(limit: 20, loadImage: false);
//Download.Print.GoldenDdl.GoldenDdl_Exe.Test_GoldenDdl_LoadDetailItemList_02(limit: 10, loadImage: true);
//Download.Print.GoldenDdl.GoldenDdl_Exe.Test_GoldenDdl_LoadDetailItemList_01(limit: 100, loadImage: true);
//Download.Print.GoldenDdl.GoldenDdl_Exe.Test_GoldenDdl_LoadDetailItemList_01(limit: 100, loadImage: false);
// 52102 - 1000 BLAGUES N?14 - (Envie de rire ?)
//Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.RefreshDocumentsStore(query: "{ _id: 52102 }", limit: 0, reloadFromWeb: false, loadImage: false);
//Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.RefreshDocumentsStore(query: "{ _id: 52102 }", limit: 0, reloadFromWeb: false, loadImage: true);


//*************************************************************************************************************************
//****                                   GoldenDdl mongo
//*************************************************************************************************************************

// GoldenDdl_Detail count    ??? creationDate 2014-08-14 to 2014-09-20  (le 20/09/2014)
pb.Data.Mongo.TraceMongoCommand.Count("dl", "GoldenDdl_Detail");
pb.Data.Mongo.TraceMongoCommand.Eval("db.GoldenDdl_Detail.aggregate( [ { $group: { _id: null, minDate: { $min: '$download.creationDate' }, maxDate: { $max: '$download.creationDate' }, count: { $sum: 1 } } } ] )", "dl");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "GoldenDdl_Detail", "{}", limit: 100, sort: "{ 'download.creationDate': -1 }", fields: "{ '_id': 1, 'download.loadFromWebDate': 1, 'download.creationDate': 1, 'download.title': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "GoldenDdl_Detail", "{}", limit: 0, sort: "{ 'download.creationDate': -1 }", fields: "{ '_id': 1, 'download.loadFromWebDate': 1, 'download.creationDate': 1, 'download.title': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");
// export GoldenDdl_Detail
pb.Data.Mongo.TraceMongoCommand.Export("dl", "GoldenDdl_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\golden-ddl\export_GoldenDdl_Detail.txt"), sort: "{ 'download.creationDate': -1 }");

//*************************************************************************************************************************
//****                                   RapideDdl
//*************************************************************************************************************************

// load new post
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 21, startPage: 1, maxPage: 200);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 7, startPage: 1, maxPage: 20);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 7, startPage: 1, maxPage: 2);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 20);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: true);
//Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: true);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 1, reloadFromWeb: false, loadImage: false);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 10, reloadFromWeb: false, loadImage: false);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 100, reloadFromWeb: false, loadImage: false);
// view last post from mongo
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 100, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 100, loadImage: false);


//*************************************************************************************************************************
//****                                   RapideDdl mongo
//*************************************************************************************************************************

// RapideDdl_Detail2 count    935 creationDate 2014-08-14 to 2014-09-20  (le 20/09/2014)
// RapideDdl_Detail2 count  14806 creationDate 2012-08-10 to 2014-09-23  (le 20/09/2014)
pb.Data.Mongo.TraceMongoCommand.Count("dl", "RapideDdl_Detail2");
pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail2.aggregate( [ { $group: { _id: null, minDate: { $min: '$download.creationDate' }, maxDate: { $max: '$download.creationDate' }, count: { $sum: 1 } } } ] )", "dl");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{}", limit: 1000, sort: "{ 'download.creationDate': -1 }", fields: "{ '_id': 1, 'download.loadFromWebDate': 1, 'download.creationDate': 1, 'download.title': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");
// export RapideDdl_Detail2
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail2.txt"), sort: "{ 'download.creationDate': -1 }");


//*************************************************************************************************************************
//****                                   Automate mongo
//*************************************************************************************************************************
//, sort: "{ 'downloadPost.post.creationDate': -1 }"
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadFile3", "{}", limit: 10);
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadFile3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_DownloadFile3.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "QueueDownloadFile3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_QueueDownloadFile3.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "QueueDownloadFile4", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_QueueDownloadFile4.txt"));
pb.Data.Mongo.TraceMongoCommand.Count("dl", "QueueDownloadFile4");
// bug
//pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadFile3", "{ 'downloadFile.file': /^golden-ddl.net\\/ }", limit: 10);
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadFile3", "{ 'downloadFile.file': /^golden-ddl.net/ }", limit: 10);
pb.Data.Mongo.TraceMongoCommand.Update("dl", "DownloadFile3", "{ 'downloadFile.file': /^golden-ddl.net/ }", "{ $set: { 'downloadFile.key.server': 'golden-ddl.net' } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);


//*************************************************************************************************************************
//****                                   Automate
//*************************************************************************************************************************

Download.Print.DownloadAutomate_f.Test_DownloadAutomate_01(loadNewPost: true, searchPostToDownload: true, uncompressFile: true, sendMail: false, version: 3, traceLevel: 0);
Download.Print.DownloadAutomate_f.Test_DownloadAutomate_01(loadNewPost: true, searchPostToDownload: false, uncompressFile: true, sendMail: false, version: 3, traceLevel: 0);

Trace.WriteLine(Download.Print.DownloadAutomate_f.GetMongoDownloadAutomateManager().GetLastRunDateTime().ToString());
Trace.WriteLine(Download.Print.DownloadAutomate_f.GetMongoDownloadAutomateManager().GetNextRunDateTime().ToString());
Download.Print.DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(DateTime.Parse("2014-07-01 00:00:00"));
Download.Print.DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(DateTime.Parse("2014-09-15 20:00:00"));
Download.Print.DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(new DateTime(2014, 9, 15, 20, 0, 0));
Download.Print.DownloadAutomate_f.GetMongoDownloadAutomateManager().SetTimeBetweenRun(TimeSpan.FromHours(1));
Download.Print.DownloadAutomate_f.GetMongoDownloadAutomateManager().SetTimeBetweenRun(TimeSpan.FromMinutes(30));
Test.Test_iTextSharp.Test_iTextSharp_f2.Test_ControlPdfDirectory_01(@"c:\pib\_dl\_pib\dl");
Test.Test_iTextSharp.Test_iTextSharp_f2.Test_ControlPdfDirectory_01(@"c:\pib\_dl\_pib\dl_pierre2");

pb.Data.Mongo.TraceMongoCommand.Find("dl", "GoldenDdl_Detail", "{ 'download.title': /guide du pirate/i }", limit: 100, sort: "{ 'download.creationDate': -1 }", fields: "{ '_id': 1, 'download.loadFromWebDate': 1, 'download.creationDate': 1, 'download.title': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");
Download.Print.DownloadAutomate_f.Test_TryDownload_GoldenDdlPost_01("{ _id: 54533 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
Download.Print.DownloadAutomate_f.Test_TryDownload_GoldenDdlPost_01("{ _id: 54533 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: false, uncompressFile: true);

//pb.Data.Mongo.TraceMongoCommand.Eval("db.QueueDownloadFile4.drop()", "dl");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ 'download.title': /journaux/i } }");
// LES JOURNAUX -  MERCREDI 19 / 20 NOVEMBRE 2014
Download.Print.DownloadAutomate_f.Test_TryDownload_RapideDdlPost_01("{ _id: 42091 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
Download.Print.GoldenDdl.GoldenDdl_Exe.Test_GoldenDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ 'download.title': /journaux/i } }");
// 52763 - LES JOURNAUX -  MERCREDI 19 / 20 NOVEMBRE 2014
Download.Print.DownloadAutomate_f.Test_TryDownload_GoldenDdlPost_01("{ _id: 52763 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);

// Pirate Informatique No.22 - Juillet/Septembre 2014 - 35149
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 35149 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// Pirate Informatique N?21 - Avril/Juin 2014 - 31397
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 31397 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// Pirate informatique - 2014-01 - no 20.pdf
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 27796 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// Pirate Informatique N 19 - Octobre-Novembre-D?cembre 2013 - 22280
// Pirate Informatique N?18 - Ao?t-Septembre-Octobre 2013 - 19083
// Pirate Informatique 1 ? 17 - 31446
// Pirate Informatique Collection 2012 - 31440
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ $or: [ { _id: 22280 }, { _id: 19083 }, { _id: 31446 }, { _id: 31440 } ] }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// Pirate Informatique
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ 'download.title': /pirate.*informatique/i } }");
// Val?rie Trierweiler - Merci pour ce moment
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 38672 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// Un kilo de culture g?n?rale
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 40331 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// Un empoisonnement universel : Comment les produits chimiques ont envahi la plan?te
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 40240 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// Israel, Parlons-en!
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 40164 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// Assimilation - La fin du mod?le fran?ais - Mich?le Tribalat
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 39278 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// La theorie du chaos - Leonard Rosen - Epub
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 39775 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// Ken Follett - Le Si?cle - 3 Volumes (s?rie compl?te)
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 39666 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
//Bouvard de A ? Z
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 39572 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// Le suicide fran?ais - Eric ZEMMOUR
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 40152 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// L'univers ?l?gant - Brian Greene
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 40308 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
// "Chroniques des atomes et des galaxies - Hubert Reeves" - "http://www.rapide-ddl.com/ebooks/livres/40166-chroniques-des-atomes-et-des-galaxies-hubert-reeves.html"
// error uncompress file "c:\pib\_dl\_pib\dm\..\dl\book\Chroniques des atomes et des galaxies - hubert reeves\Chroniques des atomes et des galaxies - hubert reeves.zip"
// Could not find a part of the path 'c:\pib\_dl\_pib\dl\book\Chroniques des atomes et des galaxies - hubert reeves\Chroniques des atomes et des galaxies - hubert reeves\Chroniques des atomes et des galaxies - Hubert Reeves\'.
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 40166 }", version: 3, simulateDownload: false, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 40166 }", version: 3, simulateDownload: true, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ _id: 40166 } }");
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ 'download.title': /.*air.*cosmos.*/i }", version: 3);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ 'download.title': 39071 } }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ 'download.title': /express/i }", limit: 10, sort: "{ 'download.creationDate': -1 }", fields: "{ '_id': 1, 'download.loadFromWebDate': 1, 'download.creationDate': 1, 'download.title': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");
// 40925 - L'Express N?3304 - 29 Octobre au 4 Novembre 2014
// 40703 - L'Express Hors-S?rie No.36 - Hiver 2015
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 40925 }", version: 3, simulateDownload: true, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ _id: 39572 }", version: 3, simulateDownload: true, forceSelect: true, forceDownloadAgain: true, uncompressFile: true);
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ 'download.title': /avenir/i }", limit: 100, sort: "{ 'download.creationDate': -1 }", fields: "{ '_id': 1, 'download.loadFromWebDate': 1, 'download.creationDate': 1, 'download.title': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");


//*************************************************************************************************************************
//****                                   Automate mongo
//*************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Eval("{ listDatabases: 1 }");
// ["Download", "Download2", "DownloadAutomate", "DownloadAutomate2", "DownloadAutomate3", "DownloadFile3", "DownloadedFile3", "IdGenerator", "Images", "QueueDownloadFile3", "RapideDdl_Detail", "RapideDdl_Detail2", "TelechargementPlusDetail", "TelechargementPlus_old", "[object Object]", "system.indexes"]
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.IdGenerator.drop()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.DownloadFile3.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "IdGenerator", "{}");
pb.Data.Mongo.TraceMongoCommand.FindAll("dl", "IdGenerator");
pb.Data.Mongo.TraceMongoCommand.Update("dl", "IdGenerator", "{ collection: 'Download2' }", "{ $set: { lastId: 49 } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
pb.Data.Mongo.TraceMongoCommand.Eval("db.IdGenerator.count()", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "IdGenerator", "{}");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "DownloadAutomate", "{}");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "DownloadAutomate2", "{}");
pb.Data.Mongo.TraceMongoCommand.Eval("db.DownloadAutomate.count()", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Download", "{}", limit: 5);
pb.Data.Mongo.TraceMongoCommand.Find("dl", "Download", "{}", limit: 5, sort: "{ 'downloadPost.post.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "Download", "{ 'downloadPost.file': 'l_express\\\\l_express_autre.pdf' }", limit: 5, sort: "{ 'downloadPost.post.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Download2", "{}", limit: 5, sort: "{ 'downloadPost.post.creationDate': -1 }", fields: "{ 'downloadPost.post.loadFromWebDate': 1, 'downloadPost.post.creationDate': 1, 'downloadPost.post.title': 1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Download2", "{}", limit: 15, sort: "{ '_id': 1 }", fields: "{ 'downloadPost.post.loadFromWebDate': 1, 'downloadPost.post.creationDate': 1, 'downloadPost.post.title': 1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Download2", "{}", limit: 15, sort: "{ '_id': -1 }", fields: "{ 'downloadPost.post.loadFromWebDate': 1, 'downloadPost.post.creationDate': 1, 'downloadPost.post.title': 1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("db.Download.count()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.Download.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadAutomate2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\DownloadAutomate\export_DownloadAutomate2.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadAutomate3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\DownloadAutomate\export_DownloadAutomate3.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "Download2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_Download2.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadFile3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_DownloadFile3.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadedFile3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_DownloadedFile3.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "QueueDownloadFile3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_QueueDownloadFile3.txt"));
//pb.Data.Mongo.TraceMongoCommand.Eval("db.QueueDownloadFile4.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "QueueDownloadFile4");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "QueueDownloadFile4", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_QueueDownloadFile4.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "QueueDownloadFile4", "{}", limit: 5);
pb.Data.Mongo.TraceMongoCommand.Find("dl", "QueueDownloadFile4", "{ 'downloadFile.key.server': 'golden-ddl.net', 'downloadFile.key._id': 52910 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail2.txt"));
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadFile3", "{ 'downloadFile.key.server' : 'rapide-ddl.com', 'downloadFile.key._id' : 39207 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadedFile3", "{ 'downloadFile.key.server' : 'rapide-ddl.com', 'downloadFile.key._id' : 39207 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadedFile3", "{ _id: 4 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadedFile3", "{}", limit: 5);



pb.Data.Mongo.TraceMongoCommand.Update("dl", "Download2", "{}", "{ $unset: { 'downloadPost.post.OriginalTitle': 1 } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);

Trace.WriteLine("{0}", Download.Print.DownloadAutomate_f.GetDate("LES JOURNAUX - VENDREDI 15 / 16 AOUT 2014 & + [PDF][Lien Direct]")); 
Trace.WriteLine("{0}", Download.Print.DownloadAutomate_f.GetDate("L'Equipe du vendredi 15 ao?t 2014")); 
Trace.WriteLine("{0}", Download.Print.DownloadAutomate_f.GetDate("Marianne N?904 - 15 au 21 Aout 2014")); 
Trace.WriteLine("{0}", Download.Print.DownloadAutomate_f.GetDate("Le Point N? 2187 - Du 14 au 20 Ao?t 2014")); 

//*************************************************************************************************************************
//****                                   RapideDdl
//*************************************************************************************************************************

XNodeDescendants.Trace = false;
XNodeDescendants.Trace = true;

// load new post
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 7, startPage: 1, maxPage: 20);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 20);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 1, reloadFromWeb: false, loadImage: false);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 10, reloadFromWeb: false, loadImage: false);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 100, reloadFromWeb: false, loadImage: false);
// view last post from mongo
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 1, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 100, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 1000, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ 'download.loadFromWebDate': { $lt: ISODate('2014-09-14') } }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ _id: 39071 } }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 100, loadImage: true, query: "{ 'download.title': \"LES JOURNAUX -  JEUDI 28 / 29 AOUT 2014 & + [PDF][Lien Direct]\" }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 100, loadImage: true, query: "{ 'download.title': 'LES JOURNAUX -  MARDI 16 SEPTEMBRE 2014' }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: true, query: "{ 'download.title': /les journaux .*/i }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 100, loadImage: true, query: "{ 'download.title': /.*les.*journaux.*/i }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 100, loadImage: true, query: "{ 'download.title': /.*air.*cosmos.*/i }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 100, loadImage: true, query: "{ 'download.title': /.*express.*/i }");
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ 'download.title': /.*air.*cosmos.*/i }");
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ 'download.title': /.*express.*/i }");
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ 'download.title': 'LES JOURNAUX -  MARDI 16 SEPTEMBRE 2014' }");
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ 'download.title': 'LES JOURNAUX -  MARDI 16 SEPTEMBRE 2014' }", false);
Download.Print.DownloadAutomate_f.Test_TryDownloadPost_01("{ 'download.title': 'France Dimanche N 3551 Du 19 au 25 Septembre 2014' }", forceDownloadAgain: false);
Trace.WriteLine(Download.Print.DownloadAutomate_f.CreateDownloadAutomate().DownloadManager.GetDownloadFileState(new Download.Print.DownloadPostKey { server = "rapide-ddl.com", id = 39350 }).ToString());

Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_02(limit: 0, loadImage: true);

// test
Download.Print.DownloadAutomate_f.Test_DownloadAutomate_SelectPost_01();
Download.Print.DownloadAutomate_f.Test_RapideDdl_SelectPost_01(query: "{}", limit: 1000, sort: "{ 'downloadPost.post.creationDate': -1 }");
Download.Print.DownloadAutomate_f.Test_RapideDdl_SelectPost_02(query: "{}", limit: 1000, sort: "{ 'downloadPost.post.creationDate': -1 }");
Download.Print.DownloadAutomate_f.Test_RapideDdl_SelectPost_01(query: "{ 'download.title': /.*express.*/i }", limit: 100);
Download.Print.DownloadAutomate_f.Test_UncompressFile_01(@"c:\pib\_dl\_pib\dl\l_express\l_express_2014-07-02.rar");
Download.Print.DownloadAutomate_f.Test_UncompressFile_01(@"c:\pib\_dl\_pib\dl\l_express\l_express_Hors-S?rie Montres N? 14 - JuilletAo?t 2014(1).rar");
Download.Print.DownloadAutomate_f.Test_UncompressFile_01(@"c:\pib\_dl\_pib\dl\journaux\journaux_2014-07-28.zip", archiveCompress: false);
Download.Print.DownloadAutomate_f.Test_UncompressFile_01(@"c:\pib\_dl\_pib\dl\journaux\journaux_2014-07-28.zip", archiveCompress: true);
Download.Print.DownloadAutomate_f.Test_UncompressFile_01(@"c:\pib\_dl\_pib\dl\journaux\test\journaux_2014-07-28_test_02.zip", archiveCompress: false);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadHeaderPagesFromWeb_01();
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadPostDetail_01();
RunSource.CurrentRunSource.View(Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadDetailItem(36744));
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadPostImages_01(limit: 10, loadImage: false);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadPostImages_01(limit: 100, loadImage: false);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadPostImages_01(limit: 0, loadImage: false);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadPostLinks_01(limit: 100);

// old
Download.Print.RapideDdl.v1.RapideDdl_Exe_v1.Test_RapideDdl_LoadDetailItemList_02(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false);
Download.Print.RapideDdl.v1.RapideDdl_Exe_v1.Test_RapideDdl_LoadDetailItemList_02(startPage: 1, maxPage: 10, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false);
Download.Print.RapideDdl.v1.RapideDdl_Exe_v1.Test_RapideDdl_LoadDetailItemList_02(startPage: 1, maxPage: 10, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: true);
Download.Print.RapideDdl.v1.RapideDdl_Exe_v1.Test_RapideDdl_LoadDetailItemList_02(startPage: 1, maxPage: 25, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false);
Download.Print.RapideDdl.v1.RapideDdl_Exe_v1.Test_RapideDdl_LoadDetailItemList_01(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: false, loadImage: true);
Download.Print.RapideDdl.v1.RapideDdl_Exe_v1.Test_RapideDdl_LoadDetailItemList_01(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: true);
Download.Print.RapideDdl.v1.RapideDdl_Exe_v1.Test_RapideDdl_LoadDetailItemList_02(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: true, loadImage: true, refreshDocumentStore: false);
Download.Print.RapideDdl.v1.RapideDdl_Exe_v1.Test_RapideDdl_LoadHeaderPages_01(startPage: 1, maxPage: 3, reload: true, loadImage: true);
Download.Print.RapideDdl.v1.RapideDdl_Exe_v1.Test_RapideDdl_LoadHeaderPages_01(startPage: 1, maxPage: 3, reload: false, loadImage: false);

Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadHeaderPages_01(startPage: 1, maxPage: 2, reload: true, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadHeaderPages_01(startPage: 1, maxPage: 2, reload: false, loadImage: false);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01_old(startPage: 1, maxPage: 2, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_02_old(startPage: 1, maxPage: 2, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_02_old(startPage: 1, maxPage: 10, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false);

//*************************************************************************************************************************
//****                                   mongo pierre2
//*************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Eval("{ listDatabases: 1 }", server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl", server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "IdGenerator", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\IdGenerator\export_IdGenerator.txt"), server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadAutomate", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\DownloadAutomate\export_DownloadAutomate.txt"), server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "DownloadAutomate2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\DownloadAutomate\export_DownloadAutomate_03_Pierre2.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "Download", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_Download.txt"), server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "Download2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_Download_03_Pierre2.txt"));
pb.Data.Mongo.TraceMongoCommand.Import("dl", "Download2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_Download_03_Pierre2_bug.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail2.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail.txt"), server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "RapideDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail_03_Pierre2.txt"));
pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail.count()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail.count()", "dl", server: "mongodb://pierre2");



//*************************************************************************************************************************
//****                                   RapideDdl mongo
//*************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl");

// RapideDdl_Detail2 count 1583 creationDate 2014-06-22 to 2014-09-01  (le 20/09/2014)
pb.Data.Mongo.TraceMongoCommand.Count("dl", "RapideDdl_Detail");
pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail.aggregate( [ { $group: { _id: null, minDate: { $min: '$download.creationDate' }, maxDate: { $max: '$download.creationDate' }, count: { $sum: 1 } } } ] )", "dl");


//pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail", "{}", limit: 5, sort: "{ 'download.creationDate': -1 }");
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 2, reloadFromWeb: false);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(reloadFromWeb: false, query: "{ _id: 39025 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ _id: 39025 }", limit: 10, sort: "{ 'download.creationDate': -1 }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ _id: 39025 } }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ _id: 39071 } }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{}", limit: 10, sort: "{ 'download.creationDate': -1 }");

// RapideDdl_Detail2 query { "id" : 42083 }
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ _id: 42083 }", limit: 10, sort: "{ 'download.creationDate': -1 }");

// control creationDate
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{}", limit: 1000, sort: "{ 'download.creationDate': -1 }",
  fields: "{ 'download.loadFromWebDate': 1, 'download.creationDate': 1, 'download.category': 1, 'download.title': 1, 'download.sourceUrl': 1 }");
//http://www.rapide-ddl.com/ebooks/journaux/37309-journal-le-monde-mardi-12-aogt-2014.html
// pb date html : Aujourd'hui, 21:10, date du fichier 11-08-2014 22:51 ebooks_journaux_37309-journal-le-monde-mardi-12-aogt-2014.html
// date de la page html 11-08-2014, 21:10
// date web 19-08-2014, 00:15 date html file Aujourd'hui, 00:15  file 19/08/2014  00:31 ebooks_livres_37659-multi-un-gternel-amour-ebook.html
// http://www.rapide-ddl.com/ebooks/livres/37659-multi-un-gternel-amour-ebook.html
// http://www.rapide-ddl.com/ebooks/livres/39471-le-christianisme-occidental-au-moyen-gge-ive-xve-sicle-jacques-paul.html
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(query: "{ _id: 37309 }");
// pb category
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(query: "{ _id: 39619 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ _id: 39619 }", limit: 10, fields: "{ 'download.category': 1 }");
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(query: "{ _id: 39587 }");
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(query: "{ _id: 39662 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ _id: 39662 }", limit: 10, fields: "{ 'download.category': 1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail2.aggregate( [ { $group: { _id: null, category: 1, count: { $sum: 1 } } } ] )", "dl");



pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ _id: 37309 }", limit: 1000, sort: "{ 'download.creationDate': -1 }",
  fields: "{ 'download.loadFromWebDate': 1, 'download.creationDate': 1, 'download.category': 1, 'download.title': 1, 'download.sourceUrl': 1 }");


pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ 'download.title': \"LES JOURNAUX -  JEUDI 28 / 29 AOUT 2014 & + [PDF][Lien Direct]\" }", limit: 5, sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{}", limit: 5, sort: "{ 'download.creationDate': 1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{ _id: { $gt: 100000 } }", fields: "{ title: 1, creationDate: 1 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{}", fields: "{ 'download.title': 1, 'download.infos': 1, 'download.images.Source': 1, 'download.downloadLinks': 1 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{}", fields: "{ 'download.images': 1 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{}", fields: "{ 'download.images.Source': 1, 'download.images.ImageWidth': 1, 'download.images.ImageHeight': 1 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{}", fields: "{ 'download.creationDate': 1 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{ 'download.creationDate': { $gt: ISODate('2014-06-25') } }", sort: "{ 'download.creationDate': -1 }", fields: "{ 'download.creationDate': 1 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{ 'download.creationDate': { $gt: ISODate('2014-06-25') } }", sort: "{ 'download.creationDate': -1 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{ 'download.creationDate': { $gt: ISODate('2014-06-25') } }", sort: "{ 'download.creationDate': -1 }", fields: "{ _id: 1, 'download.title': 1, 'download.loadFromWebDate': 1, 'download.creationDate': 1, 'download.sourceUrl': 1 }", limit: 50);
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"sites\rapide-ddl\mongo\export_RapideDdl_Detail.txt"));
pb.Data.Mongo.TraceMongoCommand.Import("dl", "Test_RapideDdl_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"sites\rapide-ddl\mongo\export_RapideDdl_Detail.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "Test_RapideDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"sites\rapide-ddl\mongo\export_RapideDdl_Detail.txt"));
pb.Data.Mongo.TraceMongoCommand.RenameCollection("dl", "Test_RapideDdl_Detail2", "Test_RapideDdl_Detail3");
pb.Data.Mongo.TraceMongoCommand.Eval("db.runCommand( { count: 'Test_RapideDdl_Detail3' } )", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail3", "{}", limit: 5, sort: "{ 'download.creationDate': -1 }");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.Test_RapideDdl_Detail.drop()", "dl");





pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail.findOne()", "dl");

Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ 'download.loadFromWebDate': { $lt: ISODate('2014-09-14') } }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ _id: 39071 }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ _id: 39001 }");
// GetCurrentBsonType can only be called when State is Value, not when State is EndOfArray. (System.InvalidOperationException)
// An error occurred while deserializing the infos property of class Download.Print.RapideDdl.RapideDdl_PostDetail: GetCurrentBsonType can only be called when State is Value, not when State is EndOfArray. (System.IO.FileFormatException)
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{}", limit: 10, sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail3", "{ _id: 39025 }", limit: 10, sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail2.txt"));
//pb.Data.Mongo.TraceMongoCommand.Import("dl", "RapideDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail2_01.txt"));

pb.Data.Mongo.TraceMongoCommand.Update("dl", "RapideDdl_Detail2", "{ _id: 39025 }", "{ $unset: { 'download.infos': 1 } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
pb.Data.Mongo.TraceMongoCommand.Update("dl", "RapideDdl_Detail2", "{ _id: 37607 }", "{ $unset: { 'download.infos': 1 } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
pb.Data.Mongo.TraceMongoCommand.Update("dl", "RapideDdl_Detail2", "{}", "{ $unset: { 'download.OriginalTitle': 1 } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
//pb.Data.Mongo.TraceMongoCommand.Update("dl", "RapideDdl_Detail2", "{}", "{ $rename: { 'download.OriginalTitle': 'download.originalTitle' } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
//pb.Data.Mongo.TraceMongoCommand.Update("dl", "RapideDdl_Detail2", "{}", "{ $rename: { 'download.originalTitle': 'download2' } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
//pb.Data.Mongo.TraceMongoCommand.Update("dl", "RapideDdl_Detail2", "{}", "{ $unset: { 'download2': 1 } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
//pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail2.drop()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail3.drop()", "dl");
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(reloadFromWeb: false, query: "{ _id: 39025 }");
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, query: "{}");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ _id: 39025 }");  // erreur
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: false, query: "{ _id: 37607 }");  // erreur
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 0, loadImage: false);
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ _id: 39025 }", limit: 10, sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ _id: 37607 }", limit: 10, sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{}", limit: 23, sort: "{ 'download.creationDate': -1 }");
Test.Test_Bson.Test_Bson_f2.Test_Serialize_06(Test.Test_Bson.Test_Bson_f2.Create_RapideDdl_PostDetail_01());
Test.Test_Bson.Test_Bson_f2.Test_Serialize_07(Test.Test_Bson.Test_Bson_f2.Create_RapideDdl_PostDetail_01());
Test.Test_Bson.Test_Bson_f2.Test_Serialize_07(Test.Test_Bson.Test_Bson_f2.Create_RapideDdl_PostDetail_02());
Test.Test_Bson.Test_Bson_f2.Test_Deserialize_01(Test.Test_Bson.Test_Bson_f2.Create_RapideDdl_PostDetail_01());
Test.Test_Bson.Test_Bson_f2.Test_Deserialize_01(Test.Test_Bson.Test_Bson_f2.Create_RapideDdl_PostDetail_02());
Test.Test_Bson.Test_Bson_f2.Test_DeserializeFromFile_01(@"c:\pib\dev_data\exe\runsource\test\RapideDdl_PostDetail_01.txt", typeof(Download.Print.RapideDdl.RapideDdl_PostDetail));
Test.Test_Bson.Test_Bson_f2.Test_DeserializeFromFile_01(@"c:\pib\dev_data\exe\runsource\test\RapideDdl_PostDetail_02_01.txt", typeof(Download.Print.RapideDdl.RapideDdl_PostDetail));
Test.Test_Bson.Test_Bson_f2.Test_DeserializeFromFile_01(@"c:\pib\dev_data\exe\runsource\test\RapideDdl_PostDetail_02_02.txt", typeof(Download.Print.RapideDdl.RapideDdl_PostDetail));
Test.Test_Bson.Test_Bson_f2.Test_DeserializeFromFile_01(@"c:\pib\dev_data\exe\runsource\test\RapideDdl_PostDetail_02_03.txt", typeof(Download.Print.RapideDdl.RapideDdl_PostDetail));
Test.Test_Bson.Test_Bson_f2.Test_DeserializeFromFile_01(@"c:\pib\dev_data\exe\runsource\test\RapideDdl_PostDetail_02_04.txt", typeof(Download.Print.RapideDdl.RapideDdl_PostDetail));










//pb.Data.Mongo.TraceMongoCommand.Eval("db.Test_RapideDdl_Detail.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail.copyTo('Test_RapideDdl_Detail')", "dl");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_MongoUpdateDetailItemList_01("{}");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_MongoUpdateDetailItemList_02("{}");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail", "{}", limit: 5, sort: "{ 'download.creationDate': -1 }");
//pb.Data.Mongo.TraceMongoCommand.Update("dl", "Test_RapideDdl_Detail", "{}", "{ $unset: { 'download.images': '' } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
//pb.Data.Mongo.TraceMongoCommand.Update("dl", "Test_RapideDdl_Detail", "{}", "{ $rename: { 'download.images2': 'download.images' } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail", "{ _id: 37085 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail", "{ _id: 35195 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail", "{ _id: 35195 }", fields: "{ 'download.images.Source': 1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail", "{ _id: 35195 }", fields: "{ 'download.images': 1 }");
pb.Data.Mongo.TraceMongoCommand.Update("dl", "Test_RapideDdl_Detail", "{ _id: 35195 }", "{ $set: { images2: [] } }", MongoDB.Driver.UpdateFlags.Upsert);
pb.Data.Mongo.TraceMongoCommand.Update("dl", "Test_RapideDdl_Detail", "{ _id: 35195 }", "{ $unset: { images2: '' } }", MongoDB.Driver.UpdateFlags.Upsert);
pb.Data.Mongo.TraceMongoCommand.Update("dl", "Test_RapideDdl_Detail", "{ _id: 35195 }", "{ $push: { images2: { $each: 'download.images.Source' } } }", MongoDB.Driver.UpdateFlags.Upsert);

pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail", "{ 'download.images': { $size: 2 } }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail", "{ 'download.images': { $size: 3 } }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail", "{ 'download.images': { $size: 4 } }", limit: 5);

pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail", "{ _id: 35195 }");
pb.Data.Mongo.TraceMongoCommand.UpdateDocuments("dl", "Test_RapideDdl_Detail", "{ _id: 35195 }", doc => doc["download"]["category"] = "Ebooks/Magazine tata");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail", "{ _id: 34984 }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_MongoUpdateDetailItemList_01("{ _id: 35195 }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_MongoUpdateDetailItemList_01("{ _id: 34984 }");

//pb.Data.Mongo.TraceMongoCommand.Eval("db.Test_RapideDdl_Detail2.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.Test_RapideDdl_Detail.copyTo('Test_RapideDdl_Detail2')", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail2", "{}", limit: 5, sort: "{ 'download.creationDate': -1 }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_MongoUpdateDetailItemList_02("{}");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_MongoUpdateDetailItemList_02("{ _id: 35195 }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_MongoUpdateDetailItemList_02("{ _id: 34984 }");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_MongoUpdateDetailItemList_02("{ _id: 37213 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail2", "{ _id: 35195 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail2", "{ _id: 34984 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_RapideDdl_Detail2", "{ _id: 37213 }");



//*************************************************************************************************************************
//****                                   Images cache mongo
//*************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Images", "{}", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Images", "{}");
pb.Data.Mongo.TraceMongoCommand.FindAs<pb.Web.MongoImage>("dl", "Images", "{}");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Images", "{}", sort: "{ 'Height': 1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Images", "{ _id: 'http://zupimages.net/up/14/11/agur.gif' }", sort: "{ 'Height': 1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("db.runCommand( { count: 'Images' } )", "dl");
images_cache_f.Test_ViewImagesCache_01();

pb.Data.Mongo.TraceMongoCommand.Update("dl", "Images", "{ _id: 'http://zupimages.net/up/14/11/agur.gif' }", "{ $set: { Category: 'layout' } }", MongoDB.Driver.UpdateFlags.Upsert);

pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Images", "{ Category: { $exists: true } }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Images", "{ Category: { $exists: false } }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.Update("dl", "Images", "{ Category: { $exists: false } }", "{ $set: { Category: null } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);


pb.Data.Mongo.TraceMongoCommand.Eval("db.Images.find().limit(5)", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.Images.find().limit(5).forEach(function(doc){ db.Test_Images.insert(doc); })", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_Images", "{}");
pb.Data.Mongo.TraceMongoCommand.Eval("db.Images.find().count()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.Test_Images.find().count()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.Images.copyTo('Test_Images')", "dl");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "Images");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "Test_Images");
pb.Data.Mongo.TraceMongoCommand.Eval("db.Test_Images.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_Images", "{}", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_Images", "{ Category: { $exists: true } }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Test_Images", "{ Category: { $exists: false } }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.Update("dl", "Test_Images", "{ Category: { $exists: false } }", "{ $set: { Category: null } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);


//*************************************************************************************************************************
//****                                   test DownloadManager
//*************************************************************************************************************************

Download.Print.DownloadManager_f.Test_DownloadService_01();
Download.Print.DownloadManager_f.Test_DownloadService_Start_01();
Download.Print.DownloadManager_f.Test_DownloadService_Stop_01();
Download.Print.DownloadManager_f.Test_DownloadService_IsStarted_01();
Download.Print.DownloadManager_f.Test_DownloadService_AddDownload_01("http://s19.alldebrid.com/dl/dh0bc9040f/Les_inRocKuptibles_976-.pdf", "Les_inRocKuptibles_976-.pdf", "Les_inRocKuptibles_976-");
// L'Express N? 3293 - Du 13 au 19 Ao?t 2014    - http://uploaded.net/file/rx2k0zxa - http://s05.alldebrid.com/dl/dqqwoxbf14/L_Express_3292.pdf
Download.Print.DownloadManager_f.Test_DownloadService_AddDownload_01("http://s05.alldebrid.com/dl/dqqwoxbf14/L_Express_3292.pdf", "l_express_2014-08-13.pdf", "l_express");
Download.Print.DownloadManager_f.Test_DownloadService_GetDownloadLocalFileById_01(238);
Download.Print.DownloadManager_f.Test_DownloadService_GetDownloadIsWorkingById_01(238);
Download.Print.DownloadManager_f.Test_DownloadService_GetDownloadStateById_01(238);
Download.Print.DownloadManager_f.Test_DownloadService_GetDownloadStatusMessageById_01(238);
Download.Print.DownloadManager_f.Test_DownloadService_GetDownloadFileSizeById_01(238);
Download.Print.DownloadManager_f.Test_DownloadService_GetDownloadTransferedById_01(238);
Download.Print.DownloadManager_f.Test_DownloadService_GetDownloadRateById_01(238);
Download.Print.DownloadManager_f.Test_DownloadService_GetDownloadLeftTimeById_01(238);
Download.Print.DownloadManager_f.Test_DownloadService_GetDownloadProgressById_01(238);
Download.Print.DownloadManager_f.Test_DownloadService_RemoveDownloadById_01(int id);
Download.Print.DownloadManager_f.Test_DownloadService_StartAndTrace_01();
Download.Print.DownloadManager_f.TraceCurrentDownload();
Download.Print.DownloadManager_f.Test_DownloadService_RemoveCompleted_01();



//*************************************************************************************************************************
//****                                   TelechargementPlus
//*************************************************************************************************************************
Test_TelechargementPlus_LoadDetailItemList_02(startPage: 1, maxPage: 20, reloadHeaderPage: true, reloadDetail: false, loadImage: true);
Test_TelechargementPlus_LoadDetailItemList_02(startPage: 1, maxPage: 50, reloadHeaderPage: true, reloadDetail: false, loadImage: true);
Test_TelechargementPlus_LoadDetailItemList_02(startPage: 1, maxPage: 1, reloadHeaderPage: false, reloadDetail: false, loadImage: true);
Test_TelechargementPlus_LoadDetailItemList_03(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: false, loadImage: false);
Test_TelechargementPlus_LoadDetailItemList_01(startPage: 1, maxPage: 5, reloadHeaderPage: true, reloadDetail: false, loadImage: true);
Test_TelechargementPlus_LoadDetail_01("http://www.telechargement-plus.com/e-book-magazines/magazines/135738-photographie-facile-magazine-n19.html", reload: false, loadImage: true);
Test_TelechargementPlus_LoadHeaderPages_01(startPage: 1, maxPage: 2, reload: true, loadImage: true);
Test_TelechargementPlus_LoadHeaderPages_01(startPage: 1, maxPage: 5, reload: true, loadImage: true);
Test_TelechargementPlus_LoadHeaderPages2_old_01(startPage: 1, maxPage: 5, reload: true, loadImage: true);
Test_TelechargementPlus_LoadHeaderPages_01_old("http://www.telechargement-plus.com/e-book-magazines/", reload: true, loadImage: true);
Test_TelechargementPlus_LoadDetailItemList_01_old(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: false, loadImage: true);
Test_TelechargementPlus_LoadDetailItemList_02_old(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: false, loadImage: true);
Test_TelechargementPlus_LoadDetailItemList_02_old(startPage: 1, maxPage: 20, reloadHeaderPage: true, reloadDetail: false, loadImage: true);
Test_TelechargementPlus_LoadHeaderPages_01(startPage: 1, maxPage: 1, reload: false, loadImage: true);
Test_TelechargementPlus_LoadHeaderPages_01(startPage: 1, maxPage: 5, reload: true, loadImage: true);
Test_TelechargementPlus_LoadHeaderPages_01(startPage: 1, maxPage: 50, reload: false, loadImage: true);

Test_telechargement_plus_loadPostFromWeb_01(reload: true, loadImage: true);
Test_telechargement_plus_loadPostFromWeb_02(reload: false, startPage: 1, maxPage: 2, loadImage: true);
Test_telechargement_plus_loadPostFromWeb_02(reload: false, startPage: 1, maxPage: 200, loadImage: true);
Test_telechargement_plus_loadPostFromWeb_03(loadImage: true);
Test_telechargement_plus_loadPostFromWeb_04(minTime: "2013-11-07 14:00");
Test_telechargement_plus_loadPostFromWeb_05(minTime: "2013-11-07 14:00", reload: false, loadImage: true);
Test_telechargement_plus_loadPostFromWeb_05(minTime: "2013-11-07 14:00", reload: true, loadImage: true);
Test_telechargement_plus_loadPostFromWeb_05(minTime: null, reload: true, loadImage: true);

Test_DownloadService_Start_01();
Test_DownloadService_Stop_01();
Test_DownloadService_IsStarted_01();
Test_DownloadService_AddDownload_01();
Test_DownloadService_RemoveCompleted_01();
Test_DownloadService_RemoveDownloadById_01(5);
Test_AllDebrid_01();
Test_AllDebrid_03();

Test_telechargement_plus_loadPostHeader_01(startPage:1,  maxPage: 1, loadImage: true);
Test_telechargement_plus_loadPostHeader_01(startPage:2,  maxPage: 1, loadImage: true);
Test_telechargement_plus_loadPostHeader_01(startPage:3,  maxPage: 1, loadImage: true);

Test_zone_ebooks_loadPostHeader_01(maxPage: 1, loadImage: true);
Test_zone_ebooks_loadPostHeader_01(startPage:1,  maxPage: 11, loadImage: true);
Test_zone_ebooks_loadPostHeader_01(startPage:10, maxPage: 11, loadImage: true);
Test_zone_ebooks_loadPostHeader_01(startPage:20, maxPage: 11, loadImage: true);
Test_zone_ebooks_loadPostHeader_01(startPage:30, maxPage: 11, loadImage: true);

Test_magazine3k_loadPostHeader_01(maxPage: 1, loadImage: true);

Test_pdf4fr_loadPostHeader_01(maxPage: 1, loadImage: true);
Test_pdf4fr_loadPostHeader_01(maxPage: 10, loadImage: true);

Test_frboard_updatePost_01(maxPage:100);
Test_frboard_loadPostHeaderFromWeb_02(maxPage:3);
Test_frboard_loadPost_02(loadImage:true);
Test_frboard_loadPost_02(loadImage:false);
Test_frboard_loadPrint_02(maxPage: 1, loadImage: true);
Test_frboard_loadPostFromWeb_02(loadImage:true);
Test_frboard_savePostToXml_02(saveImage:false);
Test_frboard_savePostToXml_02(saveImage:true);
Test_frboard_loadPostFromXml_02(loadImage:false);
Test_frboard_loadPostFromXml_02(loadImage:true);


Test_frboard_01();
Test_frboard_02();
Test_frboard_03();
Test_frboard_04();
Test_FrboardPost_01();
Test_frboard_loadPrint_01();
Test_frboard_loadPrint_02();
Test_frboard_loadPrint_02(loadImage:true);
Test_frboard_loadPost_01(loadImage:true);
Test_frboard_savePostToXml_01();
Test_frboard_loadPostFromXml_01();
Test_frboard_search_01();
Test_frboard_search_01(maxPage:3);
Test_frboard_search_01(maxPage:2, detail:true, loadImage:true);
Test_frboard_search_post_01();
Test_frboard_search_post_01(maxPage:3);
Test_frboard_search_print_01(maxPage:1, loadImage:true);

Test_XmlFilesXPathEvaluate_01("/post/print/image/@source");
Test_XmlFilesXPathEvaluate_01("print/image/@source");
Test_Download_Url_01();
Test_Download_Image_01();
Test_Download_Image_02();
Test_Download_Image_03();
Test_Download_Image_04();
Test_Download_Image_04(removeUserAgent: false);
Test_Save_Image_01();
Test_Save_Image_02();
Test_Save_Image_03();
Test_Save_Image_04();
Test_Save_Image_05();
Test_Load_Image_01();
Test_Load_Image_02();
Test.Test_Http.Test_Uri.Test();
Test_ZValue_01();
Test_XmlReader_01();
Test_XmlReader_02();
Test_AllDebrid_01();
Test_AllDebrid_02();

Test_Download_01();
Test_Download_02();



//*************************************************************************************************************************
//****                                   print
//*************************************************************************************************************************

PrintManager.Trace = false;
PrintManager.Trace = true;
_tr.WriteLine("{0}", PrintManager.Trace);

Test_PrintManager_05();
Control_PrintNumber_01();

Test_Print_RenameFile_01(simulate : true,  moveFile : false);
Test_Print_RenameFile_01(simulate : false, moveFile : false);
Test_Print_RenameFile_01(simulate : false, moveFile : true);

AssemblyResolve.Trace = true;
Compiler.TraceLevel = 2;

Test_GetNewIndexedDirectory_01();
Test_Regex_01();
Test_utf8_01();
Test_TextValues_01();
Test_SpecialDays_01();
Test_Attribute_01();
Test_telecharger_pdf_search_02();
Test_telecharger_pdf_search_01();
Test_WebRequest_01();
Test_http_01();
Test_AllDebrid_01();
Test_01();
Test_AddTraceFile_01();
Test_ViewResult_01();
Test_ReadFileLines_01();
Print.Tunit.Tunit_print_01.Test_LeMonde_01();
Test_RegexValues_01();
Test_PrintManager_01();
Test_PrintManager_02();
Test_PrintManager_03();
Test_PrintManager_04();
Test_PrintManager_05();
Test_PrintManager_06();
Test_LeParisien_01();
Control_RegexValues_01();
Control_RegexValues_02();
Control_LaCroixNumber_01();
Control_LaCroixNumber_02();
Control_LeFigaroNumber_01();
Control_LeMondeNumber_01();
Test_LeMonde_filename_01();
Test_LeMonde_filename_02();
Test_JScript_01();
Test_JScript_02();
Test_Base64_01();
Test_magazine3k_search_01();
Test_magazine3k_search_02();
Test_magazine3k_search_03();
Test_magazine3k_search_03(true);
Test_magazine3k_print_01();
Test_magazine3k_print_02();
Test_magazine3k_print_03();

wr.Select("//div/@class");
wr.Select("//div[@class]");
wr.Select("//div[@class='res']");

wr.LoadXml(@"c:\pib\dev_data\exe\wrun\test\test\test_01.xml");
wr.Select("//div");
wr.Select("//div[@class='res']:EmptyRow");
// ".//a/@title"
wr.Select("//div[@class='res']", "@class", ".//a/@href:n(href)", ".//a//text():n(label1)",
  ".//span[1]//text():n(info1)", ".//span[2]//text():n(info2)", ".//span[3]//text():n(info3)", ".//span[4]//text():n(info4)",
  ".//img/@src:n(img)", ".//div[@class='justi']//text():n(label2):Concat()", ".//div[@class='cat']/text():n(category)");

wr.Load("http://magazine3k.com/magazine/other/31496/lexpress-lexpress-styles-3180-13-au-19-juin-2012.html");
wr.Load("http://magazine3k.com/magazine/other/31496/lexpress-lexpress-styles-3180-13-au-19-juin-2012.html");
wr.Select("//div[@class='headline']:.:EmptyRow", ".//text()");
wr.Select("//div[@class='res_data']:.:EmptyRow", ".//span[1]//text():.:n(info1)", ".//span[2]//text():.:n(info2)", ".//span[3]//text():.:n(info3)", ".//span[4]//text():.:n(info4)");
wr.Select("//div[@class='res_image']:.:EmptyRow", ".//img/@src");
wr.Select("//div[@class='justi']:.:EmptyRow", ".//div/text()", ".//div/following-sibling::text()");
wr.Select("//div[@class='download_top']/following-sibling::div", "./script/text()");
wr.Select("//div[@class='res_data']//text():.:EmptyRow");

// page suivante
hxr.ReadSelect("//a[@class='pBtnSelected']/following-sibling::a/@href:.:EmptyRow");
hxr.ReadSelect("//a[@class='pBtnSelected']/following-sibling::a:.:EmptyRow");
wr.Print(hxr.SelectValue("//a[@class='pBtnSelected']/following-sibling::a/@href:.:EmptyRow"));
hxr.Load(hxr.SelectValue("//a[@class='pBtnSelected']/following-sibling::a/@href:.:EmptyRow"));



_hxr.Load("http://magazine3k.com/search/?q=l%27express&as_occt=title&as_ordb=relevance&sitesearch=magazine3k.com");
_hxr.ReadSelect("//a", ".//text()", "./@href");

_hxr.Load("http://telecharger-pdf.com/");
_hxr.ReadSelect("//article:.:EmptyRow", ".//a//text()", ".//time/@datetime", ".//span[@class='by-author']//a//text()",
  ".//div[@class='entry-content']//img/@src",
  ".//div[@class='entry-content']//p[1]//text()",
  ".//div[@class='entry-content']//p[1]//text():.:Value(2)",
  ".//div[@class='entry-content']//p[2]//text():.:NoSubNodes(a)",
  ".//div[@class='entry-content']//p[2]//text():.:Value(2):NoSubNodes(a)"
  );
  ".//div[@class='entry-content']//p:.:n(p)"
NoSubNodes
Test_telecharger_pdf_view_01();
_tr.WriteLine(LaCroix.PrintExists(new Date(2013, 5, 1)).ToString());
_tr.WriteLine("{0}", LeMonde.PrintExists(new Date(2012, 12, 3)));

PrintManager pm = new PrintManager(_wr.Config.GetElement("Print"));
IPrint print = pm.Get("le_monde");
print.SetDate(new Date(2012, 12, 3));
_tr.WriteLine("{0}", print.GetPrintNumber());
_tr.WriteLine("current directory \"{0}\"", Directory.GetCurrentDirectory());
GetPrintManager();
string s = null;
_tr.WriteLine("toto" + s + "tata");
_tr.WriteLine(Path.GetFullPath(@"..\toto.txt"));

_hxr.ReadSelect("//li[@class='threadbit ']:.:EmptyRow", ".//h3[@class='threadtitle']//a/text():.:EmptyRow", ".//div[@class='author']//a//text()");
//ol id="threads"
//<ol id="threads" class="threads">
//:.:EmptyRow
//"//li[starts-with(@class, 'threadbit')]"
_hxr.ReadSelect("//ol[@class='threads']/li",
  ".//h3[@class='threadtitle']//a/text():.:n(title)",
  ".//div[@class='author']//text():.:Concat( ):n(author)");

_hxr.ReadSelect("//span[@class='prev_next']//a[@rel='next']", "@href");
<span class="prev_next">

_hxr.SelectValue("//span[@class='prev_next']//a[rel='next']/@href:.:EmptyRow");


_hxr.ReadSelect("//div[@class='postbody']//blockquote:.:EmptyRow", ".//text():.:Concat( )");
_hxr.ReadSelect("//span[@class='date']//text()");
_tr.WriteLine(_hxr.XDocument.zXPathValue("//span[@class='date']//text()"));

string xpath = "//div[@class='postbody']//div[@class='postrow has_after_content']";
XElement xe = _hxr.XDocument.Root.zXPathElement(xpath);
_tr.WriteLine("{0}", xe.Name);
IEnumerable<XElement> xes = xe.XPathSelectElements(".//div[@class='content']//img"); // 
_tr.WriteLine("{0}", xes.Count());

_tr.WriteLine("{0}", 104998527 / (6 * 60 + 20) / 1024.0);
_tr.WriteLine("{0}", new TimeSpan(25, 2, 3).ToString(@"hh\:mm\:ss"));
_tr.WriteLine(@"{0:hh\:mm\:ss}", new TimeSpan(23, 2, 3));

_tr.WriteLine(_hxr.WebRequestUserAgent);


_hxr.Load("http://pdf4fr.com/");
_hxr.ReadSelect("//article:.:EmptyRow", "./@id", "./header//a/@href", "./header//a//text()");
//_hxr.ReadSelect("//div[@class='postbody']//blockquote:.:EmptyRow", ".//text():.:Concat( )");
_hxr.ReadSelect("//li[@class='next']//a:.:EmptyRow", "./@href", ".//text()");
Test_XXElement_01();
Test_XText_01();

_hxr.Load("http://zone-ebooks.com/");
// <a href='http://zone-ebooks.com/page/2' class='nextpostslink'>?</a>
_hxr.ReadSelect("//a[@class='nextpostslink']:.:EmptyRow", "./@href");
// <div id="post-1838" class="post-1838 post type-post status-publish format-standard hentry category-journaux tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ebook tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ebook-gratuit tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-gratuit tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-pdf tag-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-telechargement tag-telecharge-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ddl tag-telecharge-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-uptobox tag-telechargement-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre tag-telecharger-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre tag-telecharger-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-ebook tag-telecharger-le-parisien-journal-de-paris-supp-economie-du-lundi-07-octobre-pdf clear-block count-1 odd author-admin first">
_hxr.ReadSelect("//div[starts-with(@id, 'post-')]:.:EmptyRow", ".//div[@class='post-date']//text()");


_hxr.Load("http://magazine3k.com/");
//<div class="res">
_hxr.ReadSelect("//div[@class='res']:.:EmptyRow", ".//text()");

_hxr.Load("http://www.telechargement-plus.com/e-book-magazines/");
_hxr.ReadSelect("//div[@class='base shortstory']:.:EmptyRow", ".//text()");
_hxr.ReadSelect("//div[@class='navigation']//a[text()='Next']:.:EmptyRow", "text()", "@href");
_hxr.Load("http://www.telechargement-plus.com/e-book-magazines/86887-advanced-cryation-photoshop-h-syrie-n19-novembre-2013-lien-direct.html");
_hxr.ReadSelect("//span[@id='post-img']:.:EmptyRow");
XXElement xe = new XXElement(_hxr.XDocument.Root);
xe = xe.XPathElement("//div[@id='dle-content']");
xe = xe.XPathElement(".//div[@class='binner']");
_tr.WriteLine(xe.XElement.zGetPath());
xe = xe.XPathElement(".//span[@id='post-img']");

/xml[1]/html[1]/body[1]/div[3]/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[1]/div[1]
/xml[1]/html[1]/body[1]/div[3]/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/center[1]/span[1]
/xml[1]/html[1]/body[1]/div[3]/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/span[1]
/xml[1]/html[1]/body[1]/div[3]/div[1]/div[1]/div[2]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]/div[1]/span[2]

_hxr.Load("http://www.telechargement-plus.com/e-book-magazines/");
_hxr.Load("http://www.telechargement-plus.com/e-book-magazines/livres/87929-browsing-natures-aisles-a-year-of-foraging-for-wild-food-in-the-suburbs.html");

_hxr.LoadXml(@"c:\pib\dev_data\exe\wrun\test\telechargement-plus\test\87003-kamasutra.xml");
_hxr.ReadSelect("/post/print/image/@source:.:EmptyRow");
_hxr.ReadSelect("//image/@source:.:EmptyRow");
Test_XmlFilesXPathEvaluate_01("/post/print/image/@source");
Test_XmlFilesXPathEvaluate_01("print/image/@source");

Test_LoadXml_01();
Test_LoadXml_02();
Test_LoadXml_03();

_hxr.Load(@"c:\pib\dev_data\exe\wrun\test\telechargement-plus\post\000087\87000-60-petits-maux-soignys-par-les-huiles-essentielles.html");
_hxr.ReadSelect("//div[@id='dle-content']//div[@class='maincont']//div[@class='binner']//div[@class='story-text']//text():.:EmptyRow");
_hxr.ReadSelect("//div[@id='dle-content']//div[@class='maincont']//div[@class='binner']//div[@class='story-text']//*:.:EmptyRow", "@type");
_hxr.ReadSelect("//div[@id='dle-content']//div[@class='maincont']//div[@class='binner']//div[@class='story-text']//*[@type != a]:.:EmptyRow");
_hxr.LoadXml(@"c:\pib\dev_data\exe\wrun\test\telechargement-plus\test\test_01.xml");
_hxr.ReadSelect("//*:.:EmptyRow");
_hxr.ReadSelect("//*[@type != //a]:.:EmptyRow");
_hxr.ReadSelect("//*[@type = //div]:.:EmptyRow");
Test_DescendantNodes_01();
Test_DescendantNodes_02();

//**************************************************************************************************************************************************************************************************
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\runsource\runsource_dll\irunsource_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\runsource\runsource_dll\runsource_dll_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\runsource\runsource_domain\runsourced32_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\runsource\runsource\runsource32_project.xml");

RunSource.CurrentRunSource.Compile_Project(@"..\..\..\DownloadManager\Core\MyDownloader.Core_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\DownloadManager\Extension\MyDownloader.Extension_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\DownloadManager\IEPlugin\MyDownloader.IEPlugin_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\DownloadManager\Spider\MyDownloader.Spider_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\DownloadManager\App\MyDownloader.App_project.xml");

Compiler.TraceLevel = 1;
Compiler.TraceLevel = 2;

RunSource.CurrentRunSource.Compile_Project(@"..\..\..\runsource\runsource_dll\runsource2_dll_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\runsource\runsource_domain\runsourced32_2_project.xml");
//**************************************************************************************************************************************************************************************************


Test_MongoDB_entity_01();
Test_MongoDB_entity_02();
Test_MongoDB_data01_01();
Test_MongoDB_Insert_01("data01", "_id", 1, "name", "tata");
Test_MongoDB_Insert_01("data01", "_id", 2, "name", "toto");
Test_MongoDB_Insert_01("data01", new BsonDocument { { "_id", 3 }, { "name", "tutu" } });
Test_MongoDB_Save_01("data01", "_id", 1, "age", 20);
Test_MongoDB_Update_01("data01", new QueryDocument { { "_id", 2} }, new UpdateDocument { { "$set", new BsonDocument { { "age", 20 } } } });
Test_MongoDB_Update_Data01_01();
Test_MongoDB_Count_01("data01", new QueryDocument { { "_id", 2} });
Test_MongoDB_FindAll_01("data01");
Test_MongoDB_FindAll_01("entities");
Test_MongoDB_FindAll_02<Entity>("entities");
Test_MongoDB_FindAll_02<Entity>("test");
Test_MongoDB_FindAll_02<Entity>("testData");
Test_MongoDB_Drop_01("data01");
Test_MongoDB_DateTime_01();
Test_DownloadService_remove_01(1);
Test_Dictionary_01();
Test_MongoDB_FindAll_01("");

_tr.WriteLine("toto");
Test_MongoDB_Serialize_01();
Test_Mongo_CollectionName_01("dl", "TelechargementPlusDetail");


//*************************************************************************************************************************
//****                                   test MongoCommand
//*************************************************************************************************************************
pb.Data.Mongo.TraceMongoCommand.Eval("{ buildInfo: 1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("{ isMaster: 1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("{ listDatabases: 1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("{ listCommands: 1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "admin");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "local");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "test");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "damien");
pb.Data.Mongo.TraceMongoCommand.Eval("db.runCommand( { count: 'TelechargementPlusDetail' } )", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.runCommand( { count: 'RapideDdl_Detail' } )", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.TelechargementPlus.find( {} )", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.createCollection('test')", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.test.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("test", "db.test_update.remove({ _id: 2 })");
pb.Data.Mongo.TraceMongoCommand.Eval("dl", "db.TelechargementPlus.renameCollection('TelechargementPlus_old')");


pb.Data.Mongo.TraceMongoCommand.Count("dl", "TelechargementPlusDetail");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "TelechargementPlusDetail", "{ _id: { $gt: 138100 } }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "TelechargementPlusDetail", "{}");
pb.Data.Mongo.TraceMongoCommand.FindOneByIdAs<BsonDocument>("dl", "TelechargementPlusDetail", 137747);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "TelechargementPlusDetail", "{}", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "TelechargementPlusDetail", "{ _id: { $gt: 138100 } }", sort: "{ _id: -1 }", fields: "{ _id: 1, 'download.title': 1, 'download.creationDate': 1 } }",
    limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "TelechargementPlusDetail", "{ $and: [ { _id: { $gt: 138100 } }, { 'download.creationDate': { $gt: ISODate('2014-06-11T17:46:00Z') } } ] }",
    sort: "{ _id: -1 }", fields: "{ _id: 1, 'download.title': 1, 'download.creationDate': 1 } }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "TelechargementPlus_old", "{}");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "TelechargementPlus", "{ _id: { $gt: 100000 } }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "TelechargementPlus", "{ _id: { $gt: 100000 } }", fields: "{ title: 1, creationDate: 1 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "TelechargementPlus", "{ _id: { $gt: 100000 } }", fields: "{ title: 1, creationDate: 1 }", sort: "{ creationDate: -1 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "TelechargementPlus", "{ _id: { $gt: 100000 } }", fields: "{ _id: 0 }", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("test", "data01", "{}");
pb.Data.Mongo.TraceMongoCommand.Remove("test", "data01", "{}");




//*************************************************************************************************************************
//****                                   test mongo update
//*************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "test");
pb.Data.Mongo.TraceMongoCommand.Eval("test", "db.createCollection('test_update')");
//pb.Data.Mongo.TraceMongoCommand.Eval("test", "db.test_update.drop()");
pb.Data.Mongo.TraceMongoCommand.Eval("test", "db.test_update.count()");
pb.Data.Mongo.TraceMongoCommand.FindAs("test", "test_update", "{}");
// UpdateFlags.None UpdateFlags.Multi UpdateFlags.Upsert
pb.Data.Mongo.TraceMongoCommand.Update("test", "test_update", "{ _id: 1 }", "{ $set: { dl: { url: 'http://test/1/', title: 'test 1' } } }", UpdateFlags.Upsert);
pb.Data.Mongo.TraceMongoCommand.Update("test", "test_update", "{ _id: 1 }", "{ $set: { info: { toto: 'toto', tata: 'tata' } } }", UpdateFlags.Upsert);
pb.Data.Mongo.TraceMongoCommand.Update("test", "test_update", "{ _id: 1 }", "{ $unset: { info: '' } }", UpdateFlags.Upsert);
pb.Data.Mongo.TraceMongoCommand.Update("test", "test_update", "{ _id: 2 }", "{ _id: 2, dl: { url: 'http://test/2/', title: 'test 2' } }", UpdateFlags.Upsert);
pb.Data.Mongo.TraceMongoCommand.Remove("test", "test_update", "{ _id: 2 }");
pb.Data.Mongo.TraceMongoCommand.Update("test", "test_update", "{ _id: 3 }", "{ _id: 4, dl: { url: 'http://test/2/', title: 'test 2' } }", UpdateFlags.Upsert);

pb.Data.Mongo.TraceMongoCommand.Find("test", "test", "{}");
pb.Data.Mongo.TraceMongoCommand.FindAll("test", "test");
pb.Data.Mongo.TraceMongoCommand.Remove("test", "test", "{}");
pb.Data.Mongo.TraceMongoCommand.RemoveAll("test", "test");
pb.Data.Mongo.TraceMongoCommand.Count("test", "test", "{}");
pb.Data.Mongo.TraceMongoCommand.Update("test", "test", "{ _id: 1 }", "{ $set: { title: 'toto', n: 1 } }", MongoDB.Driver.UpdateFlags.Upsert);
pb.Data.Mongo.TraceMongoCommand.Update("test", "test", "{ _id: 2 }", "{ $set: { title: 'tata', n: 2 } }", MongoDB.Driver.UpdateFlags.Upsert);
pb.Data.Mongo.TraceMongoCommand.Update("test", "test", "{ _id: 3 }", "{ $set: { title: 'tutu', n: 3 } }", MongoDB.Driver.UpdateFlags.Upsert);
pb.Data.Mongo.TraceMongoCommand.FindAndModify("test", "test", "{ _id: 1 }", "{ $inc: { n: 2 } }");
pb.Data.Mongo.TraceMongoCommand.FindAndRemove("test", "test", "{ _id: 1 }");


pb.Data.Mongo.TraceMongoCommand.Eval("db.Images.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Images", "{}");
pb.Data.Mongo.TraceMongoCommand.FindAs<pb.Web.MongoImage>("dl", "Images", "{}");

pb.Web.MongoImage mongoImage = new pb.Web.MongoImage { Url = "http://pixhst.com/avaxhome/24/73/002e7324.jpeg", File = "pixhst.com\\pixhst.com_avaxhome_24_73_002e7324.jpeg", Width = 905, Height = 908 };
Trace.WriteLine(mongoImage.zToJson(true));

pb.Data.Mongo.TraceMongoCommand.Update("dl", "Images", "{ _id: 3 }", "{ _id: 4, dl: { url: 'http://test/2/', title: 'test 2' } }", UpdateFlags.Upsert);




Trace.WriteLine(XmlConfig.CurrentConfig.Get("DataDir"));
Trace.WriteLine(XmlConfig.CurrentConfig.Get("TelechargementPlus/DataDir"));
Trace.WriteLine(XmlConfig.CurrentConfig.Get("TelechargementPlus/Header/CacheDirectory"));

Test_TelechargementPlus_PostDetail_Serialize_01("http://www.telechargement-plus.com/e-book-magazines/136552-les-journaux-mercredi-04-05-juin-2014-pdflien-direct.html", reload: false, loadImage: true);
Test_TelechargementPlus_PostDetail_Serialize_01("http://www.telechargement-plus.com/e-book-magazines/magazines/137747-multi-motor-sport-n58-juin-juillet-2014.html", reload: false, loadImage: true);
Test_TelechargementPlus_PostDetail_Serialize_02("http://www.telechargement-plus.com/e-book-magazines/magazines/137747-multi-motor-sport-n58-juin-juillet-2014.html", reload: false, loadImage: true);
Test_TelechargementPlus_PostDetail_Serialize_03(137747, "dl", "TelechargementPlusDetail");
Test_TelechargementPlus_PostDetail_Serialize_04("http://www.telechargement-plus.com/e-book-magazines/magazines/137747-multi-motor-sport-n58-juin-juillet-2014.html", 137747, "dl", "TelechargementPlusDetail");
Test_TelechargementPlus_PostDetail_Serialize_05(true, "http://www.telechargement-plus.com/e-book-magazines/magazines/137747-multi-motor-sport-n58-juin-juillet-2014.html", 137747, "dl", "TelechargementPlusDetail");
Test_TelechargementPlus_PostDetail_Serialize_05(false, "http://www.telechargement-plus.com/e-book-magazines/magazines/137747-multi-motor-sport-n58-juin-juillet-2014.html", 137747, "dl", "TelechargementPlusDetail");
Test_TelechargementPlus_PostDetail_Serialize_06();
Test_ZValue_Serialize_01();
Test_ZValue_Serialize_02();
Test_BsonDocument_CompareTo_01();

Test_Download_Image_05();
Http2.LoadToFile("http://www.babelio.com/couv/11657_639334.jpeg", Path.Combine(_dataDir, "11657_639334.jpeg"));


Test_TelechargementPlus_LoadDetailItemList_02(startPage: 1, maxPage: 1, reloadHeaderPage: false, reloadDetail: false, loadImage: true);
pb.Data.Mongo.TraceMongoCommand.Query("dl", "TelechargementPlusDetail", "{}");
pb.Data.Mongo.TraceMongoCommand.Remove("dl", "TelechargementPlusDetail", "{}");
Trace.WriteLine("{0}", typeof(ImageHtml));


MongoDB.Bson.BsonDocument d = new MongoDB.Bson.BsonDocument { { "", "" } };
Trace.WriteLine(new MongoDB.Bson.BsonDocument { { "_id", 1234 } }.ToJson());
Trace.WriteLine("{0}", new MongoDB.Bson.BsonDocument { { "_id", 1234 } }["_idzz"]);

Trace.WriteLine(MongoDB.Driver.UpdateFlags.None.ToString());
Trace.WriteLine((MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi).ToString());

Test_zone_ebooks_loadPostHeader_01(startPage: 1, maxPage: 1, loadImage: true);
Test_zone_ebooks_loadPostHeader_01(startPage: 1, maxPage: 3, loadImage: true);
Test_zone_ebooks_loadPostHeader_01(startPage: 1, maxPage: 10, loadImage: true);
Test_ZoneEbooks_LoadHeaderPages_01(startPage: 1, maxPage: 10, reload: false, loadImage: true);
Test_ZoneEbooks_LoadHeaderPages_01(startPage: 1, maxPage: 3, reload: false, loadImage: true);
Test_ZoneEbooks_LoadHeaderPages_01(startPage: 1, maxPage: 3, reload: true, loadImage: true);

Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://www.site.com/toto/tata/index.php?name=search", pb.Web.UrlFileNameType.FileName));
Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://www.site.com/toto/tata/index.php?name=search", pb.Web.UrlFileNameType.Path));
Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://www.site.com/toto/tata/index.php?name=search", pb.Web.UrlFileNameType.Query));
Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://www.site.com/toto/tata/index.php?name=search", pb.Web.UrlFileNameType.Content));
Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://www.site.com/toto/tata/index.php?name=search", pb.Web.UrlFileNameType.Content, requestParameters: new HttpRequestParameters { content = "raisonSociale=&SIRET=&departements%5B%5D=67&departements%5B%5D=68" }));
Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://www.site.com/toto/tata/index.php?name=search", pb.Web.UrlFileNameType.Content, requestParameters: new HttpRequestParameters { content = "raisonSociale=&SIRET=&departements[]=67&departements[]=68" }));
Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://www.site.com/toto/tata/index.php?name=search", pb.Web.UrlFileNameType.Content, requestParameters: new HttpRequestParameters { content = "name1=value1&name2[]=value2&name2[]=value3" }));
Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://www.site.com/toto/tata/index.php?name=search", pb.Web.UrlFileNameType.Host | pb.Web.UrlFileNameType.Path));
Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://zone-ebooks.com/page/2", pb.Web.UrlFileNameType.Host | pb.Web.UrlFileNameType.Path));
Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://zone-ebooks.com/page/2/toto.php", pb.Web.UrlFileNameType.Host | pb.Web.UrlFileNameType.Path));
Trace.WriteLine("toto" + null);

Trace.WriteLine(new Uri("http://www.site.com/toto/tata/index.php?name=search").Authority);
Trace.WriteLine(new Uri("http://www.site.com/toto/tata/index.php?name=search").DnsSafeHost);
Trace.WriteLine(new Uri("http://www.site.com/toto/tata/index.php?name=search").Fragment);
Trace.WriteLine(new Uri("http://www.site.com/toto/tata/index.php?name=search").Host);
Trace.WriteLine(new Uri("http://www.site.com/toto/tata/index.php?name=search").HostNameType.ToString());
Trace.WriteLine(new Uri("http://localhost/toto/tata/index.php?name=search").HostNameType.ToString());
Trace.WriteLine(new Uri("http://127.1.2.3/toto/tata/index.php?name=search").HostNameType.ToString());
Trace.WriteLine(new Uri("http://www.site.com/toto/tata/index.php?name=search").Scheme);

Trace.WriteLine(new Uri("http://zone-ebooks.com/page/2").Host);
Trace.WriteLine(new Uri("http://zone-ebooks.com/page/2").PathAndQuery);
Trace.WriteLine(new Uri("http://zone-ebooks.com/page/2").AbsolutePath);
Trace.WriteLine(System.Web.HttpUtility.UrlDecode(new Uri("http://zone-ebooks.com/page/2").AbsolutePath));



Test.Test_Unit.Test_Unit_UrlToFileName.Test(@"c:\pib\dev_data\exe\runsource\test_unit");


_hxr.Load("http://www.rapide-ddl.com/ebooks/magazine/35034-automobiles-classiques-no241-juillet-aogt-2014.html");
_hxr.ReadSelect("//div[@class='lcolomn mainside']:.:EmptyRow", ".//text()");
_hxr.ReadSelect("//div[@class='lcolomn mainside']//div[@class='spbar']:.:EmptyRow", ".//text()");
_hxr.ReadSelect("//div[@class='lcolomn mainside']//div[@class='spbar']//text():.:EmptyRow");

_hxr.Load(@"c:\pib\dev_data\exe\runsource\download\sites\rapide-ddl\cache\detail\ebooks_magazine_36143-secrets-dhistoire-de-dgtours-en-france-no1-2014.html");
_hxr.ReadSelect("//div[@class='maincont']:.:EmptyRow", "@class");
_hxr.ReadSelect("//div[@class='maincont']/div:.:EmptyRow", "@class", "@id");
_hxr.ReadSelect("//div[@class='maincont']/div/div:.:EmptyRow", "@class", "@id");
//T1.1.1(t1)/tbody[1]/tr[1]/td[1]/div[1]/div[1]/div[2]/div[1]/div[1]
_hxr.ReadSelect("//div[@class='maincont']//div[2]:.:EmptyRow", "@class", "@id");
_hxr.ReadSelect("//div[@class='maincont']//div[2]//a:.:EmptyRow", "@href", ".//text()");
_hxr.ReadSelect("//div[@class='maincont']//div[6]//a:.:EmptyRow", "@href", ".//text()");
_hxr.ReadSelect("//div[@class='maincont']//div[7]//a:.:EmptyRow", "@href", ".//text()");
_hxr.ReadSelect("//div[@class='maincont']//div[8]//a:.:EmptyRow", "@href", ".//text()");
_hxr.ReadSelect("//div[@class='maincont']//div[9]//a:.:EmptyRow", "@href", ".//text()");









Trace.WriteLine(System.IO.Directory.GetCurrentDirectory());
Trace.WriteLine(RunSource.CurrentRunSource.DataDir);
Test.Test_Image.Test_Image_f.Test_Download_Image_05("http://pixhst.com/avaxhome/05/a4/002da405.jpeg");
Test.Test_Image.Test_Image_f.Test_Save_Image_05("http://pixhst.com/avaxhome/05/a4/002da405.jpeg");
Test.Test_Image.Test_Image_f.Test_Save_Image_05("http://imagizer.imageshack.us/v2/360x600q150/http://pixhst.com/avaxhome/9a/67/002e679a_medium.jpeg", @"c:\pib\_dl\002e679a_medium.jpeg");
Test.Test_Image.Test_Image_f.Test_Save_Image_05("http://pixhst.com/avaxhome/9a/67/002e679a_medium.jpeg", @"c:\pib\_dl\002e679a_medium_2.jpeg");
Test.Test_Image.Test_Image_f.Test_Load_Image_FromFile_02("002da405.jpeg");
Trace.WriteLine(pb.Web.zurl.UrlToFileName("http://imagizer.imageshack.us/v2/360x600q150/http://pixhst.com/avaxhome/9a/67/002e679a_medium.jpeg", pb.Web.UrlFileNameType.Host | pb.Web.UrlFileNameType.Path));

Test_Bson.Test_Bson_f.Test_BsonWriter_01();

Test.Test_Unit.Mongo.Test_Unit_BsonDocumentsToDataTable.Test();
Test.Test_Unit.Mongo.Test_Unit_BsonReader.Test();
Test.Test_Unit.Mongo.Test_Unit_PBBsonReader.Test_TextReader();
Test.Test_Unit.Mongo.Test_Unit_PBBsonReader.Test_CloneTextReader();
Test.Test_Unit.Mongo.Test_Unit_PBBsonReader.Test_BookmarkTextReader();
Test.Test_Unit.Mongo.Test_Unit_PBBsonReader.Test_BinaryReader();
Test.Test_Unit.Mongo.Test_Unit_PBBsonReader.Test_CloneBinaryReader();
Test.Test_Unit.Mongo.Test_Unit_PBBsonReader.Test_BookmarkBinaryReader();
Test.Test_Unit.Mongo.Test_Unit_PBBsonEnumerateValues.Test();

Trace.WriteLine("toto");
Trace.CurrentTrace.RemoveTraceFile(@"c:\pib\dev_data\exe\runsource\test_unit\BsonReader\BsonReader_01_out.txt");

Test_LoadMongoDetail_01();
Test_ClassMap_01();
Test_IsClassMapRegistered_01(typeof(Download.Print.RapideDdl.RapideDdl_Base));
Test_RegisterClassMap_RapideDdl_Base_01();

Test.Test_Bson.Test_Bson_f2.Test_Serialize_01(MongoDB.Bson.Serialization.Options.DictionarySerializationOptions.Document);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_01(MongoDB.Bson.Serialization.Options.DictionarySerializationOptions.ArrayOfDocuments);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_01(MongoDB.Bson.Serialization.Options.DictionarySerializationOptions.ArrayOfArrays);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfDocuments);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_03(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_03(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfDocuments);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_03(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays);

Test.Test_Bson.Test_Bson_f2.Test_Serialize_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_02(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfDocuments);

Test.Test_Bson.Test_Bson_f2.Test_Serialize_04(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.Document);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_04(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfArrays);
Test.Test_Bson.Test_Bson_f2.Test_Serialize_04(MongoDB.Bson.Serialization.Options.DictionaryRepresentation.ArrayOfDocuments);

Test.Test_Bson.Test_Bson_f2.Test_DateTime_01();

Trace.WriteLine(Test.Test_Bson.Test_Bson_f2.Create_Test_Bson_Class_01().zToJson());
Trace.WriteLine(Test.Test_Bson.Test_Bson_f2.Create_RapideDdl_PostDetail_01().zToJson());
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{}", limit: 5, sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("test", "Test_RapideDdl_Detail", "{}", limit: 5, sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "test");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_MongoDocumentStore_01(1234, Test.Test_Bson.Test_Bson_f2.Create_RapideDdl_PostDetail_01());
Test.Test_Bson.Test_Bson_f2.Test_Serialize_06(Test.Test_Bson.Test_Bson_f2.Create_RapideDdl_PostDetail_01());
Test.Test_Bson.Test_Bson_f2.Test_Serialize_06(Test.Test_Bson.Test_Bson_f2.Create_Test_Bson_Class_01());
Test.Test_Bson.Test_Bson_f2.Test_Serialize_06(Test.Test_Bson.Test_Bson_f2.Create_Test_Bson_Class_02());


MongoDB.Bson.Serialization.BsonSerializer.Trace = true;
MongoDB.Bson.Serialization.BsonMemberMap.Trace = true;
MongoDB.Bson.Serialization.BsonSerializer.Trace = false;
MongoDB.Bson.Serialization.BsonMemberMap.Trace = false;

Test.Test_Reflection.Test_Reflection_f.Test_Reflection_01();
Test.Test_Reflection.Test_Reflection_f.Test_TraceType_01();
Test.Test_Reflection.Test_Reflection_f.Test_Assembly_01();
Test.Test_Reflection.Test_Reflection_f.Test_Assembly_02();
System.Reflection.Assembly.GetEntryAssembly().Location.zTrace();
System.Reflection.Assembly.GetEntryAssembly().Location.zTraceJson();
System.Reflection.Assembly.GetEntryAssembly().GetTypes().zView();
System.Reflection.Assembly.GetEntryAssembly().GetTypes().Select(type => type.zGetTypeName()).zView();

Test.Test_Diagnostics.Test_Diagnostics_Trace.Test_Diagnostics_Trace_01();
System.Diagnostics.Trace.WriteLine("test trace listener 01");
System.Diagnostics.Trace.TraceInformation("information {0}", 1);
System.Diagnostics.Trace.TraceWarning("warning {0}", 1);
System.Diagnostics.Trace.TraceError("error {0}", 1);

Test_Bson.Test_Bson_f2.Test_BsonReader_01();
Test_Bson.Test_Bson_f2.Test_PBBsonEnumerateValues_01();


Trace.WriteLine(new Dictionary<string, string>().GetType().AssemblyQualifiedName);
Trace.WriteLine(new Dictionary<string, string>().GetType().GetGenericTypeDefinition().AssemblyQualifiedName);
Trace.WriteLine(typeof(Dictionary<,>).AssemblyQualifiedName);

Trace.WriteLine("{0}", 34901 / 1000 * 1000);
Trace.WriteLine(DateTime.UtcNow.ToString("o"));
Trace.WriteLine(DateTime.Now.ToUniversalTime().ToString("o"));
Trace.WriteLine("{{ {0} }}", "toto");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_01();
Test.Test_Http.Test_AllDebrib.GetAllDebribAccount();
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://ul.to/epws504d");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://uploadrocket.net/zc3b4205na1v/DF475.rar.html");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://www.uploadable.ch/file/BgeEV6KxnCbB/VPFN118.rar");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://rapidgator.net/file/096cca55aba4d9e9dac902b9508a23b1/MiHN65.rar.html");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://turbobit.net/15cejdxrzleh.html");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://uploaded.net/file/rx2k0zxa");
http://s05.alldebrid.com/dl/dqqwoxbf14/L_Express_3292.pdf
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://uptobox.com/6wk2ni9g2nfs");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://www.uploadable.ch/file/gdF2ekYGX7am");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://hsrpnt950v.1fichier.com/");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("");

Test.Test_Web.Test_Mail_f.Test_Mail_01();
Test.Test_Web.Test_Mail_f.Test_Mail_02("pierre.beuzart@gmail.com", "test 3", "test 3");


Test.Test_iTextSharp.Test_iTextSharp_f2.Test_ControlPdfDirectory_01(@"c:\pib\_dl\_pib\dl");
Test.Test_iTextSharp.Test_iTextSharp_f2.Test_ReadPdf_01(@"c:\pib\_dl\_pib\dl\l_express\l_express_2014-08-06.pdf");
Test.Test_iTextSharp.Test_iTextSharp_f2.Test_ReadPdf_01(@"c:\pib\_dl\_pib\dl\l_express\l_express_2014-08-13.pdf");
Test.Test_iTextSharp.Test_iTextSharp_f2.Test_ControlPdf_01(@"c:\pib\_dl\_pib\dl\l_express\l_express_2014-08-06.pdf");
Test.Test_iTextSharp.Test_iTextSharp_f2.Test_ControlPdf_01(@"c:\pib\_dl\_pib\dl\l_express\l_express_2014-08-13.pdf");
Test.Test_Zip.Test_Zip_f.Test_Uncompress_01(@"c:\pib\_dl\_pib\dl\l_express\a\l_express_2014-07-02.rar");
Test.Test_Zip.Test_Zip_f.Test_Uncompress_01(@"c:\pib\_dl\_pib\dl\journaux\journaux_2014-07-28.zip");
Test.Test_Zip.Test_Zip_f.Test_Uncompress_01(@"c:\pib\_dl\_pib\dl\journaux\test\journaux_2014-07-28_test_01.zip");
Test.Test_Zip.Test_Zip_f.Test_Uncompress_01(@"c:\pib\_dl\_pib\dl\journaux\test\journaux_2014-07-28_test_02.zip");
Test.Test_Zip.Test_Zip_f.Test_Uncompress_02(@"c:\pib\_dl\_pib\dl\journaux\journaux_2014-07-28.zip");
Test.Test_Zip.Test_Zip_f.Test_ViewCompressFile_01(@"c:\pib\_dl\_pib\dl\journaux\journaux_2014-07-28.zip");
Test.Test_Zip.Test_Zip_f.Test_ViewCompressFile_01(@"c:\pib\_dl\_pib\dl\journaux\test\journaux_2014-07-28_test_02.zip");
Test.Test_Zip.Test_Zip_f.Test_CompressManager_01(@"c:\pib\_dl\_pib\dl\journaux\journaux_2014-07-28.zip", @"c:\pib\_dl\_pib\dl\journaux\journaux_2014-07-28",
  pb.IO.UncompressOptions.ExtractFullPath | pb.IO.UncompressOptions.RenameExistingFile | pb.IO.UncompressOptions.UncompressCompressFiles);
Test.Test_Zip.Test_Zip_f.Test_CompressManager_01(@"c:\pib\_dl\_pib\dl\journaux\journaux_2014-07-28.zip", @"c:\pib\_dl\_pib\dl\journaux\journaux_2014-07-28",
  pb.IO.UncompressOptions.ExtractFullPath | pb.IO.UncompressOptions.RenameExistingFile);

Trace.WriteLine(RunSource.CurrentRunSource.Config.Element.zXPathElement("SelectPosts/Date").zAttribValue("regex"));
Trace.WriteLine(RunSource.CurrentRunSource.Config.Get("Log"));
Trace.WriteLine(RunSource.CurrentRunSource.Config.Get("Image/CacheDirectory"));
Trace.WriteLine(RunSource.CurrentRunSource.Config.Get("RapideDdl/DataDir"));
Trace.WriteLine(RunSource.CurrentRunSource.Config.Get("RapideDdl/Header/CacheDirectory"));
Trace.WriteLine(RunSource.CurrentRunSource.Config.Get("SelectPosts/Date/@regex"));

















//***********************************************************************************************************************************************************
//****                                                  Test.Test_Unit.Print
//****                                                  Test_Unit_RegexValues.Test_FindDate
//****                      c:\pib\dev_data\exe\runsource\test_unit\Print\RegexValues\Date\
//***********************************************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{}", fields: "{ 'download.title': 1 }", limit: 5, sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", @"c:\pib\dev_data\exe\runsource\test_unit\Print\RegexValues\Date\RegexValues_FindDate.txt",
  fields: "{ '_id': 0 'download.title': 1 }", limit: 1000, sort: "{ 'download.creationDate': -1 }",
  transformDocument: doc => new MongoDB.Bson.BsonDocument { { "text", doc["download"]["title"] } });
Test.Test_Unit.Print.Test_Unit_RegexValues.Test_FindDate_02(new pb.Text.FindDateManager(RunSource.CurrentRunSource.Config.GetElements("FindPrints/Dates/Date"), compileRegex: true), "RegexValues_FindDate.txt");
Test.Test_Unit.Print.Test_Unit_RegexValues.Test_FindDate_03(new pb.Text.FindDateManager(RunSource.CurrentRunSource.Config.GetElements("FindPrints/Dates/Date"), compileRegex: true), "RegexValues_FindDate.txt");
Test.Test_Unit.Print.Test_Unit_RegexValues.Test_FindDate_04(new pb.Text.FindDateManager_new(RunSource.CurrentRunSource.Config.GetElements("FindPrints/Dates/DateNew"), compileRegex: true), "RegexValues_FindDate.txt");
Test.Test_Unit.Print.Test_Unit_RegexValues.FindDate(new pb.Text.FindDateManager(RunSource.CurrentRunSource.Config.GetElements("FindPrints/Dates/Date"), compileRegex: true),
  //"France Football N? 3566 - Mardi 19 Ao?t 2014");
  //"L'Equipe du lundi 22 septembre 2014");
  //"L'Equipe lundi 22 septembre 2014");
  //"L'Equipe - lundi 22 septembre 2014");
  //"France Football N?  - Mardi 22 septembre 2014");
  "Le Parisien Magazine - 1er au 7 F?vrier 2013");
Trace.WriteLine(Test.Test_Unit.Print.Test_Unit_RegexValues.FindDateNew(
  new pb.Text.FindDateManager_new(new XmlConfig(RunSource.CurrentRunSource.GetPathSource(RunSource.CurrentRunSource.Config.GetExplicit("PrintList1Config"))).GetElements("FindPrints/Dates/DateNew"), compileRegex: true),
  "Le Parisien Magazine - 1er au 7 F?vrier 2013").zToJson());
/*****************************************  
Download.Print.SelectPostManager_f.Test_FindDate_01("{}", limit: 50);
Test.Test_Unit.Print.Test_Unit_RegexValues.Test_FindDate_01(new RegexValuesList(RunSource.CurrentRunSource.Config.GetElements("FindPrints/Dates/Date"), compileRegex: true),
  //@"c:\pib\dev_data\exe\runsource\test_unit\RegexValues\RegexValues_FindDate_01.txt");
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\RegexValues\Date\RegexValues_FindDate_01.txt");
Test.Test_Unit.Print.Test_Unit_RegexValues.Test_FindDate_01(new RegexValuesList(RunSource.CurrentRunSource.Config.GetElements("FindPrints/Dates/Date"), compileRegex: true),
  //@"c:\pib\dev_data\exe\runsource\test_unit\RegexValues\RegexValues_FindDate_02.txt");
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\RegexValues\Date\RegexValues_FindDate_02.txt");
Test.Test_Unit.Print.Test_Unit_RegexValues.Test_FindDate_01(new RegexValuesList(RunSource.CurrentRunSource.Config.GetElements("FindPrints/Dates/Date"), compileRegex: true),
  //@"c:\pib\dev_data\exe\runsource\test_unit\RegexValues\RegexValues_FindDate_03.txt");
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\RegexValues\Date\RegexValues_FindDate_03.txt");
Test.Test_Unit.Print.Test_Unit_RegexValues.Test_FindDate_02(new pb.Text.FindDateManager(RunSource.CurrentRunSource.Config.GetElements("FindPrints/Dates/Date"), compileRegex: true),
  //@"c:\pib\dev_data\exe\runsource\test_unit\RegexValues\RegexValues_FindDate_01.txt");
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\RegexValues\Date\RegexValues_FindDate_01.txt");
Test.Test_Unit.Print.Test_Unit_RegexValues.Test_FindDate_02(new pb.Text.FindDateManager(RunSource.CurrentRunSource.Config.GetElements("FindPrints/Dates/Date"), compileRegex: true),
  //@"c:\pib\dev_data\exe\runsource\test_unit\RegexValues\RegexValues_FindDate_02.txt");
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\RegexValues\Date\RegexValues_FindDate_02.txt");
*****************************************/


//***********************************************************************************************************************************************************
//****                                                  Test.Test_Unit.Print
//****                                                  Test_Unit_RegexValues.Test_FindNumber
//****                      c:\pib\dev_data\exe\runsource\test_unit\Print\RegexValues\Number\
//***********************************************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", @"c:\pib\dev_data\exe\runsource\test_unit\Print\RegexValues\Number\RegexValues_FindNumber.txt",
  fields: "{ '_id': 0 'download.title': 1 }", limit: 1000, sort: "{ 'download.creationDate': -1 }",
  transformDocument: doc => new MongoDB.Bson.BsonDocument { { "title", doc["download"]["title"] } });
Test.Test_Unit.Print.Test_Unit_RegexValues.Test_FindNumber_02(new pb.Text.FindNumberManager(RunSource.CurrentRunSource.Config.GetElements("FindPrints/Numbers/Number"), compileRegex: true),
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\RegexValues\Number\RegexValues_FindNumber.txt");


//***********************************************************************************************************************************************************
//****                                                  Test.Test_Unit.Print
//****                                                  Test_Unit_FindPrint
//****                      c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\FindPrint.txt
//***********************************************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", @"c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\FindPrint.txt",
  fields: "{ '_id': 0 'download.title': 1, 'download.category': 1, 'download.isPrint': 1, 'download.infos': 1 }", limit: 0, sort: "{ 'download.creationDate': -1 }",
  transformDocument: doc => new MongoDB.Bson.BsonDocument { { "title", doc["download"]["title"] }, { "category", doc["download"]["category"] },
  { "isPrint", doc["download"]["isPrint"] }, { "infos", doc["download"]["infos"] } });

Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 1), "FindPrint.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2), "FindPrint.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "FindPrint.txt");
//Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 4), "FindPrint.txt");

Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("FindPrint_out_bson.txt", @"_archive\FindPrint_v1_02\FindPrint_out_bson.txt", "FindPrint_out_bson_compare_v1_02.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("FindPrint_out_bson.txt", @"_archive\FindPrint_v1_02\FindPrint_out_bson_corrected.txt", "FindPrint_out_bson_compare_v1_02_corrected.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("FindPrint_out_bson.txt", @"_archive\FindPrint_v2_02\FindPrint_out_bson.txt", "FindPrint_out_bson_compare_v2_02.txt");


Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Correction(@"_archive\FindPrint_v1_02\FindPrint_out_bson.txt", @"_archive\FindPrint_v1_02\FindPrint_out_bson_correction.txt",
  @"_archive\FindPrint_v1_02\FindPrint_out_bson_corrected.txt");

Test.Test_Unit.Print.Test_Unit_FindPrint.Test_ViewDateCapture_01("FindPrint_out_bson.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_ViewDateCapture_02("FindPrint_out_bson.txt");


RunSource.CurrentRunSource.View(
from tfp in zmongo.BsonReader<Test.Test_Unit.Print.TestFindPrint>(@"c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\FindPrint_out_bson.txt")
where tfp.findPrint_dateCaptureList != null && tfp.findPrint_dateCaptureList.Length > 0 select tfp);



// sort: "{ 'download.creationDate': -1 }"     sort: "{ 'download.title': 1 }"
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", @"c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\le_parisien.txt",
  query: "{ 'download.title': /.*parisien.*/i }",
  fields: "{ '_id': 0 'download.title': 1, 'download.category': 1, 'download.isPrint': 1, 'download.infos': 1 }", limit: 0, sort: "{ 'download.creationDate': -1 }",
  transformDocument: doc => new MongoDB.Bson.BsonDocument { { "title", doc["download"]["title"] }, { "category", doc["download"]["category"] },
  { "isPrint", doc["download"]["isPrint"] }, { "infos", doc["download"]["infos"] } });
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 1), "le_parisien.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2), "le_parisien.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "le_parisien.txt");
//Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 4), "le_parisien.txt");

Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("le_parisien_out_bson.txt", @"_archive\LeParisien_v1_01\le_parisien_out_bson.txt", "le_parisien_out_bson_compare_v1_01.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("le_parisien_out_bson.txt", @"_archive\LeParisien_v1_01\le_parisien_out_bson_corrected.txt", "le_parisien_out_bson_compare_v1_01_corrected.txt");

Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Correction(@"_archive\LeParisien_v1_01\le_parisien_out_bson.txt", @"_archive\LeParisien_v1_01\le_parisien_out_bson_correction.txt",
  @"_archive\LeParisien_v1_01\le_parisien_out_bson_corrected.txt");

//Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrintFromMongo(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2), "le_parisien",
//  "{ 'download.title': /.*parisien.*/i }", limit: 0, sort: "{ 'download.title': 1 }");
//Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrintFromMongo(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "le_parisien",
//  "{ 'download.title': /.*parisien.*/i }", limit: 0, sort: "{ 'download.title': 1 }");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("le_parisien_out_bson.txt", @"_archive\LeParisien_v2_01\le_parisien_out_bson.txt", "le_parisien_out_bson_compare.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("le_parisien_out_bson.txt", @"_archive\LeParisien_v2_01\le_parisien_out_bson.txt", "le_parisien_out_bson_compare.txt", options: pb.Data.Mongo.BsonDocumentComparatorOptions.ReturnAll);

pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", @"c:\pib\dev_data\exe\runsource\test_unit\Print\FindPrint\le_monde.txt",
  query: "{ $or: [ { 'download.title': /^le *monde/i }, { 'download.title': /^journal *le *monde/i } ] }",
  fields: "{ '_id': 0 'download.title': 1, 'download.category': 1, 'download.isPrint': 1, 'download.infos': 1 }", limit: 0, sort: "{ 'download.creationDate': -1 }",
  transformDocument: doc => new MongoDB.Bson.BsonDocument { { "title", doc["download"]["title"] }, { "category", doc["download"]["category"] },
  { "isPrint", doc["download"]["isPrint"] }, { "infos", doc["download"]["infos"] } });

Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 1), "le_monde.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2), "le_monde.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "le_monde.txt");
//Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 4), "le_monde.txt");

Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("le_monde_out_bson.txt", @"_archive\LeMonde_v1_01\le_monde_out_bson_corrected.txt", "le_monde_out_bson_compare_v1_01_corrected.txt");

Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Correction(@"_archive\LeMonde_v1_01\le_monde_out_bson.txt", @"_archive\LeMonde_v1_01\le_monde_out_bson_correction.txt",
  @"_archive\LeMonde_v1_01\le_monde_out_bson_corrected.txt");


//Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrintFromMongo(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2), "le_monde",
//  "{ $or: [ { 'download.title': /^le *monde/i }, { 'download.title': /^journal *le *monde/i } ] }", limit: 0, sort: "{ 'download.title': 1 }");
//Test.Test_Unit.Print.Test_Unit_FindPrint.Test_FindPrintFromMongo(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "le_monde",
//  "{ $or: [ { 'download.title': /^le *monde/i }, { 'download.title': /^journal *le *monde/i } ] }", limit: 0, sort: "{ 'download.title': 1 }");

Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("le_monde_out_bson.txt", @"_archive\LeMonde_v1_01\le_monde_out_bson.txt", "le_monde_out_bson_compare_v1_01.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("le_monde_out_bson.txt", @"_archive\LeMonde_v2_01\le_monde_out_bson.txt", "le_monde_out_bson_compare.txt");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_Compare("le_monde_out_bson.txt", @"_archive\LeMonde_v2_01\le_monde_out_bson.txt", "le_monde_out_bson_compare.txt", options: pb.Data.Mongo.BsonDocumentComparatorOptions.ReturnAll);

// ok pb de date "Janv" "post_title" : "Le Monde du Plein Air N?109 - Janv-Fevrier 2013",
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Le Monde du Plein Air N?109 - Janv-Fevrier 2013");
// ok 
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Chez-Soi - Octobre 2014");
// ok 
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "William Katz - Nuits sanglantes");
// ok pb "Dragon Eternity" Ete est pris pour date
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Dragon Eternity");
// ok pb de date  "post_title" : "M Comme Maison N17 Septembre-Octobre 2014",
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "M Comme Maison N17 Septembre-Octobre 2014");
// pb de date (pb de split) "post_title" : "Tatouage Magazine N?100 - Septembre - Octobre 2014",
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Tatouage Magazine N?100 - Septembre - Octobre 2014");

// pb split (du ...)  "post_title" : "Maisons du Fenua N 37",
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Maisons du Fenua N 37");
// pb split (du ...)  "post_title" : "Balilla, les enfants du Duce",
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Balilla, les enfants du Duce");

// pb split entre du et - : "Le Monde du Jeudi - 15 Mai 2014",
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Le Monde du Jeudi - 15 Mai 2014");
// pb split entre du et - : "Le Monde du Vendredi - 03 Octobre 2014",
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Le Monde du Vendredi - 03 Octobre 2014");

// pb split ne trouve pas le num?ro : "01 Informatique - Business et Techno N?2141"
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "01 Informatique - Business et Techno N?2141");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2), "01 Informatique - Business et Techno N?2141");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 1), "01 Informatique - Business et Techno N?2141");

// ok pb print Sud Ouest pas trouv? : Sud Ouest (Rive Gauche) Du Mercredi 10 Septembre 2014
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Sud Ouest (Rive Gauche) Du Mercredi 10 Septembre 2014");
// ok v?rifier G?o et G?o France : G?o France Hors S?rie Best-Seller No.1 - Ao?t/Septembre 2014
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "G?o France Hors S?rie Best-Seller No.1 - Ao?t/Septembre 2014");


// new version pb
// ok  "post_title" : "Le Journal du Dimanche No.3529 - Dimanche 31 Ao?t 2014",
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Le Journal du Dimanche No.3529 - Dimanche 31 Ao?t 2014");
//ok "Le Monde + T?l? & Eco & Entreprise - Dim 30 / Lundi 01 Juillet 2013"
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Le Monde + T?l? & Eco & Entreprise - Dim 30 / Lundi 01 Juillet 2013");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "");
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "");



//ok
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 4), "SUD OUEST bassin d'Arcachon Lundi 8 septembre 2014");
//ok
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 4), "M Comme Maison N17 Septembre-Octobre 2014");

// pb de date  "post_title" : "LA DERNIERE HEURE DU 10-09-2014",
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "LA DERNIERE HEURE DU 10-09-2014");
// pb de date need country manager "Psychologies N? 343 - Septembre 2014 / France"
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Psychologies N? 343 - Septembre 2014 / France");
// pb de date day=84 year=2013 "Ebooks/Magazine" "Le Monde de l'Image 84  - 2013"
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Le Monde de l'Image 84  - 2013");
// pb de date capture="29 - F?vrier-Mars 2013" "Le Monde de L'Intelligence 29 - F?vrier-Mars 2013"
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Le Monde de L'Intelligence 29 - F?vrier-Mars 2013");
// pb de date capture="7 - F?vrier-Mars 2013" mais ok quand meme "Le Monde des Sciences 7 - F?vrier-Mars 2013"
Test.Test_Unit.Print.Test_Unit_FindPrint.Test_OneFindPrint(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "Le Monde des Sciences 7 - F?vrier-Mars 2013");



//Test.Test_Unit.Print.Test_Unit_SelectPost.Test_SelectPost_01(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(), @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\SelectPost.txt");
//Test.Test_Unit.Print.Test_Unit_SelectPost.Test_SelectPost_02(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(), "SelectPost.txt");
//Test.Test_Unit.Print.Test_Unit_SelectPost.Test_SelectPost_02(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2),
//  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\SelectPost.txt");
//Test.Test_Unit.Print.Test_Unit_SelectPost.Test_SelectPost_03(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2), "SelectPost.txt");
//Test.Test_Unit.Print.Test_Unit_SelectPost.Test_SelectPost_04(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2), "SelectPost.txt");
//Test.Test_Unit.Print.Test_Unit_SelectPost.Test_SelectPost_04(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 3), "SelectPost.txt");

//Test.Test_mongodb.Test_Compare_f.Test_Compare_02();
//RunSource.CurrentRunSource.SetResult(pb.Data.Mongo.BsonDocumentsToDataTable_old2.ToDataTable(zmongo.BsonReader<MongoDB.Bson.BsonDocument>(
//  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\test_compare.txt").Where(doc => doc["result"]["result"] == "not equal")));

//pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ 'download.title': /.*parisien.*/i }", limit: 0, sort: "{ 'download.title': 1 }", fields: "{ '_id': 1, 'download.title': 1, 'download.creationDate': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1  }");
//Test.Test_Unit.Print.Test_Unit_SelectPost.Test_SelectPostFromMongo_03(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2),
//  "{ 'download.title': /.*parisien.*/i }", limit: 0, sort: null);
//Test.Test_Unit.Print.Test_Unit_SelectPost.Test_SelectPostFromMongo_04(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2), "le_parisien.txt",
//  "{ 'download.title': /.*parisien.*/i }", limit: 0, sort: null);
//Test.Test_Unit.Print.Test_Unit_SelectPost.Test_SelectPostFromMongo_04(Download.Print.DownloadAutomate_f.CreateDownloadAutomate(version: 2), "le_monde.txt",
//  "{ $or: [ { 'download.title': /^le *monde/i }, { 'download.title': /^journal *le *monde/i } ] }", limit: 0, sort: "{ 'download.title': 1 }");


  
Test.Test_Text.Test_Regex.Test(
  //@"^\s*le\s*parisien\s*(?:(?:\+|et)\s*)?(?:(?:cahier(?:\s*paris)?)|(?:(?:(?:supp\.?l?|suppl[e?]ment)\s*)?(?:[e?]conomie|la\s*parisienne|votre\s*[e?]t[e?]))|(?:ma?gazine)|(?:(?:le\s*)guide(?:des?\s*(?:vos?\s*)?loisirs|de\s*votre|de\s*votre\s*dimanche|du\s*dimanche)?)|(?:(?:le\s*)?journal\s*de\s*paris))?$",
  //@"^\s*le\s*parisien\s*(?:(?:\+|et)\s*)?(?:(?:cahier(?:\s*paris)?)|(?:(?:(?:supp\.?l?|suppl[e?]ment)\s*)?(?:[e?]conomie|la\s*parisienne|votre\s*[e?]t[e?]))|(?:ma?gazine)|(?:(?:le\s*)?guide\s*(?:des?\s*(?:vos?\s*)?loisirs|de\s*votre|de\s*votre\s*dimanche|du\s*dimanche)?)|(?:(?:le\s*)?journal\s*de\s*paris))?$",
  @"^\s*le\s*parisien\s*(?:(?:(?:\+|et)\s*)?(?:(?:cahier(?:\s*paris)?)|(?:(?:(?:supp\.?l?|suppl[e?]ment)\s*)?(?:[e?]conomie|la\s*parisienne|votre\s*[e?]t[e?]))|(?:ma?gazine)|(?:(?:le\s*)?guide\s*(?:des?\s*(?:vos?\s*)?loisirs|de\s*votre|de\s*votre\s*dimanche|du\s*dimanche)?)|(?:(?:le\s*)?journal\s*de\s*paris))\s*)*$",
  "Le parisien + journal de paris et magazine");
// Le parisien + journal de paris + economie
// Le parisien + journal de paris et magazine
// Le parisien
// Le parisien + journal de paris
// Le parisien + economie + cahier paris
// Le parisien + guide de votre dimanche
Test.Test_Text.Test_Regex.Test(
  //@"^(?:(?:le\s*)?guide(?:des?\s*(?:vos?\s*)?loisirs|de\s*votre|de\s*votre\s*dimanche|du\s*dimanche)?)$",
  @"^(?:(?:le\s*)?guide\s*(?:de\s*votre|de\s*votre\s*dimanche|du\s*dimanche)?)$",
  "guide de votre dimanche");

Test.Test_Text.Test_Regex.Test(
  @"^\s*(?:le\s*parisien\s*magazine|le\s*magazine\s*du\s*parisien)$",
  "Le magazine du parisien");




//***********************************************************************************************************************************************************
//****                                                  Test.Test_Unit.Print
//****                                                  Test_Unit_PrintMagazineGetTitle
//****                      c:\pib\dev_data\exe\runsource\test_unit\Print\MagazineTitle\MagazineTitle.txt
//***********************************************************************************************************************************************************
// $$info.test_unit.print

Test_Unit_PrintTitleManager.Test_ExportTitle_TelechargerMagazine_01();
Test_Unit_PrintTitleManager.Test_ExportTitle_Vosbooks_01();
Test_Unit_PrintTitleManager.Test_ExportTitle_Ebookdz_01();

// version 3 = old find date, version 5 = new find date
Test_Unit_PrintTitleManager.Test_GetPrintTitleInfo_01(Download.Print.DownloadAutomate_f.CreatePrintTitleManager(version: 5), "PrintTitle_TelechargerMagazine.txt");
Test_Unit_PrintTitleManager.Test_GetPrintTitleInfo_01(Download.Print.DownloadAutomate_f.CreatePrintTitleManager(version: 5), "PrintTitle_Vosbooks.txt");
Test_Unit_PrintTitleManager.Test_GetPrintTitleInfo_01(Download.Print.DownloadAutomate_f.CreatePrintTitleManager(version: 5), "PrintTitle_Ebookdz.txt");

//string filename = "PrintTitle_TelechargerMagazine";
string filename = "PrintTitle_Vosbooks";
string filename = "PrintTitle_Ebookdz";
BsonDocumentComparator.CompareBsonDocumentFiles(
  @"$TestUnitDirectory$\Print\PrintTitle\".zConfigGetVariableValue() + filename + "_out_bson_old.txt",
  @"$TestUnitDirectory$\Print\PrintTitle\".zConfigGetVariableValue() + filename + "_out_bson.txt",
  comparatorOptions: BsonDocumentComparatorOptions.ReturnNotEqualDocuments | BsonDocumentComparatorOptions.ResultNotEqualElements,
  elementsToCompare: new string[] { "PrintTitleInfo.date", "PrintTitleInfo.dateType", "PrintTitleInfo.file" })
  .Select(result => result.GetResultDocument())
  .zSave(@"$TestUnitDirectory$\Print\PrintTitle\".zConfigGetVariableValue() + filename + "_out_bson_compare.txt");


RunSource.CurrentRunSource.SetResult(pb.Data.Mongo.BsonDocumentsToDataTable_old2.ToDataTable(zmongo.BsonReader<MongoDB.Bson.BsonDocument>(@"c:\pib\dev_data\exe\runsource\test_unit\Print\MagazineTitle\MagazineTitle_out_bson.txt")));
//RunSource.CurrentRunSource.SetResult(pb.Data.Mongo.BsonDocumentsToDataTable_old2.ToDataTable(zmongo.BsonReader<MongoDB.Bson.BsonDocument>(@"c:\pib\dev_data\exe\runsource\test_unit\Print\MagazineTitle\MagazineTitle.txt")));

Test.Test_Unit.Print.Test_Unit_PrintMagazineGetTitle.Test_SplitTitle_01("MagazineTitle.txt");


//***********************************************************************************************************************************************************
//****                                                  Test.Test_Unit.Print
//****                                                  Test_CalculatePrintDateNumber
//****                      c:\pib\dev_data\exe\runsource\test_unit\Print\CalculatePrintDateNumber\
//***********************************************************************************************************************************************************

Test.Test_Unit.Print.Test_Unit_Print.Test_CalculatePrintNumber_01(Download.Print.DownloadAutomate_f.CreatePrintManager()["courrier_international"], @"c:\pib\dev_data\exe\runsource\test_unit", Date.Parse("2013-04-04"), 15);
Test.Test_Unit.Print.Test_Unit_Print.Test_CalculatePrintDate_01(Download.Print.DownloadAutomate_f.CreatePrintManager()["courrier_international"], @"c:\pib\dev_data\exe\runsource\test_unit", 1170, 15);
Test.Test_Unit.Print.Test_Unit_Print.Test_CalculatePrintDateNumber_01(Download.Print.DownloadAutomate_f.CreatePrintManager(), @"c:\pib\dev_data\exe\runsource\test_unit");
Test.Test_Unit.Print.Test_Unit_Print.Test_CalculatePrintDateNumber_02(Download.Print.DownloadAutomate_f.CreatePrintManager(), @"c:\pib\dev_data\exe\runsource\test_unit");
Test.Test_Unit.Print.Test_Unit_Print.Test_CalculatePrintDateNumber_03(Download.Print.DownloadAutomate_f.CreatePrintManager(), @"c:\pib\dev_data\exe\runsource\test_unit");





















Test.Test_Bson.Test_BsonWriter_f.Test_BsonWriter_01();

Http2.LoadUrl(@"c:\pib\dev_data\exe\runsource\download\sites\rapide-ddl\cache\detail\39000\ebooks_magazine_39023-multi-lautomobile-no821-octobre-2014.html");
XXElement xe = new XXElement(Http2.HtmlReader.XDocument.Root).XPathElement("//div[@class='lcolomn mainside']").XPathElement(".//div[@class='maincont']");
Download.Print.RapideDdl.RapideDdl_Exe.Test_XXElement_DescendantTextList_01();

Trace.WriteLine(zdir.GetNewDirectory(@"c:\pib\_dl\_pib\dl\.01_quotidien\journaux\Journaux\Journaux - 2014-09-16"));

Trace.WriteLine("{0} - {1}", Type.GetTypeCode(typeof(int)), (int)Type.GetTypeCode(typeof(int)));
Trace.WriteLine("{0} - {1}", Type.GetTypeCode(typeof(long)), (int)Type.GetTypeCode(typeof(long)));
Trace.WriteLine("{0} - {1}", Type.GetTypeCode(typeof(Console)), (int)Type.GetTypeCode(typeof(Console)));
Trace.WriteLine("{0} - {1}", Type.GetTypeCode(typeof(DateTime)), (int)Type.GetTypeCode(typeof(DateTime)));
Trace.WriteLine("{0} - {1}", Type.GetTypeCode(typeof(Date)), (int)Type.GetTypeCode(typeof(Date)));
Trace.WriteLine("{0} - {1}", (TypeCode)9, (int)(TypeCode)9);
Trace.WriteLine("{0} - {1}", (TypeCode)20, (int)(TypeCode)20);
Trace.WriteLine("{0} - {1}", typeof(int?), Nullable.GetUnderlyingType(typeof(int?)));
int value = 123456;
int i = __refvalue(__makeref(value), int);
//int i = __refvalue(__makeref(value), typeof(int));
Trace.WriteLine("{0}", i);
int? value = 123456;
int? i = __refvalue(__makeref(value), int?);
Trace.WriteLine("{0}", i);
int? value = 123456;
int i = __refvalue(__makeref(value), int);
Trace.WriteLine("{0}", i);

// $$title$$ $$special$$  $$number$$ - $$date$$ 
// good one
Test.Test_Text.Test_Regex.Test(@"\$\$([a-z]+)\$\$(.*?)(?=\$\$|$)", "$$title$$ title @title $$special$$ special @special $$number$$ number - number $$date$$ date date");
Test.Test_Text.Test_Regex.Test(@"\$\$[a-z]+\$\$", "$$title$$ title @title $$special$$ special @special $$number$$ number - number $$date$$ date date");

Test.Test_Text.Test_Regex.Test(@"^((\$\$[a-z]+\$\$)([^\$]*))*$", "$$title$$ title @title $$special$$ special @special $$number$$ number - number $$date$$ date date");

Test.Test_Text.Test_Regex.Test(@"((\$\$[a-z]+\$\$)([^\$]*))*", "$$title$$ title title $$special$$ special special $$number$$ number - number $$date$$ date date");
Test.Test_Text.Test_Regex.Test(@"^((\$\$[a-z]+\$\$)(.*))*$", "$$title$$ title title $$special$$ special special $$number$$ number - number $$date$$ date date");
Test.Test_Text.Test_Regex.Test(@"(\$\$[a-z]+\$\$)([^\$]*)", "$$title$$ title title $$special$$ special special $$number$$ number - number $$date$$ date date");
Test.Test_Text.Test_Regex.Test(@"(\$\$[a-z]+\$\$)", "$$title$$ title title $$special$$ special special $$number$$ number - number $$date$$ date date");
Test.Test_Text.Test_Regex.Test(@"(\$\$[a-z]+\$\$)*", "$$title$$ title title $$special$$ special special $$number$$ number - number $$date$$ date date");
Test.Test_Text.Test_Regex.Test(@"([a-z]+)*", "$$title$$ title title $$special$$ special special $$number$$ number - number $$date$$ date date");



// Le Parisien + Journal de Paris
// good one
Test.Test_Text.Test_Regex.Test(@"^(?:([^\s\-\+_]+)(?:[\s\-\+_]+|$))*$", "Best-Of Android Mobiles  et  Tablettes");
Test.Test_Text.Test_Regex.Test(@"^(?:([^\s\-\+_]+)(?:[\s\-\+_]+|$))*$", "Le Parisien + Journal de Paris");
Test.Test_Text.Test_Regex.Test(@"^(?:([^\s]+)(?:\s+|$))*$", "Le Parisien + Journal de Paris");
Test.Test_Text.Test_Regex.Test(@"([^\s]+)", "Le Parisien + Journal de Paris");
Test.Test_Text.Test_Regex.Test(@"(([^\s]+)(\s+|$))*", "Le Parisien + Journal de Paris");
Test.Test_Text.Test_Regex.Test(@"^(([^\s]+)(\s+|$))*$", "Le Parisien + Journal de Paris");
Test.Test_Text.Test_Regex.Test(@"^([^\s]+)*$", "Le Parisien + Journal de Paris");
Test.Test_Text.Test_Regex.Test(@"^\s*elle(?:\s*france)?$", "Elle france");

Trace.WriteLine(RunSource.CurrentRunSource.Config.Get("FindPrints/Dates/DateNew/@regex"));

Test.Test_Text.Test_Regex.Test(@"(?:^|[_\s])(?:du\s*)?(?:(?:(?:lundi|mardi|mercre?di|jeudi|vendredi|samedi|dimanche)|([0-9]{4})|([0-9]{1,2}|1er)|(janvier|f[e?]vrier|mars|avril|mai|juin|juillet|juill|a[o0][u?]t?|septembre|sptembre|octobre|novembre|d[e?]cembre|january|february|march|april|may|june|july|august|september|october|november|december)|(?:au|et|/|&))[_\.,\-/\\\s]*)+",
  //"Tout Comprendre No.25 - Octobre 2014");
  //"L'Equipe du lundi 22 septembre 2014");
  //"Hifi Video Home Cinema N?419 - Septembre-Octobre 2014");
  //"Investir No.2124 - Du 20 au 26 Septembre 2014");
  "LES JOURNAUX -  DIMANCHE 21 / 22 SEPTEMBRE 2014");

Test.Test_Unit.Text.Test_Unit_Regex.Test_Regex_01(@"(?:^|[_\s])(?:du\s*)?(?:(?:(?:lundi|mardi|mercre?di|jeudi|vendredi|samedi|dimanche)|([0-9]{4})|([0-9]{1,2}|1er)|(janvier|f[e?]vrier|mars|avril|mai|juin|juillet|juill|a[o0][u?]t?|septembre|sptembre|octobre|novembre|d[e?]cembre|january|february|march|april|may|june|july|august|september|october|november|december)|(?:au|et|/|&))[_\.,\-/\\\s]*)+",
  "RegexValues_FindDate.txt");
Test.Test_Unit.Text.Test_Unit_Regex.TraceDetailRegexMatches("RegexValues_FindDate_matches_detail_bson.txt", "RegexValues_FindDate_matches_detail_trace.txt");
Test.Test_Unit.Text.Test_Unit_Regex.TraceCompactRegexMatches("RegexValues_FindDate_matches_compact_bson.txt", "RegexValues_FindDate_matches_compact_trace.txt");
RunSource.CurrentRunSource.View();


Http2.LoadUrl(@"c:\pib\dev_data\exe\runsource\download\sites\rapide-ddl\cache\detail\39000\ebooks_magazine_39023-multi-lautomobile-no821-octobre-2014.html");

string date = "25-09-2014, 23:25";
string date = "5-9-2014, 23:25";
DateTime dt;
if (DateTime.TryParseExact(date, @"d-M-yyyy, HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
  PB_Util.Trace.WriteLine("date time \"{0}\"", dt);
else
  PB_Util.Trace.WriteLine("unknow date time \"{0}\"", date);

Test.Test_Basic.Test_CollectionsConcurrent.Test_ConcurrentDictionary_01();
Test.Test_Basic.Test_CollectionsConcurrent.Test_ConcurrentDictionary_02();
Test.Test_Basic.Test_CollectionsConcurrent.Test_ConcurrentDictionary_03();
Test.Test_Basic.Test_CollectionsConcurrent.Test_ConcurrentDictionary_04();
MongoDB.Bson.BsonValue value = MongoDB.Bson.BsonValue.Create(null);
Trace.WriteLine(value == null ? "$$null$$" : value.ToString());
MongoDB.Bson.BsonDocument doc = MongoDB.Bson.BsonDocument.Parse("{ toto: 'tata', toto2: null }");
MongoDB.Bson.BsonValue value = doc["toto2"];
Trace.WriteLine(value == null ? "$$null$$" : value.ToString());
MongoDB.Bson.BsonValue value1 = MongoDB.Bson.BsonValue.Create("toto");
//MongoDB.Bson.BsonValue value2 = MongoDB.Bson.BsonValue.Create("toto");
//MongoDB.Bson.BsonValue value2 = MongoDB.Bson.BsonValue.Create(10);
MongoDB.Bson.BsonValue value2 = null;
Trace.WriteLine(value1 == value2 ? "equal" : "not equal");

pb.Data.Mongo.BsonDocumentComparator.CompareBsonDocumentFilesToFile(@"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\SelectPost_out_bson.txt",
@"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\_archive\26\SelectPost_out_bson.txt", @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\SelectPost_compare.txt");
pb.Data.Mongo.BsonDocumentComparator.CompareBsonDocumentFiles(
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\SelectPost_out_bson.txt",
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\_archive\26\SelectPost_out_bson.txt").zSaveToJsonFile(
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\SelectPost_compare.txt");
RunSource.CurrentRunSource.SetResult(pb.Data.Mongo.BsonDocumentComparator.CompareBsonDocumentFiles(
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\SelectPost_out_bson.txt",
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\_archive\26\SelectPost_out_bson.txt").zToDataTable2());
RunSource.CurrentRunSource.SetResult();

Test.Test_CS.Test_CallerInformation_f.Test_CallerInformation_01("test");
Test.Test_CS.Test_ExpressionTrees.Test_ExpressionTrees_f_01.Test_ExpressionTrees_01();
Test.Test_CS.Test_ExpressionTrees.Test_ExpressionTrees_f_01.Test_ExpressionTrees_02();
Test.Test_CS.Test_ExpressionTrees.Test_ExpressionTrees_f_01.Test_ExpressionTrees_03();

Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_InnerJoin_ProductCategory_LinqRequest_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_InnerJoin_ProductCategory_LinqMethod_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_InnerJoin_CategoryProduct_LinqRequest_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_InnerJoin_CategoryProduct_LinqMethod_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_GroupJoin_ProductCategory_LinqRequest_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_GroupJoin_ProductCategory_LinqMethod_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_GroupJoin_CategoryProduct_LinqRequest_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_LeftOuterJoin_ProductCategory_LinqRequest_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_LeftOuterJoin_ProductCategory_LinqRequest_Expression_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_LeftOuterJoin_ProductCategory_LinqMethod_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_LeftOuterJoin_ProductCategory_LinqRequest_02();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_LeftOuterJoin_ProductCategory_LinqMethod_02();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_LeftOuterJoin_CategoryProduct_LinqRequest_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_LeftOuterJoin_CategoryProduct_LinqRequest_02();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_FullOuterJoin_ProductCategory_LinqRequest_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_FullOuterJoin_ProductCategory_LinqRequest_02();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_FullOuterJoin_ProductCategory_LinqMethod_01();
Test.Test_CS.Test_Linq.Test_Linq_Join_f.Test_Linq_FullOuterJoin_ProductCategory_LinqExtension_01();

Test.Test_mongodb.Test_Compare_f.Test_InnerJoin_01(
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\SelectPost_out_bson.txt",
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\_archive\26\SelectPost_out_bson.txt",
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\test_bson.txt");

pb.Data.Mongo.BsonDocumentComparator.CompareBsonDocumentFilesWithKey(
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\SelectPost_out_bson.txt",
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\_archive\26\SelectPost_out_bson.txt",
  "postTitle", "postTitle",
  joinType: pb.Linq.JoinType.FullOuterJoin
  //where: doc => doc["document1"]["file"] != null || doc["document2"]["file"] != null
  ).zSaveToJsonFile(@"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\test_compare.txt");
  
Test.Test_mongodb.Test_Compare_f.Test_Compare_02();
RunSource.CurrentRunSource.SetResult(pb.Data.Mongo.BsonDocumentsToDataTable_old2.ToDataTable(zmongo.BsonReader<MongoDB.Bson.BsonDocument>(@"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\test_compare.txt")));
RunSource.CurrentRunSource.SetResult(pb.Data.Mongo.BsonDocumentsToDataTable_old2.ToDataTable(zmongo.BsonReader<MongoDB.Bson.BsonDocument>(
  @"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\test_compare.txt").Where(doc => doc["result"]["result"] == "not equal")));

// 884
Trace.WriteLine("{0}", zmongo.BsonReader<MongoDB.Bson.BsonDocument>(@"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\SelectPost_out_bson.txt").Count());
// 916
Trace.WriteLine("{0}", zmongo.BsonReader<MongoDB.Bson.BsonDocument>(@"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\_archive\26\SelectPost_out_bson.txt").Count());
// 882   922 full
Trace.WriteLine("{0}", zmongo.BsonReader<MongoDB.Bson.BsonDocument>(@"c:\pib\dev_data\exe\runsource\test_unit\Print\SelectPost\test_compare.txt").Count());


MongoDB.Bson.BsonDocument document1 = new MongoDB.Bson.BsonDocument { { "test", "test" } };
MongoDB.Bson.BsonDocument document2 = null;
MongoDB.Bson.BsonDocument doc = new MongoDB.Bson.BsonDocument { { "document1", document1 }, { "document2", document2 } };
Trace.WriteLine(doc.zToJson());
Trace.WriteLine(new MongoDB.Bson.BsonDocument { { "document1", null } }.zToJson());


Test.Test_Unit.Linq.Test_Unit_Join.CreateBasicFiles();
Test.Test_Unit.Linq.Test_Unit_Join.ViewFile("Products.txt");
Test.Test_Unit.Linq.Test_Unit_Join.ViewFile("Categories.txt");
Test.Test_Unit.Linq.Test_Unit_Join.Test();
Test.Test_Unit.Linq.Test_Unit_Join.ViewFile("Products_Categories_InnerJoin.txt");
Test.Test_Unit.Linq.Test_Unit_Join.ViewFile("Products_Categories_LeftOuterJoin.txt");
Test.Test_Unit.Linq.Test_Unit_Join.ViewFile("Products_Categories_RightOuterJoin.txt");
Test.Test_Unit.Linq.Test_Unit_Join.ViewFile("Products_Categories_FullOuterJoin.txt");
Test.Test_Unit.Linq.Test_Unit_Join.ViewFile("Products_Categories_LeftOuterJoinWithoutInner.txt");
Test.Test_Unit.Linq.Test_Unit_Join.ViewFile("Products_Categories_RightOuterJoinWithoutInner.txt");
Test.Test_Unit.Linq.Test_Unit_Join.ViewFile("Products_Categories_FullOuterJoinWithoutInner.txt");


Test.Test_Unit.Mongo.Test_Unit_BsonDocumentComparator.CreateBasicFiles();
Test.Test_Unit.Mongo.Test_Unit_BsonDocumentComparator.Test();
Test.Test_Unit.Mongo.Test_Unit_BsonDocumentComparator.ViewFile("Products1.txt");
Test.Test_Unit.Mongo.Test_Unit_BsonDocumentComparator.ViewFile("Products_Compare_InnerJoin.txt");

pb.Data.Mongo.BsonDocumentComparator.CompareBsonDocumentFilesWithKey(
  @"",
  @"",
  key1, key2, joinType).zSaveToJsonFile(resultFile);

  
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail2", "{ 'download.title': /.*journaux.*/i }", limit: 0, sort: "{ 'download.creationDate': -1 }", fields: "{ '_id': 1, 'download.title': 1, 'download.creationDate': 1, 'download.category': 1, 'download.isPrint': 1, 'download.sourceUrl': 1, 'download.downloadLinks': 1  }");

Test.Test_Bson.Test_Bson_f2.Test_StringArray_01();
Test.Test_Bson.Test_Bson_f2.Test_ZValue_01();
Test.Test_Bson.Test_Bson_f2.Test_ZValue_02();
Test.Test_Bson.Test_Bson_f2.Test_EnumAsString_01(setEnumConvention: false);
Test.Test_Bson.Test_Bson_f2.Test_EnumAsString_01(setEnumConvention: true);
Test.Test_Bson.Test_Bson_f2.Test_EnumDerialization_01("{ 'enum_01' : 3 }");
Test.Test_Bson.Test_Bson_f2.Test_EnumDerialization_01("{ 'enum_01' : 'Value3' }");
Test.Test_Bson.Test_Bson_f2.Test_RapideDdl_PostDetail_01();

Test.Test_cpp.Test_cpp_err_f.Test_cpp_err_01(@"c:\pib\_dl\dev\Mega\mega.co.nz\mega-sdk2.net\sqlite3\err.txt");
Test.Test_cpp.Test_cpp_err_f.Test_cpp_err_patch_01(
  @"c:\pib\_dl\dev\Mega\mega.co.nz\mega-sdk2.net\sqlite3\sqlite3.c",
  @"c:\pib\_dl\dev\Mega\mega.co.nz\mega-sdk2.net\sqlite3\sqlite3_99.c",
  @"c:\pib\_dl\dev\Mega\mega.co.nz\mega-sdk2.net\sqlite3\err.txt");

Test.Test_Text.Test_Regex.Test(
  @"([^\s\(]+)\(",
  "error C2664: 'void sqlite3VdbeChangeP4(Vdbe *,int,const char *,int)' : cannot convert argument 3 from 'void *' to 'const char *'");

// User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36
Http2.LoadUrl("http://rapide-ddl.com/ebooks/");
Http2.LoadUrl("http://rapide-ddl.com/ebooks/", new HttpRequestParameters { userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36" });

Http2.LoadUrl("http://www.free-telechargement.org/1/categorie-Magazines/");
Trace.WriteLine(Trace.CurrentTrace.TraceDir);
Trace.CurrentTrace.TraceDir = @"c:\pib\dev_data\exe\runsource\download\http";

// /xml[1]/html[1]/body[1]/div[4]/div[3]
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='content']:.:EmptyRow");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='content']//table:.:EmptyRow", "@style");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='content']//table//a:.:EmptyRow", "@href");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@class='pagination']//a[starts-with(text(), 'suiv ')]:.:EmptyRow", "@href");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@class='pagination']//a[starts-with(text(), 'suiv ')]/@href:.:EmptyRow");


Http2.LoadUrl("http://www.free-telechargement.org/magazines/43247-pc-update-no10-mars-avril-2004-pdf.html");


Http2.LoadUrl("http://www.golden-ddl.net/ebooks/");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='dle-content']//div[@class='base']:.:EmptyRow");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='dle-content']//div[@class='base']:.:EmptyRow", ".//div[@class='bheading']//a/@href");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@class='basenavi']//span[@class='nnext']//a/@href:.:EmptyRow");


Http2.LoadUrl("http://www.golden-ddl.net/ebooks/magazines/51974-les-journaux-mercredi-29-30-octobre-2014-pdflien-direct.html");



// @"d-M-yyyy, HH:mm"
//string date = "25-10-2014";
string date = "25-octobre-2014";
string date = "25-10-2014";
string date = "25-october-2014";
DateTime dt;
if (DateTime.TryParseExact(date, "d-MMMM-yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
  Trace.WriteLine("{0:d-M-yyyy}", dt);
else
  Trace.WriteLine("error date");

string date = "25-10-2014";
string date = "25-october-2014";
string date = "24 October 2014";
Trace.WriteLine("{0:d-M-yyyy}", zdate.ParseDateTimeLikeToday(date, DateTime.Now, "d M yyyy", "d MMMM yyyy"));


//pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail3.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "RapideDdl_Detail3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\test_export_RapideDdl_Detail2.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail3.txt"), sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Update("dl", "RapideDdl_Detail3", "{}", "{ $rename: { 'download.postType': 'download.printType' } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
//pb.Data.Mongo.TraceMongoCommand.Update("dl", "RapideDdl_Detail2", "{}", "{ $rename: { 'download.originalTitle': 'download2' } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: true);
//pb.Data.Mongo.TraceMongoCommand.Import("dl", "RapideDdl_Detail3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\RapideDdl_Detail2\export_RapideDdl_Detail2_19.txt"));
pb.Data.Mongo.TraceMongoCommand.Import("dl", "RapideDdl_Detail3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\test_export_RapideDdl_Detail2_19.txt"));
pb.Data.Mongo.TraceMongoCommand.Update("dl", "RapideDdl_Detail3", "{ 'download.postType': { $exists: true } }", "{ $rename: { 'download.postType': 'download.printType' } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail3", "{ 'download.postType': { $exists: true } }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail3", "{ $and: [ { 'download.postType': { $exists: true } }, { 'download.printType': { $exists: true } } ] }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail3", "{ $and: [ { 'download.postType': { $exists: false } }, { 'download.printType': { $exists: false } } ] }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "RapideDdl_Detail3", "{ $and: [ { 'download.postType': { $exists: false } }, { 'download.printType': { $exists: true } } ] }");
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.DocumentStore.UpdateDocuments(post => post.id);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.DocumentStore.UpdateDocuments(post => { Trace.WriteLine("update post {0} {1}", post.id, post.title); return post.id; });

pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail2.txt"), sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Update("dl", "RapideDdl_Detail2", "{ 'download.postType': { $exists: true } }", "{ $rename: { 'download.postType': 'download.printType' } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.DocumentStore.UpdateDocuments(post => post.id);

Test.Test_Bson.Test_Bson_f.Test_BsonReader_01(@"c:\pib\dev_data\exe\runsource\download\pcap\rapide-ddl.com_02.har");

Trace.WriteLine("DateTime {0:dd-MM-yyyy HH:mm:ss K zz}", DateTime.Now);
Trace.WriteLine("DateTime {0:dd-MM-yyyy HH:mm:ss K}", DateTime.Now);
Trace.WriteLine("DateTime {0:u}", DateTime.Now);
// time from rapide-ddl.com_02.har "2014-11-02T10:33:59.394Z" => "2014-11-02 11:33:59.394"
Trace.WriteLine("DateTime {0:dd-MM-yyyy HH:mm:ss K}", DateTime.Parse("2014-11-02 18:24:33"));
Trace.WriteLine("DateTime {0:dd-MM-yyyy HH:mm:ss K}", DateTime.Parse("2014-11-02 18:24:33 +01:00"));
Trace.WriteLine("DateTime {0:dd-MM-yyyy HH:mm:ss K}", DateTime.Parse("2014-11-02T10:33:59.394Z"));

Test.Test_Unit.Web.Test_Unit_HttpArchive.Test_01(@"c:\pib\dev_data\exe\runsource\download\pcap\rapide-ddl.com_02.har");
Test.Test_Unit.Web.Test_Unit_HttpArchive.Test_02(@"c:\pib\dev_data\exe\runsource\download\pcap\rapide-ddl.com_02.har");
Test.Test_Unit.Web.Test_Unit_HttpArchive.Test_Cookie_01();
Test.Test_Unit.Web.Test_Unit_HttpArchive.Test_GZipDecompress_01(@"c:\pib\dev_data\exe\runsource\download\http\2382_rapide-ddl.com_ebooks_.html.gzip", @"c:\pib\dev_data\exe\runsource\download\http\2382_rapide-ddl.com_ebooks_.html");
Test.Test_Unit.Web.Test_Unit_HttpRequest.Test_01(@"c:\pib\dev_data\exe\runsource\download\pcap\rapide-ddl.com_02.har", executeRequest: true);
Test.Test_Unit.Web.Test_Unit_HttpRequest.Test_WebRequest_01();
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadUrl_01("http://rapide-ddl.com/ebooks/");
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadUrl_01("http://www.rapide-ddl.com/ebooks/magazine/41287-science-vie-n1139.html");

Http2.LoadUrl("http://www.golden-ddl.net/ebooks/");
Http2.LoadUrl("http://rapide-ddl.com/ebooks/");
Http2.LoadUrl("http://www.rapide-ddl.com/ebooks/magazine/41287-science-vie-n1139.html");

Uri uri = new Uri("http://rapide-ddl.com/ebooks/");
Trace.WriteLine("Authority \"{0}\" DnsSafeHost \"{1}\"", uri.Authority, uri.DnsSafeHost);


Test.Test_Trace.Test_Trace_01();

Download.Print.RapideDdl.RapideDdl_LoadHeaderPages.Load(100, maxPage: 10, reload: true).Count();

Download.Print.GoldenDdl.GoldenDdl_LoadHeaderPages_new.CurrentLoadHeaderPages.LoadPages();



Uri uri = new Uri("http://www.golden-ddl.net/ebooks/page/2/");
Trace.WriteLine(uri.Segments[uri.Segments.Length - 1]);

Download.Print.GoldenDdl.GoldenDdl_Exe.Test_GoldenDdl_LoadHeaderPages_01(startPage: 1, maxPage: 1, reload: false, loadImage: false, refreshDocumentStore: false);
Download.Print.GoldenDdl.GoldenDdl_Exe.Test_GoldenDdl_LoadHeaderPages_01(startPage: 1, maxPage: 3, reload: true, loadImage: false, refreshDocumentStore: false);



pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail2.txt"), sort: "{ 'download.creationDate': -1 }");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail3.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "RapideDdl_Detail3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\RapideDdl_Detail2\export_RapideDdl_Detail2_21.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail3.txt"), sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "RapideDdl_Detail3");
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 1000, reloadFromWeb: false, loadImage: false);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);

pb.Data.Mongo.TraceMongoCommand.Export("dl", "GoldenDdl_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\golden-ddl\export_GoldenDdl_Detail.txt"), sort: "{ 'download.creationDate': -1 }");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.GoldenDdl_Detail2.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "GoldenDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\golden-ddl\GoldenDdl_Detail\export_GoldenDdl_Detail_06.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "GoldenDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\golden-ddl\export_GoldenDdl_Detail2.txt"), sort: "{ 'download.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "GoldenDdl_Detail2");
Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 20, reloadFromWeb: false, loadImage: false);
Download.Print.GoldenDdl.GoldenDdl_LoadPostDetail.CurrentLoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);

Trace.WriteLine(string.Format("{{ id: {0} }}", 123));

Trace.WriteLine(Directory.GetFiles("", "", SearchOption.AllDirectories).zToStringValues());
Trace.WriteLine(Directory.GetFiles(@"c:\pib\_dl\_pib\dl\rapide-ddl.com\book\10 cl?s pour r?ussir sa certification qse _ iso 9001 _\10 cl?s pour r?ussir sa certification qse _ iso 9001 _ - 2008").zToStringValues("\r\n"));
Trace.WriteLine(Directory.GetDirectories(@"c:\pib\_dl\_pib\dl\rapide-ddl.com\book\10 cl?s pour r?ussir sa certification qse _ iso 9001 _\10 cl?s pour r?ussir sa certification qse _ iso 9001 _ - 2008").zToStringValues("\r\n"));

Trace.WriteLine(zdir.EnumerateDirectories(@"c:\pib\_dl\_pib\dl\rapide-ddl.com\print", maxLevel: 1).zToStringValues("\r\n"));
Trace.WriteLine(zdir.EnumerateDirectories(@"c:\pib\_dl\_pib\dl\rapide-ddl.com\print", maxLevel: 2).zToStringValues("\r\n"));
Trace.WriteLine(zdir.EnumerateDirectories(@"c:\pib\_dl\_pib\dl\rapide-ddl.com\print", minLevel: 2, maxLevel: 2).zToStringValues("\r\n"));

zdir.DeleteEmptyDirectory(@"c:\pib\_test\test");
zdir.DeleteEmptyDirectory(@"c:\pib\_test\test", deleteOnlySubdirectory: false);
new DirectoryInfo(@"c:\pib\_test\test").Delete();
new DirectoryInfo(@"c:\pib\_test\test\b").Delete();

Download.Print.DownloadAutomate_f.CreatePrintFileManager().ManageDirectory(@"c:\pib\_dl\_pib\dl\rapide-ddl.com\book", 1);
//Download.Print.DownloadAutomate_f.CreatePrintFileManager().ManageDirectory(@"c:\pib\_dl\_pib\dl\_test\10 cl?s pour r?ussir sa certification qse _ iso 9001 _");
//Download.Print.DownloadAutomate_f.CreatePrintFileManager().ManageDirectory(@"c:\pib\_dl\_pib\dl\_test\Collection de 775 essais, romans histoire format epub");
Trace.WriteLine(zpath.PathGetFilenameWithoutNumber("Le parisien - 2014-11-11 - no 21827(1).pdf"));
Trace.WriteLine(zpath.PathGetFilenameWithoutNumber("Le parisien - 2014-11-11 - no 21827[1].pdf"));
Trace.WriteLine(zpath.PathGetFilenameWithoutNumber("Le parisien - 2014-11-11 - no 21827.pdf"));
Trace.WriteLine("{0}", zfile.AreFileEqual(@"c:\pib\_dl\_pib\dl\golden-ddl.net\print\.01_quotidien\Le parisien\Le parisien - 2014-11-15 - no 21831.pdf", @"c:\pib\_dl\_pib\dl\golden-ddl.net\print\.01_quotidien\Le parisien\Le parisien - 2014-11-15 - no 21831(1).pdf"));























//************************************************************************************************************************************************************************************
//**
//**
//**                                                 Manage print directory
//**
//**
//************************************************************************************************************************************************************************************
// $$info.manage.print.directory

// manage new print
string[] directories = new string[] {
	@"g:\pib\media\ebook\_dl\_dl_pib\print\07\print"
	};
DownloadAutomate_f.Test_ManageDirectories_01(directories, @"g:\pib\media\ebook\print", usePrintDirectories: true, simulate: false, moveFiles: true);

// manage new book
string[] directories = new string[] {
	@"g:\pib\media\ebook\_dl\_dl_pib\book\01\book"
	};
DownloadAutomate_f.Test_ManageDirectories_01(directories, @"g:\pib\media\ebook\book\unsorted", bonusDirectory: @"g:\pib\media\ebook\book\bonus",
	usePrintDirectories: false, simulate: false, moveFiles: true);

// manage new Journaux
DownloadAutomate_f.Test_RenamePrintFiles_01(@"g:\pib\media\ebook\_dl\_dl_pib\journaux\03\print", @"g:\pib\media\ebook\Journaux", simulate: true, version: 5);
DownloadAutomate_f.Test_RenamePrintFiles_01(@"g:\pib\media\ebook\_dl\_dl_pib\journaux\02\print", @"g:\pib\media\ebook\Journaux", simulate: false, version: 5);
DownloadAutomate_f.Test_ManageDirectories_01(new string[] { @"g:\pib\media\ebook\Journaux\print" }, @"g:\pib\media\ebook\print", usePrintDirectories: true, simulate: false, moveFiles: true);


//************************************************************************************************************************************************************************************
//************************************************************************************************************************************************************************************
//************************************************************************************************************************************************************************************


DownloadAutomate_f.Test_GetDirectoryInfo_01(@"g:\pib\media\ebook\_dl\_dl_pib\book\01\book", excludeBonusDirectory: true);
DownloadAutomate_f.Test_GetDirectoryInfo_01(@"g:\pib\media\ebook\_dl\_dl_pib\book\01\book", excludeBonusDirectory: false);


string[] directories = new string[] {
	@"g:\pib\media\ebook\Journaux\print"
	};
DownloadAutomate_f.Test_ManageDirectories_01(directories, @"g:\pib\media\ebook\print", usePrintDirectories: true, simulate: false, moveFiles: true);

// test print
string[] directories = new string[] {
	@"g:\pib\media\ebook\_dl\_test\_dl\print\01\print"
	};
DownloadAutomate_f.Test_ManageDirectories_01(directories, @"g:\pib\media\ebook\_dl\_test\print\", usePrintDirectories: true,
	simulate: false, moveFiles: true);

string[] directories = new string[] {
	@"g:\pib\media\ebook\_dl\_test\_dl\book\01\book"
	//@"g:\pib\media\ebook\_dl\_test\_dl\book\02\book"
	};
DownloadAutomate_f.Test_ManageDirectories_01(directories, @"g:\pib\media\ebook\_dl\_test\book\", bonusDirectory: @"g:\pib\media\ebook\_dl\_test\bonus", usePrintDirectories: false,
	simulate: false, moveFiles: true);


DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\print", @"c:\pib\_dl\_pib\dl\print" }).zTraceJson();
DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\print", @"c:\pib\_dl\_pib\dl\print" }).zView();
DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" }).zTraceJson();
DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[".02_hebdo\\01 net"].zTraceJson();


PrintFileManager.GetFileGroups(DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\print", @"c:\pib\_dl\_pib\dl\print" })[".01_quotidien\\Journaux"]).zTraceJson();
PrintFileManager.GetFileGroups(DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[".02_hebdo\\01 net"]).zTraceJson();
PrintFileManager.GetFileGroups(DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[".02_hebdo\\01 net"]).zView();

string[] directories = new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" };
string subDirectory = @".03_mensuel\Alternatives économiques";
DownloadAutomate_f.Test_ManageDirectoryGroup_01(directories, subDirectory, simulate: true, moveFiles: true);

XmlConfig.CurrentConfig.GetConfig("PrintList2Config").Get("FindPrints/Prints/Print/@name").zTrace();
XmlConfig.CurrentConfig.GetConfig("PrintList2Config").Get("FindPrints/Prints/Print[@name='le_monde']/@regex").zTrace();

//string subDirectory = @".03_mensuel\60 millions de consommateurs";
//string subDirectory = @".03_mensuel\Afrique magazine";
//string subDirectory = @".03_mensuel\Alternatives économiques";
//string subDirectory = @".03_mensuel\Android mobiles et tablettes";
//string subDirectory = @".03_mensuel\Beaux arts magazine";
string[] subDirectories = new string[] {
	@".03_mensuel\Canard PC hardware",
	@".03_mensuel\Capital",
	@".03_mensuel\Cerveau et psycho",
	@".03_mensuel\Ciel et espace",
	@".03_mensuel\Classica",
	@".03_mensuel\Connaissance des arts",
	@".03_mensuel\DownLoad"
	};
string[] subDirectories = new string[] {
	@".03_mensuel\F1 racing",
	@".03_mensuel\Géo",
	@".03_mensuel\Jeux vidéo magazine",
	@".03_mensuel\L'art de l'aquarelle",
	@".03_mensuel\National geographic",
	@".03_mensuel\Pianiste",
	@".03_mensuel\Pirate informatique",
	@".03_mensuel\Plume",
	@".03_mensuel\Pour la science",
	@".03_mensuel\Première",
	@".03_mensuel\Psychologies magazine",
	@".03_mensuel\Que choisir",
	@".03_mensuel\Questions internationales",
	@".03_mensuel\Science et vie",
	@".03_mensuel\Sciences et avenir"
	};

string[] directories = new string[] {
	@"g:\pib\media\ebook\print",
	@"g:\pib\media\ebook\_dl\_dl_pib\01\print",
	@"g:\pib\media\ebook\_dl\_dl_pib\02\print",
	@"g:\pib\media\ebook\_dl\_dl_pib\03\print",
	@"g:\pib\media\ebook\_dl\_dl_pib\04\print",
	@"g:\pib\media\ebook\_dl\_dl_pib\05\print",
	@"g:\pib\media\ebook\_dl\_dl_pib\06\print",
	@"g:\pib\media\ebook\_dl\_dl_pib\07\print",
	@"g:\pib\media\ebook\_dl\_dl_pib\08\print"
	};
string[] subDirectories = new string[] {
	@".02_hebdo\01 net",
	@".02_hebdo\Challenges",
	@".02_hebdo\Courrier international",
	@".02_hebdo\France football",
	@".02_hebdo\Le journal du dimanche",
	@".02_hebdo\Le nouvel observateur",
	@".02_hebdo\Le point",
	@".02_hebdo\Les inrockuptibles",
	@".02_hebdo\L'express",
	@".02_hebdo\Marianne",
	@".02_hebdo\Valeurs actuelles"
	};
DownloadAutomate_f.Test_ManageDirectoryGroup_01(directories, subDirectories, simulate: false, moveFiles: true);

// ok print
string[] directories = new string[] {
	@"g:\pib\media\ebook\print",
	//@"g:\pib\media\ebook\_dl\_dl_pib\print\02\print",
	@"g:\pib\media\ebook\_dl\_dl_pib\print\03\print"
	};
string[] subDirectories = new string[] {
	".01_quotidien",
	".02_hebdo",
	".03_mensuel"
	//".04_autre",
	//".05_new_print",
	//".06_unknow_print"
	};
DownloadAutomate_f.Test_ManageDirectoryGroup_02(directories, subDirectories, usePrintDirectories: true, simulate: false, moveFiles: true);

// ok book
string[] directories = new string[] {
	@"g:\pib\media\ebook\book",
	@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book"
	};
DownloadAutomate_f.Test_ManageDirectoryGroup_03(directories, bonusDirectory: @"g:\pib\media\ebook\bonus", usePrintDirectories: false, simulate: false, moveFiles: true);

string[] directories = new string[] {
	@"g:\pib\media\ebook\_test\book",
	@"g:\pib\media\ebook\_test\_dl\book\01\book"
	//@"g:\pib\media\ebook\_test\_dl\book\02\book"
	};
DownloadAutomate_f.Test_ManageDirectoryGroup_03(directories, bonusDirectory: @"g:\pib\media\ebook\_test\bonus", usePrintDirectories: false, simulate: false, moveFiles: true);

// trace
string[] directories = new string[] {
	@"g:\pib\media\ebook\_test\print",
	@"g:\pib\media\ebook\_test\_dl\print"
	};
DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(directories).zToJson().zTrace();

string[] directories = new string[] {
	@"g:\pib\media\ebook\_test\book",
	@"g:\pib\media\ebook\_test\_dl\book\01\book"
	//@"g:\pib\media\ebook\_test\_dl\book\02\book"
	};
PrintFileManager.GetFileGroups(PrintFileManager.GetBonusDirectories(@"g:\pib\media\ebook\_test\bonus", DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(directories, usePrintDirectories: false)["Analyse des besoins _ la gestion de projet par étapes"])).zToJson().zTrace();
PrintFileManager.GetBonusDirectories(@"g:\pib\media\ebook\_test\bonus", DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(directories, usePrintDirectories: false)["Analyse des besoins _ la gestion de projet par étapes"]).zToJson().zTrace();
//DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(directories, usePrintDirectories: false).zToJson().zTrace();
PrintFileManager.GetFileGroups(DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(directories, usePrintDirectories: false).Values.First()).zToJson().zTrace();
PrintFileManager.GetBonusDirectories(@"g:\pib\media\ebook\_dl\_dl_pib\book\01\book").zToJson().zTrace();
PrintFileManager.GetBonusDirectories(@"g:\pib\media\ebook\_dl\_dl_pib\book\01\book").Select(directoryInfo => Path.GetFileName(directoryInfo.SubDirectory)).zToJson().zTrace();
PrintFileManager.GetBonusDirectories(@"g:\pib\media\ebook\_test\_dl\book\01\book").Select(directoryInfo => Path.GetFileName(directoryInfo.SubDirectory)).zToJson().zTrace();

string[] directories = new string[] {
	@"g:\pib\media\ebook\print",
	@"g:\pib\media\ebook\_dl\_dl_pib\08\print"
	};
DownloadAutomate_f.Test_ManageDirectoryGroup_02(directories, ".03_mensuel", simulate: false, moveFiles: true);


DownloadAutomate_f.CreatePrintFileManager(simulate: true, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".03_mensuel\60 millions de consommateurs"],
  @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: false, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".03_mensuel\60 millions de consommateurs"],
  @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: false, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".03_mensuel\Afrique magazine"],
  @"g:\pib\media\ebook\print"
  );

  
DownloadAutomate_f.CreatePrintFileManager(simulate: true, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\01 net"], @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: true, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\Le nouvel observateur"], @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: true, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\Courrier international"], @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: true, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\Micro hebdo"], @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: true, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\France football"], @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: true, moveFiles: true).ManageDirectoryGroups(
  new PrintDirectoryManager(new string[] { ".02_hebdo" }).GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" }).Values, @"g:\pib\media\ebook\print"
  );

DownloadAutomate_f.CreatePrintFileManager(simulate: false, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\01 net"], @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: false, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\Challenges"], @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: false, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\Le nouvel observateur"], @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: false, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\Courrier international"], @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: false, moveFiles: true).ManageDirectoryGroup(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\France football"], @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: false, moveFiles: true).ManageDirectoryGroups(
  new PrintDirectoryManager(new string[] { ".02_hebdo" }).GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" }).Values, @"g:\pib\media\ebook\print"
  );
DownloadAutomate_f.CreatePrintFileManager(simulate: false, moveFiles: true).ManageDirectoryGroups(
  new PrintDirectoryManager(new string[] { ".02_hebdo" }).GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\02\print" }).Values, @"g:\pib\media\ebook\print"
  );

PrintFileManager.GetFileGroups(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\Courrier international"]
  ).zTraceJson();
PrintFileManager.GetFileGroups(
  DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\ebook\print", @"g:\pib\media\ebook\_dl\_dl_pib\01\print" })[@".02_hebdo\Micro hebdo"]
  ).zTraceJson();

zpath.PathGetFilenameNumberInfo("France football - 2014-11-18 - no 3579").zTraceJson();
zpath.PathGetFilenameNumberInfo("France football - 2014-11-18 - no 3579[1]").zTraceJson();
zpath.PathGetFilenameNumberInfo("France football - 2014-12-02 - no 3581(1)").zTraceJson();
zpath.PathGetFilenameNumberInfo("Le nouvel observateur - 2013-07-25 - no 2542_2").zTraceJson();
PrintDirectoryManager.GetDirectoryDateStorage(@"g:\pib\media\ebook\print\.01_quotidien\Journaux").zTraceJson();
PrintDirectoryManager.GetDirectoryDateStorage(@"g:\pib\media\ebook\print\.01_quotidien\Journaux").GetDirectory(Date.Today).zTraceJson();
PrintDirectoryManager.GetDirectoryDateStorage(@"g:\pib\media\ebook\print\.02_hebdo\01 net").zTraceJson();
PrintDirectoryManager.GetDirectoryDateStorage(@"g:\pib\media\ebook\print\.02_hebdo\01 net").GetDirectory(Date.Today).zTraceJson();
PrintDirectoryManager.GetDirectoryDateStorage(@"g:\pib\media\ebook\_dl\_dl_pib\01\print\.02_hebdo\01 net").zTraceJson();
PrintDirectoryManager.GetDirectoryDateStorage(@"g:\pib\media\ebook\_dl\_dl_pib\01\print\.02_hebdo\01 net").GetDirectory(Date.Today).zTraceJson();
PrintIssue.GetPrintInfo("Le figaro - 2015-02-03 - no 21930").zView();
PrintIssue.GetPrintInfo("01 net - 2014-09-04 - no 803").zView();
PrintIssue.GetPrintInfo("01 net - hors-série - 2014-10-01 - no 82").zView();
PrintIssue.GetPrintInfo("Pour la science - 2014-03 - no 437 - le pouvoirs insoupçonné de l'inconscient").zView();


//Download.Print.DownloadAutomate_f.CreatePrintFileManager().ManageDirectory(3, @"c:\pib\_dl\_pib\dl\_test\test02\rapide-ddl.com", @"c:\pib\_dl\_pib\dl\_test\test02\golden-ddl.net");
Download.Print.DownloadAutomate_f.CreatePrintFileManager(simulate: true, moveFiles: true).ManageDirectory(2,
  @"c:\pib\_dl\_pib\dl\print", @"c:\pib\_dl\_pib\dl\rapide-ddl.com\print", @"c:\pib\_dl\_pib\dl\golden-ddl.net\print");
Download.Print.DownloadAutomate_f.Test_GetPrintDirectories_01(@"c:\pib\_dl\_pib\dl2\rapide-ddl.com\print");
Download.Print.DownloadAutomate_f.Test_ManagePrintDirectory_01(
  new string[] { @"c:\pib\_dl\_pib\dl\print", @"c:\pib\_dl\_pib\dl2\rapide-ddl.com\print", @"c:\pib\_dl\_pib\dl2\golden-ddl.net\print" },
  simulate: true, moveFiles: true);
Download.Print.DownloadAutomate_f.Test_ManagePrintDirectory_01(
  new string[] { @"c:\pib\_dl\_pib\dl\print", @"c:\pib\_dl\_pib\dl2\rapide-ddl.com\print", @"c:\pib\_dl\_pib\dl2\golden-ddl.net\print" },
  level: 2, journauxDirectory: ".01_quotidien\\Journaux", simulate: false, moveFiles: true);
Download.Print.DownloadAutomate_f.Test_ManagePrintDirectory_01(
  new string[] { @"g:\pib\media\print\.05_new_print", @"g:\pib\media\print\..new\.05_new_print" },
  level: 1, journauxDirectory: null, simulate: true, moveFiles: true);
Download.Print.DownloadAutomate_f.Test_ManagePrintDirectory_01(
  new string[] { @"g:\pib\media\print\.05_new_print", @"g:\pib\media\print\..new\.05_new_print" },
  level: 1, journauxDirectory: null, simulate: false, moveFiles: true);
Download.Print.DownloadAutomate_f.Test_ManagePrintDirectory_01(
  new string[] { @"g:\pib\media\print\.06_unknow_print", @"g:\pib\media\print\..new\.06_unknow_print" },
  level: 1, journauxDirectory: null, simulate: false, moveFiles: true);

Test_PrintDirectoryManager_01(@"c:\pib\_dl\_pib\dl\print");
Test_PrintDirectoryManager_01(@"g:\pib\media\print");

Download.Print.DownloadAutomate_f.Test_GetPrintDirectories_01(new string[] { @"c:\pib\_dl\_pib\dl\print" });
Download.Print.DownloadAutomate_f.Test_GetPrintDirectories_01(new string[] { @"c:\pib\_dl\_pib\dl\print\.01_quotidien" });


Trace.WriteLine(Download.Print.DownloadAutomate_f.CreatePrintDirectoryManager().GetDirectoryGroups(new string[] { @"g:\pib\media\print", @"c:\pib\_dl\_pib\dl\print" }).zToJson());
Trace.WriteLine(zdir.EnumerateDirectoriesInfo(@"c:\pib\_dl\_pib\dl\print\.01_quotidien\Journaux", followDirectoryTree: dir => { Trace.WriteLine("follow level {0} - \"{1}\"", dir.Level, dir.SubDirectory); }).zToJson());


zmongo.BsonReader<BsonValue>(@"c:\pib\dev_data\exe\runsource\download\log\5227_log_PrintDirectory_01.txt").zSave(@"c:\pib\dev_data\exe\runsource\download\log\5227_log_PrintDirectory_02.txt");
zmongo.BsonReader<BsonArray>(@"c:\pib\dev_data\exe\runsource\download\log\5227_log_PrintDirectory_01.txt")
  .SelectMany(v => v)
  .Select(v => 
  {
    v.zAsBsonArray()[1] = v.zAsBsonArray()[1].zAsBsonArray()[0];
    return v;
  }
  )
  .zSave(@"c:\pib\dev_data\exe\runsource\download\log\5227_log_PrintDirectory_02.txt");

zmongo.BsonReader<BsonArray>(@"c:\pib\dev_data\exe\runsource\download\log\5232_log_PrintDirectory_01.txt")
  .SelectMany(v => v)
  .Select(v => 
  {
    var v2 = v.zAsBsonArray()[1].zAsBsonArray()[0];
    v2.zSet("SubDirectoryType", v2.zGet("SubDirectory1Type").zGet("Type"));
    v2.zRemove("SubDirectory1Type");
    v.zAsBsonArray()[1] = v2;
    return v;
  }
  )
  .zSave(@"c:\pib\dev_data\exe\runsource\download\log\5232_log_PrintDirectory_02.txt");



























new pb.IO.SharpCompressManager().Uncompress(@"c:\pib\_dl\_pib\dl\_test\test02\rapide-ddl.com\print\.01_quotidien\Le monde\Le monde - 2014-05-09 - no 21556 .zip", @"c:\pib\_dl\_pib\dl\_test\test02\rapide-ddl.com\print\.01_quotidien\Le monde\bb");
new pb.IO.ZipManager().Uncompress(@"c:\pib\_dl\_pib\dl\_test\test02\rapide-ddl.com\print\.01_quotidien\Le monde\Le monde - 2014-05-09 - no 21556 .zip", @"c:\pib\_dl\_pib\dl\_test\test02\rapide-ddl.com\print\.01_quotidien\Le monde\bb");



Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_01("http://extreme-protect.net/bihYv");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_01("http://extreme-protect.net/hercules-2014-extended-2in1-multi-1080p-bluray-avc-dts-hdma-7-1-pch");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/1gasXCIOK");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/fWMAAbTwu5");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/wiz6Qqp");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/SN1E");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/cSULi");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/fGjGzmkDx");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/TeeMy6B");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/OrVc");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/XExbDclhDs");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/9A6l6g");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/LQdls");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/CjaX717WY8");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/0i5T");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/RTBhxrwftH");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/DEIrh");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/UM7Nv");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/AVYynltQQ");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/YPrdoK");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/rytJFoytgs");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/mtt4yxg1f");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/LCM85otRAs");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/hCLeWMJKbp");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/mMsSRiU");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/vRSXr");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/UQHfO9I");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/yQ56");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/qGxsSW");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/LWcfBSFpo");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/3v5ulKQ");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/OJ3YbK");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/fF7yv9gl");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/fNvQxKaNXq");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/MJiTZk2p4");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/SyOqupoFP");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/CrLCp2");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/JKsyUph");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("");
Download.Print.ExtremeDown.ExtremeDown_Exe.Test_ExtremeProtect_02("http://extreme-protect.net/hercules-2014-extended-2in1-multi-1080p-bluray-avc-dts-hdma-7-1-pch");

Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] { "http://www.extreme-down.net/ebooks/journaux/23700-les-journaux-samedi-03-janvier-2015-.html" }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] { "http://www.extreme-down.net/ebooks/journaux/23715-les-journaux-dimanche-04-janvier-2015-.html" }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] { "http://www.extreme-down.net/ebooks/journaux/23753-les-journaux-mardi-06-janvier-2015-.html" }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] { "http://www.extreme-down.net/ebooks/journaux/23779-les-journaux-mercredi-07-janvier-2015-.html" }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] { "http://www.extreme-down.net/ebooks/journaux/23781-les-journaux-jeudi-08-janvier-2015.html" }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] 
  { "http://www.extreme-down.net/ebooks/magazines/15615-questions-internationales-n64-novembre-dcembre-2013.html",
    "http://www.extreme-down.net/ebooks/journaux/23811-les-journaux-vendredi-9-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] 
  { "http://www.extreme-down.net/ebooks/journaux/23841-les-journaux-dimanche-11-janvier-2015.html",
    "http://www.extreme-down.net/ebooks/journaux/23824-les-journaux-samedi-10-janvier-2015.html",
    "http://www.extreme-down.net/ebooks/magazines/20296-le-monde-des-sciences-no14-juillet-aot-2014.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/23849-les-journaux-lundi-12-janvier-2015.html",
  "http://www.extreme-down.net/ebooks/magazines/15036-dossier-pour-la-science-n81-octobre-novembre-dcembre-2013.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/23868-les-journaux-mardi-13-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/23897-les-journaux-mercredi-14-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/23906-les-journaux-jeudi-15-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/23922-les-journaux-vendredi-16-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/23961-les-journaux-dimanche-18-janvier-2015.html",
  "http://www.extreme-down.net/ebooks/journaux/23947-les-journaux-samedi-17-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/23978-les-journaux-lundi-19-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/magazines/14363-go-histoire-n11-octobre-novembre-2013.html",
  "http://www.extreme-down.net/ebooks/journaux/24012-les-journaux-mercredi-21-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/23988-les-journaux-mardi-20-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/24027-les-journaux-jeudi-22-janvier-2015.html",
  "http://www.extreme-down.net/ebooks/magazines/14646-national-geographic-n169-octobre-2013.html",
  "http://www.extreme-down.net/ebooks/magazines/14556-sciences-et-avenir-hors-srie-n176-octobre-novembre-2013.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/magazines/14652-pour-la-science-n432-octobre-2013.html",
  "http://www.extreme-down.net/ebooks/journaux/24040-les-journaux-vendredi-23-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/24053-les-journaux-samedi-24-janvier-2015.html",
  "http://www.extreme-down.net/ebooks/magazines/14652-pour-la-science-n432-octobre-2013.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/24063-les-journaux-dimanche-25-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/24103-les-journaux-mercredi-28-janvier-2015.html",
  "http://www.extreme-down.net/ebooks/magazines/17582-les-cahiers-de-science-vie-n43-fvrier-2014.html",
  "http://www.extreme-down.net/ebooks/journaux/24090-les-journaux-mardi-27-janvier-2015.html",
  "http://www.extreme-down.net/ebooks/magazines/22623-psychologies-magazine-no345-novembre-2014.html",
  "http://www.extreme-down.net/ebooks/journaux/24082-les-journaux-lundi-26-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/24115-les-journaux-jeudi-29-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "http://www.extreme-down.net/ebooks/journaux/24127-les-journaux-vendredi-30-janvier-2015.html"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(new string[] {
  "",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);


  
  
Download.Print.DownloadAutomate_f.Test_TryDownload_EbookdzPost_FromUrls_01(new string[] {
  //"http://www.ebookdz.com/forum/showthread.php?t=110843"
  //"http://www.ebookdz.com/forum/showthread.php?t=110674"
  "http://www.ebookdz.com/forum/showthread.php?t=109977"
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);

Download.Print.DownloadAutomate_f.Test_TryDownload_EbookdzPost_FromUrls_01(new string[] {
  "http://www.ebookdz.com/forum/showthread.php?t=109977",
  ""
  }, uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: false);


Trace.WriteLine(Download.Print.ExtremeDown.ExtremeProtect.GetKey());

Http2.LoadUrl("http://www.extreme-down.net/ebooks/", new HttpRequestParameters { encoding = Encoding.UTF8 });
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='dle-content']//div[@class='blockbox']:.:EmptyRow");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='dle-content']//div[@class='blockbox']:.:EmptyRow", ".//h2[@class='blocktitle']//a//text()");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@class='navigation ignore-select']//a[starts-with(text(), 'Suivant')]/@href:.:EmptyRow");

RunSource.CurrentRunSource.View(Download.Print.ExtremeDown.ExtremeDown_LoadHeaderPagesManager.CurrentLoadHeaderPagesManager.LoadPages(startPage: 1, maxPage: 2, reload: false, loadImage: false, refreshDocumentStore: false));

Http2.LoadUrl("http://www.extreme-down.net/ebooks/journaux/23375-les-journaux-mardi-09-dcembre-2014.html", new HttpRequestParameters { encoding = Encoding.UTF8 });
// Pack1 Pack2
Http2.LoadUrl("http://www.extreme-down.net/ebooks/journaux/23357-les-journaux-lundi-08-dcembre-2014.html", new HttpRequestParameters { encoding = Encoding.UTF8 });
// Science & Vie Guerres & Histoire No.22 - Décembre 2014
Http2.LoadUrl("http://www.extreme-down.net/ebooks/magazines/14546-science-vie-n1153-octobre-2013-.html", new HttpRequestParameters { encoding = Encoding.UTF8 });

HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='dle-content']:.:EmptyRow", ".//h2//text()");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='dle-content']//h2:.:EmptyRow", ".//text()");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='dle-content']//script/parent::div//following-sibling::h2:.:EmptyRow", ".//text()");



Test.Test_Web.Test_Url.Test_Uri_01("http://ul.to/j5wg2i2u");
Test.Test_Web.Test_Url.Test_Uri_01("http://6i5mqc65bc.1fichier.com/");

Trace.WriteLine(pb.Web.DownloadServer.GetServerNameFromLink("http://ul.to/j5wg2i2u"));
Trace.WriteLine(pb.Web.DownloadServer.GetServerNameFromLink("http://6i5mqc65bc.1fichier.com/"));




Trace.WriteLine(Download.Print.DownloadAutomate_f.CreateDebrider().DebridLink(new string[] { "http://ul.to/cqy8lpra" }));

Trace.WriteLine("toto");

Download.Print.DownloadAutomate_f.Test_PostDownloadLinks_01();

Trace.WriteLine(Download.Print.ExtremeDown.ExtremeDown_LoadPostDetail.LoadPostDetailFromWebManager.Load(new pb.Web.RequestFromWeb("http://www.extreme-down.net/musique/rock/23312-eric-clapton-the-platinum-collection.html", new HttpRequestParameters { encoding = Encoding.UTF8 })).zToJson());
Trace.WriteLine(Download.Print.ExtremeDown.ExtremeDown_LoadPostDetail.LoadPostDetailFromWebManager.Load(new pb.Web.RequestFromWeb("http://www.extreme-down.net/musique/rock/22179-jeff-beck-albums-collection-9-albums-10cd-2013-2014.html", new HttpRequestParameters { encoding = Encoding.UTF8 })).zToJson());

Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(
  new string[] { "http://www.extreme-down.net/musique/rock/23312-eric-clapton-the-platinum-collection.html",
  "http://www.extreme-down.net/musique/rock/22179-jeff-beck-albums-collection-9-albums-10cd-2013-2014.html" },
  uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: true);
Download.Print.DownloadAutomate_f.Test_TryDownload_ExtremeDownPost_FromUrls_01(
  new string[] { "http://www.extreme-down.net/musique/rock/23312-eric-clapton-the-platinum-collection.html",
  "http://www.extreme-down.net/musique/rock/22179-jeff-beck-albums-collection-9-albums-10cd-2013-2014.html" },
  uncompressFile: true, forceDownloadAgain: true, forceSelect: true, simulateDownload: false, useTestManager: true);
pb.Data.Mongo.TraceMongoCommand.Export("dl", "QueueDownloadFile_new", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_QueueDownloadFile_new.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "CurrentDownloadFile", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_CurrentDownloadFile.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadedFile", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_DownloadedFile.txt"), sort: "{ _id: 1 }");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.QueueDownloadFile_new.drop()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.CurrentDownloadFile.drop()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.DownloadedFile.drop()", "dl");


Http2.LoadUrl("http://telechargementz.tv/ebooks/");
//Http2.LoadUrl("http://telechargementz.tv/ebooks/", new HttpRequestParameters { encoding = Encoding.UTF8 });
//Http2.LoadUrl("http://telechargementz.tv/ebooks/", new HttpRequestParameters { encoding = Encoding.Default });
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='dle-content']//div[@class='custom-post']:.:EmptyRow");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@class='navigation']//a[text()=\"vers l'avant\"]/@href:.:EmptyRow");

Http2.LoadUrl("http://telechargementz.tv/ebooks/6484-alternatives-internationales-hors-srie-no16-janvier-2015.html");
Http2.LoadUrl("http://telechargementz.tv/ebooks/6561-ben-tome-03-bd.html");
Http2.LoadUrl("http://telechargementz.tv/ebooks/6468-les-cent-pomes-franais-les-plus-clbres.html");

Trace.WriteLine(Download.Print.Telechargementz.Telechargementz_LoadPostDetail.CurrentLoadPostDetail.LoadDocument("http://telechargementz.tv/ebooks/6484-alternatives-internationales-hors-srie-no16-janvier-2015.html", refreshDocumentStore: true).Document.zToJson());
Trace.WriteLine(Download.Print.Telechargementz.Telechargementz_LoadPostDetail.CurrentLoadPostDetail.LoadDocument("http://telechargementz.tv/ebooks/6561-ben-tome-03-bd.html", refreshDocumentStore: true).Document.zToJson());
Trace.WriteLine(Download.Print.Telechargementz.Telechargementz_LoadPostDetail.CurrentLoadPostDetail.LoadDocument("http://telechargementz.tv/ebooks/6468-les-cent-pomes-franais-les-plus-clbres.html", refreshDocumentStore: true).Document.zToJson());
Trace.WriteLine(Download.Print.Telechargementz.Telechargementz_LoadPostDetail.CurrentLoadPostDetail.LoadDocument("http://telechargementz.tv/ebooks/7006-grand-guide-des-huiles-essentielles-alessandra-buronzo.html", refreshDocumentStore: true).Document.zToJson());


HttpRun.Load("http://telechargementz.tv/ebooks/");
HtmlRun.Select("//div[@id='dle-content']//div[@class='custom-post']:.:EmptyRow");
Trace.WriteLine("toto");


//*************************************************************************************************************************
//****                                   $$info.test.regex
//*************************************************************************************************************************

Trace.WriteLine("toto");
Trace.WriteLine(RunSource.CurrentRunSource.ProjectFile);
RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\Text\Test\Test_Text.project.xml");

Test_RegexValues.Test(
  new RegexValuesList(XmlConfig.CurrentConfig.GetConfig("PrintList2Config").GetElements("FindPrints/Prints/Print"), compileRegex: true),
  "La Dernière Heure Namur");
  "Le Monde dossier Hiroshima");
  //"Le Monde Culture et Idées");
  //"Le Monde Culture");

Test.Test_Text.Test_Regex.Test("&(?:(?:#([0-9]+))|(?:#x([0-9a-f]+))|([a-z]+));", "toto&#amp;tata");
Test.Test_Text.Test_Regex.Test("&(?:(?:#([0-9]+))|(?:#x([0-9a-f]+))|([a-z]+));", "toto&amp;tata");
Test.Test_Text.Test_Regex.Test("&(?:(?:#([0-9]+))|(?:#x([0-9a-f]+))|([a-z]+));", "toto&#38;tata");
Test.Test_Text.Test_Regex.Test("&(?:(?:#([0-9]+))|(?:#x([0-9a-f]+))|([a-z]+));", "toto&#x3c;tata");

int.Parse("3c", System.Globalization.NumberStyles.HexNumber).zTrace();
Encoding.UTF8.GetString(new byte[] { 0x3c }).zTrace();
Encoding.UTF8.GetString(new byte[] { 0x01, 0x52 }).zTrace();
Encoding.UTF8.GetString(new byte[] { 0x01, 0x53 }).zTrace();
((char)0x152).ToString().zTrace();
((char)0x153).zTrace();
zstr.DecodeHtmlSpecialCharacters("toto&amp;tata").zTrace();
zstr.DecodeHtmlSpecialCharacters("toto&#38;tata").zTrace();
zstr.DecodeHtmlSpecialCharacters("toto&#x3c;tata").zTrace();

// DateNew1 : "(?:^|[_\s])(?:(?:du|des)\s*)?(?:(?:lundi|lun|mardi|mar|mercre?di|mer|jeudi|jeu|vendredi|ven|samedi|sam|dimanche|dim)|((?<![0-9])[0-9]{4})|((?<![0-9])[0-9]{1,2}|1er)|(janvier|janv|f[eé]vrier|mars|avril|mai|juin|juillet|juill|a[o0][uû\s]t?|septembre|sptembre|octobre|novembre|d[eé]cembre|january|february|march|april|may|june|july|august|september|october|november|december|printemps|[eé]t[eé]|automne|hiver)|(?:au|et|/|&)|(?:[_\.,\-/\\\s]+))+[a-z]?$"
// DateNew2 : "(?:^|[_\s])(?:(?:du|des)\s*)?(?:(?:lundi|lun|mardi|mar|mercre?di|mer|jeudi|jeu|vendredi|ven|samedi|sam|dimanche|dim)|((?<![0-9])[0-9]{4})|((?<![0-9])[0-9]{1,2}|1er)|(janvier|janv|f[eé]vrier|mars|avril|mai|juin|juillet|juill|a[o0][uû\s]t?|septembre|sptembre|octobre|novembre|d[eé]cembre|january|february|march|april|may|june|july|august|september|october|november|december|printemps|[eé]t[eé]|automne|hiver)|(?:au|et|/|&)|(?:[_\.,\-/\\\s]+))+[a-z]?(?:$|[_\s])"

Trace.WriteLine(XmlConfig.CurrentConfig.GetConfig("PrintList1Config").Get("FindPrints/Dates/DateNew/@regex"));
Trace.WriteLine(XmlConfig.CurrentConfig.GetConfig("PrintList1Config").Get("FindPrints/Dates/DateNew1/@regex"));

"$FindPrints/Dates/Weekday$".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")).zTrace();
"$FindPrints/Dates/Date2/@regex$".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")).zTrace();
"$FindPrints/Dates/Date2[3]/@regex$".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")).zTrace();

Test.Test_Text.Test_Regex.Test(
  "$FindPrints/Dates/Date2/@regex$".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  "derni__res_nouvelles_d_alsace_du_mardi_04_ao__t_2015");

Test.Test_Text.Test_Regex.Test(
  "$FindPrints/Dates/Date2[3]/@regex$".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  "lacr-2015-08-27");
  //"Les Echos + Echos week-end du vendredi 04 et samedi 05 sepembre 2015");
  //"La Croix Week-End Du Samedi 12 et Dimanche 13 Septembre 2015");
  //"Le Parisien + Le Guide De Votre Dimanche Du 18 Octobre 2015");
  // found no 1 "Dimanche", found no 2 "Du 18 Octobre 2015"

Test.Test_Text.Test_Regex.Test(
  //"$FindPrints/Dates/DateNew/@regex$".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  "$FindPrints/Dates/DateNew2/@regex$".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  "Système D Bricothèmes N°16 - Mars 2014");

  // (?<=^|[_\s])(?:(?:du|des)\s*)?(?:(?:lundi|lun|mardi|mar|mercre?di|mer|jeudi|jeu|vendredi|ven|samedi|sam|dimanche|dim)|((?<![0-9])[0-9]{4})|((?<![0-9])[0-9]{1,2}|1er)|(janvier|janv|f[eé]vrier|mars|avril|mai|juin|juillet|juill|a[o0][uû\s]t?|septembre|sptembre|octobre|novembre|d[eé]cembre|january|february|march|april|may|june|july|august|september|october|november|december|printemps|[eé]t[eé]|automne|hiver)|(?:au|et|/|&)|(?:[_\.,\-/\\\s]+))+(?=$|[_\s])
  //@"(?<=^|[_\s])(?:(?:du|des)\s*)?(?:mars|2014|[_\.,\-/\\\s])+(?=$|[_\s])",
  //@"(?<=^|[_\s])(?:(?:du|des)\s*)?(?:mars|2014|[_\.,\-/\\\s])+[a-z]?(?=$|[_\s])",
  //"(?<=$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:$FindPrints/Dates/Weekday$)|($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/To$)|(?:$FindPrints/Dates/Separator1$+))+(?=$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  //"(?<=$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:$FindPrints/Dates/Weekday$)|($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/To$)|(?:$FindPrints/Dates/Separator1$+))+[a-z]?(?=$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
Test.Test_Text.Test_Regex.Test(
  //"(?<=$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:$FindPrints/Dates/Weekday$)|($FindPrints/Dates/Year2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/Separator1$+))+[a-z]?(?=$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  //"(?<=$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:$FindPrints/Dates/Weekday$)|2014|mars|(?:$FindPrints/Dates/Separator1$+))+[a-z]?(?=$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  //"(?<=$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:lundi|lun|mardi|mar|mercre?di|mer|jeudi|jeu|vendredi|ven|samedi|sam|dimanche|dim)|2014|mars|(?:$FindPrints/Dates/Separator1$+))+[a-z]?(?=$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  "(?<=$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:mar|mercre?di|mer)|2014|mars|(?:$FindPrints/Dates/Separator1$+))+[a-z]?(?=$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  //"(?<=$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/To$)|(?:$FindPrints/Dates/Separator1$+))+[a-z]?(?=$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  "Système D Bricothèmes N°16 - Mars 2014");

Test.Test_Text.Test_Regex.Test(@"([0-9]+)(?:$|[_\s])", "2014");

// DateNew1
Test.Test_Text.Test_Regex.Test(
  @"(?:^|[_\s])(?:(?:du|des)\s*)?(?:(?:lundi|lun|mardi|mar|mercre?di|mer|jeudi|jeu|vendredi|ven|samedi|sam|dimanche|dim)|((?<![0-9])[0-9]{4})|((?<![0-9])[0-9]{1,2}|1er)|(janvier|janv|f[eé]vrier|mars|avril|mai|juin|juillet|juill|a[o0][uû\s]t?|septembre|sptembre|octobre|novembre|d[eé]cembre|january|february|march|april|may|june|july|august|september|october|november|december|printemps|[eé]t[eé]|automne|hiver)|(?:au|et|/|&)|(?:[_\.,\-/\\\s]+))+[a-z]?$",
  "Système D Bricothèmes N°16 - Mars 2014");

  //@"^\s*les\s*[eé]chos(?:\s*(?:\+|et)(?:\s*les)?\s*[eé]chos\s*(?:soci[eé]t[eé]|socit)|week[\-\s]*end|business)?$",
Test.Test_Text.Test_Regex.Test(
  @"^\s*les\s*[eé]chos(?:\s*(?:\+|et))?(?:\s*(?:\s*les)?\s*[eé]chos|\s*soci[eé]t[eé]|\s*socit|\s*week[\-\s]*end|\s*business)*$",
  "Les Echos Business");
  "Les Echos + Echos week-end");
  "Les Echos et les Echos Socit");
  "Les Echos");
  "Les Echos + échos Société");
  //"Les echos + echos week-end");

// DateNew2
Test.Test_Text.Test_Regex.Test(
  //@"(?:^|[_\s])(?:(?:du|des)\s*)?(?:(?:lundi|lun|mardi|mar|mercre?di|mer|jeudi|jeu|vendredi|ven|samedi|sam|dimanche|dim)|((?<![0-9])[0-9]{4})|((?<![0-9])[0-9]{1,2}|1er)|(janvier|janv|f[eé]vrier|mars|avril|mai|juin|juillet|juill|a[o0][uû\s]t?|septembre|sptembre|octobre|novembre|d[eé]cembre|january|february|march|april|may|june|july|august|september|october|november|december|printemps|[eé]t[eé]|automne|hiver)|(?:au|et|/|&)|(?:[_\.,\-/\\\s]+))+[a-z]?(?:$|[_\s])",
  //@"(?:^|[_\s])(?:(?:du|des)\s*)?(?:(?:lundi|lun|mardi|mar|mercre?di|mer|jeudi|jeu|vendredi|ven|samedi|sam|dimanche|dim)|((?<![0-9])[0-9]{4})|((?<![0-9])[0-9]{1,2}|1er)|(janvier|janv|f[eé]vrier|mars|avril|mai|juin|juillet|juill|a[o0][uû\s]t?|septembre|sptembre|octobre|novembre|d[eé]cembre|january|february|march|april|may|june|july|august|september|october|november|december|printemps|[eé]t[eé]|automne|hiver)|(?:au|et|/|&)|(?:[_\.,\-/\\\s]+))+[a-z]?(_| |$)",
  //"(?:$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:$FindPrints/Dates/Weekday$)|($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/To$)|(?:$FindPrints/Dates/Separator1$+))+[a-z]?(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  //"(?:$FindPrints/Dates/BeginSeparator$)(?:($FindPrints/Dates/Month2$)|($FindPrints/Dates/Year2$))+[a-z]?(_| |$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  //"(?:$FindPrints/Dates/BeginSeparator$)(?:($FindPrints/Dates/Month2$)|($FindPrints/Dates/Year2$)|(?:$FindPrints/Dates/Separator1$+))+$".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  // /* ok */ "(?:$FindPrints/Dates/BeginSeparator$)(?:($FindPrints/Dates/Month2$)|($FindPrints/Dates/Year2$)|(?:$FindPrints/Dates/Separator1$+))+(_| |$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  // /* ok */ "(?:$FindPrints/Dates/BeginSeparator$)(?:($FindPrints/Dates/Month2$)|($FindPrints/Dates/Year2$)|(?:$FindPrints/Dates/Separator1$+))+(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  // /* ok */ "(?:$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:($FindPrints/Dates/Month2$)|($FindPrints/Dates/Year2$)|(?:$FindPrints/Dates/Separator1$+))+(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  // /* ok */ "(?:$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:$FindPrints/Dates/Weekday$)|($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/Separator1$+))+(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  // /* ok */ "(?:$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:$FindPrints/Dates/Weekday$)|($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/To$)|(?:$FindPrints/Dates/Separator1$+))+(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  // (?:^|[_\s])
  // /* ok */ @"\s(?:du\s+|des\s+)?(?:($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/Separator1$+))+(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  // /* ok */ @"(?:$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/Separator1$+))+(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  // /* ok */ @"(?:$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/To$)|(?:$FindPrints/Dates/Separator1$+))+(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  /* ok */ @"(?:$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:dimanche)|($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/To$)|(?:$FindPrints/Dates/Separator1$+))+(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  //        "(?:$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:$FindPrints/Dates/Weekday$)|($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/To$)|(?:$FindPrints/Dates/Separator1$+))+[a-z]?(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  //"Système D Bricothèmes N°16 - Mars 2014");
  "Le Parisien + Le Guide De Votre Dimanche Du 18 Octobre 2015");

Test.Test_Text.Test_Regex.Test(
  "Le Parisien + Le Guide De Votre Dimanche Du 18 Octobre 2015 ");
  //@"(?:$FindPrints/Dates/BeginSeparator$)(?:$FindPrints/Dates/From$)?(?:(?:dimanche)|($FindPrints/Dates/Year2$)|($FindPrints/Dates/Day2$)|($FindPrints/Dates/Month2$)|(?:$FindPrints/Dates/To$)|(?:$FindPrints/Dates/Separator1$+))+(?:$FindPrints/Dates/EndSeparator$)".zConfigGetVariableValue(XmlConfig.CurrentConfig.GetConfig("PrintList1Config")),
  // BeginSep   From                 Weekday       Year2                Day2                      Month2                                                                                                                                                                                                                                            To            Separator1          EndSeparator
  //@"(?:^|[_\s])(?:(?:du|des)\s*)?(?:(?:dimanche)|((?<![0-9])[0-9]{4})|((?<![0-9])[0-9]{1,2}|1er)|(janvier|janv|f[eé]vrier|mars|avril|mai|juin|juillet|juill|a[o0][uû\s]t?|septembre|sptembre|octobre|novembre|d[eé]cembre|january|february|march|april|may|june|july|august|september|october|november|december|printemps|[eé]t[eé]|automne|hiver)|(?:au|et|/|&)|(?:[_\.,\-/\\\s]+))+(?:$|[_\s])",
  //@"\s(?:du\s*)?(?:(ddimanche)|([0-9]{4})|([0-9]{1,2})|(octobre)|(?:[_\.,\-/\\\s]+))+(?:$|[_\s])",
  //@"\s(du\s*)?((dimanche)|([0-9]{4})|([0-9]{1,2})|(octobre)|(\s+))+($|\s)",
  //@"\s(du\s*)?([0-9]{4}|[0-9]{1,2}|octobre|\s+|dimanche)+($|\s)",
  //@"\s(du\s)?([0-9]{1,2}|octobre|[0-9]{4}|\s+|dimanche)+($|\s)",
  //@"\s(du)?(18|octobre|2015|\s|ddimanche)+(_|\s|$)",
Test.Test_Text.Test_Regex.Test(
  //@"\s(du)?(18|octobre|2015|\s|dimanche)+\s",  // (_|\s|$)
  //@"(\sdu)?(\s18|\soctobre|\s2015|\sddimanche)+\s",  // (_|\s|$)
  //@"(\sdu)?(\s18|\soctobre|\s2015|\sdimanche)+(?=\s)",  // (_|\s|$)
  @"(\sdu)?(\s18|\soctobre|\s2015|\sdimanche)+(?=$|[_\s])",  // (_|\s|$)
  "Le Parisien + Le Guide De Votre Dimanche Du 18 Octobre 2015 ");

//^\s*(?:journal\s*)?le\s*monde(?:[\-+,;\s]*|et|le|de|&amp;|(?:[1-4]\s*)?(?:supp|les\s*supp|suppl[eé]ments?|suppl[eé]ments\s*su)|argent|colloque|culture|decine|diplomatique|dosier|dossier|[eé]co|[eé]conomique|entreprise|festival|forme|g[eé]o|g[eé]opol[io]tique|id[eé]es|livres|le\s*monde\s*des\s*livres|monde\s*des\s*livres|m|mag|magazine|le\s*magazine|m[eé]decine|monde|placement|politique|science|sport|monde\s*tv|monde\s*t[eé]l[eé]vision|t[eé]l[eé]|supp\s*t[eé]l[eé]|univ|week[\-\s]*end|br[eé]sil|journal|[0-9]{4})*$

Test.Test_Text.Test_Regex.Test(
  @"^\s*(?:journal\s*)?le\s*monde(?:[\-+,;\s]*|et|le|de|&amp;|(?:[1-4]\s*)?(?:supp|les\s*supp|suppl[eé]ments?|suppl[eé]ments\s*su)|argent|colloque|culture|decine|diplomatique|dosier|dossier|[eé]co|[eé]conomique|entreprise|festival|forme|g[eé]o|g[eé]opol[io]tique|id[eé]es|livres|le\s*monde\s*des\s*livres|monde\s*des\s*livres|m|mag|magazine|le\s*magazine|m[eé]decine|monde|placement|politique|science|sport|monde\s*tv|monde\s*t[eé]l[eé]vision|t[eé]l[eé]|supp\s*t[eé]l[eé]|univ|week[\-\s]*end|br[eé]sil|journal|[0-9]{4})*$",
  "Le Monde Culture&amp;Idées");
  //"Le Monde Culture &amp; Idées");
  //"Le Monde Culture et Idées");
  //"Le Monde Culture&amp;Idées");
  //"le monde");
  

Test.Test_Text.Test_Regex.Test(
  "(?<number>[0-9]+)(?<name>.*)$",
  "0001_pib_log");

Regex regex = new Regex("(?<number>[0-9]+)(?<name>.*)$");
Match match = regex.Match("0001_pib_log");
if (match.Success)
{
	string name;
	name = "number"; Trace.WriteLine("group \"{0}\" : \"{1}\"", name, match.Groups[name].Value);
	name = "name"; Trace.WriteLine("group \"{0}\" : \"{1}\"", name, match.Groups[name].Value);
}
else
	Trace.WriteLine("not found");
  
Test.Test_Text.Test_Regex.Test(
  @"(?:\[|.:)?\s*(?:hq)?\s*pdf\s*(?:\]|:.)?",
  "Format: PDF");
Test.Test_Text.Test_Regex.Test(
  "^([0-9]{4})|([0-9]{4}-[0-9]{2})|(.*)([0-9]{4}-[0-9]{2}-[0-9]{2})$",
  "2014-10");
Test.Test_Text.Test_Regex.Test(
  "^(([0-9]{4})|([0-9]{4}-[0-9]{2})|(.*)([0-9]{4}-[0-9]{2}-[0-9]{2}))$",
  "Journaux - 2014-10-01");
Test.Test_Text.Test_Regex.Test(
  "^(.*?)( - hors-série)?( - (([0-9]{4}-[0-9]{2}-[0-9]{2})|([0-9]{4}-[0-9]{2})|([0-9]{4})))( - no ([0-9]+))?( - (.*))?$",
  "Le figaro - 2015-02-03 - no 21930", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
Test.Test_Text.Test_Regex.Test(
  "^(.*?)( - hors-série)?( - (([0-9]{4}-[0-9]{2}-[0-9]{2})|([0-9]{4}-[0-9]{2})|([0-9]{4})))( - no ([0-9]+))?( - (.*))?$",
  "01 net - 2014-09-04 - no 803", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
Test.Test_Text.Test_Regex.Test(
  "^(.*?)( - hors-série)?( - (([0-9]{4}-[0-9]{2}-[0-9]{2})|([0-9]{4}-[0-9]{2})|([0-9]{4})))( - no ([0-9]+))?( - (.*))?$",
  "01 net - hors-série - 2014-10-01 - no 82", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
Test.Test_Text.Test_Regex.Test(
  "^(.*?)( - hors-série)?( - (([0-9]{4}-[0-9]{2}-[0-9]{2})|([0-9]{4}-[0-9]{2})|([0-9]{4})))( - no ([0-9]+))?( - (.*))?$",
  "Pour la science - 2014-03 - no 437 - le pouvoirs insoupçonné de l'inconscient", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);




Test_Object_01(DateTime.Now);

Test.Test_Bson.Test_BsonWriter_f.Test_BsonWriter_02(@"c:\pib\dev_data\exe\runsource\test_unit\Print\PrintTextValues\PrintTextValues_test_01.txt");
Test.Test_Bson.Test_BsonWriter_f.Test_BsonWriter_02(@"c:\pib\dev_data\exe\runsource\test_unit\Print\PrintTextValues\PrintTextValues_test_02.txt");
Test.Test_Bson.Test_BsonWriter_f.Test_BsonWriter_02(@"c:\pib\dev_data\exe\runsource\test_unit\Print\PrintTextValues\Dictionary_test_01.txt");
Test.Test_Bson.Test_BsonWriter_f.Test_BsonWriter_02(@"c:\pib\dev_data\exe\runsource\test_unit\Print\PrintTextValues\Dictionary_test_02.txt");
Test.Test_Bson.Test_BsonWriter_f.Test_BsonWriter_02(@"c:\pib\dev_data\exe\runsource\test_unit\Print\PrintTextValues\Dictionary_test_03.txt");



Http2.LoadUrl("http://www.ebookdz.com/");
Http2.LoadUrl("http://www.ebookdz.com/forum/showthread.php?t=109595");

Download.Print.Ebookdz.Ebookdz_Exe.Test_Md5_01("toto");
Download.Print.Ebookdz.Ebookdz_Exe.Test_Md5_02();
Download.Print.Ebookdz.Ebookdz_Exe.Test_Login_01("http://www.ebookdz.com/");
Download.Print.Ebookdz.Ebookdz_Exe.Test_Login_01("http://www.ebookdz.com/forum/showthread.php?t=109595");
Download.Print.Ebookdz.Ebookdz_Exe.Test_LoadWithCookies_01("http://www.ebookdz.com/forum/showthread.php?t=109595");
Download.Print.Ebookdz.Ebookdz_Exe.Test_SaveCookie_01();
Download.Print.Ebookdz.Ebookdz_Exe.Test_LoadCookie_01();
Download.Print.Ebookdz.Ebookdz_Exe.Test_Login_02();


HtmlXmlReader.CurrentHtmlXmlReader.LoadXml(@"c:\pib\dev_data\exe\runsource\test_unit\HtmlXml\test_03.xml");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//toto/@key:.:EmptyRow");


object o = XDocument.Load(@"c:\pib\dev_data\exe\runsource\test_unit\HtmlXml\test_04.xml").XPathEvaluate("//toto/text()");
Trace.WriteLine(o.GetType().zGetName());
object o = XDocument.Load(@"c:\pib\dev_data\exe\runsource\test_unit\HtmlXml\test_03.xml").XPathEvaluate("/root/tata");
Trace.WriteLine(o.GetType().zGetName());
if (o is IEnumerable)
{
  foreach (object o2 in o as IEnumerable)
    Trace.WriteLine(o2.GetType().zGetName());
}

Test.Test_Unit.Web.Test_Unit_HtmlToXml.Test();
Test.Test_Unit.Web.Test_Unit_HtmlToXml.ArchiveOkFiles();
Test.Test_Unit.Web.Test_Unit_HtmlToXml.SetFilesAsOk();
Test.Test_Unit.Web.Test_Unit_HtmlToXml.FileHtmlToXml(@"c:\pib\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_01_01.html", traceHtmlReader: true);
Test.Test_Unit.Web.Test_Unit_HtmlToXml.FileHtmlToXml(@"c:\pib\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_02_01.html", traceHtmlReader: true);

Trace.WriteLine(zfile.GetNewIndexedDirectory(@"c:\pib\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\ebookdz.com\detail\archive\"));

Trace.WriteLine(zfile.GetNewIndexedDirectory_new(@"c:\pib\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\ebookdz.com\detail\archive"));
int maxIndexLength;
int index = zdir.GetLastDirectoryIndex_new(@"c:\pib\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\ebookdz.com\detail\archive", null, out maxIndexLength);
Trace.WriteLine("{0} {1}", index, maxIndexLength);
int maxIndexLength;
int index = zfile.GetLastFileNameIndex_new(@"c:\pib\prog\tools\runsource\exe\run", null, out maxIndexLength);
Trace.WriteLine("{0} {1}", index, maxIndexLength);

Trace.WriteLine(zfile.GetNewIndexedFileName(@"c:\pib\dev_data\exe\runsource\download\http"));
Trace.WriteLine(zdir.GetNewIndexedDirectory(@"c:\pib\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\ebookdz.com\detail\archive"));

pb.Data.Mongo.BsonDocumentComparator.CompareBsonDocumentFilesWithKey(
  @"c:\pib\dev_data\exe\runsource\download\mongo\export\rapide-ddl\RapideDdl_Detail2\export_RapideDdl_Detail2_23.txt",
  @"c:\pib\dev_data\exe\runsource\download\mongo\export\rapide-ddl\RapideDdl_Detail2\export_RapideDdl_Detail2_24.txt",
  "_id", "_id",
  joinType: pb.Linq.JoinType.FullOuterJoin,
  //where: doc => doc["document1"]["file"] != null || doc["document2"]["file"] != null
  comparatorOptions: BsonDocumentComparatorOptions.ReturnNotEqualDocuments | BsonDocumentComparatorOptions.ResultNotEqualElements,
  enumerateElementsOptions: EnumerateElementsOptions.LowercaseName
  ).zGetResults().zSaveToJsonFile(@"c:\pib\dev_data\exe\runsource\download\mongo\export\rapide-ddl\RapideDdl_Detail2\compare_RapideDdl_Detail2_23_24.txt");
//.Select(result => result.GetResultDocument())
  
BsonDocumentComparator.CompareBsonDocuments(
zmongo.ReadFileAs<MongoDB.Bson.BsonDocument>(@"c:\pib\dev_data\exe\runsource\download\mongo\export\rapide-ddl\RapideDdl_Detail2_23_42160.txt"),
zmongo.ReadFileAs<MongoDB.Bson.BsonDocument>(@"c:\pib\dev_data\exe\runsource\download\mongo\export\rapide-ddl\RapideDdl_Detail2_24_42160.txt"),
// comparatorOptions: BsonDocumentComparatorOptions.ReturnAllDocuments | BsonDocumentComparatorOptions.ResultAllElements | BsonDocumentComparatorOptions.ResultDocumentsSource)
comparatorOptions: BsonDocumentComparatorOptions.ReturnAllDocuments | BsonDocumentComparatorOptions.ResultNotEqualElements
//enumerateElementsOptions: EnumerateElementsOptions.LowercaseName
).zSave(@"c:\pib\dev_data\exe\runsource\download\mongo\export\rapide-ddl\compare_RapideDdl_Detail2_23_24_42160.txt");
  
BsonDocumentComparator.CompareBsonDocuments(
zmongo.ReadFileAs<MongoDB.Bson.BsonDocument>(@"c:\pib\dev_data\exe\runsource\download\mongo\export\rapide-ddl\RapideDdl_Detail2_23_42152.txt"),
zmongo.ReadFileAs<MongoDB.Bson.BsonDocument>(@"c:\pib\dev_data\exe\runsource\download\mongo\export\rapide-ddl\RapideDdl_Detail2_24_42152.txt"),
// comparatorOptions: BsonDocumentComparatorOptions.ReturnAllDocuments | BsonDocumentComparatorOptions.ResultAllElements | BsonDocumentComparatorOptions.ResultDocumentsSource)
comparatorOptions: BsonDocumentComparatorOptions.ReturnAllDocuments | BsonDocumentComparatorOptions.ResultNotEqualElements
//enumerateElementsOptions: EnumerateElementsOptions.LowercaseName
).zSave(@"c:\pib\dev_data\exe\runsource\download\mongo\export\rapide-ddl\compare_RapideDdl_Detail2_23_24_42152.txt");

MongoDB.Bson.BsonElement e = new MongoDB.Bson.BsonElement("toto.tata.tutu", "zozo");
Trace.WriteLine("{0} - {1}", e.Name, e.Value);

Trace.WriteLine("{0}", zdate.ParseDateTimeLikeToday("Aujourd'hui, 07h32", DateTime.Now, @"d/M/yyyy, HH\hmm", @"d-M-yyyy, HH\hmm"));
Trace.WriteLine("{0}", zdate.ParseDateTimeLikeToday("Hier, 12h55", DateTime.Now, @"d/M/yyyy, HH\hmm", @"d-M-yyyy, HH\hmm"));
Trace.WriteLine("{0}", zdate.ParseDateTimeLikeToday("22/02/2014, 21h09", DateTime.Now, @"d/M/yyyy, HH\hmm", @"d-M-yyyy, HH\hmm"));


HttpRun.Load("http://www.ebookdz.com/");
HtmlRun.Select("//div[@id='vba_news4']//div[@class='collapse']:.:EmptyRow");
HtmlRun.Select("//div[@id='vba_news4']//div[@class='collapse']:.:EmptyRow", ".//text()", ".//h2[@class='blockhead']//a[@class!='mcbadge mcbadge_r']//text()");
HtmlRun.Select("//div[@id='vba_news4']//div[@class='collapse']:.:EmptyRow", ".//text()", ".//h2[@class='blockhead']//a[@class!='mcbadge mcbadge_r']//text()", ".//h2[@class='blockhead']//a[1]//text()", ".//h2[@class='blockhead']//a[2]//text()");

Http2.LoadUrl("http://www.ebookdz.com/");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='vba_news4']//div[@class='collapse']:.:EmptyRow");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='vba_news4']//div[@class='collapse']:.:EmptyRow", ".//text()", ".//h2[@class='blockhead']//a[@class!='mcbadge mcbadge_r']//text()");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@id='vba_news4']//div[@class='collapse']:.:EmptyRow", ".//text()", ".//h2[@class='blockhead']//a[@class!='mcbadge mcbadge_r']//text()", ".//h2[@class='blockhead']//a[1]//text()", ".//h2[@class='blockhead']//a[2]//text()");

HttpRun.Load(@"c:\pib\dev_data\exe\runsource\download\sites\ebookdz.com\cache\detail\110000\forum_showthread_t=110839.php");
HtmlRun.Select("//div[@class='body_bd']//div[@id='postlist']:.:EmptyRow");

Http2.LoadUrl(@"c:\pib\dev_data\exe\runsource\download\sites\ebookdz.com\cache\detail\110000\forum_showthread_t=110839.php");
HtmlXmlReader.CurrentHtmlXmlReader.ReadSelect("//div[@class='body_bd']//div[@id='postlist']:.:EmptyRow");

HttpRun.Load("http://www.ebookdz.com/forum/forum.php");
// <ol id="forums" class="floatcontainer">
HtmlRun.Select("//ol[@id='forums']:.:EmptyRow");
HtmlRun.Select("//ol[@id='forums']:.:EmptyRow", ".//text()");
HtmlRun.Select("//ol[@id='forums']/li:.:EmptyRow", ".//text()", ".//a//text()", ".//a/@href");
ebookdz.com_forum_forum.html
ebookdz.com_forum_forum.php_01_01.html
ebookdz.com_forum_forum.php.html

Test.Test_Xml.Test_Xml_f.Test_Select_01();
Test.Test_Xml.Test_Xml_f.Test_Select_02();

Test.Test_Web.Test_Url.Test_Query_01("http://www.ebookdz.com/forum/forumdisplay.php?f=1&s=1fdf76d35a57d09aa11e75ff6f0d9985");
Test.Test_Web.Test_Url.Test_UriBuilder_01("http://www.ebookdz.com/forum/forumdisplay.php?f=1&s=1fdf76d35a57d09aa11e75ff6f0d9985");
Test.Test_Web.Test_Url.Test_PBUriBuilder_01("http://www.ebookdz.com/forum/forumdisplay.php?f=1&s=1fdf76d35a57d09aa11e75ff6f0d9985");
Test.Test_Web.Test_Url.Test_UrlToFileName_01("http://www.ebookdz.com/forum/forumdisplay.php?f=1", ".php", UrlFileNameType.FileName);
Test.Test_Web.Test_Url.Test_UrlToFileName_01("http://www.ebookdz.com/forum/forumdisplay.php?f=1", ".php", UrlFileNameType.Host | UrlFileNameType.FileName);
Test.Test_Web.Test_Url.Test_UrlToFileName_01("http://www.ebookdz.com/forum/forumdisplay.php?f=1", ".php", UrlFileNameType.Host | UrlFileNameType.Path);
Test.Test_Web.Test_Url.Test_UrlToFileName_01("http://www.ebookdz.com/forum/forumdisplay.php?f=1", ".php", UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Query);
Test.Test_Web.Test_Url.Test_UrlToFileName_01("http://www.ebookdz.com/forum/forumdisplay.php?f=1", null, UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Ext | UrlFileNameType.Query);
Test.Test_Web.Test_Url.Test_UrlToFileName_01("http://www.ebookdz.com/forum/forumdisplay.php", null, UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Ext | UrlFileNameType.Query);
Test.Test_Web.Test_Url.Test_UrlToFileName_01("http://www.ebookdz.com/forum/forum.php", ".html", UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Ext | UrlFileNameType.Query);

HttpRun.Load("http://www.ebookdz.com/forum/forumdisplay.php?f=11");
// <ol id="childforum_for_161" class="childsubforum">
// <div class="titleline">
HtmlRun.Select("//ol[@class='childsubforum']/li:.:EmptyRow");
HtmlRun.Select("//ol[@class='childsubforum']/li:.:EmptyRow", ".//text()");
HtmlRun.Select("//ol[@class='childsubforum']/li//div[@class='titleline']//a:.:EmptyRow", ".//text()", "@href");
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz_LoadForumFromWebManager.CurrentLoadForumFromWebManager.LoadMainForum(reload: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz_LoadForumFromWebManager.CurrentLoadForumFromWebManager.LoadMainForum(reload: true));
// 338 forums
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz_LoadForumFromWebManager.CurrentLoadForumFromWebManager.LoadForums(reload: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz_LoadForumFromWebManager.CurrentLoadForumFromWebManager.LoadForums(reload: true));

RunSource.CurrentRunSource.View(Download.Print.Ebookdz.EbookdzForumManager.LoadMainForum(reload: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.EbookdzForumManager.ForumWebDataPageManager.LoadPages(startPage: 1, maxPage: 1, reload: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.EbookdzForumManager.ForumWebDataPageManager.LoadMainForum(reload: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.EbookdzMainForumManager.CurrentMainForumManager.LoadMainForum(reload: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.EbookdzMainForumManager.CurrentMainForumManager.LoadSubForums(reload: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.EbookdzSubForumManager.CurrentSubForumManager.LoadPages(new HttpRequest { Url = "http://www.ebookdz.com/forum/forumdisplay.php?f=11" }, maxPage: 0, reload: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.EbookdzForumHeaderManager.CurrentForumHeaderManager.LoadPages(new HttpRequest { Url = "http://www.ebookdz.com/forum/forumdisplay.php?f=157" }, maxPage: 0, reload: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.EbookdzForumHeaderManager.CurrentForumHeaderManager.LoadPages(new HttpRequest { Url = "http://www.ebookdz.com/forum/forumdisplay.php?f=157" }, maxPage: 0, reload: true));
// la provence
Trace.WriteLine(Download.Print.Ebookdz.EbookdzDetailManager.WebForumHeaderDetailManager.LoadNewDocuments(new HttpRequest { Url = "http://www.ebookdz.com/forum/forumdisplay.php?f=157" }, maxNbDocumentsLoadedFromStore: 0, maxPage: 0, loadImage: true).zToJson());
// Nouveautés en Librairies
Trace.WriteLine(Download.Print.Ebookdz.EbookdzDetailManager.WebForumHeaderDetailManager.LoadNewDocuments(new HttpRequest { Url = "http://www.ebookdz.com/forum/forumdisplay.php?f=450" }, maxNbDocumentsLoadedFromStore: 0, maxPage: 0, loadImage: true).zToJson());
// BD
Trace.WriteLine(Download.Print.Ebookdz.EbookdzDetailManager.WebForumHeaderDetailManager.LoadNewDocuments(new HttpRequest { Url = "http://www.ebookdz.com/forum/forumdisplay.php?f=443" }, maxNbDocumentsLoadedFromStore: 0, maxPage: 0, loadImage: true).zToJson());
// Algorithmique, modélisation, programmation, développement
Trace.WriteLine(Download.Print.Ebookdz.EbookdzDetailManager.WebForumHeaderDetailManager.LoadNewDocuments(new HttpRequest { Url = "http://www.ebookdz.com/forum/forumdisplay.php?f=364" }, maxNbDocumentsLoadedFromStore: 0, maxPage: 0, loadImage: true).zToJson());
Trace.WriteLine(Download.Print.Ebookdz.EbookdzDetailManager.LoadForumNewDocuments(reloadForums: false, maxNbDocumentsLoadedFromStore: 0, maxPage: 0, loadImage: true, filter: forum => forum.Name == "le point").zToJson());

pb.Data.Mongo.TraceMongoCommand.Export("dl", "Ebookdz_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\ebookdz.com\export_Ebookdz_Detail.txt"), sort: "{ _id: 1 }");


HttpRun.Load("http://www.ebookdz.com/forum/forumdisplay.php?f=9");
HtmlRun.Select("//div[@id='forumbits']/ol/li:.:EmptyRow");
HtmlRun.Select("//div[@id='forumbits']/ol/li:.:EmptyRow", ".//div[@class='forumrow']//a");

HttpRun.Load("http://www.ebookdz.com/forum/forumdisplay.php?f=74");
// <div id="threadlist" class="threadlist">
// <ol id="threads" class="threads">
HtmlRun.Select("//div[@id='threadlist']//ol[@id='threads']/li:.:EmptyRow");
// <div class="threadinfo" title="">
// <div class="inner">
// <a title="" class="title" href="showthread.php?t=111210&amp;s=4807e931448c05da34dd54fbd0308479" id="thread_title_111210">L'OPINION du mardi  20 janvier 2015</a>
HtmlRun.Select("//div[@id='threadlist']//ol[@id='threads']/li:.:EmptyRow", ".//div[@class='threadinfo']//a[@class='title']//text()", ".//div[@class='threadinfo']//a[@class='title']/@href");
HtmlRun.Select("//div[@id='above_threadlist']//span[@class='prev_next']//a[@rel='next']/@href:.:EmptyRow");

RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz_LoadForumHeaderPagesManager.CurrentLoadHeaderPagesManager.LoadPages(new HttpRequest { Url = "http://www.ebookdz.com/forum/forumdisplay.php?f=414" }, maxPage: 2, reload: false, loadImage: false, refreshDocumentStore: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz_LoadForumHeaderPagesManager.CurrentLoadHeaderPagesManager.LoadPages(new HttpRequest { Url = "http://www.ebookdz.com/forum/forumdisplay.php?f=181" }, maxPage: 2, reload: false, loadImage: false, refreshDocumentStore: false));
RunSource.CurrentRunSource.View(Download.Print.Ebookdz.Ebookdz_LoadForumHeaderPagesManager.CurrentLoadHeaderPagesManager.LoadPages(new HttpRequest { Url = "http://www.ebookdz.com/forum/forumdisplay.php?f=166" }, maxPage: 4, reload: false, loadImage: false, refreshDocumentStore: false));


Trace.WriteLine(UrlFileNameType.Path.ToString());
Trace.WriteLine((UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Ext | UrlFileNameType.Query | UrlFileNameType.Content).ToString());

pb.Data.Mongo.TraceMongoCommand.Export("dl", "Ebookdz_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\ebookdz.com\export_Ebookdz_Detail.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "Ebookdz_Detail_test", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\ebookdz.com\export_Ebookdz_Detail_test.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "Ebookdz_Detail_test", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\ebookdz.com\export_Ebookdz_Detail.txt"));
pb.Data.Mongo.TraceMongoCommand.Count("dl", "Ebookdz_Detail_test");
Download.Print.Ebookdz.Ebookdz.DetailWebDataManager.RefreshDocumentsStore();
Download.Print.Ebookdz.Ebookdz.DetailWebDataManager.UpdateDocuments(post => { post.Size = "test test test"; }, query: "{ _id: 110839 }");

HttpRun.Load("http://www.ebookdz.com/forum/forumdisplay.php?f=157");
HtmlRun.Select("//div[@class='threadpagenav']//span[@class='prev_next']//a[@rel='next']:.:EmptyRow", "@href");
HtmlRun.Select("//div[@id='threadlist']//ol[@id='threads']/li:.:EmptyRow");

HttpRun.Load("http://www.vosbooks.net/");
HtmlRun.Select("//table[@id='layout']//div[@id='content']/div:.:EmptyRow");
HtmlRun.Select("//div[@class='page-nav']//li[last()]//a[text()='>']:.:EmptyRow");

HttpRun.Load("http://www.vosbooks.net/74299-livre/les-imposteurs-francois-cavanna.html");
HtmlRun.Select("//table[@id='layout']//div[@id='content']/div:.:EmptyRow");
//<div class="post-views post-74299 entry-meta">
HtmlRun.Select("//div[@class='entry']/div[starts-with(@class, 'post-views')]", "@class", ".//text()");
///xml[1]/html[1]/body[1]/div[1]/div[1]/table[1]/tbody[1]/tr[2]/td[2]/div[1]/div[1]/div[1]/div[2]/div[1]/div[1]
///xml[1]/html[1]/body[1]/div[1]/div[1]/table[1]/tbody[1]/tr[2]/td[2]/div[1]/div[1]/div[1]/div[2]/div[1]/h3[1]
HtmlRun.Select("//div[@class='entry']/div[starts-with(@class, 'post-views')]/following-sibling::h3");
HtmlRun.Select("//div[@class='entry']/div[starts-with(@class, 'post-views')]/following-sibling::h3/p:.:EmptyRow");
HtmlRun.Select("//div[@class='entry']/div[starts-with(@class, 'post-views')]/following-sibling::h3/following-sibling::p:.:EmptyRow");
HtmlRun.Select("//div[@class='entry']/div[starts-with(@class, 'post-views')]/following-sibling::h3/following-sibling::p:.:NotEmptyRow");
HtmlRun.Select("//div[@class='entry']/div[starts-with(@class, 'post-views')]/following-sibling::h3/following-sibling::p");
HtmlRun.Select("//div[@class='entry']/div[starts-with(@class, 'post-views')]/following-sibling::h3/following-sibling::p/img:.:EmptyRow");
//Cached page generated by WP-Super-Cache on 2015-01-29 13:15:33
// Magazines Anglais
// sidebox start
HtmlRun.Select("//body/comment()[contains(., 'Cached page')]:.:EmptyRow");
HtmlRun.Select("//body/comment():.:EmptyRow");
HtmlRun.Select("//comment():.:EmptyRow");
HtmlRun.Select("//comment()[starts-with(., 'sidebox')]:.:EmptyRow");
HtmlRun.Select("//comment()[starts-with(., ' Cached')]:.:EmptyRow");
HtmlRun.Select("//text()[starts-with(., 'Magazines')]:.:EmptyRow");
HtmlRun.Select("//body/comment()[starts-with(., ' Cached')]:.:EmptyRow");
HtmlRun.Select("//body/*[starts-with(comment(), 'Cached')]:.:EmptyRow");
HtmlRun.Select("//text():.:EmptyRow");

HttpRun.Load("http://www.vosbooks.net/156-cours-informatique/oracle10g-hotfile.html");
HtmlRun.Select("//body/comment()[contains(., 'Cached page')]:.:EmptyRow");



Uri uri = new Uri("http://www.vosbooks.net/page/2");
Uri uri = new Uri("http://www.vosbooks.net/page/2/");
Uri uri = new Uri("http://www.vosbooks.net/74299-livre/les-imposteurs-francois-cavanna.html");
Uri uri = new Uri("http://www.vosbooks.net/74650-livre/medecine-sante/flash-sante-n-2-2015.html");
Trace.WriteLine(uri.Segments.zToStringValues());

global::Print.PrintTextValuesManager.Trace = true;
DownloadPrint.PrintTextValuesManager.SetExportDataFile(RunSource.CurrentRunSource.Config.Get("TextInfos/ExportDataFile"));
Download.Print.Vosbooks.Vosbooks_DetailManager.Trace = true;
Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager.Load(new WebRequest { HttpRequest = new HttpRequest { Url = "http://www.vosbooks.net/74739-revues-magazines/special-chats-n27-fevrier-mars-avril-2015.html" }, RefreshDocumentStore = true });
Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager.Load(new WebRequest { HttpRequest = new HttpRequest { Url = "http://www.vosbooks.net/74657-journaux/le-monde-ecoentreprise-du-mardi-03-fevrier-2015.html" }, RefreshDocumentStore = true, LoadImage = true });
Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager.Load(new WebRequest { HttpRequest = new HttpRequest { Url = "http://www.vosbooks.net/74299-livre/les-imposteurs-francois-cavanna.html" }, RefreshDocumentStore = true });

Test.Test_Unit.Print.Test_Unit_PrintTextValuesManager.Test_01("PrintTextValues_test_03.txt");
Test.Test_Unit.Print.Test_Unit_PrintTextValuesManager.Test_02("PrintTextValues_test_03.txt");
Test.Test_Unit.Print.Test_Unit_PrintTextValuesManager.Test_02("PrintTextValues_test_01.txt");
Test.Test_Unit.Print.Test_Unit_PrintTextValuesManager.Test_02("PrintTextValues_test_02.txt");
Test.Test_Unit.Print.Test_Unit_PrintTextValuesManager.Test_02("PrintTextValues_golden-ddl.net_01.txt");
Test.Test_Unit.Print.Test_Unit_PrintTextValuesManager.Test_02("PrintTextValues_rapide-ddl.net_01.txt");


HttpRun.Load("http://www.vosbooks.net/74722-revues-magazines/top-sante-n294-mars-2015.html");
HtmlRun.Select("//div[@class='entry']//a", "@href");

Test.Test_CS.Test_GenericConvert.Test_01("123");
Trace.WriteLine("{0}", bool.Parse("True"));
Trace.WriteLine("{0}", bool.Parse("true"));
Trace.WriteLine("{0}", pb.Text.zparse.ParseAs<bool>("true"));

zfile.WriteFile(@"c:\pib\dev_data\exe\runsource\download\mongo\export\vosbooks.net\test1.txt", "1-2\u20133\r\n");
ztrace.TraceBytes(File.ReadAllBytes(@"c:\pib\dev_data\exe\runsource\download\mongo\export\vosbooks.net\test1.txt"));
ztrace.TraceBytes(Encoding.UTF8.GetBytes("1234567890 abcdefghijklmnopqrstuvwxyz"));
Trace.WriteLine(new string(Encoding.UTF8.GetChars(new byte[] { 0xE2, 0x80, 0x93 })));
Trace.WriteLine(new string(Encoding.ASCII.GetChars(new byte[] { 0xE2, 0x80, 0x93 })));
Trace.WriteLine(new string(Encoding.Default.GetChars(new byte[] { 0xE2, 0x80, 0x93 })));
Trace.WriteLine(new string(Encoding.GetEncoding("ibm850").GetChars(new byte[] { 0xE2, 0x80, 0x93 })));

Trace.WriteLine(Encoding.GetEncodings().Select(encodingInfo => new { CodePage = encodingInfo.CodePage, DisplayName = encodingInfo.DisplayName, Name = encodingInfo.Name } ).zToJson());

Trace.WriteLine("{0}", ((int)'a').zToHex());
Trace.WriteLine("{0}", ((int)new string(Encoding.UTF8.GetChars(new byte[] { 0xE2, 0x80, 0x93 }))[0]).zToHex());
ztrace.TraceUnicode("1234567890 abcdefghijklmnopqrstuvwxyz");
ztrace.TraceUnicode("abc\u2013");
ztrace.TraceUnicode(zfile.ReadAllText(@"c:\pib\dev_data\exe\runsource\download\mongo\export\vosbooks.net\test1.txt"));
ztrace.TraceUnicode(zfile.ReadAllText(@"c:\pib\dev_data\exe\runsource\download\mongo\export\vosbooks.net\a11.txt"));
ztrace.TraceUnicode(zfile.ReadAllText(@"c:\pib\dev_data\exe\runsource\download\mongo\export\vosbooks.net\a12.txt"));

HttpRun.Load("http://www.vosbooks.net/74739-revues-magazines/special-chats-n27-fevrier-mars-avril-2015.html");
HttpRun.GetXDocument().zXXElement().XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']").GetPath().zTrace();
HttpRun.GetXDocument().zXXElement().XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']").XPathElement(".//div[@class='entry']").GetPath().zTrace();
HttpRun.GetXDocument().zXXElement().XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']").XPathElement(".//div[@class='entry']")
.XPathElements(".//p").Select(xe => xe.GetPath()).zView();
HttpRun.GetXDocument().zXXElement().XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']").XPathElement(".//div[@class='entry']")
.XPathElements(".//p").DescendantTexts().zView();
HttpRun.GetXDocument().zXXElement().XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']").XPathElement(".//div[@class='entry']")
.XPathElements(".//p").DescendantTexts().Select(DownloadPrint.ReplaceChars).Select(DownloadPrint.TrimWithoutColon).zView();
HttpRun.GetXDocument().zXXElement().XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']").XPathElement(".//div[@class='entry']")
.XPathElements(".//p").DescendantTextNodes().zView();
HttpRun.GetXDocument().zXXElement().XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']").XPathElement(".//div[@class='entry']")
.XPathElements(".//p").DescendantTextNodes(
node =>
{
  if (node is XText)
  {
    string text = ((XText)node).Value.Trim();
    if (text.StartsWith("Lien Direct"))
      return XNodeFilter.Stop;
  }
  return XNodeFilter.SelectNode;
}
).zView();
XmlDescendant.Trace = true;
HttpRun.GetXDocument().zXXElement().XPathElement("//table[@id='layout']//div[@id='content']//div[@class='post']").XPathElement(".//div[@class='entry']")
.XPathElements(".//p").DescendantTextNodes(
node =>
{
  if (node is XText)
  {
    string text = ((XText)node).Value.Trim();
    if (text.StartsWith("Lien Direct"))
      return XNodeFilter.Stop;
  }
  if (node is XElement)
  {
      XElement xe2 = (XElement)node;
      if (xe2.Name == "p" && xe2.zAttribValue("class") == "submeta")
      {
          return XNodeFilter.Stop;
      }
  }
  return XNodeFilter.SelectNode;
}
).zView();



HtmlRun.Select("//div[@class='entry']//a", "@href");

HttpRun.Load("http://www.ebookdz.com/forum/showthread.php?t=111510");

HttpRun.Load(@"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Download\Print\download\download_project.xml");

HttpRun.Load("http://www.vosbooks.net/74299-livre/les-imposteurs-francois-cavanna.html");
Download.Print.Vosbooks.Vosbooks_DetailManager.DetailWebDataManager.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false, query: "{ _id: 74299 }");

pb.Data.Mongo.TraceMongoCommand.Find("dl", "Ebookdz_Detail", "{ \"_id\" : 75543 }", sort: "{ \"download.id\" : 1 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "Ebookdz_Detail", "{ \"_id\" : 75543 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "Ebookdz_Detail", "{ _id: 75543 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "Ebookdz_Detail", "{}", limit: 10);

MongoCommand.Find("dl", "DownloadFile3", "{}", limit: 100, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadFile.key': 1, 'downloadFile.file': 1, 'downloadFile.downloadedFile': 1, 'downloadFile.state': 1, 'downloadFile.startDownloadTime': 1 }").zView();
MongoCommand.Find("dl", "DownloadFile3", "{ 'downloadFile.key.server': 'vosbooks.net' }", limit: 100, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadFile.key': 1, 'downloadFile.file': 1, 'downloadFile.downloadedFile': 1, 'downloadFile.state': 1, 'downloadFile.startDownloadTime': 1 }").zView();
MongoCommand.Find("dl", "DownloadFile3", "{ 'downloadFile.key.server': 'ebookdz.com' }", limit: 100, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadFile.key': 1, 'downloadFile.file': 1, 'downloadFile.downloadedFile': 1, 'downloadFile.state': 1, 'downloadFile.startDownloadTime': 1 }").zView();
var q1 = MongoCommand.Find("dl", "DownloadFile3", "{ 'downloadFile.key.server': 'vosbooks.net' }", limit: 100, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadFile.key': 1, 'downloadFile.file': 1, 'downloadFile.downloadedFile': 1, 'downloadFile.state': 1, 'downloadFile.startDownloadTime': 1 }");
var q2 = MongoCommand.Find("dl", "DownloadFile3", "{ 'downloadFile.key.server': 'ebookdz.com' }", limit: 100, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadFile.key': 1, 'downloadFile.file': 1, 'downloadFile.downloadedFile': 1, 'downloadFile.state': 1, 'downloadFile.startDownloadTime': 1 }");
q1.zJoin(q2, doc => doc.zGet("downloadFile.file"), doc => doc.zGet("downloadFile.file"),
  (doc1, doc2) => new BsonDocument {
    { "file", Path.GetFileName(doc1.zGet("downloadFile.file").zAsString()) },
    { "server1", doc1.zGet("downloadFile.key.server") }, { "server2", doc2.zGet("downloadFile.key.server") },
    // { "downloadedFile1", doc1.zGet("downloadFile.downloadedFile") }, { "downloadedFile2", doc2.zGet("downloadFile.downloadedFile") },
    { "downloadedFile1", Path.GetFileName(doc1.zGet("downloadFile.downloadedFile").zAsString()) }, { "downloadedFile2", Path.GetFileName(doc2.zGet("downloadFile.downloadedFile").zAsString()) },
    { "state1", doc1.zGet("downloadFile.state") }, { "state2", doc2.zGet("downloadFile.state") },
    { "startDownloadTime1", doc1.zGet("downloadFile.startDownloadTime") }, { "startDownloadTime2", doc2.zGet("downloadFile.startDownloadTime") }
  }, JoinType.FullOuterJoin).zView();
//'downloadFile.state': 1, 'downloadFile.startDownloadTime'


// $$info.debrid-link.fr
HttpRun.Load("https://api.debrid-link.fr/rest/token/1R6858wC6lO15X8i/new");
HttpRun.Http.ResultContentType.zTraceJson();
HttpRun.Http.ResultText.zTraceJson();
MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(HttpRun.Http.ResultText).zTraceJson();
Test.Test_Http.Test_DebridLink.Test_DebridLink_01();
//DownloadAutomate_f.CreateDebriderDebridLink().Connect();

DebridLinkFr.Trace = true;
DownloadAutomate_f.CreateDebridLinkFr().GetAccountInfos();
DownloadAutomate_f.CreateDebridLinkFr().GetAccountInfos().zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().GetAccountInfos().zView();

DownloadAutomate_f.CreateDebridLinkFr().GetTorrentActivity().zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().GetTorrentStats().zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().GetTorrentPoints().zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().GetTorrentList().zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().GetTorrentList().zGet("value").zView();
DownloadAutomate_f.CreateDebridLinkFr().AddTorrent("http://pbeuzart.free.fr/torrent/LaFranceBigBrotherLaurentObertone.torrent").zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().StopTorrents("35154fdac83a6876457").zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().ResumeTorrents("35154fdac83a6876457").zTraceJson();

DownloadAutomate_f.CreateDebridLinkFr().GetDownloaderStatus().zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().GetDownloaderStatus().zGet("value.hosters").zView();
DownloadAutomate_f.CreateDebridLinkFr().GetDownloaderTraffic().zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().GetDownloaderTraffic().zGet("value.limit").zView();
DownloadAutomate_f.CreateDebridLinkFr().GetDownloaderStats().zTraceJson();

DownloadAutomate_f.CreateDebridLinkFr().GetDownloaderList().zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().GetDownloaderList().zGet("value").zView();
DownloadAutomate_f.CreateDebridLinkFr().DownloaderAdd("http://ul.to/2o4ntzs4").zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().DownloaderAdd("http://hitfile.net/3P1R").zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().DownloaderAdd("http://uploaded.net/file/9t3ylkvn").zTraceJson();


DownloadAutomate_f.CreateDebridLinkFr().DownloaderRemove("a0bbc06d1b2762166d9139f3d2b81e89b7e78af5").zTraceJson();



DownloadAutomate_f.CreateDebridLinkFr().GetRemoteHostList().zTraceJson();




DownloadAutomate_f.CreateDebridLinkFr().RemoveTorrents("74954f6c4f99eccb849").zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().ExecuteCommand("/account/infos").zTraceJson();
DownloadAutomate_f.CreateDebridLinkFr().ExecuteCommand("/seedbox/list").zTraceJson();
8DownloadAutomate_f.CreateDebridLinkFr().ExecuteCommand("/seedbox/list").zGet("value").zView();
DownloadAutomate_f.CreateDebridLinkFr().ExecuteCommand("/seedbox/ids/[\"44654f844ce2ad4c678\"]/remove").zTraceJson();

DownloadAutomateManager downloadAutomate = DownloadAutomate_f.CreateDownloadAutomate();
downloadAutomate.DownloadManager_v1.DownloadManagerClient.AddDownload("http://seed8-1.debrid-link.fr/dl/131526/82454f844d33fa05447/1/Le+Nouvel+Observateur+No.2565+-+2+Janvier+2014.pdf", @"torrent\Le Nouvel Observateur No.2565 - 2 Janvier 2014.pdf", startNow: true);

HttpRun.Load("https://secure.debrid-link.fr/user/10_a3a206c4398f195283a4843d44f017f3211275e443747173/login");
HttpRun.Load("https://secure.debrid-link.fr/user/2_2d481d8991e4db60f43d24d9d387b75699db7a0157182967/login");
// <form action='' method='POST' class='form-horizontal'>
// <input type='text' class='form-control' name='user'>
// <input type='password' class='form-control' name='password'>
// <input type='hidden' value='10_a3a206c4398f195283a4843d44f017f3211275e443747173' name='token'>
// <button type='submit' name='authorizedToken' value='1' class='btn btn-dl'>Envoyer</button>
HtmlRun.Select("//head//script", "text()");
HtmlRun.Select("//form", "@action", "@method");
HtmlRun.Select("//form//input", "@type", "@name");
HtmlRun.Select("//form//button", "@name", "@value");

DateTime.Now.zTrace();
DateTime.Now.ToLocalTime().zTrace();
zdate.UnixTimeStampToDateTime(1425316478).zTrace();
zdate.UnixTimeStampToDateTime(1984656).zTrace();


byte b = 127;
byte b = 7;
b.ToString("x2").zTrace();
b.ToString("X2").zTrace();
b.ToString("x4").zTrace();
b.ToString("X4").zTrace();
b.ToString("x5").zTrace();
b.ToString("X5").zTrace();
b.zToHex().zTrace();
byte? b = 7;
b.ToString("x2").zTrace();
Convert.ToBase64String(new byte[] { b }).zTrace();
Convert.ToBase32String(new byte[] { b }).zTrace();
Crypt.ComputeSHA1Hash("1418758917/account/infosi619yOI4Kt8WB02g").zToHex(lowercase: true).zTrace();
//ab90fa6a2c9f1bc2bbd7988ff266971b5c10583c
//ab90fa6a2c9f1bc2bbd7988ff266971b5c10583c

DateTime? dt = null;
TimeSpan? ts = null;
dt = DateTime.Now + ts;
Trace.WriteLine(dt != null ? dt.ToString() : "null");

Trace.WriteLine(new BsonDocument[5] as IEnumerable<BsonDocument> != null ? "ok" : "null");
Trace.WriteLine(new BsonDocument[5] as IEnumerable<BsonValue> != null ? "ok" : "null");

DownloadAutomate_f.CreateDownloadManagerClient().GetDownloadCount().zTrace();
DownloadAutomate_f.CreateDownloadManagerClient().GetOrderedDownloadId(0).zTrace();
DownloadAutomate_f.CreateDownloadManagerClient().GetDownloadStateById(8473).zTrace();
DownloadAutomate_f.CreateDownloadManagerClient().GetDownloadRetryCountById(8473).zTrace();
DownloadAutomate_f.CreateDownloadManagerClient().GetMaxRetryCount().zTrace();
PostDownloadLinks.Create(new string[] { 
  "http://ul.to/nyhegfdv", "http://liens.free-telechargement.org/113bj-uploaded", "http://vosprotects.com/linkidwoc.php?linkid=7CZcTi6N9w", "http://www.gboxes.com/lzjn3mn8d4iu/j010215.rar",
  "http://rockfile.eu/6pigp8mv281j.html", "http://www.uploadable.ch/file/G5psncAbNx5z/Gala N 1130 - 4 au 10 Février 2015.pdf" }).zTraceJson();
  
zfile.GetNewIndexedFileName(@"c:\pib\dev_data\exe\runsource\download\log", "_log.txt").zTrace();

DownloadAutomate_f.CreateDownloadAutomate(version: 3, useNewDownloadManager: true, useTestManager: false, traceLevel: null);
Trace.WriteLine("toto");
Ebookdz.Ebookdz.UrlMainPage.zTrace();
Ebookdz.Ebookdz.FakeInit();
Vosbooks.Vosbooks.FakeInit();

ServerManagers.Add("Vosbooks_test", Vosbooks.Vosbooks.CreateServerManager(getPostList: lastRunDateTime => Vosbooks.Vosbooks_DetailManager.DetailWebDataManager.FindDocuments("{ _id: { $gt: 78425 } }", sort: "{ 'download.PostCreationDate': -1 }", loadImage: false)));
Download.Print.DownloadAutomate_f.Test_DownloadAutomate_01(loadNewPost: false, searchPostToDownload: true, uncompressFile: true, sendMail: false,
  version: 3, useNewDownloadManager: true, useTestManager: true, traceLevel: null);
Download.Print.DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(DateTime.Parse("2015-03-25 11:00:00"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadedFile_test", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_DownloadedFile_test.txt"));
pb.Data.Mongo.TraceMongoCommand.Count("dl", "DownloadedFile_test");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.DownloadedFile_test.drop()", "dl");
// queue export
pb.Data.Mongo.TraceMongoCommand.Export("dl", "QueueDownloadFile_new", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_QueueDownloadFile_new.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "CurrentDownloadFile", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_CurrentDownloadFile.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "QueueDownloadFile_new");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "CurrentDownloadFile");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.QueueDownloadFile_new.drop()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.CurrentDownloadFile.drop()", "dl");

pb.Data.Mongo.TraceMongoCommand.Find<DownloadedFile_v1<DownloadPostKey>>("dl", "DownloadFile3", "{}", sort: "{ _id: 1 }").First().zTraceJson();
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadFile3", "{}", sort: "{ _id: 1 }").First().zTraceJson();
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadFile3", "{}", sort: "{ _id: 1 }").Select(doc => doc.zGet("downloadFile").zDeserialize<DownloadedFile_v1<DownloadPostKey>>()).First().zTraceJson();
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadFile3", "{}", sort: "{ _id: 1 }").Select(doc => doc.zGet("downloadFile").zDeserialize<DownloadedFile_v1<DownloadPostKey>>()).Take(10).zView();
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadFile3", "{}", sort: "{ _id: 1 }").Select(doc => doc.zGet("downloadFile").zDeserialize<DownloadedFile_v1<DownloadPostKey>>()).Take(10)
  .Select(df => df.ToDownloadedFile()).zToBsonDocuments().zSave(MongoCommand.GetDatabase("mongodb://localhost", "dl").GetCollection("DownloadedFile_test"));

pb.Data.Mongo.TraceMongoCommand.Import("dl", "DownloadedFile2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_DownloadedFile_08_before_import_DownloadFile3.txt"));
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadFile3", "{}", sort: "{ _id: 1 }").Select(doc => doc.zGet("downloadFile").zDeserialize<DownloadedFile_v1<DownloadPostKey>>()).Take(10)
  .Select(df => df.ToDownloadedFile()).zToBsonDocuments().zSave(MongoCommand.GetDatabase("mongodb://localhost", "dl").GetCollection("DownloadedFile_test"));






pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadedFile_test", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_DownloadedFile_test.txt"));
pb.Data.Mongo.TraceMongoCommand.Count("dl", "DownloadedFile_test");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.DownloadedFile_test.drop()", "dl");

// DownloadedFile : DownloadFile3, DownloadedFile
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadFile3", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_DownloadFile3.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadedFile", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_DownloadedFile.txt"));
pb.Data.Mongo.TraceMongoCommand.Count("dl", "DownloadFile3");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "DownloadedFile");

pb.Data.Mongo.TraceMongoCommand.Import("dl", "DownloadedFile_test", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_DownloadedFile_09_before_import_DownloadFile3.txt"));


// begin code
MongoCollectionManager_v1<DownloadPostKey, DownloadedFile_v1<DownloadPostKey>> mongoDownloadManager_old =
    new MongoCollectionManager_v1<DownloadPostKey, DownloadedFile_v1<DownloadPostKey>>("mongodb://localhost", "dl", "DownloadFile3", "downloadFile");
mongoDownloadManager_old.QueryKey = key => new MongoDB.Driver.QueryDocument { { "downloadFile.key.server", key.server }, { "downloadFile.key._id", BsonValue.Create(key.id) } };

MongoCollectionManager_v1<DownloadPostKey, DownloadedFile<DownloadPostKey>> mongoDownloadedFileManager_new =
  new MongoCollectionManager_v1<DownloadPostKey, DownloadedFile<DownloadPostKey>>("mongodb://localhost", "dl", "DownloadedFile_test", "downloadedFile");
mongoDownloadedFileManager_new.QueryKey = key => new MongoDB.Driver.QueryDocument { { "downloadedFile.Key.server", key.server }, { "downloadedFile.Key._id", BsonValue.Create(key.id) } };

//mongoDownloadManager_old.Find("{}", sort: "{ _id: 1 }").Take(10).zView();
//mongoDownloadManager_old.Find("{}", sort: "{ _id: 1 }").Select(df => df.ToDownloadedFile()).Take(10).zView();
int nbSaved = 0, nbNotSaved = 0;
mongoDownloadManager_old.Find("{}", sort: "{ _id: 1 }").Select(df => df.ToDownloadedFile()).zForEach(
  downloadedFile => { if (DownloadedFile_v1<DownloadPostKey>.Save(mongoDownloadedFileManager_new, downloadedFile)) nbSaved++; else nbNotSaved++; }
);
Trace.WriteLine("nbSaved {0} nbNotSaved {1}", nbSaved, nbNotSaved);
// end code
pb.Data.Mongo.TraceMongoCommand.Count("dl", "DownloadedFile");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "DownloadedFile_test");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "DownloadedFile_old");
pb.Data.Mongo.TraceMongoCommand.RenameCollection("dl", "DownloadedFile", "DownloadedFile_old");
pb.Data.Mongo.TraceMongoCommand.RenameCollection("dl", "DownloadedFile_test", "DownloadedFile");



DownloadedFile<DownloadPostKey> downloadedFile = null;
downloadedFile.Id = mongoDownloadedFileManager.GetNewId();
mongoDownloadedFileManager.Save(downloadedFile.Id, downloadedFile);
DownloadPostKey kkey = null;
downloadedFile = mongoDownloadedFileManager.Load(kkey);

("\"" + "123".PadRight(5) + "\"").zTrace();
("\"" + "123".PadRight(0) + "\"").zTrace();


pb.Data.Mongo.TraceMongoCommand.Count("dl", "QueueDownloadFile_new");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "CurrentDownloadFile");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.QueueDownloadFile_new.drop()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.CurrentDownloadFile.drop()", "dl");
Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.Ebookdz.Ebookdz_DetailManager.DetailWebDataManager, "{ _id: 117468 }", downloadDirectory: "ebookdz.com", uncompressFile: true,
  forceDownloadAgain: false, forceSelect: false, simulateDownload: false, useNewDownloadManager: true, useTestManager: false);

Path.GetDirectoryName("ebookdz.com\\print\\.06_unknow_print\\Le monde + dossier \"départementales 2015 \"\\Le monde + dossier \"départementales 2015 \" - 2015-03-24").zTrace();
Path.GetDirectoryName("ebookdz.com\\print\\.06_unknow_print\\Le monde + dossier départementales 2015 \\Le monde + dossier départementales 2015  - 2015-03-24").zTrace();
Path.GetDirectoryName("Le monde + dossier \"départementales 2015 \"\\Le monde + dossier \"départementales 2015 \" - 2015-03-24").zTrace();
Path.GetDirectoryName("Le monde + dossier départementales 2015  - 2015-03-24").zTrace();

@"(/|\?|%|&|\\|:)+".zTrace();
"Le monde + dossier \"départementales 2015 \" - 2015-03-24".zTrace();
zfile.ReplaceBadFilenameChars("Le monde + dossier \"départementales 2015 \" - 2015-03-24").zTrace();

Trace.WriteLine("toto");
Download_Exe.Test_01();

Compiler.TraceLevel = 1;
Compiler.TraceLevel = 2;
RunSource.CurrentRunSource.Compile_Project(@"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source.git\RunSource\v1\runsource.runsourced32\runsource.runsourced32.project.xml");


pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadedFile", "{ 'downloadedFile.Key': { server : 'vosbooks.net', _id : 81210 } }", limit: 100, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadedFile.Key': 1, 'downloadedFile.State': 1, 'downloadedFile.DownloadedFiles': 1, 'downloadedFile.UncompressFiles': 1 }").zView();
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadedFile", "{ 'downloadedFile.State': 'DownloadFailed' }", limit: 0, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadedFile.Key': 1, 'downloadedFile.State': 1, 'downloadedFile.RequestTime': 1, 'downloadedFile.DownloadedFiles': 1, 'downloadedFile.UncompressFiles': 1 }").zView();
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadedFile", "{ $and: [ { 'downloadedFile.State': 'DownloadFailed' }, { 'downloadedFile.Key.server': 'vosbooks.net' } ] }", limit: 0, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadedFile.Key': 1, 'downloadedFile.State': 1, 'downloadedFile.RequestTime': 1, 'downloadedFile.DownloadedFiles': 1, 'downloadedFile.UncompressFiles': 1 }").zView();
pb.Data.Mongo.TraceMongoCommand.Remove("dl", "DownloadedFile", "{ $and: [ { 'downloadedFile.State': 'DownloadFailed' }, { 'downloadedFile.Key.server': 'vosbooks.net' } ] }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadedFile", "{ $and: [ { 'downloadedFile.State': 'DownloadFailed' }, { 'downloadedFile.Key.server': 'ebookdz.com' } ] }", limit: 0, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadedFile.Key': 1, 'downloadedFile.State': 1, 'downloadedFile.RequestTime': 1, 'downloadedFile.DownloadedFiles': 1, 'downloadedFile.UncompressFiles': 1 }").zView();
pb.Data.Mongo.TraceMongoCommand.Remove("dl", "DownloadedFile", "{ $and: [ { 'downloadedFile.State': 'DownloadFailed' }, { 'downloadedFile.Key.server': 'ebookdz.com' } ] }");

pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadedFile", "{ _id: 2418 }", limit: 100, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadedFile.Key': 1, 'downloadedFile.State': 1, 'downloadedFile.DownloadedFiles': 1, 'downloadedFile.UncompressFiles': 1 }").zView();
pb.Data.Mongo.TraceMongoCommand.Find("dl", "DownloadedFile", "{}", limit: 100, sort: "{ _id: -1 }", fields: "{ _id: 1, 'downloadedFile.Key': 1, 'downloadedFile.State': 1, 'downloadedFile.DownloadedFiles': 1, 'downloadedFile.UncompressFiles': 1 }");


File.Copy(@"c:\pib\prog\tools\runsource\test\new2\ScintillaNET.xml", @"c:\pib\prog\tools\runsource\test\new2\ScintillaNET2.xml");




















DownloadAutomateManager downloadAutomate = DownloadAutomate_f.CreateDownloadAutomate(version: 3, useNewDownloadManager: true, useTestManager: false, traceLevel: 0);
if (downloadAutomate == null)
	Trace.WriteLine("downloadAutomate is null");
else
{
	Trace.WriteLine("downloadAutomate is ok");
	downloadAutomate.Dispose();
}

var downloadManager = DownloadAutomate_f.CreateDownloadManager(useTestManager: false);
if (downloadManager == null)
	Trace.WriteLine("downloadManager is null");
else
{
	Trace.WriteLine("downloadManager is ok");
	downloadManager.Dispose();
}

DownloadManager<DownloadPostKey> downloadManager = new DownloadManager<DownloadPostKey>();
if (downloadManager == null)
	Trace.WriteLine("downloadManager is null");
else
{
	Trace.WriteLine("downloadManager is ok");
	downloadManager.Dispose();
}

var downloadManagerClient = DownloadAutomate_f.CreateDownloadManagerClient(useTestManager: false);
if (downloadManagerClient == null)
	Trace.WriteLine("downloadManagerClient is null");
else
{
	Trace.WriteLine("downloadManagerClient is ok");
	//downloadManagerClient.Dispose();
}

var downloadManager = DownloadAutomate_f.CreateDownloadManager_test(useTestManager: false);
if (downloadManager == null)
	Trace.WriteLine("downloadManager is null");
else
{
	Trace.WriteLine("downloadManager is ok");
	downloadManager.Dispose();
}

var uncompressManager = DownloadAutomate_f.CreateUncompressManager();
if (uncompressManager == null)
	Trace.WriteLine("uncompressManager is null");
else
{
	Trace.WriteLine("uncompressManager is ok");
	//uncompressManager.Dispose();
}


UncompressManager uncompressManager = new UncompressManager();
if (uncompressManager == null)
	Trace.WriteLine("uncompressManager is null");
else
{
	Trace.WriteLine("uncompressManager is ok");
	//uncompressManager.Dispose();
	uncompressManager.Stop();
}
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\PibApp\Pib.project.xml");

zfile.GetNewIndexedFileName(@"c:\pib\prog\tools\runsource\exe\run", "_log.txt").zTrace();

// $$info.dev.c#.doc.format
byte b = 123;
Trace.WriteLine(((byte)123).ToString());
Trace.WriteLine(((byte)123).ToString((string)null));
Trace.WriteLine(((byte)123).ToString("G"));
Trace.WriteLine(((byte)123).ToString("F"));
Trace.WriteLine(((byte)123).ToString("F4"));
Trace.WriteLine(((byte)123).ToString("N"));
Trace.WriteLine(((byte)123).ToString("N4"));

Trace.WriteLine(((int)123).zXmlSerialize());
Test_Class_02 test = new Test_Class_02 { Int = 123, DateTime = DateTime.Parse("2015-01-25 22:33:44") };
Trace.WriteLine(test.zXmlSerialize());

Trace.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
Trace.WriteLine(zapp.GetEntryAssemblyCompany() ?? "null");
Trace.WriteLine(System.Reflection.Assembly.GetEntryAssembly().Location);
Trace.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().Location);
Assembly.GetExecutingAssembly()

Trace.WriteLine(HttpRun.Load("http://localhost:11937/RestTestService.svc/GetString_Get").ResultText);
Trace.WriteLine(HttpRun.Load("http://localhost:11937/RestTestService.svc/GetString_Get_BodyWrapped").ResultText);
Trace.WriteLine(HttpRun.Load(new HttpRequest { Url = "http://localhost:11937/RestTestService.svc/GetString_Post", Method = HttpRequestMethod.Post, Content = "" }).ResultText);
Trace.WriteLine(HttpRun.Load(new HttpRequest { Url = "http://localhost:11937/RestTestService.svc/GetString_Post_BodyWrapped", Method = HttpRequestMethod.Post, Content = "" }).ResultText);
Trace.WriteLine(HttpRun.Load("http://localhost:11937/RestTestService.svc/GetTest?a=toto&b=tata").ResultText);
Trace.WriteLine(HttpRun.Load(new HttpRequest { Url = "http://localhost:11937/RestTestService.svc/PostTest", Method = HttpRequestMethod.Post, Content = "{ 'a': 'toto', 'b': 'tata' }" }).ResultText);
Trace.WriteLine(HttpRun.Load(new HttpRequest { Url = "http://localhost:11937/RestTestService.svc/PostTest", Method = HttpRequestMethod.Post, Content = "{ \"a\": \"toto\", \"b\": \"tata\" }" }).ResultText);
Trace.WriteLine(HttpRun.Load(new HttpRequest { Url = "http://localhost:11937/RestTestService.svc/PostGetTest", Method = HttpRequestMethod.Post, Content = "" }).ResultText);

Trace.WriteLine(System.Reflection.Assembly.ReflectionOnlyLoadFrom(@"c:\pib\drive\google\dev\project\Source\Test\Test.Test_01\Source\Test_wcf\test\Test_wcf_01\Test_wcf_service_2\bin\Test_wcf_service.dll").FullName);
Trace.WriteLine(System.Reflection.Assembly.ReflectionOnlyLoadFrom(@"c:\pib\drive\google\dev\project\Source\Test\Test.Test_01\Source\Test_wcf\test\Test_wcf_02\Test_wcf_service_05\bin\Test_wcf_service_05.dll").FullName);
Trace.WriteLine(typeof(string).FullName);
Trace.WriteLine(typeof(string).AssemblyQualifiedName);

string file = @"c:\pib\_dl\_test\test_long_path\long_path_1234567890_1234567890_1234567890_1234567890\long_path_1234567890_1234567890_1234567890_1234567890\long_path_1234567890_1234567890_1234567890_1234567890\long_path_1234567890_1234567890_1234567890_1234567890\long_path_1234567890_1234567890_1234567890_1234567890\test.txt";
Trace.WriteLine(zfile.ReadAllText(file));


RunSource.CurrentRunSource.SetProjectFile(@"..\..\..\..\Test\Test.Test_01\Source\Test_wcf\Test_wcf\Test_wcf.project.xml");
Test.Test_wcf.Test_wcf_f.Test_wcf_01();
RunSource.CurrentRunSource.SetProjectFile(@"..\..\..\..\..\WebData\Source\Print\Project\download.project.xml");

RunSource.CurrentRunSource.SetProjectFile(@"..\..\..\..\Test\Test.Test_01\Source\Test_Reflection\Test_Reflection.project.xml");
Test.Test_Reflection.Test_Reflection_f.Test_GetCurrentMethod_01();
Test.Test_Reflection.Test_Reflection_f.Test_GetCallerInfo_01();
Test.Test_Reflection.Test_Reflection_f.Test_StackFrame_01();
Test.Test_Reflection.Test_Reflection_f.Test_StackTrace_01();
RunSource.CurrentRunSource.SetProjectFile(@"..\..\..\..\WebData\Source\Print\Project\download.project.xml");

Trace.WriteLine("{0}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

RunSource.CurrentRunSource.Compile_Project(@"..\..\..\..\Test\Test.Test_01\Source\Test_AppConfig\Test_AppConfig.project.xml");

Trace.CurrentTrace.TraceLevel = 0;
Trace.CurrentTrace.TraceLevel = 1;
Trace.WriteLine("TraceLevel {0}", Trace.CurrentTrace.TraceLevel);
HttpRun.Load("http://www.ebookdz.com/");


HttpManager.CurrentHttpManager.ExportResult = false;
HttpManager.CurrentHttpManager.ExportResult = true;
Trace.WriteLine("ExportResult {0} ExportDirectory \"{1}\"", HttpManager.CurrentHttpManager.ExportResult, HttpManager.CurrentHttpManager.ExportDirectory);
HttpRun.Load("http://www.telecharger-magazine.com/index.php");
HttpRun.Load("http://www.telecharger-magazine.com/journaux/3831-journaux-franais-du-17-juillet-2015.html");
HttpRun.Load("http://www.telecharger-magazine.com/informatique/3701-a-la-dcouverte-de-photoshop-spcial-grands-dbutants-.html");
//
Download.Print.TelechargerMagazine.TelechargerMagazine_HeaderManager.HeaderWebDataPageManager.LoadPages(startPage: 1, maxPage: 5, reload: true, loadImage: false, refreshDocumentStore: false).zView();
Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.WebHeaderDetailManager.LoadDetails(startPage: 1, maxPage: 5, reloadHeaderPage: false, reloadDetail: false, loadImage: true, refreshDocumentStore: false).zView();
Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.WebHeaderDetailManager.LoadDetails(startPage: 1, maxPage: 5, reloadHeaderPage: false, reloadDetail: false, loadImage: true, refreshDocumentStore: true).zView();
Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.WebHeaderDetailManager.LoadDetails(startPage: 1, maxPage: 0, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false).zView();
Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.WebHeaderDetailManager.LoadDetails(startPage: 200, maxPage: 0, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false).zView();
Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.WebHeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 25, startPage: 1, maxPage: 20);

pb.Data.Mongo.TraceMongoCommand.Export("dl", "TelechargerMagazine_Detail", Path.Combine(AppData.DataDirectory, @"mongo\export\telecharger-magazine.com\export_TelechargerMagazine_Detail.txt"), sort: "{ '_id': -1 }");


Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.DetailWebDataManager.Load(new pb.Web.Data.WebRequest { HttpRequest = new HttpRequest { Url = "http://www.telecharger-magazine.com/livres/502-tout-sur-les-lgumes-lencyclopdie-des-aliments.html" }, LoadImage = false, RefreshDocumentStore = false, ReloadFromWeb = true }).Document.zTrace();
Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.DetailWebDataManager.Load(new pb.Web.Data.WebRequest { HttpRequest = new HttpRequest { Url = "http://www.telecharger-magazine.com/actualit/117-grand-guide-2014-du-seo.html" }, LoadImage = false, RefreshDocumentStore = false, ReloadFromWeb = true }).Document.zTrace();

Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.DetailWebDataManager, "{ _id: 3872 }", downloadDirectory: "telecharger-magazine.com", uncompressFile: true,
  forceDownloadAgain: true, forceSelect: false, simulateDownload: false, useNewDownloadManager: true, useTestManager: false);
Download.Print.DownloadAutomate_f.Test_TryDownload_02(Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.DetailWebDataManager, "{ _id: 3868 }", uncompressFile: true,
  forceDownloadAgain: false, forceSelect: false, simulateDownload: false, useNewDownloadManager: true, useTestManager: false);

Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.DetailWebDataManager.FindDocuments("{ _id: 3851 }").FirstOrDefault().zToJson().zTrace();
Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.DetailWebDataManager.FindDocuments("{ _id: 3850 }").FirstOrDefault().zToJson().zTrace();

Uri uri = new Uri("http://www.telecharger-magazine.com/page/2/");
Trace.WriteLine(uri.Segments[uri.Segments.Length - 1]);
Uri uri = new Uri("http://www.telecharger-magazine.com/2015/07/17/");
Trace.WriteLine("{0} - {1} - {2} - {3} - {4}", uri.Segments.Length, uri.Segments[0], uri.Segments[1], uri.Segments[2], uri.Segments[3]);
Uri uri = new Uri("http://www.telecharger-magazine.com/journaux/3841-le-monde-eco-et-entreprise-sport-et-forme-du-samedi-18-juillet-2015.html");
Trace.WriteLine("{0} - {1} - {2} - {3}", uri.Segments.Length, uri.Segments[0], uri.Segments[1], uri.Segments[2]);

FilenameNumberInfo.GetFilenameNumberInfo(@"Comment se mettre dans le pétrin en une étape facile - iris miller\[Maïa Perséides - 1] Comment se mettre dans le petri - Iris Miller.epub").zToJson().zTrace();
FilenameNumberInfo.GetFilenameNumberInfo("La_cuisine_italienne_2263041563.pdf").zToJson().zTrace();



zdir.EnumerateDirectoriesInfo(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les 50 meilleures recettes de quiches").zToJson().zTrace();
zdir.EnumerateDirectoriesInfo(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les 50 meilleures recettes de quiches",
	followDirectoryTree: dir => { dir.zToJson().zTrace(); }
	).zToJson().zTrace();
PrintFileManager_v2.GetBonusFiles(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les 50 meilleures recettes de quiches").zToJson().zTrace();
DownloadAutomate_f.CreatePrintFileManager_v2().GetNotBonusFiles(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les fées cuisinent et les lutins dînent").zToJson().zTrace();
DownloadAutomate_f.CreatePrintFileManager_v2().GetFiles(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les fées cuisinent et les lutins dînent", FileFilter.NotBonusFiles).zToJson().zTrace();
DownloadAutomate_f.CreatePrintFileManager_v2().GetNotBonusFiles(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les 50 meilleures recettes de quiches").zToJson().zTrace();
DownloadAutomate_f.CreatePrintFileManager_v2().GetFiles(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les 50 meilleures recettes de quiches", FileFilter.NotBonusFiles).zToJson().zTrace();
DownloadAutomate_f.CreatePrintFileManager_v2().GetNotBonusFiles(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les bienveillantes - jonathan littell").zToJson().zTrace();
DownloadAutomate_f.CreatePrintFileManager_v2().GetFiles(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les bienveillantes - jonathan littell", FileFilter.NotBonusFiles).zToJson().zTrace();
DownloadAutomate_f.CreatePrintFileManager_v2().GetBonusFiles(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les bienveillantes - jonathan littell").zToJson().zTrace();
DownloadAutomate_f.CreatePrintFileManager_v2().GetFiles(@"g:\pib\media\ebook\_dl\_dl_pib\book\10\book\Les bienveillantes - jonathan littell", FileFilter.BonusFiles).zToJson().zTrace();
DownloadAutomate_f.CreatePrintFileManager_v2().GetBonusFiles(@"g:\pib\media\ebook\_dl\_test\_test_bonus").zToJson().zTrace();
DownloadAutomate_f.CreatePrintFileManager_v2().GetFiles(@"g:\pib\media\ebook\_dl\_test\_test_bonus", FileFilter.BonusFiles).zToJson().zTrace();
DownloadAutomate_f.Test_ManageDirectories_01(@"c:\pib\_dl\_pib\dl\print", @"g:\pib\media\ebook\print", usePrintDirectories: true);
DownloadAutomate_f.Test_ManageDirectories_01(@"c:\pib\_dl\_pib\dl\book", @"g:\pib\media\ebook\book\unsorted_verified", usePrintDirectories: false);


@"zozo\bonus\toto".Substring(11).zTrace();
@"zozo\bonus\".Substring(11).zTrace();
@"zozo\bonus".Substring(11).zTrace();

zdir.EnumerateFilesInfo(@"g:\pib\media\ebook\_dl\_test\_dl\book\02\zzbook").zToJson().zTrace();

string dir = @"c:\pib\_dl\_test\toto ";
Trace.WriteLine("dir : \"{0}\"", dir);
string file = Path.Combine(dir, "toto.txt");
Trace.WriteLine("file : \"{0}\"", file);
zfile.CreateFileDirectory(file);
zfile.WriteFile(file, "toto\r\n");
string file2 = Path.Combine(dir, "tata.txt");
Trace.WriteLine("file : \"{0}\"", file2);
zfile.CreateFileDirectory(file2);
zfile.WriteFile(file2, "tata\r\n");

string dir1 = @"c:\pib\_dl\_test\toto1 ";
Trace.WriteLine("dir1 : \"{0}\"", dir1);
string dir2 = zPath.Combine(dir1, "toto2");
Trace.WriteLine("dir2 : \"{0}\"", dir2);
Directory.CreateDirectory(dir2);
zDirectory.CreateDirectory(dir2);
string file = zPath.Combine(dir2, "toto.txt");
Trace.WriteLine("file : \"{0}\"", file);
zfile.CreateFileDirectory(file);
zfile.WriteFile(file, "toto\r\n");
string file2 = zPath.Combine(dir2, "tata.txt");
Trace.WriteLine("file : \"{0}\"", file2);
zfile.CreateFileDirectory(file2);
zfile.WriteFile(file2, "tata\r\n");

$$info.GetPrintTitleInfo

DownloadAutomate_f.CreatePrintTitleManager(version: 5).GetPrintTitleInfo("Le Journal du Dimanche n°3576 du 26 juillet 2015").zTraceJson();
DownloadAutomate_f.CreatePrintTitleManager(version: 5).GetPrintTitleInfo("La Croix Week-End Du Samedi 12 et Dimanche 13 Septembre 2015").zTraceJson();
DownloadAutomate_f.CreatePrintTitleManager().GetPrintTitleInfo("La Croix Week-End Du Samedi 12 & Dimanche 13 Septembre 2015").zTraceJson();
DownloadAutomate_f.CreatePrintTitleManager().GetPrintTitleInfo("Nice Matin Du Mardi 1Er Septembre 2015").zTraceJson();
DownloadAutomate_f.CreatePrintTitleManager().GetPrintTitleInfo("Le Figaro - Samedi 08 &amp; Dimanche 09 Août 2015").zTraceJson();
DownloadAutomate_f.CreatePrintTitleManager().GetPrintTitleInfo("La Dernière Heure Namur du mardi 04 aout 2015").zTraceJson();
DownloadAutomate_f.CreatePrintTitleManager().GetPrintTitleInfo("La Voix Du Nord (Lille) - Lundi 27 Juillet 2015").zTraceJson();
DownloadAutomate_f.CreatePrintTitleManager().GetPrintTitleInfo("Système D Bricothèmes N°16 - Mars 2014").zTraceJson();
DownloadAutomate_f.CreatePrintTitleManager().GetPrintTitleInfo("Le Parisien du dimanche 02 aout").zTraceJson();
DownloadAutomate_f.CreatePrintTitleManager().GetPrintTitleInfo("Le Parisien du dimanche 02 aout + Envies dété").zTraceJson();
DownloadAutomate_f.CreatePrintTitleManager().GetPrintTitleInfo("Le Monde Culture&amp;Idées du samedi 25 juillet 2015").zTraceJson();


DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("DirectMatin-20150925", PrintType.Print, expectedDate: Date.Parse("2015-09-25")).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Journal du Dimanche n°3576 du 26 juillet 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Les Echos + Echos week-end du vendredi 04 et samedi 05 sepembre 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Monde du vendredi 07 aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("20150829_QUO", PrintType.Print, expectedDate: Date.Parse("2015-08-29")).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("20150829_ARH", PrintType.Print, expectedDate: Date.Parse("2015-08-29")).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("vdn_lille_27_08_15", PrintType.Print, expectedDate: Date.Parse("2015-08-27")).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("lesechos_20150903", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("lesechos_20150903", PrintType.Print, expectedDate: Date.Parse("2015-09-03")).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("DirectMatin-20150831", PrintType.Print, expectedDate: Date.Parse("2015-08-31")).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Soir Edition Bruxelles-Peripherie Namurluxembourg Du Lundi 23 Aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Soir Edition Bruxelles-Peripherie + Namurluxembourg + Immo Du Jeudi 27 Aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Monde 3 en 1 du mercredi 02 septembre 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("lesechos_20150903", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("lacr-2015-08-27", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Telegramme Du Dimanche 23 Aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Sud Ouest (Gironde) Du Dimanche 13 Septembre 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Sud Ouest (Bordeaux Rive Gauche) Du Jeudi 10 Septembre 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Sud Ouest (Rive Gauche) Du Mercredi 10 Septembre 2014", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("SUD OUEST bassin d'Arcachon Jeudi 4 septembre 2014", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Sud Ouest (Bordeaux Agglo) Du Mardi 24 Aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("NICE MATIN MERCREDI 12.08.2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Liberation WEEK-END Du Samedi 08 Dimanche 09 Aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Les Echos et les Echos Socit Du Jeudi 20 Aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Les Echos Week- End  - Vendredi 07 &amp; Samedi 08 Août 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Les Echos Business Du Lundi 31 Aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Les Echos + Echos week-end du vendredi 04 et samedi 05 sepembre 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Telegramme (Brest ) Du Jeudi 27 Aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Midi Olympique week-end du 04 au 06 septembre 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Midi Olympique du 31 aout au 06 septembre 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Figaro WEEK-END Du Samedi 12 Dimanche 13 Septembre 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("La Voix Des Sports Du Lundi 07 Septembre 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("La Dernière Heure + supplément  du jeudi 27 aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("La Croix Week-End Du Samedi 12 et Dimanche 13 Septembre 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("La Dernière Heure Namur du mardi 04 aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Monde dossier Hiroshima du jeudi 06 aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager(version: 5).Find("Le Monde Culture&amp;Idées du samedi 25 juillet 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager().Find("Le Monde Culture&amp;Idées du samedi 25 juillet 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager().Find("Le Figaro du jeudi 23 juillet 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager().Find("La Provence Marseille du jeudi 06 aout 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager().Find("Les Echos Sociétés - Jeudi 06 Août 2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager().Find("derni__res_nouvelles_d_alsace_du_vendredi_24_juillet_2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager().Find("sud_ouest__gironde__du_dimanche_26_juillet_2015", PrintType.Print).zToJson().zTrace();
DownloadAutomate_f.CreateFindPrintManager().Find("sud_ouest_bassin_d_arcachon_du_jeudi_06_ao__t_2015", PrintType.Print).zToJson().zTrace();



DownloadAutomate_f.Test_RenamePrintFiles_01(@"g:\pib\media\ebook\_dl\_dl_pib\print\02\print", @"g:\pib\media\ebook\Journaux", simulate: false);
DownloadAutomate_f.Test_RenamePrintFiles_01(@"g:\pib\media\ebook\_dl\_dl_pib\print\02\print", @"g:\pib\media\ebook\Journaux", simulate: true);

zPath.GetDirectoryName(@"..\..\..\..\..\..\RuntimeLibrary\LongPath\LongPath\Pri.LongPath.dll").zTrace();
zPath.GetDirectoryName(@"c:\..\..\..\..\..\..\RuntimeLibrary\LongPath\LongPath\tata.dll").zTrace();
zPath.GetDirectoryName(@"c:\toto\RuntimeLibrary\LongPath\LongPath\tata.dll").zTrace();
zPath.GetDirectoryName(@"..\..\..\..\..\..\RuntimeLibrary\toto\toto.dll").zTrace();
zPath.GetDirectoryName(@"..\..\..\..\Lib\Source\pb\_pb\Application.cs").zTrace();
zPath.GetDirectoryName(@"toto\tata\tutu\zozo.cs").zTrace();
zPath.GetDirectoryName(@"..\toto\tata\tutu\zozo.cs").zTrace();

Path.GetDirectoryName(@"..\..\..\..\..\..\RuntimeLibrary\LongPath\LongPath\Pri.LongPath.dll").zTrace();
Path.GetDirectoryName(@"c:\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto.txt").zTrace();
zPath.GetDirectoryName(@"c:\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto\toto.txt").zTrace();

zdir.EnumerateFilesInfo(@"").zToJson().zTrace();
zPath.GetFileName(@"g:\pib\media\ebook\_dl\_test\_dl\print\01\print\.01_quotidien\L'équipe\L'équipe - 2015-07-23 - no 22284.pdf").zTrace();
zPath.GetFileNameWithoutExtension(@"g:\pib\media\ebook\_dl\_test\_dl\print\01\print\.01_quotidien\L'équipe\L'équipe - 2015-07-23 - no 22284.pdf").zTrace();
FilenameNumberInfo.GetFilenameNumberInfo(@"g:\pib\media\ebook\_dl\_test\_dl\print\01\print\.01_quotidien\L'équipe\L'équipe - 2015-07-23 - no 22284.pdf").zToJson().zTrace();

BsonDocumentComparator.CompareBsonDocumentFiles(
	Test_Unit_PrintTitleManager.GetFile("PrintTitle_TelechargerMagazine_out_bson_01_02.txt"),
	Test_Unit_PrintTitleManager.GetFile("PrintTitle_TelechargerMagazine_out_bson_02.txt"),
	BsonDocumentComparatorOptions.ReturnNotEqualDocuments | BsonDocumentComparatorOptions.ResultNotEqualElements | BsonDocumentComparatorOptions.StringComparisonIgnoreCase | BsonDocumentComparatorOptions.StringComparisonIgnoreWhiteSpace
	)
	.Select(result => result.GetResultDocument())
	.zSave(Test_Unit_PrintTitleManager.GetFile("PrintTitle_TelechargerMagazine_out_bson_compare.txt"));

zmongo.BsonReader<BsonDocument>(Test_Unit_PrintTitleManager.GetFile("PrintTitle_TelechargerMagazine_out_bson_01.txt"))
	.zAction(doc => { BsonElement element = doc.zGetElement("PrintTitleInfo.title"); if (element != null && element.Value.IsString) element.Value = element.Value.AsString.Replace("&", " et "); } )
	.zSave(Test_Unit_PrintTitleManager.GetFile("PrintTitle_TelechargerMagazine_out_bson_01_02.txt"));

BsonDocumentComparator.CompareBsonDocumentFiles(
	Test_Unit_PrintTitleManager.GetFile("PrintTitle_Ebookdz_out_bson_01.txt"),
	Test_Unit_PrintTitleManager.GetFile("PrintTitle_Ebookdz_out_bson_01.txt"),
	BsonDocumentComparatorOptions.ReturnNotEqualDocuments | BsonDocumentComparatorOptions.ResultNotEqualElements | BsonDocumentComparatorOptions.StringComparisonIgnoreCase | BsonDocumentComparatorOptions.StringComparisonIgnoreWhiteSpace
	)
	.Select(result => result.GetResultDocument())
	.zSave(Test_Unit_PrintTitleManager.GetFile("PrintTitle_Ebookdz_out_bson_compare.txt"));


BsonDocumentComparator.CompareBsonDocumentFiles(
	Test_Unit_PrintTitleManager.GetFile("PrintTitle_Vosbooks_out_bson_01_02.txt"),
	Test_Unit_PrintTitleManager.GetFile("PrintTitle_Vosbooks_out_bson_02.txt"),
	BsonDocumentComparatorOptions.ReturnNotEqualDocuments | BsonDocumentComparatorOptions.ResultNotEqualElements | BsonDocumentComparatorOptions.StringComparisonIgnoreCase | BsonDocumentComparatorOptions.StringComparisonIgnoreWhiteSpace
	)
	.Select(result => result.GetResultDocument())
	.zSave(Test_Unit_PrintTitleManager.GetFile("PrintTitle_Vosbooks_out_bson_compare.txt"));

zmongo.BsonReader<BsonDocument>(Test_Unit_PrintTitleManager.GetFile("PrintTitle_Vosbooks_out_bson_01.txt"))
	.zAction(doc =>
	{
		BsonElement element = doc.zGetElement("PrintTitleInfo.title"); if (element != null && element.Value.IsString) element.Value = element.Value.AsString.Replace("&", " et ");
		element = doc.zGetElement("PrintTitleInfo.titleStructure"); if (element != null && element.Value.IsString) element.Value = element.Value.AsString.Replace("&", " et ");
		element = doc.zGetElement("PrintTitleInfo.remainText"); if (element != null && element.Value.IsString) element.Value = element.Value.AsString.Replace("&", " et ");
	} )
	.zSave(Test_Unit_PrintTitleManager.GetFile("PrintTitle_Vosbooks_out_bson_01_02.txt"));





BsonDocumentComparator.CompareBsonDocumentFiles(
	@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Print\PrintTitle\test_01.txt",
	@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Print\PrintTitle\test_02.txt",
	BsonDocumentComparatorOptions.ReturnAllDocuments | BsonDocumentComparatorOptions.ResultAllElements)
	.Select(result => result.GetResultDocument())
	.zSave(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Print\PrintTitle\test_compare.txt");


Type type = typeof(bool?);
//Trace.WriteLine("Type {0} BaseType {1} IsGenericType {2} GetGenericTypeDefinition {3}", type, type.BaseType, type.IsGenericType, type.GetGenericTypeDefinition());
Trace.WriteLine("Type {0} GetGenericArguments().Length {1} GetGenericArguments()[0] {2}", type, type.GetGenericArguments().Length, type.GetGenericArguments()[0]);
Trace.WriteLine("Type {0} IsGenericType {1} GetGenericTypeDefinition() {2} GetGenericTypeDefinition() == typeof(Nullable<>) {3}", type, type.IsGenericType, type.GetGenericTypeDefinition(), type.GetGenericTypeDefinition() == typeof(Nullable<>));
//Type type = abc.GetType().GetGenericArguments()[0];
//type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)

Trace.WriteLine("{0}", zparse.ParseAs<bool>("true"));
Trace.WriteLine("{0}", zparse.ParseAs<bool?>("true"));

XmlConfigElement xmlConfigElement = new XmlConfig(@"c:\pib\drive\google\dev\project\Source\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml").GetConfigElement("/AssemblyProject");
XmlConfigElement xmlConfigElement = new XmlConfig(@"c:\pib\drive\google\dev\project\Source\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml").GetConfigElement("/*");
if (xmlConfigElement != null)
	Trace.WriteLine("Element.zGetPath() {0}", xmlConfigElement.Element.zGetPath());
else
	Trace.WriteLine("null");

XmlConfig.CurrentConfig.XDocument.Root.zGetVariableValue("$DataDir$").zTrace();
RunSource.CurrentRunSource.ProjectFile.zTrace();
new XmlConfig(RunSource.CurrentRunSource.ProjectFile).XDocument.Root.zGetVariableValue("$Project/Root$").zTrace();
new XmlConfig(RunSource.CurrentRunSource.ProjectFile).XDocument.Root.zGetVariableValue("$//Root$").zTrace();
string root = new XmlConfig(RunSource.CurrentRunSource.ProjectFile).XDocument.Root.zGetVariableValue("$//Root$");
//zPath.Combine(zPath.GetDirectoryName(RunSource.CurrentRunSource.ProjectFile), root);
root.zRootPath(RunSource.CurrentRunSource.ProjectDirectory).zTrace();

"toto".zView();
"toto".zTrace();
"toto".zToJson().zTrace();
Trace.WriteLine("toto");
Trace.WriteLine("toto".zToJson());
"toto".zTraceJson();

//*************************************************************************************************************************
//****                                   Test_SharpCompressManager
//*************************************************************************************************************************

Trace.WriteLine(RunSource.CurrentRunSource.ProjectFile);
RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Source\Lib\pb\Source\pb\IO\Test\Test_SharpCompressManager.project.xml");
Test_SharpCompressManager.Test_SharpCompressManager_01();
// CompressionType : None = 0, GZip = 1, BZip2 = 2, PPMd = 3, Deflate = 4, Rar = 5, LZMA = 6, BCJ = 7, BCJ2 = 8, Unknown = 9,
// ArchiveType : Rar = 0, Zip = 1, Tar = 2, SevenZip = 3, GZip = 4
//CompressionType compressionType = CompressionType.None;
CompressionType compressionType = CompressionType.GZip;
//CompressionType compressionType = CompressionType.BZip2;
//CompressionType compressionType = CompressionType.PPMd;
//CompressionType compressionType = CompressionType.Deflate;
//CompressionType compressionType = CompressionType.Rar;
//CompressionType compressionType = CompressionType.LZMA;
//CompressionType compressionType = CompressionType.BCJ;
//CompressionType compressionType = CompressionType.BCJ2;
//CompressionType compressionType = CompressionType.Unknown;
//ArchiveType archiveType = ArchiveType.Zip;
//ArchiveType archiveType = ArchiveType.Rar;
//ArchiveType archiveType = ArchiveType.Tar;
//ArchiveType archiveType = ArchiveType.SevenZip;
ArchiveType archiveType = ArchiveType.GZip;
Test_SharpCompressManager.Test_Compress_01(@"c:\pib\dev_data\exe\runsource\test\Test_SharpCompressManager\Test_SharpCompressManager.gz",
	new string[] { @"c:\pib\dev_data\exe\runsource\test\Test_SharpCompressManager\export_TelechargerMagazine_Detail.txt" },
	archiveType: archiveType, compressionType: compressionType, compressionLevel: CompressionLevel.BestCompression);
string file = @"c:\pib\dev_data\exe\runsource\test\Test_SharpCompressManager\export_TelechargerMagazine_Detail.txt";
string file = @"c:\pib\dev_data\exe\runsource\test\Test_SharpCompressManager\export_TelechargerMagazine_Detail_02.txt";
Test_SharpCompressManager.Test_CompressZip_01(@"c:\pib\dev_data\exe\runsource\test\Test_SharpCompressManager\Test_SharpCompressManager.zip",
	new string[] { file }, compressionLevel: CompressionLevel.BestCompression);

Test_ZipArchive.Test_ZipArchive_01();
Test_ZipArchive.Test_ZipArchive_AddEntry_01(@"c:\pib\dev_data\exe\runsource\test\Test_ZipArchive\Test_ZipArchive_AddEntry_01.zip", "Readme.txt");
Test_ZipArchive.Test_ZipArchive_AddFile_01(@"c:\pib\dev_data\exe\runsource\test\Test_ZipArchive\Test_ZipArchive_AddFile_01.zip",
	@"c:\pib\dev_data\exe\runsource\test\Test_ZipArchive\export_TelechargerMagazine_Detail.txt",
	"export_TelechargerMagazine_Detail.txt",
	System.IO.Compression.CompressionLevel.Optimal, FileMode.OpenOrCreate); // CompressionLevel : Optimal, Fastest, NoCompression
Test_ZipArchive.Test_ZipArchive_AddFile_01(@"c:\pib\dev_data\exe\runsource\test\Test_ZipArchive\Test_ZipArchive_AddFile_01.zip",
	@"c:\pib\dev_data\exe\runsource\test\Test_ZipArchive\export_TelechargerMagazine_Detail.txt",
	"export_TelechargerMagazine_Detail.txt",
	System.IO.Compression.CompressionLevel.Optimal, FileMode.Create); // CompressionLevel : Optimal, Fastest, NoCompression
Test_ZipArchive.Test_ZipArchive_AddFile_01(@"c:\pib\dev_data\exe\runsource\test\Test_ZipArchive\Test_ZipArchive_AddFile_01.zip",
	@"c:\pib\dev_data\exe\runsource\test\Test_ZipArchive\export_TelechargerMagazine_Detail_02.txt",
	@"data\test\toto\export_TelechargerMagazine_Detail_02.txt",
	System.IO.Compression.CompressionLevel.Optimal, FileMode.OpenOrCreate); // CompressionLevel : Optimal, Fastest, NoCompression
Test_ZipArchive.Test_ZipArchive_AddFile_01(@"c:\pib\dev_data\exe\runsource\test\Test_ZipArchive\Test_ZipArchive_AddFile_01.zip",
	@"c:\pib\dev_data\exe\runsource\test\Test_ZipArchive\export_TelechargerMagazine_Detail_02.txt",
	@"c:\pib\dev_data\exe\runsource\test\Test_ZipArchive\export_TelechargerMagazine_Detail_02.txt",
	System.IO.Compression.CompressionLevel.Optimal, FileMode.OpenOrCreate); // CompressionLevel : Optimal, Fastest, NoCompression


//*************************************************************************************************************************
//****                                   Test_Project
//*************************************************************************************************************************

RunSource.CurrentRunSource.SetProject(@"$Root$\Source\Test\Test.Test_01\Source\Test_Project\Test_Project.project.xml");
Test.Test_Project.Test_IncludeProject.Test_01();

Compiler.TraceLevel = 1;
Compiler.TraceLevel = 2;

Trace.WriteLine("ExportResult {0} ExportDirectory \"{1}\"", HttpManager.CurrentHttpManager.ExportResult, HttpManager.CurrentHttpManager.ExportDirectory);
HttpManager.CurrentHttpManager.ExportResult = false;
HttpRun.Load("http://www.telecharger-magazine.com/journaux/3831-journaux-franais-du-17-juillet-2015.html");
HttpRun.Load("http://www.telecharger-magazine.com/science/4588-pour-la-science-n455-septembre-2015.html");
c:\pib\drive\google\dev_data\exe\runsource\download\sites\
HttpRun.Load(@"c:\pib\drive\google\dev_data\exe\runsource\download\sites\telecharger-magazine.com\model\detail\bug_telecharger-magazine.com_science_4588-pour-la-science-n455-septembre-2015_01_01.html");
HttpRun.Load(@"c:\pib\drive\google\dev_data\exe\runsource\download\sites\telecharger-magazine.com\model\detail\bug_telecharger-magazine.com_science_4588-pour-la-science-n455-septembre-2015_01_02.html");

HtmlReader_v2.TraceHtmlReaderFile = null;
HtmlReader_v2.TracePeekChar = false;
HtmlReader_v2.TraceHtmlReaderFile = @"c:\pib\drive\google\dev_data\exe\runsource\download\sites\telecharger-magazine.com\model\detail\TraceHtmlReaderFile.txt";
HtmlReader_v2.TracePeekChar = true;
HtmlReader_v2.TraceHtmlReaderFile.zTrace();
HtmlReader_v2.TracePeekChar.zTrace();

HtmlToXml.HtmlReaderVersion = 1;
HtmlToXml.HtmlReaderVersion = 2;
HtmlToXml.HtmlReaderVersion.zTrace();

HtmlRun.Select("//div");

//string url = @"c:\pib\drive\google\dev_data\exe\runsource\download\sites\telecharger-magazine.com\model\detail\bug_telecharger-magazine.com_science_4588-pour-la-science-n455-septembre-2015_01_01.html";
string url = @"http://www.telecharger-magazine.com/science/4588-pour-la-science-n455-septembre-2015.html";
HttpRequest httpRequest = new HttpRequest { Url = url };
Http http = HttpManager.CurrentHttpManager.Load(httpRequest, new HttpRequestParameters { Encoding = Encoding.UTF8 });
var data = Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.GetData(new WebResult { Http = http, WebRequest = new WebRequest { HttpRequest = httpRequest } });
data.zToJson().zTrace();

Download.Print.TelechargerMagazine.TelechargerMagazine_DetailManager.WebHeaderDetailManager.LoadNewDocuments(startPage: 1, maxPage: 100, maxNbDocumentsLoadedFromStore: 25);


pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "test");
pb.Data.Mongo.TraceMongoCommand.Eval("db.system.js.find()", "test");
pb.Data.Mongo.TraceMongoCommand.Find("test", "system.js", "{}").zTraceJson();
pb.Data.Mongo.TraceMongoCommand.Find("test", "data01", "{}").zTraceJson();
pb.Data.Mongo.TraceMongoCommand.Find("test", "data01", "{}").zView();
pb.Data.Mongo.TraceMongoCommand.Insert("test", "data01", "{ _id: 2, name: 'toto' }");
pb.Data.Mongo.TraceMongoCommand.Insert("test", "data01", "{ _id: 3, name: 'toto', num: myAddFunction(3, 4) }");
pb.Data.Mongo.TraceMongoCommand.Remove("test", "data01", "{ name: 'toto' }");
pb.Data.Mongo.TraceMongoCommand.Count("dl", "DownloadFile3");

pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl");
pb.Data.Mongo.TraceMongoCommand.Find("test", "data01", "{}").zView();
pb.Data.Mongo.TraceMongoCommand.Find("dl", "CurrentDownloadFile", "{}", limit: 10).zView();
pb.Data.Mongo.TraceMongoCommand.Count("dl", "CurrentDownloadFile");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "Download", "{}", limit: 10).zView();
pb.Data.Mongo.TraceMongoCommand.Find("dl", "Download", "{}", sort: "{ _id: 1 }", fields: "{ _id: 1 }", limit: 10).zView();
//pb.Data.Mongo.TraceMongoCommand.Export("dl", "Download", Path.Combine(AppData.DataDirectory, @"mongo\export\id\export_Download.txt"), sort: "{ _id: 1 }");
pb.Data.Mongo.TraceMongoCommand.Find("dl", "IdGenerator", "{}").zTraceJson();


//pb.Data.Mongo.TraceMongoCommand.Eval("db.TestId.drop()", "test");
TraceMongoCommand.Insert("test", "TestId", "{ _id: 0, LastId: 0 }");
TraceMongoCommand.Find("test", "TestId", "{}").zTraceJson();
TraceMongoCommand.Count("dl", "TestId");
TraceMongoCommand.FindAndModify("test", "TestId", "{ _id: 0 }", "{ $inc: { LastId: 1 } }");
TraceMongoCommand.FindAndModify("test", "TestId", "{ _id: 0 }", "{ $inc: { LastId: 1 } }", upsert: true);
TraceMongoCommand.Remove("test", "TestId", "{ _id: 0 }");

//pb.Data.Mongo.TraceMongoCommand.Eval("db.TestId2.drop()", "test");
TraceMongoCommand.Count("dl", "TestId2");
TraceMongoCommand.Find("test", "TestId2", "{}").zTraceJson();
MongoIntIdGenerator.GetNewId("test", "TestId2").zTrace();

TraceMongoCommand.Export("dl", "IdGenerator", Path.Combine(AppData.DataDirectory, @"mongo\export\id\export_IdGenerator.txt"), sort: "{ _id: 1 }");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.IdGenerator.drop()", "test_id");
TraceMongoCommand.Import("test_id", "IdGenerator", Path.Combine(AppData.DataDirectory, @"mongo\export\id\export_IdGenerator.txt"));
TraceMongoCommand.Find("test_id", "IdGenerator", "{}").zView();
MongoIntIdGenerator.TransfertOldLastId("QueueDownloadFile3", "test_id");
TraceMongoCommand.Find("test_id", "QueueDownloadFile3", "{}").zTraceJson();

new UpdateDocument { { "$inc", { "LastId", 1 } } }.zTraceJson();
new UpdateDocument { { "$set", "{ \"LastId\", 1 }" } }.zTraceJson();
new UpdateDocument { { "$set", new BsonDocument { { "LastId", 1 } } } }.zTraceJson();

MongoCommand.GetDatabase(null, "test").GetCollection("zzzz").FindOneAs<BsonDocument>(new QueryDocument { { "collection", "toto" } }).zTraceJson();
MongoCommand.GetDatabase(null, "dl").GetCollection("IdGenerator").FindOneAs<BsonDocument>(new QueryDocument { { "collection", "toto" } }).zTraceJson();
MongoCommand.GetDatabase(null, "dl").GetCollection("IdGenerator").FindOneAs<BsonDocument>(new QueryDocument { { "collection", "DownloadedFile" } }).zTraceJson();


TraceMongoCommand.ExportDatabase("test", Path.Combine(AppData.DataDirectory, @"mongo\export\database\test"));
TraceMongoCommand.ExportDatabase("dl", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl"), sort: "{ _id: 1 }");

TraceMongoCommand.Eval("{ listDatabases: 1 }");
MongoCommand.GetDatabase(server: null, databaseName: "admin").zEval("{ listDatabases: 1 }".zToEvalArgs()).zTraceJson();
MongoCommand.GetDatabase(server: null, databaseName: "admin").zEval("{ listDatabases: 1 }".zToEvalArgs()).zView();
MongoCommand.GetDatabase(server: null, databaseName: "admin").zEval("{ listDatabases: 1 }".zToEvalArgs())["databases"].zView();
TraceMongoCommand.ExportDatabase("dl_test", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl_test"), sort: "{ _id: 1 }");

zmongo.BsonReader<BsonDocument>(@"c:\pib\dev_data\exe\runsource\download\mongo\export\database\dl_01\export_[object Object].txt")
	.OrderBy(doc => doc["_id"])
	.zSave(@"c:\pib\dev_data\exe\runsource\download\mongo\export\database\dl_01\sorted\export_[object Object].txt");
zmongo.BsonReader<BsonDocument>(@"c:\pib\dev_data\exe\runsource\download\mongo\export\database\dl_test_01\export_Images.txt")
	.OrderBy(doc => doc["_id"])
	.zSave(@"c:\pib\dev_data\exe\runsource\download\mongo\export\database\dl_test_01\export_Images_02.txt");



foreach (string file in zDirectory.EnumerateFiles(@"c:\pib\dev_data\exe\runsource\download\mongo\export\database\dl_01"))
{
	//file.zTrace();
	string file2 = zPath.Combine(zPath.GetDirectoryName(file), "sorted", zPath.GetFileName(file));
	file2.zTrace();
	// .AsInt32
	zmongo.BsonReader<BsonDocument>(file).OrderBy(doc => doc["_id"]).zSave(file2);
}

pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl");


pb.Data.Mongo.TraceMongoCommand.Eval("{ listDatabases: 1 }");
MongoCommand.GetDatabase(server: null, databaseName: "admin").zEval("{ listDatabases: 1 }".zToEvalArgs())["databases"].zView();
pb.Data.Mongo.TraceMongoCommand.Eval("db.dropDatabase()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.copyDatabase('dl_test', 'dl')");
"".zTrace();

MongoIntIdGenerator.TransfertOldLastId("Download", "dl");
TraceMongoCommand.Export("dl", "Download", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl\export_Download.txt"), sort: "{ _id: 1 }");
MongoIntIdGenerator.TransfertOldLastId("Download2", "dl");
TraceMongoCommand.Export("dl", "Download2", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl\export_Download2.txt"), sort: "{ _id: 1 }");
MongoIntIdGenerator.TransfertOldLastId("DownloadFile3", "dl");
TraceMongoCommand.Export("dl", "DownloadFile3", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl\export_DownloadFile3.txt"), sort: "{ _id: 1 }");
MongoIntIdGenerator.TransfertOldLastId("DownloadedFile", "dl");
TraceMongoCommand.Export("dl", "DownloadedFile", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl\export_DownloadedFile.txt"), sort: "{ _id: 1 }");
MongoIntIdGenerator.TransfertOldLastId("DownloadedFile3", "dl");
TraceMongoCommand.Export("dl", "DownloadedFile3", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl\export_DownloadedFile3.txt"), sort: "{ _id: 1 }");
MongoIntIdGenerator.TransfertOldLastId("DownloadedFile_test", "dl");
TraceMongoCommand.Export("dl", "DownloadedFile_test", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl\export_DownloadedFile_test.txt"), sort: "{ _id: 1 }");
MongoIntIdGenerator.TransfertOldLastId("QueueDownloadFile3", "dl");
TraceMongoCommand.Export("dl", "QueueDownloadFile3", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl\export_QueueDownloadFile3.txt"), sort: "{ _id: 1 }");
MongoIntIdGenerator.TransfertOldLastId("QueueDownloadFile4", "dl");
TraceMongoCommand.Export("dl", "QueueDownloadFile4", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl\export_QueueDownloadFile4.txt"), sort: "{ _id: 1 }");
MongoIntIdGenerator.TransfertOldLastId("QueueDownloadFile_new", "dl");
TraceMongoCommand.Export("dl", "QueueDownloadFile_new", Path.Combine(AppData.DataDirectory, @"mongo\export\database\dl\export_QueueDownloadFile_new.txt"), sort: "{ _id: 1 }");

//db.copyDatabase('dl', 'dl_test2')
//pb.Data.Mongo.TraceMongoCommand.Eval("db.IdGenerator.drop()", "dl");





Trace.WriteLine(RunSource.CurrentRunSource.ProjectFile);
RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Source\Lib\pb\Source\pb\IO\Test\Test_utf8.project.xml");
Test_utf8.Test_utf8_01();

XmlConfig.CurrentConfig.ConfigFile.zTrace();

QueryDocument queryDocument = "{ 'downloadedFile.Key.server': 'extreme-down.net' }".zToQueryDocument();
QueryDocument queryDocument = "{}".zToQueryDocument();
//string collection = "DownloadedFile";
string collection = "QueueDownloadFile_new";
new MongoIntIdGenerator(collection, "dl").GetQueryToSkipIdGeneratorDocument(queryDocument).zTraceJson();

QueryDocument queryDocument = "{ 'downloadedFile.Key.server': 'extreme-down.net' }".zToQueryDocument();
QueryDocument queryDocument = "{}".zToQueryDocument();
//string collection = "DownloadedFile";
string collection = "QueueDownloadFile_new";
QueryDocument queryDocument2 = new MongoIntIdGenerator(collection, "dl").GetQueryToSkipIdGeneratorDocument(queryDocument);
zmongo.GetCollection(collection, "dl").zFind<BsonDocument>(queryDocument2, limit: 10, sort: "{ _id: -1 }".zToSortByWrapper()).zView();

QueryDocument queryDocument = "{ _id: 12 }".zToQueryDocument();
zmongo.GetCollection("DownloadedFile", "dl").zFind<BsonDocument>(queryDocument, limit: 10, sort: "{ _id: -1 }".zToSortByWrapper()).zView();



MongoCollectionManager_v1<DownloadPostKey, QueueDownloadFile<DownloadPostKey>> mongoQueueDownloadFileManager =
new MongoCollectionManager_v1<DownloadPostKey, QueueDownloadFile<DownloadPostKey>>(XmlConfig.CurrentConfig.GetElement("DownloadAutomateManager/MongoQueueDownloadFile_new"));
mongoQueueDownloadFileManager.Find("{}").zTraceJson();

string file = @"$Root$\Source\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml".zGetRunSourceProjectVariableValue().zRootPath(RunSource.CurrentRunSource.ProjectDirectory);
file.zTrace();
//var config = new XmlConfig(file);
//Trace.WriteLine("{0}", config.ConfigFile);
var element = new XmlConfig(@"c:\pib\drive\google\dev\project\Source\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml").GetConfigElement("/*");
Trace.WriteLine("{0} - {1}", element.Element.zGetPath(), element.Element.Name);
var element = new XmlConfig(@"c:\pib\drive\google\dev\project\Source\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml").GetConfigElement("/AssemblyProject");
Trace.WriteLine("{0} - {1}", element.Element.zGetPath(), element.Element.Name);

XDocument xdocument = XDocument.Load(@"$Root$\Source\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml".zGetRunSourceProjectVariableValue());
Trace.WriteLine("{0}", xdocument.Root.Name);

Trace.WriteLine("{0}", RunSource.CurrentRunSource.GetProjectConfig().ConfigFile);

@"$Roaaot$\Source\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml".zGetRunSourceProjectVariableValue().zTrace();
@"$Roaaot$\Source\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml".zGetRunSourceProjectVariableValue(throwError: true).zTrace();
@"$Root$\Source\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml".zGetRunSourceProjectVariableValue().zRootPath(RunSource.CurrentRunSource.ProjectDirectory).zTrace();


RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Source\Test\Test.Test_01\Source\Test_Reflection\Test_Reflection.project.xml");
Test_Assembly.Test_Assembly_01();
Test_Assembly.Test_Assembly_02();
Test_Assembly.Test_Assembly_Module_01();
Test_Assembly.Test_Assembly_Type_01();
Test_Assembly.Test_Assembly_Type_02();
Test_Assembly.Test_Assembly_Type_03("runsource.irunsource.dll");
Test_Assembly.Test_AppDomain_Assembly_01();
Test_Assembly.Test_AppDomain_Assembly_02();
Test_Assembly.Test_GetType_01("zPath");  // not found
Test_Assembly.Test_GetType_01("pb.IO.zPath"); // not found
Test_Assembly.Test_GetType_01("pb.IO.zPath, runsource.irunsource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"); // found
Test_Assembly.Test_GetType_01("Test.Test_Reflection.w");  // found
Test_Assembly.Test_GetType_01("System.Int32");  // System.Int32 : found, System.int : not found, int : not found
Test_Assembly.Test_GetType_01("int");  // 

Type.GetType("pb.IO.zPath, runsource.irunsource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null").AssemblyQualifiedName.zTrace();
Type.GetType("Download.Print._RunCode, ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").AssemblyQualifiedName.zTrace();


Assembly.GetCallingAssembly().FullName.zTrace();   // Assembly.GetCallingAssembly()   : FullName = "runsource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
Assembly.GetEntryAssembly().FullName.zTrace();     // Assembly.GetEntryAssembly()     : FullName = "runsource.runsource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
Assembly.GetExecutingAssembly().FullName.zTrace(); // Assembly.GetExecutingAssembly() : FullName = "RunCode_00007, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
Assembly.ReflectionOnlyLoadFrom(@"c:\pib\drive\google\dev\project\Source\Apps\WebData\Source\Print\Project\bin32\ebook.download.dll").FullName.zTrace(); // "ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
Assembly assembly = Assembly.GetCallingAssembly();
Trace.WriteLine("FullName : \"{0}\"", assembly.FullName);
Trace.WriteLine("GetName :");
assembly.GetName().zTraceJson();

Reflection.GetMethodElements("Init").zTraceJson();
Reflection.GetMethodElements("Test._RunCode.Init").zTraceJson();
Reflection.GetMethodElements("Test._RunCode.Init, ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").zTraceJson();



RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Source\Test\Test.Test_01\Source\Test_CS\Test_cs4_project.xml");


string s = null;
s.Substring(10);

int i = 0x0010;
int i = 0x001F;
i.zToHex().zTrace();
(i >> 4).zToHex().zTrace();
(i >> 4 << 4).zToHex().zTrace();

RunSource.CurrentRunSource.UseNewRunCode.zTrace();
RunSource.CurrentRunSource.UseNewRunCode = false;
RunSource.CurrentRunSource.UseNewRunCode = true;

AppDomain.CurrentDomain.GetAssemblies().Select(assembly => new { IsDynamic = assembly.IsDynamic, FullName = assembly.FullName, Location = !assembly.IsDynamic ? assembly.Location : null }).zTraceJson();


Download_Exe.Test_rv_01();

Trace.WriteLine(AppData.DataDirectory);

zDirectory.Delete(@"c:\pib\drive\google\dev\project\Source\Apps\WebData\Source\Print\Project\bin32\copySource\Source\Apps\WebData", true);


//*************************************************************************************************************************
//****                                   CopyProjectSourceFiles - ZipProjectSourceFiles
//*************************************************************************************************************************
RunSource.CurrentRunSource.CopyProjectSourceFiles(@"$Root$\Source\Apps\WebData\Source\Print\Project\download.project.xml", @"bin32\copySource");
RunSource.CurrentRunSource.ZipProjectSourceFiles(@"$Root$\Source\Apps\WebData\Source\Print\Project\download.project.xml", @"bin32\source.zip");


Trace.WriteLine("toto");
