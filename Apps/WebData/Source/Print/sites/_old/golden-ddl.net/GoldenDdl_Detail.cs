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
using pb.Linq;
using pb.Web;
using pb.Web.old;
using Print;

namespace Download.Print.GoldenDdl
{
    // IPostToDownload
    public class GoldenDdl_PostDetail : IPost
    {
        public GoldenDdl_PostDetail()
        {
            infos = new NamedValues<ZValue>();
        }

        //public string server { get { return "golden-ddl.net"; } }
        public int id;
        public string sourceUrl;
        public DateTime loadFromWebDate;

        public string title;
        public PrintType printType;
        public string originalTitle;
        public string postAuthor;
        public DateTime? creationDate;
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
            return "golden-ddl.net";
        }

        public int GetKey()
        {
            return id;
        }

        public HttpRequest GetDataHttpRequest()
        {
            return new HttpRequest { Url = sourceUrl };
        }

        public DateTime GetLoadFromWebDate()
        {
            return loadFromWebDate;
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

    //public class GoldenDdl_LoadPostDetailFromWebManager : LoadDataFromWebManager<GoldenDdl_PostDetail>
    public class GoldenDdl_LoadPostDetailFromWebManager : LoadDataFromWebManager_v3<IPost>
    {
        private static bool __trace = false;

        public GoldenDdl_LoadPostDetailFromWebManager(UrlCache_v1 urlCache = null)
            : base(urlCache)
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        //protected override GoldenDdl_PostDetail GetDataFromWeb(LoadDataFromWeb loadDataFromWeb)
        protected override IPost GetDataFromWeb(LoadDataFromWeb_v3 loadDataFromWeb)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
            GoldenDdl_PostDetail data = new GoldenDdl_PostDetail();
            data.sourceUrl = loadDataFromWeb.request.Url;
            data.loadFromWebDate = loadDataFromWeb.loadFromWebDate;
            data.id = GetPostDetailKey(data.sourceUrl);

            XXElement xePost = xeSource.XPathElement("//div[@id='dle-content']");

            //data.category = xePost.DescendantTextList(".//div[@class='hdiin']//a").Select(DownloadPrint.TrimFunc1).zToStringValues("/");
            data.category = xePost.XPathElements(".//div[@class='hdiin']//a").DescendantTexts().Select(DownloadPrint.Trim).zToStringValues("/");
            string category = data.category.ToLowerInvariant();
            data.printType = GetPrintType(category);
            //pb.Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);

            //data.title = xePost.XPathValue(".//div[@class='bheading']//text()", DownloadPrint.Trim);
            data.title = xePost.XPathValue(".//div[@class='bheading']//text()").Trim(DownloadPrint.TrimChars);
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.title);
            if (titleInfos.foundInfo)
            {
                data.originalTitle = data.title;
                data.title = titleInfos.title;
                data.infos.SetValues(titleInfos.infos);
            }

            string date = xePost.XPathValue(".//div[@class='datenews']//text()");
            data.creationDate = zdate.ParseDateTimeLikeToday(date, loadDataFromWeb.loadFromWebDate, "d-M-yyyy, HH:mm", "d M yyyy", "d MMMM yyyy");
            if (data.creationDate == null)
                pb.Trace.WriteLine("unknow date time \"{0}\"", date);
            if (__trace)
                pb.Trace.WriteLine("creationDate {0} - \"{1}\"", data.creationDate, date);

            data.postAuthor = xePost.XPathValue(".//div[@class='argr']//a//text()");

            XXElement xe = xePost.XPathElement(".//div[@class='maincont']");
            //data.images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).ToArray();
            data.images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).ToArray();


            // force load image to get image width and height
            if (loadDataFromWeb.request.LoadImage)
                data.images = DownloadPrint.LoadImages(data.images).ToArray();

            // get infos, description, language, size, nbPages
            //PrintTextValues_old textValues = DownloadPrint.PrintTextValuesManager.GetTextValues_old(xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a"), data.title);
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

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        private static PrintType GetPrintType(string category)
        {
            // category : "Ebooks", "Ebooks/Bandes Dessinée", "Ebooks/Journaux", "Ebooks/Livres", "Ebooks/Magazine", "Magazines/Journaux"
            switch (category.ToLower())
            {
                case "ebooks/journaux":
                case "ebooks/magazines":
                case "magazines/journaux":
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

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        public static int GetPostDetailKey(string url)
        {
            // http://www.free-telechargement.org/magazines/43247-pc-update-no10-mars-avril-2004-pdf.html
            Uri uri = new Uri(url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(file);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", url);
            return int.Parse(match.Value);
        }
    }

    //public class GoldenDdl_LoadPostDetail : HeaderDetailWebDocumentStore<int, GoldenDdl_HeaderPage, GoldenDdl_PostHeader, int, GoldenDdl_PostDetail>
    public class GoldenDdl_LoadPostDetail : HeaderDetailWebDocumentStore_v2<int, int, IPost>
    {
        private static bool __trace = false;
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path;

        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        private static GoldenDdl_LoadPostDetail __currentLoadPostDetail = new GoldenDdl_LoadPostDetail();

        static GoldenDdl_LoadPostDetail()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("GoldenDdl/Detail"));
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
                //__currentLoadPostDetail._documentStore = new MongoDocumentStore_new<int, GoldenDdl_PostDetail>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                MongoDocumentStore_v3<int, IPost> mongoDocumentStore = new MongoDocumentStore_v3<int, IPost>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                mongoDocumentStore.DefaultSort = "{ 'download.creationDate': -1 }";
                mongoDocumentStore.GetDataKey = post => post.GetKey();
                mongoDocumentStore.Deserialize = document => BsonSerializer.Deserialize<GoldenDdl_PostDetail>(document);
                __currentLoadPostDetail._documentStore = mongoDocumentStore;
            }

            //__currentLoadPostDetail._loadWebDataManager = new LoadWebDataManager_new<int, GoldenDdl_PostDetail>(new GoldenDdl_LoadPostDetailFromWebManager(GetUrlCache()), __currentLoadPostDetail._documentStore);
            __currentLoadPostDetail._loadWebDataManager = new LoadWebDataManager_v4<int, IPost>(new GoldenDdl_LoadPostDetailFromWebManager(GetUrlCache()), __currentLoadPostDetail._documentStore);
            __currentLoadPostDetail._loadEnumDataPagesFromWeb = GoldenDdl_LoadHeaderPagesManager.CurrentLoadHeaderPagesManager;
        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType, (url, requestParameters) => (GoldenDdl_LoadPostDetailFromWebManager.GetPostDetailKey(url) / 1000 * 1000).ToString());
            return urlCache;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static GoldenDdl_LoadPostDetail CurrentLoadPostDetail { get { return __currentLoadPostDetail; } }

        protected override int GetKeyFromUrl(string url)
        {
            return GoldenDdl_LoadPostDetailFromWebManager.GetPostDetailKey(url);
        }

        //protected override int GetDataKey(GoldenDdl_PostDetail post)
        //{
        //    return post.id;
        //}

        //protected override string GetDataSourceUrl(GoldenDdl_PostDetail post)
        protected override string GetDataSourceUrl(IPost post)
        {
            return post.GetDataHttpRequest().Url;
        }

        //protected override string GetDefaultSort()
        //{
        //    return "{ 'download.creationDate': -1 }";
        //}

        //protected override void LoadImages(GoldenDdl_PostDetail post)
        protected override void LoadImages(IPost post)
        {
            //DownloadPrint.LoadImages(post.GetImages());
            DownloadPrint.LoadImages(post);
        }

        protected override HttpRequestParameters_v1 GetHttpRequestParameters()
        {
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.encoding = Encoding.Default;
            return requestParameters;
        }

        //public static LoadWebData<GoldenDdl_PostDetail> Load(string url, HttpRequestParameters requestParameters = null, bool reloadFromWeb = false, bool loadImage = false, bool refreshDocumentStore = false)
        //{
        //    //Trace.WriteLine("GoldenDdl_LoadDetail.Load  \"{0}\"", url);
        //    if (requestParameters == null)
        //        requestParameters = new HttpRequestParameters();
        //    requestParameters.encoding = Encoding.Default;
        //    LoadWebData<GoldenDdl_PostDetail> loadWebData = _load.Load(new RequestWebData(new RequestFromWeb(url, requestParameters, reloadFromWeb, loadImage), GoldenDdl_LoadPostDetailFromWebManager.GetPostDetailKey(url), refreshDocumentStore));
        //    return loadWebData;
        //}

        //public static IEnumerable<GoldenDdl_PostDetail> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
        //{
        //    if (query == null)
        //        query = "{}";
        //    if (sort == null)
        //        sort = "{ 'download.creationDate': -1 }";
        //    return _documentStore.FindDocuments(query, sort: sort, limit: limit).zAction(post => { if (loadImage) GoldenDdl.LoadImages(post.images); });
        //}

        //public static void UpdateDocuments(Action<GoldenDdl_PostDetail> updateDocument, string query = null, string sort = null, int limit = 0)
        //{
        //    if (query == null)
        //        query = "{}";
        //    if (sort == null)
        //        sort = "{ 'download.creationDate': -1 }";
        //    int nb = 0;
        //    foreach (GoldenDdl_PostDetail post in GoldenDdl_LoadPostDetail.Find(query, sort: sort, limit: limit))
        //    {
        //        updateDocument(post);
        //        _documentStore.SaveDocument(post.id, post);
        //        nb++;
        //    }
        //    Trace.WriteLine("{0} document(s) updated", nb);
        //}

        //public IEnumerable<GoldenDdl_PostDetail> LoadDetailItemList(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
        //    bool refreshDocumentStore = false)
        //{
        //    //return from header in GoldenDdl_LoadHeaderPages.Load(startPage, maxPage, reloadHeaderPage, false) select LoadDetailItem(header, reloadDetail, loadImage, refreshDocumentStore);
        //    return from header in GoldenDdl_LoadHeaderPages.Load(startPage, maxPage, reloadHeaderPage, false)
        //           select LoadDocument(header.urlDetail, reloadFromWeb: reloadDetail, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore).Document;
        //}

        //public IEnumerable<GoldenDdl_PostDetail> LoadDetailItemList(IEnumerable<object> keys)
        //public IEnumerable<GoldenDdl_PostDetail> LoadDetailItemList(IEnumerable<int> keys)
        //{
        //    //return from key in keys select LoadDetailItem(key);
        //    return from key in keys select LoadDocument(key);
        //}

        //public GoldenDdl_PostDetail LoadDetailItem(GoldenDdl_PostHeader header, bool reloadFromWeb = false, bool loadImage = false, bool refreshDocumentStore = false)
        //{
        //    // GoldenDdl_LoadPostDetail
        //    return Load(header.urlDetail, reloadFromWeb: reloadFromWeb, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore).Document;
        //}

        //public static GoldenDdl_PostDetail LoadDetailItem(object key, bool loadImage = false)
        //{
        //    GoldenDdl_PostDetail post = _documentStore.LoadDocument(key);
        //    if (loadImage)
        //        GoldenDdl.LoadImages(post.images);
        //    return post;
        //}

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
                        //GoldenDdl_PostDetail post = loadWebData.Document;
                        IPost post = loadWebData.Document;
                        pb.Trace.Write("post nb {0} load from ", nbDocumentLoadedFromStore);
                        if (loadWebData.DocumentLoadedFromWeb)
                            pb.Trace.Write("web ");
                        if (loadWebData.DocumentLoadedFromStore)
                            pb.Trace.Write("store ");
                        pb.Trace.Write("load date {0:dd/MM/yyyy HH:mm:ss} ", post.GetLoadFromWebDate());
                        pb.Trace.Write("post date {0:dd/MM/yyyy HH:mm:ss} ", post.GetPostCreationDate());
                        pb.Trace.Write("title \"{0}\" ", post.GetTitle());
                        pb.Trace.Write("url \"{0}\" ", post.GetDataHttpRequest().Url);
                        pb.Trace.WriteLine();
                    }
                }
                );
        }

        //// maxPage = 0 : all pages
        //public void LoadNewDocuments(int maxNbDocumentLoadedFromStore = 7, int startPage = 1, int maxPage = 20, bool loadImage = true)
        //{
        //    //bool loadImage = true;              // obligatoire pour charger les images
        //    bool refreshDocumentStore = false;  // obligatoire sinon nbDocumentLoadedFromStore reste à 0
        //    int nbDocumentLoadedFromStore = 0;
        //    foreach (GoldenDdl_PostHeader header in GoldenDdl_LoadHeaderPages.Load(startPage, maxPage, reload: true, loadImage: false))
        //    {
        //        //LoadWebData<GoldenDdl_PostDetail> loadWebData = GoldenDdl_LoadPostDetail.Load(header.urlDetail, reloadFromWeb: false, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);
        //        LoadWebData_new<int, GoldenDdl_PostDetail> loadWebData = LoadDocument(header.urlDetail, reloadFromWeb: false, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);
        //        GoldenDdl_PostDetail post = loadWebData.Document;
        //        if (loadWebData.DocumentLoadedFromStore)
        //            nbDocumentLoadedFromStore++;

        //        if (__trace)
        //        {
        //            Trace.Write("post nb {0} load from ", nbDocumentLoadedFromStore);
        //            if (loadWebData.DocumentLoadedFromWeb)
        //                Trace.Write("web ");
        //            if (loadWebData.DocumentLoadedFromStore)
        //                Trace.Write("store ");
        //            Trace.Write("load date {0:dd/MM/yyyy HH:mm:ss} ", post.loadFromWebDate);
        //            Trace.Write("post date {0:dd/MM/yyyy HH:mm:ss} ", post.creationDate);
        //            Trace.Write("title \"{0}\" ", post.title);
        //            Trace.Write("url \"{0}\" ", post.sourceUrl);
        //            Trace.WriteLine();
        //        }

        //        if (maxNbDocumentLoadedFromStore != 0 && nbDocumentLoadedFromStore == maxNbDocumentLoadedFromStore)
        //            break;
        //    }
        //}

        //public static void RefreshDocumentsStore(int limit = 100, bool reloadFromWeb = false, string query = null, string sort = null, bool loadImage = false)
        //{
        //    //string query = null;                 // pour avoir tous les posts
        //    //string sort = null;                  // pour avoir le tri par defaut du plus récent au plus vieux
        //    //bool loadImage = true;               // pour filtrer les images
        //    //bool refreshDocumentStore = true;    // pour mettre à jour les documents
        //    int traceLevel = Trace.CurrentTrace.TraceLevel;
        //    Trace.CurrentTrace.TraceLevel = 0;
        //    int i = 1;
        //    foreach (GoldenDdl_PostDetail post in from post in GoldenDdl_LoadPostDetail.Find(query, sort: sort, limit: limit, loadImage: false) select post)
        //    {
        //        //LoadWebData<GoldenDdl_PostDetail> loadWebData = GoldenDdl_LoadPostDetail.Load(post.sourceUrl, reloadFromWeb: reloadFromWeb, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);

        //        HttpRequestParameters requestParameters = new HttpRequestParameters();
        //        requestParameters.encoding = Encoding.Default;
        //        // refreshDocumentStore
        //        LoadWebData<GoldenDdl_PostDetail> loadWebData = _load.LoadDocumentFromWeb(new RequestWebData(new RequestFromWeb(post.sourceUrl, requestParameters, reloadFromWeb, loadImage),
        //            GoldenDdl_LoadPostDetailFromWebManager.GetPostDetailKey(post.sourceUrl)));

        //        GoldenDdl_PostDetail post2 = loadWebData.Document;
        //        // si loadImage == false on récupère la liste des images du post initial
        //        if (!loadImage)
        //            post2.images = post.images;

        //        _load.SaveDocument(loadWebData);

        //        Trace.WriteLine("post {0} {1:dd/MM/yyyy HH:mm:ss} images nb {2} links nb {3} title \"{4}\" url \"{5}\"", i++, post2.creationDate, post2.images.Count, post2.downloadLinks.Length, post2.title, post2.sourceUrl);
        //    }
        //    Trace.CurrentTrace.TraceLevel = traceLevel;
        //}

        public void RefreshDocumentsStore(int limit = 100, bool reloadFromWeb = false, string query = null, string sort = null, bool loadImage = false)
        {
            int i = 1;
            RefreshDocumentsStore((post, newPost) =>
                {
                    // si loadImage == false on récupère la liste des images du post initial
                    if (!loadImage)
                        //newPost.images = post.images;
                        newPost.SetImages(post.GetImages());
                    pb.Trace.WriteLine("post {0} {1:dd/MM/yyyy HH:mm:ss} images nb {2} links nb {3} title \"{4}\" url \"{5}\"", i++, newPost.GetPostCreationDate(), newPost.GetImages().Length, newPost.GetDownloadLinks().Length, newPost.GetTitle(), newPost.GetDataHttpRequest().Url);
                },
                limit, reloadFromWeb, query, sort, loadImage);
        }
    }

    //public static class GoldenDdl_LoadPostDetail_old
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

    //    private static IDocumentStore<GoldenDdl_PostDetail> _documentStore = null;
    //    private static LoadWebDataManager<GoldenDdl_PostDetail> _load = null;

    //    static GoldenDdl_LoadPostDetail_old()
    //    {
    //        ClassInit(XmlConfig.CurrentConfig.GetElement("GoldenDdl/Detail"));
    //    }

    //    public static IDocumentStore<GoldenDdl_PostDetail> DocumentStore { get { return _documentStore; } }

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
    //            _documentStore = new MongoDocumentStore<GoldenDdl_PostDetail>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
    //        }

    //        _load = new LoadWebDataManager<GoldenDdl_PostDetail>(new GoldenDdl_LoadPostDetailFromWebManager(GetUrlCache()), _documentStore);
    //    }

    //    public static UrlCache GetUrlCache()
    //    {
    //        UrlCache urlCache = null;
    //        if (__useUrlCache)
    //            urlCache = new UrlCache(__cacheDirectory, __urlFileNameType, (url, requestParameters) => (GoldenDdl_LoadPostDetailFromWebManager.GetPostDetailKey(url) / 1000 * 1000).ToString());
    //        return urlCache;
    //    }

    //    public static LoadWebData<GoldenDdl_PostDetail> Load(string url, HttpRequestParameters requestParameters = null, bool reloadFromWeb = false, bool loadImage = false, bool refreshDocumentStore = false)
    //    {
    //        //Trace.WriteLine("GoldenDdl_LoadDetail.Load  \"{0}\"", url);
    //        if (requestParameters == null)
    //            requestParameters = new HttpRequestParameters();
    //        requestParameters.encoding = Encoding.Default;
    //        LoadWebData<GoldenDdl_PostDetail> loadWebData = _load.Load(new RequestWebData(new RequestFromWeb(url, requestParameters, reloadFromWeb, loadImage), GoldenDdl_LoadPostDetailFromWebManager.GetPostDetailKey(url), refreshDocumentStore));
    //        return loadWebData;
    //    }

    //    public static IEnumerable<GoldenDdl_PostDetail> Find(string query = null, string sort = null, int limit = 0, bool loadImage = false)
    //    {
    //        if (query == null)
    //            query = "{}";
    //        if (sort == null)
    //            sort = "{ 'download.creationDate': -1 }";
    //        return _documentStore.FindDocuments(query, sort: sort, limit: limit).zAction(post => { if (loadImage) GoldenDdl.LoadImages(post.images); });
    //    }

    //    public static void UpdateDocuments(Action<GoldenDdl_PostDetail> updateDocument, string query = null, string sort = null, int limit = 0)
    //    {
    //        if (query == null)
    //            query = "{}";
    //        if (sort == null)
    //            sort = "{ 'download.creationDate': -1 }";
    //        int nb = 0;
    //        //foreach (GoldenDdl_PostDetail post in Find(query, sort: sort, limit: limit))
    //        foreach (GoldenDdl_PostDetail post in Find(query, sort: sort, limit: limit))
    //        {
    //            updateDocument(post);
    //            _documentStore.SaveDocument(post.id, post);
    //            nb++;
    //        }
    //        Trace.WriteLine("{0} document(s) updated", nb);
    //    }

    //    // not used
    //    public static IEnumerable<GoldenDdl_PostDetail> LoadDetailItemList(int startPage = 1, int maxPage = 1, bool reloadHeaderPage = false, bool reloadDetail = false, bool loadImage = false,
    //        bool refreshDocumentStore = false)
    //    {
    //        return from header in GoldenDdl_LoadHeaderPages.Load(startPage, maxPage, reloadHeaderPage, false) select LoadDetailItem(header, reloadDetail, loadImage, refreshDocumentStore);
    //    }

    //    // not used
    //    public static IEnumerable<GoldenDdl_PostDetail> LoadDetailItemList(IEnumerable<object> keys)
    //    {
    //        return from key in keys select LoadDetailItem(key);
    //    }

    //    public static GoldenDdl_PostDetail LoadDetailItem(GoldenDdl_PostHeader header, bool reloadFromWeb = false, bool loadImage = false, bool refreshDocumentStore = false)
    //    {
    //        return Load(header.urlDetail, reloadFromWeb: reloadFromWeb, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore).Document;
    //    }

    //    public static GoldenDdl_PostDetail LoadDetailItem(object key, bool loadImage = false)
    //    {
    //        GoldenDdl_PostDetail post = _documentStore.LoadDocument(key);
    //        if (loadImage)
    //            GoldenDdl.LoadImages(post.images);
    //        return post;
    //    }

    //    // maxPage = 0 : all pages
    //    public static void LoadNewDocuments(int maxNbDocumentLoadedFromStore = 7, int startPage = 1, int maxPage = 20, bool loadImage = true)
    //    {
    //        //bool loadImage = true;              // obligatoire pour charger les images
    //        bool refreshDocumentStore = false;  // obligatoire sinon nbDocumentLoadedFromStore reste à 0
    //        int nbDocumentLoadedFromStore = 0;
    //        foreach (GoldenDdl_PostHeader header in GoldenDdl_LoadHeaderPages.Load(startPage, maxPage, reload: true, loadImage: false))
    //        {
    //            LoadWebData<GoldenDdl_PostDetail> loadWebData = Load(header.urlDetail, reloadFromWeb: false, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);
    //            GoldenDdl_PostDetail post = loadWebData.Document;
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
    //        foreach (GoldenDdl_PostDetail post in from post in Find(query, sort: sort, limit: limit, loadImage: false) select post)
    //        {
    //            //LoadWebData<GoldenDdl_PostDetail> loadWebData = Load(post.sourceUrl, reloadFromWeb: reloadFromWeb, loadImage: loadImage, refreshDocumentStore: refreshDocumentStore);

    //            HttpRequestParameters requestParameters = new HttpRequestParameters();
    //            requestParameters.encoding = Encoding.Default;
    //            // refreshDocumentStore
    //            LoadWebData<GoldenDdl_PostDetail> loadWebData = _load.LoadDocumentFromWeb(new RequestWebData(new RequestFromWeb(post.sourceUrl, requestParameters, reloadFromWeb, loadImage),
    //                GoldenDdl_LoadPostDetailFromWebManager.GetPostDetailKey(post.sourceUrl)));

    //            GoldenDdl_PostDetail post2 = loadWebData.Document;
    //            // si loadImage == false on récupère la liste des images du post initial
    //            if (!loadImage)
    //                post2.images = post.images;

    //            _load.SaveDocument(loadWebData);

    //            Trace.WriteLine("post {0} {1:dd/MM/yyyy HH:mm:ss} images nb {2} links nb {3} title \"{4}\" url \"{5}\"", i++, post2.creationDate, post2.images.Count, post2.downloadLinks.Length, post2.title, post2.sourceUrl);
    //        }
    //        Trace.CurrentTrace.TraceLevel = traceLevel;
    //    }
    //}
}
