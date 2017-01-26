Trace.WriteLine("toto");


Test.Test_Unit.Web.Test_Unit_HtmlToXml.Test();

Test.Test_Unit.Web.Test_Unit_HtmlToXml.ArchiveOkFiles();
Test.Test_Unit.Web.Test_Unit_HtmlToXml.SetFilesAsOk();
Test.Test_Unit.Web.Test_Unit_HtmlToXml.FileHtmlToXml(@"c:\pib\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_01_01.html", traceHtmlReader: true);
Test.Test_Unit.Web.Test_Unit_HtmlToXml.FileHtmlToXml(@"c:\pib\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_02_01.html", traceHtmlReader: true);


TestUnit_HtmlToXml.FileHtmlToXml(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_01_01.html", traceHtmlReader: true, htmlReaderVersion: 2);
TestUnit_HtmlToXml.FileHtmlToXml(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\ebookdz.com\detail\ebookdz.com_forum_showthread.php_t_109595_logged_01_01.html", traceHtmlReader: true, htmlReaderVersion: 2);
TestUnit_HtmlToXml.FileHtmlToXml(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\ebookdz.com\detail\ebookdz.com_forum_showthread.php_t_109595_not_logged_01_01.html", traceHtmlReader: true, htmlReaderVersion: 2);
TestUnit_HtmlToXml.FileHtmlToXml(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\zone-ebooks.com\zone-ebooks.com_img_info_livre_02_01.html", traceHtmlReader: true, htmlReaderVersion: 2, encoding: Encoding.Default);
TestUnit_HtmlToXml.FileHtmlToXml(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\zone-ebooks.com\zone-ebooks.com_img_info_livre_02_01_utf8.html", traceHtmlReader: true, htmlReaderVersion: 2);

TestUnit_HtmlToXml.TestUnit();
TestUnit_HtmlToXml.SetFilesAsOk(overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles();

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\cdefi.fr", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\cdefi.fr");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\cdefi.fr", htmlReaderVersion: 2);

TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\ebookdz.com", htmlReaderVersion: 2);

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\extreme-down.net", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\extreme-down.net");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\extreme-down.net", htmlReaderVersion: 2);

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\free-telechargement.org", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\free-telechargement.org");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\free-telechargement.org", htmlReaderVersion: 2);

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\golden-ddl.net", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\golden-ddl.net");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\golden-ddl.net", htmlReaderVersion: 2);

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\handeco.org", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\handeco.org");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\handeco.org", htmlReaderVersion: 2);

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\magazines-gratuits.info", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\magazines-gratuits.info");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\magazines-gratuits.info", htmlReaderVersion: 2);

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\onisep.fr", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\onisep.fr");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\onisep.fr", htmlReaderVersion: 2);

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\rapide-ddl", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\rapide-ddl");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\rapide-ddl", htmlReaderVersion: 2);

TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\telechargementz.tv");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\telechargementz.tv", htmlReaderVersion: 2);

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\telecharger-magazine.com", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\telecharger-magazine.com");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\telecharger-magazine.com", htmlReaderVersion: 2);

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\vosbooks.net", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\vosbooks.net");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\vosbooks.net", htmlReaderVersion: 2);

TestUnit_HtmlToXml.SetFilesAsOk(@"Web\HtmlToXml\sites\zone-ebooks.com", overwrite: true);
TestUnit_HtmlToXml.ArchiveOkFiles(@"Web\HtmlToXml\sites\zone-ebooks.com");
TestUnit_HtmlToXml.Test(@"Web\HtmlToXml\sites\zone-ebooks.com", htmlReaderVersion: 2);



TestUnit_HtmlToXml.GetHtmlFiles(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\ebookdz.com\detail").zView();
TestUnit_HtmlToXml.GetHtmlFiles(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\ebookdz.com").zView();
TestUnit_HtmlToXml.GetOkFile(@"c:\pib\toto.xml").zTrace();

zdir.GetNewIndexedDirectory(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\magazines-gratuits.info\detail\archive", indexLength: 2).zTrace();


HtmlReader_v3.ReadFile(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_01_01.html").zSave(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_01_01.html.reader.txt");
TestUnit_HtmlToXml.FileHtmlToXml_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_01_01.html", traceHtmlReader: true);
HtmlReader_v3.ReadFile(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_03_99.html").zSave(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_03_99.html.reader.txt");
HtmlReader_v3.ReadFile(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_04_99.html").zSave(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_04_99.html.reader.txt");
TestUnit_HtmlToXml.FileHtmlToXml(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_01_01.html", traceHtmlReader: true, htmlReaderVersion: 2);


TestUnit_HtmlToXml.FileHtmlToXml(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\test_01.html", traceHtmlReader: true);
TestUnit_HtmlToXml.FileHtmlToXml_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\test_01.html", traceHtmlReader: true);

TestUnit_HtmlToXml.TraceHtmlReader_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\test_01.html");
TestUnit_HtmlToXml.TraceHtmlReader_v3(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\test_01.html");
TestUnit_HtmlToXml.TraceHtmlReader_v3_2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\test_01.html");
TestUnit_HtmlToXml.TraceHtmlReader_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\test_01.html");
TestUnit_HtmlToXml.TraceHtmlReader_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\test_03.html",
	generateCloseTag: false, disableLineColumn: true, useReadAttributeValue_v2: false);

TestUnit_HtmlToXml.TraceHtmlReader_v3(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_01_01.html");
TestUnit_HtmlToXml.TraceHtmlReader_v3_2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_01_01.html");
TestUnit_HtmlToXml.TraceHtmlReader_v3(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_02_01.html");
TestUnit_HtmlToXml.TraceHtmlReader_v3_2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\test\ebookdz.com_logged_02_01.html");

TestUnit_HtmlToXml.Test_HtmlReader_v2_v3();


TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\cdefi.fr");
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\telecharger-magazine.com.test\cache\detail");

TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\ebookdz.com\detail");
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4();

// vosbooks.net
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\vosbooks.net\cache\header", @"c:\pib\dev_data\exe\runsource\download\sites\___trace");
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\vosbooks.net\cache\detail\115000", @"c:\pib\dev_data\exe\runsource\download\sites\___trace");
// ebookdz.com
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\ebookdz.com\cache\header",
	traceDirectory: @"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.*");
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\ebookdz.com\cache\detail\149000",
	traceDirectory: @"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.*");
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\ebookdz.com\cache\forum",
	traceDirectory: @"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.*");
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\ebookdz.com\cache\forumHeader",
	traceDirectory: @"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.*");
// extreme-down.net
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\extreme-down.net\cache\header",
	traceDirectory: @"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.*");
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\extreme-down.net\cache\detail\30000",
	traceDirectory: @"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.*");
// magazines-gratuits.info
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\magazines-gratuits.info\cache\header",
	traceDirectory: @"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.*");
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\___trace\magazines-gratuits.info",
	traceDirectory: @"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.*");
// telecharger-magazine.com
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\telecharger-magazine.com\cache\header",
	traceDirectory: @"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.*");
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\telecharger-magazine.com\cache\detail\12000",
	traceDirectory: @"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.*");
TestUnit_HtmlToXml.Test_HtmlReader_v2_v4(@"c:\pib\dev_data\exe\runsource\download\sites\___trace", pattern: "*.html");


TestUnit_HtmlToXml.TraceHtmlReader_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\free-telechargement.org\header\free-telechargement.org_1_categorie-Magazines_01_01.html",
	disableScriptTreatment: true, useReadAttributeValue_v2: false);
TestUnit_HtmlToXml.TraceHtmlReader_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\free-telechargement.org\header\free-telechargement.org_1_categorie-Magazines_01_01.html",
	disableScriptTreatment: true, useReadAttributeValue_v2: true);
TestUnit_HtmlToXml.TraceHtmlReader_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\free-telechargement.org\header\free-telechargement.org_1_categorie-Magazines_01_01.html",
	generateCloseTag: false, disableLineColumn: true, useReadAttributeValue_v2: false);
TestUnit_HtmlToXml.TraceHtmlReader_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\free-telechargement.org\header\free-telechargement.org_1_categorie-Magazines_01_01.html",
	generateCloseTag: false, disableLineColumn: true, useReadAttributeValue_v2: true);


// 	bug_telecharger-magazine.com_science_4588-pour-la-science-n455-septembre-2015_01_01.html
//    diff between HtmlReader_v2 HtmlReader_v4 : HtmlReader_v2 read one \d less after bad </iframe (line 378)
TestUnit_HtmlToXml.TraceHtmlReader_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\telecharger-magazine.com\detail\bug_telecharger-magazine.com_science_4588-pour-la-science-n455-septembre-2015_01_01.html",
	disableScriptTreatment: false, useReadAttributeValue_v2: false);
TestUnit_HtmlToXml.TraceHtmlReader_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\telecharger-magazine.com\detail\bug_telecharger-magazine.com_science_4588-pour-la-science-n455-septembre-2015_01_01.html",
	generateCloseTag: false, disableLineColumn: false, useReadAttributeValue_v2: false);
TestUnit_HtmlToXml.TraceHtmlReader_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\telecharger-magazine.com\detail\bug_telecharger-magazine.com_science_4588-pour-la-science-n455-septembre-2015_01_01.html",
	generateCloseTag: false, disableLineColumn: true, useReadAttributeValue_v2: false);

// livres_12066-lislam-pour-les-nuls.html
TestUnit_HtmlToXml.TraceHtmlReader_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\telecharger-magazine.com\detail\livres_12066-lislam-pour-les-nuls.html",
	disableScriptTreatment: false, useReadAttributeValue_v2: false);
TestUnit_HtmlToXml.TraceHtmlReader_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\telecharger-magazine.com\detail\livres_12066-lislam-pour-les-nuls.html",
	generateCloseTag: false, disableLineColumn: true, disableScriptTreatment: false, useReadAttributeValue_v2: false, useTranslateChar: true);


// ebooks_bandes-dessinee_36158-x-men_01_01.html
TestUnit_HtmlToXml.TraceHtmlReader_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\rapide-ddl\detail\ebooks_bandes-dessinee_36158-x-men_01_01.html",
	disableScriptTreatment: true, useReadAttributeValue_v2: false);
TestUnit_HtmlToXml.TraceHtmlReader_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\rapide-ddl\detail\ebooks_bandes-dessinee_36158-x-men_01_01.html",
	disableScriptTreatment: false, useReadAttributeValue_v2: false);
TestUnit_HtmlToXml.TraceHtmlReader_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\rapide-ddl\detail\ebooks_bandes-dessinee_36158-x-men_01_01.html",
	generateCloseTag: false, disableLineColumn: false, disableScriptTreatment: true, useReadAttributeValue_v2: false);
TestUnit_HtmlToXml.TraceHtmlReader_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\rapide-ddl\detail\ebooks_bandes-dessinee_36158-x-men_01_01.html",
	generateCloseTag: false, disableLineColumn: true, disableScriptTreatment: true, useReadAttributeValue_v2: false);
TestUnit_HtmlToXml.TraceHtmlReader_v4(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\rapide-ddl\detail\ebooks_bandes-dessinee_36158-x-men_01_01.html",
	generateCloseTag: false, disableLineColumn: true, disableScriptTreatment: false, useReadAttributeValue_v2: false);




char car = 'a';
car.zTrace();
((int)car).zTrace();
((char)97).zTrace();
((char)-1).zTrace();
int code = 97;
((char)code).zTrace();
int code = -1;
((char)code).zTrace();


TestUnit_HtmlToXml.FileHtmlToXml_HtmlReader_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\vosbooks.net\detail\vosbooks.net_74299-livre_les-imposteurs-francois-cavanna_01_01.html", traceHtmlReader: true, traceHtmlToXml: true, useXDocumentCreator: true, correctionMarkBeginEnd: true);
TestUnit_HtmlToXml.FileHtmlToXml_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\vosbooks.net\detail\vosbooks.net_74299-livre_les-imposteurs-francois-cavanna_01_01.html", traceHtmlReader: true, traceHtmlToXml: true);

TestUnit_HtmlToXml.FileHtmlToXml_HtmlReader_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\_test\test_01.html", traceHtmlReader: true, traceHtmlToXml: true, useXDocumentCreator: true, correctionMarkBeginEnd: true);
TestUnit_HtmlToXml.FileHtmlToXml_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\_test\test_03.html", traceHtmlReader: true, traceHtmlToXml: true);

TestUnit_HtmlToXml.Test_HtmlToXml_v1_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\vosbooks.net", correctionMarkBeginEnd: true);
TestUnit_HtmlToXml.Test_HtmlToXml_v1_v2(@"c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites", correctionMarkBeginEnd: true);
