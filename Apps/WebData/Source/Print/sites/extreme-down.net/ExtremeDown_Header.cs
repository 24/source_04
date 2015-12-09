using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace Download.Print.ExtremeDown
{
    public class ExtremeDown_PostHeader : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        //public string Title;
        public string UrlDetail;

        public WebImage[] Images;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class ExtremeDown_HeaderPage : IEnumDataPages<ExtremeDown_PostHeader>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public ExtremeDown_PostHeader[] PostHeaders;
        public string UrlNextPage;

        public IEnumerable<ExtremeDown_PostHeader> GetDataList()
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

    public static class ExtremeDown_HeaderManager
    {
        //private static string __urlMainPage = "http://www.extreme-down.net/ebooks/";
        private static string __urlMainPage = "http://www.ex-down.com/ebooks/";
        private static WebDataPageManager<int, ExtremeDown_HeaderPage, ExtremeDown_PostHeader> __headerWebDataPageManager = null;

        static ExtremeDown_HeaderManager()
        {
            __headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("ExtremeDown/Header"));
        }

        public static WebDataPageManager<int, ExtremeDown_HeaderPage, ExtremeDown_PostHeader> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager<int, ExtremeDown_HeaderPage, ExtremeDown_PostHeader> CreateWebDataPageManager(XElement xe)
        {
            WebDataPageManager<int, ExtremeDown_HeaderPage, ExtremeDown_PostHeader> headerWebDataPageManager = new WebDataPageManager<int, ExtremeDown_HeaderPage, ExtremeDown_PostHeader>();

            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<ExtremeDown_HeaderPage>();

            //if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            //{
            //    UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
            //    urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
            //    headerWebDataPageManager.WebLoadDataManager.UrlCache = urlCache;
            //}
            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            //headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = ExtremeDown.GetHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = GetData;
            //detailWebDataManager.GetKeyFromHttpRequest = GetPostDetailKey;
            //detailWebDataManager.LoadImages = DownloadPrint.LoadImages; // IPost

            //if (xe.zXPathValue("UseMongo").zTryParseAs(false))
            //{
            //    MongoDocumentStore<int, Vosbooks_HeaderPage> documentStore = new MongoDocumentStore<int, Vosbooks_HeaderPage>(xe.zXPathValue("MongoServer"), xe.zXPathValue("MongoDatabase"), xe.zXPathValue("MongoCollection"), xe.zXPathValue("MongoDocumentItemName"));
            //    documentStore.DefaultSort = xe.zXPathValue("MongoDefaultSort");
            //    headerWebDataPageManager.DocumentStore = documentStore;
            //}
            //documentStore.GetDataKey = headerPage => headerPage.GetKey();
            //documentStore.Deserialize = document => (IEnumDataPages_new<int, IHeaderData_new>)BsonSerializer.Deserialize<Ebookdz_HeaderPage>(document);
            headerWebDataPageManager.DocumentStore = MongoDocumentStore<int, ExtremeDown_HeaderPage>.Create(xe);

            headerWebDataPageManager.GetHttpRequestPage = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        private static ExtremeDown_HeaderPage GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            ExtremeDown_HeaderPage data = new ExtremeDown_HeaderPage();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='navigation ignore-select']//a[starts-with(text(), 'Suivant')]/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='dle-content']//div[@class='blockbox']");
            List<ExtremeDown_PostHeader> headers = new List<ExtremeDown_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                ExtremeDown_PostHeader header = new ExtremeDown_PostHeader();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                header.UrlDetail = xeHeader.XPathValue(".//h2[@class='blocktitle']//a/@href");

                headers.Add(header);
            }
            data.PostHeaders = headers.ToArray();
            return data;
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.extreme-down.net/ebooks/
            // page 2 : http://www.extreme-down.net/ebooks/page/2/
            // new
            // page 1 : http://www.ex-down.com/ebooks/
            // page 2 : http://www.ex-down.com/ebooks/page/2/
            string url = httpRequest.Url;
            if (url == __urlMainPage)
                return 1;
            Uri uri = new Uri(url);
            string lastSegment = uri.Segments[uri.Segments.Length - 1];
            if (lastSegment.EndsWith("/"))
                lastSegment = lastSegment.Substring(0, lastSegment.Length - 1);
            int page;
            if (!int.TryParse(lastSegment, out page))
                throw new PBException("header page key not found in url \"{0}\"", url);
            return page;
        }

        private static HttpRequest GetHttpRequestPage(int page)
        {
            // page 1 : http://www.extreme-down.net/ebooks/
            // page 2 : http://www.extreme-down.net/ebooks/page/2/
            // new
            // page 1 : http://www.ex-down.com/ebooks/
            // page 2 : http://www.ex-down.com/ebooks/page/2/
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            string url = __urlMainPage;
            if (page > 1)
                url += string.Format("page/{0}/", page);
            return new HttpRequest { Url = url };
        }
    }
}
