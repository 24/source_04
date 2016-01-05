using System.Collections.Generic;
using System.Xml.Linq;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace Download.Print.Ebookdz.old
{
    //public class Ebookdz_ForumHeaderManager : WebDataPageManager<int, Ebookdz_HeaderPage, Ebookdz_PostHeader>
    public static class Ebookdz_ForumHeaderManager_v2
    {
        private static WebDataPageManager<IHeaderData> __headerWebDataPageManager = null;

        static Ebookdz_ForumHeaderManager_v2()
        {
            Ebookdz_v2.FakeInit();
        }

        public static void Init(XElement xe)
        {
            //__currentForumHeaderManager = CreateForumHeaderManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/ForumHeader"));
            __headerWebDataPageManager = CreateForumHeaderManager(xe.zXPathElement("ForumHeader"));
        }

        public static WebDataPageManager<IHeaderData> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        //private static Ebookdz_ForumHeaderManager CreateForumHeaderManager(XElement xe)
        private static WebDataPageManager<IHeaderData> CreateForumHeaderManager(XElement xe)
        {
            //Ebookdz_ForumHeaderManager forumHeaderManager = new Ebookdz_ForumHeaderManager();

            //forumHeaderManager.WebLoadDataManager = new WebLoadDataManager<Ebookdz_HeaderPage>();
            //if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            //{
            //    UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
            //    urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
            //    forumHeaderManager.WebLoadDataManager.UrlCache = urlCache;
            //}
            //forumHeaderManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            //forumHeaderManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin.GetHttpRequestParameters;
            //forumHeaderManager.WebLoadDataManager.GetData = GetData;

            //if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            //{
            //    MongoDocumentStore<int, Ebookdz_HeaderPage> documentStore = new MongoDocumentStore<int, Ebookdz_HeaderPage>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
            //    documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
            //    forumHeaderManager.DocumentStore = documentStore;
            //}

            WebDataPageManager<IHeaderData> webDataPageManager = new WebDataPageManager<IHeaderData>();

            webDataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<IHeaderData>>();
            webDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);
            webDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin_v2.InitLoadFromWeb;
            webDataPageManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin_v2.GetHttpRequestParameters;
            webDataPageManager.WebLoadDataManager.GetData = GetData;

            webDataPageManager.DocumentStore = MongoDocumentStore<IEnumDataPages<IHeaderData>>.Create(xe);
            if (webDataPageManager.DocumentStore != null)
                webDataPageManager.DocumentStore.NominalType = typeof(Ebookdz_PostHeader);

            return webDataPageManager;
        }

        //private static Ebookdz_HeaderPage GetData(WebResult webResult)
        private static IEnumDataPages<IHeaderData> GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Ebookdz_HeaderPage_v2 data = new Ebookdz_HeaderPage_v2();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            //data.Id = _GetPageKey(webResult.WebRequest.HttpRequest);

            // <div class="threadpagenav">
            // <span class="prev_next">
            // <a rel="next" href="forumdisplay.php?f=157&amp;page=2&amp;s=fec27f3bac2b58debbb727ab8725c8a4" title="Page suivante - Résultats de 21 à 40 sur 61">
            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='threadpagenav']//span[@class='prev_next']//a[@rel='next']/@href"));

            List<Ebookdz_PostHeader> headers = new List<Ebookdz_PostHeader>();

            // <div class="body_bd">
            // <div id="threadlist" class="threadlist">
            // <ol id="threads" class="threads">
            // <li class="threadbit " id="thread_111977">
            //   <h3 class="threadtitle">
            //   <a title="" class="title" href="showthread.php?t=111977&amp;s=fec27f3bac2b58debbb727ab8725c8a4" id="thread_title_111977">La  Provence Marseille du lundi 26 janvier 2015</a>

            foreach (XXElement xeHeader in xeSource.XPathElements("//div[@id='threadlist']//ol[@id='threads']/li"))
            {
                Ebookdz_PostHeader header = new Ebookdz_PostHeader();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                XXElement xe = xeHeader.XPathElement(".//h3[@class='threadtitle']//a[@class='title']");
                header.Title = xe.XPathValue(".//text()");
                header.UrlDetail = Ebookdz_v1.GetUrl(zurl.GetUrl(url, xe.XPathValue("@href")));

                headers.Add(header);
            }

            data.PostHeaders = headers.ToArray();

            //Trace.WriteLine(data.zToJson());

            return data;
        }
    }
}
