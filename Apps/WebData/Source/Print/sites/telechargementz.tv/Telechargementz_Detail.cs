using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MongoDB.Bson.Serialization;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web.old;
using pb.Web;
using Print;

namespace Download.Print.Telechargementz
{
    // IPostToDownload
    public class Telechargementz_PostDetail : IPost
    {
        public Telechargementz_PostDetail()
        {
            infos = new NamedValues<ZValue>();
        }

        //public string server { get { return "telechargementz.tv"; } }
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public string Title;
        public PrintType PrintType;
        public string OriginalTitle;
        public string PostAuthor;
        public DateTime? PostCreationDate;
        public string category { get; set; }
        public string[] description { get; set; }
        public string language { get; set; }
        public string size { get; set; }
        public int? nbPages { get; set; }
        public NamedValues<ZValue> infos { get; set; }
        //public List<UrlImage> images { get; set; }
        public WebImage[] Images;
        public string[] DownloadLinks;

        public string GetServer()
        {
            return "telechargementz.tv";
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
            return PostDownloadLinks.Create(DownloadLinks);
        }
    }

    //public class Telechargementz_LoadPostDetailFromWebManager : LoadDataFromWebManager<Telechargementz_PostDetail>
    public class Telechargementz_LoadPostDetailFromWebManager : LoadDataFromWebManager_v3<IPost>
    {
        private static bool __trace = false;

        public Telechargementz_LoadPostDetailFromWebManager(UrlCache_v1 urlCache = null)
            : base(urlCache)
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        //protected override Telechargementz_PostDetail GetDataFromWeb(LoadDataFromWeb loadDataFromWeb)
        protected override IPost GetDataFromWeb(LoadDataFromWeb_v3 loadDataFromWeb)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
            Telechargementz_PostDetail data = new Telechargementz_PostDetail();
            data.SourceUrl = loadDataFromWeb.request.Url;
            data.LoadFromWebDate = loadDataFromWeb.loadFromWebDate;
            data.Id = GetPostDetailKey(data.SourceUrl);

            XXElement xePost = xeSource.XPathElement("//div[@id='dle-content']");

            data.PostAuthor = xePost.XPathValue(".//div[@class='title-info']//a//text()");

            // , 26.12.14
            string date = xePost.XPathValue(".//div[@class='title-info']//a/following-sibling::text()");
            if (date != null)
            {
                data.PostCreationDate = zdate.ParseDateTimeLikeToday(date.Trim(' ', ','), loadDataFromWeb.loadFromWebDate, "dd.MM.yy");
                if (data.PostCreationDate == null)
                    pb.Trace.WriteLine("unknow date time \"{0}\"", date);
                if (__trace)
                    pb.Trace.WriteLine("creationDate {0} - \"{1}\"", data.PostCreationDate, date);
            }
            else
                pb.Trace.WriteLine("creationDate not found \"{0}\"", data.SourceUrl);

            //data.Title = xePost.XPathElement(".//div[@class='post-title']").DescendantTextList(func: DownloadPrint.TrimFunc1).FirstOrDefault();
            data.Title = xePost.XPathElement(".//div[@class='post-title']").DescendantTexts().Select(DownloadPrint.Trim).FirstOrDefault();
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.Title);
            if (titleInfos.foundInfo)
            {
                data.OriginalTitle = data.Title;
                data.Title = titleInfos.title;
                data.infos.SetValues(titleInfos.infos);
            }

            XXElement xe = xePost.XPathElement(".//div[starts-with(@id, 'news-id-')]");
            if (xe.XElement == null)
                pb.Trace.WriteLine("element not found \".//div[starts-with(@id, 'news-id-')]\"");

            //data.Images = new List<UrlImage>();
            //data.Images.Add(xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).FirstOrDefault());
            //data.Images = new UrlImage[] { xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).FirstOrDefault() };
            WebImage image = xe.DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).FirstOrDefault();
            if (image != null)
                data.Images = new WebImage[] { image };

            // force load image to get image width and height
            if (loadDataFromWeb.request.LoadImage)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            data.DownloadLinks = xe.XPathValues(".//a/@href").ToArray();

            //data.category = xePost.DescendantTextList(".//div[@class='hdiin']//a").Select(DownloadPrint.TrimFunc1).zToStringValues("/");
            //string category = data.category.ToLowerInvariant();
            //data.printType = GetPrintType(category);
            ////pb.Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);


            // get infos, description, language, size, nbPages
            // nodeFilter: not <a> and not <span>
            //   nodeFilter: node => !(node is XElement) || (((XElement)node).Name != "a" && ((XElement)node).Name != "span")
            // nodeFilter: not <a>
            //PrintTextValues_old textValues = DownloadPrint.PrintTextValuesManager.GetTextValues_old(xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a"), data.Title);
            PrintTextValues_v1 textValues = DownloadPrint.PrintTextValuesManager.GetTextValues_v1(xe.DescendantTexts(node => !(node is XElement) || ((XElement)node).Name != "a" ? XNodeFilter.SelectNode : XNodeFilter.SkipNode), data.Title);
            data.description = textValues.description;
            data.language = textValues.language;
            data.size = textValues.size;
            data.nbPages = textValues.nbPages;
            data.infos.SetValues(textValues.infos);

            data.PrintType = PrintType.UnknowEBook;
            if (data.infos.ContainsKey("Bd") || data.infos.ContainsKey("bd") || data.infos.ContainsKey("BD"))
                data.PrintType = PrintType.Comics;
            // Editeur : Presse fr
            else if (data.infos.ContainsKey("editeur") && data.infos["editeur"] is ZString && ((string)data.infos["editeur"]).ToLowerInvariant() == "presse fr")
                data.PrintType = PrintType.Print;
            else if (data.infos.ContainsKey("isbn"))
                data.PrintType = PrintType.Book;


            //pb.Trace.WriteLine(xe.DescendantNodes(returnNodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a").Select(node => new { type = node.NodeType, name = node is XElement ? ((XElement)node).Name.LocalName : null, value = node is XText ? ((XText)node).Value : null }).zToJson());
            //pb.Trace.WriteLine(xe.DescendantNodes(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a").Select(node => new { type = node.NodeType, name = node is XElement ? ((XElement)node).Name.LocalName : null, value = node is XText ? ((XText)node).Value : null }).zToJson());
            //pb.Trace.WriteLine(xe.DescendantNodes(returnNodeFilter: node => node is XText).Select(node => new { type = node.NodeType, name = node is XElement ? ((XElement)node).Name.LocalName : null, value = node is XText ? ((XText)node).Value : null }).zToJson());
            //pb.Trace.WriteLine(xe.DescendantNodes(nodeFilter: node => !(node is XElement) || (((XElement)node).Name != "a" && ((XElement)node).Name != "span"), returnNodeFilter: node => node is XText).Select(node => new { type = node.NodeType, name = node is XElement ? ((XElement)node).Name.LocalName : null, value = node is XText ? ((XText)node).Value : null }).zToJson());

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        //private static PrintType GetPrintType(string category)
        //{
        //    // category : "Ebooks", "Ebooks/Bandes Dessinée", "Ebooks/Journaux", "Ebooks/Livres", "Ebooks/Magazine", "Magazines/Journaux"
        //    switch (category.ToLower())
        //    {
        //        case "ebooks/journaux":
        //        case "ebooks/magazines":
        //        case "magazines/journaux":
        //            return PrintType.Print;
        //        case "ebooks/livres":
        //            return PrintType.Book;
        //        case "ebooks/bandes dessinée":
        //            return PrintType.Comics;
        //        case "ebooks":
        //            return PrintType.UnknowEBook;
        //        default:
        //            return PrintType.Unknow;
        //    }
        //}

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        public static int GetPostDetailKey(string url)
        {
            // http://telechargementz.tv/ebooks/6484-alternatives-internationales-hors-srie-no16-janvier-2015.html
            Uri uri = new Uri(url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(file);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", url);
            return int.Parse(match.Value);
        }
    }

    //public class Telechargementz_LoadPostDetail : HeaderDetailWebDocumentStore<int, Telechargementz_HeaderPage, Telechargementz_PostHeader, int, Telechargementz_PostDetail>
    public class Telechargementz_LoadPostDetail : HeaderDetailWebDocumentStore_v2<int, int, IPost>
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

        private static Telechargementz_LoadPostDetail __currentLoadPostDetail = new Telechargementz_LoadPostDetail();
        private static Telechargementz_LoadPostDetailFromWebManager __loadPostDetailFromWebManager = null;

        static Telechargementz_LoadPostDetail()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("Telechargementz/Detail"));
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
                //__currentLoadPostDetail._documentStore = new MongoDocumentStore_new<int, Telechargementz_PostDetail>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                MongoDocumentStore_v3<int, IPost> mongoDocumentStore = new MongoDocumentStore_v3<int, IPost>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                mongoDocumentStore.DefaultSort = "{ 'download.creationDate': -1 }";
                mongoDocumentStore.GetDataKey = post => post.GetKey();
                mongoDocumentStore.Deserialize = document => BsonSerializer.Deserialize<Telechargementz_PostDetail>(document);
                __currentLoadPostDetail._documentStore = mongoDocumentStore;
            }

            __loadPostDetailFromWebManager = new Telechargementz_LoadPostDetailFromWebManager(GetUrlCache());
            //__currentLoadPostDetail._loadWebDataManager = new LoadWebDataManager_new<int, Telechargementz_PostDetail>(new Telechargementz_LoadPostDetailFromWebManager(GetUrlCache()), __currentLoadPostDetail._documentStore);
            __currentLoadPostDetail._loadWebDataManager = new LoadWebDataManager_v4<int, IPost>(__loadPostDetailFromWebManager, __currentLoadPostDetail._documentStore);
            __currentLoadPostDetail._loadEnumDataPagesFromWeb = Telechargementz_LoadHeaderPagesManager.CurrentLoadHeaderPagesManager;
        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType, (url, requestParameters) => (Telechargementz_LoadPostDetailFromWebManager.GetPostDetailKey(url) / 1000 * 1000).ToString());
            return urlCache;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static Telechargementz_LoadPostDetail CurrentLoadPostDetail { get { return __currentLoadPostDetail; } }
        public static Telechargementz_LoadPostDetailFromWebManager LoadPostDetailFromWebManager { get { return __loadPostDetailFromWebManager; } }

        protected override int GetKeyFromUrl(string url)
        {
            return Telechargementz_LoadPostDetailFromWebManager.GetPostDetailKey(url);
        }

        protected override string GetDataSourceUrl(IPost post)
        {
            return post.GetDataHttpRequest().Url;
        }

        //protected override void LoadImages(Telechargementz_PostDetail post)
        protected override void LoadImages(IPost post)
        {
            //post.SetImages(DownloadPrint.LoadImages(post.GetImages()));
            DownloadPrint.LoadImages(post);
        }

        protected override HttpRequestParameters_v1 GetHttpRequestParameters()
        {
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.encoding = Encoding.Default;
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
                        //Telechargementz_PostDetail post = loadWebData.Document;
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
                        //newPost.images = post.images;
                        newPost.SetImages(post.GetImages());
                    pb.Trace.WriteLine("post {0} {1:dd/MM/yyyy HH:mm:ss} images nb {2} links nb {3} title \"{4}\" url \"{5}\"", i++, newPost.GetPostCreationDate(), newPost.GetImages().Length, newPost.GetDownloadLinks_new().LinksCount, newPost.GetTitle(), newPost.GetDataHttpRequest().Url);
                },
                limit, reloadFromWeb, query, sort, loadImage);
        }
    }
}
