// $$info.

Trace.WriteLine("toto");


//**********************************************************************************************************************************************************************************************
//****                                                   new
//**********************************************************************************************************************************************************************************************

WebData.CreateDataManager_v4().HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 5, startPage: 1, maxPage: 0, webImageRequest: new WebImageRequest { LoadImageFromWeb = true }).zTraceJson();
WebData.CreateDataManager_v4().HeaderDetailManager.LoadDetails(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: false, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false).zView();


TraceMongoCommand.Export("BlogDemoor", "BlogDemoor_Detail", @"c:\pib\drive\google\map\voyage Demoor\sites\BlogDemoor\mongo\export_BlogDemoor_Detail.txt", sort: "{ '_id': -1 }");
TraceMongoCommand.Eval("db.getCollectionNames()", "BlogDemoor");
//TraceMongoCommand.Eval("db.BlogDemoor_Detail.drop()", "BlogDemoor");

//**********************************************************************************************************************************************************************************************
//****                                                   old
//**********************************************************************************************************************************************************************************************

//WebData.CreateDataManager().HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 5, startPage: 1, maxPage: 0, loadImageFromWeb: true).zTraceJson();
WebData.CreateDataManager().HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 5, startPage: 1, maxPage: 0, webImageRequest: new WebImageRequest { LoadImageFromWeb = true }).zTraceJson();
WebData.Backup();


//**********************************************************************************************************************************************************************************************
//****                                                   Test
//**********************************************************************************************************************************************************************************************

WebData.CreateDataManager_v4("test = true").HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 5, startPage: 1, maxPage: 1, webImageRequest: new WebImageRequest { LoadImageFromWeb = true }).zTraceJson();
// LoadNewDocuments ne peut pas charger les missing images (LoadMissingImageFromWeb)
// loadImage: true
WebData.CreateDataManager_v4("test = true").HeaderDetailManager.LoadHeaderDetails(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: false, refreshDocumentStore: false).zView();

TraceMongoCommand.Export("BlogDemoor_test", "BlogDemoor_Detail_Test", @"c:\pib\drive\google\map\voyage Demoor\sites\BlogDemoor.test\mongo\export_BlogDemoor_Detail_Test.txt", sort: "{ '_id': -1 }");
TraceMongoCommand.Eval("db.getCollectionNames()", "BlogDemoor_test");
//TraceMongoCommand.Eval("db.BlogDemoor_Detail_Test.drop()", "BlogDemoor_test");

//**********************************************************************************************************************************************************************************************
//****                                                   Test HtmlToDocx
//**********************************************************************************************************************************************************************************************

Test_HtmlToOXmlDoc.Test_Trace_Html(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_Trace_HtmlDoc(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_Trace_HtmlDocText(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_Trace_HtmlToOXml(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_Trace_HtmlToOXmlText(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_HtmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\oxml.example.json");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01_01.oxml.json");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01_02.oxml.json");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01_03.oxml.json");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01_04.oxml.json");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01_05.oxml.json");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01_06.oxml.json");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01_07.oxml.json");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\drive\google\dev\project\.net\Lib\pb\Source\pb\Data\OpenXml\Test\Data\page_01_06_99.oxml.json");

OXmlElementReader.Read(@"c:\pib\_dl\test\BlogDemoor\test\page_01_06.oxml.json").zSave(@"c:\pib\_dl\test\BlogDemoor\test\page_01_06_save.oxml.json");
zmongo.BsonRead<OXmlElement>(@"c:\pib\_dl\test\BlogDemoor\test\page_01_06_save.oxml.json").zSave(@"c:\pib\_dl\test\BlogDemoor\test\page_01_06_save_02.oxml.json");

//**********************************************************************************************************************************************************************************************
//****                                                   Test Test_OpenXml.project.xml
//**********************************************************************************************************************************************************************************************

RunSource.CurrentRunSource.SetProjectFromSource();
//RunSource.CurrentRunSource.SetProject(@"$Root$\Test\Test.Test_01\Source\Test_OpenXml\Test_OpenXml.project.xml");
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\Data\OpenXml\_Test\Test_OpenXml.project.xml");

Trace.WriteLine("toto");

Test_OpenXml_Image.Test_01();
Test_OpenXml_Image.Test_02();
Test_OpenXml_Image.Test_03();
Test_OpenXml_Image.Test_04();
Test_OpenXml_Image.Test_05();
Test_OpenXml_Image.Test_06();

Test_OpenXml_Style.Test_DocDefaults_01();
Test_OpenXml_Style.Test_Style_01();

Test_OpenXml_Style.Test_Header_01(header: true, footer: true, pageNumber: true);
Test_OpenXml_Style.Test_Header_01(header: true, footer: true, pageNumber: false);
Test_OpenXml_Style.Test_Header_01(header: true, footer: false);
Test_OpenXml_Style.Test_Header_01(header: false, footer: false);

Test_OpenXml_Style.Test_Formule_01("=5+7");
Test_OpenXml_Style.Test_Formule_01("FILENAME");


OXmlElementReader.Test_01(@"c:\pib\_dl\test\BlogDemoor\test\oxml.example.json");

Test_OpenXml_Serialize.Test_01(@"c:\pib\_dl\test\BlogDemoor\test\test.oxml\test_oxml_01.json");
Test_OpenXml_Serialize.Test_01(@"c:\pib\_dl\test\BlogDemoor\test\test.oxml\test_oxml_01_out.json");
Test_OpenXml_Serialize.Test_01(@"c:\pib\drive\google\dev\project\.net\Lib\pb\Source\pb\Data\OpenXml\_Test\Data\test_oxml_01.json");
Test_OpenXml_Serialize.Test_01(@"c:\pib\drive\google\dev\project\.net\Lib\pb\Source\pb\Data\OpenXml\_Test\Data\test_oxml_01_out.json");
Test_OpenXml_Serialize.Test_01(@"c:\pib\drive\google\dev\project\.net\Lib\pb\Source\pb\Data\OpenXml\_Test\Data\test_oxml_01_03.json", jsonIndent: true);



//**********************************************************************************************************************************************************************************************
//****                                                   Test old
//**********************************************************************************************************************************************************************************************

WebData.CreateDataManager("test = true").HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 5, startPage: 1, maxPage: 1, webImageRequest: new WebImageRequest { LoadImageFromWeb = true }).zTraceJson();
// LoadNewDocuments ne peut pas charger les missing images (LoadMissingImageFromWeb)
//WebData.CreateDataManager("test = true").HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 5, startPage: 1, maxPage: 1, webImageRequest: new WebImageRequest { LoadImageFromWeb = true, LoadMissingImageFromWeb = true }).zTraceJson();



//*************************************************************************************************************************
//****
//*************************************************************************************************************************

WebData.CreateDataManager().HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 0, startPage: 1, maxPage: 0, webImageRequest: new WebImageRequest { LoadImageFromWeb = true, LoadMissingImageFromWeb = true }).zTraceJson();


WebData.CreateDataManager().HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 0, startPage: 1, maxPage: 0, loadImageFromWeb: true).zTraceJson();
WebData.CreateDataManager().HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 5, startPage: 1, maxPage: 1, loadImageFromWeb: true).zTraceJson();


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

WebData.CreateDataManager().HeaderDetailManager.LoadNewDocuments(maxNbDocumentsLoadedFromStore: 0, startPage: 1, maxPage: 0, loadImageFromWeb: true).zTraceJson();


TraceMongoCommand.Export("BlogDemoor", "BlogDemoor_Detail", @"c:\pib\drive\google\map\voyage Demoor\sites\BlogDemoor\mongo\export_BlogDemoor_Detail.txt", sort: "{ '_id': -1 }");
TraceMongoCommand.Export("BlogDemoor", "Images", @"c:\pib\drive\google\map\voyage Demoor\sites\BlogDemoor\mongo\export_Images.txt", sort: "{ '_id': -1 }");
TraceMongoCommand.Eval("db.getCollectionNames()", "BlogDemoor");
//TraceMongoCommand.Eval("db.BlogDemoor_Detail.drop()", "BlogDemoor");
//TraceMongoCommand.Eval("db.Images.drop()", "BlogDemoor");



//**********************************************************************************************************************************************************************************************
//****                                                   Test
//**********************************************************************************************************************************************************************************************


WebData.CreateDataManager("test = true").HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 0, maxDocumentsLoaded: 2, startPage: 1, maxPage: 1, loadImageFromWeb: true).zTraceJson();
WebData.CreateDataManager("test = true").HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 0, startPage: 1, maxPage: 0, loadImageFromWeb: true).zTraceJson();

//TraceMongoCommand.Export("BlogDemoor_test", "Images_Test", @"c:\pib\dev_data\exe\runsource\blogdemoor\sites\BlogDemoor.test\mongo\export_Images_Test.txt", sort: "{ '_id': -1 }");

TraceMongoCommand.Find("BlogDemoor_test", "BlogDemoor_Header_Test", "{}", limit: 100, sort: "{ _id: -1 }").zView();
TraceMongoCommand.Find("BlogDemoor_test", "BlogDemoor_Detail_Test", "{}", limit: 100, sort: "{ _id: -1 }").zView();
TraceMongoCommand.Find("BlogDemoor_test", "Images_Test", "{}", limit: 100, sort: "{ _id: -1 }").zView();
TraceMongoCommand.Eval("db.getCollectionNames()", "BlogDemoor_test");
//TraceMongoCommand.Eval("db.BlogDemoor_Detail_Test.drop()", "BlogDemoor_test");
//TraceMongoCommand.Eval("db.Images.drop()", "BlogDemoor_test");
//TraceMongoCommand.Eval("db.Images_Test.drop()", "BlogDemoor_test");


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
new Uri("http://p1.storage.canalblog.com/11/54/1436926/112126657.jpg").Host.zTrace();
Uri uri = new Uri("http://p1.storage.canalblog.com/11/54/1436926/112126657.jpg");
uri.Segments[uri.Segments.Length - 1].zTrace();


RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\Data\Mongo\Test\Test_Mongo.project.xml");
Test_Mongo.Test_01();
Trace.WriteLine("toto");
new Test_01 { Text = "toto", Number = 123 }.zTraceJson();
new Test_01 { Text = "toto", Number = 123 }.zSave(@"c:\pib\_dl\test\test_01.json");
zmongo.ReadFileAs<Test_01>(@"c:\pib\_dl\test\test_01.json").zTraceJson();


//**********************************************************************************************************************************************************************************************
//****                                                   Test Test_SharpCompressManager.project.xml
//**********************************************************************************************************************************************************************************************

RunSource.CurrentRunSource.SetProjectFromSource();
//RunSource.CurrentRunSource.SetProject(@"$Root$\Test\Test.Test_01\Source\Test_OpenXml\Test_OpenXml.project.xml");
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\IO\_Test\Test_SharpCompressManager.project.xml");

Trace.WriteLine("toto");

// uncompress all .docx files in directory .docx.zip
string docxFile = @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx";
string dir = @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx.zip";
new CompressManager(new ZipManager()).Uncompress(docxFile, dir, null, UncompressOptions.ExtractFullPath).zTraceJson();
string docxFile = @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx";
new CompressManager(new ZipManager()).Uncompress(docxFile, null, new string[] { @"word/document.xml" });

var xd = System.Xml.Linq.XDocument.Load(@"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01\word\document.xml");
xd.Save(@"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01\word\document.xml");

//string dir = @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx.zip$$\";
string dir = @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx.zip\";
string[] files = new string[] {
    dir + @"word\document.xml"
    //dir + @"word\styles.xml"
    //dir + @"word\fontTable.xml"
};
ZipArchive.Zip(@"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01_test.docx", files, FileMode.OpenOrCreate, ZipArchiveOptions.StorePartialPath, dir);
//Test_SharpCompress.Test_Compress_01(@"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01_test.docx", files, dir);

Test_ZipReader.Test_ZipReader_01(@"c:\pib\_dl\test\BlogDemoor\test\word\test_zip_08_pb.IO.ZipArchive.docx");


zDirectory.EnumerateFileSystemEntries(@"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx.zip").zTrace();


RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\Data\OpenXml\Test\Test_OpenXml.project.xml");
// ok word and error google doc
Test_OpenXml_Zip.Test_OpenXml_Zip_01(@"c:\pib\_dl\test\BlogDemoor\test\word\test_zip_03_System.IO.Compression.ZipArchive.docx", @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx.zip", useSlash: false, addDirectoryEntry: false);
// ok word and error google doc
Test_OpenXml_Zip.Test_OpenXml_Zip_01(@"c:\pib\_dl\test\BlogDemoor\test\word\test_zip_04_System.IO.Compression.ZipArchive.docx", @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx.zip", useSlash: false, addDirectoryEntry: true);
// ok word and ok google doc
Test_OpenXml_Zip.Test_OpenXml_Zip_01(@"c:\pib\_dl\test\BlogDemoor\test\word\test_zip_05_System.IO.Compression.ZipArchive.docx", @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx.zip", useSlash: true, addDirectoryEntry: false);
// ok word and ok google doc
Test_OpenXml_Zip.Test_OpenXml_Zip_01(@"c:\pib\_dl\test\BlogDemoor\test\word\test_zip_06_System.IO.Compression.ZipArchive.docx", @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx.zip", useSlash: true, addDirectoryEntry: true);

// ok word and error google doc
Test_OpenXml_Zip.Test_OpenXml_Zip_02(@"c:\pib\_dl\test\BlogDemoor\test\word\test_zip_07_pb.IO.ZipArchive.docx", @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx.zip", useSlash: false);
// ok word and ok google doc
Test_OpenXml_Zip.Test_OpenXml_Zip_02(@"c:\pib\_dl\test\BlogDemoor\test\word\test_zip_08_pb.IO.ZipArchive.docx", @"c:\pib\_dl\test\BlogDemoor\test\word\test_01_01.docx.zip", useSlash: true);

//**********************************************************************************************************************************************************************************************
//****                                                   Test Test_Json.project.xml
//**********************************************************************************************************************************************************************************************

RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\Data\Json\_Test\Test_Json.project.xml");

Test_Json.Test_Json_01();


//**********************************************************************************************************************************************************************************************
//****                                                   Test Test_MongoSerializers.project.xml
//**********************************************************************************************************************************************************************************************

RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\Data\Mongo\Serializers\_Test\Test_MongoSerializers.project.xml");

Trace.WriteLine("toto");

Test_MongoSerializers.Test_MongoSerializers_01();
Test_MongoSerializers.Test_Serialize_01();
Test_MongoSerializers.Test_Serialize_02();
Test_MongoSerializers.Test_Serialize_03();
Test_MongoSerializers.Test_Deserialize_01(trace: false);
Test_MongoSerializers.Test_Deserialize_01(trace: true);

ZValue zv = new ZString("toto");
zv.zTraceJson();
ZValue zv = null;
zv.zTraceJson();
new ZString("toto").zTraceJson();
Trace.WriteLine(new ZString("toto").ToJson());
"toto".zTraceJson();
DateTime.Now.zTraceJson();
Date.Today.zTraceJson();



//**********************************************************************************************************************************************************************************************
//****                                                   Test Test_Data.project.xml
//**********************************************************************************************************************************************************************************************

RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\Data\_Test\Test_Data.project.xml");

Test_Data.Test_Data_01();
Test_NamedValues.Test_ParseValues_01("bool = true, int = 123, double = 1.123, string1 = \"toto tata\", string2 = 'toto tata'", useLowercaseKey: true);
Test_NamedValues.Test_ParseValues_01("datetime1 = 01/01/2015 01:35:52.123, datetime2 = 2015-01-01 01:35:52.123, date1 = 01/01/2015, date2 = 2015-01-01, time = 1.01:35:52.1234567", useLowercaseKey: true);


//**********************************************************************************************************************************************************************************************
//****                                                   Test Test_Reflection.project.xml
//**********************************************************************************************************************************************************************************************

RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\Reflection\_Test\Test_Reflection.project.xml");


Trace.WriteLine("toto");
// pb.Data.Mongo.Serializers.RunSerializer.Init
zReflection.GetMethodElements("pb.Reflection.Test.Test_MethodInfo.Test_01").zTraceJson();
zReflection.GetType("pb.Reflection.Test.Test_MethodInfo")?.AssemblyQualifiedName.zTrace();

Test_MethodInfo.Test_GetMethod_01("pb.Reflection.Test.Test_MethodInfo.Test_01");
Test_MethodInfo.Test_GetMethod_01("pb.Reflection.Test.Test_MethodInfo.Test_GetMethod_01");


//**********************************************************************************************************************************************************************************************
//****                                                   Test TestBasic.project.xml
//**********************************************************************************************************************************************************************************************

RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\TestBasic.project.xml");

ProjectCompiler.TraceLevel = 1;
ProjectCompiler.TraceLevel = 2;
Trace.WriteLine("toto");
