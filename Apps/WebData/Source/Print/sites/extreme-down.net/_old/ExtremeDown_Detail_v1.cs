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
using Print;

// http://www.extreme-down.net/ebooks/journaux/23357-les-journaux-lundi-08-dcembre-2014.html
// http://www.extreme-down.net/ebooks/journaux/23375-les-journaux-mardi-09-dcembre-2014.html
// post avec plusieurs journaux http://www.extreme-down.net/ebooks/magazines/14546-science-vie-n1153-octobre-2013-.html
//   Science & Vie N°1167 - Decembre 2014
//   Science & Vie Hors Série No.269 - Décembre 2014
//   Science & Vie Guerres & Histoire No.22 - Décembre 2014
//   Science & vie Hors-série Spécial No.39 - 2014
// post http://www.extreme-down.net/musique/rock/23312-eric-clapton-the-platinum-collection.html
//   taille 1.13 Go 2 liens 
// post http://www.extreme-down.net/musique/rock/22179-jeff-beck-albums-collection-9-albums-10cd-2013-2014.html
//   taille 1.5 Go 2 liens 

namespace Download.Print.ExtremeDown
{
    // IPostToDownload
    public class ExtremeDown_PostDetail : IPost
    {
        public ExtremeDown_PostDetail()
        {
            infos = new NamedValues<ZValue>();
            DownloadLinks_new = new PostDownloadLinks();
        }

        //public string server { get { return "extreme-down.net"; } }
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
        public PostDownloadLinks DownloadLinks_new;

        public string GetServer()
        {
            return "extreme-down.net";
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
            return null;
        }

        public PostDownloadLinks GetDownloadLinks_new()
        {
            return DownloadLinks_new;
        }
    }

    //public class ExtremeDown_LoadPostDetailFromWebManager : LoadDataFromWebManager<ExtremeDown_PostDetail>
    public class ExtremeDown_LoadPostDetailFromWebManager : LoadDataFromWebManager_v3<IPost>
    {
        private static bool __trace = false;
        private static bool __getLinksExtremeProtect = false;
        private static ExtremeProtect __extremeProtect = new ExtremeProtect();

        public ExtremeDown_LoadPostDetailFromWebManager(UrlCache_v1 urlCache = null)
            : base(urlCache)
        {
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static bool GetLinksExtremeProtect { get { return __getLinksExtremeProtect; } set { __getLinksExtremeProtect = value; } }

        //protected override ExtremeDown_PostDetail GetDataFromWeb(LoadDataFromWeb loadDataFromWeb)
        protected override IPost GetDataFromWeb(LoadDataFromWeb_v3 loadDataFromWeb)
        {
            XXElement xeSource = new XXElement(loadDataFromWeb.GetXmlDocument().Root);
            ExtremeDown_PostDetail data = new ExtremeDown_PostDetail();
            data.SourceUrl = loadDataFromWeb.request.Url;
            data.LoadFromWebDate = loadDataFromWeb.loadFromWebDate;
            data.Id = GetPostDetailKey(data.SourceUrl);

            XXElement xePost = xeSource.XPathElement("//div[@id='dle-content']");

            //data.Title = xePost.XPathValue(".//h2[@class='blocktitle']//text()", DownloadPrint.Trim);
            data.Title = xePost.XPathValue(".//h2[@class='blocktitle']//text()").Trim(DownloadPrint.TrimChars);
            PrintTitleInfos titleInfos = DownloadPrint.PrintTextValuesManager.ExtractTitleInfos(data.Title);
            if (titleInfos.foundInfo)
            {
                data.OriginalTitle = data.Title;
                data.Title = titleInfos.title;
                data.infos.SetValues(titleInfos.infos);
            }

            XXElement xeDiv = xePost.XPathElement(".//div[@class='blockheader']");

            data.PostAuthor = xeDiv.XPathValue(".//span/i[@class='icon-user']/ancestor::span//a//text()");

            string date = xeDiv.XPathValue(".//span/i[@class='icon-date']/ancestor::span//a//text()");
            data.PostCreationDate = zdate.ParseDateTimeLikeToday(date, loadDataFromWeb.loadFromWebDate, "d-M-yyyy, HH:mm", "d M yyyy", "d MMMM yyyy");
            if (data.PostCreationDate == null)
                pb.Trace.WriteLine("unknow date time \"{0}\"", date);
            if (__trace)
                pb.Trace.WriteLine("creationDate {0} - \"{1}\"", data.PostCreationDate, date);

            xeDiv = xePost.XPathElement(".//div[@class='blockcontent']");

            List<string> description = new List<string>();
            description.AddRange(xeDiv.XPathValues(".//p[@class='release-name']//text()"));

            //data.Images = xeDiv.XPathElement(".//table//td[@class='image-block']").XPathImages(xeImg => new UrlImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).ToArray();
            data.Images = xeDiv.XPathElement(".//table//td[@class='image-block']").DescendantNodes(node => XmlDescendant.ImageFilter(node)).Select(xeImg => new WebImage(zurl.GetUrl(loadDataFromWeb.request.Url, xeImg.zAttribValue("src")))).ToArray();

            // force load image to get image width and height
            if (loadDataFromWeb.request.LoadImage)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            description.AddRange(xeDiv.XPathValues(".//table//td//blockquote//text()"));

            xeDiv = xePost.XPathElement(".//div[@class='clearfix']");
            description.AddRange(xeDiv.XPathValues(".//table//text()"));

            data.description = description.ToArray();

            string title = null;
            //foreach (XXElement xe in xePost.XPathElements(".//h2").Skip(1))
            foreach (XXElement xe in xePost.XPathElements(".//script/parent::div//following-sibling::h2"))
            {
                string s = xe.XPathValue(".//text()");
                // Liens de téléchargement - Pack 1
                if (s.StartsWith("Liens de téléchargement"))
                {
                    s = s.Substring(23).Trim(' ', '-');
                    if (s == "")
                        s = title;
                    else if (title != null)
                        s = title + " - " + s;
                    title = null;
                    data.DownloadLinks_new.AddItem(s);
                    foreach (XXElement xe2 in xe.XPathElements("following-sibling::div[1]//a"))
                    {
                        //s = xe2.DescendantTextList().FirstOrDefault();
                        s = xe2.DescendantTexts().FirstOrDefault();
                        string link = xe2.XPathValue("@href");
                        if (__getLinksExtremeProtect && __extremeProtect.IsLinkProtected(link))
                        {
                            data.DownloadLinks_new.AddServer(s, link);
                            data.DownloadLinks_new.AddLinks(__extremeProtect.UnprotectLink(link));
                        }
                        else
                        {
                            data.DownloadLinks_new.AddServer(s);
                            data.DownloadLinks_new.AddLink(link);
                        }
                    }
                }
                else if (s != null)
                    title = s;
            }

            xeDiv = xePost.XPathElement(".//div[@class='blockfooter links']");
            //data.category = xeDiv.DescendantTextList(".//i[@class='icon-cats']/parent::span//a").Select(DownloadPrint.TrimFunc1).zToStringValues("/");
            data.category = xeDiv.XPathElements(".//i[@class='icon-cats']/parent::span//a").DescendantTexts().Select(DownloadPrint.Trim).zToStringValues("/");
            string category = data.category.ToLowerInvariant();
            data.PrintType = GetPrintType(category);
            //pb.Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        private static PrintType GetPrintType(string category)
        {
            switch (category.ToLower())
            {
                case "ebooks/journaux":
                case "ebooks/magazines":
                    return PrintType.Print;
                //case "ebooks/livres":
                //    return PrintType.Book;
                //case "ebooks/bandes dessinée":
                //    return PrintType.Comics;
                //case "ebooks":
                //    return PrintType.UnknowEBook;
                default:
                    return PrintType.Unknow;
            }
        }

        private static Regex __postKeyRegex = new Regex(@"^[0-9]+", RegexOptions.Compiled);
        public static int GetPostDetailKey(string url)
        {
            // http://www.extreme-down.net/ebooks/journaux/23375-les-journaux-mardi-09-dcembre-2014.html
            Uri uri = new Uri(url);
            string file = uri.Segments[uri.Segments.Length - 1];
            Match match = __postKeyRegex.Match(file);
            if (!match.Success)
                throw new PBException("post key not found in url \"{0}\"", url);
            return int.Parse(match.Value);
        }
    }

    //public class ExtremeDown_LoadPostDetail : HeaderDetailWebDocumentStore<int, ExtremeDown_HeaderPage, ExtremeDown_PostHeader, int, ExtremeDown_PostDetail>
    public class ExtremeDown_LoadPostDetail : HeaderDetailWebDocumentStore_v2<int, int, IPost>
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

        private static ExtremeDown_LoadPostDetail __currentLoadPostDetail = new ExtremeDown_LoadPostDetail();
        private static ExtremeDown_LoadPostDetailFromWebManager __loadPostDetailFromWebManager = null;

        static ExtremeDown_LoadPostDetail()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("ExtremeDown/Detail"));
        }

        public static void ClassInit(XElement xe)
        {
            //
            //__useUrlCache = xe.zXPathValueBool("UseUrlCache", false);
            __useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            __cacheDirectory = xe.zXPathValue("CacheDirectory");

            __useMongo = xe.zXPathValue("UseMongo").zTryParseAs(__useMongo);
            __mongoServer = xe.zXPathValue("MongoServer", __mongoServer);
            __mongoDatabase = xe.zXPathValue("MongoDatabase");
            __mongoCollectionName = xe.zXPathValue("MongoCollection");
            __mongoDocumentItemName = xe.zXPathValue("MongoDocumentItemName");

            if (__useMongo)
            {
                //__currentLoadPostDetail._documentStore = new MongoDocumentStore_new<int, ExtremeDown_PostDetail>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                MongoDocumentStore_v3<int, IPost> mongoDocumentStore = new MongoDocumentStore_v3<int, IPost>(__mongoServer, __mongoDatabase, __mongoCollectionName, __mongoDocumentItemName);
                mongoDocumentStore.DefaultSort = "{ 'download.creationDate': -1 }";
                mongoDocumentStore.GetDataKey = post => post.GetKey();
                mongoDocumentStore.Deserialize = document => BsonSerializer.Deserialize<ExtremeDown_PostDetail>(document);
                __currentLoadPostDetail._documentStore = mongoDocumentStore;
            }

            //ExtremeDown_LoadPostDetailFromWebManager.GetLinksExtremeProtect = xe.zXPathValueBool("GetLinksExtremeProtect", ExtremeDown_LoadPostDetailFromWebManager.GetLinksExtremeProtect);
            ExtremeDown_LoadPostDetailFromWebManager.GetLinksExtremeProtect = xe.zXPathValue("GetLinksExtremeProtect").zTryParseAs(ExtremeDown_LoadPostDetailFromWebManager.GetLinksExtremeProtect);
            __loadPostDetailFromWebManager = new ExtremeDown_LoadPostDetailFromWebManager(GetUrlCache());
            //__currentLoadPostDetail._loadWebDataManager = new LoadWebDataManager_new<int, ExtremeDown_PostDetail>(__loadPostDetailFromWebManager, __currentLoadPostDetail._documentStore);
            __currentLoadPostDetail._loadWebDataManager = new LoadWebDataManager_v4<int, IPost>(__loadPostDetailFromWebManager, __currentLoadPostDetail._documentStore);
            __currentLoadPostDetail._loadEnumDataPagesFromWeb = ExtremeDown_LoadHeaderPagesManager.CurrentLoadHeaderPagesManager;
        }

        public static UrlCache_v1 GetUrlCache()
        {
            UrlCache_v1 urlCache = null;
            if (__useUrlCache)
                urlCache = new UrlCache_v1(__cacheDirectory, __urlFileNameType, (url, requestParameters) => (ExtremeDown_LoadPostDetailFromWebManager.GetPostDetailKey(url) / 1000 * 1000).ToString());
            return urlCache;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static ExtremeDown_LoadPostDetail CurrentLoadPostDetail { get { return __currentLoadPostDetail; } }
        public static ExtremeDown_LoadPostDetailFromWebManager LoadPostDetailFromWebManager { get { return __loadPostDetailFromWebManager; } }

        protected override int GetKeyFromUrl(string url)
        {
            return ExtremeDown_LoadPostDetailFromWebManager.GetPostDetailKey(url);
        }

        protected override string GetDataSourceUrl(IPost post)
        {
            return post.GetDataHttpRequest().Url;
        }

        protected override void LoadImages(IPost post)
        {
            //DownloadPrint.LoadImages(post.images);
            //post.SetImages(DownloadPrint.LoadImages_new(post.GetImages()).ToArray());
            DownloadPrint.LoadImages(post);
        }

        protected override HttpRequestParameters_v1 GetHttpRequestParameters()
        {
            HttpRequestParameters_v1 requestParameters = new HttpRequestParameters_v1();
            requestParameters.encoding = Encoding.UTF8;
            return requestParameters;
        }

        public void LoadNewDocuments(int maxNbDocumentLoadedFromStore = 10, int startPage = 1, int maxPage = 20, bool loadImage = true)
        {
            int nbDocumentLoadedFromStore = 0;
            base.LoadNewDocuments(maxNbDocumentLoadedFromStore, startPage, maxPage, loadImage,
                loadWebData =>
                {
                    if (__trace)
                    {
                        if (loadWebData.DocumentLoadedFromStore)
                            nbDocumentLoadedFromStore++;
                        //ExtremeDown_PostDetail post = loadWebData.Document;
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
