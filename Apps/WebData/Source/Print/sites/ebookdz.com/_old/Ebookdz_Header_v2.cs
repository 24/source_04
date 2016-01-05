using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Download.Print.Ebookdz.old
{
    public class Ebookdz_PostHeader : IHeaderData
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

    public class Ebookdz_HeaderPage_v2 : IEnumDataPages<IHeaderData>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public Ebookdz_PostHeader[] PostHeaders;
        public string UrlNextPage;

        public IEnumerable<IHeaderData> GetDataList()
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

    public static class Ebookdz_HeaderManager_v2
    {
        //private static WebDataPageManager<int, Ebookdz_HeaderPage, Ebookdz_PostHeader> __headerWebDataPageManager = null;
        private static WebDataPageManager<IHeaderData> __headerWebDataPageManager = null;

        static Ebookdz_HeaderManager_v2()
        {
            Ebookdz_v2.FakeInit();
        }

        public static void Init(XElement xe)
        {
            //__headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("Ebookdz/Header"));
            __headerWebDataPageManager = CreateWebDataPageManager(xe.zXPathElement("Header"));
        }

        public static WebDataPageManager<IHeaderData> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager<IHeaderData> CreateWebDataPageManager(XElement xe)
        {
            //WebDataPageManager<int, Ebookdz_HeaderPage, Ebookdz_PostHeader> headerWebDataPageManager = new WebDataPageManager<int, Ebookdz_HeaderPage, Ebookdz_PostHeader>();
            WebDataPageManager<IHeaderData> headerWebDataPageManager = new WebDataPageManager<IHeaderData>();

            //headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<Ebookdz_HeaderPage>();
            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<IEnumDataPages<IHeaderData>>();
            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin_v2.InitLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = EbookdzLogin_v2.GetHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = GetData;

            //headerWebDataPageManager.DocumentStore = MongoDocumentStore<int, Ebookdz_HeaderPage>.Create(xe);
            headerWebDataPageManager.DocumentStore = MongoDocumentStore<IEnumDataPages<IHeaderData>>.Create(xe);
            if (headerWebDataPageManager.DocumentStore != null)
                headerWebDataPageManager.DocumentStore.NominalType = typeof(Ebookdz_HeaderPage_v2);

            headerWebDataPageManager.GetHttpRequestPageFunction = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        //private static Ebookdz_HeaderPage GetData(WebResult webResult)
        private static IEnumDataPages<IHeaderData> GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            Ebookdz_HeaderPage_v2 data = new Ebookdz_HeaderPage_v2();
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
