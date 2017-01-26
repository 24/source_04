using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using MongoDB.Bson;
using pb.Web.Data.Mongo;

namespace Download.Print.Ebookdz
{
    public class EbookdzForumData : PostHeader
    {
        public string Forum;
        public string Category;
        public string Name;
    }

    public static class EbookdzForum
    {
        public static IEnumerable<EbookdzForumData> LoadSubForums(bool reload = false, Predicate<EbookdzForumData> filter = null)
        {
            //foreach (IHeaderData header in __headerWebDataPageManager.LoadPages(startPage: 1, maxPage: 1, reload: reload, loadImage: false, refreshDocumentStore: false))
            // loadImage: false
            foreach (EbookdzForumData mainForum in Ebookdz_MainForum.Current.LoadPages(startPage: 1, maxPage: 1, reload: reload, refreshDocumentStore: false))
            {
                foreach (EbookdzForumData subForum in Ebookdz_SubForum.Current.LoadPages(new HttpRequest { Url = mainForum.UrlDetail }, maxPage: 0, reload: reload))
                {
                    subForum.Forum = mainForum.Name;
                    if (filter == null || filter(subForum))
                        yield return subForum;
                }
            }
        }

        public static IEnumerable<PostHeader> LoadForumHeaders(bool reload = false, Predicate<EbookdzForumData> filter = null, int limit = 0)
        {
            int nb = 0;
            foreach(EbookdzForumData forum in LoadSubForums(reload, filter))
            {
                foreach (PostHeader forumHeader in Ebookdz_ForumHeader.Current.LoadPages(new HttpRequest { Url = forum.UrlDetail }, maxPage: 0, reload: reload))
                {
                    yield return forumHeader;
                    if (limit != 0 && ++nb == limit)
                        yield break;
                }
            }
        }

        public static LoadNewDocumentsResult LoadNewDocumentsFromForum(bool reloadForum = false, bool reloadForumHeader = false, bool reloadDetail = false, int maxNbDocumentsLoadedFromStore = 5, int maxPage = 20, bool loadImage = true, Predicate<EbookdzForumData> filter = null)
        {
            LoadNewDocumentsResult result = new LoadNewDocumentsResult();
            foreach (EbookdzForumData forum in LoadSubForums(reloadForum, filter))
            {
                int nbDocumentLoadedFromStore = 0;
                int nbDocumentLoadedFromWeb = 0;
                foreach (PostHeader forumHeader in Ebookdz_ForumHeader.Current.LoadPages(new HttpRequest { Url = forum.UrlDetail }, maxPage: maxPage, reload: reloadForumHeader))
                {
                    WebData<Ebookdz_PostDetail> webData = Ebookdz.Current.DetailDataManager.Load(new WebRequest { HttpRequest = new HttpRequest { Url = forumHeader.UrlDetail },  ReloadFromWeb = reloadDetail });
                    if (webData.DataLoadedFromStore)
                        nbDocumentLoadedFromStore++;
                    if (webData.DataLoadedFromWeb)
                    {
                        nbDocumentLoadedFromWeb++;
                        if (nbDocumentLoadedFromWeb == maxNbDocumentsLoadedFromStore)
                            break;
                    }
                }
                result.NbDocumentsLoadedFromStore += nbDocumentLoadedFromStore;
                result.NbDocumentsLoadedFromWeb += nbDocumentLoadedFromWeb;
            }
            return result;
        }
    }

    public class Ebookdz_MainForum : WebDataPageMongoManagerBase<EbookdzForumData>
    {
        private static string __configName = "Ebookdz";
        private static Ebookdz_MainForum __current = null;
        private static string __urlMainForum = "http://www.ebookdz.com/forum/forum.php";
        //private static WebDataPageManager_v2<IHeaderData> __mainForumDataPageManager = null;
        private static Predicate<string> __forumFilter = name => { name = name.ToLowerInvariant(); return name == "journaux" || name == "magazines" || name == "les livres"; };

        public static Ebookdz_MainForum Current { get { return __current; } }

        //static Ebookdz_MainForum()
        //{
        //    Ebookdz.FakeInit();
        //    Init(test: DownloadPrint.Test);
        //}

        public static void Init(bool test = false)
        {
            XElement xe;
            if (!test)
                xe = XmlConfig.CurrentConfig.GetElement(__configName);
            else
            {
                pb.Trace.WriteLine("{0} init for test", __configName);
                xe = XmlConfig.CurrentConfig.GetElement(__configName + "_Test");
            }
            EbookdzLogin.Init(xe);
            __current = new Ebookdz_MainForum();
            __current.DataPageNominalType = typeof(PostHeaderDataPage<EbookdzForumData>);
            __current.Create(xe.zXPathElement("Forum"));
        }

        protected override void InitLoadFromWeb()
        {
            EbookdzLogin.InitLoadFromWeb();
        }

        protected override HttpRequestParameters GetHttpRequestParameters()
        {
            return EbookdzLogin.GetHttpRequestParameters();
        }

        protected override IEnumDataPages<EbookdzForumData> GetDataPage(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            PostHeaderDataPage<EbookdzForumData> data = new PostHeaderDataPage<EbookdzForumData>();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            data.UrlNextPage = null;

            Predicate<string> filter = __forumFilter;
            List<EbookdzForumData> forums = new List<EbookdzForumData>();
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

                EbookdzForumData forum = new EbookdzForumData();
                forum.SourceUrl = url;
                forum.LoadFromWebDate = webResult.LoadFromWebDate;
                forum.Name = name;

                forum.UrlDetail = GetUrl(zurl.GetUrl(url, xe2.XPathValue("@href")));
                forums.Add(forum);
            }

            data.Headers = forums.ToArray();
            return data;
        }

        protected override BsonValue GetKey(HttpRequest httpRequest)
        {
            return GetPageKey(httpRequest);
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.ebookdz.com/
            // page 2 : no pagination
            if (httpRequest.Url == __urlMainForum)
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

        public static string GetUrl(string url)
        {
            if (url != null)
            {
                // remove "s" value in query
                // http://www.ebookdz.com/forum/forumdisplay.php?f=1&s=1fdf76d35a57d09aa11e75ff6f0d9985
                PBUriBuilder uriBuilder = new PBUriBuilder(url);
                uriBuilder.RemoveQueryValue("s");
                url = uriBuilder.ToString();
            }
            return url;
        }

        protected override HttpRequest GetHttpRequestPage(int page)
        {
            // no pagination
            if (page != 1)
                throw new PBException("error wrong page number {0}", page);
            return new HttpRequest { Url = __urlMainForum };
        }
    }
}
