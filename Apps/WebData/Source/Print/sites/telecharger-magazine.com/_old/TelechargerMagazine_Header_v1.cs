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

namespace Download.Print.TelechargerMagazine.old
{
    public class TelechargerMagazine_PostHeader_v1 : IHeaderData
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

    public class TelechargerMagazine_HeaderPage_v1 : IEnumDataPages<TelechargerMagazine_PostHeader_v1>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public TelechargerMagazine_PostHeader_v1[] PostHeaders;
        public string UrlNextPage;

        public IEnumerable<TelechargerMagazine_PostHeader_v1> GetDataList()
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

    public static class TelechargerMagazine_HeaderManager_v1
    {
        private static string __urlMainPage = "http://www.telecharger-magazine.com/index.php";
        private static string __urlPage = "http://www.telecharger-magazine.com/";
        private static WebDataPageManager_v1<int, TelechargerMagazine_HeaderPage_v1, TelechargerMagazine_PostHeader_v1> __headerWebDataPageManager = null;

        static TelechargerMagazine_HeaderManager_v1()
        {
            __headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("TelechargerMagazine/Header"));
        }

        public static WebDataPageManager_v1<int, TelechargerMagazine_HeaderPage_v1, TelechargerMagazine_PostHeader_v1> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager_v1<int, TelechargerMagazine_HeaderPage_v1, TelechargerMagazine_PostHeader_v1> CreateWebDataPageManager(XElement xe)
        {
            WebDataPageManager_v1<int, TelechargerMagazine_HeaderPage_v1, TelechargerMagazine_PostHeader_v1> headerWebDataPageManager = new WebDataPageManager_v1<int, TelechargerMagazine_HeaderPage_v1, TelechargerMagazine_PostHeader_v1>();

            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<TelechargerMagazine_HeaderPage_v1>();
            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            //headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = TelechargerMagazine_v1.GetHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = GetData;
            headerWebDataPageManager.DocumentStore = MongoDocumentStore_v4<int, TelechargerMagazine_HeaderPage_v1>.Create(xe);
            headerWebDataPageManager.GetHttpRequestPage = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        private static TelechargerMagazine_HeaderPage_v1 GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            TelechargerMagazine_HeaderPage_v1 data = new TelechargerMagazine_HeaderPage_v1();
            data.SourceUrl = url;
            data.LoadFromWebDate = webResult.LoadFromWebDate;
            data.Id = GetPageKey(webResult.WebRequest.HttpRequest);

            // <div id='dle-content'>
            // <div class="leftpane"> 
            // <div class="movieposter" title="Watch Movie Pachamama : Cuisine des premières nations">
            // <a href="http://www.telecharger-magazine.com/livres/3833-pachamama-cuisine-des-premires-nations.html">
            // <a href="http://www.telecharger-magazine.com/livres/3833-pachamama-cuisine-des-premires-nations.html" title="Pachamama : Cuisine des premières nations">
            // <img src="http://pxhst.co/avaxhome/cd/2a/00152acd.jpeg" width="110" height="150" alt="télécharger Pachamama : Cuisine des premières nations" title="télécharger Pachamama : Cuisine des premières nations" />
            // </a>
            // </div>
            // </div>
            // ...
            // <div class="navigation" align="center">
            // <div class="clear"></div>
            // <span>&#8592; Previous</span> <span>1</span>
            // <a href="http://www.telecharger-magazine.com/page/2/">2</a>
            // ...
            // <a href="http://www.telecharger-magazine.com/page/2/">Next &#8594;</a>
            // <div class="clear"></div>
            // </div>

            XXElement xe = xeSource.XPathElement("//div[@id='dle-content']");

            data.UrlNextPage = zurl.GetUrl(url, xe.XPathValue(".//a[starts-with(text(), 'Next')]/@href"));

            IEnumerable<XXElement> xeHeaders = xe.XPathElements(".//div[@class='leftpane']");
            List<TelechargerMagazine_PostHeader_v1> headers = new List<TelechargerMagazine_PostHeader_v1>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                TelechargerMagazine_PostHeader_v1 header = new TelechargerMagazine_PostHeader_v1();
                header.SourceUrl = url;
                header.LoadFromWebDate = webResult.LoadFromWebDate;

                if (xeHeader.XPathValue("@class") == "page-nav")
                    continue;

                XXElement xe2 = xeHeader.XPathElement(".//a/a");
                header.Title = xe2.AttribValue("title");
                header.UrlDetail = xe2.AttribValue("href");

                headers.Add(header);
            }
            data.PostHeaders = headers.ToArray();
            return data;
        }

        private static int GetPageKey(HttpRequest httpRequest)
        {
            // page 1 : http://www.telecharger-magazine.com/index.php
            // page 2 : http://www.telecharger-magazine.com/page/2/
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
            // page 1 : http://www.telecharger-magazine.com/index.php
            // page 2 : http://www.telecharger-magazine.com/page/2/
            if (page < 1)
                throw new PBException("error wrong page number {0}", page);
            string url = __urlMainPage;
            if (page > 1)
                url = __urlPage + string.Format("page/{0}/", page);
            return new HttpRequest { Url = url };
        }
    }
}
