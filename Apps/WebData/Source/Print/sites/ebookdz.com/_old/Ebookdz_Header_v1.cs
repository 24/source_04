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
    public class Ebookdz_HeaderPage_v1 : IEnumDataPages<Ebookdz_PostHeader>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public Ebookdz_PostHeader[] PostHeaders;
        public string UrlNextPage;

        //public int GetKey()
        //{
        //    return Id;
        //}

        public IEnumerable<Ebookdz_PostHeader> GetDataList()
        {
            return PostHeaders;
        }

        public HttpRequest GetHttpRequestNextPage()
        {
            if (UrlNextPage != null)
                return new HttpRequest { Url = UrlNextPage };
            else
                return null;
        }
    }

    public static class Ebookdz_HeaderManager_v1
    {
        private static WebDataPageManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader> __headerWebDataPageManager = null;

        static Ebookdz_HeaderManager_v1()
        {
            __headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/Header"));
        }

        public static WebDataPageManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader> CreateWebDataPageManager(XElement xe)
        {
            WebDataPageManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader> headerWebDataPageManager = new WebDataPageManager_v1<int, Ebookdz_HeaderPage_v1, Ebookdz_PostHeader>();

            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<Ebookdz_HeaderPage_v1>();

            //if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            //{
            //    UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
            //    urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
            //    headerWebDataPageManager.WebLoadDataManager.UrlCache = urlCache;
            //}
            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin_v1.InitLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin_v1.GetHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = GetData;
            //detailWebDataManager.GetKeyFromHttpRequest = GetPostDetailKey;
            //detailWebDataManager.LoadImages = DownloadPrint.LoadImages; // IPost

            //if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            //{
            //    MongoDocumentStore<int, Ebookdz_HeaderPage> documentStore = new MongoDocumentStore<int, Ebookdz_HeaderPage>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
            //    documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
            //    headerWebDataPageManager.DocumentStore = documentStore;
            //}
            //documentStore.GetDataKey = headerPage => headerPage.GetKey();
            //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Ebookdz_HeaderPage>(document);
            headerWebDataPageManager.DocumentStore = MongoDocumentStore_v4<int, Ebookdz_HeaderPage_v1>.Create(xe);

            headerWebDataPageManager.GetHttpRequestPage = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        private static Ebookdz_HeaderPage_v1 GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Ebookdz_HeaderPage_v1 data = new Ebookdz_HeaderPage_v1();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            //data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='basenavi']//span[@class='nnext']//a/@href"));
            data.UrlNextPage = null;

            // <div id="vba_news4">
            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='vba_news4']//div[@class='collapse']");
            List<Ebookdz_PostHeader> headers = new List<Ebookdz_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Ebookdz_PostHeader header = new Ebookdz_PostHeader();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                //XXElement xe = xeHeader.XPathElement(".//h2[@class='blockhead']//a[@class!='mcbadge mcbadge_r']");
                XXElement xe = xeHeader.XPathElement(".//h2[@class='blockhead']//a[2]");
                header.Title = xe.XPathValue(".//text()");
                header.UrlDetail = xe.XPathValue("./@href");

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
            return data;
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.ebookdz.com/
            // page 2 : no pagination
            if (httpRequest.Url == Ebookdz_v1.UrlMainPage)
                return 1;
            //Uri uri = new Uri(url);
            //string lastSegment = uri.Segments[uri.Segments.Length - 1];
            //lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
            //int page;
            //if (!int.TryParse(lastSegment, out page))
            //    throw new PBException("header page key not found in url \"{0}\"", url);
            //return page;
            throw new PBException("header page key not found in url \"{0}\"", httpRequest.Url);
        }

        private static HttpRequest GetHttpRequestPage(int page)
        {
            // no pagination
            if (page != 1)
                throw new PBException("error wrong page number {0}", page);
            return new HttpRequest { Url = Ebookdz_v1.UrlMainPage };
        }
    }
}
