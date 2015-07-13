using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace Download.Print.Ebookdz
{
    public class Ebookdz_ForumHeaderManager : WebDataPageManager<int, Ebookdz_HeaderPage, Ebookdz_PostHeader>
    {
        private static Ebookdz_ForumHeaderManager __currentForumHeaderManager = null;

        static Ebookdz_ForumHeaderManager()
        {
            __currentForumHeaderManager = CreateForumHeaderManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/ForumHeader"));
        }

        public static Ebookdz_ForumHeaderManager CurrentForumHeaderManager { get { return __currentForumHeaderManager; } }

        private static Ebookdz_ForumHeaderManager CreateForumHeaderManager(XElement xe)
        {
            //WebDataPageManager<int, Ebookdz_ForumPage, Ebookdz_Forum> forumWebDataPageManager = new WebDataPageManager<int, Ebookdz_ForumPage, Ebookdz_Forum>();
            Ebookdz_ForumHeaderManager forumHeaderManager = new Ebookdz_ForumHeaderManager();

            forumHeaderManager.WebLoadDataManager = new WebLoadDataManager<Ebookdz_HeaderPage>();
            //if (xe.zXPathValueBool("UseUrlCache", false))
            if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            {
                UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
                urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
                //urlCache.GetUrlSubDirectoryFunction = httpRequest => (GetPostDetailKey(httpRequest) / 1000 * 1000).ToString();
                forumHeaderManager.WebLoadDataManager.UrlCache = urlCache;
            }
            forumHeaderManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            forumHeaderManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin.GetHttpRequestParameters;
            forumHeaderManager.WebLoadDataManager.GetData = GetData;
            //detailWebDataManager.GetKeyFromHttpRequest = GetPostDetailKey;
            //detailWebDataManager.LoadImages = DownloadPrint.LoadImages; // IPost

            //if (xe.zXPathValueBool("UseMongo", false))
            if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            {
                MongoDocumentStore<int, Ebookdz_HeaderPage> documentStore = new MongoDocumentStore<int, Ebookdz_HeaderPage>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
                //documentStore.DefaultSort = "{ 'download.id': 1 }";
                documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
                //documentStore.GetDataKey = headerPage => headerPage.GetKey();
                //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Ebookdz_HeaderPage>(document);
                forumHeaderManager.DocumentStore = documentStore;
            }
            //subForumManager.GetHttpRequestPage = _GetHttpRequestPage;
            return forumHeaderManager;
        }

        private static Ebookdz_HeaderPage GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Ebookdz_HeaderPage data = new Ebookdz_HeaderPage();
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
                header.UrlDetail = Ebookdz.GetUrl(zurl.GetUrl(url, xe.XPathValue("@href")));

                //header.images = xeHeader.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

                //XXElement xe = xeHeader.XPathElement(".//*[@class='shd']//a");
                //header.urlDetail = zurl.GetUrl(url, xe.XPathValue("@href"));
                //header.title = RapideDdl.ExtractTextValues(header.infos, xe.XPathValue(".//text()", RapideDdl.TrimFunc1));

                //xe = xeHeader.XPathElement(".//div[@class='shdinfo']");
                //header.postAuthor = xe.XPathValue(".//span[@class='arg']//a//text()");
                //// Aujourd'hui, 17:13
                //header.creationDate = RapideDdl.ParseDateTime(xe.XPathValue(".//span[@class='date']//text()"), loadDataFromWeb.loadFromWebDate);

                //xe = xeHeader.XPathElement(".//div[@class='maincont']");
                //header.images = xe.XPathImages(xeImg => new UrlImage(zurl.GetUrl(url, xeImg.zAttribValue("src")))).ToList();

                //RapideDdl.SetTextValues(header, xe.DescendantTextList());

                //xe = xeHeader.XPathElement(".//div[@class='morelink']//span[@class='arg']");
                //header.category = xe.DescendantTextList(".//a").Select(RapideDdl.TrimFunc1).Where(s => !s.StartsWith("Commentaires")).zToStringValues("/");

                headers.Add(header);
            }

            data.PostHeaders = headers.ToArray();

            //Trace.WriteLine(data.zToJson());

            return data;
        }
    }
}
