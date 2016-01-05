using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace Download.Print.Ebookdz.old
{
    public class Ebookdz_Forum_v2 : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Forum;
        public string Category;
        public string Name;
        public string Url;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = Url };
        }
    }

    public class Ebookdz_ForumPage_v2 : IEnumDataPages<IHeaderData>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public Ebookdz_Forum_v2[] Forums;
        public string UrlNextPage;

        public IEnumerable<IHeaderData> GetDataList()
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

    //public class Ebookdz_MainForumManager_v2 : WebDataPageManager<int, Ebookdz_ForumPage, Ebookdz_Forum>
    public static class Ebookdz_MainForumManager_v2
    {
        private static string __urlForum = "http://www.ebookdz.com/forum/forum.php";
        private static WebDataPageManager<IHeaderData> __headerWebDataPageManager = null;
        private static Predicate<string> __forumFilter = name => { name = name.ToLowerInvariant(); return name == "journaux" || name == "magazines" || name == "les livres"; };

        static Ebookdz_MainForumManager_v2()
        {
            Ebookdz_v2.FakeInit();
        }

        public static void Init(XElement xe)
        {
            //__currentMainForumManager = CreateMainForumManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/Forum"));
            __headerWebDataPageManager = CreateMainForumManager(xe.zXPathElement("Forum"));
        }

        public static WebDataPageManager<IHeaderData> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        //public IEnumerable<Ebookdz_Forum> LoadMainForum(bool reload = false)
        public static IEnumerable<IHeaderData> LoadMainForum(bool reload = false)
        {
            return __headerWebDataPageManager.LoadPages(startPage: 1, maxPage: 1, reload: reload, loadImage: false, refreshDocumentStore: false);
        }

        public static IEnumerable<Ebookdz_Forum_v2> LoadSubForums(bool reload = false, Predicate<Ebookdz_Forum_v2> filter = null)
        {
            //foreach (Ebookdz_Forum mainForum in LoadPages(startPage: 1, maxPage: 1, reload: reload, loadImage: false, refreshDocumentStore: false))
            foreach (IHeaderData header in __headerWebDataPageManager.LoadPages(startPage: 1, maxPage: 1, reload: reload, loadImage: false, refreshDocumentStore: false))
            {
                Ebookdz_Forum_v2 mainForum = (Ebookdz_Forum_v2)header;
                //yield return mainForum;
                //foreach (Ebookdz_Forum_v2 subForum in Ebookdz_SubForumManager.CurrentSubForumManager.LoadPages(new HttpRequest { Url = mainForum.Url }, maxPage: 0, reload: reload))
                foreach (Ebookdz_Forum_v2 subForum in Ebookdz_SubForumManager_v2.HeaderWebDataPageManager.LoadPages(new HttpRequest { Url = mainForum.Url }, maxPage: 0, reload: reload))
                {
                    subForum.Forum = mainForum.Name;
                    if (filter == null || filter(subForum))
                        yield return subForum;
                }
            }
        }

        //private static Ebookdz_MainForumManager_v2 CreateMainForumManager(XElement xe)
        private static WebDataPageManager<IHeaderData> CreateMainForumManager(XElement xe)
        {
            //Ebookdz_MainForumManager_v2 mainForumManager = new Ebookdz_MainForumManager_v2();
            WebDataPageManager<IHeaderData> webDataPageManager = new WebDataPageManager<IHeaderData>();

            //mainForumManager.WebLoadDataManager = new WebLoadDataManager<Ebookdz_ForumPage>();
            //if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            //{
            //    UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
            //    urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
            //    mainForumManager.WebLoadDataManager.UrlCache = urlCache;
            //}
            webDataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<IHeaderData>>();
            webDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);
            webDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin_v2.InitLoadFromWeb;
            webDataPageManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin_v2.GetHttpRequestParameters;
            webDataPageManager.WebLoadDataManager.GetData = GetData;

            //if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            //{
            //    MongoDocumentStore<int, Ebookdz_ForumPage> documentStore = new MongoDocumentStore<int, Ebookdz_ForumPage>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
            //    documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
            //    webDataPageManager.DocumentStore = documentStore;
            //}
            webDataPageManager.DocumentStore = MongoDocumentStore<IEnumDataPages<IHeaderData>>.Create(xe);
            if (webDataPageManager.DocumentStore != null)
                webDataPageManager.DocumentStore.NominalType = typeof(Ebookdz_ForumPage_v2);

            webDataPageManager.GetHttpRequestPageFunction = GetHttpRequestPage;
            return webDataPageManager;
        }

        //private static Ebookdz_ForumPage GetData(WebResult webResult)
        private static IEnumDataPages<IHeaderData> GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Ebookdz_ForumPage_v2 data = new Ebookdz_ForumPage_v2();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = _GetPageKey(webResult.WebRequest.HttpRequest);

            //data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='basenavi']//span[@class='nnext']//a/@href"));
            data.UrlNextPage = null;

            Predicate<string> filter = __forumFilter;
            List<Ebookdz_Forum_v2> forums = new List<Ebookdz_Forum_v2>();
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

                Ebookdz_Forum_v2 forum = new Ebookdz_Forum_v2();
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

        private static HttpRequest GetHttpRequestPage(int page)
        {
            // no pagination
            if (page != 1)
                throw new PBException("error wrong page number {0}", page);
            return new HttpRequest { Url = __urlForum };
        }
    }

    //public class Ebookdz_SubForumManager : WebDataPageManager<int, Ebookdz_ForumPage, Ebookdz_Forum>
    public static class Ebookdz_SubForumManager_v2
    {
        private static WebDataPageManager<IHeaderData> __headerWebDataPageManager = null;

        static Ebookdz_SubForumManager_v2()
        {
            Ebookdz_v2.FakeInit();
        }

        public static void Init(XElement xe)
        {
            //__currentSubForumManager = CreateSubForumManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/Forum"));
            __headerWebDataPageManager = CreateSubForumManager(xe.zXPathElement("Forum"));
        }

        public static WebDataPageManager<IHeaderData> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager<IHeaderData> CreateSubForumManager(XElement xe)
        {
            //Ebookdz_SubForumManager subForumManager = new Ebookdz_SubForumManager();

            //subForumManager.WebLoadDataManager = new WebLoadDataManager<Ebookdz_ForumPage>();
            //if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            //{
            //    UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
            //    urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
            //    subForumManager.WebLoadDataManager.UrlCache = urlCache;
            //}
            //subForumManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            //subForumManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin.GetHttpRequestParameters;
            //subForumManager.WebLoadDataManager.GetData = GetData;

            //if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            //{
            //    MongoDocumentStore<int, Ebookdz_ForumPage> documentStore = new MongoDocumentStore<int, Ebookdz_ForumPage>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
            //    documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
            //    subForumManager.DocumentStore = documentStore;
            //}

            WebDataPageManager<IHeaderData> webDataPageManager = new WebDataPageManager<IHeaderData>();


            webDataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<IHeaderData>>();
            webDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);
            webDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin_v2.InitLoadFromWeb;
            webDataPageManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin_v2.GetHttpRequestParameters;
            webDataPageManager.WebLoadDataManager.GetData = GetData;

            webDataPageManager.DocumentStore = MongoDocumentStore<IEnumDataPages<IHeaderData>>.Create(xe);
            if (webDataPageManager.DocumentStore != null)
                webDataPageManager.DocumentStore.NominalType = typeof(Ebookdz_ForumPage_v2);

            return webDataPageManager;
        }

        //private static Ebookdz_ForumPage GetData(WebResult webResult)
        private static IEnumDataPages<IHeaderData> GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Ebookdz_ForumPage_v2 data = new Ebookdz_ForumPage_v2();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;

            data.UrlNextPage = null;

            List<Ebookdz_Forum_v2> forums = new List<Ebookdz_Forum_v2>();

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
                    forums.Add(new Ebookdz_Forum_v2 { SourceUrl = url, LoadFromWebDate = webResult.LoadFromWebDate, Category = category, Name = name, Url = urlSubForum });
                }
            }

            data.Forums = forums.ToArray();
            return data;
        }
    }
}
