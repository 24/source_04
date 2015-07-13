using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MongoDB.Bson.Serialization.Attributes;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Linq;
using pb.Web;
using pb.Web.old;
using Print;

// error cant find link :
//   http://www.rapide-ddl.com/ebooks/magazine/37441-le-nouvel-observateur-no2597-14-au-20-aogt-2014.html

namespace Download.Print.RapideDdl
{
    //[BsonIgnoreExtraElements]
    //public class RapideDdl_PostDetail_old : IRapideDdl_Base, IPostToDownload
    //{
    //    public RapideDdl_PostDetail_old()
    //    {
    //        description = new List<string>();
    //        infos = new NamedValues<ZValue>();
    //        //downloadLinks = new List<string>();
    //    }

    //    public string server { get { return "rapide-ddl.com"; } }
    //    public int id { get; set; }
    //    public string sourceUrl { get; set; }
    //    public DateTime loadFromWebDate { get; set; }

    //    public string title { get; set; }
    //    public PrintType printType { get; set; }
    //    public string originalTitle { get; set; }
    //    public string postAuthor { get; set; }
    //    public DateTime? creationDate { get; set; }
    //    public string category { get; set; }
    //    public List<string> description { get; set; }
    //    public string language { get; set; }
    //    public string size { get; set; }
    //    public int? nbPages { get; set; }
    //    public NamedValues<ZValue> infos { get; set; }
    //    public List<UrlImage> images { get; set; }
    //    //public List<string> downloadLinks { get; set; }
    //    public string[] downloadLinks { get; set; }
    //}

    public class RapideDdl_PostDetail : IPostToDownload
    {
        public RapideDdl_PostDetail()
        {
            infos = new NamedValues<ZValue>();
        }

        //public string server { get { return "rapide-ddl.com"; } }
        public int id;
        public string sourceUrl;
        public DateTime loadFromWebDate { get; set; }

        public string title;
        public PrintType printType;
        public string originalTitle { get; set; }
        public string postAuthor { get; set; }
        public DateTime? creationDate { get; set; }
        public string category { get; set; }
        public string[] description { get; set; }
        public string language { get; set; }
        public string size { get; set; }
        public int? nbPages { get; set; }
        public NamedValues<ZValue> infos { get; set; }
        //public List<UrlImage> images { get; set; }
        public WebImage[] images;
        public string[] downloadLinks;

        public string GetServer()
        {
            return "rapide-ddl.com";
        }

        public int GetKey()
        {
            return id;
        }

        public HttpRequest GetDataHttpRequest()
        {
            return new HttpRequest { Url = sourceUrl };
        }

        public string GetTitle()
        {
            return title;
        }

        public PrintType GetPrintType()
        {
            return printType;
        }

        public string GetOriginalTitle()
        {
            return originalTitle;
        }

        public string GetPostAuthor()
        {
            return postAuthor;
        }

        public DateTime? GetPostCreationDate()
        {
            return creationDate;
        }

        public WebImage[] GetImages()
        {
            return images;
        }

        public void SetImages(WebImage[] images)
        {
            this.images = images;
        }

        public string[] GetDownloadLinks()
        {
            return downloadLinks;
        }

        public PostDownloadLinks GetDownloadLinks_new()
        {
            return null;
        }
    }

    public class RapideDdl_LoadPostDetailFromWebManager : LoadDataFromWebManager_v3<RapideDdl_PostDetail>
    {
        private static bool __trace = false;

        public RapideDdl_LoadPostDetailFromWebManager(UrlCache_v1 urlCache = null)
            : base(urlCache)
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        protected override RapideDdl_PostDetail GetDataFromWeb(LoadDataFromWeb_v3 loadDataFromWeb)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
            RapideDdl_PostDetail data = new RapideDdl_PostDetail();
            data.sourceUrl = loadDataFromWeb.request.Url;
            data.loadFromWebDate = loadDataFromWeb.loadFromWebDate;
            data.id = GetPostDetailKey(data.sourceUrl);

            XXElement xePost = xeSource.XPathElement("//div[@class='lcolomn mainside']");

            //data.category = xePost.DescendantTextList(".//div[@class='spbar']//a").Select(DownloadPrint.TrimFunc1).Where(
            data.category = xePost.XPathElements(".//div[@class='spbar']//a").DescendantTexts().Select(DownloadPrint.Trim).Where(
                s =>
                {
                    s = s.ToLowerInvariant();
                    return s != "" && !s.Contains("acceuil") && !s.Contains("accueil");
                }
                ).zToStringValues("/");
            string category = data.category.ToLowerInvariant();
            data.printType = GetPostType(category);

            //data.title = xePost.DescendantTextList(".//div[@class='spbar']", func: DownloadPrint.TrimFunc1).LastOrDefault();
            data.title = xePost.XPathElements(".//div[@class='spbar']").DescendantTexts().Select(DownloadPrint.Trim).LastOrDefault();
            //ExtractTitleInfos(data);
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.title);
            if (titleInfos.foundInfo)
            {
                data.originalTitle = data.title;
                data.title = titleInfos.title;
                data.infos.SetValues(titleInfos.infos);
            }

            XXElement xe = xePost.XPathElement(".//div[@class='shdinfo']");
            string date = xe.XPathValue(".//span[@class='date']//text()");
            //data.creationDate = Download.Print.RapideDdl.RapideDdl.ParseDateTime(date, loadDataFromWeb.loadFromWebDate);
            data.creationDate = zdate.ParseDateTimeLikeToday(date, loadDataFromWeb.loadFromWebDate, "d-M-yyyy, HH:mm", "d M yyyy", "d MMMM yyyy");
            if (data.creationDate == null)
                pb.Trace.WriteLine("unknow date time \"{0}\"", date);
            if (__trace)
                pb.Trace.WriteLine("creationDate {0} - \"{1}\"", data.creationDate, date);
            data.postAuthor = xe.XPathValue(".//span[@class='arg']//a//text()");

            xe = xePost.XPathElement(".//div[@class='maincont']");
            //data.images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).ToArray();
            data.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).ToArray();

            if (loadDataFromWeb.request.LoadImage)
                data.images = DownloadPrint.LoadImages(data.images).ToArray();

            //RapideDdl.SetTextValues(data, xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a" ));
            // xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a")
            PrintTextValues_v1 textValues = DownloadPrint.PrintTextValuesManager.GetTextValues_v1(xe.DescendantTexts(node => !(node is XElement) || ((XElement)node).Name != "a" ? XNodeFilter.SelectNode : XNodeFilter.SkipNode), data.title);
            data.description = textValues.description;
            data.language = textValues.language;
            data.size = textValues.size;
            data.nbPages = textValues.nbPages;
            data.infos.SetValues(textValues.infos);

            List<string> downloadLinks = new List<string>();
            foreach (XXElement xe2 in xe.XPathElements("div/div"))
            {
                // http://prezup.eu http://pixhst.com/avaxhome/27/36/002e3627.jpeg http://www.zupmage.eu/i/R1UgqdXn4F.jpg
                // http://i.imgur.com/Gu7hagN.jpg http://img11.hostingpics.net/pics/591623liens.png http://www.hapshack.com/images/jUfTZ.gif
                // http://pixhst.com/pictures/3029467
                downloadLinks.AddRange(xe2.XPathValues(".//a/@href").Where(url => !url.StartsWith("http://prezup.eu") && !url.StartsWith("http://pixhst.com")
                    && !url.EndsWith(".jpg") && !url.EndsWith("jpeg") && !url.EndsWith("png") && !url.EndsWith("gif")));
            }
            data.downloadLinks = downloadLinks.ToArray();

            //if (__trace)
            //    RapideDdl_LoadPostDetail.Trace_RapideDdl_PostDetail(data);

            return data;
        }

        private static PrintType GetPostType(string category)
        {
            // category : "Ebooks", "Ebooks/Bandes Dessinée", "Ebooks/Journaux", "Ebooks/Livres", "Ebooks/Magazine"
            switch(category.ToLower())
            {
                case "ebooks/journaux":
                case "ebooks/magazine":
                    return PrintType.Print;
                case "ebooks/livres":
                    return PrintType.Book;
                case "ebooks/bandes dessinée":
                    return PrintType.Comics;
                case "ebooks":
                    return PrintType.UnknowEBook;
                default:
                    return PrintType.Unknow;
            }
        }

        //private static Regex __rgTitleInfo = new Regex(@"\[([^\]]+)\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static void ExtractTitleInfos(RapideDdl_PostDetail post)
        //{
        //    string title = post.title;
        //    bool foundInfo = false;
        //    while (true)
        //    {
        //        Match match = __rgTitleInfo.Match(title);
        //        if (!match.Success)
        //            break;
        //        foundInfo = true;
        //        post.infos.SetValue(match.Groups[1].Value, null);
        //        title = match.zReplace(title, "_");
        //    }
        //    if (foundInfo)
        //    {
        //        post.originalTitle = post.title;
        //        post.title = Download.Print.RapideDdl.RapideDdl.TrimFunc1(title);
        //    }
        //}

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        public static int GetPostDetailKey(string url)
        {
            // http://www.rapide-ddl.com/ebooks/magazine/35030-le-nouvel-observateur-hors-sgrie-week-end-nv-5-juillet-aogt-2014.html
            Uri uri = new Uri(url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(file);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", url);
            return int.Parse(match.Value);
        }
    }

    public class RapideDdl_LoadPostDetail : HeaderDetailWebDocumentStore_v1<int, RapideDdl_HeaderPage, RapideDdl_PostHeader, int, RapideDdl_PostDetail>
    {
        private static bool __trace = false;

        private static string __urlDomain = "http://www.rapide-ddl.com/";

        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        private static RapideDdl_LoadPostDetail __currentLoadPostDetail = new RapideDdl_LoadPostDetail();

        static RapideDdl_LoadPostDetail()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("RapideDdl/Detail"));
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

            if (__useMongo)
            {
                //_documentStore = new MongoDocumentStore<RapideDdl_PostDetail>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                __currentLoadPostDetail._documentStore = new MongoDocumentStore_v3<int, RapideDdl_PostDetail>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                __currentLoadPostDetail._documentStore.DefaultSort = "{ 'download.creationDate': -1 }";
                __currentLoadPostDetail._documentStore.GetDataKey = post => post.GetKey();
            }
            __currentLoadPostDetail._loadWebDataManager = new LoadWebDataManager_v4<int, RapideDdl_PostDetail>(new RapideDdl_LoadPostDetailFromWebManager(GetUrlCache()), __currentLoadPostDetail._documentStore);
            __currentLoadPostDetail._loadEnumDataPagesFromWeb = RapideDdl_LoadHeaderPagesManager.CurrentLoadHeaderPagesManager;
        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType, (url, requestParameters) => (RapideDdl_LoadPostDetailFromWebManager.GetPostDetailKey(url) / 1000 * 1000).ToString());
            return urlCache;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static RapideDdl_LoadPostDetail CurrentLoadPostDetail { get { return __currentLoadPostDetail; } }

        protected override int GetKeyFromUrl(string url)
        {
            return RapideDdl_LoadPostDetailFromWebManager.GetPostDetailKey(url);
        }

        protected override string GetDataSourceUrl(RapideDdl_PostDetail post)
        {
            return post.sourceUrl;
        }

        protected override void LoadImages(RapideDdl_PostDetail post)
        {
            post.images = DownloadPrint.LoadImages(post.images).ToArray();
        }

        protected override HttpRequestParameters_v1 GetHttpRequestParameters()
        {
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.encoding = Encoding.UTF8;
            requestParameters.cookies.Add(new Uri(__urlDomain), new Cookie("hasVisitedSite", "Yes"));
            return requestParameters;
        }

        public void LoadNewDocuments(int maxNbDocumentLoadedFromStore = 7, int startPage = 1, int maxPage = 20, bool loadImage = true)
        {
            int nbDocumentLoadedFromStore = 0;
            base.LoadNewDocuments(maxNbDocumentLoadedFromStore, startPage, maxPage, loadImage,
                loadWebData =>
                {
                    if (__trace)
                    {
                        if (loadWebData.DocumentLoadedFromStore)
                            nbDocumentLoadedFromStore++;
                        RapideDdl_PostDetail post = loadWebData.Document;
                        pb.Trace.Write("post nb {0} load from ", nbDocumentLoadedFromStore);
                        if (loadWebData.DocumentLoadedFromWeb)
                            pb.Trace.Write("web ");
                        if (loadWebData.DocumentLoadedFromStore)
                            pb.Trace.Write("store ");
                        pb.Trace.Write("load date {0:dd/MM/yyyy HH:mm:ss} ", post.loadFromWebDate);
                        pb.Trace.Write("post date {0:dd/MM/yyyy HH:mm:ss} ", post.creationDate);
                        pb.Trace.Write("title \"{0}\" ", post.title);
                        pb.Trace.Write("url \"{0}\" ", post.sourceUrl);
                        pb.Trace.WriteLine();
                    }
                }
                );
        }

        public void RefreshDocumentsStore(int limit = 100, bool reloadFromWeb = false, string query = null, string sort = null, bool loadImage = false)
        {
            int i = 1;
            RefreshDocumentsStore((post, newPost) =>
            {
                // si loadImage == false on récupère la liste des images du post initial
                if (!loadImage)
                    //newPost.images = post.images;
                    newPost.SetImages(post.GetImages());
                pb.Trace.WriteLine("post {0} {1:dd/MM/yyyy HH:mm:ss} images nb {2} links nb {3} title \"{4}\" url \"{5}\"", i++, newPost.creationDate, newPost.GetImages().Length, newPost.downloadLinks.Length, newPost.title, newPost.sourceUrl);
            },
                limit, reloadFromWeb, query, sort, loadImage);
        }
    }

    //public static class RapideDdl_LoadPostDetail_old
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

    //    private static IDocumentStore<RapideDdl_PostDetail> _documentStore = null;
    //    private static LoadWebDataManager<RapideDdl_PostDetail> _load = null;

    //    static RapideDdl_LoadPostDetail_old()
    //    {
    //        ClassInit(XmlConfig.CurrentConfig.GetElement("RapideDdl/Detail"));
    //    }

    //    //public static LoadWebDataManager<RapideDdl_PostDetail> Load { get { return _load; } }
    //    public static IDocumentStore<RapideDdl_PostDetail> DocumentStore { get { return _documentStore; } }

    //    public static void ClassInit(XElement xe)
    //    {
    //        __useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
    //        __cacheDirectory = xe.zXPathValue("CacheDirectory");

    //        __useMongo = xe.zXPathValueBool("UseMongo", __useMongo);
    //        __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
    //        __mongoDatabase = xe.zXPathValue("MongoDatabase");
    //        __mongoCollectionName = xe.zXPathValue("MongoCollection");
    //        __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

    //        if (__useMongo)
    //        {
    //            _documentStore = new MongoDocumentStore<RapideDdl_PostDetail>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
    //            //RapideDdl.InitMongoClassMap();
    //        }

    //        _load = new LoadWebDataManager<RapideDdl_PostDetail>(new RapideDdl_LoadPostDetailFromWebManager(GetUrlCache()), _documentStore);
    //    }

    //    public static UrlCache GetUrlCache()
    //    {
    //        UrlCache urlCache = null;
    //        if (__useUrlCache)
    //            urlCache = new UrlCache(__cacheDirectory, __urlFileNameType, (url, requestParameters) => (RapideDdl_LoadPostDetailFromWebManager.GetPostDetailKey(url) / 1000 * 1000).ToString());
    //        return urlCache;
    //    }

    //    public static LoadWebData<RapideDdl_PostDetail> Load(string url, HttpRequestParameters requestParameters = null, bool reloadFromWeb = false, bool loadImage = false, bool refreshDocumentStore = false)
    //    {
    //        //Trace.WriteLine("RapideDdl_LoadDetail.Load  \"{0}\"", url);
    //        if (requestParameters == null)
    //            requestParameters = new HttpRequestParameters();
    //        requestParameters.encoding = Encoding.UTF8;
    //        requestParameters.cookies.Add(new Uri(url), new Cookie("hasVisitedSite", "Yes"));
    //        LoadWebData<RapideDdl_PostDetail> loadWebData = _load.Load(new RequestWebData(new RequestFromWeb(url, requestParameters, reloadFromWeb, loadImage), RapideDdl_LoadPostDetailFromWebManager.GetPostDetailKey(url), refreshDocumentStore));
    //        return loadWebData;
    //    }

    //    public static IEnumerable<RapideDdl_PostDetail> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
    //    {
    //        if (query == null)
    //            query = "{}";
    //        if (sort == null)
    //            sort = "{ 'download.creationDate': -1 }";
    //        return _documentStore.FindDocuments(query, sort: sort, limit: limit).zAction(post => { if (loadImage) RapideDdl.LoadImages(post.images); });
    //    }

    //    public static void UpdateDocuments(Action<RapideDdl_PostDetail> updateDocument, string query = null, string sort = null, int limit = 0)
    //    {
    //        if (query == null)
    //            query = "{}";
    //        if (sort == null)
    //            sort = "{ 'download.creationDate': -1 }";
    //        int nb = 0;
    //        foreach (RapideDdl_PostDetail post in RapideDdl_LoadPostDetail_old.Find(query, sort: sort, limit: limit))
    //        {
    //            updateDocument(post);
    //            //_documentStore.SaveDocument(RapideDdl_LoadPostDetailFromWebManager.GetPostDetailKey(post.sourceUrl), post);
    //            _documentStore.SaveDocument(post.id, post);
    //            nb++;
    //        }
    //        Trace.WriteLine("{0} document(s) updated", nb);
    //    }

    //    public static IEnumerable<RapideDdl_PostDetail> LoadDetailItemList(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
    //        bool refreshDocumentStore = false)
    //    {
    //        return from header in RapideDdl_LoadHeaderPages.Load(startPage, maxPage, reloadHeaderPage, false) select LoadDetailItem(header, reloadDetail, loadImage, refreshDocumentStore);
    //    }

    //    public static IEnumerable<RapideDdl_PostDetail> LoadDetailItemList(IEnumerable<object> keys)
    //    {
    //        return from key in keys select LoadDetailItem(key);
    //    }

    //    public static RapideDdl_PostDetail LoadDetailItem(RapideDdl_PostHeader header, bool reloadFromWeb = false, bool loadImage = false, bool refreshDocumentStore = false)
    //    {
    //        return RapideDdl_LoadPostDetail_old.Load(header.urlDetail, reloadFromWeb: reloadFromWeb, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore).Document;
    //    }

    //    public static RapideDdl_PostDetail LoadDetailItem(object key, bool loadImage = false)
    //    {
    //        RapideDdl_PostDetail post = _documentStore.LoadDocument(key);
    //        if (loadImage)
    //            RapideDdl.LoadImages(post.images);
    //        return post;
    //    }

    //    // maxPage = 0 : all pages
    //    public static void LoadNewDocuments(int maxNbDocumentLoadedFromStore = 7, int startPage = 1, int maxPage = 20, bool loadImage = true)
    //    {
    //        //bool loadImage = true;              // obligatoire pour charger les images
    //        bool refreshDocumentStore = false;  // obligatoire sinon nbDocumentLoadedFromStore reste à 0
    //        int nbDocumentLoadedFromStore = 0;
    //        foreach (RapideDdl_PostHeader header in RapideDdl_LoadHeaderPages.Load(startPage, maxPage, reload: true, loadImage: false))
    //        {
    //            LoadWebData<RapideDdl_PostDetail> loadWebData = RapideDdl_LoadPostDetail_old.Load(header.urlDetail, reloadFromWeb: false, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);
    //            RapideDdl_PostDetail post = loadWebData.Document;
    //            if (loadWebData.DocumentLoadedFromStore)
    //                nbDocumentLoadedFromStore++;

    //            if (__trace)
    //            {
    //                Trace.Write("post nb {0} load from ", nbDocumentLoadedFromStore);
    //                if (loadWebData.DocumentLoadedFromWeb)
    //                    Trace.Write("web ");
    //                if (loadWebData.DocumentLoadedFromStore)
    //                    Trace.Write("store ");
    //                Trace.Write("load date {0:dd/MM/yyyy HH:mm:ss} ", post.loadFromWebDate);
    //                Trace.Write("post date {0:dd/MM/yyyy HH:mm:ss} ", post.creationDate);
    //                Trace.Write("title \"{0}\" ", post.title);
    //                Trace.Write("url \"{0}\" ", post.sourceUrl);
    //                Trace.WriteLine();
    //            }

    //            if (maxNbDocumentLoadedFromStore != 0 && nbDocumentLoadedFromStore == maxNbDocumentLoadedFromStore)
    //                break;
    //        }
    //    }

    //    public static void RefreshDocumentsStore(int limit = 100, bool reloadFromWeb = false, string query = null, string sort = null, bool loadImage = false)
    //    {
    //        //string query = null;                 // pour avoir tous les posts
    //        //string sort = null;                  // pour avoir le tri par defaut du plus récent au plus vieux
    //        //bool loadImage = true;               // pour filtrer les images
    //        //bool refreshDocumentStore = true;    // pour mettre à jour les documents
    //        int traceLevel = Trace.CurrentTrace.TraceLevel;
    //        Trace.CurrentTrace.TraceLevel = 0;
    //        int i = 1;
    //        foreach (RapideDdl_PostDetail post in from post in RapideDdl_LoadPostDetail_old.Find(query, sort: sort, limit: limit, loadImage: false) select post)
    //        {
    //            //LoadWebData<RapideDdl_PostDetail> loadWebData = RapideDdl_LoadPostDetail_old.Load(post.sourceUrl, reloadFromWeb: reloadFromWeb, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);

    //            HttpRequestParameters requestParameters = new HttpRequestParameters();
    //            requestParameters.encoding = Encoding.UTF8;
    //            requestParameters.cookies.Add(new Uri(post.sourceUrl), new Cookie("hasVisitedSite", "Yes"));
    //            // refreshDocumentStore
    //            LoadWebData<RapideDdl_PostDetail> loadWebData = _load.LoadDocumentFromWeb(new RequestWebData(new RequestFromWeb(post.sourceUrl, requestParameters, reloadFromWeb, loadImage),
    //                RapideDdl_LoadPostDetailFromWebManager.GetPostDetailKey(post.sourceUrl)));

    //            RapideDdl_PostDetail post2 = loadWebData.Document;
    //            // si loadImage == false on récupère la liste des images du post initial
    //            if (!loadImage)
    //                post2.images = post.images;

    //            _load.SaveDocument(loadWebData);

    //            Trace.WriteLine("post {0} {1:dd/MM/yyyy HH:mm:ss} images nb {2} links nb {3} title \"{4}\" url \"{5}\"", i++, post2.creationDate, post2.images.Count, post2.downloadLinks.Length, post2.title, post2.sourceUrl);
    //        }
    //        Trace.CurrentTrace.TraceLevel = traceLevel;
    //    }

    //    public static void Trace_RapideDdl_PostDetail(RapideDdl_PostDetail post)
    //    {
    //        Trace.WriteLine("RapideDdl_PostDetail :");
    //        Trace.WriteLine("  id                                   : {0}", post.id);
    //        Trace.WriteLine("  sourceUrl                            : \"{0}\"", post.sourceUrl);
    //        Trace.WriteLine("  loadFromWebDate                      : {0}", post.loadFromWebDate);
    //        Trace.WriteLine("  title                                : \"{0}\"", post.title);
    //        Trace.WriteLine("  originalTitle                        : \"{0}\"", post.originalTitle);
    //        Trace.WriteLine("  postAuthor                           : \"{0}\"", post.postAuthor);
    //        Trace.WriteLine("  creationDate                         : {0}", post.creationDate);
    //        Trace.WriteLine("  category                             : \"{0}\"", post.category);
    //        //Trace.Write("  description                          : ");
    //        if (post.description.Count == 0)
    //            Trace.WriteLine("  description                          : ");
    //        bool first = true;
    //        foreach (string s in post.description)
    //        {
    //            if (first)
    //                Trace.Write("  description                          : ");
    //            else
    //                Trace.Write("                                       : ");
    //            Trace.WriteLine("\"{0}\"", s);
    //            first = false;
    //        }
    //        Trace.WriteLine("  language                             : \"{0}\"", post.language);
    //        Trace.WriteLine("  size                                 : \"{0}\"", post.size);
    //        Trace.WriteLine("  nbPages                              : {0}", post.nbPages);
    //        //if (post.infos == null)
    //        //    Trace.WriteLine("  infos                                : null");
    //        if (post.infos.Count == 0)
    //            Trace.WriteLine("  infos                                : ");
    //        first = true;
    //        foreach (KeyValuePair<string, ZValue> info in post.infos)
    //        {
    //            if (first)
    //                Trace.Write("  infos                                : ");
    //            else
    //                Trace.Write("                                       : ");
    //            //Trace.WriteLine("\"{0}\" = {1} ({2})", info.Key, info.Value, info.Value != null ? info.Value.GetType().zGetName() : "null");
    //            Trace.Write("\"{0}\" = ", info.Key);
    //            if (info.Value == null)
    //                Trace.WriteLine("null");
    //            else if (info.Value is ZString)
    //            {
    //                string value = (string)(ZString)info.Value;
    //                if (value == null)
    //                    Trace.WriteLine("(null ZString)");
    //                else if (value == "")
    //                    Trace.WriteLine("(empty ZString)");
    //                else
    //                    Trace.WriteLine("\"{0}\" (ZString length {1})", value, value.Length);
    //            }
    //            else
    //                Trace.WriteLine("{0} ({1})", info.Value, info.Value != null ? info.Value.GetType().zGetName() : "null");
    //            first = false;
    //        }
    //        if (post.images.Count == 0)
    //            Trace.WriteLine("  images                               : ");
    //        first = true;
    //        foreach (UrlImage image in post.images)
    //        {
    //            if (first)
    //                Trace.Write("  images                               : ");
    //            else
    //                Trace.Write("                                       : ");
    //            Trace.WriteLine("\"{0}\"", image.Url);
    //            first = false;
    //        }
    //        if (post.downloadLinks.Length == 0)
    //            Trace.WriteLine("  downloadLinks                        : ");
    //        first = true;
    //        foreach (string s in post.downloadLinks)
    //        {
    //            if (first)
    //                Trace.Write("  downloadLinks                        : ");
    //            else
    //                Trace.Write("                                       : ");
    //            Trace.WriteLine("\"{0}\"", s);
    //            first = false;
    //        }

    //    }
    //}
}
