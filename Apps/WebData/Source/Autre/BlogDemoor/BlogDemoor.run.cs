// $$info.

Trace.WriteLine("toto");


//**********************************************************************************************************************************************************************************************
//****                                                   new
//**********************************************************************************************************************************************************************************************

WebData.CreateDataManager_v4().HeaderDetailManager.LoadNewDocuments(maxDocumentsLoadedFromStore: 5, startPage: 1, maxPage: 0, webImageRequest: new WebImageRequest { LoadImageFromWeb = true }).zTraceJson();
WebData.CreateDataManager_v4().HeaderDetailManager.LoadDetails(startPage: 1, maxPage: 1, reloadHeaderPage: true, reloadDetail: false, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false).zView();
// maxPage: 0, loadImageFromWeb: true
WebData.CreateDataManager_v4().HeaderDetailManager.LoadDetails(startPage: 1, maxPage: 0, reloadHeaderPage: false, reloadDetail: false, loadImageFromWeb: true, loadImageToData: false, refreshImage: false, refreshDocumentStore: false).zView();
WebData.CreateDataManager_v4().DetailDataManager.Find().zView();
WebData.CreateDataManager_v4().HeaderDataPageManager.LoadPages(startPage: 1, maxPage: 0, reload: false, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false).zView();
WebData.CreateDataManager_v4().HeaderDataPageManager.LoadPages(startPage: 1, maxPage: 0, reload: false, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false).Reverse().zView();
WebData.CreateDataManager_v4().HeaderDataPageManager.LoadPages(startPage: 1, maxPage: 0, reload: true, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false).zView();
WebData.CreateDataManager_v4().HeaderDetailManager.LoadDetails(startPage: 1, maxPage: 0, reloadHeaderPage: false, reloadDetail: false, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false).zView();
WebData.CreateDataManager_v4().HeaderDataPageManager.LoadPages(startPage: 1, maxPage: 1, reload: false, loadImageFromWeb: true, loadImageToData: false, refreshImage: false, refreshDocumentStore: true).zView();
// loadImageFromWeb: true, refreshDocumentStore: true
WebData.CreateDataManager_v4().HeaderDetailManager.LoadDetails(startPage: 1, maxPage: 1, reloadHeaderPage: false, reloadDetail: false, loadImageFromWeb: true, loadImageToData: false, refreshImage: false, refreshDocumentStore: true).zView();


TraceMongoCommand.Export("BlogDemoor", "BlogDemoor_Header", @"c:\pib\drive\google\map\voyage Demoor\sites\BlogDemoor\mongo\export_BlogDemoor_Header.txt", sort: "{ '_id': -1 }");
TraceMongoCommand.Export("BlogDemoor", "BlogDemoor_Detail", @"c:\pib\drive\google\map\voyage Demoor\sites\BlogDemoor\mongo\export_BlogDemoor_Detail.txt", sort: "{ '_id': -1 }");
TraceMongoCommand.Eval("db.getCollectionNames()", "BlogDemoor");
//TraceMongoCommand.Eval("db.BlogDemoor_Header.drop()", "BlogDemoor");
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
// reloadHeaderPage: false
WebData.CreateDataManager_v4("test = true").HeaderDetailManager.LoadHeaderDetails(startPage: 1, maxPage: 1, reloadHeaderPage: false, reloadDetail: false, refreshDocumentStore: false).zView();
WebData.CreateDataManager_v4("test = true").DetailDataManager.Find().zView();

TraceMongoCommand.Export("BlogDemoor_test", "BlogDemoor_Detail_Test", @"c:\pib\drive\google\map\voyage Demoor\sites\BlogDemoor.test\mongo\export_BlogDemoor_Detail_Test.txt", sort: "{ '_id': -1 }");
TraceMongoCommand.Eval("db.getCollectionNames()", "BlogDemoor_test");
//TraceMongoCommand.Eval("db.BlogDemoor_Detail_Test.drop()", "BlogDemoor_test");

//**********************************************************************************************************************************************************************************************
//****                                                   Test HtmlToDocx
//**********************************************************************************************************************************************************************************************

// create all docx
Test_HtmlToOXmlDoc_v2.CreateAllDocx(@"c:\pib\_dl\test\BlogDemoor\test\doc", "Voyage Demoor 2016",
  options: OXmlDocOptions.ExportHtml | OXmlDocOptions.ExportOXmlPage | OXmlDocOptions.ExportOXmlDoc, patchesFile: @"c:\pib\drive\google\dev\project\.net\Apps\WebData\Source\Autre\BlogDemoor\Data\PagePatches.json");

// create test\voyage_01.docx
Test_HtmlToOXmlDoc_v2.CreateDocx(@"c:\pib\_dl\test\BlogDemoor\test\test\voyage_01.docx", "Voyage Demoor 2016", limit: 40, skip: 0,
  options: OXmlDocOptions.ExportHtml | OXmlDocOptions.ExportOXmlPage | OXmlDocOptions.ExportOXmlDoc, patchesFile: @"c:\pib\drive\google\dev\project\.net\Apps\WebData\Source\Autre\BlogDemoor\Data\PagePatches.json");

// page 009 - 20 avril 2016     -  Lisbonne centre et bord de mer - http://dccjta6europe.canalblog.com/archives/2016/04/20/33690064.html
Test_HtmlToOXmlDoc_v2.CreateDocx(@"c:\pib\_dl\test\BlogDemoor\test\test\page_009_Lisbonne.docx", "Voyage Demoor 2016", new int[] { 33690064 }, options: OXmlDocOptions.ExportHtml | OXmlDocOptions.ExportOXmlPage | OXmlDocOptions.ExportOXmlDoc);
Test_HtmlToOXmlDoc_v2.OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\test\page_009_Lisbonne_02.oxml.json");
// page 008 - 19 avril 2016     - une école - http://dccjta6europe.canalblog.com/archives/2016/04/19/33690051.html
Test_HtmlToOXmlDoc_v2.CreateDocx(@"c:\pib\_dl\test\BlogDemoor\test\test\page_008_une_école.docx", "Voyage Demoor 2016", new int[] { 33690051 }, options: OXmlDocOptions.ExportHtml | OXmlDocOptions.ExportOXmlPage | OXmlDocOptions.ExportOXmlDoc);
Test_HtmlToOXmlDoc_v2.OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\test\page_008_une_école_02.oxml.json");
// 01 mars 2016  - tests de camping-cars -http://dccjta6europe.canalblog.com/archives/2016/03/01/33448954.html
Test_HtmlToOXmlDoc_v2.CreateDocx(@"c:\pib\_dl\test\BlogDemoor\test\test\page_001_01-03_tests_de_camping-cars.docx", "Voyage Demoor 2016", new int[] { 33448954 }, options: OXmlDocOptions.ExportHtml | OXmlDocOptions.ExportOXmlPage | OXmlDocOptions.ExportOXmlDoc);
Test_HtmlToOXmlDoc_v2.OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\test\page_001_01-03_tests_de_camping-cars_02.oxml.json");
// title "Kurtosskalacs // Trdelnik" url "http://dccjta6europe.canalblog.com/archives/2016/08/23/34222332.html"
Test_HtmlToOXmlDoc_v2.CreateDocx(@"c:\pib\_dl\test\BlogDemoor\test\test\page_153_23-08_Kurtosskalacs_Trdelnik.docx", "Voyage Demoor 2016", new int[] { 34222332 },
  options: OXmlDocOptions.ExportHtml | OXmlDocOptions.ExportOXmlPage | OXmlDocOptions.ExportOXmlDoc, patchesFile: @"c:\pib\drive\google\dev\project\.net\Apps\WebData\Source\Autre\BlogDemoor\Data\PagePatches.json");
// page 168 date 15-09 title "plovdiv : 2ème ville" url "http://dccjta6europe.canalblog.com/archives/2016/09/15/34324922.html"
Test_HtmlToOXmlDoc_v2.CreateDocx(@"c:\pib\_dl\test\BlogDemoor\test\test\page_168_15-09_plovdiv_2ème_ville.docx", "Voyage Demoor 2016", new int[] { 34324922 },
  options: OXmlDocOptions.ExportHtml | OXmlDocOptions.ExportOXmlPage | OXmlDocOptions.ExportOXmlDoc, patchesFile: @"c:\pib\drive\google\dev\project\.net\Apps\WebData\Source\Autre\BlogDemoor\Data\PagePatches.json");


Test_HtmlToOXmlDoc_v2.TracePages(limit: 0, skip: 0);

Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_01.docx", "Voyage Demoor 2016", limit: 10, skip: 0);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_02.docx", "Voyage Demoor 2016", limit: 10, skip: 10);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_03.docx", "Voyage Demoor 2016", limit: 10, skip: 20);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_04.docx", "Voyage Demoor 2016", limit: 10, skip: 30);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_05.docx", "Voyage Demoor 2016", limit: 10, skip: 40);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_06.docx", "Voyage Demoor 2016", limit: 10, skip: 50);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_07.docx", "Voyage Demoor 2016", limit: 10, skip: 60);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_08.docx", "Voyage Demoor 2016", limit: 10, skip: 70);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_09.docx", "Voyage Demoor 2016", limit: 10, skip: 80);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_10.docx", "Voyage Demoor 2016", limit: 10, skip: 90);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_11.docx", "Voyage Demoor 2016", limit: 10, skip: 100);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_12.docx", "Voyage Demoor 2016", limit: 10, skip: 110);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_13.docx", "Voyage Demoor 2016", limit: 10, skip: 120);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_14.docx", "Voyage Demoor 2016", limit: 10, skip: 130);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_15.docx", "Voyage Demoor 2016", limit: 10, skip: 140);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_16.docx", "Voyage Demoor 2016", limit: 10, skip: 150);
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_17.docx", "Voyage Demoor 2016", limit: 10, skip: 160);
// "plovdiv : 2ème ville" url "http://dccjta6europe.canalblog.com/archives/2016/09/15/34324922.html"
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_01.docx", new int[] { 34324922 }, "Voyage Demoor 2016");
Test_HtmlToOXmlDoc.Test_BlogDemoorToOXml(@"c:\pib\_dl\test\BlogDemoor\test\page_plovdiv", new int[] { 34324922 }, "Voyage Demoor 2016");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_plovdiv_05.oxml.json");
// "Kartoffel Knödel" url "http://dccjta6europe.canalblog.com/archives/2016/08/27/34237889.html"
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_01.docx", new int[] { 34237889 }, "Voyage Demoor 2016");
// "Union Européenne " url "http://dccjta6europe.canalblog.com/archives/2016/08/25/34227419.html"
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_01.docx", new int[] { 34227419 }, "Voyage Demoor 2016");
// "avis de Chloé" url "http://dccjta6europe.canalblog.com/archives/2016/03/01/33451107.html"
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_01.docx", new int[] { 33451107 }, "Voyage Demoor 2016");
Test_HtmlToOXmlDoc.Test_BlogDemoorToOXml(@"c:\pib\_dl\test\BlogDemoor\test\page_33451107", new int[] { 33451107 }, "Voyage Demoor 2016");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_33451107_04.oxml.json");
// "Notre maison roulante !" url "http://dccjta6europe.canalblog.com/archives/2016/04/13/33658425.html"
Test_HtmlToOXmlDoc.Test_BlogDemoorToDocx(@"c:\pib\_dl\test\BlogDemoor\test\voyage_01.docx", new int[] { 33658425 }, "Voyage Demoor 2016");
Test_HtmlToOXmlDoc.Test_BlogDemoorToOXml(@"c:\pib\_dl\test\BlogDemoor\test\page_notre_maison", new int[] { 33658425 }, "Voyage Demoor 2016");
Test_HtmlToOXmlDoc.Test_OXmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_notre_maison.oxml.json");


title "tests de camping-cars" url "http://dccjta6europe.canalblog.com/archives/2016/03/01/33448954.html"
title "direction Londres" url "http://dccjta6europe.canalblog.com/archives/2016/03/01/33450019.html"
title "avis de Chloé" url "http://dccjta6europe.canalblog.com/archives/2016/03/01/33451107.html"
title "Notre maison roulante !" url "http://dccjta6europe.canalblog.com/archives/2016/04/13/33658425.html"

title "BALOO" url "http://dccjta6europe.canalblog.com/archives/2016/04/15/33668361.html"
title "en vol pour Lisbonne" url "http://dccjta6europe.canalblog.com/archives/2016/04/19/33690013.html"
title "très bonne adaptation au pays !" url "http://dccjta6europe.canalblog.com/archives/2016/04/19/33690043.html"
title "une école" url "http://dccjta6europe.canalblog.com/archives/2016/04/19/33690051.html"

title " Lisbonne centre et bord de mer" url "http://dccjta6europe.canalblog.com/archives/2016/04/20/33690064.html"
title "à la recherche d'une poussette - découverte d'un très bel endroit" url "http://dccjta6europe.canalblog.com/archives/2016/04/20/33690067.html"
title "Lisbonne - objectif médico-scientifique" url "http://dccjta6europe.canalblog.com/archives/2016/04/22/33701837.html"
title "Reportage à l'école Saint Dominique, France, NOTRE ECOLE" url "http://dccjta6europe.canalblog.com/archives/2016/05/01/33746918.html"

title "Remerciements" url "http://dccjta6europe.canalblog.com/archives/2016/05/01/33749342.html"
title "parcours" url "http://dccjta6europe.canalblog.com/archives/2016/05/04/33760695.html"
title "A bientôt les amis !!! :-)" url "http://dccjta6europe.canalblog.com/archives/2016/05/14/33810671.html"
title "A bientôt la famille !!! :-)" url "http://dccjta6europe.canalblog.com/archives/2016/05/15/33814708.html"

WebData.CreateDataManager_v4().HeaderDetailManager.LoadDetails(startPage: 1, maxPage: 0, reloadHeaderPage: false, reloadDetail: false, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false)
  .Reverse().Take(4).zAction(page => Trace.WriteLine(page.Title)).zView();
WebData.CreateDataManager_v4().HeaderDetailManager.LoadDetails(startPage: 1, maxPage: 0, reloadHeaderPage: false, reloadDetail: false, loadImageFromWeb: false, loadImageToData: false, refreshImage: false, refreshDocumentStore: false).zView();

Test_HtmlToOXmlDoc.Test_Trace_Html(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_Trace_HtmlDoc(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_Trace_HtmlDocText(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_Trace_HtmlToOXml_v1(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_Trace_HtmlToOXml_v2(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_Trace_HtmlToOXmlText(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_HtmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html");
Test_HtmlToOXmlDoc.Test_HtmlToDocx_v1(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html", @"c:\pib\_dl\test\BlogDemoor\test\header_01.oxml.json", "plovdiv : 2ème ville - 15 septembre 2016");
Test_HtmlToOXmlDoc.Test_HtmlToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html", "plovdiv : 2ème ville - 15 septembre 2016", "Voyage Demoor");
Test_HtmlToOXmlDoc.Test_HtmlPagesToDocx(@"c:\pib\_dl\test\BlogDemoor\test\page_01.html", "plovdiv : 2ème ville", "15 septembre 2016", "Voyage Demoor");
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


BlogDemoor_v4.GetPageKey(new HttpRequest { Url = "http://dccjta6europe.canalblog.com/archives/p10-10.html" });

Date.Today.ToString().zTrace();
Trace.WriteLine($"{Date.Today}");
Trace.WriteLine($"{Date.Today:dd MMMM}");
Trace.WriteLine($"{CultureInfo.CurrentCulture.Name}");
Trace.WriteLine($"{CultureInfo.CurrentCulture.NativeName}");
Trace.WriteLine($"{CultureInfo.CurrentCulture.LCID}");
CultureInfo.GetCultures(CultureTypes.AllCultures).Select(culture => new { LCID = culture.LCID, Name = culture.Name, NativeName = culture.NativeName }).zView();
CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).Select(culture => new { LCID = culture.LCID, Name = culture.Name, NativeName = culture.NativeName }).zView();
Trace.WriteLine($"{CultureInfo.GetCultureInfo("fr-FR").NativeName}");
Trace.WriteLine(string.Format(CultureInfo.GetCultureInfo("fr-FR"), "{0:dd MMMM}", Date.Today));

Test_HtmlToOXmlDoc.DetailDataToHtmlPage(new BlogDemoorDetailData { Title = "toto", Date = Date.Today, Content = "<div></div>" }).zTraceJson();

Trace.WriteLine(WebData.CreateDataManager_v4().DetailDataManager.WebLoadImageManager.WebImageCacheManager.UrlCache.CacheDirectory);

HttpRun.LoadToFile("http://p2.storage.canalblog.com/24/99/1436926/112509351.jpg", @"c:\pib\_dl\test\112509351.jpg", exportRequest: true);
HttpManager_v2 httpManager = new HttpManager_v2();
httpManager.UrlCache = new UrlCache(@"c:\pib\_dl\test\LoadImage\cache");
httpManager.UrlCache.SaveRequest = true;
httpManager.LoadImage(new HttpRequest { Url = "http://p2.storage.canalblog.com/24/99/1436926/112509351.jpg" });

pb.IO.zfile.WriteFile(@"c:\pib\_dl\test\BlogDemoor\test\content.txt", WebData.CreateDataManager_v4().DetailDataManager.Find(query: "{ _id: 34227419 }").First().Content);

Trace.WriteLine(zurl.GetUrl("http://dccjta6europe.canalblog.com/archives/2016/08/23/34222332.html", "http://voyages.ideoz.fr/wp-content/uploads/2013/09/Kürtöskalacs-Noel-Budapest-640x480.jpg"));


//**********************************************************************************************************************************************************************************************
//****                                                   Test_Text.project.xml
//**********************************************************************************************************************************************************************************************

RunSource.CurrentRunSource.SetProjectFromSource();
RunSource.CurrentRunSource.SetProject(@"$Root$\Lib\pb\Source\pb\Text\_Test\Test_Text.project.xml");

Trace.WriteLine("toto");

Test_Regex.Test("_{2,}", "aa_bb__cc___dd");

zmongo.BsonRead<PagePatch>(@"c:\pib\drive\google\dev\project\.net\Apps\WebData\Source\Autre\BlogDemoor\Data\PagePatches.json").zTraceJson();

"toto".zTrace();
$"{1:000}".zTrace();
