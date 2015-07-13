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

// todo :
// ok  interface IPost ... faire des Get
// ok  html to xml header
// ok  cache detail tenir compte t=109595 http://www.ebookdz.com/forum/showthread.php?t=109595
// ok  changer IHeaderData GetUrlDetail() => GetHttpRequestDetail()
// ok  changer IPostToDownload sourceUrl => sourceHttpRequest
// ok  faire Ebookdz.InitLoadFromWeb()
// ok  supprimer where dans LoadWebEnumDataPagesManager_new


namespace Download.Print.Ebookdz.old
{
    public class Ebookdz_PostHeader : IHeaderData_v2, IKeyData<int>
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Title;
        public string UrlDetail;

        public WebImage[] Images;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }

        public int GetKey()
        {
            throw new PBException("Ebookdz_PostHeader.GetKey() not implemented");
        }
    }

    public class Ebookdz_HeaderPage : IEnumDataPages_v2<int, IHeaderData_v2>, IEnumDataPages_v3<int, IKeyData<int>>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public Ebookdz_PostHeader[] PostHeaders;
        //public HttpRequest HttpRequestNextPage;
        public string UrlNextPage;

        public int GetKey()
        {
            return Id;
        }

        //public IEnumerable<IHeaderData_new> GetDataList()
        IEnumerable<IHeaderData_v2> IEnumDataPages_v2<int, IHeaderData_v2>.GetDataList()
        {
            return PostHeaders;
        }

        IEnumerable<IKeyData<int>> IEnumDataPages_v3<int, IKeyData<int>>.GetDataList()
        {
            return PostHeaders;
        }

        public HttpRequest GetHttpRequestNextPage()
        {
            if (UrlNextPage != null)
                return new HttpRequest { Url = UrlNextPage };
            else
                return null;
        }
    }

    //public class Ebookdz_LoadHeaderPageFromWebManager : LoadDataFromWebManager<IEnumDataPages<int, IHeaderData>>
    public class Ebookdz_LoadHeaderPageFromWebManager : LoadDataFromWebManager_v4<IEnumDataPages_v2<int, IHeaderData_v2>>
    {
        public Ebookdz_LoadHeaderPageFromWebManager(UrlCache urlCache = null)
            //: base(urlCache)
        {
            _urlCache = urlCache;
        }

        protected override void InitLoadFromWeb()
        {
            Ebookdz.InitLoadFromWeb();
        }

        protected override HttpRequestParameters GetHttpRequestParameters()
        {
            return Ebookdz.GetHttpRequestParameters();
        }

        //protected override IEnumDataPages<int, IHeaderData> GetDataFromWeb(LoadDataFromWeb loadDataFromWeb)
        protected override IEnumDataPages_v2<int, IHeaderData_v2> GetData(LoadDataFromWeb_v4 loadDataFromWeb)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.Http.zGetXDocument().Root);
            string url = loadDataFromWeb.WebRequest.HttpRequest.Url;
            Ebookdz_HeaderPage data = new Ebookdz_HeaderPage();
            data.SourceUrl = url;
            data.LoadFromWebDate = loadDataFromWeb.LoadFromWebDate;
            data.Id = Ebookdz_LoadHeaderPagesManager.GetHeaderPageKey(loadDataFromWeb.WebRequest.HttpRequest);

            //data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='basenavi']//span[@class='nnext']//a/@href"));
            data.UrlNextPage = null;

            // <div id="vba_news4">
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='vba_news4']//div[@class='collapse']");
            List<Ebookdz_PostHeader> headers = new List<Ebookdz_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Ebookdz_PostHeader header = new Ebookdz_PostHeader();
                header.SourceUrl = url;
                header.LoadFromWebDate = loadDataFromWeb.LoadFromWebDate;

                //XXElement xe = xeHeader.XPathElement(".//h2[@class='blockhead']//a[@class!='mcbadge mcbadge_r']");
                XXElement xe = xeHeader.XPathElement(".//h2[@class='blockhead']//a[2]");
                header.Title = xe.XPathValue(".//text()");
                header.UrlDetail = xe.XPathValue("./@href");

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
            data.PostHeaders = headers.ToArray();
            return (IEnumDataPages_v2<int, IHeaderData_v2>)data;
        }
    }

    //public class Ebookdz_LoadHeaderPagesManager : LoadWebEnumDataPagesManager_new<int, IEnumDataPages_new<int, IHeaderData>, IHeaderData>
    public class Ebookdz_LoadHeaderPagesManager : LoadWebEnumDataPagesManager_v4<int, IHeaderData_v2>
    {
        //private static string __url = "http://www.ebookdz.com/";
        private static Ebookdz_LoadHeaderPagesManager __currentLoadHeaderPagesManager = null;

        private static bool __trace = false;

        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        //private static UrlFileNameType __urlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Host | UrlFileNameType.Path | UrlFileNameType.Query;

        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        static Ebookdz_LoadHeaderPagesManager()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("Ebookdz/Header"));
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

            IDocumentStore_v3<int, IEnumDataPages_v2<int, IHeaderData_v2>> documentStore = null;
            if (__useMongo)
            {
                MongoDocumentStore_v3<int, IEnumDataPages_v2<int, IHeaderData_v2>> mongoDocumentStore = new MongoDocumentStore_v3<int, IEnumDataPages_v2<int, IHeaderData_v2>>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                mongoDocumentStore.DefaultSort = "{ 'download.id': 1 }";
                mongoDocumentStore.GetDataKey = headerPage => headerPage.GetKey();
                mongoDocumentStore.Deserialize = document => (IEnumDataPages_v2<int, IHeaderData_v2>)BsonSerializer.Deserialize<Ebookdz_HeaderPage>(document);
                documentStore = mongoDocumentStore;
            }

            __currentLoadHeaderPagesManager = new Ebookdz_LoadHeaderPagesManager(new Ebookdz_LoadHeaderPageFromWebManager(GetUrlCache()), documentStore);
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

        public Ebookdz_LoadHeaderPagesManager(LoadDataFromWebManager_v4<IEnumDataPages_v2<int, IHeaderData_v2>> loadDataFromWeb, IDocumentStore_v3<int, IEnumDataPages_v2<int, IHeaderData_v2>> documentStore = null)
            : base(loadDataFromWeb, documentStore)
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static Ebookdz_LoadHeaderPagesManager CurrentLoadHeaderPagesManager { get { return __currentLoadHeaderPagesManager; } }

        protected override HttpRequest GetHttpRequestPage(int page)
        {
            // no pagination
            if (page != 1)
                throw new PBException("error wrong page number {0}", page);
            return new HttpRequest { Url = Ebookdz.UrlWebSite };
        }

        protected override int GetKeyFromHttpRequest(HttpRequest httpRequest)
        {
            return GetHeaderPageKey(httpRequest);
        }

        //public static int GetHeaderPageKey(string url)
        public static int GetHeaderPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.ebookdz.com/
            // page 2 : no pagination
            if (httpRequest.Url == Ebookdz.UrlWebSite)
                return 1;
            //Uri uri = new Uri(url);
            //string lastSegment = uri.Segments[uri.Segments.Length - 1];
            //lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
            //int page;
            //if (!int.TryParse(lastSegment, out page))
            //    throw new PBException("header page key not found in url \"{0}\"", url);
            //return page;
            throw new PBException("header page key not found in url \"{0}\"", httpRequest.Url);
        }
    }
}
