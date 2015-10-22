using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;
using PB_Util;

namespace Download.Handeco
{
    public class Handeco_HeaderCompany
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate = null;
        public string urlDetail = null;
        public string name = null;
        public string siret = null;
        public string type = null;
        public string[] groupes = null;
        public string[] activités = null;
        public string postalCode = null;
    }

    public class Handeco_HeaderPage
    {
        public Handeco_HeaderCompany[] headerCompanies;
        public string urlNextPage;
    }

    public class Handeco_LoadHeaderFromWeb : LoadDataFromWeb_v1<Handeco_HeaderPage>
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

        public Handeco_LoadHeaderFromWeb(string url, HttpRequestParameters_v1 requestParameters = null, bool reload = false, bool loadImage = false)
            : base(url, requestParameters, reload)
        {
            //SetRequestParameters(new HttpRequestParameters() { encoding = Encoding.UTF8 });
            if (__useUrlCache)
                SetUrlCache(new UrlCache_v1(__cacheDirectory, __urlFileNameType));
            _loadImage = loadImage;
        }

        protected override Handeco_HeaderPage GetData()
        {
            XXElement xeSource = new XXElement(GetXmlDocument().Root);
            string url = Url;
            Handeco_HeaderPage data = new Handeco_HeaderPage();

            // <div class="paginationControl">
            // page n    : <a href="/fournisseurs/rechercher/page/2#resultats">&gt;</a> |
            // last page : <span class="disabled">&gt;</span> |
            data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='paginationControl']//*[position()=last()-1]/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//table//tr[position() > 1]");
            List<Handeco_HeaderCompany> headers = new List<Handeco_HeaderCompany>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Handeco_HeaderCompany header = new Handeco_HeaderCompany();
                header.sourceUrl = url;
                header.loadFromWebDate = DateTime.Now;
                //header.name = xeHeader.XPathValue(".//td[1]//text()", _trimFunc1);
                header.name = _trimFunc1(xeHeader.XPathValue(".//td[1]//text()"));
                header.urlDetail = zurl.GetUrl(url, xeHeader.XPathValue(".//td[1]//a/@href"));
                //header.siret = xeHeader.XPathValue(".//td[2]//text()", _trimFunc1);
                header.siret = _trimFunc1(xeHeader.XPathValue(".//td[2]//text()"));
                //header.type = xeHeader.XPathValue(".//td[3]//text()", _trimFunc1);
                header.type = _trimFunc1(xeHeader.XPathValue(".//td[3]//text()"));
                //header.group = xeHeader.XPathValue(".//td[4]//text()", _trimFunc1);
                //header.groupes = xeHeader.XPathValues(".//td[4]//text()", _trimFunc1);
                header.groupes = xeHeader.XPathValues(".//td[4]//text()").Select(_trimFunc1).ToArray();
                //header.sector = xeHeader.XPathValue(".//td[5]//text()");
                //header.activités = xeHeader.XPathValues(".//td[5]//text()", _trimFunc1);
                header.activités = xeHeader.XPathValues(".//td[5]//text()").Select(_trimFunc1).ToArray();
                //header.postalCode = xeHeader.XPathValue(".//td[6]//text()", _trimFunc1);
                header.postalCode = _trimFunc1(xeHeader.XPathValue(".//td[6]//text()"));
                headers.Add(header);
            }
            data.headerCompanies = headers.ToArray();
            return data;
        }
    }

    public class Handeco_LoadHeader : LoadWebData_v1<Handeco_HeaderPage>
    {
        // http://www.handeco.org/fournisseurs/rechercher
        // http://www.handeco.org/fournisseurs/rechercher/page/2
        protected static Regex __KeyRegex = new Regex("http://www.handeco.org/fournisseurs/rechercher(?:/page/([0-9]+))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
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

        public Handeco_LoadHeader(string url, HttpRequestParameters_v1 requestParameters = null)
            : base(url, requestParameters)
        {
            //_imageCacheDirectory = __imageCacheDirectory;
            SetXmlParameters(__useXml);
            SetMongoParameters(__useMongo, __mongoServer, __mongoDatabase, __mongoCollectionName);
        }

        protected override string GetName()
        {
            return "Handeco header";
        }

        protected override Handeco_HeaderPage LoadDocumentFromWeb(bool reload = false, bool loadImage = false)
        {
            Handeco_LoadHeaderFromWeb loadFromWeb = new Handeco_LoadHeaderFromWeb(Url, RequestParameters, reload, loadImage);
            loadFromWeb.Load();
            return loadFromWeb.Data;
        }

        protected override object GetDocumentKey()
        {
            // http://www.handeco.org/fournisseurs/rechercher
            // http://www.handeco.org/fournisseurs/rechercher/page/2
            // key = handeco_fournisseurs_page_n
            Match match = __KeyRegex.Match(Url);
            if (!match.Success)
                throw new PB_Util_Exception("key not found in url \"{0}\"", Url);
            string key = match.Groups[1].Value;
            if (key == "")
                key = "1";
            key = "handeco_fournisseurs_page_" + key;
            //Trace.CurrentTrace.WriteLine("key \"{0}\"", key);
            return key;
        }
    }

    //public class Unea_LoadHeaderPages : LoadWebDataPages<Unea_HeaderCompany>
    public class Handeco_LoadHeaderPages : LoadWebDataPages_v1<Handeco_HeaderCompany>
    {
        private IEnumerator<Handeco_HeaderCompany> _enumerator = null;
        private int _page;
        protected Handeco_HeaderPage _headerPage = null;
        //private HttpRequestParameters _requestParameters = null;
        private static string __url = "http://www.handeco.org/fournisseurs/rechercher";
        // selection de tous les départements
        private static string __content = "raisonSociale=&SIRET=&departements%5B%5D=67&departements%5B%5D=68&departements%5B%5D=24&departements%5B%5D=33&departements%5B%5D=40&departements%5B%5D=47&departements%5B%5D=64&departements%5B%5D=03&departements%5B%5D=15&departements%5B%5D=43&departements%5B%5D=63&departements%5B%5D=14&departements%5B%5D=50&departements%5B%5D=61&departements%5B%5D=21&departements%5B%5D=58&departements%5B%5D=71&departements%5B%5D=89&departements%5B%5D=22&departements%5B%5D=29&departements%5B%5D=35&departements%5B%5D=56&departements%5B%5D=18&departements%5B%5D=28&departements%5B%5D=36&departements%5B%5D=37&departements%5B%5D=41&departements%5B%5D=45&departements%5B%5D=08&departements%5B%5D=10&departements%5B%5D=51&departements%5B%5D=52&departements%5B%5D=2A&departements%5B%5D=2B&departements%5B%5D=25&departements%5B%5D=39&departements%5B%5D=70&departements%5B%5D=90&departements%5B%5D=27&departements%5B%5D=76&departements%5B%5D=75&departements%5B%5D=77&departements%5B%5D=78&departements%5B%5D=91&departements%5B%5D=92&departements%5B%5D=93&departements%5B%5D=94&departements%5B%5D=95&departements%5B%5D=11&departements%5B%5D=30&departements%5B%5D=34&departements%5B%5D=48&departements%5B%5D=66&departements%5B%5D=19&departements%5B%5D=23&departements%5B%5D=87&departements%5B%5D=54&departements%5B%5D=55&departements%5B%5D=57&departements%5B%5D=88&departements%5B%5D=09&departements%5B%5D=12&departements%5B%5D=31&departements%5B%5D=32&departements%5B%5D=46&departements%5B%5D=65&departements%5B%5D=81&departements%5B%5D=82&departements%5B%5D=59&departements%5B%5D=62&departements%5B%5D=44&departements%5B%5D=49&departements%5B%5D=53&departements%5B%5D=72&departements%5B%5D=85&departements%5B%5D=02&departements%5B%5D=60&departements%5B%5D=80&departements%5B%5D=16&departements%5B%5D=17&departements%5B%5D=79&departements%5B%5D=86&departements%5B%5D=04&departements%5B%5D=05&departements%5B%5D=06&departements%5B%5D=13&departements%5B%5D=83&departements%5B%5D=84&departements%5B%5D=01&departements%5B%5D=07&departements%5B%5D=26&departements%5B%5D=38&departements%5B%5D=42&departements%5B%5D=69&departements%5B%5D=73&departements%5B%5D=74&departements%5B%5D=971&departements%5B%5D=973&departements%5B%5D=972&departements%5B%5D=974&departements%5B%5D=988&departements%5B%5D=987&departements%5B%5D=975&departements%5B%5D=976&departements%5B%5D=986&experience_cotraitance=0&motsCles=&submitRecherche=Rechercher";

        public Handeco_LoadHeaderPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
            : base(startPage, maxPage, reload, loadImage)
        {
        }

        protected override void GetUrlPage(int page, out string url, out HttpRequestParameters_v1 requestParameters)
        {
            if (page != 1)
                throw new PB_Util_Exception("error impossible to load directly page n, only page 1 must be loaded first");
            _page = page;
            url = __url;
            requestParameters = new HttpRequestParameters_v1();
            requestParameters.method = HttpRequestMethod.Post;
            requestParameters.content = __content;
            requestParameters.encoding = Encoding.UTF8;
            //_requestParameters = requestParameters;
        }

        protected override void Load()
        {
            //Gesat_LoadHeader2 load = new Gesat_LoadHeader2(Url);
            Handeco_LoadHeader load = new Handeco_LoadHeader(Url, RequestParameters);
            load.Load(Reload, LoadImage);
            _headerPage = load.Data;
            if (_headerPage != null)
                _enumerator = _headerPage.headerCompanies.AsEnumerable<Handeco_HeaderCompany>().GetEnumerator();
        }

        protected override bool GetNextItem()
        {
            if (_enumerator == null)
                return false;
            return _enumerator.MoveNext();
        }

        protected override Handeco_HeaderCompany GetCurrentItem()
        {
            return _enumerator.Current;
        }

        protected override bool GetUrlNextPage(out string url, out HttpRequestParameters_v1 requestParameters)
        {
            if (_headerPage != null)
                url = _headerPage.urlNextPage;
            else
                url = null;
            //requestParameters = _requestParameters;
            requestParameters = RequestParameters;
            requestParameters.method = HttpRequestMethod.Get;
            requestParameters.content = null;
            if (url != null)
                return true;
            else
                return false;
        }
    }
}
