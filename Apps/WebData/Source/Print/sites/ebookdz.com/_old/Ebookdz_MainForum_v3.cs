using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using Download.Print.old;
using pb.Data.Mongo.old;

namespace Download.Print.Ebookdz
{
    public class Ebookdz_MainForum_v3 : WebDataPageMongoManagerBase_v1
    {
        private static string __configName = "Ebookdz";
        private static Ebookdz_MainForum_v3 __current = null;
        private static string __urlMainForum = "http://www.ebookdz.com/forum/forum.php";
        //private static WebDataPageManager_v2<IHeaderData> __mainForumDataPageManager = null;
        private static Predicate<string> __forumFilter = name => { name = name.ToLowerInvariant(); return name == "journaux" || name == "magazines" || name == "les livres"; };

        public static Ebookdz_MainForum_v3 Current { get { return __current; } }

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
            __current = new Ebookdz_MainForum_v3();
            __current.DataPageNominalType = typeof(PostHeaderDataPage_v1);
            __current.CreateDataPageManager(xe.zXPathElement("Forum"));
        }

        protected override void InitLoadFromWeb()
        {
            EbookdzLogin.InitLoadFromWeb();
        }

        protected override HttpRequestParameters GetHttpRequestParameters()
        {
            return EbookdzLogin.GetHttpRequestParameters();
        }

        protected override IEnumDataPages<IHeaderData> GetDataPage(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            PostHeaderDataPage_v1 data = new PostHeaderDataPage_v1();
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
