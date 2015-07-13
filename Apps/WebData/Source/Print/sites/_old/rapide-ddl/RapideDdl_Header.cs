using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;
using Print;

namespace Download.Print.RapideDdl
{
    //public class RapideDdl_PostHeader_old : IRapideDdl_Base
    //{
    //    public RapideDdl_PostHeader_old()
    //    {
    //        description = new List<string>();
    //        infos = new NamedValues<ZValue>();
    //    }

    //    public string sourceUrl { get; set; }
    //    public DateTime? loadFromWebDate { get; set; }
    //    public string urlDetail { get; set; }

    //    public string title { get; set; }
    //    public string postAuthor { get; set; }
    //    public DateTime? creationDate { get; set; }
    //    public string category { get; set; }
    //    public List<string> description { get; set; }
    //    public string language { get; set; }
    //    public string size { get; set; }
    //    public int? nbPages { get; set; }
    //    public NamedValues<ZValue> infos { get; set; }
    //    //public List<ImageHtml> images = new List<ImageHtml>();
    //    public List<UrlImage> images { get; set; }
    //}

    public class RapideDdl_PostHeader : IHeaderData_v1
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate;
        public string urlDetail;

        public string title;
        public string postAuthor;
        public DateTime? creationDate;
        public string category;
        public string[] description;
        public string language;
        public string size;
        public int? nbPages;
        public NamedValues<ZValue> infos = new NamedValues<ZValue>();
        //public List<UrlImage> images = new List<UrlImage>();
        public WebImage[] images;

        public string GetUrlDetail()
        {
            return urlDetail;
        }
    }

    public class RapideDdl_HeaderPage : IEnumDataPages_v1<int, RapideDdl_PostHeader>
    {
        public int id;
        public string sourceUrl;
        public DateTime loadFromWebDate;

        public RapideDdl_PostHeader[] postHeaders;
        public string urlNextPage;

        public int GetKey()
        {
            return id;
        }

        public IEnumerable<RapideDdl_PostHeader> GetDataList()
        {
            return postHeaders;
        }

        public string GetUrlNextPage()
        {
            return urlNextPage;
        }
    }

    public class RapideDdl_LoadHeaderPageFromWebManager : LoadDataFromWebManager_v3<RapideDdl_HeaderPage>
    {
        private static bool __trace = false;

        public RapideDdl_LoadHeaderPageFromWebManager(UrlCache_v1 urlCache = null)
            : base(urlCache)
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        protected override RapideDdl_HeaderPage GetDataFromWeb(LoadDataFromWeb_v3 loadDataFromWeb)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
            string url = loadDataFromWeb.request.Url;
            RapideDdl_HeaderPage data = new RapideDdl_HeaderPage();
            data.sourceUrl = url;
            data.loadFromWebDate = loadDataFromWeb.loadFromWebDate;
            data.id = RapideDdl_LoadHeaderPagesManager.GetHeaderPageKey(url);

            data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='basenavi']//span[@class='nnext']//a/@href"));
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@class='base shortstory']");
            List<RapideDdl_PostHeader> headers = new List<RapideDdl_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                RapideDdl_PostHeader header = new RapideDdl_PostHeader();
                header.sourceUrl = url;
                header.loadFromWebDate = loadDataFromWeb.loadFromWebDate;

                XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
                header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));

                //header.title = RapideDdl.ExtractTextValues(header.infos, xe.XPathValue(".//text()", RapideDdl.TrimFunc1));
                //header.title = xe.XPathValue(".//text()", DownloadPrint.Trim);
                header.title = xe.XPathValue(".//text()").Trim(DownloadPrint.TrimChars);
                PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(header.title);
                if (titleInfos.foundInfo)
                {
                    //header.originalTitle = header.title;
                    header.title = titleInfos.title;
                    header.infos.SetValues(titleInfos.infos);
                }

                xe = xeHeader.XPathElement(".//div[@class='shdinfo']");
                header.postAuthor = xe.XPathValue(".//span[@class='arg']//a//text()");
                // Aujourd'hui, 17:13
                //header.creationDate = RapideDdl.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"), loadDataFromWeb.loadFromWebDate);
                string date = xe.XPathValue(".//span[@class='date']//text()");
                header.creationDate = zdate.ParseDateTimeLikeToday(date, loadDataFromWeb.loadFromWebDate, "d-M-yyyy, HH:mm", "d M yyyy", "d MMMM yyyy");
                if (header.creationDate == null)
                    pb.Trace.WriteLine("unknow date time \"{0}\"", date);
                if (__trace)
                    pb.Trace.WriteLine("creationDate {0} - \"{1}\"", header.creationDate, date);

                xe = xeHeader.XPathElement(".//div[@class='maincont']");
                //header.images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToArray();
                header.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToArray();

                //if (request.LoadImage)
                //    Http2.LoadImageFromWeb(header.images);

                //RapideDdl.SetTextValues(header, xe.DescendantTextList());
                // get infos, description, language, size, nbPages
                // xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a")
                PrintTextValues_v1 textValues = DownloadPrint.PrintTextValuesManager.GetTextValues_v1(xe.DescendantTexts(node => !(node is XElement) || ((XElement)node).Name != "a" ? XNodeFilter.SelectNode : XNodeFilter.SkipNode), header.title);
                header.description = textValues.description;
                header.language = textValues.language;
                header.size = textValues.size;
                header.nbPages = textValues.nbPages;
                header.infos.SetValues(textValues.infos);

                xe = xeHeader.XPathElement(".//div[@class='morelink']//span[@class='arg']");
                //header.category = xe.DescendantTextList(".//a").Select(DownloadPrint.TrimFunc1).Where(s => !s.StartsWith("Commentaires")).zToStringValues("/");
                header.category = xe.XPathElements(".//a").DescendantTexts().Select(DownloadPrint.Trim).Where(s => !s.StartsWith("Commentaires")).zToStringValues("/");

                headers.Add(header);
            }
            data.postHeaders = headers.ToArray();
            return data;
        }
    }

    public class RapideDdl_LoadHeaderPagesManager : LoadWebEnumDataPagesManager_v3<int, RapideDdl_HeaderPage, RapideDdl_PostHeader>
    {
        private static string __url = "http://www.rapide-ddl.com/ebooks/";
        private static RapideDdl_LoadHeaderPagesManager __currentLoadHeaderPagesManager = null;

        private static bool __trace = false;

        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        static RapideDdl_LoadHeaderPagesManager()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("RapideDdl/Header"));
        }

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");

            //__useMongo = xe.zXPathValueBool("UseMongo", __useMongo);
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
            __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

            IDocumentStore_v3<int, RapideDdl_HeaderPage> documentStore = null;
            if (__useMongo)
            {
                documentStore = new MongoDocumentStore_v3<int, RapideDdl_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                documentStore.DefaultSort = "{ 'download.id': 1 }";
                documentStore.GetDataKey = headerPage => headerPage.id;
            }

            __currentLoadHeaderPagesManager = new RapideDdl_LoadHeaderPagesManager(new RapideDdl_LoadHeaderPageFromWebManager(GetUrlCache()), documentStore);

        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType);
            return urlCache;
        }

        public RapideDdl_LoadHeaderPagesManager(LoadDataFromWebManager_v3<RapideDdl_HeaderPage> loadDataFromWeb, IDocumentStore_v3<int, RapideDdl_HeaderPage> documentStore = null)
            : base(loadDataFromWeb, documentStore)
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static RapideDdl_LoadHeaderPagesManager CurrentLoadHeaderPagesManager { get { return __currentLoadHeaderPagesManager; } }

        protected override string GetUrlPage(int page)
        {
            // http://www.rapide-ddl.com/ebooks/page/2/
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            string url = __url;
            if (page > 1)
                url += string.Format("page/{0}/", page);
            return url;
        }

        protected override HttpRequestParameters_v1 GetHttpRequestParameters()
        {
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.encoding = Encoding.UTF8;
            requestParameters.cookies.Add(new Uri(__url), new Cookie("hasVisitedSite", "Yes"));
            return requestParameters;
        }

        protected override int GetKeyFromUrl(string url)
        {
            return GetHeaderPageKey(url);
        }

        public static int GetHeaderPageKey(string url)
        {
            // page 1 : http://www.rapide-ddl.com/ebooks/
            // page 2 : http://www.rapide-ddl.com/ebooks/page/2/
            if (url == __url)
                return 1;
            Uri uri = new Uri(url);
            string lastSegment = uri.Segments[uri.Segments.Length - 1];
            lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
            int page;
            if (!int.TryParse(lastSegment, out page))
                throw new PBException("header page key not found in url \"{0}\"", url);
            return page;
        }
    }

    public static class RapideDdl_LoadHeaderPage
    {
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        private static LoadWebDataManager_v3<RapideDdl_HeaderPage> _load;

        static RapideDdl_LoadHeaderPage()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("RapideDdl/Header"));
        }

        public static LoadWebDataManager_v3<RapideDdl_HeaderPage> Load { get { return _load; } }

        public static void ClassInit(XElement xe)
        {
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");

            //__useMongo = xe.zXPathValueBool("UseMongo", __useMongo);
            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
            __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

            IDocumentStore_v2<RapideDdl_HeaderPage> documentStore = null;
            if (__useMongo)
            {
                documentStore = new MongoDocumentStore_v2<RapideDdl_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                //RapideDdl.InitMongoClassMap();
            }

            //_load = new LoadWebDataManager<RapideDdl_HeaderPage>(new LoadDataFromWeb<RapideDdl_HeaderPage>(LoadHeaderPageFromWeb, GetUrlCache()), documentStore);
            _load = new LoadWebDataManager_v3<RapideDdl_HeaderPage>(new RapideDdl_LoadHeaderPageFromWebManager(GetUrlCache()), documentStore);
        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType);
            return urlCache;
        }
    }

    public class RapideDdl_LoadHeaderPages : LoadWebEnumDataPages_v2<RapideDdl_PostHeader> // pb.Web.v1.ILoadWebEnumDataPages<RapideDdl_PostHeader>
    {
        private static string __url = "http://www.rapide-ddl.com/ebooks/";
        //private static RapideDdl_LoadHeaderPageFromWebManager _load = new RapideDdl_LoadHeaderPageFromWebManager();
        private RapideDdl_HeaderPage _headerPage = null;
        private HttpRequestParameters_v1 _requestParameters = null;

        public RapideDdl_LoadHeaderPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
            : base(startPage, maxPage, reload, loadImage)
        {
        }

        protected override IEnumerator<RapideDdl_PostHeader> LoadPage(int page, bool reload, bool loadImage)
        {
            // http://www.rapide-ddl.com/ebooks/page/2/
            string url = __url;
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            if (page > 1)
                url += string.Format("page/{0}/", page);
            _requestParameters = new HttpRequestParameters_v1();
            _requestParameters.encoding = Encoding.UTF8;
            _requestParameters.cookies.Add(new Uri(url), new Cookie("hasVisitedSite", "Yes"));
            return _Load(url, reload, loadImage);
        }

        protected override IEnumerator<RapideDdl_PostHeader> LoadNextPage(bool reload, bool loadImage)
        {
            if (_headerPage != null)
                return _Load(_headerPage.urlNextPage, reload, loadImage);
            else
                return null;
        }

        private IEnumerator<RapideDdl_PostHeader> _Load(string url, bool reload, bool loadImage)
        {
            if (url != null)
            {
                // dont use mongo to store header page so key is null and refreshDocumentStore is false
                RequestWebData_v3 request = new RequestWebData_v3(new RequestFromWeb_v3(url, _requestParameters, reload, loadImage));
                _headerPage = RapideDdl_LoadHeaderPage.Load.Load(request).Document;
                if (loadImage)
                {
                    foreach (var postHeader in _headerPage.postHeaders)
                        postHeader.images = DownloadPrint.LoadImages(postHeader.images).ToArray();
                }
                if (_headerPage != null)
                    return _headerPage.postHeaders.AsEnumerable<RapideDdl_PostHeader>().GetEnumerator();
            }
            return null;
        }

        public static IEnumerable<RapideDdl_PostHeader> Load(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
        {
            return new RapideDdl_LoadHeaderPages(startPage, maxPage, reload, loadImage);
        }
    }
}
