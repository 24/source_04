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
using Print;

namespace Download.Print.Ebookdz.old
{
    public class Ebookdz_PostDetail : IPost
    {
        public Ebookdz_PostDetail()
        {
            Infos = new NamedValues<ZValue>(useLowercaseKey: true);
        }

        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public string Title;
        public PrintType PrintType;
        public string OriginalTitle;
        public string PostAuthor;
        public DateTime? PostCreationDate;
        public string Category;
        public string[] Description;
        public string Language;
        public string Size;
        public int? NbPages;
        public NamedValues<ZValue> Infos;
        public WebImage[] Images;
        public string[] DownloadLinks;

        public string GetServer()
        {
            return "ebookdz.com";
        }

        public int GetKey()
        {
            return Id;
        }

        public HttpRequest GetDataHttpRequest()
        {
            return new HttpRequest { Url = SourceUrl };
        }

        public DateTime GetLoadFromWebDate()
        {
            return LoadFromWebDate;
        }

        public string GetTitle()
        {
            return Title;
        }

        public PrintType GetPrintType()
        {
            return PrintType;
        }

        public string GetOriginalTitle()
        {
            return OriginalTitle;
        }

        public string GetPostAuthor()
        {
            return PostAuthor;
        }

        public DateTime? GetPostCreationDate()
        {
            return PostCreationDate;
        }

        public WebImage[] GetImages()
        {
            return Images;
        }

        public void SetImages(WebImage[] images)
        {
            Images = images;
        }

        public string[] GetDownloadLinks()
        {
            return DownloadLinks;
        }

        public PostDownloadLinks GetDownloadLinks_new()
        {
            return null;
        }
    }

    public class Ebookdz_LoadPostDetailFromWebManager : LoadDataFromWebManager_v4<IPost>
    {
        private static bool __trace = false;

        public Ebookdz_LoadPostDetailFromWebManager(UrlCache urlCache = null)
        {
            _urlCache = urlCache;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        protected override void InitLoadFromWeb()
        {
            Ebookdz.InitLoadFromWeb();
        }

        protected override HttpRequestParameters GetHttpRequestParameters()
        {
            return Ebookdz.GetHttpRequestParameters();
        }

        protected override IPost GetData(LoadDataFromWeb_v4 loadDataFromWeb)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.Http.zGetXDocument().Root);
            Ebookdz_PostDetail data = new Ebookdz_PostDetail();
            data.SourceUrl = loadDataFromWeb.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = loadDataFromWeb.LoadFromWebDate;
            data.Id = GetPostDetailKey(loadDataFromWeb.WebRequest.HttpRequest);

            // <div class="body_bd">
            XXElement xePost = xeSource.XPathElement("//div[@class='body_bd']");

            // Le Monde + Magazine + 2 suppléments du samedi 03 janvier 2015
            //data.Title = xePost.XPathValue(".//div[@id='pagetitle']//a//text()", DownloadPrint.Trim);
            data.Title = xePost.XPathValue(".//div[@id='pagetitle']//a//text()").Trim(DownloadPrint.TrimChars);
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.Title);
            if (titleInfos.foundInfo)
            {
                data.OriginalTitle = data.Title;
                data.Title = titleInfos.title;
                data.Infos.SetValues(titleInfos.infos);
            }

            // Forum / Journaux / Presse quotidienne / Le Monde / Journal Le Monde + Magazine + 2 suppléments du samedi 03 janvier 2015
            string lowerTitle = null;
            if (data.Title != null)
                lowerTitle = data.Title.ToLowerInvariant();
            //data.Category = xePost.DescendantTextList(".//div[@id='breadcrumb']//a").Where(text => { text = text.ToLowerInvariant(); return text != "forum" && !text.EndsWith(lowerTitle); }).Select(DownloadPrint.TrimFunc1).zToStringValues("/");
            data.Category = xePost.XPathElements(".//div[@id='breadcrumb']//a").DescendantTexts().Where(text => { text = text.ToLowerInvariant(); return text != "forum" && !text.EndsWith(lowerTitle); }).Select(DownloadPrint.Trim).zToStringValues("/");
            string category = data.Category.ToLowerInvariant();
            data.PrintType = GetPrintType(category);
            //Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);

            // <div id="postlist" class="postlist restrain">
            XXElement xe = xePost.XPathElement(".//div[@id='postlist']");

            // Aujourd'hui, 07h32 - Aujourd'hui, 10h51 - Hier, 12h55 - 22/02/2014, 21h09
            //string date = xe.DescendantTextList(".//div[@class='posthead']//text()", nodeFilter: node => node.zGetName() != "a").zToStringValues("");
            XXElement xe2 = xe.XPathElement(".//div[@class='posthead']");
            //string date = xe2.DescendantTextList(nodeFilter: node => node.zGetName() != "a").zToStringValues("");
            string date = xe2.DescendantTexts(node => node.zGetName() != "a" ? XNodeFilter.SelectNode : XNodeFilter.SkipNode).zToStringValues("");
            date = date.Replace('\xA0', ' ');
            data.PostCreationDate = zdate.ParseDateTimeLikeToday(date, loadDataFromWeb.LoadFromWebDate, @"d/M/yyyy, HH\hmm", @"d-M-yyyy, HH\hmm");
            if (data.PostCreationDate == null)
                pb.Trace.WriteLine("unknow post creation date \"{0}\"", date);
            if (__trace)
                pb.Trace.WriteLine("post creation date {0} - \"{1}\"", data.PostCreationDate, date);

            //data.PostAuthor = xe.XPathValue(".//div[@class='userinfo']//a//text()", DownloadPrint.Trim);
            data.PostAuthor = xe.XPathValue(".//div[@class='userinfo']//a//text()").Trim(DownloadPrint.TrimChars);

            // <div class="postbody">
            xe = xePost.XPathElement(".//div[@class='postbody']//div[@class='content']//blockquote/div");

            //data.Images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(data.SourceUrl, xeImg.zAttribValue("src")))).ToArray();
            data.Images = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(data.SourceUrl, xeImg.zAttribValue("src")))).ToArray();

            // force load image to get image width and height
            if (loadDataFromWeb.WebRequest.LoadImage)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            // get infos, description, language, size, nbPages
            // xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a")
            PrintTextValues_v1 textValues = DownloadPrint.PrintTextValuesManager.GetTextValues_v1(xe.DescendantTexts(node => !(node is XElement) || ((XElement)node).Name != "a" ? XNodeFilter.SelectNode : XNodeFilter.SkipNode), data.Title);
            data.Description = textValues.description;
            data.Language = textValues.language;
            data.Size = textValues.size;
            data.NbPages = textValues.nbPages;
            data.Infos.SetValues(textValues.infos);

            data.DownloadLinks = xe.XPathValues(".//a/@href").ToArray();

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        private static PrintType GetPrintType(string category)
        {
            // Journaux/Presse quotidienne/Le Monde
            category = category.ToLower();
            if (category.StartsWith("journaux/") || category.StartsWith("magazines/"))
                return PrintType.Print;
            else
                return PrintType.UnknowEBook;
            // return PrintType.Book;
            // return PrintType.Comics;
            // return PrintType.UnknowEBook;
            // return PrintType.Unknow;
        }

        private static Regex __postKeyRegex = new Regex(@"\?t=([0-9]+)$", RegexOptions.Compiled);
        public static int GetPostDetailKey(HttpRequest httpRequest)
        {
            // http://www.ebookdz.com/forum/showthread.php?t=109595
            //Uri uri = new Uri(url);
            //string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(httpRequest.Url);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", httpRequest.Url);
            return int.Parse(match.Groups[1].Value);
        }
    }

    public class Ebookdz_LoadPostDetail : HeaderDetailWebDocumentStore_v3<int, int, IPost>
    {
        private static bool __trace = false;
        private static bool __useUrlCache = false;
        private static string __cacheDirectory = null;
        private static UrlFileNameType __urlFileNameType = UrlFileNameType.Path | UrlFileNameType.Query;

        private static bool __useMongo = false;
        private static string __mongoServer = null;
        private static string __mongoDatabase = null;
        private static string __mongoCollectionName = null;
        private static string __mongoDocumentItemName = null;

        private static Ebookdz_LoadPostDetail __currentLoadPostDetail = new Ebookdz_LoadPostDetail();
        private static Ebookdz_LoadPostDetailFromWebManager __loadPostDetailFromWebManager = null;

        static Ebookdz_LoadPostDetail()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("Ebookdz/Detail"));
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

            if (__useMongo)
            {
                MongoDocumentStore_v3<int, IPost> mongoDocumentStore = new MongoDocumentStore_v3<int, IPost>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                mongoDocumentStore.DefaultSort = "{ 'download.creationDate': -1 }";
                mongoDocumentStore.GetDataKey = post => post.GetKey();
                mongoDocumentStore.Deserialize = document => BsonSerializer.Deserialize<Ebookdz_PostDetail>(document);
                __currentLoadPostDetail._documentStore = mongoDocumentStore;
            }
            __loadPostDetailFromWebManager = new Ebookdz_LoadPostDetailFromWebManager(GetUrlCache());
            //__currentLoadPostDetail._loadWebDataManager = new LoadWebDataManager_new2<int, IPost>(new Ebookdz_LoadPostDetailFromWebManager(GetUrlCache()), __currentLoadPostDetail._documentStore);
            __currentLoadPostDetail._loadWebDataManager = new LoadWebDataManager_v5<int, IPost>(__loadPostDetailFromWebManager, __currentLoadPostDetail._documentStore);
            __currentLoadPostDetail._loadEnumDataPagesFromWeb = Ebookdz_LoadHeaderPagesManager.CurrentLoadHeaderPagesManager;
        }

        public static UrlCache GetUrlCache()
        {
            UrlCache urlCache = null;
            if (__useUrlCache)
            {
                //urlCache = new UrlCache_new(__cacheDirectory, __urlFileNameType, (url, requestParameters) => (Ebookdz_LoadPostDetailFromWebManager.GetPostDetailKey(url) / 1000 * 1000).ToString());
                urlCache = new UrlCache(__cacheDirectory);
                urlCache.UrlFileNameType = __urlFileNameType;
                urlCache.GetUrlSubDirectoryFunction = httpRequest => (Ebookdz_LoadPostDetailFromWebManager.GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
            }
            return urlCache;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static Ebookdz_LoadPostDetail CurrentLoadPostDetail { get { return __currentLoadPostDetail; } }
        public static Ebookdz_LoadPostDetailFromWebManager LoadPostDetailFromWebManager { get { return __loadPostDetailFromWebManager; } }

        protected override int GetKeyFromHttpRequest(HttpRequest httpRequest)
        {
            return Ebookdz_LoadPostDetailFromWebManager.GetPostDetailKey(httpRequest);
        }

        protected override HttpRequest GetDataSourceHttpRequest(IPost post)
        {
            return post.GetDataHttpRequest();
        }

        protected override void LoadImages(IPost post)
        {
            //post.SetImages(DownloadPrint.LoadImages(post.GetImages()));
            DownloadPrint.LoadImages(post);
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

        public void RefreshDocumentsStore(int limit = 100, bool reloadFromWeb = false, string query = null, string sort = null, bool loadImage = false)
        {
            int i = 1;
            RefreshDocumentsStore((post, newPost) =>
            {
                // si loadImage == false on récupère la liste des images du post initial
                if (!loadImage)
                    newPost.SetImages(post.GetImages());
                pb.Trace.WriteLine("post {0} {1:dd/MM/yyyy HH:mm:ss} images nb {2} links nb {3} title \"{4}\" url \"{5}\"", i++, newPost.GetPostCreationDate(), newPost.GetImages().Length, newPost.GetDownloadLinks().Length, newPost.GetTitle(), newPost.GetDataHttpRequest().Url);
            },
                limit, reloadFromWeb, query, sort, loadImage);
        }
    }
}
