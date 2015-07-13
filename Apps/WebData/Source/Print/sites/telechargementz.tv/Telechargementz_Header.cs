using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using MongoDB.Bson.Serialization;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.old;

namespace Download.Print.Telechargementz
{
    public class Telechargementz_PostHeader : IHeaderData_v1
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

    //public class Telechargementz_HeaderPage : IEnumDataPages<int, Telechargementz_PostHeader>
    public class Telechargementz_HeaderPage : IEnumDataPages_v1<int, IHeaderData_v1>
    {
        public int id;
        public string sourceUrl;
        public DateTime loadFromWebDate;

        public Telechargementz_PostHeader[] postHeaders;
        public string urlNextPage;

        public int GetKey()
        {
            return id;
        }

        //public IEnumerable<Telechargementz_PostHeader> GetDataList()
        public IEnumerable<IHeaderData_v1> GetDataList()
        {
            return postHeaders;
        }

        public string GetUrlNextPage()
        {
            return urlNextPage;
        }
    }

    //public class Telechargementz_LoadHeaderPageFromWebManager : LoadDataFromWebManager<Telechargementz_HeaderPage>
    public class Telechargementz_LoadHeaderPageFromWebManager : LoadDataFromWebManager_v3<IEnumDataPages_v1<int, IHeaderData_v1>>
    {
        public Telechargementz_LoadHeaderPageFromWebManager(UrlCache_v1 urlCache = null)
            : base(urlCache)
        {
        }

        //protected override Telechargementz_HeaderPage GetDataFromWeb(LoadDataFromWeb loadDataFromWeb)
        protected override IEnumDataPages_v1<int, IHeaderData_v1> GetDataFromWeb(LoadDataFromWeb_v3 loadDataFromWeb)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
            string url = loadDataFromWeb.request.Url;
            Telechargementz_HeaderPage data = new Telechargementz_HeaderPage();
            data.sourceUrl = url;
            data.loadFromWebDate = loadDataFromWeb.loadFromWebDate;
            data.id = Telechargementz_LoadHeaderPagesManager.GetHeaderPageKey(url);

            data.urlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='navigation']//a[text()=\"vers l'avant\"]/@href"));
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='dle-content']//div[@class='custom-post']");
            List<Telechargementz_PostHeader> headers = new List<Telechargementz_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Telechargementz_PostHeader header = new Telechargementz_PostHeader();
                header.sourceUrl = url;
                header.loadFromWebDate = loadDataFromWeb.loadFromWebDate;

                header.urlDetail = xeHeader.XPathValue(".//div[@class='custom-poster']//a/@href");

                headers.Add(header);
            }
            data.postHeaders = headers.ToArray();
            return (IEnumDataPages_v1<int, IHeaderData_v1>)data;
        }
    }

    //public class Telechargementz_LoadHeaderPagesManager : LoadWebEnumDataPagesManager<int, Telechargementz_HeaderPage, Telechargementz_PostHeader>
    public class Telechargementz_LoadHeaderPagesManager : LoadWebEnumDataPagesManager_v3<int, IEnumDataPages_v1<int, IHeaderData_v1>, IHeaderData_v1>
    {
        private static string __url = "http://telechargementz.tv/ebooks/";
        private static Telechargementz_LoadHeaderPagesManager __currentLoadHeaderPagesManager = null;

        private static bool __trace = false;

        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        static Telechargementz_LoadHeaderPagesManager()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("Telechargementz/Header"));
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

            //IDocumentStore_new<int, Telechargementz_HeaderPage> documentStore = null;
            IDocumentStore_v3<int, IEnumDataPages_v1<int, IHeaderData_v1>> documentStore = null;
            if (__useMongo)
            {
                //documentStore = new MongoDocumentStore_new<int, Telechargementz_HeaderPage>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                MongoDocumentStore_v3<int, IEnumDataPages_v1<int, IHeaderData_v1>> mongoDocumentStore = new MongoDocumentStore_v3<int, IEnumDataPages_v1<int, IHeaderData_v1>>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                mongoDocumentStore.DefaultSort = "{ 'download.id': 1 }";
                //documentStore.GetDataKey = headerPage => headerPage.id;
                mongoDocumentStore.GetDataKey = headerPage => headerPage.GetKey();
                mongoDocumentStore.Deserialize = document => (IEnumDataPages_v1<int, IHeaderData_v1>)BsonSerializer.Deserialize<Telechargementz_HeaderPage>(document);
                documentStore = mongoDocumentStore;
            }
            __currentLoadHeaderPagesManager = new Telechargementz_LoadHeaderPagesManager(new Telechargementz_LoadHeaderPageFromWebManager(GetUrlCache()), documentStore);
        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType);
            return urlCache;
        }

        //public Telechargementz_LoadHeaderPagesManager(LoadDataFromWebManager<Telechargementz_HeaderPage> loadDataFromWeb, IDocumentStore_new<int, Telechargementz_HeaderPage> documentStore = null)
        public Telechargementz_LoadHeaderPagesManager(LoadDataFromWebManager_v3<IEnumDataPages_v1<int, IHeaderData_v1>> loadDataFromWeb, IDocumentStore_v3<int, IEnumDataPages_v1<int, IHeaderData_v1>> documentStore = null)
            : base(loadDataFromWeb, documentStore)
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static Telechargementz_LoadHeaderPagesManager CurrentLoadHeaderPagesManager { get { return __currentLoadHeaderPagesManager; } }

        protected override string GetUrlPage(int page)
        {
            // http://telechargementz.tv/ebooks/page/2/
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
            // page 1 : http://telechargementz.tv/ebooks/
            // page 2 : http://telechargementz.tv/ebooks/page/2/
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
}
