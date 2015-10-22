using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using pb;
using PB_Util;
using pb.Data.Xml;
using pb.Web;

namespace Download.Gesat
{
    public class Gesat_LoadHeaderFromWeb : LoadFromWebBase_v1 
    {
        protected Gesat_HeaderPage _data = null;
        private bool _loadImage = false;
        //private XXElement _xeSource = null;
        //private string _urlNextPage = null;
        //protected IEnumerator<XXElement> _xmlEnum = null;
        private static Func<string, string> _trimFunc1 = text => text.Trim();

        public Gesat_LoadHeaderFromWeb(string url, string urlFile = null, bool reload = false, bool loadImage = false)
        {
            _url = url;
            _urlFile = urlFile;
            _reload = reload;
            _loadImage = loadImage;
        }

        public Gesat_HeaderPage Data { get { return _data; } }

        protected override void SetXml(XElement xelement)
        {
            XXElement xeSource = new XXElement(xelement);
            _data = new Gesat_HeaderPage();
            // <div class="PAGENAVIGLIST">
            // <a href="/Gesat/EtablissementList-10-10.html" title="page suivante">&gt;</a>&nbsp;
            _data.urlNextPage = GetUrl(xeSource.XPathValue("//div[@class='PAGENAVIGLIST']//a[@title='page suivante']/@href"));
            // <div class="ETABLISSEMENT STAR-1 ODD"> <div class="ETABLISSEMENT STAR-0 ODD"> <div class="ETABLISSEMENT STAR-1 EVEN">
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[starts-with(@class, 'ETABLISSEMENT STAR-')]");
            List<Gesat_HeaderCompany> headers = new List<Gesat_HeaderCompany>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Gesat_HeaderCompany header = new Gesat_HeaderCompany();
                header.sourceUrl = _url;
                header.loadFromWebDate = DateTime.Now;

                //<span class="NOM"><a title="ESAT BETTY LAUNAY-MOULIN VERT" href="/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/">ESAT BETTY LAUNAY-MOULIN VERT</a></span>
                //_header.companyName = xeHeader.ExplicitXPathValue(".//span[@class='NOM']//a//text()");
                XXElement xe = xeHeader.XPathElement(".//span[@class='NOM']//a");
                if (xe != null)
                {
                    header.url = GetUrl(xe.ExplicitXPathValue("@href"));
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
            _data.headerCompanies = headers.ToArray();
        }

        //public string GetUrlNextPage()
        //{
        //    return _urlNextPage;
        //}
    }

    public class Gesat_LoadHeader : HttpLoad, IDisposable 
    {
        protected Gesat_HeaderPage _data = null;

        protected static Regex __KeyRegex = new Regex("/Gesat/(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        protected static bool __useUrlCache = false;
        protected static string __cacheDirectory = null;
        protected static string __imageCacheDirectory = "image";
        protected static bool __documentXml = false;
        protected static bool __documentMongoDb = false;
        protected static string __mongoServer = null;
        protected static string __mongoDatabase = null;
        protected static string __mongoCollectionName = null;

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");
            __imageCacheDirectory = xe.zXPathValue("ImageCacheDirectory", __imageCacheDirectory);
            //__documentXml = xe.zXPathValueBool("DocumentXml", __documentXml);
            __documentXml = xe.zXPathValue("DocumentXml").zTryParseAs(__documentXml);
            //__documentMongoDb = xe.zXPathValueBool("DocumentMongoDb", __documentMongoDb);
            __documentMongoDb = xe.zXPathValue("DocumentMongoDb").zTryParseAs(__documentMongoDb);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
        }

        public Gesat_LoadHeader(string url)
            : base(url)
        {
            _useUrlCache = __useUrlCache;
            _cacheDirectory = __cacheDirectory;
            _imageCacheDirectory = __imageCacheDirectory;
            _documentXml = __documentXml;
            _documentMongoDb = __documentMongoDb;
            _mongoServer = __mongoServer;
            _mongoDatabase = __mongoDatabase;
            _mongoCollectionName = __mongoCollectionName;
        }

        public void Dispose()
        {
        }

        public Gesat_HeaderPage Data { get { return _data; } }

        public static bool UseUrlCache { get { return __useUrlCache; } set { __useUrlCache = value; } }
        public static string CacheDirectory { get { return __cacheDirectory; } set { __cacheDirectory = value; } }
        public static string ImageCacheDirectory { get { return __imageCacheDirectory; } set { __imageCacheDirectory = value; } }
        public static bool DocumentXml { get { return __documentXml; } set { __documentXml = value; } }
        public static bool DocumentMongoDb { get { return __documentMongoDb; } set { __documentMongoDb = value; } }
        public static string MongoServer { get { return __mongoServer; } set { __mongoServer = value; } }
        public static string MongoDatabase { get { return __mongoDatabase; } set { __mongoDatabase = value; } }
        public static string MongoCollectionName { get { return __mongoCollectionName; } set { __mongoCollectionName = value; } }

        public override string GetName()
        {
            return "Gesat header";
        }

        public override object _GetDocumentKey()
        {
            //http://www.reseau-gesat.com/Gesat/
            //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            Match match = __KeyRegex.Match(_url);
            if (!match.Success)
                throw new PB_Util_Exception("key not found in url \"{0}\"", _url);
            string key = match.Groups[1].Value;
            if (key.EndsWith("/"))
                key = key.Substring(0, key.Length - 1);
            if (key == "")
                key = "EtablissementList-0-10.html";
            Trace.CurrentTrace.WriteLine("key \"{0}\"", key);
            return key;
        }

        protected override void _LoadDocumentFromWeb(string file = null, bool reload = false, bool loadImage = false)
        {
            Gesat_LoadHeaderFromWeb loadFromWeb = new Gesat_LoadHeaderFromWeb(_url, file, reload, loadImage);
            loadFromWeb.Load();
            _data = loadFromWeb.Data;
        }

        protected override void _LoadDocumentFromXml(string file, bool loadImage = false)
        {
            //TelechargementPlus_LoadPostFromXml loadFromXml = new TelechargementPlus_LoadPostFromXml(file, loadImage);
            //_data = loadFromXml.Data;
            throw new NotImplementedException();
        }

        protected override void _SaveDocumentToXml(XmlWriter xw, bool saveImage = true)
        {
            //string imageFile = null;
            //string fileDirectory = null;
            //if (saveImage)
            //{
            //    fileDirectory = GetFileDirectory();
            //    imageFile = GetImageFile();
            //}

            //_data.DocumentXmlSave(xw);
            throw new NotImplementedException();
        }

        protected override void _LoadDocumentFromMongo(BsonDocument doc, bool loadImage = false)
        {
            ////Trace.CurrentTrace.WriteLine("_DocumentMongoLoad loadImage {0}", loadImage);
            //_data = new Gesat_Company();
            //_data.DocumentMongoLoad(doc);
            throw new NotImplementedException();
        }

        protected override void _SaveDocumentToMongo(BsonDocument doc, bool saveImage = true)
        {
            //string imageFile = null;
            //string fileDirectory = null;
            //if (saveImage)
            //{
            //    fileDirectory = GetFileDirectory();
            //    imageFile = GetImageFile();
            //}
            //_data.DocumentMongoSave(doc);
            throw new NotImplementedException();
        }
    }

    public class Gesat_LoadHeaderPages : IEnumerable<Gesat_HeaderCompany>, IEnumerator<Gesat_HeaderCompany>
    {
        protected string _url = null;
        protected int _maxPage = 1;
        protected int _nbPage = 1;
        protected bool _reload = false;
        protected bool _loadImage = false;
        protected Gesat_HeaderPage _headerPage = null; 
        protected IEnumerator<Gesat_HeaderCompany> _enumerator = null;
        protected static string _urlHeader = "http://www.reseau-gesat.com/Gesat/";
        protected static int _headersNumberByPage = 10;

        public Gesat_LoadHeaderPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            _url = GetUrlPage(startPage);
            _maxPage = maxPage;
            _reload = reload;
            _loadImage = loadImage;
        }

        public void Dispose()
        {
        }

        protected string GetUrlPage(int page)
        {
            //http://www.reseau-gesat.com/Gesat/EtablissementList-10-10.html
            string url = _urlHeader;
            if (page != 1)
                url += string.Format("EtablissementList-{0}-10.html", (page - 1) * _headersNumberByPage);
            return url;
        }

        public IEnumerator<Gesat_HeaderCompany> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public Gesat_HeaderCompany Current
        {
            get { return _enumerator.Current; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _enumerator.Current; }
        }

        public bool MoveNext()
        {
            while (true)
            {
                if (_enumerator == null)
                {
                    Gesat_LoadHeader load = new Gesat_LoadHeader(_url);
                    load.Load(_reload, _loadImage);
                    _headerPage = load.Data;
                    _enumerator = _headerPage.headerCompanies.AsEnumerable<Gesat_HeaderCompany>().GetEnumerator();
                }
                if (_enumerator.MoveNext())
                    return true;
                if (_nbPage == _maxPage && _maxPage != 0)
                    return false;
                _url = _headerPage.urlNextPage;
                if (_url == null)
                    return false;
                _headerPage = null;
                _enumerator = null;
                _nbPage++;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class Gesat_LoadCompanyFromWeb : LoadFromWebBase_v1
    {
        private Gesat_HeaderCompany _header;
        private bool _loadImage = false;
        private XXElement _xeSource = null;
        private XXElement _xeData = null;
        private Gesat_Company _data = null;
        private static Func<string, string> _trimFunc1 = text => text.Trim();
        private static Func<string, string> _trimFunc2 = text => text.Trim(' ', '>');
        

        public Gesat_LoadCompanyFromWeb(string url, Gesat_HeaderCompany header, string urlFile = null, bool reload = false, bool loadImage = false)
        {
            _header = header;
            _url = url;
            _urlFile = urlFile;
            _reload = reload;
            _loadImage = loadImage;
        }

        public Gesat_Company Data { get { /*Init();*/ return _data; } }

        protected override void SetXml(XElement xelement)
        {
            _xeSource = new XXElement(xelement);
            InitXml();
        }

        protected void InitXml()
        {
            _data = new Gesat_Company();
            _data.url = _url;
            _data.loadFromWebDate = DateTime.Now;

            if (_header != null)
            {
                _data.name = _header.name;
                _data.type = _header.type;
                _data.location = _header.location;
                _data.phone = _header.phone;
                _data.infos = _header.infos;
            }

            // <div class="PAGES" id="content">
            XXElement xe = _xeSource.XPathElement(".//div[@id='content']");

            // <h1><span>ESAT BETTY LAUNAY-MOULIN VERT >></span><br />Coordonnées & activités</h1>
            //string s = xe.XPathValue(".//h1//text()", _trimFunc2);
            string s = _trimFunc2(xe.XPathValue(".//h1//text()"));
            //s = s.Trim(' ', '>');
            if (!s.Equals(_data.name, StringComparison.InvariantCultureIgnoreCase))
            {
                _data.headerName = _data.name;
                _data.name = s;
            }

            // <div class="BLOC B100 ACCROCHE">
            // <div class="CONTENU-BLOC">Cet E.S.A.T. est ouvert depuis 1989 et accueille 55 personnes reconnues travailleurs handicapés.  Il est situé dans la ville de 
            // <a href="/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/" title="Bois-Colombes // Les ESAT et EA de la ville">Bois-Colombes</a> (
            // <a href="/Gesat/Hauts-de-Seine,92/" title="Hauts-de-Seine // Les ESAT et EA du département">Hauts-de-Seine</a>)
            // </div></div>
            _data.descryption = xe.XPathConcatText(".//div[@class='BLOC B100 ACCROCHE']//text()", resultFunc: _trimFunc1);
            _data.descryption = _data.descryption.Replace("\r", "");
            _data.descryption = _data.descryption.Replace("\n", "");
            _data.descryption = _data.descryption.Replace("\t", "");
            //_data.city = xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[1]//text()", _trimFunc1);
            _data.city = _trimFunc1(xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[1]//text()"));
            //_data.department = xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[2]//text()", _trimFunc1);
            _data.department = _trimFunc1(xe.XPathValue(".//div[@class='BLOC B100 ACCROCHE']//a[2]//text()"));

            // <div class="ADRESSE">78, RUE RASPAIL<br />92270  Bois-Colombes</div>
            _data.address = xe.XPathConcatText(".//div[@class='ADRESSE']//text()", " ", itemFunc: _trimFunc1);
            _data.address = _data.address.Replace("\r", "");
            _data.address = _data.address.Replace("\n", "");
            _data.address = _data.address.Replace("\t", "");

            // <div class="TEL">01 47 86 11 48</div>
            //s = xe.XPathValue(".//div[@class='TEL']//text()", _trimFunc1);
            s = _trimFunc1(xe.XPathValue(".//div[@class='TEL']//text()"));
            if (!s.Equals(_data.phone, StringComparison.InvariantCultureIgnoreCase))
            {
                _data.headerPhone = _data.phone;
                _data.phone = s;
            }

            // <div class="FAX">01 47 82 42 64</div>
            //_data.fax = xe.XPathValue(".//div[@class='FAX']//text()", _trimFunc1);
            _data.fax = _trimFunc1(xe.XPathValue(".//div[@class='FAX']//text()"));

            // <div class="EMAIL">production.launay<img border="0" alt="arobase.png" src="/images/bulles/arobase.png" style=" border: 0;" />lemoulinvert.org</div>
            _data.email = xe.XPathConcatText(".//div[@class='EMAIL']//text()", "@", itemFunc: _trimFunc1);

            // <div class="WWW"><a href="http://www.esat-b-launay.com" target="_blank">www.esat-b-launay.com</a></div>
            //_data.webSite = xe.XPathValue(".//div[@class='WWW']//a/@href", _trimFunc1);
            _data.webSite = _trimFunc1(xe.XPathValue(".//div[@class='WWW']//a/@href"));

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
            //_data.activities = xe.XPathValues(".//div[@class='BLOC-FICHE BLOC-ACTIVITES']//dl//text()", _trimFunc1);
            _data.activities = xe.XPathValues(".//div[@class='BLOC-FICHE BLOC-ACTIVITES']//dl//text()").Select(_trimFunc1).ToArray();
        }

        protected static void WriteLine(string msg, params object[] prm)
        {
            Trace.CurrentTrace.WriteLine(msg, prm);
        }

        public static Gesat_Company LoadCompany(string url, Gesat_HeaderCompany header, string urlFile = null, bool reload = false, bool loadImage = false)
        {
            Gesat_LoadCompanyFromWeb load = new Gesat_LoadCompanyFromWeb(url, header, urlFile, reload, loadImage);
            load.Load();
            return load.Data;
        }
    }

    public class Gesat_LoadCompany : HttpLoad, IDisposable
    {
        protected Gesat_HeaderCompany _header = null;
        protected Gesat_Company _data = null;
        //protected IEnumerable<TelechargementPlus_Print> _prints = null;

        protected static Regex __KeyRegex = new Regex("/Gesat/(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        protected static bool __useUrlCache = false;
        protected static string __cacheDirectory = null;
        protected static string __imageCacheDirectory = "image";
        protected static bool __documentXml = false;
        protected static bool __documentMongoDb = false;
        protected static string __mongoServer = null;
        protected static string __mongoDatabase = null;
        protected static string __mongoCollectionName = null;

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");
            __imageCacheDirectory = xe.zXPathValue("ImageCacheDirectory", __imageCacheDirectory);
            //__documentXml = xe.zXPathValueBool("DocumentXml", __documentXml);
            __documentXml = xe.zXPathValue("DocumentXml").zTryParseAs(__documentXml);
            //__documentMongoDb = xe.zXPathValueBool("DocumentMongoDb", __documentMongoDb);
            __documentMongoDb = xe.zXPathValue("DocumentMongoDb").zTryParseAs(__documentMongoDb);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
        }

        public Gesat_LoadCompany(string url, Gesat_HeaderCompany header)
            : base(url)
        {
            _header = header;
            //_postIdRegex = __postIdRegex;
            _useUrlCache = __useUrlCache;
            _cacheDirectory = __cacheDirectory;
            _imageCacheDirectory = __imageCacheDirectory;
            _documentXml = __documentXml;
            _documentMongoDb = __documentMongoDb;
            _mongoServer = __mongoServer;
            _mongoDatabase = __mongoDatabase;
            _mongoCollectionName = __mongoCollectionName;
        }

        public void Dispose()
        {
        }

        public Gesat_Company Data { get { return _data; } }

        public static bool UseUrlCache { get { return __useUrlCache; } set { __useUrlCache = value; } }
        public static string CacheDirectory { get { return __cacheDirectory; } set { __cacheDirectory = value; } }
        public static string ImageCacheDirectory { get { return __imageCacheDirectory; } set { __imageCacheDirectory = value; } }
        public static bool DocumentXml { get { return __documentXml; } set { __documentXml = value; } }
        public static bool DocumentMongoDb { get { return __documentMongoDb; } set { __documentMongoDb = value; } }
        public static string MongoServer { get { return __mongoServer; } set { __mongoServer = value; } }
        public static string MongoDatabase { get { return __mongoDatabase; } set { __mongoDatabase = value; } }
        public static string MongoCollectionName { get { return __mongoCollectionName; } set { __mongoCollectionName = value; } }

        public override string GetName()
        {
            return "Gesat company";
        }

        public override object _GetDocumentKey()
        {
            // http://www.reseau-gesat.com/Gesat/Hauts-de-Seine,92/Bois-Colombes,35494/esat-betty-launay-moulin-vert-competences-et-handicap-92,e1837/
            Match match = __KeyRegex.Match(_url);
            if (!match.Success)
                throw new PB_Util_Exception("key not found in url \"{0}\"", _url);
            string key = match.Groups[1].Value;
            if (key.EndsWith("/"))
                key = key.Substring(0, key.Length - 1);
            Trace.CurrentTrace.WriteLine("key \"{0}\"", key);
            return key;
        }

        protected override void _LoadDocumentFromWeb(string file = null, bool reload = false, bool loadImage = false)
        {
            //TelechargementPlus_LoadPostFromWeb loadFromWeb = new TelechargementPlus_LoadPostFromWeb(_url, file, reload, loadImage);
            Gesat_LoadCompanyFromWeb loadFromWeb = new Gesat_LoadCompanyFromWeb(_url, _header, file, reload, loadImage);
            loadFromWeb.Load();
            _data = loadFromWeb.Data;
        }

        protected override void _LoadDocumentFromXml(string file, bool loadImage = false)
        {
            //TelechargementPlus_LoadPostFromXml loadFromXml = new TelechargementPlus_LoadPostFromXml(file, loadImage);
            //_data = loadFromXml.Data;
            throw new NotImplementedException();
        }

        protected override void _SaveDocumentToXml(XmlWriter xw, bool saveImage = true)
        {
            //string imageFile = null;
            //string fileDirectory = null;
            //if (saveImage)
            //{
            //    fileDirectory = GetFileDirectory();
            //    imageFile = GetImageFile();
            //}

            //_data.DocumentXmlSave(xw);
            throw new NotImplementedException();
        }

        protected override void _LoadDocumentFromMongo(BsonDocument doc, bool loadImage = false)
        {
            //Trace.CurrentTrace.WriteLine("_DocumentMongoLoad loadImage {0}", loadImage);
            _data = new Gesat_Company();
            _data.DocumentMongoLoad(doc);
        }

        protected override void _SaveDocumentToMongo(BsonDocument doc, bool saveImage = true)
        {
            string imageFile = null;
            string fileDirectory = null;
            if (saveImage)
            {
                fileDirectory = GetCacheFileDirectory();
                imageFile = GetImageFile();
            }
            _data.DocumentMongoSave(doc);
        }

        public static Gesat_Company LoadCompany(string url, Gesat_HeaderCompany header, bool reload, bool loadImage)
        {
            Gesat_LoadCompany load = new Gesat_LoadCompany(url, header);
            load.Load(reload, loadImage);
            return load.Data;
        }
    }
}
