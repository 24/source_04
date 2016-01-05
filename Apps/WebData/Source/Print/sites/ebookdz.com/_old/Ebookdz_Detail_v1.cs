using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MongoDB.Bson.Serialization.Attributes;
using pb;
using pb.Data;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using Print;
using Print.old;
using Download.Print.old;
using pb.Data.old;
using pb.Data.Mongo.old;
using pb.Web.Data.old;

// un post peut etre supprimer ex : http://www.ebookdz.com/forum/showthread.php?t=111454
// un post avec plusieurs parties ex : http://www.ebookdz.com/forum/showthread.php?t=66819


namespace Download.Print.Ebookdz.old
{
    //IKeyData<int>
    [BsonIgnoreExtraElements]
    public class Ebookdz_PostDetail_v1 : IPost, IKeyData_v4<int>, IHttpRequestData
    {
        public Ebookdz_PostDetail_v1()
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
        //public string Language;
        //public string Size;
        //public int? NbPages;
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
            return PostDownloadLinks.Create(DownloadLinks);
        }
    }

    public static class Ebookdz_DetailManager_v1
    {
        private static bool __trace = false;
        private static WebDataManager_v1<int, Ebookdz_PostDetail_v1> __detailWebDataManager = null;
        private static WebHeaderDetailManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader, int, Ebookdz_PostDetail_v1> __webHeaderDetailManager = null;
        private static WebHeaderDetailManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader, int, Ebookdz_PostDetail_v1> __webForumHeaderDetailManager = null;

        static Ebookdz_DetailManager_v1()
        {
            __detailWebDataManager = CreateWebDataManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/Detail"));

            __webHeaderDetailManager = new WebHeaderDetailManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader, int, Ebookdz_PostDetail_v1>();
            __webHeaderDetailManager.HeaderDataPageManager = Ebookdz_HeaderManager_v1.HeaderWebDataPageManager;
            __webHeaderDetailManager.DetailDataManager = __detailWebDataManager;

            __webForumHeaderDetailManager = new WebHeaderDetailManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader, int, Ebookdz_PostDetail_v1>();
            __webForumHeaderDetailManager.HeaderDataPageManager = Ebookdz_ForumHeaderManager_v1.CurrentForumHeaderManager;
            __webForumHeaderDetailManager.DetailDataManager = __detailWebDataManager;
        }

        public static bool Trace { get { return __trace; } set { __trace = value; } }
        public static WebDataManager_v1<int, Ebookdz_PostDetail_v1> DetailWebDataManager { get { return __detailWebDataManager; } }
        public static WebHeaderDetailManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader, int, Ebookdz_PostDetail_v1> WebHeaderDetailManager { get { return __webHeaderDetailManager; } }
        public static WebHeaderDetailManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader, int, Ebookdz_PostDetail_v1> WebForumHeaderDetailManager { get { return __webForumHeaderDetailManager; } }

        public static LoadNewDocumentsResult LoadForumNewDocuments(bool reloadForums = false, int maxNbDocumentsLoadedFromStore = 5, int maxPage = 20, bool loadImage = true, Predicate<Ebookdz_Forum_v1> filter = null)
        {
            LoadNewDocumentsResult result = new LoadNewDocumentsResult();
            foreach (Ebookdz_Forum_v1 forum in Ebookdz_MainForumManager_v1.CurrentMainForumManager.LoadSubForums(reload: reloadForums, filter: filter))
            {
                pb.Trace.WriteLine("load new documents from forum \"{0}/{1}\" - \"{2}\"", forum.Forum, forum.Category, forum.Name);
                LoadNewDocumentsResult resultForum = __webForumHeaderDetailManager.LoadNewDocuments(new HttpRequest { Url = forum.Url }, maxNbDocumentsLoadedFromStore: maxNbDocumentsLoadedFromStore, maxPage: maxPage, loadImage: loadImage);
                result.NbDocumentsLoadedFromWeb += resultForum.NbDocumentsLoadedFromWeb;
                result.NbDocumentsLoadedFromStore += resultForum.NbDocumentsLoadedFromStore;
            }
            return result;
        }

        private static WebDataManager_v1<int, Ebookdz_PostDetail_v1> CreateWebDataManager(XElement xe)
        {
            WebDataManager_v1<int, Ebookdz_PostDetail_v1> detailWebDataManager = new WebDataManager_v1<int, Ebookdz_PostDetail_v1>();

            detailWebDataManager.WebLoadDataManager = new WebLoadDataManager<Ebookdz_PostDetail_v1>();

            //if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            //{
            //    UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
            //    urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
            //    urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
            //    detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            //}
            UrlCache urlCache = UrlCache.Create(xe);
            if (urlCache != null)
            {
                urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
                detailWebDataManager.WebLoadDataManager.UrlCache = urlCache;
            }

            detailWebDataManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin_v1.InitLoadFromWeb;
            detailWebDataManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin_v1.GetHttpRequestParameters;
            detailWebDataManager.WebLoadDataManager.GetData = GetData;
            detailWebDataManager.GetKeyFromHttpRequest = GetPostDetailKey;
            detailWebDataManager.LoadImages = DownloadPrint_v1.LoadImages; // IPost

            //if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            //{
            //    MongoDocumentStore<int, Ebookdz_PostDetail> documentStore = new MongoDocumentStore<int, Ebookdz_PostDetail>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
            //    //documentStore.DefaultSort = "{ 'download.id': 1 }";
            //    documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
            //    detailWebDataManager.DocumentStore = documentStore;
            //}
            //documentStore.GetDataKey = headerPage => headerPage.GetKey();
            //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Ebookdz_HeaderPage>(document);
            detailWebDataManager.DocumentStore = MongoDocumentStore_v4<int, Ebookdz_PostDetail_v1>.Create(xe);

            return detailWebDataManager;
        }

        private static Ebookdz_PostDetail_v1 GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            Ebookdz_PostDetail_v1 data = new Ebookdz_PostDetail_v1();
            data.SourceUrl = webResult.WebRequest.HttpRequest.Url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPostDetailKey(webResult.WebRequest.HttpRequest);

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
            //pb.Trace.WriteLine("category \"{0}\" printType {1}", category, data.printType);

            // <div id="postlist" class="postlist restrain">
            XXElement xe = xePost.XPathElement(".//div[@id='postlist']");

            // Aujourd'hui, 07h32 - Aujourd'hui, 10h51 - Hier, 12h55 - 22/02/2014, 21h09
            //string date = xe.DescendantTextList(".//div[@class='posthead']//text()", nodeFilter: node => node.zGetName() != "a").zToStringValues("");
            XXElement xe2 = xe.XPathElement(".//div[@class='posthead']");
            //string date = xe2.DescendantTextList(nodeFilter: node => node.zGetName() != "a").zToStringValues("");
            string date = xe2.DescendantTexts(node => node.zGetName() != "a" ? XNodeFilter.SelectNode : XNodeFilter.SkipNode).zToStringValues("");
            date = date.Replace('\xA0', ' ');
            data.PostCreationDate = zdate.ParseDateTimeLikeToday(date, webResult.LoadFromWebDate, @"d/M/yyyy, HH\hmm", @"d-M-yyyy, HH\hmm");
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
            if (webResult.WebRequest.LoadImage)
                data.Images = DownloadPrint.LoadImages(data.Images).ToArray();

            // get infos, description, language, size, nbPages
            // xe.DescendantTextList(nodeFilter: node => !(node is XElement) || ((XElement)node).Name != "a")
            PrintTextValues textValues = DownloadPrint.PrintTextValuesManager.GetTextValues(xe.DescendantTexts(node => !(node is XElement) || ((XElement)node).Name != "a" ? XNodeFilter.SelectNode : XNodeFilter.SkipNode), data.Title);
            data.Description = textValues.description;
            //data.Language = textValues.language;
            //data.Size = textValues.size;
            //data.NbPages = textValues.nbPages;
            data.Infos.SetValues(textValues.infos);

            // modif pour avoir les liens de http://www.ebookdz.com/forum/showthread.php?t=113291
            //data.DownloadLinks = xe.XPathValues(".//a/@href");
            data.DownloadLinks = xePost.XPathElement(".//div[@class='postbody']//div[@class='content']//blockquote").XPathValues(".//a/@href").ToArray();

            if (__trace)
                pb.Trace.WriteLine(data.zToJson());

            return data;
        }

        private static PrintType GetPrintType(string category)
        {
            // Journaux/Presse quotidienne/Le Monde
            // Les Livres/Littérature & Romans/Nouveautés en Librairies/Nouveauté 2014
            // Les Livres/BD, Comics et Mangas/
            category = category.ToLower();
            if (category.StartsWith("journaux/") || category.StartsWith("magazines/"))
                return PrintType.Print;
            else if (category.StartsWith("les livres/bd, comics et mangas/"))
                return PrintType.Comics;
            else if (category.StartsWith("les livres/"))
                return PrintType.Book;
            else
                return PrintType.UnknowEBook;
            // return PrintType.Book;
            // return PrintType.Comics;
            // return PrintType.UnknowEBook;
            // return PrintType.Unknow;
        }

        private static Regex __postKeyRegex = new Regex(@"\?t=([0-9]+)$", RegexOptions.Compiled);
        private static int GetPostDetailKey(HttpRequest httpRequest)
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
}
