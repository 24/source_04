using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MongoDB.Bson.Serialization;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace Download.Print.Ebookdz.old
{
    //public class Ebookdz_LoadForumHeaderPageFromWebManager : LoadDataFromWebManager_new<IEnumDataPages_new<int, IHeaderData_new>>
    //{
    //    public Ebookdz_LoadForumHeaderPageFromWebManager(UrlCache_new urlCache = null)
    //    {
    //        _urlCache = urlCache;
    //    }

    //    protected override void InitLoadFromWeb()
    //    {
    //        Ebookdz.InitLoadFromWeb();
    //    }

    //    protected override HttpRequestParameters_new GetHttpRequestParameters()
    //    {
    //        return Ebookdz.GetHttpRequestParameters();
    //    }

    //    protected override IEnumDataPages_new<int, IHeaderData_new> GetData(LoadDataFromWeb_new loadDataFromWeb)
    //    {
    //        XXElement xeSource = new XXElement(loadDataFromWeb.Http.zGetXmlDocument().Root);
    //        string url = loadDataFromWeb.WebRequest.HttpRequest.Url;
    //        Ebookdz_HeaderPage data = new Ebookdz_HeaderPage();
    //        data.SourceUrl = url;
    //        data.LoadFromWebDate = loadDataFromWeb.LoadFromWebDate;
    //        data.Id = Ebookdz_LoadHeaderPagesManager.GetHeaderPageKey(loadDataFromWeb.WebRequest.HttpRequest);

    //        // <div id="above_threadlist" class="above_threadlist">
    //        // <div class="threadpagenav">
    //        // <span class="prev_next">
    //        // <a rel="next" href="forumdisplay.php?f=74&amp;page=2&amp;s=4807e931448c05da34dd54fbd0308479" title="Page suivante - Résultats de 21 à 40 sur 66">
    //        data.UrlNextPage = Ebookdz.GetUrl(zurl.GetUrl(url, xeSource.XPathValue("//div[@id='above_threadlist']//span[@class='prev_next']//a[@rel='next']/@href")));

    //        // <div class="body_bd">
    //        XXElement xePost = xeSource.XPathElement("//div[@class='body_bd']");

    //        // <div id="breadcrumb" class="breadcrumb">
    //        // <ul class="floatcontainer">
    //        // <li class="navbit">
    //        // Forum / Journaux / Presse quotidienne / Autres Journaux

    //        // <div id="threadlist" class="threadlist">
    //        // <ol id="threads" class="threads">

    //        IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='threadlist']//ol[@id='threads']/li");
    //        List<Ebookdz_PostHeader> headers = new List<Ebookdz_PostHeader>();
    //        foreach (XXElement xeHeader in xeHeaders)
    //        {
    //            Ebookdz_PostHeader header = new Ebookdz_PostHeader();
    //            header.SourceUrl = url;
    //            header.LoadFromWebDate = loadDataFromWeb.LoadFromWebDate;

    //            // <div class="threadinfo" title="">
    //            // <div class="inner">
    //            // <a title="" class="title" href="showthread.php?t=111210&amp;s=4807e931448c05da34dd54fbd0308479" id="thread_title_111210">L'OPINION du mardi  20 janvier 2015</a>

    //            XXElement xe = xeHeader.XPathElement(".//div[@class='threadinfo']//a[@class='title']");
    //            header.Title = xe.XPathValue(".//text()");
    //            header.UrlDetail = Ebookdz.GetUrl(zurl.GetUrl(loadDataFromWeb.WebRequest.HttpRequest.Url, xe.XPathValue("@href")));

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
    //        data.PostHeaders = headers.ToArray();
    //        return (IEnumDataPages_new<int, IHeaderData_new>)data;
    //    }
    //}

    public class Ebookdz_LoadForumHeaderPagesManager : LoadWebEnumDataPagesManager_v5<int, IHeaderData_v2>
    {
        //private static string __url = "http://www.ebookdz.com/";
        private static Ebookdz_LoadForumHeaderPagesManager __currentLoadHeaderPagesManager = null;

        private static bool __trace = false;

        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        //private static UrlFileNameType __urlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path | UrlFileNameType.Query;

        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        static Ebookdz_LoadForumHeaderPagesManager()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("Ebookdz/Forum"));
        }

        public static void ClassInit(XElement xe)
        {
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");

            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
            __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

            //IDocumentStore_new<int, IEnumDataPages_new2<int, IHeaderData_new>> documentStore = null;
            IDocumentStore_v3<int, IKeyData<int>> documentStore = null;
            if (__useMongo)
            {
                //MongoDocumentStore_new<int, IEnumDataPages_new2<int, IHeaderData_new>> mongoDocumentStore = new MongoDocumentStore_new<int, IEnumDataPages_new2<int, IHeaderData_new>>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                MongoDocumentStore_v3<int, IKeyData<int>> mongoDocumentStore = new MongoDocumentStore_v3<int, IKeyData<int>>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                mongoDocumentStore.DefaultSort = "{ 'download.id': 1 }";
                mongoDocumentStore.GetDataKey = headerPage => headerPage.GetKey();
                //mongoDocumentStore.Deserialize = document => (IEnumDataPages_new2<int, IHeaderData_new>)BsonSerializer.Deserialize<Ebookdz_HeaderPage>(document);
                mongoDocumentStore.Deserialize = document => (IKeyData<int>)BsonSerializer.Deserialize<Ebookdz_HeaderPage>(document);
                documentStore = mongoDocumentStore;
            }

            //__currentLoadHeaderPagesManager = new Ebookdz_LoadForumHeaderPagesManager(new Ebookdz_LoadForumHeaderPageFromWebManager(GetUrlCache()), documentStore);
            __currentLoadHeaderPagesManager = new Ebookdz_LoadForumHeaderPagesManager();
            __currentLoadHeaderPagesManager.LoadDataFromWebManager = GetLoadDataFromWebManager();
            __currentLoadHeaderPagesManager.DocumentStore = documentStore;
        }

        //public static LoadDataFromWebManager_new2<IEnumDataPages_new2<int, IHeaderData_new>> GetLoadDataFromWebManager()
        public static LoadDataFromWebManager_v5<IKeyData<int>> GetLoadDataFromWebManager()
        {
            //LoadDataFromWebManager_new2<IEnumDataPages_new2<int, IHeaderData_new>> loadDataFromWebManager = new LoadDataFromWebManager_new2<IEnumDataPages_new2<int, IHeaderData_new>>();
            LoadDataFromWebManager_v5<IKeyData<int>> loadDataFromWebManager = new LoadDataFromWebManager_v5<IKeyData<int>>();
            loadDataFromWebManager.UrlCache = GetUrlCache();
            loadDataFromWebManager.InitLoadFromWeb = Ebookdz.InitLoadFromWeb;
            loadDataFromWebManager.GetHttpRequestParameters = Ebookdz.GetHttpRequestParameters;
            loadDataFromWebManager.GetData = Ebookdz.GetForumHeaderPageData;
            return loadDataFromWebManager;
        }

        public static UrlCache GetUrlCache()
        {
            UrlCache urlCache = null;
            if (__useUrlCache)
            {
                urlCache = new UrlCache(__cacheDirectory);
                urlCache.UrlFileNameType = __urlFileNameType;
            }
            return urlCache;
        }

        //public Ebookdz_LoadForumHeaderPagesManager(LoadDataFromWebManager_new<IEnumDataPages_new<int, IHeaderData_new>> loadDataFromWeb, IDocumentStore_new<int, IEnumDataPages_new<int, IHeaderData_new>> documentStore = null)
        //    : base(loadDataFromWeb, documentStore)
        //{
        //}

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static Ebookdz_LoadForumHeaderPagesManager CurrentLoadHeaderPagesManager { get { return __currentLoadHeaderPagesManager; } }

        protected override HttpRequest GetHttpRequestPage(int page)
        {
            // no pagination
            if (page != 1)
                throw new PBException("error wrong page number {0}", page);
            return new HttpRequest { Url = Ebookdz.UrlWebSite };
        }

        //protected override int GetKeyFromHttpRequest(HttpRequest httpRequest)
        //{
        //    return GetHeaderPageKey(httpRequest);
        //}

        //public static int GetHeaderPageKey(HttpRequest httpRequest)
        //{
        //    // http://www.ebookdz.com/forum/forumdisplay.php?f=74
        //    // page 1 : http://www.ebookdz.com/
        //    // page 2 : no pagination
        //    if (httpRequest.Url == Ebookdz.UrlWebSite)
        //        return 1;
        //    //Uri uri = new Uri(url);
        //    //string lastSegment = uri.Segments[uri.Segments.Length - 1];
        //    //lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
        //    //int page;
        //    //if (!int.TryParse(lastSegment, out page))
        //    //    throw new PBException("header page key not found in url \"{0}\"", url);
        //    //return page;
        //    throw new PBException("header page key not found in url \"{0}\"", httpRequest.Url);
        //}
    }
}
