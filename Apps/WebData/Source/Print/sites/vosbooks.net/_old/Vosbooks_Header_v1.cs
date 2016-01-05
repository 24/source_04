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

namespace Download.Print.Vosbooks.old
{
    public class Vosbooks_PostHeader_v1 : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Title;
        public string UrlDetail;

        public WebImage[] Images;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class Vosbooks_HeaderPage_v1 : IEnumDataPages<Vosbooks_PostHeader_v1>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public Vosbooks_PostHeader_v1[] PostHeaders;
        public string UrlNextPage;

        public IEnumerable<Vosbooks_PostHeader_v1> GetDataList()
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

    public static class Vosbooks_HeaderManager_v1
    {
        private static string __urlMainPage = "http://www.vosbooks.net/";
        private static WebDataPageManager_v1<int, Vosbooks_HeaderPage_v1, Vosbooks_PostHeader_v1> __headerWebDataPageManager = null;

        static Vosbooks_HeaderManager_v1()
        {
            __headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("Vosbooks/Header"));
        }

        public static WebDataPageManager_v1<int, Vosbooks_HeaderPage_v1, Vosbooks_PostHeader_v1> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager_v1<int, Vosbooks_HeaderPage_v1, Vosbooks_PostHeader_v1> CreateWebDataPageManager(XElement xe)
        {
            WebDataPageManager_v1<int, Vosbooks_HeaderPage_v1, Vosbooks_PostHeader_v1> headerWebDataPageManager = new WebDataPageManager_v1<int, Vosbooks_HeaderPage_v1, Vosbooks_PostHeader_v1>();

            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<Vosbooks_HeaderPage_v1>();

            //if (xe.zXPathValue("UseUrlCache").zTryParseAs(false))
            //{
            //    UrlCache urlCache = new UrlCache(xe.zXPathValue("CacheDirectory"));
            //    urlCache.UrlFileNameType = zurl.GetUrlFileNameType(xe.zXPathValue("CacheUrlFileNameType"));
            //    headerWebDataPageManager.WebLoadDataManager.UrlCache = urlCache;
            //}
            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            //headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = Vosbooks_v1.GetHttpRequestParameters;
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
            headerWebDataPageManager.DocumentStore = MongoDocumentStore_v4<int, Vosbooks_HeaderPage_v1>.Create(xe);

            headerWebDataPageManager.GetHttpRequestPage = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        private static Vosbooks_HeaderPage_v1 GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Vosbooks_HeaderPage_v1 data = new Vosbooks_HeaderPage_v1();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            // <div id="page">
            // <div id="wrapper">
            // <table id="layout">
            // <tr>
            // <td></td>
            // <td>
            // <div id="left-col">
            // <div id="content-padding">
            // <div id="content">
            //   <div style="height:264px;" class="cover_global" data-zt="divbyzt">...</div>
            //   ...
            // </div>

            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@class='page-nav']//li[last()]//a[text()='>']/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//table[@id='layout']//div[@id='content']/div");
            List<Vosbooks_PostHeader_v1> headers = new List<Vosbooks_PostHeader_v1>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                Vosbooks_PostHeader_v1 header = new Vosbooks_PostHeader_v1();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                if (xeHeader.XPathValue("@class") == "page-nav")
                    continue;

                // <div style="" data-zt="divbyzt">
                // <div data-zt="divbyzt">
                // <div data-zt="divbyzt">
                // <center>
                // <strong>
                // <a href="http://www.vosbooks.net/74231-journaux/pack-journaux-francais-du-28-janvier-2015.html" title="">
                // Pack Journaux Français Du 28 Janvier 2015
                // <span class="detail_release" data-zt="spanbyzt"></span>
                // </a>
                // </strong>
                // </center>
                // </div>
                // </div>
                // </div>

                XXElement xe = xeHeader.XPathElement(".//div/div/div//a");
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
            // page 1 : http://www.vosbooks.net/
            // page 2 : http://www.vosbooks.net/page/2
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
            // page 1 : http://www.vosbooks.net/
            // page 2 : http://www.vosbooks.net/page/2
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            string url = __urlMainPage;
            if (page > 1)
                url += string.Format("page/{0}/", page);
            return new HttpRequest { Url = url };
        }
    }
}
