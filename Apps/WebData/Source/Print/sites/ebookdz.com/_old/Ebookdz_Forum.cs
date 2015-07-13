using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.Web;

namespace Download.Print.Ebookdz.old
{
    public class Ebookdz_Forum
    {
        public string Forum;
        public string Category;
        public string Name;
        public string Url;
    }

    public class Ebookdz_LoadForumFromWebManager : LoadFromWebManager_v4
    {
        private static string __urlForum = "http://www.ebookdz.com/forum/forum.php";
        private static Ebookdz_LoadForumFromWebManager __currentLoadForumFromWebManager = null;

        static Ebookdz_LoadForumFromWebManager()
        {
            ClassInit(XmlConfig.CurrentConfig.GetElement("Ebookdz/Forum"));
        }

        private static void ClassInit(XElement xe)
        {
            bool useUrlCache = xe.zXPathValue("UseUrlCache").zTryParseAs(false);
            UrlCache urlCache = null;
            if (useUrlCache)
            {
                string cacheDirectory = xe.zXPathValue("CacheDirectory");
                urlCache = new UrlCache(cacheDirectory);
                urlCache.UrlFileNameType = UrlFileNameType.Path | UrlFileNameType.Query;
                //urlCache.GetUrlSubDirectoryFunction = httpRequest => (Ebookdz_LoadPostDetailFromWebManager.GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
            }
            __currentLoadForumFromWebManager = new Ebookdz_LoadForumFromWebManager(urlCache);
        }

        public Ebookdz_LoadForumFromWebManager(UrlCache urlCache = null)
        {
            _urlCache = urlCache;
        }

        public static Ebookdz_LoadForumFromWebManager CurrentLoadForumFromWebManager { get { return __currentLoadForumFromWebManager; } }

        protected override void InitLoadFromWeb()
        {
            Ebookdz.InitLoadFromWeb();
        }

        protected override HttpRequestParameters GetHttpRequestParameters()
        {
            return new HttpRequestParameters();
        }

        private static Predicate<string> __forumFilter = name => { name = name.ToLowerInvariant(); return name == "journaux" || name == "magazines" || name == "les livres"; };

        public IEnumerable<Ebookdz_Forum> LoadForums(bool reload = false)
        {
            foreach (Ebookdz_Forum forum in LoadMainForum(filter: __forumFilter, reload: reload))
            {
                foreach (Ebookdz_Forum subForum in LoadSubForum(forum.Url, forum.Forum, reload: reload))
                {
                    yield return subForum;
                }
            }
        }

        public IEnumerable<Ebookdz_Forum> LoadMainForum(bool reload = false)
        {
            return LoadMainForum(filter: __forumFilter, reload: reload);
        }

        public IEnumerable<Ebookdz_Forum> LoadMainForum(Predicate<string> filter = null, bool reload = false)
        {
            LoadDataFromWeb_v4 loadDataFromWeb = Load(new RequestFromWeb_v4(new HttpRequest { Url = __urlForum }, reload: reload));
            if (loadDataFromWeb.LoadResult)
            {
                XXElement xeSource = new XXElement(loadDataFromWeb.Http.zGetXDocument().Root);
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

                    string url = Ebookdz.GetUrl(zurl.GetUrl(loadDataFromWeb.WebRequest.HttpRequest.Url, xe2.XPathValue("@href")));
                    //if (url != null)
                    //{
                    //    PBUriBuilder uriBuilder = new PBUriBuilder(url);
                    //    uriBuilder.RemoveQueryValue("s");
                    //    url = uriBuilder.ToString();
                    //}
                    yield return new Ebookdz_Forum { Forum = name, Url = url };
                }
            }
        }

        public IEnumerable<Ebookdz_Forum> LoadSubForum(string url, string forum, Predicate<string> filter = null, bool reload = false)
        {
            LoadDataFromWeb_v4 loadDataFromWeb = Load(new RequestFromWeb_v4(new HttpRequest { Url = url }, reload: reload));
            if (loadDataFromWeb.LoadResult)
            {
                XXElement xeSource = new XXElement(loadDataFromWeb.Http.zGetXDocument().Root);

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
                    url = Ebookdz.GetUrl(zurl.GetUrl(loadDataFromWeb.WebRequest.HttpRequest.Url, xe2.XPathValue("@href")));
                    yield return new Ebookdz_Forum { Forum = forum, Category = category, Url = url };

                    foreach (XXElement xe3 in xe.XPathElements(".//ol[@class='childsubforum']/li//div[@class='titleline']//a"))
                    {
                        string name = xe3.XPathValue(".//text()");

                        if (filter != null && !filter(name))
                            continue;
                        url = Ebookdz.GetUrl(zurl.GetUrl(loadDataFromWeb.WebRequest.HttpRequest.Url, xe3.XPathValue("@href")));
                        yield return new Ebookdz_Forum { Forum = forum, Category = category, Name = name, Url = url };
                    }
                }
            }
        }
    }
}
