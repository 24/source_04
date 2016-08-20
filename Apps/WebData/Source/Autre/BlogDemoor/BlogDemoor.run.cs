// $$info.


WebData.CreateDataManager().HeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 5, startPage: 1, maxPage: 0, loadImageFromWeb: true).zTraceJson();
WebData.Backup();

TraceMongoCommand.Export("BlogDemoor", "BlogDemoor_Detail", @"c:\pib\drive\google\map\voyage Demoor\mongo\export_BlogDemoor_Detail.txt", sort: "{ '_id': -1 }");
TraceMongoCommand.Export("BlogDemoor", "Images", @"c:\pib\drive\google\map\voyage Demoor\mongo\export_Images.txt", sort: "{ '_id': -1 }");



//*************************************************************************************************************************
//****                                   Trace
//*************************************************************************************************************************

Trace.WriteLine("toto");
"toto".zView();

RunSource.CurrentRunSource.SetProjectFromSource();

RunSource.CurrentRunSource.CompileProject(@"$Root$\Lib\pb\Source\Project\Extension_01.project.xml");


Http.Trace = true;
HttpManager.CurrentHttpManager.ExportResult = false;
HttpManager.CurrentHttpManager.ExportResult = true;
Trace.WriteLine("ExportResult {0} ExportDirectory \"{1}\"", HttpManager.CurrentHttpManager.ExportResult, HttpManager.CurrentHttpManager.ExportDirectory);


WebData.CreateDataManager("test = true").HeaderWebDataPageManager.LoadPages(startPage: 1, maxPage: 0, reload: false, loadImage: false, refreshDocumentStore: false).zView();
WebData.CreateDataManager("test = true").HeaderDetailManager.LoadHeaderDetails(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false).zView();
WebData.CreateDataManager("test = true").HeaderDetailManager.LoadHeaderDetails(startPage: 1, maxPage: 0, reloadHeaderPage: true, reloadDetail: true, loadImage: true, refreshDocumentStore: true).zView();
WebData.CreateDataManager("test = true").HeaderDetailManager.LoadHeaderDetails(startPage: 1, maxPage: 0, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false).zView();
WebData.CreateDataManager("test = true").HeaderDetailManager.LoadDetails(startPage: 1, maxPage: 0, reloadHeaderPage: true, reloadDetail: false, loadImage: true, refreshDocumentStore: false).zView();
WebData.CreateDataManager();

WebData.CreateDataManager("test = true").HeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 0, startPage: 1, maxPage: 1, loadImageFromWeb: true).zTraceJson();
WebData.CreateDataManager("test = true").HeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 0, startPage: 1, maxPage: 0, loadImageFromWeb: true).zTraceJson();

WebData.CreateDataManager().HeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 0, startPage: 1, maxPage: 0, loadImageFromWeb: true).zTraceJson();


TraceMongoCommand.Export("BlogDemoor", "BlogDemoor_Detail", @"c:\pib\dev_data\exe\runsource\blogdemoor\sites\BlogDemoor\mongo\export_BlogDemoor_Detail.txt", sort: "{ '_id': -1 }");
TraceMongoCommand.Export("BlogDemoor", "Images", @"c:\pib\dev_data\exe\runsource\blogdemoor\sites\BlogDemoor\mongo\export_Images.txt", sort: "{ '_id': -1 }");

TraceMongoCommand.Find("BlogDemoor_test", "BlogDemoor_Header_Test", "{}", limit: 100, sort: "{ _id: -1 }").zView();
TraceMongoCommand.Find("BlogDemoor_test", "BlogDemoor_Detail_Test", "{}", limit: 100, sort: "{ _id: -1 }").zView();
TraceMongoCommand.Find("BlogDemoor_test", "Images", "{}", limit: 100, sort: "{ _id: -1 }").zView();
TraceMongoCommand.Eval("db.getCollectionNames()", "BlogDemoor_test");
//TraceMongoCommand.Eval("db.Images.drop()", "BlogDemoor_test");
//TraceMongoCommand.Eval("db.Images_Test.drop()", "BlogDemoor_test");
//TraceMongoCommand.Eval("db.BlogDemoor_Detail_Test.drop()", "BlogDemoor_test");
TraceMongoCommand.Export("BlogDemoor_test", "BlogDemoor_Detail_Test", @"c:\pib\dev_data\exe\runsource\blogdemoor\sites\BlogDemoor.test\mongo\export_BlogDemoor_Detail_Test.txt", sort: "{ '_id': -1 }");
TraceMongoCommand.Export("BlogDemoor_test", "Images_Test", @"c:\pib\dev_data\exe\runsource\blogdemoor\sites\BlogDemoor.test\mongo\export_Images_Test.txt", sort: "{ '_id': -1 }");


Uri uri = new Uri("http://dccjta6europe.canalblog.com/archives/2016/07/20/34103996.html");
string lastSegment = uri.Segments[uri.Segments.Length - 1];
lastSegment.zTrace();

HttpManager.CurrentHttpManager.ExportResult = false;
HttpManager.CurrentHttpManager.ExportResult = true;

HttpRun.Load("http://dccjta6europe.canalblog.com/", new HttpRequestParameters { Encoding = Encoding.UTF8 });
HtmlRun.Select("//div[@class='item_div']");
HtmlRun.Select("//div[@class='item_div']", "./@itemscope", "./@toto");
HtmlRun.Select("//div[@itemscope='']");
HttpRun.Load(@"c:\pib\drive\google\dev_data\exe\runsource\blogdemoor\sites\canalblog.com\model\header\dccjta6europe.canalblog.com_01_01.html");

HttpRun.Load("http://dccjta6europe.canalblog.com/archives/2016/07/23/34111038.html", new HttpRequestParameters { Encoding = Encoding.UTF8 });
HttpRun.Load("http://dccjta6europe.canalblog.com/archives/2016/07/22/34110577.html", new HttpRequestParameters { Encoding = Encoding.UTF8 });


HttpRun.Load("http://dccjta6europe.canalblog.com/archives/2016/07/22/34110577.html", new HttpRequestParameters { Encoding = Encoding.UTF8 });
HttpRun.Load(@"c:\pib\dev_data\exe\runsource\blogdemoor\sites\BlogDemoor.test\cache\detail\archives_2016_07_21_34107089.html", new HttpRequestParameters { Encoding = Encoding.UTF8 });
HttpRun.Load(@"c:\pib\dev_data\exe\runsource\blogdemoor\sites\BlogDemoor.test\cache\detail\archives_2016_07_19_34100276.html", new HttpRequestParameters { Encoding = Encoding.UTF8 });
HttpRun.Load("http://dccjta6europe.canalblog.com/archives/2016/07/19/34100276.html", new HttpRequestParameters { Encoding = Encoding.UTF8 });
HttpRun.LoadToFile("http://p2.storage.canalblog.com/21/14/1436926/111029603_o.jpg", @"c:\pib\_dl\111029603_o.jpg", new HttpRequestParameters { Encoding = Encoding.UTF8 });

UrlCache urlCache = new UrlCache("toto");
urlCache.GetUrlSubPath(new HttpRequest { Url = "http://p0.storage.canalblog.com/00/66/1436926/111758118_o.jpg" }).zTrace();
zurl.UrlToFileName(new HttpRequest { Url = "http://p0.storage.canalblog.com/00/66/1436926/111758118_o.jpg" }, UrlFileNameType.Path).zTrace();
zurl.UrlToFileName("http://p0.storage.canalblog.com/00/66/1436926/111758118_o.jpg", UrlFileNameType.Path).zTrace();
zurl.UrlToFileName("http://p0.storage.canalblog.com/00/66/1436926/111758118_o.jpg", UrlFileNameType.Path | UrlFileNameType.Ext).zTrace();

CultureInfo cultureInfo = CultureInfo.GetCultureInfo("fr-FR");
cultureInfo.Name.zTrace();
CultureInfo.CurrentCulture.Name.zTrace();
DateTime dt;
if (DateTime.TryParseExact("23 juillet 2016", "d MMMM yyyy", CultureInfo.GetCultureInfo("fr-FR"), DateTimeStyles.None, out dt))
  dt.zTrace();
else
  "date not found".zTrace();
Date dt;
if (Date.TryParseExact("23 juillet 2016", "d MMMM yyyy", CultureInfo.GetCultureInfo("fr-FR"), DateTimeStyles.None, out dt))
  dt.zTrace();
else
  "date not found".zTrace();


RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\Web\Http\Test\Test_HttpLog.project.xml");
RunSource.CurrentRunSource.ProjectDirectory.zTrace();
Trace.WriteLine("toto");
Test_HttpLog.Test_WebRequest("https://httpbin.org/get", "GET", null, @"http\httpbin.org_get_01.html");
Test_HttpLog.Test_WebRequest("https://httpbin.org/get?toto=tata&zozo=zaza", "GET", null, @"http\httpbin.org_get_02.html");
Test_HttpLog.Test_WebRequest("https://httpbin.org/post", "POST", "toto=tata&zozo=zaza", @"http\httpbin.org_post_01.html");
Test_HttpLog.Test_HttpRequest("https://httpbin.org/get", HttpRequestMethod.Get, null, @"http\httpbin.org_get_03.html");
Test_HttpLog.Test_HttpRequest("https://httpbin.org/get?toto=tata&zozo=zaza", HttpRequestMethod.Get, null, @"http\httpbin.org_get_04.html");
Test_HttpLog.Test_HttpRequest("https://httpbin.org/post", HttpRequestMethod.Post, "toto=tata&zozo=zaza", @"http\httpbin.org_post_02.html");
Test_HttpLog.Test_01();
Test_HttpLog.Test_WebHeader_01();
Test_HttpLog.Test_WebHeader_02();
Test_HttpLog.Test_WebHeader_03();
