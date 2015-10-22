RunSource.CurrentRunSource.Compile_Project(@"..\..\runsource\runsource_dll\irunsource_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\runsource\runsource_dll\runsource_dll_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\runsource\runsource_domain\runsourced32_project.xml");


_tr.WriteLine("toto");

Compiler.TraceLevel = 1;
Compiler.TraceLevel = 2;

_rs.Trace.SetTraceDirectory(Path.Combine(_dataDir, "http"));
_rs.Trace.SetTraceDirectory(null);
HtmlXmlReader.CurrentHtmlXmlReader.http.Dispose();

// fonction d'extraction Handeco pour damien 01
Handeco.Handeco.ExportXmlCompanyList(startPage: 1, maxPage: 0, reload: false);
Handeco.Handeco.ExportXmlCompanyList(startPage: 1, maxPage: 0, reload: true);
// fonction d'extraction Gesat pour damien 01
Gesat.Gesat.ExportXmlCompanyList(startPage: 1, maxPage: 0, reload: false, loadImage: false);
Gesat.Gesat.ExportXmlCompanyList(startPage: 1, maxPage: 0, reload: true, loadImage: false);
// fonction d'extraction Unea pour damien 01
Unea.Unea.ExportXmlCompanyList(startPage: 1, maxPage: 0, reload: false, loadImage: false);
Unea.Unea.ExportXmlCompanyList(startPage: 1, maxPage: 0, reload: true, loadImage: false);

Unea.Unea.ExportXmlCompanyList(startPage: 1, maxPage: 1, reload: false, loadImage: false);
Unea.Unea.ExportXmlCompanyList(startPage: 1, maxPage: 1, reload: true, loadImage: false);

// test Handeco
Handeco.Handeco.ExportXmlCompanyList(startPage: 1, maxPage: 0, reload: false);
Handeco.Handeco.ExportXmlCompanyList(startPage: 1, maxPage: 0, reload: true);
Handeco.Handeco.ExportXmlCompanyList(startPage: 1, maxPage: 1, reload: false);
Test_Handeco_LoadHeaderPages_01(startPage: 1, maxPage: 5, reload: false);
Test_Handeco_LoadHeaderPages_01(startPage: 1, maxPage: 5, reload: true);
Test_Handeco_LoadHeaderPages_01(nbPage: 1);
Test_Handeco_LoadHeaderPages_01(nbPage: 2, loadLastPage: true);
Test_Handeco_LoadDetailCompany_01("http://www.handeco.org/fournisseur/consulter/id/1252", reload: false);
Test_Handeco_LoadDetailCompany_01("http://www.handeco.org/fournisseur/consulter/id/5311", reload: false);
Test_Handeco_LoadDetailCompany_01("http://www.handeco.org/fournisseur/consulter/id/1254", reload: false);
Test_Handeco_LoadDetailCompany_01("http://www.handeco.org/fournisseur/consulter/id/5279", reload: false); // tel sur 2 lignes
Test_Handeco_LoadDetailCompany_01("http://www.handeco.org/fournisseur/consulter/id/5280", reload: false); // tel sur 2 lignes
Test_Handeco_LoadDetailCompany_01("http://www.handeco.org/fournisseur/consulter/id/967", reload: false); // liste de valeurs dans Adhésion à des groupements et fédérations professionnels
Test_Handeco_LoadDetailCompany_01("http://www.handeco.org/fournisseur/consulter/id/1936", reload: false); // EA  DE BELLEFONTAINE : liste de valeurs dans Adhésion à des groupements et fédérations professionnels
Test_Handeco_LoadDetailCompany_01("http://www.handeco.org/fournisseur/consulter/id/214", reload: false); // ESAT DE LA FAYE : valeur inconnu : Logo =



Test_Handeco_Load_WebRequest_01();
Test_Handeco_Load_CompanyDetail_01();

// test
Test_Gesat_LoadHeaderFromWeb_02();
Test_Gesat_LoadHeader_02();
Test_Gesat_LoadHeaderPages_02(startPage: 1, maxPage: 6, reload: false, loadImage: false);
Test_Gesat_LoadHeaderPages_02(startPage: 1, maxPage: 0, reload: false, loadImage: false);
Test_Gesat_LoadCompanyFromWeb_02();
Test_Gesat_loadCompany_02("http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/", reload: false, loadImage: false);



Test_Gesat_ExportXmlCompanyList_01(startPage: 1, maxPage: 0, reload: false, loadImage: false);
Test_Gesat_ExportXmlCompanyListFromWeb_01(startPage: 1, maxPage: 0, loadImage: false);

Test_Gesat_ExportXmlCompanyListFromWeb_01(startPage: 1, maxPage: 5, loadImage: false);
Test_Gesat_ExportTextCompanyListFromWeb_01(startPage: 1, maxPage: 5, loadImage: false);

Test_Gesat_loadHeader_01(startPage: 1, maxPage: 5, loadImage: false);
Test_Gesat_loadHeader_02(startPage: 1, maxPage: 5, loadImage: false);
Test_Gesat_loadCompanyList_01(reload: false, startPage: 1, maxPage: 1, loadImage: false);

Test_Gesat_loadCompany_01("http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/", reload: false, loadImage: false);
Test_Gesat_LoadCompanyFromWeb_01();
Test_Gesat_loadCompanyListFromWeb_01(startPage: 1, maxPage: 5, loadImage: false);
Test_Gesat_LoadHeaderFromWeb_01();
Test_Gesat_LoadHeader_01();
Test_Gesat_LoadHeaderPages_01(startPage: 1, maxPage: 5, reload: false, loadImage: false);
Test_Gesat_loadCompanyList_01(startPage: 1, maxPage: 5, reload: false, loadImage: false);

Test_Download_Url_01();
Bug_Path_01();
Test_Uri_01();
Test_Uri_02();
Test_XmlWriter_01();
Test_UrlToFileName_01();
Test_01();
Test_Xml_01();
Test_XmlFile_01();



_tr.WriteLine(zurl.UrlToFileName("http://www.reseau-gesat.com/Gesat/", UrlFileNameType.Path));
_tr.WriteLine(zurl.UrlToFileName("http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html", UrlFileNameType.Path));
_tr.WriteLine(zurl.UrlToFileName("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4017/ALSACE%20ENTREPRISE%20ADAPTEE.htm", UrlFileNameType.Path));
_tr.WriteLine(zurl.UrlToFileName("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4017/ALSACE%20ENTREPRISE%20ADAPTEE.htm", UrlFileNameType.FileName));
_tr.WriteLine(zurl.UrlToFileName("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4017", UrlFileNameType.Path));
_tr.WriteLine(zurl.UrlToFileName("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4017", UrlFileNameType.Path));
_tr.WriteLine(zurl.UrlToFileName("", UrlFileNameType.Path));
_tr.WriteLine(zurl.UrlToFileName("", UrlFileNameType.Path));
_tr.WriteLine(zurl.UrlToFileName("", UrlFileNameType.Path));
_tr.WriteLine(System.Web.HttpUtility.UrlDecode("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4017/ALSACE%20ENTREPRISE%20ADAPTEE.htm"));

Http2.LoadUrl("http://www.reseau-gesat.com/Gesat/");
//<div class="ETABLISSEMENT STAR-1 ODD"> ou <div class="ETABLISSEMENT STAR-1 EVEN">
//<span class="NOM">
//<span class="VILLE">E.S.A.T.<br />Bois-Colombes (92)</span>
_hxr.ReadSelect("//div[starts-with(@class, 'ETABLISSEMENT STAR-')]:.:EmptyRow", ".//span[@class='NOM']//a//text()", ".//span[@class='VILLE']//text()", ".//span[@class='VILLE']//text()[3]");
_hxr.ReadSelect("//div[@class='PAGENAVIGLIST']//a[@title='page suivante']/@href:.:EmptyRow");


Http2.LoadUrl("http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/");
Http2.LoadUrl("http://www.reseau-gesat.com/Gesat/Haute-Garonne,31/Blagnac,11628/esat-maniban,e8474/");
Http2.LoadUrl("http://www.reseau-gesat.com/Gesat/Rhone,69/Limonest,27906/handishare-ctpea,e8486/");


Http2.HtmlReader.WebEncoding = Encoding.UTF8;
Http2.HtmlReader.WebEncoding = Encoding.Default;
_tr.WriteLine("Http2.HtmlReader.WebEncoding \"{0}\"", Http2.HtmlReader.WebEncoding);
Http2.LoadUrl("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire-unea.htm?idRechAnnuaire=Entrez%20le%20nom%20d%27une%20entreprise");
Http2.LoadUrl("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/annuaire.asp?txtRecherche=Entrez%20le%20nom%20d'une%20entreprise&hiddenValider=true");

Test_Unea_request_01();
_hxr.ReadSelect("//div[@class='ctn_result']:.:EmptyRow");
_hxr.ReadSelect("//div[@class='ctn_result']//img:.:EmptyRow", "@alt", "@title", "@class", "@src");
Test_Unea_LoadHeaderFromWeb_01();
Test_Unea_LoadHeader_01();
Test_Unea_LoadHeaderPages_01(startPage: 1, maxPage: 1, reload: false, loadImage: false);
Test_Unea_LoadHeaderPages_01(startPage: 1, maxPage: 0, reload: false, loadImage: false);
Http2.LoadUrl("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/4017/ALSACE%20ENTREPRISE%20ADAPTEE.htm");
Http2.LoadUrl("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4017");
_hxr.ReadSelect("//*[text() = 'Documents téléchargeables : ']:.:EmptyRow", "ancestor::p/@style");
_hxr.ReadSelect("//*[text() = 'Documents téléchargeables : ']:.:EmptyRow", "ancestor::p/@style", ":.:NodeName", "ancestor::p:.:NodeName");
_hxr.ReadSelect("//*[text() = 'Documents téléchargeables : ']:.:EmptyRow", "ancestor::p/@style", ":.:NodeName", "ancestor::p:.:NodeName",
    	"ancestor::p/following::p//a:.:NodeName", "ancestor::p/following::p//a//text()", "ancestor::p/following::p//a/@href");
_hxr.ReadSelect("//*[text() = 'Documents téléchargeables : ']/ancestor::p/following::p//a:.:EmptyRow");
_hxr.ReadSelect("//*[text() = 'Documents téléchargeables : ']/ancestor::p[1]/following::p[1]//a:.:EmptyRow");
_hxr.ReadSelect("//*[text() = 'Documents téléchargeables : ']/ancestor::p[1]/following-sibling::p//a:.:EmptyRow");
_hxr.ReadSelect("//*[text() = 'Documents téléchargeables : ']/ancestor::p/following-sibling::p//a:.:EmptyRow");
Unea_LoadDetailCompany1FromWeb_01("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4017", reload: false, loadImage: false);
Unea_LoadDetailCompany1FromWeb_01("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=4814", reload: false, loadImage: false);
Unea_LoadDetailCompany2FromWeb_01();
Test_Unea_LoadCompanyList_01(startPage: 1, maxPage: 1, reload: false, loadImage: false, companyName: null);
Test_Unea_LoadCompanyList_01(startPage: 1, maxPage: 0, reload: false, loadImage: false, companyName: "ALSACE EURO SERVICES");
Test_Unea_LoadCompanyList_01(startPage: 1, maxPage: 0, reload: false, loadImage: false, companyName: "ANR SERVICES EA LANNION");
Test_Unea_LoadCompanyList_01(startPage: 1, maxPage: 0, reload: false, loadImage: false, companyName: "EA EQUILIBRE");
Test_Unea_LoadCompanyList_01(startPage: 1, maxPage: 0, reload: false, loadImage: false, companyName: "ACTIV ADIS");
Test_Unea_LoadCompanyList_01(startPage: 1, maxPage: 0, reload: false, loadImage: false, companyName: "EA CONCHYLICOLE");

Test_Unea_request_01();

Http2.LoadUrl("http://www.unea.fr/union-nationale-entreprises-adaptees/annuaire-unea/71/71/annuaire/detail.asp?id=3982");
_hxr.ReadSelect("//*[text() = 'Documents téléchargeables : ']/ancestor::p/following-sibling::p//a:.:EmptyRow", ".//text()", "@href");

Http2.LoadUrl("http://fr.wikipedia.org/wiki/Wikip%C3%A9dia:Accueil_principal");

_hxr.ReadSelect("//div:.:EmptyRow", "@class");
_hxr.ReadSelect("//div[@class = 'paginationControl']:.:EmptyRow");
_hxr.ReadSelect("//div[@class = 'paginationControl']//a[position()=last()]:.:EmptyRow", "@href");
_hxr.ReadSelect("//div[@class = 'paginationControl']//a[position()=last()-1]:.:EmptyRow", "@href");
_hxr.ReadSelect("//div[@class = 'paginationControl']//*[position()=last()-1]:.:EmptyRow", "@href", "@class");
PB_Util.Trace.WriteLine(new XXElement(_hxr.XDocument.Root).XPathElement("//div").zToStringOrNull());
PB_Util.Trace.WriteLine(new XXElement(_hxr.XDocument.Root).XPathElement("//div[@class = 'paginationControl']").XElement.zToStringOrNull());

Test_Handeco_LoadHeaderPages_01(nbPage: 1);
_hxr.ReadSelect("//table//tbody/tr:.:EmptyRow", ".//text()");
// company, siret, type, group, sector, postalCode
_hxr.ReadSelect("//table//tr[position() > 1]:.:EmptyRow", ".//td[1]//text():.:n(company)", ".//td[2]//text():.:n(siret)",
    ".//td[3]//text():.:n(type)", ".//td[4]//text():.:n(group)", ".//td[5]//text():.:n(sector)", ".//td[6]//text():.:n(post code)");

Http2.LoadUrl("http://www.handeco.org/fournisseur/consulter/id/1906", new HttpRequestParameters { encoding = Encoding.UTF8 });      // company CTRE DISTRIB TRAV A DOMICILE
Http2.LoadUrl("http://www.handeco.org/fournisseur/consulter/id/1252", new HttpRequestParameters { encoding = Encoding.UTF8 });      // contient NOTRE OFFRE avec 3 réponses, company EA JEU SER

Http2.LoadUrl(@"c:\pib\dev_data\exe\runsource\damien\cache\Handeco\company\fournisseur_consulter_id_1008.html", new HttpRequestParameters { encoding = Encoding.UTF8 });
HtmlXmlReader.CurrentHtmlXmlReader.http.Dispose();

//<table class="fiche organisation">
_hxr.ReadSelect("//table:.:EmptyRow", "@class");
_hxr.ReadSelect("//table//tr:.:EmptyRow", "ancestor::table/@id", ".//th//text()", ".//td//text()");
_hxr.ReadSelect("//em:.:EmptyRow");
PB_Util.Trace.WriteLine(new XXElement(_hxr.XDocument.Root).XPathValue("//em/text()"));
PB_Util.Trace.WriteLine(new XXElement(_hxr.XDocument.Root).XPathValue("//em[starts-with(text(), 'Dernière mise à jour')]/text()"));
PB_Tools.Test.Test_Regex("[0-9]{2}-[0-9]{2}-[0-9]{4}", "Dernière mise à jour le 18-01-2013");
