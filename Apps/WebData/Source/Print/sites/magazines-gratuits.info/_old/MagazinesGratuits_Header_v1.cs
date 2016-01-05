using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace Download.Print.MagazinesGratuits.old
{
    public class MagazinesGratuits_PostHeader_v1 : IHeaderData
    {
        public string SourceUrl;
        public DateTime? LoadFromWebDate;
        public string Title;
        public string UrlDetail;
        public string Category;

        public WebImage[] Images;

        public HttpRequest GetHttpRequestDetail()
        {
            return new HttpRequest { Url = UrlDetail };
        }
    }

    public class MagazinesGratuits_HeaderPage_v1 : IEnumDataPages<MagazinesGratuits_PostHeader_v1>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public MagazinesGratuits_PostHeader_v1[] PostHeaders;
        public string UrlNextPage;

        public IEnumerable<MagazinesGratuits_PostHeader_v1> GetDataList()
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

    public static class MagazinesGratuits_HeaderManager_v1
    {
        private static string __urlMainPage = "http://www.magazines-gratuits.org/";
        private static WebDataPageManager<int, MagazinesGratuits_HeaderPage_v1, MagazinesGratuits_PostHeader_v1> __headerWebDataPageManager = null;

        static MagazinesGratuits_HeaderManager_v1()
        {
            __headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("MagazinesGratuits/Header"));
        }

        public static WebDataPageManager<int, MagazinesGratuits_HeaderPage_v1, MagazinesGratuits_PostHeader_v1> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager<int, MagazinesGratuits_HeaderPage_v1, MagazinesGratuits_PostHeader_v1> CreateWebDataPageManager(XElement xe)
        {
            WebDataPageManager<int, MagazinesGratuits_HeaderPage_v1, MagazinesGratuits_PostHeader_v1> headerWebDataPageManager = new WebDataPageManager<int, MagazinesGratuits_HeaderPage_v1, MagazinesGratuits_PostHeader_v1>();

            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<MagazinesGratuits_HeaderPage_v1>();
            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            //headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = MagazinesGratuits.GetHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = GetData;
            //detailWebDataManager.GetKeyFromHttpRequest = GetPostDetailKey;
            //detailWebDataManager.LoadImages = DownloadPrint.LoadImages; // IPost

            headerWebDataPageManager.DocumentStore = MongoDocumentStore<int, MagazinesGratuits_HeaderPage_v1>.Create(xe);

            headerWebDataPageManager.GetHttpRequestPage = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        private static MagazinesGratuits_HeaderPage_v1 GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            MagazinesGratuits_HeaderPage_v1 data = new MagazinesGratuits_HeaderPage_v1();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            data.UrlNextPage = zurl.GetUrl(url, xeSource.XPathValue("//div[@id='wp_page_numbers']//li[last()]/@href"));

            IEnumerable<XXElement> xeHeaders = xeSource.XPathElements("//div[@id='content']/div");
            List<MagazinesGratuits_PostHeader_v1> headers = new List<MagazinesGratuits_PostHeader_v1>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                MagazinesGratuits_PostHeader_v1 header = new MagazinesGratuits_PostHeader_v1();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                XXElement xe = xeHeader.XPathElement(".//center/strong/a");
                header.Title = xe.XPathValue(".//text()");
                header.UrlDetail = xe.XPathValue("./@href");
                header.Category = xeHeader.XPathValue(".//div[@class='cover_infos_genre']/a//text()");

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
            // page 1 : http://www.magazines-gratuits.org/
            // page 2 : http://www.magazines-gratuits.org/page/2
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
            // page 1 : http://www.magazines-gratuits.org/
            // page 2 : http://www.magazines-gratuits.org/page/2
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            string url = __urlMainPage;
            if (page > 1)
                url += string.Format("page/{0}/", page);
            return new HttpRequest { Url = url };
        }
    }
}
