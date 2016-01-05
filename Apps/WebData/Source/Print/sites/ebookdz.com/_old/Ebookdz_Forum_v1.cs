using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using pb.Data.Mongo.old;
using pb.Web.Data.old;

namespace Download.Print.Ebookdz.old
{
    public class Ebookdz_Forum_v1
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Forum;
        public string Category;
        public string Name;
        public string Url;
    }

    public class Ebookdz_ForumPage_v1 : IEnumDataPages<Ebookdz_Forum_v1>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public Ebookdz_Forum_v1[] Forums;
        public string UrlNextPage;

        public IEnumerable<Ebookdz_Forum_v1> GetDataList()
        {
            return Forums;
        }

        public HttpRequest GetHttpRequestNextPage()
        {
            if (UrlNextPage != null)
                return new HttpRequest { Url = UrlNextPage };
            else
                return null;
        }
    }

    public class Ebookdz_MainForumManager_v1 : WebDataPageManager_v1<int, Ebookdz_ForumPage_v1, Ebookdz_Forum_v1>
    {
        private static string __urlForum = "http://www.ebookdz.com/forum/forum.php";
        private static Ebookdz_MainForumManager_v1 __currentMainForumManager = null;
        private static Predicate<string> __forumFilter = name => { name = name.ToLowerInvariant(); return name == "journaux" || name == "magazines" || name == "les livres"; };

        static Ebookdz_MainForumManager_v1()
        {
            __currentMainForumManager = CreateMainForumManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/Forum"));
        }

        public static Ebookdz_MainForumManager_v1 CurrentMainForumManager { get { return __currentMainForumManager; } }

        public IEnumerable<Ebookdz_Forum_v1> LoadMainForum(bool reload = false)
        {
            return LoadPages(startPage: 1, maxPage: 1, reload: reload, loadImage: false, refreshDocumentStore: false);
        }

        public IEnumerable<Ebookdz_Forum_v1> LoadSubForums(bool reload = false, Predicate<Ebookdz_Forum_v1> filter = null)
        {
            foreach (Ebookdz_Forum_v1 mainForum in LoadPages(startPage: 1, maxPage: 1, reload: reload, loadImage: false, refreshDocumentStore: false))
            {
                //yield return mainForum;
                foreach (Ebookdz_Forum_v1 subForum in Ebookdz_SubForumManager_v1.CurrentSubForumManager.LoadPages(new HttpRequest { Url = mainForum.Url }, maxPage: 0, reload: reload))
                {
                    subForum.Forum = mainForum.Name;
                    if (filter == null || filter(subForum))
                        yield return subForum;
                }
            }
        }

        private static Ebookdz_MainForumManager_v1 CreateMainForumManager(XElement xe)
        {
            //WebDataPageManager<int, Ebookdz_ForumPage, Ebookdz_Forum> forumWebDataPageManager = new WebDataPageManager<int, Ebookdz_ForumPage, Ebookdz_Forum>();
            Ebookdz_MainForumManager_v1 mainForumManager = new Ebookdz_MainForumManager_v1();

            mainForumManager.WebLoadDataManager = new WebLoadDataManager<Ebookdz_ForumPage_v1>();
            //
            //if (xe.zXPathValueBool("UseUrlCache", false))
            if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            {
                UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
                urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
                //urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
                mainForumManager.WebLoadDataManager.UrlCache = urlCache;
            }
            mainForumManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin_v1.InitLoadFromWeb;
            mainForumManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin_v1.GetHttpRequestParameters;
            mainForumManager.WebLoadDataManager.GetData = GetData;
            //detailWebDataManager.GetKeyFromHttpRequest = GetPostDetailKey;
            //detailWebDataManager.LoadImages = DownloadPrint.LoadImages; // IPost

            //if (xe.zXPathValueBool("UseMongo", false))
            if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            {
                MongoDocumentStore_v4<int, Ebookdz_ForumPage_v1> documentStore = new MongoDocumentStore_v4<int, Ebookdz_ForumPage_v1>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
                //documentStore.DefaultSort = "{ 'download.id': 1 }";
                documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
                //documentStore.GetDataKey = headerPage => headerPage.GetKey();
                //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Ebookdz_HeaderPage>(document);
                mainForumManager.DocumentStore = documentStore;
            }
            mainForumManager.GetHttpRequestPage = _GetHttpRequestPage;
            return mainForumManager;
        }

        private static Ebookdz_ForumPage_v1 GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Ebookdz_ForumPage_v1 data = new Ebookdz_ForumPage_v1();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = _GetPageKey(webResult.WebRequest.HttpRequest);

            //data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='basenavi']//span[@class='nnext']//a/@href"));
            data.UrlNextPage = null;

            Predicate<string> filter = __forumFilter;
            List<Ebookdz_Forum_v1> forums = new List<Ebookdz_Forum_v1>();
            //HtmlRun.Select("//ol[@id='forums']/li:.:EmptyRow", ".//text()", ".//a//text()", ".//a/@href");
            // <ol id="forums" class="floatcontainer">
            foreach (XXElement xe in xeSource.XPathElements("//ol[@id='forums']/li"))
            {
                // Accueil de la Board, Forum de l'entraide, Journaux, MAGAZINES, Les Livres, Sujet supprimés ou à supprimer
                // http://www.ebookdz.com/forum/forumdisplay.php?f=1&s=1fdf76d35a57d09aa11e75ff6f0d9985
                XXElement xe2 = xe.XPathElement(".//a");

                string name = xe2.XPathValue(".//text()");

                if (filter != null && !filter(name))
                    continue;

                Ebookdz_Forum_v1 forum = new Ebookdz_Forum_v1();
                forum.SourceUrl = url;
                forum.LoadFromWebDate = webResult.LoadFromWebDate;
                forum.Name = name;

                forum.Url = Ebookdz_v1.GetUrl(zurl.GetUrl(url, xe2.XPathValue("@href")));
                forums.Add(forum);
            }

            data.Forums = forums.ToArray();
            return data;
        }

        private static int _GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.ebookdz.com/
            // page 2 : no pagination
            if (httpRequest.Url == __urlForum)
                return 1;
            //Uri uri = new Uri(url);
            //string lastSegment = uri.Segments[uri.Segments.Length - 1];
            //lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
            //int page;
            //if (!int.TryParse(lastSegment, out page))
            //    throw new PBException("header page key not found in url \"{0}\"", url);
            //return page;
            throw new PBException("forum page key not found in url \"{0}\"", httpRequest.Url);
        }

        private static HttpRequest _GetHttpRequestPage(int page)
        {
            // no pagination
            if (page != 1)
                throw new PBException("error wrong page number {0}", page);
            return new HttpRequest { Url = __urlForum };
        }
    }

    public class Ebookdz_SubForumManager_v1 : WebDataPageManager_v1<int, Ebookdz_ForumPage_v1, Ebookdz_Forum_v1>
    {
        private static Ebookdz_SubForumManager_v1 __currentSubForumManager = null;

        static Ebookdz_SubForumManager_v1()
        {
            __currentSubForumManager = CreateSubForumManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/Forum"));
        }

        public static Ebookdz_SubForumManager_v1 CurrentSubForumManager { get { return __currentSubForumManager; } }

        private static Ebookdz_SubForumManager_v1 CreateSubForumManager(XElement xe)
        {
            //WebDataPageManager<int, Ebookdz_ForumPage, Ebookdz_Forum> forumWebDataPageManager = new WebDataPageManager<int, Ebookdz_ForumPage, Ebookdz_Forum>();
            Ebookdz_SubForumManager_v1 subForumManager = new Ebookdz_SubForumManager_v1();

            subForumManager.WebLoadDataManager = new WebLoadDataManager<Ebookdz_ForumPage_v1>();
            //if (xe.zXPathValueBool("UseUrlCache", false))
            if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            {
                UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
                urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
                //urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
                subForumManager.WebLoadDataManager.UrlCache = urlCache;
            }
            subForumManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin_v1.InitLoadFromWeb;
            subForumManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin_v1.GetHttpRequestParameters;
            subForumManager.WebLoadDataManager.GetData = GetData;
            //detailWebDataManager.GetKeyFromHttpRequest = GetPostDetailKey;
            //detailWebDataManager.LoadImages = DownloadPrint.LoadImages; // IPost

            //if (xe.zXPathValueBool("UseMongo", false))
            if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            {
                MongoDocumentStore_v4<int, Ebookdz_ForumPage_v1> documentStore = new MongoDocumentStore_v4<int, Ebookdz_ForumPage_v1>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
                //documentStore.DefaultSort = "{ 'download.id': 1 }";
                documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
                //documentStore.GetDataKey = headerPage => headerPage.GetKey();
                //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Ebookdz_HeaderPage>(document);
                subForumManager.DocumentStore = documentStore;
            }
            //subForumManager.GetHttpRequestPage = _GetHttpRequestPage;
            return subForumManager;
        }

        private static Ebookdz_ForumPage_v1 GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Ebookdz_ForumPage_v1 data = new Ebookdz_ForumPage_v1();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            //data.Id = _GetPageKey(webResult.WebRequest.HttpRequest);

            //data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='basenavi']//span[@class='nnext']//a/@href"));
            data.UrlNextPage = null;

            List<Ebookdz_Forum_v1> forums = new List<Ebookdz_Forum_v1>();

            Predicate<string> filter = null;

            // <div class="body_bd">
            // <div id="forumbits" class="forumbits">
            // <ol>
            // <li id="forum10" class="forumbit_post new L1">
            //   <div class="forumrow">
            //   <ol id="childforum_for_161" class="childsubforum">
            //     <div class="titleline">
            foreach (XXElement xe in xeSource.XPathElements("//div[@id='forumbits']/ol/li"))
            {
                XXElement xe2 = xe.XPathElement(".//div[@class='forumrow']//a");
                string category = xe2.XPathValue(".//text()");
                string urlCategory = Ebookdz_v1.GetUrl(zurl.GetUrl(url, xe2.XPathValue("@href")));
                // Forum = forum
                //forums.Add(new Ebookdz_Forum { Category = category, Url = urlCategory });

                foreach (XXElement xe3 in xe.XPathElements(".//ol[@class='childsubforum']/li//div[@class='titleline']//a"))
                {
                    string name = xe3.XPathValue(".//text()");

                    if (filter != null && !filter(name))
                        continue;
                    string urlSubForum = Ebookdz_v1.GetUrl(zurl.GetUrl(url, xe3.XPathValue("@href")));
                    // Forum = forum
                    forums.Add(new Ebookdz_Forum_v1 { SourceUrl = url, LoadFromWebDate = webResult.LoadFromWebDate, Category = category, Name = name, Url = urlSubForum });
                }
            }

            data.Forums = forums.ToArray();
            return data;
        }
    }
}
