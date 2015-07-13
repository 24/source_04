using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MongoDB.Bson.Serialization;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;

namespace Download.Print.GoldenDdl
{
    public class GoldenDdl_PostHeader : IHeaderData_v1
    {
        public string sourceUrl;
        public DateTime? loadFromWebDate;
        public string urlDetail;

        public List<WebImage> images;

        public string GetUrlDetail()
        {
            return urlDetail;
        }
    }

    public class GoldenDdl_HeaderPage : IEnumDataPages_v1<int, GoldenDdl_PostHeader>
    {
        public int id;
        public string sourceUrl;
        public DateTime loadFromWebDate;

        public GoldenDdl_PostHeader[] postHeaders;
        public string urlNextPage;

        public int GetKey()
        {
            return id;
        }

        public IEnumerable<GoldenDdl_PostHeader> GetDataList()
        {
            return postHeaders;
        }

        public string GetUrlNextPage()
        {
            return urlNextPage;
        }
    }

    //public class GoldenDdl_LoadHeaderPageFromWebManager : LoadDataFromWebManager<GoldenDdl_HeaderPage>
    public class GoldenDdl_LoadHeaderPageFromWebManager : LoadDataFromWebManager_v3<IEnumDataPages_v1<int, IHeaderData_v1>>
    {
        public GoldenDdl_LoadHeaderPageFromWebManager(UrlCache_v1 urlCache = null)
            : base(urlCache)
        {
        }

        //protected override GoldenDdl_HeaderPage GetDataFromWeb(LoadDataFromWeb loadDataFromWeb)
        protected override IEnumDataPages_v1<int, IHeaderData_v1> GetDataFromWeb(LoadDataFromWeb_v3 loadDataFromWeb)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
            string url = loadDataFromWeb.request.Url;
            GoldenDdl_HeaderPage data = new GoldenDdl_HeaderPage();
            data.sourceUrl = url;
            data.loadFromWebDate = loadDataFromWeb.loadFromWebDate;
            data.id = GoldenDdl_LoadHeaderPagesManager.GetHeaderPageKey(url);

            data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='basenavi']//span[@class='nnext']//a/@href"));
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='dle-content']//div[@class='base']");
            List<GoldenDdl_PostHeader> headers = new List<GoldenDdl_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                GoldenDdl_PostHeader header = new GoldenDdl_PostHeader();
                header.sourceUrl = url;
                header.loadFromWebDate = loadDataFromWeb.loadFromWebDate;

                header.urlDetail = xeHeader.XPathValue(".//div[@class='bheading']//a/@href");

                //header.images = xeHeader.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

                //XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
                //header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));
                //header.title = RapideDdl.ExtractTextValues(header.infos, xe.XPathValue(".//text()", RapideDdl.TrimFunc1));

                //xe = xeHeader.XPathElement(".//div[@class='shdinfo']");
                //header.postAuthor = xe.XPathValue(".//span[@class='arg']//a//text()");
                //// Aujourd'hui, 17:13
                //header.creationDate = RapideDdl.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"), loadDataFromWeb.loadFromWebDate);

                //xe = xeHeader.XPathElement(".//div[@class='maincont']");
                //header.images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

                //RapideDdl.SetTextValues(header, xe.DescendantTextList());

                //xe = xeHeader.XPathElement(".//div[@class='morelink']//span[@class='arg']");
                //header.category = xe.DescendantTextList(".//a").Select(RapideDdl.TrimFunc1).Where(s => !s.StartsWith("Commentaires")).zToStringValues("/");

                headers.Add(header);
            }
            data.postHeaders = headers.ToArray();
            return (IEnumDataPages_v1<int, IHeaderData_v1>)data;
        }
    }

    //public class GoldenDdl_LoadHeaderPagesManager : LoadWebEnumDataPagesManager<int, GoldenDdl_HeaderPage, GoldenDdl_PostHeader>
    public class GoldenDdl_LoadHeaderPagesManager : LoadWebEnumDataPagesManager_v3<int, IEnumDataPages_v1<int, IHeaderData_v1>, IHeaderData_v1>
    {
        private static string __url = "http://www.golden-ddl.net/ebooks/";
        private static GoldenDdl_LoadHeaderPagesManager __currentLoadHeaderPagesManager = null;

        private static bool __trace = false;

        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        static GoldenDdl_LoadHeaderPagesManager()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("GoldenDdl/Header"));
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

            //IDocumentStore_new<int, GoldenDdl_HeaderPage> documentStore = null;
            IDocumentStore_v3<int, IEnumDataPages_v1<int, IHeaderData_v1>> documentStore = null;
            if (__useMongo)
            {
                //documentStore = new MongoDocumentStore_new<int, GoldenDdl_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                MongoDocumentStore_v3<int, IEnumDataPages_v1<int, IHeaderData_v1>>  mongoDocumentStore = new MongoDocumentStore_v3<int, IEnumDataPages_v1<int, IHeaderData_v1>>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                mongoDocumentStore.DefaultSort = "{ 'download.id': 1 }";
                //mongoDocumentStore.GetDataKey = headerPage => headerPage.id;
                mongoDocumentStore.GetDataKey = headerPage => headerPage.GetKey();
                mongoDocumentStore.Deserialize = document => (IEnumDataPages_v1<int, IHeaderData_v1>)BsonSerializer.Deserialize<GoldenDdl_HeaderPage>(document);
                documentStore = mongoDocumentStore;
            }

            //__currentLoadHeaderPagesManager = new GoldenDdl_LoadHeaderPagesManager(new GoldenDdl_LoadHeaderPageFromWebManager(GetUrlCache()), documentStore);
            __currentLoadHeaderPagesManager = new GoldenDdl_LoadHeaderPagesManager(new GoldenDdl_LoadHeaderPageFromWebManager(GetUrlCache()), documentStore);

        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType);
            return urlCache;
        }

        //public GoldenDdl_LoadHeaderPagesManager(LoadDataFromWebManager<GoldenDdl_HeaderPage> loadDataFromWeb, IDocumentStore_new<int, GoldenDdl_HeaderPage> documentStore = null)
        public GoldenDdl_LoadHeaderPagesManager(LoadDataFromWebManager_v3<IEnumDataPages_v1<int, IHeaderData_v1>> loadDataFromWeb, IDocumentStore_v3<int, IEnumDataPages_v1<int, IHeaderData_v1>> documentStore = null)
            : base(loadDataFromWeb, documentStore)
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static GoldenDdl_LoadHeaderPagesManager CurrentLoadHeaderPagesManager { get { return __currentLoadHeaderPagesManager; } }

        protected override string GetUrlPage(int page)
        {
            // http://www.golden-ddl.net/ebooks/page/2/
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
            requestParameters.encoding = Encoding.Default;
            return requestParameters;
        }

        protected override int GetKeyFromUrl(string url)
        {
            return GetHeaderPageKey(url);
        }

        public static int GetHeaderPageKey(string url)
        {
            // page 1 : http://www.golden-ddl.net/ebooks/
            // page 2 : http://www.golden-ddl.net/ebooks/page/2/
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

    //public static class GoldenDdl_LoadHeaderPage
    //{
    //    private static bool __useUrlCache = false;
    //    private static string __cacheDirectory = null;
    //    private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

    //    private static bool __useMongo = false;
    //    private static string __mongoServer = null;
    //    private static string __mongoDatabase = null;
    //    private static string __mongoCollectionName = null;
    //    private static string __mongoDocumentItemName = null;

    //    private static LoadWebDataManager<GoldenDdl_HeaderPage> _load;

    //    static GoldenDdl_LoadHeaderPage()
    //    {
    //        ClassInit(XmlConfig.CurrentConfig.GetElement("GoldenDdl/Header"));
    //    }

    //    public static LoadWebDataManager<GoldenDdl_HeaderPage> Load { get { return _load; } }

    //    public static void ClassInit(XElement xe)
    //    {
    //        __useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
    //        __cacheDirectory = xe.zXPathValue("CacheDirectory");

    //        __useMongo = xe.zXPathValueBool("UseMongo", __useMongo);
    //        __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
    //        __mongoDatabase = xe.zXPathValue("MongoDatabase");
    //        __mongoCollectionName = xe.zXPathValue("MongoCollection");
    //        __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

    //        IDocumentStore<GoldenDdl_HeaderPage> documentStore = null;
    //        if (__useMongo)
    //            documentStore = new MongoDocumentStore<GoldenDdl_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);

    //        _load = new LoadWebDataManager<GoldenDdl_HeaderPage>(new GoldenDdl_LoadHeaderPageFromWebManager(GetUrlCache()), documentStore);
    //    }

    //    public static UrlCache GetUrlCache()
    //    {
    //        UrlCache urlCache = null;
    //        if (__useUrlCache)
    //            urlCache = new UrlCache(__cacheDirectory, __urlFileNameType);
    //        return urlCache;
    //    }
    //}

    //public class GoldenDdl_LoadHeaderPages : LoadWebEnumDataPages<GoldenDdl_PostHeader>
    //{
    //    private static string __url = "http://www.golden-ddl.net/ebooks/";
    //    private GoldenDdl_HeaderPage _headerPage = null;
    //    private HttpRequestParameters _requestParameters = null;

    //    public GoldenDdl_LoadHeaderPages(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
    //        : base(startPage, maxPage, reload, loadImage)
    //    {
    //    }

    //    protected override IEnumerator<GoldenDdl_PostHeader> LoadPage(int page, bool reload, bool loadImage)
    //    {
    //        // http://www.golden-ddl.net/ebooks/page/2/
    //        if (page < 1)
    //            throw new PBException("error wrong page number {0}", page);
    //        string url = __url;
    //        if (page > 1)
    //            url += string.Format("page/{0}/", page);
    //        _requestParameters = new HttpRequestParameters();
    //        _requestParameters.encoding = Encoding.Default;
    //        return _Load(url, reload, loadImage);
    //    }

    //    protected override IEnumerator<GoldenDdl_PostHeader> LoadNextPage(bool reload, bool loadImage)
    //    {
    //        if (_headerPage != null)
    //            return _Load(_headerPage.urlNextPage, reload, loadImage);
    //        else
    //            return null;
    //    }

    //    private IEnumerator<GoldenDdl_PostHeader> _Load(string url, bool reload, bool loadImage)
    //    {
    //        if (url != null)
    //        {
    //            // dont use mongo to store header page so key is null and refreshDocumentStore is false
    //            RequestWebData request = new RequestWebData(new RequestFromWeb(url, _requestParameters, reload, loadImage));
    //            _headerPage = GoldenDdl_LoadHeaderPage.Load.Load(request).Document;
    //            if (loadImage)
    //            {
    //                foreach (var postHeader in _headerPage.postHeaders)
    //                    GoldenDdl.LoadImages(postHeader.images);
    //            }
    //            if (_headerPage != null)
    //                return _headerPage.postHeaders.AsEnumerable<GoldenDdl_PostHeader>().GetEnumerator();
    //        }
    //        return null;
    //    }

    //    public static IEnumerable<GoldenDdl_PostHeader> Load(int startPage = 1, int maxPage = 1, bool reload = false, bool loadImage = false)
    //    {
    //        return new GoldenDdl_LoadHeaderPages(startPage, maxPage, reload, loadImage);
    //    }
    //}

    //public class GoldenDdl_LoadHeaderPage_new : WebDocumentStore<int, GoldenDdl_HeaderPage>
    //{
    //    private static bool __trace = false;
    //    private static bool __useUrlCache = false;
    //    private static string __cacheDirectory = null;
    //    private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

    //    private static bool __useMongo = false;
    //    private static string __mongoServer = null;
    //    private static string __mongoDatabase = null;
    //    private static string __mongoCollectionName = null;
    //    private static string __mongoDocumentItemName = null;

    //    private static GoldenDdl_LoadHeaderPage_new __currentLoadHeaderPage = new GoldenDdl_LoadHeaderPage_new();

    //    static GoldenDdl_LoadHeaderPage_new()
    //    {
    //        ClassInit(XmlConfig.CurrentConfig.GetElement("GoldenDdl/Header"));
    //    }

    //    public static void ClassInit(XElement xe)
    //    {
    //        __useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
    //        __cacheDirectory = xe.zXPathValue("CacheDirectory");

    //        __useMongo = xe.zXPathValueBool("UseMongo", __useMongo);
    //        __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
    //        __mongoDatabase = xe.zXPathValue("MongoDatabase");
    //        __mongoCollectionName = xe.zXPathValue("MongoCollection");
    //        __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

    //        //IDocumentStore<GoldenDdl_HeaderPage> documentStore = null;
    //        if (__useMongo)
    //        {
    //            //documentStore = new MongoDocumentStore<GoldenDdl_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
    //            __currentLoadHeaderPage._documentStore = new MongoDocumentStore_new<int, GoldenDdl_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
    //            __currentLoadHeaderPage._documentStore.DefaultSort = "{ 'download.id': 1 }";
    //            __currentLoadHeaderPage._documentStore.GetDataKey = headerPage => headerPage.id;
    //        }

    //        //_load = new LoadWebDataManager<GoldenDdl_HeaderPage>(new GoldenDdl_LoadHeaderPageFromWebManager(GetUrlCache()), documentStore);
    //        __currentLoadHeaderPage._load = new LoadWebDataManager_new<int, GoldenDdl_HeaderPage>(new GoldenDdl_LoadHeaderPageFromWebManager(GetUrlCache()), __currentLoadHeaderPage._documentStore);
    //    }

    //    public static UrlCache GetUrlCache()
    //    {
    //        UrlCache urlCache = null;
    //        if (__useUrlCache)
    //            urlCache = new UrlCache(__cacheDirectory, __urlFileNameType);
    //        return urlCache;
    //    }

    //    public static GoldenDdl_LoadHeaderPage_new CurrentLoadHeaderPage { get { return __currentLoadHeaderPage; } }

    //    protected override int GetKeyFromUrl(string url)
    //    {
    //        return GoldenDdl_LoadHeaderPagesManager.GetHeaderPageKey(url);
    //    }

    //    protected override string GetDataSourceUrl(GoldenDdl_HeaderPage headerPage)
    //    {
    //        return headerPage.sourceUrl;
    //    }

    //    protected override void LoadImages(GoldenDdl_HeaderPage headerPage)
    //    {
    //        foreach (GoldenDdl_PostHeader postHeader in headerPage.postHeaders)
    //            GoldenDdl.LoadImages(postHeader.images);
    //    }

    //    protected override HttpRequestParameters GetHttpRequestParameters()
    //    {
    //        HttpRequestParameters requestParameters = new HttpRequestParameters();
    //        requestParameters.encoding = Encoding.Default;
    //        return requestParameters;
    //    }
    //}

    //public class GoldenDdl_LoadHeaderPages_new : LoadEnumDataPagesFromWeb_new<GoldenDdl_PostHeader>
    //{
    //    private static GoldenDdl_LoadHeaderPages_new _currentLoadHeaderPages = new GoldenDdl_LoadHeaderPages_new();
    //    private static string __url = "http://www.golden-ddl.net/ebooks/";

    //    public static GoldenDdl_LoadHeaderPages_new CurrentLoadHeaderPages { get { return _currentLoadHeaderPages; } }

    //    protected override string GetUrlPage(int page)
    //    {
    //        // http://www.golden-ddl.net/ebooks/page/2/
    //        if (page < 1)
    //            throw new PBException("error wrong page number {0}", page);
    //        string url = __url;
    //        if (page > 1)
    //            url += string.Format("page/{0}/", page);
    //        return url;
    //    }

    //    protected override HttpRequestParameters GetHttpRequestParameters()
    //    {
    //        HttpRequestParameters httpRequestParameters = new HttpRequestParameters();
    //        httpRequestParameters.encoding = Encoding.Default;
    //        return httpRequestParameters;
    //    }

    //    protected override IEnumDataPages<GoldenDdl_PostHeader> GetDataFromWeb(LoadDataFromWeb loadDataFromWeb)
    //    {
    //        XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
    //        string url = loadDataFromWeb.request.Url;
    //        GoldenDdl_HeaderPage data = new GoldenDdl_HeaderPage();

    //        data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='basenavi']//span[@class='nnext']//a/@href"));
    //        IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='dle-content']//div[@class='base']");
    //        List<GoldenDdl_PostHeader> headers = new List<GoldenDdl_PostHeader>();
    //        foreach (XXElement xeHeader in xeHeaders)
    //        {
    //            GoldenDdl_PostHeader header = new GoldenDdl_PostHeader();
    //            header.sourceUrl = url;
    //            header.loadFromWebDate = loadDataFromWeb.loadFromWebDate;

    //            header.urlDetail = xeHeader.XPathValue(".//div[@class='bheading']//a/@href");

    //            //header.images = xeHeader.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

    //            //XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
    //            //header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));
    //            //header.title = RapideDdl.ExtractTextValues(header.infos, xe.XPathValue(".//text()", RapideDdl.TrimFunc1));

    //            //xe = xeHeader.XPathElement(".//div[@class='shdinfo']");
    //            //header.postAuthor = xe.XPathValue(".//span[@class='arg']//a//text()");
    //            //// Aujourd'hui, 17:13
    //            //header.creationDate = RapideDdl.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"), loadDataFromWeb.loadFromWebDate);

    //            //xe = xeHeader.XPathElement(".//div[@class='maincont']");
    //            //header.images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

    //            //RapideDdl.SetTextValues(header, xe.DescendantTextList());

    //            //xe = xeHeader.XPathElement(".//div[@class='morelink']//span[@class='arg']");
    //            //header.category = xe.DescendantTextList(".//a").Select(RapideDdl.TrimFunc1).Where(s => !s.StartsWith("Commentaires")).zToStringValues("/");

    //            headers.Add(header);
    //        }
    //        data.postHeaders = headers.ToArray();
    //        return data;
    //    }
    //}
}
