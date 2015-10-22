using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using MongoDB.Bson;
using pb;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;
using PB_Util;

// Entreprise adaptée (EA) http://fr.wikipedia.org/wiki/Entreprise_adapt%C3%A9e
// Établissement et service d'aide par le travail (ESAT) http://fr.wikipedia.org/wiki/%C3%89tablissement_et_service_d'aide_par_le_travail
// lecture de
//   http://www.reseau-gesat.com/Gesat/

// Excel
// Créer une liste déroulante dans Excel 2010 http://syskb.com/creer-une-liste-deroulante-dans-excel-2010/

namespace Download.Gesat
{
    public static class Gesat
    {
        public static void Init()
        {
            Gesat_LoadHeaderFromWeb2.ClassInit(XmlConfig.CurrentConfig.GetElement("Gesat/Header"));
            Gesat_LoadHeader2.ClassInit(XmlConfig.CurrentConfig.GetElement("Gesat/Header"));
            Gesat_LoadCompanyFromWeb2.ClassInit(XmlConfig.CurrentConfig.GetElement("Gesat/Company"));
        }

        public static void ExportXmlCompanyList(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            WriteLine("ExportXmlCompanyList : startPage {0} maxPage {1} reload {2} loadImage {3}", startPage, maxPage, reload, loadImage);
            Init();
            _ExportXmlCompanyList(from header in new Gesat_LoadHeaderPages2(startPage: startPage, maxPage: maxPage, reload: reload, loadImage: loadImage) select Gesat_LoadCompany2.LoadCompany(header.url, header, reload: reload, loadImage: loadImage));
        }

        public static void _ExportXmlCompanyList(IEnumerable<Gesat_Company> companyList)
        {
            string file = @"c:\pib\dev_data\exe\wrun\damien\export\Gesat.xml";
            string fileDetail = @"c:\pib\dev_data\exe\wrun\damien\export\GesatDetail.xml";
            WriteLine("export réseau Gesat");
            WriteLine("   file        \"{0}\"", file);
            WriteLine("   file detail \"{0}\"", fileDetail);
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(file, settings), xwDetail = XmlWriter.Create(fileDetail, settings))
            {
                xw.WriteStartElement("Gesat");
                xwDetail.WriteStartElement("Gesat");
                foreach (Gesat_Company company in companyList)
                {
                    //if (_rs.Abort)
                    //    break;
                    ExportXml_Company(xw, company, false);
                    ExportXml_Company(xwDetail, company, true);
                }
                xw.WriteEndElement();
                xwDetail.WriteEndElement();
            }
        }

        public static void ExportXml_Company(XmlWriter xw, Gesat_Company company, bool detail)
        {
            IEnumerator<string> infos = null;
            IEnumerator<string> activities = null;

            if (detail)
            {
                infos = ((IEnumerable<string>)company.infos).GetEnumerator();
                activities = ((IEnumerable<string>)company.activities).GetEnumerator();
            }

            xw.WriteStartElement("Company");
            xw.zWriteElementText("société", company.name);
            xw.zWriteElementText("société2", company.headerName);
            xw.zWriteElementText("type", company.type);
            if (detail)
            {
                if (infos.MoveNext())
                    xw.zWriteElementText("info", infos.Current);
                if (activities.MoveNext())
                    xw.zWriteElementText("activité", activities.Current);
            }
            xw.zWriteElementText("ville", company.city);
            xw.zWriteElementText("département", company.department);
            xw.zWriteElementText("tel", company.phone);
            xw.zWriteElementText("tel2", company.headerPhone);
            xw.zWriteElementText("fax", company.fax);
            xw.zWriteElementText("email", company.email);
            xw.zWriteElementText("site", company.webSite);
            xw.zWriteElementText("descryption", company.descryption);
            xw.zWriteElementText("adresse", company.address);
            xw.zWriteElementText("load_date", string.Format("{0:dd/MM/yyyy HH:mm}", company.loadFromWebDate));
            xw.zWriteElementText("url", company.url);
            xw.zWriteElementText("emplacement", company.location);
            xw.WriteEndElement();

            while (detail)
            {
                bool info = infos.MoveNext();
                bool activity = activities.MoveNext();
                if (!info && !activity)
                    break;
                xw.WriteStartElement("Company");
                if (info)
                    xw.zWriteElementText("info", infos.Current);
                if (activity)
                    xw.zWriteElementText("activité", activities.Current);
                xw.WriteEndElement();
            }
        }

        public static void ExportTextCompanyList(IEnumerable<Gesat_Company> companyList)
        {
            string file = @"c:\pib\dev_data\exe\wrun\damien\export\Gesat.txt";
            string fileInfos = @"c:\pib\dev_data\exe\wrun\damien\export\Gesat_infos.txt";
            string fileUniqInfos = @"c:\pib\dev_data\exe\wrun\damien\export\Gesat_infos_uniq.txt";
            string fileActivities = @"c:\pib\dev_data\exe\wrun\damien\export\Gesat_activities.txt";
            string fileUniqActivities = @"c:\pib\dev_data\exe\wrun\damien\export\Gesat_activities_uniq.txt";
            WriteLine("export réseau Gesat to \"{0}\"", file);

            SortedDictionary<string, string> uniqInfos = new SortedDictionary<string, string>();
            SortedDictionary<string, string> uniqActivities = new SortedDictionary<string, string>();

            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            FileStream fsInfos = new FileStream(fileInfos, FileMode.Create, FileAccess.Write, FileShare.Read);
            FileStream fsActivities = new FileStream(fileActivities, FileMode.Create, FileAccess.Write, FileShare.Read);
            using (StreamWriter sw = new StreamWriter(fs, Encoding.Default), swInfo = new StreamWriter(fsInfos, Encoding.Default), swActivity = new StreamWriter(fsActivities, Encoding.Default))
            {
                // BOM EFBBBF from EditPlus
                //sw.Write("\xEF\xBB\xBF");

                sw.Write("société");
                sw.Write("\tsociété 2");
                sw.Write("\ttype");
                sw.Write("\tville");
                sw.Write("\tdepartement");
                sw.Write("\ttel");
                sw.Write("\ttel 2");
                sw.Write("\tfax");
                sw.Write("\temail");
                sw.Write("\tsite");
                sw.Write("\tdescription");
                sw.Write("\tadresse");
                //sw.Write("\tactivities");
                //sw.Write("\tinfos");
                sw.Write("\tload date");
                sw.Write("\turl");
                sw.Write("\templacement");
                sw.WriteLine();

                swInfo.Write("société");
                swInfo.Write("\tinfo");
                swInfo.WriteLine();

                swActivity.Write("société");
                swActivity.Write("\tactivité");
                swActivity.WriteLine();

                foreach (Gesat_Company company in companyList)
                {
                    sw.Write(company.name);
                    sw.Write("\t"); sw.Write(company.headerName);
                    sw.Write("\t"); sw.Write(company.type);
                    sw.Write("\t"); sw.Write(company.city);
                    sw.Write("\t"); sw.Write(company.department);
                    sw.Write("\t"); sw.Write(company.phone);
                    sw.Write("\t"); sw.Write(company.headerPhone);
                    sw.Write("\t"); sw.Write(company.fax);
                    sw.Write("\t"); sw.Write(company.email);
                    sw.Write("\t"); sw.Write(company.webSite);
                    sw.Write("\t"); sw.Write(company.descryption);
                    sw.Write("\t"); sw.Write(company.address);
                    //sw.Write("\t"); sw.Write(company.activities);
                    //sw.Write("\t"); sw.Write(company.infos);
                    sw.Write("\t"); sw.Write("{0:dd/MM/yyyy HH:mm}", company.loadFromWebDate);
                    sw.Write("\t"); sw.Write(company.url);
                    sw.Write("\t"); sw.Write(company.location);
                    sw.WriteLine();

                    foreach (string info in company.infos)
                    {
                        swInfo.Write(company.name);
                        swInfo.Write("\t"); swInfo.Write(info);
                        swInfo.WriteLine();

                        if (!uniqInfos.ContainsKey(info))
                            uniqInfos.Add(info, null);
                    }

                    foreach (string activity in company.activities)
                    {
                        swActivity.Write(company.name);
                        swActivity.Write("\t"); swActivity.Write(activity);
                        swActivity.WriteLine();

                        if (!uniqActivities.ContainsKey(activity))
                            uniqActivities.Add(activity, null);
                    }
                }
            }
            fs.Close();
            fsInfos.Close();
            fsActivities.Close();

            FileStream fsUniqInfos = new FileStream(fileUniqInfos, FileMode.Create, FileAccess.Write, FileShare.Read);
            using (StreamWriter swUniqInfo = new StreamWriter(fsUniqInfos, Encoding.Default))
            {
                foreach (string info in uniqInfos.Keys)
                {
                    swUniqInfo.WriteLine(info);
                }
            }
            fsUniqInfos.Close();

            FileStream fsUniqActivities = new FileStream(fileUniqActivities, FileMode.Create, FileAccess.Write, FileShare.Read);
            using (StreamWriter swUniqActivity = new StreamWriter(fsUniqActivities, Encoding.Default))
            {
                foreach (string activity in uniqActivities.Keys)
                {
                    swUniqActivity.WriteLine(activity);
                }
            }
            fsUniqActivities.Close();
        }

        private static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }

    public class Gesat_Base
    {
        public string url = null;
        public DateTime? loadFromWebDate = null;
        public string name = null;
        public string type = null;
        public string location = null;
        public string phone = null;
        public string[] infos = null;
    }

    public class Gesat_HeaderCompany : Gesat_Base
    {
        public string sourceUrl;
    }

    public class Gesat_HeaderPage
    {
        public Gesat_HeaderCompany[] headerCompanies;
        public string urlNextPage;
    }

    public class Gesat_Company : Gesat_Base
    {
        public string headerName = null;
        public string headerPhone = null;
        public string city = null;
        public string department = null;
        public string descryption = null;
        public string address = null;
        public string fax = null;
        public string email = null;
        public string webSite = null;
        public string[] activities = null;

        public void DocumentMongoLoad(BsonDocument doc)
        {
            url = doc.zGetString("url");
            loadFromWebDate = doc.zGetDateTime("loadFromWebDate");
            //title = doc.zGetString("title");
            //creationDate = doc.zGetDateTime("creationDate");
            //author = doc.zGetString("author");
            //category = doc.zGetString("category");
            //foreach (BsonDocument doc2 in doc.zGetBsonArray("values"))
            //{
            //    string name = doc2.zGetString("name");
            //    string value = doc2.zGetString("value");
            //    if (name != null)
            //        infos.SetValue(name, new ZString(value));
            //}
        }

        public void DocumentMongoSave(BsonDocument doc)
        {
            doc.zAdd("url", url);
            doc.zAdd("loadFromWebDate", loadFromWebDate);
            //doc.zAdd("title", title);
            //doc.zAdd("creationDate", creationDate);
            //doc.zAdd("author", author);
            //doc.zAdd("category", category);
            //BsonArray values = doc.zAddArray("values");
            //foreach (KeyValuePair<string, ZValue> value in infos)
            //{
            //    BsonDocument valueDoc = values.zAddDocument();
            //    valueDoc.zAdd("name", value.Key);
            //    valueDoc.zAdd("value", (string)value.Value);
            //}
        }

        private static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }

    public class Gesat_LoadHeaderFromWeb2 : LoadDataFromWeb_v1<Gesat_HeaderPage>
    {
        private bool _loadImage = false;
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;
        private static Func<string, string> _trimFunc1 = text => text.Trim();

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");
        }

        public Gesat_LoadHeaderFromWeb2(string url, bool reload = false, bool loadImage = false)
            : base(url, reload: reload)
        {
            SetRequestParameters(new HttpRequestParameters_v1() { encoding = Encoding.UTF8 });
            if (__useUrlCache)
                SetUrlCache(new UrlCache_v1(__cacheDirectory, __urlFileNameType));
            _loadImage = loadImage;
        }

        protected override Gesat_HeaderPage GetData()
        {
            XXElement xeSource = new XXElement(GetXmlDocument().Root);
            string url = Url;
            Gesat_HeaderPage data = new Gesat_HeaderPage();
            // <div class="PAGENAVIGLIST">
            // <a href="/Gesat/EtablissementList-10-10.html" title="page suivante">&gt;</a>&nbsp;
            data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='PAGENAVIGLIST']//a[@title='page suivante']/@href"));
            // <div class="ETABLISSEMENT STAR-1 ODD"> <div class="ETABLISSEMENT STAR-0 ODD"> <div class="ETABLISSEMENT STAR-1 EVEN">
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[starts-with(@class, 'ETABLISSEMENT STAR-')]");
            List<Gesat_HeaderCompany> headers = new List<Gesat_HeaderCompany>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Gesat_HeaderCompany header = new Gesat_HeaderCompany();
                header.sourceUrl = url;
                header.loadFromWebDate = DateTime.Now;

                //<span class="NOM"><a title="ESAT BETTY LAUNAY-MOULIN VERT" href="/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/">ESAT BETTY LAUNAY-MOULIN VERT</a></span>
                //_header.companyName = xeHeader.ExplicitXPathValue(".//span[@class='NOM']//a//text()");
                XXElement xe = xeHeader.XPathElement(".//span[@class='NOM']//a");
                if (xe != null)
                {
                    header.url = zurl.GetUrl(url, xe.ExplicitXPathValue("@href"));
                    //header.name = xe.ExplicitXPathValue(".//text()", _trimFunc1);
                    header.name = _trimFunc1(xe.ExplicitXPathValue(".//text()"));
                }
                //<span class="VILLE">E.S.A.T.<br />Bois-Colombes (92)</span>
                xe = xeHeader.XPathElement(".//span[@class='VILLE']");
                if (xe != null)
                {
                    //IEnumerator<string> texts = xe.DescendantTextList().GetEnumerator();
                    IEnumerator<string> texts = xe.DescendantTexts().GetEnumerator();
                    if (texts.MoveNext())
                        header.type = texts.Current.Trim();
                    else
                        Trace.CurrentTrace.WriteLine("error companyType not found");
                    if (texts.MoveNext())
                        header.location = texts.Current.Trim();
                    else
                        Trace.CurrentTrace.WriteLine("error companyLocation not found");

                }
                // <span class="TELEPHONE">01 47 86 11 48</span>
                //header.phone = xeHeader.ExplicitXPathValue(".//span[@class='TELEPHONE']//text()", _trimFunc1);
                header.phone = _trimFunc1(xeHeader.ExplicitXPathValue(".//span[@class='TELEPHONE']//text()"));
                //<img info_bulle="Signataire de la charte Ethique et Valeurs" border="0" alt="/images/bullesGesat/pictoCharte.png" src="/images/bullesGesat/pictoCharte.png" style=" border: 0;" />
                //<img info_bulle="Lauréat des trophées HandiResponsables 2013" border="0" alt="/images/bullesGesat/LAURIERS-OR-2013.png" src="/images/bullesGesat/LAURIERS-OR-2013.png" style=" border: 0;" />
                //header.infos = xeHeader.XPathValues(".//img/@info_bulle", _trimFunc1);
                header.infos = xeHeader.XPathValues(".//img/@info_bulle").Select(_trimFunc1).ToArray();
                //_header.SetInfo(xeHeader.XPathValues(".//img/@info_bulle"));
                headers.Add(header);
            }
            data.headerCompanies = headers.ToArray();
            return data;
        }
    }

    public class Gesat_LoadHeader2 : LoadWebData_v1<Gesat_HeaderPage>
    {
        protected static Regex __KeyRegex = new Regex("/Gesat/(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //protected static string __imageCacheDirectory = "image";
        protected static bool __useXml = false;
        protected static bool __useMongo = false;
        protected static string __mongoServer = null;
        protected static string __mongoDatabase = null;
        protected static string __mongoCollectionName = null;

        public static void ClassInit(XElement xe)
        {
            //__imageCacheDirectory = xe.zXPathValue("ImageCacheDirectory", __imageCacheDirectory);
            //__useXml = xe.zXPathValueBool("UseXml", __useXml);
            __useXml = xe.zXPathValue("UseXml").zTryParseAs(__useXml);
            //__useMongo = xe.zXPathValueBool("UseMongo", __useMongo);
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
        }

        public Gesat_LoadHeader2(string url)
            : base(url)
        {
            //_imageCacheDirectory = __imageCacheDirectory;
            SetXmlParameters(__useXml);
            SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);
        }

        //public static string ImageCacheDirectory { get { return __imageCacheDirectory; } set { __imageCacheDirectory = value; } }
        //public static bool DocumentXml { get { return __documentXml; } set { __documentXml = value; } }
        //public static bool DocumentMongoDb { get { return __documentMongoDb; } set { __documentMongoDb = value; } }
        //public static string MongoServer { get { return __mongoServer; } set { __mongoServer = value; } }
        //public static string MongoDatabase { get { return __mongoDatabase; } set { __mongoDatabase = value; } }
        //public static string MongoCollectionName { get { return __mongoCollectionName; } set { __mongoCollectionName = value; } }

        protected override string GetName()
        {
            return "Gesat header";
        }

        protected override Gesat_HeaderPage LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            Gesat_LoadHeaderFromWeb2 loadFromWeb = new Gesat_LoadHeaderFromWeb2(Url, reload, loadImage);
            loadFromWeb.Load();
            return loadFromWeb.Data;
        }

        //protected override Gesat_HeaderPage LoadDocumentFromXml(string file, bool loadImage = false)
        //{
        //    //_data = loadFromXml.Data;
        //    throw new NotImplementedException();
        //}

        //protected override void SaveDocumentToXml(XmlWriter xw, bool saveImage = true)
        //{
        //    //string imageFile = null;
        //    //string fileDirectory = null;
        //    //if (saveImage)
        //    //{
        //    //    fileDirectory = GetFileDirectory();
        //    //    imageFile = GetImageFile();
        //    //}

        //    //_data.DocumentXmlSave(xw);
        //    throw new NotImplementedException();
        //}

        //protected override Gesat_HeaderPage LoadDocumentFromMongo(BsonDocument doc, bool loadImage = false)
        //{
        //    ////Trace.CurrentTrace.WriteLine("_DocumentMongoLoad loadImage {0}", loadImage);
        //    //_data = new Gesat_Company();
        //    //_data.DocumentMongoLoad(doc);
        //    throw new NotImplementedException();
        //}

        //protected override void SaveDocumentToMongo(BsonDocument doc, bool saveImage = true)
        //{
        //    //string imageFile = null;
        //    //string fileDirectory = null;
        //    //if (saveImage)
        //    //{
        //    //    fileDirectory = GetFileDirectory();
        //    //    imageFile = GetImageFile();
        //    //}
        //    //_data.DocumentMongoSave(doc);
        //    throw new NotImplementedException();
        //}

        protected override object GetDocumentKey()
        {
            //http://www.reseau-gesat.com/Gesat/
            //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            Match match = __KeyRegex.Match(Url);
            if (!match.Success)
                throw new PB_Util_Exception("key not found in url \"{0}\"", Url);
            string key = match.Groups[1].Value;
            if (key.EndsWith("/"))
                key = key.Substring(0, key.Length - 1);
            if (key == "")
                key = "EtablissementList-0-10.html";
            Trace.CurrentTrace.WriteLine("key \"{0}\"", key);
            return key;
        }
    }

    public class Gesat_LoadHeaderPages2 : LoadWebDataPages_v1<Gesat_HeaderCompany>
    {
        protected Gesat_HeaderPage _headerPage = null;
        protected IEnumerator<Gesat_HeaderCompany> _enumerator = null;
        protected static string _urlHeader = "http://www.reseau-gesat.com/Gesat/";
        protected static int _headersNumberByPage = 10;

        public Gesat_LoadHeaderPages2(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
            : base(startPage, maxPage, reload, loadImage)
        {
        }

        //protected override string GetUrlPage(int page)
        protected override void GetUrlPage(int page, out string url, out HttpRequestParameters_v1 requestParameters)
        {
            //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            url = _urlHeader;
            if (page != 1)
                url += string.Format("EtablissementList-{0}-10.html", (page - 1) * _headersNumberByPage);
            requestParameters = null;
        }

        protected override void Load()
        {
            Gesat_LoadHeader2 load = new Gesat_LoadHeader2(Url);
            load.Load(Reload, LoadImage);
            _headerPage = load.Data;
            _enumerator = _headerPage.headerCompanies.AsEnumerable<Gesat_HeaderCompany>().GetEnumerator();
        }

        protected override bool GetNextItem()
        {
            return _enumerator.MoveNext();
        }

        protected override Gesat_HeaderCompany GetCurrentItem()
        {
            return _enumerator.Current;
        }

        //protected override string GetUrlNextPage()
        protected override bool GetUrlNextPage(out string url, out HttpRequestParameters_v1 requestParameters)
        {
            url = _headerPage.urlNextPage;
            requestParameters = null;
            if (url != null)
                return true;
            else
                return false;
        }

        //public bool MoveNext()
        //{
        //    while (true)
        //    {
        //        if (_enumerator == null)
        //        {
        //            Gesat_LoadHeader load = new Gesat_LoadHeader(_url);
        //            load.Load(_reload, _loadImage);
        //            _headerPage = load.Data;
        //            _enumerator = _headerPage.headerCompanies.AsEnumerable<Gesat_HeaderCompany>().GetEnumerator();
        //        }
        //        if (_enumerator.MoveNext())
        //            return true;
        //        if (_nbPage == _maxPage && _maxPage != 0)
        //            return false;
        //        _url = _headerPage.urlNextPage;
        //        if (_url == null)
        //            return false;
        //        _headerPage = null;
        //        _enumerator = null;
        //        _nbPage++;
        //    }
        //}
    }

    public class Gesat_LoadCompanyFromWeb2 : LoadDataFromWeb_v1<Gesat_Company>
    {
        private Gesat_HeaderCompany _header;
        private bool _loadImage = false;
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;
        private static Func<string, string> _trimFunc1 = text => text.Trim();
        private static Func<string, string> _trimFunc2 = text => text.Trim(' ', '>');

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");
        }

        public Gesat_LoadCompanyFromWeb2(string url, Gesat_HeaderCompany header, bool reload = false, bool loadImage = false)
            : base(url, reload: reload)
        {
            _header = header;
            SetRequestParameters(new HttpRequestParameters_v1() { encoding = Encoding.UTF8 });
            if (__useUrlCache)
                SetUrlCache(new UrlCache_v1(__cacheDirectory, __urlFileNameType));
            _loadImage = loadImage;
        }

        protected override Gesat_Company GetData()
        {
            XXElement xeSource = new XXElement(GetXmlDocument().Root);
            Gesat_Company data = new Gesat_Company();
            data.url = Url;
            data.loadFromWebDate = DateTime.Now;

            if (_header != null)
            {
                data.name = _header.name;
                data.type = _header.type;
                data.location = _header.location;
                data.phone = _header.phone;
                data.infos = _header.infos;
            }

            // <div class="PAGES" id="content">
            XXElement xe = xeSource.XPathElement(".//div[@id='content']");

            // <h1><span>ESAT BETTY LAUNAY-MOULIN VERT >></span><br />Coordonnées & activités</h1>
            //string s = xe.XPathValue(".//h1//text()", _trimFunc2);
            string s = _trimFunc2(xe.XPathValue(".//h1//text()"));
            //s = s.Trim(' ', '>');
            if (!s.Equals(data.name, StringComparison.InvariantCultureIgnoreCase))
            {
                data.headerName = data.name;
                data.name = s;
            }

            // <div class="BLOC B100 ACCROCHE">
            // <div class="CONTENU-BLOC">Cet E.S.A.T. est ouvert depuis 1989 et accueille 55 personnes reconnues travailleurs handicapés.  Il est situé dans la ville de 
            // <a href="/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/" title="Bois-Colombes // Les ESAT et EA de la ville">Bois-Colombes</a> (
            // <a href="/Gesat/Hauts-de-Seine,92/" title="Hauts-de-Seine // Les ESAT et EA du département">Hauts-de-Seine</a>)
            // </div></div>
            data.descryption = xe.XPathConcatText(".//div[@class='BLOC B100 ACCROCHE']//text()", resultFunc: _trimFunc1);
            data.descryption = data.descryption.Replace("\r", "");
            data.descryption = data.descryption.Replace("\n", "");
            data.descryption = data.descryption.Replace("\t", "");
            //data.city = xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[1]//text()", _trimFunc1);
            data.city = _trimFunc1(xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[1]//text()"));
            //data.department = xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[2]//text()", _trimFunc1);
            data.department = _trimFunc1(xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[2]//text()"));

            // <div class="ADRESSE">78, RUE RASPAIL<br />92270  Bois-Colombes</div>
            data.address = xe.XPathConcatText(".//div[@class='ADRESSE']//text()", " ", itemFunc: _trimFunc1);
            data.address = data.address.Replace("\r", "");
            data.address = data.address.Replace("\n", "");
            data.address = data.address.Replace("\t", "");

            // <div class="TEL">01 47 86 11 48</div>
            //s = xe.XPathValue(".//div[@class='TEL']//text()", _trimFunc1);
            s = _trimFunc1(xe.XPathValue(".//div[@class='TEL']//text()"));
            if (!s.Equals(data.phone, StringComparison.InvariantCultureIgnoreCase))
            {
                data.headerPhone = data.phone;
                data.phone = s;
            }

            // <div class="FAX">01 47 82 42 64</div>
            //data.fax = xe.XPathValue(".//div[@class='FAX']//text()", _trimFunc1);
            data.fax = _trimFunc1(xe.XPathValue(".//div[@class='FAX']//text()"));

            // <div class="EMAIL">production.launay<img border="0" alt="arobase.png" src="/images/bulles/arobase.png" style=" border: 0;" />lemoulinvert.org</div>
            data.email = xe.XPathConcatText(".//div[@class='EMAIL']//text()", "@", itemFunc: _trimFunc1);

            // <div class="WWW"><a href="http://www.esat-b-launay.com" target="_blank">www.esat-b-launay.com</a></div>
            //data.webSite = xe.XPathValue(".//div[@class='WWW']//a/@href", _trimFunc1);
            data.webSite = _trimFunc1(xe.XPathValue(".//div[@class='WWW']//a/@href"));

            // <div class="BLOC-FICHE BLOC-ACTIVITES">
            // <dl><dt>Conditionnement, travaux &agrave; fa&ccedil;on</dt></dl>
            // <dl><dt>Assemblage, montage</dt></dl>
            // <dl><dt>Mise sous pli, mailing, routage</dt></dl>
            // <dl><dt>Toutes activit&eacute;s en entreprise </dt></dl>
            // <dl><dt>Num&eacute;risation, saisie informatique</dt></dl>
            // <dl><dt>Remplissage, ensachage, flaconnage</dt></dl>
            // <dl><dt>Etiquetage, codage, badges</dt></dl>
            // <dl><dt>Secr&eacute;tariat, travaux administratifs</dt></dl>
            // <dl><dt>Artisanats divers</dt></dl>
            // </div>
            //data.activities = xe.XPathValues(".//div[@class='BLOC-FICHE BLOC-ACTIVITES']//dl//text()", _trimFunc1);
            data.activities = xe.XPathValues(".//div[@class='BLOC-FICHE BLOC-ACTIVITES']//dl//text()").Select(_trimFunc1).ToArray();

            return data;
        }

        //protected static void WriteLine(string msg, params object[] prm)
        //{
        //    Trace.CurrentTrace.WriteLine(msg, prm);
        //}

        public static Gesat_Company LoadCompany(string url, Gesat_HeaderCompany header, bool reload = false, bool loadImage = false)
        {
            Gesat_LoadCompanyFromWeb2 load = new Gesat_LoadCompanyFromWeb2(url, header, reload, loadImage);
            load.Load();
            return load.Data;
        }
    }

    public class Gesat_LoadCompany2 : LoadWebData_v1<Gesat_Company>
    {
        protected Gesat_HeaderCompany _header = null;
        //protected Gesat_Company _data = null;

        protected static Regex __KeyRegex = new Regex("/Gesat/(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //protected static string __imageCacheDirectory = "image";
        protected static bool __useXml = false;
        protected static bool __useMongo = false;
        protected static string __mongoServer = null;
        protected static string __mongoDatabase = null;
        protected static string __mongoCollectionName = null;

        public static void ClassInit(XElement xe)
        {
            //__imageCacheDirectory = xe.zXPathValue("ImageCacheDirectory", __imageCacheDirectory);
            //__useXml = xe.zXPathValueBool("UseXml", __useXml);
            __useXml = xe.zXPathValue("UseXml").zTryParseAs(__useXml);
            //__useMongo = xe.zXPathValueBool("UseMongo", __useMongo);
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
        }

        public Gesat_LoadCompany2(string url, Gesat_HeaderCompany header)
            : base(url)
        {
            _header = header;
            //_imageCacheDirectory = __imageCacheDirectory;
            SetXmlParameters(__useXml);
            SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);
        }

        //public Gesat_Company Data { get { return _data; } }

        //public static string ImageCacheDirectory { get { return __imageCacheDirectory; } set { __imageCacheDirectory = value; } }
        //public static bool DocumentXml { get { return __documentXml; } set { __documentXml = value; } }
        //public static bool DocumentMongoDb { get { return __documentMongoDb; } set { __documentMongoDb = value; } }
        //public static string MongoServer { get { return __mongoServer; } set { __mongoServer = value; } }
        //public static string MongoDatabase { get { return __mongoDatabase; } set { __mongoDatabase = value; } }
        //public static string MongoCollectionName { get { return __mongoCollectionName; } set { __mongoCollectionName = value; } }

        protected override string GetName()
        {
            return "Gesat company";
        }

        protected override Gesat_Company LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            Gesat_LoadCompanyFromWeb2 loadFromWeb = new Gesat_LoadCompanyFromWeb2(Url, _header, reload, loadImage);
            loadFromWeb.Load();
            return loadFromWeb.Data;
        }

        //protected override Gesat_Company LoadDocumentFromXml(string file, bool loadImage = false)
        //{
        //    //TelechargementPlus_LoadPostFromXml loadFromXml = new TelechargementPlus_LoadPostFromXml(file, loadImage);
        //    //_data = loadFromXml.Data;
        //    throw new NotImplementedException();
        //}

        //protected override void SaveDocumentToXml(XmlWriter xw, bool saveImage = true)
        //{
        //    //string imageFile = null;
        //    //string fileDirectory = null;
        //    //if (saveImage)
        //    //{
        //    //    fileDirectory = GetFileDirectory();
        //    //    imageFile = GetImageFile();
        //    //}

        //    //_data.DocumentXmlSave(xw);
        //    throw new NotImplementedException();
        //}

        //protected override Gesat_Company LoadDocumentFromMongo(BsonDocument doc, bool loadImage = false)
        //{
        //    ////Trace.CurrentTrace.WriteLine("_DocumentMongoLoad loadImage {0}", loadImage);
        //    //_data = new Gesat_Company();
        //    //_data.DocumentMongoLoad(doc);
        //    throw new NotImplementedException();
        //}

        //protected override void SaveDocumentToMongo(BsonDocument doc, bool saveImage = true)
        //{
        //    //string imageFile = null;
        //    //string fileDirectory = null;
        //    //if (saveImage)
        //    //{
        //    //    fileDirectory = GetCacheFileDirectory();
        //    //    imageFile = GetImageFile();
        //    //}
        //    //_data.DocumentMongoSave(doc);
        //    throw new NotImplementedException();
        //}

        protected override object GetDocumentKey()
        {
            // http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/
            Match match = __KeyRegex.Match(Url);
            if (!match.Success)
                throw new PB_Util_Exception("key not found in url \"{0}\"", Url);
            string key = match.Groups[1].Value;
            if (key.EndsWith("/"))
                key = key.Substring(0, key.Length - 1);
            Trace.CurrentTrace.WriteLine("key \"{0}\"", key);
            return key;
        }

        public static Gesat_Company LoadCompany(string url, Gesat_HeaderCompany header, bool reload, bool loadImage)
        {
            Gesat_LoadCompany2 load = new Gesat_LoadCompany2(url, header);
            load.Load(reload, loadImage);
            return load.Data;
        }
    }
}
