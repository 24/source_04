//*************************************************************************************************************************
//****                                   Automate
//*************************************************************************************************************************
// use new download manager
DownloadAutomate_f.Test_DownloadAutomate_01(loadNewPost: true, searchPostToDownload: true, uncompressFile: true, sendMail: false,
  version: 3, useNewDownloadManager: true, traceLevel: 0);
//*************************************************************************************************************************
//*************************************************************************************************************************


Trace.CurrentTrace.TraceLevel = 0;
Trace.CurrentTrace.TraceLevel = 1;

Compiler.TraceLevel = 1;
Compiler.TraceLevel = 2;

Trace.WriteLine("toto");
Trace.WriteLine(Trace.CurrentTrace.GetLogFile());

RunSource.CurrentRunSource.Compile_Project("magazine3k_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\pt\pt1_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\pt\pt_project.xml");

RunSource.CurrentRunSource.Compile_Project(@"..\..\..\DownloadManager\Core\MyDownloader.Core_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\DownloadManager\Extension\MyDownloader.Extension_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\DownloadManager\IEPlugin\MyDownloader.IEPlugin_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\DownloadManager\Spider\MyDownloader.Spider_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\..\DownloadManager\App\MyDownloader.App_project.xml");


_rs.Trace.SetTraceDirectory(@"c:\pib\dev_data\exe\wrun\test\http");
_rs.Trace.SetTraceDirectory(null);

//*************************************************************************************************************************
//****                                   Automate
//*************************************************************************************************************************

RunSource.CurrentRunSource.SetProject("download.project.xml");
Trace.WriteLine(RunSource.CurrentRunSource.ProjectFile);
Trace.WriteLine(RunSource.CurrentRunSource.ProjectDirectory);


// use new download manager, send mail
Download.Print.DownloadAutomate_f.Test_DownloadAutomate_01(loadNewPost: true, searchPostToDownload: true, uncompressFile: true, sendMail: true,
  version: 3, useNewDownloadManager: true, traceLevel: 0);
// use old download manager
//Download.Print.DownloadAutomate_f.Test_DownloadAutomate_01(loadNewPost: true, searchPostToDownload: true, uncompressFile: true, sendMail: false,
//  version: 3, useNewDownloadManager: false, traceLevel: 0);
// dont search post to download
//Download.Print.DownloadAutomate_f.Test_DownloadAutomate_01(loadNewPost: true, searchPostToDownload: false, uncompressFile: true, sendMail: false,
//  version: 3, useNewDownloadManager: false, traceLevel: 0);
//Download.Print.DownloadAutomate_f.Test_DownloadAutomate_01(loadNewPost: true, uncompressFile: true, sendMail: false);

DownloadAutomate_f.GetMongoDownloadAutomateManager().GetLastRunDateTime().zTrace();
DownloadAutomate_f.GetMongoDownloadAutomateManager().GetNextRunDateTime().zTrace();
DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(DateTime.Parse("2015-09-14 08:00:00"));
DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(DateTime.Parse("2015-03-22 00:00:00"));
DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(DateTime.Parse("2015-01-10 00:00:00"));
DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(DateTime.Parse("2015-03-01 00:00:00"));
DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(DateTime.Parse("2014-11-16 00:00:00"));
DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(DateTime.Parse("2014-10-01 12:00:00"));
DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(new DateTime(2014, 9, 13, 20, 0, 0));
DownloadAutomate_f.GetMongoDownloadAutomateManager().SetLastRunDateTime(new DateTime(2014, 8, 25, 20, 0, 0));
DownloadAutomate_f.GetMongoDownloadAutomateManager().SetTimeBetweenRun(TimeSpan.FromHours(1));
DownloadAutomate_f.GetMongoDownloadAutomateManager().SetTimeBetweenRun(TimeSpan.FromMinutes(30));

//*************************************************************************************************************************
//****                                   Automate mongo
//*************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Eval("{ listDatabases: 1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.IdGenerator.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "IdGenerator", "{}");
pb.Data.Mongo.TraceMongoCommand.Update("dl", "IdGenerator", "{ collection: 'Download2' }", "{ $set: { lastId: 49 } }", MongoDB.Driver.UpdateFlags.Upsert | MongoDB.Driver.UpdateFlags.Multi);
pb.Data.Mongo.TraceMongoCommand.Eval("db.IdGenerator.count()", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "IdGenerator", "{}");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "DownloadAutomate", "{}");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "DownloadAutomate2", "{}");
pb.Data.Mongo.TraceMongoCommand.Eval("db.DownloadAutomate.count()", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Download", "{}", limit: 5);
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Download", "{}", limit: 5, sort: "{ 'downloadPost.post.creationDate': -1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Download2", "{}", limit: 5, sort: "{ 'downloadPost.post.creationDate': -1 }", fields: "{ 'downloadPost.post.loadFromWebDate': 1, 'downloadPost.post.creationDate': 1, 'downloadPost.post.title': 1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Download2", "{}", limit: 15, sort: "{ '_id': 1 }", fields: "{ 'downloadPost.post.loadFromWebDate': 1, 'downloadPost.post.creationDate': 1, 'downloadPost.post.title': 1 }");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "Download2", "{}", limit: 15, sort: "{ '_id': -1 }", fields: "{ 'downloadPost.post.loadFromWebDate': 1, 'downloadPost.post.creationDate': 1, 'downloadPost.post.title': 1 }");
pb.Data.Mongo.TraceMongoCommand.Eval("db.Download.count()", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.Download.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "Download2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_Download.txt"));

Trace.WriteLine("{0}", Download.Print.DownloadAutomate_f.GetDate("LES JOURNAUX - VENDREDI 15 / 16 AOUT 2014 & + [PDF][Lien Direct]")); 
Trace.WriteLine("{0}", Download.Print.DownloadAutomate_f.GetDate("L'Equipe du vendredi 15 ao?t 2014")); 
Trace.WriteLine("{0}", Download.Print.DownloadAutomate_f.GetDate("Marianne N?904 - 15 au 21 Aout 2014")); 
Trace.WriteLine("{0}", Download.Print.DownloadAutomate_f.GetDate("Le Point N? 2187 - Du 14 au 20 Ao?t 2014")); 

//*************************************************************************************************************************
//****                                   RapideDdl
//*************************************************************************************************************************

// load new post
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 7, startPage: 1, maxPage: 20);
// load all post
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 0, startPage: 1, maxPage: 0);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 0, startPage: 415, maxPage: 0, loadImage: false);
// refresh all post
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 0, reloadFromWeb: false, loadImage: false);



Download.Print.RapideDdl.RapideDdl_LoadPostDetail.LoadNewDocuments(maxNbDocumentLoadedFromStore: 20);
Download.Print.RapideDdl.RapideDdl_LoadPostDetail.RefreshDocumentsStore(limit: 10, reloadFromWeb: false);
// view last post from mongo
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 100, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 1000, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01(limit: 10, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_02(limit: 0, loadImage: true);

// test
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

Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadHeaderPages_01(startPage: 1, maxPage: 2, reload: true, loadImage: true);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_01_old(startPage: 1, maxPage: 2, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_02_old(startPage: 1, maxPage: 2, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false);
Download.Print.RapideDdl.RapideDdl_Exe.Test_RapideDdl_LoadDetailItemList_02_old(startPage: 1, maxPage: 10, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false);


//*************************************************************************************************************************
//****                                   mongo pierre2
//*************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Eval("{ listDatabases: 1 }", server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl", server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "IdGenerator", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\IdGenerator\export_IdGenerator.txt"), server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Export("dl", "DownloadAutomate", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\DownloadAutomate\export_DownloadAutomate.txt"), server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "DownloadAutomate2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\DownloadAutomate\export_DownloadAutomate_03_Pierre2.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "Download", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_Download.txt"), server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "Download2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_Download_03_Pierre2.txt"));
pb.Data.Mongo.TraceMongoCommand.Import("dl", "Download2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\Download\export_Download_03_Pierre2_bug.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail.txt"));
pb.Data.Mongo.TraceMongoCommand.Export("dl", "RapideDdl_Detail", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail.txt"), server: "mongodb://pierre2");
pb.Data.Mongo.TraceMongoCommand.Import("dl", "RapideDdl_Detail2", Path.Combine(RunSource.CurrentRunSource.DataDir, @"mongo\export\rapide-ddl\export_RapideDdl_Detail_03_Pierre2.txt"));
pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail.count()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail.count()", "dl", server: "mongodb://pierre2");

//*************************************************************************************************************************
//****                                   RapideDdl mongo
//*************************************************************************************************************************

pb.Data.Mongo.TraceMongoCommand.Eval("db.getCollectionNames()", "dl");
pb.Data.Mongo.TraceMongoCommand.Eval("db.runCommand( { count: 'RapideDdl_Detail' } )", "dl");
//pb.Data.Mongo.TraceMongoCommand.Eval("db.RapideDdl_Detail.drop()", "dl");
pb.Data.Mongo.TraceMongoCommand.FindAs("dl", "RapideDdl_Detail", "{}", limit: 5, sort: "{ 'download.creationDate': -1 }");
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
Download.Print.DownloadManager_f.Test_DownloadService_RemoveDownloadById_01(int id);
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

pb.Data.Mongo.TraceMongoCommand.Eval("test", "db.getCollectionNames()");
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

Test.Test_Unit.Test_Unit_BsonDocumentsToDataTable.Test(@"c:\pib\dev_data\exe\runsource\test_unit");
Test.Test_Unit.Test_Unit_BsonReader.Test(@"c:\pib\dev_data\exe\runsource\test_unit");
Test.Test_Unit.Test_Unit_PBBsonReader.Test_TextReader(@"c:\pib\dev_data\exe\runsource\test_unit");
Test.Test_Unit.Test_Unit_PBBsonReader.Test_CloneTextReader(@"c:\pib\dev_data\exe\runsource\test_unit");
Test.Test_Unit.Test_Unit_PBBsonReader.Test_BookmarkTextReader(@"c:\pib\dev_data\exe\runsource\test_unit");
Test.Test_Unit.Test_Unit_PBBsonReader.Test_BinaryReader(@"c:\pib\dev_data\exe\runsource\test_unit");
Test.Test_Unit.Test_Unit_PBBsonReader.Test_CloneBinaryReader(@"c:\pib\dev_data\exe\runsource\test_unit");
Test.Test_Unit.Test_Unit_PBBsonReader.Test_BookmarkBinaryReader(@"c:\pib\dev_data\exe\runsource\test_unit");
Test.Test_Unit.Test_Unit_PBBsonEnumerateValues.Test(@"c:\pib\dev_data\exe\runsource\test_unit");

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
Trace.WriteLine(RunSource.CurrentRunSource.ProjectName);
RunSource.CurrentRunSource.ProjectName = "download_project.xml";

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

Test.Test_Reflection.Test_Reflection_f.Test_TraceType_01();

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
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://ul.to/epws504d");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://uploadrocket.net/zc3b4205na1v/DF475.rar.html");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://www.uploadable.ch/file/BgeEV6KxnCbB/VPFN118.rar");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://rapidgator.net/file/096cca55aba4d9e9dac902b9508a23b1/MiHN65.rar.html");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("http://turbobit.net/15cejdxrzleh.html");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("");
Test.Test_Http.Test_AllDebrib.Test_AllDebrib_Link_01("");

Test.Test_Web.Test_Mail_f.Test_Mail_01();
Test.Test_Web.Test_Mail_f.Test_Mail_02("pierre.beuzart@gmail.com", "test 3", "test 3");

Http2.LoadUrl(@"c:\pib\dev_data\exe\runsource\download\sites\rapide-ddl\cache\detail\39000\ebooks_magazine_39023-multi-lautomobile-no821-octobre-2014.html");


RunSource.CurrentRunSource.SetProjectFile("download.project.xml");
Trace.WriteLine(RunSource.CurrentRunSource.ProjectFile);
Trace.WriteLine(RunSource.CurrentRunSource.ProjectDirectory);

RunSource.CurrentRunSource.SetProjectFile("test.project.xml");

Trace.WriteLine("ExportResult {0} ExportDirectory \"{1}\"", HttpManager.CurrentHttpManager.ExportResult, HttpManager.CurrentHttpManager.ExportDirectory);
HttpManager.CurrentHttpManager.ExportResult = true;
HttpRun.Load("http://www.telecharger-magazine.com/journaux/3831-journaux-franais-du-17-juillet-2015.html");
HttpRun.Load("http://www.telecharger-magazine.com/science/4588-pour-la-science-n455-septembre-2015.html");

HtmlRun.Select("//div");

Type.GetType("Download.Print._RunCode, ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").AssemblyQualifiedName.zTrace();
Reflection.GetType("Download.Print._RunCode, ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ErrorOptions.TraceWarning).AssemblyQualifiedName.zTrace();
Reflection.GetType("ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", "Download.Print._RunCode", ErrorOptions.TraceWarning).AssemblyQualifiedName.zTrace();
Reflection.GetAssembly("ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", ErrorOptions.TraceWarning).FullName.zTrace();
Reflection.GetMethodElements("Download.Print._RunCode.Init, ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").zTraceJson();

AppDomain.CurrentDomain.GetAssemblies().Select(assembly => new { IsDynamic = assembly.IsDynamic, FullName = assembly.FullName, Location = !assembly.IsDynamic ? assembly.Location : null }).zTraceJson();

AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName == "ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").First().FullName.zTrace();
AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.FullName == "ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").First().GetType("Download.Print._RunCode").AssemblyQualifiedName.zTrace();

LoadDll.Fake();
Assembly.Load("ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null").FullName.zTrace();

Trace.WriteLine("toto");
