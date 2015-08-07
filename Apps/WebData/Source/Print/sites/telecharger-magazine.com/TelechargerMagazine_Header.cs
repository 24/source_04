using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Mongo;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Data;

namespace Download.Print.TelechargerMagazine
{
    public class TelechargerMagazine_PostHeader : IHeaderData
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

    public class TelechargerMagazine_HeaderPage : IEnumDataPages<TelechargerMagazine_PostHeader>
    {
        public int Id;
        public string SourceUrl;
        public DateTime LoadFromWebDate;

        public TelechargerMagazine_PostHeader[] PostHeaders;
        public string UrlNextPage;

        public IEnumerable<TelechargerMagazine_PostHeader> GetDataList()
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

    public static class TelechargerMagazine_HeaderManager
    {
        private static string __urlMainPage = "http://www.telecharger-magazine.com/index.php";
        private static string __urlPage = "http://www.telecharger-magazine.com/";
        private static WebDataPageManager<int, TelechargerMagazine_HeaderPage, TelechargerMagazine_PostHeader> __headerWebDataPageManager = null;

        static TelechargerMagazine_HeaderManager()
        {
            __headerWebDataPageManager = CreateWebDataPageManager(XmlConfig.CurrentConfig.GetElement("TelechargerMagazine/Header"));
        }

        public static WebDataPageManager<int, TelechargerMagazine_HeaderPage, TelechargerMagazine_PostHeader> HeaderWebDataPageManager { get { return __headerWebDataPageManager; } }

        private static WebDataPageManager<int, TelechargerMagazine_HeaderPage, TelechargerMagazine_PostHeader> CreateWebDataPageManager(XElement xe)
        {
            WebDataPageManager<int, TelechargerMagazine_HeaderPage, TelechargerMagazine_PostHeader> headerWebDataPageManager = new WebDataPageManager<int, TelechargerMagazine_HeaderPage, TelechargerMagazine_PostHeader>();

            headerWebDataPageManager.WebLoadDataManager = new WebLoadDataManager<TelechargerMagazine_HeaderPage>();
            headerWebDataPageManager.WebLoadDataManager.UrlCache = UrlCache.Create(xe);

            //headerWebDataPageManager.WebLoadDataManager.InitLoadFromWeb = EbookdzLogin.InitLoadFromWeb;
            headerWebDataPageManager.WebLoadDataManager.GetHttpRequestParameters = TelechargerMagazine.GetHttpRequestParameters;
            headerWebDataPageManager.WebLoadDataManager.GetData = GetData;
            headerWebDataPageManager.DocumentStore = MongoDocumentStore<int, TelechargerMagazine_HeaderPage>.Create(xe);
            headerWebDataPageManager.GetHttpRequestPage = GetHttpRequestPage;
            return headerWebDataPageManager;
        }

        private static TelechargerMagazine_HeaderPage GetData(WebResult webResult)
        {
            XXElement xeSource = new XXElement(webResult.Http.zGetXDocument().Root);
            string url = webResult.WebRequest.HttpRequest.Url;
            TelechargerMagazine_HeaderPage data = new TelechargerMagazine_HeaderPage();
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
            List<TelechargerMagazine_PostHeader> headers = new List<TelechargerMagazine_PostHeader>();
            foreach (XXElement xeHeader in xeHeaders)
            {
                TelechargerMagazine_PostHeader header = new TelechargerMagazine_PostHeader();
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
